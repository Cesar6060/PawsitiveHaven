# Pawsitive Haven Pet Rescue

A comprehensive pet management system with integrated AI capabilities for pet rescue organizations.

## Tech Stack

- **.NET 9** - Latest .NET framework
- **Blazor Server** - Frontend with no prerendering for simple auth state
- **ASP.NET Core Web API** - Backend REST API
- **PostgreSQL 17** - Database
- **Entity Framework Core 9** - ORM with repository pattern
- **JWT Authentication** - Secure token-based auth
- **Bootstrap 5** - UI framework

## Project Structure

```
PawsitiveHaven/
├── docker-compose.yml          # PostgreSQL container
├── database/
│   └── init.sql               # Database schema
└── src/
    ├── PawsitiveHaven.Api/    # Backend API
    │   ├── Controllers/       # REST endpoints
    │   ├── Data/              # DbContext & Repositories
    │   ├── Models/            # Entities & DTOs
    │   ├── Services/          # Business logic
    │   └── Extensions/        # DI extensions
    └── PawsitiveHaven.Web/    # Frontend
        ├── Components/        # Blazor components
        ├── Services/          # API clients
        └── Models/            # Client models
```

## Getting Started

### Prerequisites

- .NET 9 SDK
- Docker & Docker Compose

### Quick Start

1. **Start PostgreSQL:**
   ```bash
   docker-compose up -d
   ```

2. **Run the Backend (Terminal 1):**
   ```bash
   cd src/PawsitiveHaven.Api
   dotnet run
   ```
   API runs at: http://localhost:5052

3. **Run the Frontend (Terminal 2):**
   ```bash
   cd src/PawsitiveHaven.Web
   dotnet run
   ```
   Frontend runs at: http://localhost:5180

### Default Users

| Username | Password   | Role  |
|----------|------------|-------|
| admin    | Test12345  | Admin |
| demo     | Test12345  | User  |

## API Endpoints

| Endpoint                     | Method | Auth     | Description          |
|------------------------------|--------|----------|----------------------|
| `/api/auth/login`            | POST   | No       | User login           |
| `/api/auth/register`         | POST   | No       | User registration    |
| `/api/pets`                  | GET    | Required | Get user's pets      |
| `/api/pets`                  | POST   | Required | Create pet           |
| `/api/pets/{id}`             | PUT    | Required | Update pet           |
| `/api/pets/{id}`             | DELETE | Required | Delete pet           |
| `/api/appointments`          | GET    | Required | Get appointments     |
| `/api/appointments/upcoming` | GET    | Required | Get upcoming tasks   |
| `/api/faqs`                  | GET    | No       | Get active FAQs      |
| `/health`                    | GET    | No       | Health check         |

## Environment Variables

Create a `.env` file in the project root:

```bash
DB_USER=pawsitive
DB_PASSWORD=your_secure_password
JWT_SECRET=your_32_char_minimum_secret_key
OPENAI_API_KEY=sk-your-openai-api-key
```

## Development

### Build Both Projects
```bash
dotnet build
```

### Run Tests
```bash
cd src/PawsitiveHaven.Api
dotnet test
```

## Features

- [x] User authentication (JWT)
- [x] Pet profile management
- [x] Appointment/task tracking
- [x] FAQ system
- [ ] AI chat integration
- [ ] Pet bio generator
- [ ] Admin dashboard

## Architecture Decisions

1. **No Prerendering** - Simplifies auth state management
2. **Repository Pattern** - Clean data access layer
3. **Proper DI** - All services injected, no `new` in controllers
4. **PostgreSQL from Day 1** - No migration headaches
5. **Docker-first** - Consistent development environment
