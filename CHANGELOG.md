# Changelog

All notable changes to this project will be documented in this file.

## [3.2.1] - 2026-11-16

### Added

- **Scroll to top button** — floating amber circular button, bottom-right, appears after 320px of scroll with a smooth fade/rise animation, smooth-scrolls to the top (instant on `prefers-reduced-motion`), shifts upward while the cookie banner is visible. ~50 lines of dependency-free JS/CSS.

### Changed

- Footer bottom bar: "Built with EraaSoft" external link to https://eraasoft.com added to the copyright line (amber, subtle underline).
- Footer bottom bar now shows **accepted payment method** glyphs instead of the "Company information…" note: Visa, Mastercard, American Express and Meeza as inline SVG cards (no visible text; `aria-label`s retained for screen readers).

### Fixed

- **Duplicate logo and brand name in the header** — `.zp-brand img { display: block }` (specificity 0-1-1) out-specified the theme switch hiding rule `.zp-logo--light` / `.zp-logo--dark` (0-1-0). Removed the redundant `display: block` (brand link is inline-flex, so images are blockified automatically); exactly one logo renders per theme.

## [3.2.0] - 2026-11-16

### Added

- **Cookie consent banner** — modern, app-style bottom banner that fades in with a smooth animation ~3 seconds after page load (never before first paint). Choice persisted to `localStorage` (`zp-consent`) and a first-party cookie (`zp_consent`, 1 year, `SameSite=Lax`); "Essentials only" and close/Escape dismiss without storing consent; banner stays hidden after choice. `aria` wiring, Escape-to-dismiss, `prefers-reduced-motion` support, safe-area insets for iOS. Small `window.ZeroPrimeConsent` API for future non-essential features.
- **Session** support with hardened defaults — `zp.session` cookie: HttpOnly, SameSite=Lax, 20-minute sliding expiry, `IsEssential`; wired into the pipeline via `app.UseSession()`.
- **Privacy Policy** now documents the real, complete cookie/storage inventory (antiforgery, `zp_consent`, `zp.session`, `zp-consent`, `zp-consent-dismissed`, `zp-theme`) with durations and purposes, in a responsive theme-aware table.
- Integration tests asserting the banner markup renders on public pages and the privacy policy documents the real cookies (20 integration tests total).

## [3.1.1] - 2026-11-16

### Fixed

- **Footer social icons rendered as a vertical stack** — `.zp-footer ul` (specificity 0-1-1) out-specified `.zp-social` (0-1-0), forcing `display: grid` with a single column; `.zp-footer a:hover` also beat the amber hover. The reset now uses `:not(.zp-social)` and the social rules are scoped to the footer with higher specificity. Icons render horizontally with the amber hover restored.
- **Error page required the database** — both layout-level view components (`OrganizationSchema`, `SocialLinks`) called the database, so with the DB down the exception handler itself failed with a bare 500. Both components now fall back to verified static values and log a warning; `/Home/Error` renders the friendly page with the full layout even with the database unreachable (verified against a dead-DB instance).
- Button `:disabled` state (opacity, cursor, no transform/shadow) and `touch-action: manipulation` on buttons.
- Footer bottom bar centers on narrow screens; dark-mode error numeral contrast; `dl` default margin reset in the fact strip.
- Removed dead `.zp-products-page` CSS rules.

### Added

- `.gitignore`: `Properties/PublishProfiles/` (local deployment profiles — the `.pubxml.user` password file was already ignored via `*.user`).

## [3.1.0] - 2026-11-16

### Added

