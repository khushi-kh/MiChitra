# MiChitra - Movie Booking Application

## Overview

MiChitra is a full-stack movie ticket booking platform that lets users browse movies, find shows by city and theatre, book seats, and manage their bookings — all in one place. The backend exposes a RESTful API built with ASP.NET Core 8, while the frontend is a React 19 SPA. An admin panel provides full control over movies, theatres, shows, bookings, and users.

---

## Key Features

- User registration and login with JWT authentication
- Password strength validation and secure BCrypt hashing
- Forgot/reset password flow
- Browse movies and search by title
- View theatres by city and check available shows
- Seat selection and ticket booking
- Mock payment processing with booking confirmation
- View and manage personal bookings
- Automatic ticket expiration for unpaid bookings
- Automatic ticket completion after show ends
- Role-based access control (User / Admin)
- Admin dashboard to manage movies, theatres, shows, bookings, and users
- Rate limiting on auth and booking endpoints
- Global exception handling middleware
- Swagger UI for API exploration

---

## Tech Stack

**Backend**
- ASP.NET Core 8 Web API
- Entity Framework Core 8 with SQL Server
- JWT Authentication
- BCrypt password hashing
- Swagger / OpenAPI
- Rate limiting (fixed window)

**Frontend**
- React 19 + Vite
- React Router v7
- Axios

---

## Project Structure

```
MiChitra/
├── Controllers/        # API controllers
├── Services/           # Business logic
├── Interfaces/         # Service contracts
├── Models/             # EF Core entities
├── DTOs/               # Request/response models
├── Data/               # DbContext + seeder
├── Middleware/         # Global exception handler
├── Attributes/         # Custom validation attributes
├── MiChitra.Tests/     # Unit tests
└── Frontend/
    └── michitra-ui/    # React frontend (Vite)
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18+)
- SQL Server or LocalDB

### Backend Setup

1. Clone the repository.

2. Update the connection string in `appsettings.json` if needed (defaults to LocalDB):
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MiChitraDB;Trusted_Connection=True;"
   }
   ```

3. Add your JWT secret key to `appsettings.Development.json`:
   ```json
   "Jwt": {
     "Key": "your-secret-key-here",
     "Issuer": "MiChitraApplication",
     "Audience": "MiChitraAudience",
     "ExpireMinutes": 60
   }
   ```

4. Apply migrations and run:
   ```bash
   dotnet ef database update
   dotnet run
   ```

   The API will be available at `http://localhost:5267`.  
   Swagger UI: `http://localhost:5267/swagger`

### Frontend Setup

```bash
cd Frontend/michitra-ui
npm install
npm run dev
```

The frontend runs at `http://localhost:5173`.

---

## API Overview

| Controller     | Base Route           | Description                        |
|----------------|----------------------|------------------------------------|
| Auth           | `/api/auth`          | Register, login                    |
| Movies         | `/api/movies`        | CRUD + search                      |
| Theatres       | `/api/theatres`      | CRUD                               |
| MovieShows     | `/api/movieshows`    | Schedule management                |
| Tickets        | `/api/tickets`       | Book tickets                       |
| Bookings       | `/api/bookings`      | View user bookings                 |
| Payment        | `/api/payment`       | Mock payment processing            |
| Users          | `/api/users`         | User management (Admin)            |

### Rate Limits

| Policy        | Limit         | Window   |
|---------------|---------------|----------|
| AuthPolicy    | 5 requests    | 1 minute |
| BookingPolicy | 10 requests   | 1 minute |
| GeneralPolicy | 100 requests  | 1 minute |

---

## Authentication

JWT Bearer tokens are used. Include the token in the `Authorization` header:

```
Authorization: Bearer <token>
```

Roles: `Admin`, `User`

Admin-only routes are protected via `[Authorize(Roles = "Admin")]` and guarded on the frontend with `ProtectedRoute`.

---

## Running Tests

```bash
cd MiChitra.Tests
dotnet test
```
