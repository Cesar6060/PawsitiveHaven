# Pawsitive Haven - Project Documentation

## Project Overview

Pawsitive Haven is a comprehensive pet management system with AI-powered assistance for pet owners. Built as a fresh rebuild to address architectural issues from a previous project, it features proper dependency injection, PostgreSQL from day one, and Docker-first development.

**Tech Stack:**
- Backend: ASP.NET Core 9 Web API
- Frontend: Blazor Server (no prerendering)
- Database: PostgreSQL 17
- AI: OpenAI GPT-4o-mini
- Auth: JWT with BCrypt password hashing
- Containerization: Docker & Docker Compose

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Blazor Server Frontend                   │
│                    (Port 5051 / 8080 in Docker)             │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐         │
│  │   Pages     │  │  Services   │  │   Models    │         │
│  │  (Razor)    │  │ (API calls) │  │   (DTOs)    │         │
│  └─────────────┘  └─────────────┘  └─────────────┘         │
└─────────────────────────┬───────────────────────────────────┘
                          │ HTTP/REST API
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                ASP.NET Core Backend API                     │
│                (Port 5052 / 8080 in Docker)                 │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐         │
│  │ Controllers │  │  Services   │  │Repositories │         │
│  │  (REST)     │  │  (Business) │  │   (Data)    │         │
│  └─────────────┘  └─────────────┘  └─────────────┘         │
│                          │                                   │
│                          ▼                                   │
│              ┌─────────────────────┐                        │
│              │ PostgreSQL Database │                        │
│              │  (pawsitive_haven)  │                        │
│              └─────────────────────┘                        │
└─────────────────────────────────────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                    External Services                        │
│              ┌─────────────────────┐                        │
│              │    OpenAI API       │                        │
│              │  (GPT-4o-mini)      │                        │
│              └─────────────────────┘                        │
└─────────────────────────────────────────────────────────────┘
```

---

## Project Structure

```
PawsitiveHaven/
├── docker-compose.yml          # Development PostgreSQL
├── docker-compose.prod.yml     # Production (all services)
├── .env.example                # Environment template
├── database/
│   └── init.sql               # Database schema
└── src/
    ├── PawsitiveHaven.Api/    # Backend
    │   ├── Controllers/       # REST endpoints
    │   ├── Data/
    │   │   ├── AppDbContext.cs
    │   │   └── Repositories/  # Data access
    │   ├── Models/
    │   │   ├── Entities/      # Database entities
    │   │   └── DTOs/          # Data transfer objects
    │   ├── Services/          # Business logic
    │   ├── Extensions/        # DI registration
    │   └── Dockerfile
    └── PawsitiveHaven.Web/    # Frontend
        ├── Components/
        │   ├── Layout/        # MainLayout, NavMenu
        │   ├── Pages/         # All page components
        │   │   └── Admin/     # Admin-only pages
        │   └── Shared/        # Reusable components
        ├── Services/          # API client services
        ├── Models/            # Frontend DTOs
        ├── wwwroot/           # Static assets
        └── Dockerfile
```

---

## Completed Phases

### Phase 1: Project Scaffolding + Docker + Database
- Created solution structure with API and Web projects
- Set up Docker Compose with PostgreSQL 17
- Created database schema (users, pets, appointments, faqs, conversations)
- Added health check endpoint

### Phase 2: Backend API Foundation
- Implemented JWT authentication with BCrypt (work factor 12)
- Created repository pattern for data access
- Built auth endpoints (login/register)
- Added Pet CRUD endpoints with authorization
- Configured proper dependency injection

### Phase 3: Frontend Foundation
- Set up Blazor Server with prerender disabled
- Created login/register pages
- Implemented AuthStateService for session management
- Built responsive layout (sidebar desktop, bottom nav mobile)
- Added localStorage persistence for auth tokens

### Phase 4: Core Features
- Dashboard with pet carousel and quick actions
- Pet profile management (add/edit/delete)
- Appointment/task management with filters
- Species-specific icons (dog/cat/other)

### Phase 5: AI Integration
- OpenAI GPT-4o-mini chat integration
- Conversation history with persistence
- Pet bio generator
- FAQ injection into AI context
- Chat UI with conversation sidebar

### Phase 6: Admin Features + Polish
- Admin user management (CRUD)
- Admin FAQ management (CRUD)
- Settings page with logout
- Resources page with pet care guides
- Production Docker setup with multi-stage builds

---

## API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | Login, returns JWT |

### Pets (Requires Auth)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/pets` | Get user's pets |
| GET | `/api/pets/{id}` | Get specific pet |
| POST | `/api/pets` | Create pet |
| PUT | `/api/pets/{id}` | Update pet |
| DELETE | `/api/pets/{id}` | Delete pet |

### Appointments (Requires Auth)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/appointments` | Get user's appointments |
| POST | `/api/appointments` | Create appointment |
| PUT | `/api/appointments/{id}` | Update appointment |
| DELETE | `/api/appointments/{id}` | Delete appointment |

