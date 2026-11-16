# Testing Guide

## Layout

```text
tests/
├── Zeroprime.UnitTests/        # Fast, no I/O — domain, validators, services (stubbed repos)
├── Zeroprime.ArchitectureTests/# Clean Architecture boundary enforcement (NetArchTest)
└── Zeroprime.IntegrationTests/ # Real app + real PostgreSQL via Testcontainers
```

## Running

```bash
dotnet test                                  # full suite
dotnet test tests/Zeroprime.UnitTests        # ~1s
dotnet test tests/Zeroprime.ArchitectureTests
dotnet test tests/Zeroprime.IntegrationTests  # needs Docker; first run pulls postgres:16-alpine
```

## Unit tests

- Domain: `CompanyProfile.Create` validation, `ContactInquiry` lifecycle transitions, `Product` activation toggling (constructor guards, UTC timestamps)
- `CreateContactInquiryRequestValidator`: required fields, e-mail format, message length bounds, phone character whitelist
- `ContactInquiryService`: stores valid inquiries; returns field-keyed errors otherwise (with a stub repository)
- `ProductService`: JSON→DTO mapping, empty-list fallback on malformed JSON, single repository hit via the in-memory cache

Stubs are hand-written (`IContactInquiryRepository`, `IProductRepository`); no mocking framework — behavior is simple enough to fake explicitly.

## Architecture tests

- Domain does not depend on Application/Infrastructure/Web
- Application does not depend on Infrastructure/Web
- Infrastructure does not depend on Web
- Domain entities do not reference EF Core

These fail CI if a forbidden reference is introduced.

## Integration tests

`ZeroprimeWebFactory` (a `WebApplicationFactory<Program>`) starts a disposable `postgres:16-alpine` container (Testcontainers), sets the connection string via in-memory configuration (resolved lazily by the DI registration), applies migrations, and serves the real application in-memory. Each test class gets its own fixture/container.

Coverage includes:

- All public pages return 200 with expected content markers
- Products listing/detail, unknown product → 404 (custom page with 404 status)
- `/health` → `Healthy` when the database is reachable
- `/sitemap.xml` contains the products route; `/robots.txt` references the sitemap
- Security headers present on responses (CSP, nosniff, X-Frame-Options, …)
- Contact form end-to-end: GET renders with anti-forgery token; valid POST redirects and **persists** the inquiry (verified against the container DB); invalid POST redisplays with errors and persists nothing; POST without anti-forgery token → 400

### Notes & limitations

- Docker must be running for integration tests locally and in CI (`ubuntu-latest` provides it).
- Testcontainers maps a random host port per container; tests never touch your local PostgreSQL.
- Rate limiting (`contact`, 5/10min/IP) applies in tests too; the valid-submission test submits once, well below the limit.
- `Database:MigrateOnStartup=false` is set for the factory; migrations are applied explicitly in the fixture via `db.Database.Migrate()`.

## CI

`.github/workflows/ci.yml` runs restore → build (Release) → unit tests → architecture tests → integration tests → `dotnet format --verify-no-changes` on pushes and PRs to `main` and `feature/**`.
