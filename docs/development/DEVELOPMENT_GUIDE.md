# Development Guide

## Prerequisites

| Tool | Version | Purpose |
|---|---|---|
| .NET SDK | 9.0.x | Build & run |
| PostgreSQL | 16 (docker-compose) | Development database |
| Docker | any recent | Local database + integration tests (Testcontainers) |
| EF Core CLI | 9.x+ | `dotnet ef` migrations (`dotnet tool install --global dotnet-ef`) |

## First-time setup

```bash
git clone https://github.com/eraasoft-train/Zeroprime-Tech-Eg-Mvc.git
cd Zeroprime-Tech-Eg-Mvc

docker compose up -d postgres           # publishes 5432; user/db=zeroprime
dotnet restore
dotnet ef database update --project src/Zeroprime.Infrastructure --startup-project src/Zeroprime.Web
dotnet run --project src/Zeroprime.Web
```

App starts at http://localhost:5135 (HTTP) or https://localhost:7068 (HTTPS). `Database:MigrateOnStartup` (default `true`) also applies migrations at startup — the explicit `ef database update` is useful when you want to inspect or control the migration step.

## Configuration

Configuration follows standard ASP.NET Core precedence (later wins):

1. `appsettings.json` — committed, **no real secrets** (local-dev connection string only)
2. `appsettings.Development.json` — committed, logging levels only
3. Environment variables — `ConnectionStrings__Postgres`, `Database__MigrateOnStartup`, …
4. Command-line arguments

| Setting | Default | Notes |
|---|---|---|
| `ConnectionStrings:Postgres` | `localhost:5432` local dev default | **Override in every real environment** |
| `Database:MigrateOnStartup` | `true` | Set `false` to manage migrations manually |
| `AllowedHosts` | `*` | Restrict in production if you terminate outside a proxy |

Local development secrets: use `dotnet user-secrets` — never appsettings.
```bash
dotnet user-secrets set "ConnectionStrings:Postgres" "Host=...;Password=..." --project src/Zeroprime.Web
```

## Common tasks

### Add an EF Core migration
```bash
dotnet ef migrations add <Name> --project src/Zeroprime.Infrastructure --startup-project src/Zeroprime.Web --output-dir Persistence/Migrations
dotnet ef database update --project src/Zeroprime.Infrastructure --startup-project src/Zeroprime.Web
```
The design-time factory (`Infrastructure/Persistence/Context/ZeroprimeDbContextFactory.cs`) lets EF tooling work without starting the web app. Set `ConnectionStrings__Postgres` when you need `database update` against a non-default server.

### Seed data
Seed data lives in EF `HasData` (configurations under `Infrastructure/Persistence/Configurations`). Changing seed data requires a new migration — it does not update existing rows.

### Run the site with Docker (full stack)
```bash
docker compose up --build
# web on http://localhost:8080, postgres on localhost:5432
```

### Frontend libraries
Vendored under `wwwroot/lib` with `libman.json` as the manifest. To restore/update: install LibMan CLI (`dotnet tool install --global Microsoft.Web.LibraryManager.Cli`) and run `libman restore` / edit `libman.json`.

### Code style & formatting
```bash
dotnet format
dotnet format --verify-no-changes     # CI gate
```

## Debugging

- Logs go to the console (Serilog). Set `Serilog:MinimumLevel:Default` to `Debug` for more detail.
- `/health` — 200 `Healthy` when PostgreSQL is reachable.
- Development errors: the developer exception page shows full stack traces (Production shows `/Home/Error` instead).
- Contact form not submitting? Check the browser console (unobtrusive validation), server logs, and that anti-forgery is not being stripped by a proxy (cookies must flow).

## Troubleshooting

| Symptom | Fix |
|---|---|
| `Connection string 'Postgres' is not configured` | Set `ConnectionStrings__Postgres` or check appsettings |
| 28P01 password auth failed | You are pointing at an unknown/existing PostgreSQL on 5432. Use docker-compose (`zeroprime`/`zeroprime_dev_only`) or override the connection string |
| `PendingModelChanges` on `ef database update` | You changed entities/config without a new migration |
| Static files 404 in Docker | Ensure `wwwroot` was copied (see `.dockerignore`); the published image includes it |
| Rate limit 429 on contact form | 5 submissions / 10 min per IP; wait or restart the dev server |

## Project conventions

- Namespaces match folders; file-scoped namespaces; nullable enabled solution-wide
- Records for DTOs/ViewModels; classes with invariants for entities
- Private setters / factory methods (`Create`) on entities; state changes via explicit methods (`MarkAsResolved`, …)
- `AsNoTracking()` for read queries; async all the way down; `CancellationToken` on service methods
