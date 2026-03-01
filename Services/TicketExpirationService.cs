using Microsoft.EntityFrameworkCore;
using MiChitra.Data;
using MiChitra.Models;

namespace MiChitra.Services
{
    public class TicketExpirationService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TicketExpirationService> _logger;

        public TicketExpirationService(IServiceProvider serviceProvider, ILogger<TicketExpirationService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await ExpireReservedTicketsAsync();
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ExpireReservedTicketsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MiChitraDbContext>();

            var expiredTickets = await context.Tickets
                .Include(t => t.MovieShow)
                .Where(t => t.Status == TicketStatus.Reserved && 
                           t.ReservationExpiry.HasValue && 
                           t.ReservationExpiry.Value <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var ticket in expiredTickets)
            {
                ticket.Status = TicketStatus.Cancelled;
                ticket.UpdatedAt = DateTime.UtcNow;
                ticket.MovieShow.AvailableSeats += ticket.NumberOfSeats;

                var occupancyPercentage = (double)(ticket.MovieShow.TotalSeats - ticket.MovieShow.AvailableSeats) / ticket.MovieShow.TotalSeats;
                ticket.MovieShow.Status = occupancyPercentage switch
                {
                    >= 1.0 => MovieShowStatus.SoldOut,
                    >= 0.8 => MovieShowStatus.AlmostFull,
                    _ => MovieShowStatus.Available
                };
            }

            if (expiredTickets.Any())
            {
                await context.SaveChangesAsync();
                _logger.LogInformation("Expired {Count} reserved tickets", expiredTickets.Count);
            }
        }
    }
}