- **Dark mode** — user-toggleable light/dark theme via a header button with `localStorage` persistence and `prefers-color-scheme` detection (applied pre-paint by `wwwroot/js/theme.js`, no flash). Full dark palette for surfaces, borders, forms, cards, shadows, header and footer.
- **Custom scrollbars** — thin scrollbars themed with the site's emerald/amber palette (`scrollbar-color` for Firefox + `::-webkit-scrollbar` for Chromium); native up/down arrow buttons are hidden (`::-webkit-scrollbar-button { display: none }`).
- `docs/deployment/MONSTERASP_DEPLOYMENT.md` — verified MonsterASP.NET deployment guide (PostgreSQL 18 Beta on the free tier, environment variables via Control Panel, Web Deploy / FTP / GitHub deploy, Let's Encrypt, free subdomain). No code changes were required — Npgsql works through their PgBouncer pooler unmodified. Includes a security note about a leaked `db80289` credential found in a local config file (never committed; rotate it).

### Changed

- Footer is pinned to the bottom on all page heights (flex-column body, growing `main`, non-shrinking footer) — verified via DOM geometry on short and long pages.
- Social links: fixed raw Razor text leak in the SocialLinks view (`@if` missing `@`), amber hover state, outline-square icons, proper accessible markup.
- Layout/grid: responsive fact strip (2→4 cols), 2-column card grid on tablets, consistent gaps, sections and radii; tighter mobile spacing and typography; improved grid stacking at all breakpoints; no horizontal scroll at any width.
- Navbar: always-visible theme toggle button, logo switches to the light variant in dark mode, brand → panel → theme toggle ordering with mobile wrap.
- Lighthouse re-audit (production build): **Accessibility 1.0 · Best Practices 1.0 · SEO 1.0** (was 0.94 / 1.0 / 1.0).

## [3.0.0] - 2026-11-16

Complete transformation of the repository from the FinanceControl API demo into the production-ready **ZeroPrime Technologies** ASP.NET Core MVC website. See `docs/architecture/MIGRATION_PLAN.md` and `docs/architecture/REPOSITORY_AUDIT.md`.

### Added

#### Application
- ASP.NET Core 9 MVC application with Razor views and a custom responsive design system (deep-emerald/amber palette, Bootstrap 5.3 grid)
- Public pages: Home, About (mission/vision/values/backers), CanBank product index + details, Smart Cities, Contact, Privacy, Terms, custom 404 and error pages
- Contact inquiries persisted to PostgreSQL (FluentValidation + DataAnnotations, anti-forgery protected, rate limited to 5 submissions per 10 minutes per IP, PRG pattern)
- JSON-LD Organization schema, Open Graph/Twitter metadata, canonical URLs, breadcrumb structured data, dynamic `/sitemap.xml` and `/robots.txt`
- Security headers middleware (CSP, X-Content-Type-Options, X-Frame-Options, Referrer-Policy, Permissions-Policy, Cross-Origin-Opener-Policy), HSTS, secure error handling without internal leaks
- `/health` readiness endpoint (EF Core DbContext health check)
- Serilog structured logging (console) with request logging
- Response caching headers for static assets, in-memory caching for brochure content

#### Architecture
- Clean Architecture solution: `Zeroprime.Domain`, `Zeroprime.Application`, `Zeroprime.Infrastructure`, `Zeroprime.Web`
- Repository + application service pattern, DTOs (no entity exposure to views), EF Core Fluent API configurations
- EF Core migration `InitialCreate` with idempotent seed of verified company data (company profile, CanBank product, site settings, social links)
- `IDesignTimeDbContextFactory` for safe `dotnet ef` tooling

#### Testing & Quality
- 29 unit tests (domain rules, validator, application services with stub repositories)
- 4 architecture tests (NetArchTest) enforcing Clean Architecture boundaries
- 15 integration tests (WebApplicationFactory + Testcontainers PostgreSQL): public pages, security headers, sitemap/robots, health check, contact form (valid/invalid/missing anti-forgery token), 404 handling
- GitHub Actions: `ci.yml` (restore, build, test, `dotnet format --verify-no-changes`), `docker.yml` (GHCR image build/push on main)
- `libman.json` pinning frontend library versions

#### Documentation
- Rewrote README, CONTRIBUTING, CHANGELOG
- Added `docs/architecture/SYSTEM_ARCHITECTURE.md`, `docs/development/*` guides, `docs/deployment/DEPLOYMENT_GUIDE.md`
- New `docker-compose.yml` (postgres + web) and production `Dockerfile`

### Changed
- Application purpose: personal finance API demo → ZeroPrime Technologies public website (company identity based on public sources only; see `docs/research/ZEROPRIME_COMPANY_RESEARCH.md`)
- Target database schema redesigned (`CompanyProfiles`, `Products`, `ContactInquiries`, `SocialMediaLinks`, `SiteSettings`)

### Removed
- FinanceControl API implementation (Controllers, Domain, Application, Data/Migrations) and its React-less static site shell — full removal documented in the migration plan; no useful finance functionality was retained because it is unrelated to the company website
- Kafka and Redis integrations (not required by the public website; documented in `docs/architecture/REPOSITORY_AUDIT.md`)
- Swagger/OpenAPI (no public API surface)
- Finance-specific docs and old brand assets
- Leaked external connection string that was staged in `appsettings.json` (never committed; replaced with documented local-development defaults + environment-variable configuration)
- Placeholder GitHub funding configuration

### Migration notes
- Database: fresh schema. Previous `Expenses`/`Incomes` tables and FinanceControl data are not migrated (demo data, no business value). See `docs/deployment/DEPLOYMENT_GUIDE.md` for greenfield rollout steps.
- Configuration: `ConnectionStrings:Postgres` moved to environment variables; local dev defaults live in `docker-compose.yml` only.
- Branch: work proceeds on `feature/zeroprime-mvc-transformation`; never merged to `main` directly.

## [2.1.0] - 2026-04-21

### Added

#### Static Pages & UI
- **Professional Static Pages**
  - `wwwroot/index.html` - Home page with hero section, features showcase, quick start guide, technology stack, and status indicators
  - `wwwroot/docs.html` - API documentation page with sidebar navigation, endpoint documentation, and interactive examples
  - `wwwroot/404.html` - Custom error page with helpful suggestions and navigation links

- **Microsoft Fluent Design System Styling**
  - `wwwroot/styles/main.css` - Base styles, typography, colors, spacing system, and layout components (1,100+ lines)
  - `wwwroot/styles/fluent.css` - Fluent Design gradients, animations, and cinematic effects (800+ lines)
  - `wwwroot/styles/docs.css` - Documentation-specific styles for sidebar, code blocks, and tables (600+ lines)

#### Design Features
- Gradient blobs with smooth floating animations
- Responsive design for desktop, tablet, and mobile
- Smooth transitions and hover effects
- Glassmorphism effects on status cards
- Professional color palette with Microsoft Blue (#0078d4) and Cyan (#50e6ff)
- Feature cards with interactive hover states
- Code blocks with syntax highlighting
- API endpoint cards with method badges (GET, POST, PUT, DELETE)
- Info and warning boxes for documentation

#### Middleware
- Static file serving via `app.UseStaticFiles()`
- 404 fallback middleware to serve `404.html` for non-existent routes
- Preserves API endpoints (doesn't interfere with `/api/*` routes)

### Changed

#### Program.cs
- Added `app.UseStaticFiles()` middleware for serving static files
- Added 404 fallback middleware to handle non-existent routes gracefully
- Middleware order: HTTPS → Static Files → Authorization → Controllers → 404 Fallback

### Documentation

#### Updated Files
- `CHANGELOG.md` - Added v2.1.0 release notes

### Performance

- Static files served directly from wwwroot without processing
- CSS files optimized for fast loading
- Responsive images and lazy loading ready
- Minimal JavaScript for maximum performance

### Browser Support

- Chrome/Edge 90+
- Firefox 88+
- Safari 14+
- Mobile browsers (iOS Safari, Chrome Mobile)

---

## [2.0.0] - 2026-04-21

### Added

#### Infrastructure & Observability
- **Redis Integration**
  - Added StackExchange.Redis 2.7.10 for distributed caching
  - Created `CacheService` interface and implementation for cache operations
  - Supports get, set, remove, and exists operations with TTL support
  - Automatic JSON serialization/deserialization
  - Comprehensive error logging and cache hit/miss tracking

- **Serilog Structured Logging**
  - Integrated Serilog 4.0.1 for structured logging
  - Added Serilog.AspNetCore 8.0.1 for ASP.NET Core integration
  - Configured console sink for development
  - Added environment and thread ID enrichment
  - Automatic request/response logging

- **Elasticsearch Integration**
  - Added Serilog.Sinks.Elasticsearch 9.0.3 for log aggregation
  - Configured automatic template registration
  - Index format: `finance-api-{yyyy.MM.dd}` for daily log rotation
  - Minimum log level: Information

- **Kibana Visualization**
  - Added Kibana 8.11.0 container for log visualization
  - Accessible at `http://localhost:5601`
  - Pre-configured to connect to Elasticsearch

#### Docker & Deployment
- **Enhanced docker-compose.yml**
  - Updated PostgreSQL to version 15 with persistent volumes
  - Added Redis 7-alpine with persistent storage
  - Added Elasticsearch 8.11.0 with single-node configuration
  - Added Kibana 8.11.0 for log visualization
  - Added health checks for all services
  - Configured proper environment variables and networking
  - Added volume management for data persistence

#### Configuration
- **Updated appsettings.Development.json**
  - Added Redis connection string configuration
  - Added Elasticsearch URI configuration
  - Maintained existing PostgreSQL and Kafka settings

#### Dependencies
- `StackExchange.Redis` 2.7.10 - Redis client library
- `Serilog` 4.0.1 - Structured logging framework
- `Serilog.AspNetCore` 8.0.1 - ASP.NET Core integration
- `Serilog.Sinks.Elasticsearch` 9.0.3 - Elasticsearch sink
- `Serilog.Enrichers.Environment` 2.3.0 - Environment enrichment
- `Serilog.Enrichers.Thread` 3.1.0 - Thread ID enrichment

### Changed

#### Program.cs
- Integrated Serilog as the logging provider
- Added Redis connection multiplexer registration
- Added ICacheService dependency injection
- Configured Elasticsearch sink with automatic template registration
- Added environment and thread enrichment to logs

#### Database
- Cleaned up duplicate migrations (removed Data/Migrations folder from project)
- Kept only Infra/Migrations as the active migration folder
- Migration: `20230720004620_InitialMigration` (Expenses and Incomes tables)

### Fixed

#### Migration Issues
- **Resolved duplicate migrations**: Removed inactive Data/Migrations folder
  - `Data/Migrations/20230720004358_InitialMigration.cs` (inactive - excluded from project)
  - `Infra/Migrations/20230720004620_InitialMigration.cs` (active - used by EF Core)
  - Both created identical schema; only Infra version is now used
  - Prevents migration conflicts and confusion

#### Configuration
- Updated PostgreSQL password from `P@ssw0rd` to `admin` for consistency
- Added proper health checks for all Docker services
- Configured proper networking between services

### Documentation

#### New Files
- `CHANGELOG.md` - This file, tracking all changes and versions
- `docs/REDIS.md` - Redis caching implementation guide with usage patterns, configuration, and best practices
- `docs/LOGGING.md` - Serilog and Elasticsearch logging guide with Kibana usage and log analysis
- `docs/OBSERVABILITY.md` - Complete observability stack documentation with monitoring workflows and dashboards

### Testing

#### New Test Cases
- **CacheService Tests** (`CacheServiceTests.cs`)
  - `GetAsync_WithValidKey_ReturnsValue` - Verify cache retrieval
  - `GetAsync_WithInvalidKey_ReturnsNull` - Verify cache miss handling
  - `SetAsync_WithValue_StoresInCache` - Verify cache storage
  - `SetAsync_WithExpiration_SetsExpiry` - Verify TTL functionality
  - `RemoveAsync_WithValidKey_DeletesFromCache` - Verify cache removal
  - `ExistsAsync_WithValidKey_ReturnsTrue` - Verify key existence check
  - `ExistsAsync_WithInvalidKey_ReturnsFalse` - Verify non-existent key check
  - `MultipleOperations_WorkCorrectly` - Verify concurrent operations

- **Redis Integration Tests** (`RedisIntegrationTests.cs`)
  - `Redis_Connection_IsSuccessful` - Verify Redis connectivity
  - `CacheService_WorksWithRedis_InIntegrationTests` - Verify cache operations
  - `CacheService_CanRemoveValues_FromRedis` - Verify cache removal
  - `CacheService_CanCheckKeyExistence_InRedis` - Verify key existence check
  - `CacheService_RespectsTTL_InRedis` - Verify TTL expiration
  - `CacheService_HandlesMultipleDataTypes_InRedis` - Verify type handling
  - `CacheService_HandlesLargeValues_InRedis` - Verify large data handling
  - `CacheService_HandlesConcurrentOperations_InRedis` - Verify concurrent access

- **Logging Integration Tests** (`LoggingIntegrationTests.cs`)
  - `Application_StartsSuccessfully_WithSerilogConfigured` - Verify Serilog initialization
  - `DatabaseContext_IsAvailable_InApplication` - Verify database connectivity
  - `Logger_IsConfigured_InDependencyInjection` - Verify logger registration
  - `HealthCheck_ReturnsSuccessful_WhenServicesAreRunning` - Verify application health

### Infrastructure Changes

#### Docker Services
| Service | Image | Port | Purpose |
|---------|-------|------|---------|
| PostgreSQL | postgres:15 | 5432 | Primary database |
| Kafka | bashj79/kafka-kraft | 9092 | Event streaming |
| Redis | redis:7-alpine | 6379 | Distributed cache |
| Elasticsearch | elasticsearch:8.11.0 | 9200 | Log aggregation |
| Kibana | kibana:8.11.0 | 5601 | Log visualization |

#### Health Checks
All services now include health checks:
- PostgreSQL: `pg_isready` command
- Kafka: Broker API versions check
- Redis: `redis-cli ping` command
- Elasticsearch: Cluster health endpoint
- Kibana: Status API endpoint

### Performance Improvements

- **Caching Layer**: Reduces database queries for frequently accessed data
- **Structured Logging**: Enables efficient log searching and analysis
- **Log Aggregation**: Centralized logging for better observability
- **Distributed Cache**: Supports horizontal scaling with shared cache

### Breaking Changes

None - This is a backward-compatible release with new features.

### Migration Guide

#### For Existing Deployments

1. **Update NuGet Packages**
   ```bash
   dotnet restore
   ```

2. **Update Configuration**
   - Add Redis connection string to appsettings
   - Add Elasticsearch URI configuration
   - Update PostgreSQL password if needed

3. **Update Docker Compose**
   ```bash
   docker-compose down
   docker-compose up -d
   ```

4. **Verify Services**
   - PostgreSQL: `psql -h localhost -U postgres -d FinanceControl`
   - Redis: `redis-cli ping`
   - Elasticsearch: `curl http://localhost:9200/_cluster/health`
   - Kibana: Visit `http://localhost:5601`

### Known Issues

None at this time.

---

## [1.0.0] - 2026-04-20

### Initial Release

- ASP.NET Core 9 API with PostgreSQL
- Kafka event streaming integration
- Integration tests with Testcontainers
- Docker support with multi-stage builds
- Swagger/OpenAPI documentation
- GitHub Actions CI/CD pipeline
