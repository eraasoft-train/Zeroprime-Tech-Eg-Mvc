# Zeroprime-Tech-Eg-Mvc — ZeroPrime Technologies Website

> Production-ready ASP.NET Core 9 MVC application for **ZeroPrime Technologies**, an Egyptian startup building smart, sustainable cities infrastructure and autonomous waste management — starting with **CanBank**, a reverse vending machine that rewards recycling.

[![CI](https://github.com/eraasoft-train/Zeroprime-Tech-Eg-Mvc/actions/workflows/ci.yml/badge.svg)](https://github.com/eraasoft-train/Zeroprime-Tech-Eg-Mvc/actions/workflows/ci.yml)
[![Docker](https://github.com/eraasoft-train/Zeroprime-Tech-Eg-Mvc/actions/workflows/docker.yml/badge.svg)](https://github.com/eraasoft-train/Zeroprime-Tech-Eg-Mvc/actions/workflows/docker.yml)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## What this is

The public website and company profile for **ZeroPrime Technologies** (Cairo, Egypt, founded 2017):

- **Corporate pages** — home, about, mission & vision, smart-cities vision
- **Product showcase** — CanBank, a reverse vending machine for aluminum cans and plastic bottles
- **Contact inquiries** — validated, anti-forgery protected, rate-limited contact form persisted to PostgreSQL
- **SEO** — canonical URLs, Open Graph/Twitter cards, JSON-LD Organization schema, breadcrumb structured data, dynamic sitemap.xml and robots.txt
- **Security** — security headers + CSP, anti-forgery tokens, rate limiting, Serilog structured logs, `/health` readiness endpoint

Company content is based on **publicly sourced, cross-checked information** (see [`docs/research/ZEROPRIME_COMPANY_RESEARCH.md`](docs/research/ZEROPRIME_COMPANY_RESEARCH.md)). Nothing about clients, deployments, statistics or team members is fabricated.

## Technology stack

| Layer | Technology |
|-------|-----------|
| Runtime | .NET 9, ASP.NET Core MVC (Razor) |
| Architecture | Clean Architecture (Domain → Application → Infrastructure → Web) |
| Database | PostgreSQL 16+, EF Core 9 (migrations + seed data) |
| Frontend | Bootstrap 5.3, jQuery 3.7, jQuery Unobtrusive Validation, custom design system (no UI kits/templates) |
| Logging | Serilog (console, structured) |
| Validation | FluentValidation (server) + DataAnnotations (client-side) |
| Testing | xUnit, WebApplicationFactory, Testcontainers (PostgreSQL), NetArchTest |
| CI/CD | GitHub Actions (build/test/format + Docker image to GHCR) |

## Quick start

Prerequisites: [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0), [Docker](https://www.docker.com/).

```bash
git clone https://github.com/eraasoft-train/Zeroprime-Tech-Eg-Mvc.git
cd Zeroprime-Tech-Eg-Mvc

# 1. Start PostgreSQL (docker-compose publishes 5432)
docker compose up -d postgres

# 2. Apply migrations (seeds the verified company profile, CanBank product,
#    site settings and social links)
dotnet ef database update --project src/Zeroprime.Infrastructure --startup-project src/Zeroprime.Web

# 3. Run
dotnet run --project src/Zeroprime.Web
```

Visit http://localhost:5135. The app also migrates automatically on startup when `Database:MigrateOnStartup` is `true` (default) — step 2 is then optional.

### Full stack with Docker

```bash
docker compose up --build
# Web: http://localhost:8080  ·  Postgres: localhost:5432
```

## Testing

```bash
dotnet test                                  # everything
dotnet test tests/Zeroprime.UnitTests        # domain, validators, services (fast)
dotnet test tests/Zeroprime.ArchitectureTests# layer dependency rules
dotnet test tests/Zeroprime.IntegrationTests # boots the app against a disposable PostgreSQL container
```

Integration tests require Docker. See [`docs/development/TESTING_GUIDE.md`](docs/development/TESTING_GUIDE.md).

## Project structure

```text
src/
├── Zeroprime.Domain/         # Entities, enums, domain rules (no dependencies)
├── Zeroprime.Application/    # DTOs, application services, FluentValidation validators
├── Zeroprime.Infrastructure/ # EF Core context, migrations, repositories, seeding
└── Zeroprime.Web/            # MVC controllers, Razor views, ViewComponents, wwwroot
tests/
├── Zeroprime.UnitTests/
├── Zeroprime.IntegrationTests/
└── Zeroprime.ArchitectureTests/
docs/
├── architecture/             # Audit, migration plan, system architecture
├── research/                 # Company research with source citations
├── development/              # Dev guide, testing guide, dependencies, structure
└── deployment/               # Deployment guide
```

Details: [`docs/development/PROJECT_STRUCTURE.md`](docs/development/PROJECT_STRUCTURE.md)

## Documentation

- [`docs/architecture/SYSTEM_ARCHITECTURE.md`](docs/architecture/SYSTEM_ARCHITECTURE.md) — architecture and decisions
- [`docs/architecture/REPOSITORY_AUDIT.md`](docs/architecture/REPOSITORY_AUDIT.md) — what the repo was, and why it changed
- [`docs/architecture/MIGRATION_PLAN.md`](docs/architecture/MIGRATION_PLAN.md) — transformation plan
- [`docs/research/ZEROPRIME_COMPANY_RESEARCH.md`](docs/research/ZEROPRIME_COMPANY_RESEARCH.md) — every content claim with sources
- [`docs/development/DEVELOPMENT_GUIDE.md`](docs/development/DEVELOPMENT_GUIDE.md) — dev workflow, migrations, configuration
- [`docs/deployment/DEPLOYMENT_GUIDE.md`](docs/deployment/DEPLOYMENT_GUIDE.md) — Docker, CI/CD, production checklist
- [`docs/deployment/MONSTERASP_DEPLOYMENT.md`](docs/deployment/MONSTERASP_DEPLOYMENT.md) — MonsterASP.NET (IIS hosting, hosted PostgreSQL 18, free tier)
- [`CHANGELOG.md`](CHANGELOG.md) — version history

## Configuration

No secrets live in this repository. Provide the connection string via environment variable:

```bash
ConnectionStrings__Postgres="Host=...;Port=5432;Database=zeroprime;Username=...;Password=..."
```

Optional settings: `Database:MigrateOnStartup` (default `true`). See [appsettings.json](src/Zeroprime.Web/appsettings.json).

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

MIT — see [LICENSE](LICENSE). Company information remains the property of ZeroPrime Technologies.
