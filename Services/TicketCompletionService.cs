using Microsoft.EntityFrameworkCore;
using MiChitra.Data;
using MiChitra.Models;

namespace MiChitra.Services
{
    public class TicketCompletionService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TicketCompletionService> _logger;

        public TicketCompletionService(IServiceProvider serviceProvider, ILogger<TicketCompletionService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CompleteFinishedShowsAsync();
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        private async Task CompleteFinishedShowsAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MiChitraDbContext>();

            var completedTickets = await context.Tickets
                .Include(t => t.MovieShow)
                .Where(t => t.Status == TicketStatus.Booked && 
                           t.MovieShow.ShowTime <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var ticket in completedTickets)
            {
                ticket.Status = TicketStatus.Completed;
                ticket.UpdatedAt = DateTime.UtcNow;
            }

            if (completedTickets.Any())
            {
                await context.SaveChangesAsync();
                _logger.LogInformation("Marked {Count} tickets as completed", completedTickets.Count);
            }
        }
    }
}
