# Dependencies

Every dependency below is pinned and intentional. Frontend assets are vendored locally (see `src/Zeroprime.Web/libman.json`); the site has **no runtime CDN dependencies**.

## Build-time tools

| Tool | Where | Why |
|---|---|---|
| `dotnet-ef` 9.0.12 | Repo-local tool manifest (`.config/dotnet-tools.json`) | EF Core migrations from any machine: `dotnet tool restore` then `dotnet tool run dotnet-ef …`. Pinned to the .NET 9 line; works even when the global `dotnet-ef` install is broken or unavailable. **Not** needed on the server — migrations apply at startup. |

## Runtime — Zeroprime.Web

| Package | Version | Why it exists |
|---|---|---|
| `Serilog.AspNetCore` | 9.0.0 | Structured request/application logging to console |
| `Serilog.Enrichers.Environment` | 3.0.1 | `EnvironmentName` property on logs |
| `Serilog.Enrichers.Process` | 3.0.0 | Process id on logs |
| `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore` | 9.0.0 | `/health` readiness probe backed by the DbContext |
| `Microsoft.EntityFrameworkCore.Design` | 9.0.0 | `dotnet ef` tooling (design-time only, private assets) |
| `Serilog.Sinks.Console` (transitive) | — | Console sink used by `Program.cs` |

## Runtime — Zeroprime.Application

| Package | Version | Why it exists |
|---|---|---|
| `FluentValidation` | 11.11.0 | Authoritative server-side validation of the contact form (and future inputs) |
| `Microsoft.Extensions.Caching.Abstractions` | 9.0.0 | `IMemoryCache` for read-mostly brochure content |
| `Microsoft.Extensions.DependencyInjection.Abstractions` | 9.0.0 | Service registration extensions |
| `Microsoft.Extensions.Logging.Abstractions` | 9.0.0 | `ILogger<T>` in services |

## Runtime — Zeroprime.Infrastructure

| Package | Version | Why it exists |
|---|---|---|
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 9.0.0 | PostgreSQL provider for EF Core |
| `Microsoft.EntityFrameworkCore` | 9.0.0 | ORM, migrations, `ApplyConfigurationsFromAssembly` |
| `Microsoft.EntityFrameworkCore.Design` | 9.0.0 | Migration generation (design-time assets) |

## Runtime — Zeroprime.Domain

None. Domain is dependency-free by design (architecture-test enforced).

## Frontend (vendored via LibMan)

| Library | Version | Source |
|---|---|---|
| Bootstrap (CSS + bundle JS) | 5.3.3 | cdnjs; MIT |
| jQuery | 3.7.1 | cdnjs; MIT |
| jQuery Validation | 1.21.0 | cdnjs; MIT |
| jQuery Validation Unobtrusive | 4.0.0 | cdnjs; Apache-2.0 |

Note: Bootstrap bundle ships its own Popper — no separate Popper copy exists. Bootstrap Icons were **not** added (inline SVG icons are used instead, zero additional payload). No `Select2`/`DataTables`/`SweetAlert2` — the site has no such UI surface (YAGNI).

## Test projects

| Package | Project | Why |
|---|---|---|
| `xunit`, `xunit.runner.visualstudio`, `Microsoft.NET.Test.Sdk`, `coverlet.collector` | all | Test framework |
| `Microsoft.AspNetCore.Mvc.Testing` | IntegrationTests | Boots the real MVC app in-memory |
| `Testcontainers.PostgreSql` | IntegrationTests | Disposable PostgreSQL for DB-backed tests |
| `NetArchTest.Rules` | ArchitectureTests | Static analysis of layer dependencies |
| `Microsoft.Extensions.Caching.Memory` | UnitTests | Real `MemoryCache` in service tests |

## Explicitly removed

| Removed | Reason |
|---|---|
| Kafka (`Confluent.Kafka`) | Demo eventing; not used by the public website |
| Redis (`StackExchange.Redis`) | Demo caching; in-memory cache is sufficient |
| Swagger (`Swashbuckle`) | No public API surface |
| Elasticsearch sink | No log aggregation requirement; console logs suffice |
| AutoMapper | No mapping complexity justified — manual DTO mapping is clearer |
| MediatR | No CQRS complexity justified — direct service calls are sufficient |
| FinanceControl.* | Demo finance domain removed wholesale |
