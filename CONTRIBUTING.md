# Contributing to Zeroprime-Tech-Eg-Mvc

Thanks for your interest in contributing to the ZeroPrime Technologies website.
This document describes the expected workflow and standards.

## Code of Conduct

Be respectful, inclusive, and professional in all interactions.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) (9.0.x)
- [Docker](https://www.docker.com/) (for PostgreSQL via docker-compose, and for the Testcontainers-based integration tests)
- Git

## Getting Started

```bash
git clone https://github.com/eraasoft-train/Zeroprime-Tech-Eg-Mvc.git
cd Zeroprime-Tech-Eg-Mvc
docker compose up -d postgres
dotnet restore
dotnet ef database update --project src/Zeroprime.Infrastructure --startup-project src/Zeroprime.Web
dotnet run --project src/Zeroprime.Web
```

Visit http://localhost:5135 (see `src/Zeroprime.Web/Properties/launchSettings.json`).

## Development Workflow

1. Create a branch from `main`: `git checkout -b feature/your-change`
2. Make your changes in small, logical commits with conventional messages (`feat:`, `fix:`, `docs:`, `test:`, `refactor:`).
3. Add or update tests for any behavior change.
4. Run the full quality gate locally:
   ```bash
   dotnet build
   dotnet test
   dotnet format --verify-no-changes
   ```
5. Open a pull request against `main`. Fill in the PR template, including screenshots for UI changes.

## Standards

- **Architecture** — keep the Clean Architecture boundaries intact: Domain must not reference Infrastructure, Application or Web; Application must not reference Infrastructure or Web. Architecture tests enforce this in CI.
- **Controllers** stay thin; business logic lives in Application services.
- **Views** render models; no business logic in Razor.
- **Content accuracy** — company facts must stay traceable to the verified research in `docs/research/ZEROPRIME_COMPANY_RESEARCH.md`. Do not fabricate statistics, clients, testimonials or team members.
- **No secrets** — never commit credentials or connection strings with real passwords. Use environment variables (`ConnectionStrings__Postgres`) or user-secrets locally.

## Reporting Issues

Open a GitHub issue using the bug report template. For security reports, email **zeroprime.tech@gmail.com** instead of filing a public issue.

## Pull Request Checklist

- [ ] `dotnet build` succeeds with no warnings
- [ ] `dotnet test` passes (unit, architecture and integration tests)
- [ ] `dotnet format --verify-no-changes` passes
- [ ] Documentation updated where relevant
- [ ] No secrets or generated artifacts committed