### AI (Requires Auth)
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/ai/chat` | Send chat message |
| POST | `/api/ai/generate-bio` | Generate pet bio |
| GET | `/api/ai/conversations` | Get user's conversations |
| GET | `/api/ai/conversations/{id}` | Get conversation with messages |
| DELETE | `/api/ai/conversations/{id}` | Delete conversation |

### FAQs
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/faqs` | Get active FAQs |
| GET | `/api/faqs/all` | Get all FAQs (admin) |
| POST | `/api/faqs` | Create FAQ (admin) |
| PUT | `/api/faqs/{id}` | Update FAQ (admin) |
| DELETE | `/api/faqs/{id}` | Delete FAQ (admin) |

### Admin (Requires Admin Role)
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/admin/users` | Get all users |
| GET | `/api/admin/users/{id}` | Get specific user |
| POST | `/api/admin/users` | Create user |
| PUT | `/api/admin/users/{id}` | Update user |
| DELETE | `/api/admin/users/{id}` | Delete user |

---

## How to Run

### Development (Local)

1. **Start PostgreSQL:**
   ```bash
   docker-compose up -d
   ```

2. **Set environment variables:**
   ```bash
   cp .env.example .env
   # Edit .env with your values
   ```

3. **Run Backend:**
   ```bash
   cd src/PawsitiveHaven.Api
   dotnet run
   ```

4. **Run Frontend (new terminal):**
   ```bash
   cd src/PawsitiveHaven.Web
   dotnet run
   ```

5. **Access:**
   - Frontend: http://localhost:5051
   - API: http://localhost:5052
   - Health: http://localhost:5052/health

### Production (Docker)

```bash
# Set environment variables
export DB_PASSWORD=your_secure_password
export JWT_SECRET=your_32_char_minimum_secret
export OPENAI_API_KEY=sk-your-key

# Build and run
docker-compose -f docker-compose.prod.yml up --build -d
```

---

## Environment Variables

| Variable | Description | Required |
|----------|-------------|----------|
| `DB_USER` | PostgreSQL username | No (default: pawsitive) |
| `DB_PASSWORD` | PostgreSQL password | Yes (production) |
| `JWT_SECRET` | JWT signing key (32+ chars) | Yes |
| `OPENAI_API_KEY` | OpenAI API key | Yes |

---

## Test Accounts

After running `init.sql`, these accounts are available:

| Username | Password | Role |
|----------|----------|------|
| admin | Test12345 | Admin |
| testuser | Test12345 | User |

---

## Pull Request History

| PR | Phase | Status |
|----|-------|--------|
| #1 | Phases 1-3 | Merged |
| #2 | Phase 4: Core Features | Merged |
| #3 | Phase 5: AI Integration | Merged |
| #5 | Phase 6: Admin + Docker | Merged |

---

## What's Next

### Immediate (Before Production)

1. **Test Production Docker Build:**
   ```bash
   docker-compose -f docker-compose.prod.yml up --build
   ```
2. **Verify all features work end-to-end**
3. **Set up production environment variables**

### Short-term Enhancements

1. **Email Notifications**
   - Password reset functionality
   - Appointment reminders
   - Welcome emails for new users

2. **Pet Features**
   - Pet photo upload
   - Medical records tracking
   - Vaccination reminders

3. **Appointment Improvements**
   - Calendar view
   - Recurring appointments
   - Push notifications

4. **UI/UX Polish**
   - Dark mode support
   - Accessibility improvements (ARIA labels)
   - Loading skeletons

### Long-term Features

1. **Multi-pet Household**
   - Shared pet access between users
   - Family/household grouping

2. **Vet Integration**
   - Vet clinic directory
   - Appointment booking with vets
   - Medical record sharing

3. **Community Features**
   - Pet adoption listings
   - Lost & found pets
   - Pet owner forums

4. **Mobile App**
   - React Native or .NET MAUI app
   - Push notifications
   - Offline support

5. **Analytics Dashboard**
   - Pet health trends
   - Appointment statistics
   - AI usage metrics

### Infrastructure

1. **CI/CD Pipeline**
   - GitHub Actions for build/test
   - Automated Docker image publishing
   - Deployment to cloud (Azure/AWS/GCP)

2. **Monitoring**
   - Application Insights / Sentry
   - Health check dashboard
   - Log aggregation (ELK/Seq)

3. **Security Hardening**
   - Rate limiting on all endpoints
   - CORS configuration
   - Security headers (CSP, HSTS)

4. **Performance**
   - Redis caching
   - Response compression
   - Database query optimization

---

## Known Issues / Technical Debt

1. **Warnings to address:**
   - Unused `ex` variable in Login.razor and Register.razor
   - Unawaited async call in Chat.razor line 162

2. **Improvements needed:**
   - Add comprehensive unit tests
   - Add integration tests
   - Implement request/response logging
   - Add Swagger/OpenAPI documentation

---

## Contributing

1. Create a feature branch from `main`
2. Make changes following existing patterns
3. Ensure build passes: `dotnet build`
4. Create PR targeting `main`
5. Wait for Greptile review
6. Address any feedback
7. Merge after approval

---

## Contact

For questions or issues, create a GitHub issue in the repository.
