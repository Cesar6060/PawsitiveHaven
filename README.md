# Pawsitive Haven

**A full-stack pet rescue management platform with AI-powered assistance**

![.NET](https://img.shields.io/badge/.NET%209-512BD4?style=flat&logo=dotnet&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=flat&logo=blazor&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=flat&logo=postgresql&logoColor=white)
![OpenAI](https://img.shields.io/badge/OpenAI-412991?style=flat&logo=openai&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-2496ED?style=flat&logo=docker&logoColor=white)

---

## Overview

Pawsitive Haven is a comprehensive management system designed for pet rescue organizations. It streamlines the adoption process, supports foster families, and provides AI-powered assistance to answer common questions about pet care, fostering, and adoption.

Built with a security-first approach, the platform implements defense-in-depth protections for its AI chatbot, including prompt injection detection, rate limiting, and output filtering.

---

## Key Features

### For Foster Families & Adopters
- **AI Chat Assistant** - Get instant answers about pet care, fostering guidelines, and adoption processes
- **Pet Profiles** - Track pets with detailed information and AI-generated bios
- **Appointment Management** - Schedule and manage vet visits, vaccinations, and check-ups
- **Resource Library** - Access comprehensive guides for first-time fosters and adopters

### For Administrators
- **User Management** - Full CRUD operations for managing platform users
- **FAQ Management** - Maintain the knowledge base that powers the AI assistant
- **Role-Based Access** - Secure admin-only sections with JWT authentication

### Security-Hardened AI
- **Prompt Injection Detection** - 30+ pattern matching rules to prevent manipulation
- **Rate Limiting** - Tiered limits (per-minute, hourly, daily) with automatic bans
- **Input Sanitization** - Unicode normalization, control character removal
- **Output Filtering** - Prevents system prompt leakage

---

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    Blazor Server Frontend                   │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐         │
│  │   Pages     │  │  Services   │  │   Models    │         │
│  │  (Razor)    │  │ (API calls) │  │   (DTOs)    │         │
│  └─────────────┘  └─────────────┘  └─────────────┘         │
└─────────────────────────┬───────────────────────────────────┘
                          │ REST API
                          ▼
┌─────────────────────────────────────────────────────────────┐
│                ASP.NET Core 9 Web API                       │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐         │
│  │ Controllers │  │  Services   │  │Repositories │         │
│  │  (REST)     │  │  (Business) │  │   (Data)    │         │
│  └─────────────┘  └─────────────┘  └─────────────┘         │
│         │                │                │                 │
│         ▼                ▼                ▼                 │
│  ┌─────────────────────────────────────────────┐           │
│  │           Security Layer                     │           │
│  │  • ChatSecurityService (injection detection) │           │
│  │  • RateLimitService (request throttling)     │           │
│  │  • JWT Authentication                        │           │
│  └─────────────────────────────────────────────┘           │
└─────────────────────────┬───────────────────────────────────┘
                          │
            ┌─────────────┴─────────────┐
            ▼                           ▼
┌───────────────────┐       ┌───────────────────┐
│    PostgreSQL     │       │    OpenAI API     │
│   pawsitive_haven │       │   GPT-4o-mini     │
└───────────────────┘       └───────────────────┘
```

---

## Tech Stack

| Layer | Technology |
|-------|------------|
| **Frontend** | Blazor Server, Razor Components |
| **Backend** | ASP.NET Core 9 Web API |
| **Database** | PostgreSQL 17 with Entity Framework Core |
| **AI** | OpenAI GPT-4o-mini |
| **Authentication** | JWT with BCrypt password hashing |
| **Containerization** | Docker & Docker Compose |
| **Security** | Custom prompt injection detection, rate limiting |

---

## Security Implementation

The AI chatbot implements multiple layers of defense based on OWASP LLM Top 10 guidelines:

| Layer | Protection |
|-------|------------|
| **Input Validation** | Message length limits, character filtering |
| **Injection Detection** | Regex patterns for role manipulation, jailbreak attempts |
| **Rate Limiting** | 20/min, 100/hr, 500/day with automatic 24h bans |
| **System Prompt** | Hardened with strict boundaries and manipulation responses |
| **Output Filtering** | Detects and blocks system prompt leakage |

See [CHATBOT_SECURITY.md](docs/CHATBOT_SECURITY.md) for the complete threat model and security documentation.

---

## Project Structure

```
PawsitiveHaven/
├── src/
│   ├── PawsitiveHaven.Api/        # Backend Web API
│   │   ├── Controllers/           # REST endpoints
│   │   ├── Services/              # Business logic + security
│   │   ├── Data/Repositories/     # Data access layer
│   │   └── Models/                # Entities and DTOs
│   └── PawsitiveHaven.Web/        # Blazor Frontend
│       ├── Components/Pages/      # UI pages
│       ├── Services/              # API clients
│       └── wwwroot/Assets/        # Knowledge base documents
├── database/
│   └── init.sql                   # Schema + seed data (26 FAQs)
├── docs/
│   ├── CHATBOT_SECURITY.md        # Security threat model
│   ├── CHATBOT_IMPLEMENTATION_PLAN.md
│   └── KNOWLEDGE_BASE.md          # AI knowledge base catalog
└── docker-compose.yml             # Development environment
```

---

## API Endpoints

| Category | Endpoints | Auth |
|----------|-----------|------|
| **Authentication** | `POST /api/auth/login`, `/register` | Public |
| **Pets** | `GET/POST/PUT/DELETE /api/pets` | JWT |
| **Appointments** | `GET/POST/PUT/DELETE /api/appointments` | JWT |
| **AI Chat** | `POST /api/ai/chat`, `GET /api/ai/conversations` | JWT |
| **FAQs** | `GET /api/faqs`, admin CRUD | Public / Admin |
| **Admin** | `GET/POST/PUT/DELETE /api/admin/users` | Admin |

---

## Knowledge Base

The AI assistant is powered by a comprehensive knowledge base:

- **26 FAQ entries** covering adoption, fostering, medical care, and behavior
- **4 guide documents**: First-time foster guide, adoption process, foster care checklist, organizational contacts
- **Emergency information** with quick-reference contact numbers

---

## Development

### Prerequisites
- .NET 9 SDK
- Docker & Docker Compose
- OpenAI API key

### Quick Start

```bash
# Start PostgreSQL
docker-compose up -d

# Run backend (terminal 1)
cd src/PawsitiveHaven.Api
dotnet run

# Run frontend (terminal 2)
cd src/PawsitiveHaven.Web
dotnet run
```

### Production

```bash
docker-compose -f docker-compose.prod.yml up --build -d
```

---

## License

This project is part of a portfolio demonstration.

---

*Built with ASP.NET Core 9, Blazor, and OpenAI*
