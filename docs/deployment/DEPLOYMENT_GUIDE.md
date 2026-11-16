# Deployment Guide

## Overview

The application ships as a single container image (`src/Zeroprime.Web/Dockerfile`) and expects **PostgreSQL 16+**. It applies EF migrations at startup when `Database:MigrateOnStartup=true` (default) and exposes `/health` for readiness.

No reverse proxy is bundled — put any TLS-terminating proxy/load balancer in front (nginx, Caddy, Traefik, Azure App Gateway, …).

## Environment variables

| Variable | Required | Example | Notes |
|---|---|---|---|
| `ConnectionStrings__Postgres` | **yes** | `Host=db.internal;Port=5432;Database=zeroprime;Username=zeroprime;Password=…;Ssl Mode=Require` | Never commit; supply via your secret store |
| `Database__MigrateOnStartup` | no (default `true`) | `true` / `false` | Set `false` if migrations are applied out-of-band |
| `ASPNETCORE_ENVIRONMENT` | no | `Production` | Controls error handling and HSTS |
| `ASPNETCORE_URLS` | no | `http://+:8080` | Defaults baked into the image |
| `AllowedHosts` | no | `example.com;www.example.com` | Set to your domains |

## Option A — docker compose (recommended for single-host deployments)

```bash
git clone <repo-url> && cd Zeroprime-Tech-Eg-Mvc
# edit docker-compose.yml or override via environment:
docker compose build
docker compose up -d          # web: http://<host>:8080, postgres internal+5432
docker compose ps             # both services healthy
curl http://localhost:8080/health   # -> Healthy
```

The compose file uses **development-only** credentials for PostgreSQL. For any shared or production host, override them via the environment or a `.env` file (not committed):

```bash
ConnectionStrings__Postgres="Host=postgres;Port=5432;Database=zeroprime;Username=zeroprime;Password=$(openssl rand -base64 32)"
POSTGRES_PASSWORD="...same value..."
```

Data persists in the `zeroprime-pgdata` volume.

## Option B — container registry image

CI (`.github/workflows/docker.yml`) builds the image and, on pushes to `main`, pushes it to **GHCR**: `ghcr.io/eraasoft-train/zeroprime-tech-eg-mvc:latest`.

```bash
docker run -d --name zeroprime-web \
  -p 8080:8080 \
  -e ConnectionStrings__Postgres="Host=…;Port=5432;Database=zeroprime;Username=…;Password=…;Ssl Mode=Require" \
  -e ASPNETCORE_ENVIRONMENT=Production \
  ghcr.io/eraasoft-train/zeroprime-tech-eg-mvc:latest
```

## Option C — platform-as-a-service

Any .NET-capable PaaS works (Azure App Service, Render, Railway, Fly.io, a VM with systemd):

1. Deploy the Dockerfile from repo root `src/Zeroprime.Web/Dockerfile` (or the registry image).
2. Provision managed PostgreSQL (16+).
3. Set the environment variables above.
4. Route HTTPS traffic to container port **8080**; platform terminates TLS.
5. Point `/health` at your uptime monitor.

## Database

- Migrations live in `src/Zeroprime.Infrastructure/Persistence/Migrations` and are idempotent.
- First deployment: `docker exec` into the web container or run `dotnet ef database update` from CI with `ConnectionStrings__Postgres` set — or rely on startup migration (default).
- Seed data (company profile, CanBank, site settings, social links) ships via EF `HasData` in migration `InitialCreate`.
- **No data migration** is performed from the retired FinanceControl app (demo data; see migration plan).
- Backups: standard PostgreSQL tooling (`pg_dump`). The `ContactInquiries` table is the only write-generated data.

## Post-deployment checklist

- [ ] `https://<host>/health` returns `Healthy`
- [ ] `https://<host>/` renders; `/sitemap.xml` and `/robots.txt` resolve
- [ ] `curl -I https://<host>/` shows CSP, nosniff, X-Frame-Options headers
- [ ] Contact form: valid submission redirects and is stored (check logs for `Contact inquiry {Id}` line); `__RequestVerificationToken` present on the page
- [ ] 404 page renders for an unknown URL (custom page, HTTP 404)
- [ ] Secrets are supplied via environment — nothing in the image or repository

## Rollback

Images are tagged per commit SHA on GHCR — redeploy the previous tag. Database migrations are additive for the current schema; if you ever need to roll back a schema change, restore from `pg_dump` (documented) or down-migrate manually.

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| Container starts, pages 500 | DB unreachable / wrong connection string | Check logs; verify network path and credentials; `/health` will be Unhealthy |
| `Migrate on startup failed` in logs | DB not yet up or schema lock | Wait for DB healthy; restart; or apply migrations out-of-band |
| HTTPS redirect warning in logs | Container serves plain HTTP on 8080; edge proxy terminates TLS | Expected — `UseHttpsRedirection` finds no HTTPS port and logs a warning instead of redirecting. Serve HTTPS at the proxy; do not expose the container directly |
| 429 on `/contact` POST | Rate limit (5 / 10 min / IP) | Wait for the window; do not disable in production |
| Static assets 404 | `.dockerignore` excluded wwwroot | Verify `.dockerignore`; publish output must contain `wwwroot/` |
