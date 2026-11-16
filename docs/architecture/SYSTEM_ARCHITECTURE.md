# System Architecture — ZeroPrime Technologies Website

**Last updated:** 2026-11-16 · **Stack:** .NET 9 · ASP.NET Core MVC · PostgreSQL 16 · Bootstrap 5.3

## 1. Purpose and product direction

ZeroPrime Technologies is a real Egyptian startup (founded 2017, Cairo) building smart, sustainable cities infrastructure and autonomous waste management — starting with **CanBank**, a reverse vending machine. This application is the company's public website and contact-inquiry pipeline.

It replaces the former `FinanceControl` personal-finance API demo. Finance-domain code was removed entirely; nothing from it is load-bearing. The decision and rationale are documented in `docs/architecture/REPOSITORY_AUDIT.md`.

Content on the site is constrained to **verified public information** (see `docs/research/ZEROPRIME_COMPANY_RESEARCH.md`). No clients, deployments, statistics, team members or testimonials are invented.

## 2. Solution layout

```text
src/
├── Zeroprime.Domain/          # Entities (CompanyProfile, Product, ContactInquiry,
│                              #   SocialMediaLink, SiteSetting), InquiryStatus enum,
│                              #   domain creation/transition rules. No dependencies.
├── Zeroprime.Application/     # DTOs, application services (CompanyProfileService,
│                              #   ProductService, ContactInquiryService, settings/social
│                              #   services), FluentValidation validators, cache keys,
│                              #   repository abstractions (ports). Depends on Domain only.
├── Zeroprime.Infrastructure/  # EF Core context, migrations, Fluent API configurations +
│                              #   seed data, repository implementations, DI extensions,
│                              #   IDesignTimeDbContextFactory. Depends on Application + Domain.
└── Zeroprime.Web/             # Controllers, Razor views, ViewComponents, ViewModels,
                               #   security-headers middleware, wwwroot (assets, vendored
                               #   libraries), sitemap/robots endpoints. Depends on all.
tests/
├── Zeroprime.UnitTests/       # Domain rules, FluentValidation, application services (stubs)
├── Zeroprime.ArchitectureTests/  # Layer-dependency rules (NetArchTest)
└── Zeroprime.IntegrationTests/   # WebApplicationFactory + Testcontainers PostgreSQL
```

## 3. Clean Architecture rules (enforced by tests)

1. **Domain** may not reference Application, Infrastructure or Web.
2. **Application** may not reference Infrastructure or Web.
3. **Infrastructure** may not reference Web.
4. Views hold no business logic; controllers stay thin and delegate to services.
5. ViewModels/DTOs cross boundaries — entities never reach views (serialization of entities is never required; mapping is manual and explicit).

`tests/Zeroprime.ArchitectureTests` fails the build when a rule is violated.

## 4. Request flow

```text
Browser → SecurityHeadersMiddleware → StatusCodePages → HttpsRedirection
        → StaticFiles → Routing → RateLimiter → Authorization → Controller
Controller → I<Page>Service (Application) → I<…>Repository (abstraction)
          → Infrastructure implementation → ZeroprimeDbContext → PostgreSQL
ViewModel ← — — — — — ← — — — — — — ← DTO ← — — — — — — — — ← projection
```

- Read-mostly brochure content (company profile, products, settings, social links) is cached in-memory for 5 minutes (`Application/Common/CacheKeys`).
- Contact inquiries are writes; they bypass the cache, are validated server-side (FluentValidation) and logged without message contents (inquiry id only).

## 5. Data model

| Table | Key | Notes |
|---|---|---|
| CompanyProfiles | Id (singleton, Id=1) | Seeded from verified public data |
| Products | Id | Seeded with CanBank; `KeyFeatures`/`TargetLocations` stored as JSON text (owned lists) |
| ContactInquiries | Id | Captured via `/contact`; status enum (New/InProgress/Resolved/Closed) |
| SocialMediaLinks | Id | Seeded LinkedIn/Instagram/Facebook; renders footer social icons |
| SiteSettings | Key | SiteName, SiteDescription, HeroTitle, HeroSubtitle, ContactEmail |

EF migrations: `src/Zeroprime.Infrastructure/Persistence/Migrations`. Seeding lives in `HasData` inside each configuration — deterministic and identical across environments. `db.Database.MigrateAsync()` runs at startup unless `Database:MigrateOnStartup=false`; failures log a critical message but the process starts (readiness is exposed via `/health`).

## 6. Security model

- **Contact form anti-abuse**: anti-forgery token (built-in MVC), FluentValidation + DataAnnotations, rate limit 5 submissions / 10 min / IP (fixed window, `contact` policy), server-side only decisions (JS never makes authz/validation decisions).
- **Headers**: CSP `default-src 'self'` (no inline scripts; Bootstrap/jQuery are local files), `X-Content-Type-Options: nosniff`, `X-Frame-Options: DENY`, `Referrer-Policy`, `Permissions-Policy`, `Cross-Origin-Opener-Policy: same-origin`.
- **Errors**: developer pages in Development only; production returns `/Home/Error` (no exception details). 404/429/etc. re-execute to `/Home/StatusCode/{code}` with the proper status code.
- **Secrets**: none in the repo. `ConnectionStrings:Postgres` via `ConnectionStrings__Postgres` env var (see `appsettings.json` for local-dev defaults and the deployment guide).
- **Logging**: Serilog console sink; enrichment with environment and process id; no PII or credentials in logs.

## 7. SEO strategy

- Unique `<title>` and `<meta name="description">` per page (from `ViewData`, with layout fallback)
- Canonical link derived from request; Open Graph + Twitter `summary` card
- JSON-LD Organization (name, founding date 2017, Cairo/EG, email, sameAs social profiles) on every page
- Breadcrumbs with `itemscope` BreadcrumbList markup; semantic HTML and a single h1 per page
- `/sitemap.xml` generated from routes + active products (1-hour cache); `/robots.txt` generated with absolute sitemap URL
- Descriptive, hyphenated URLs (`/smart-cities`, `/products/1`)

## 8. Performance

- `asp-append-version="true"` fingerprints for CSS/JS; static files served with 7-day `Cache-Control`
- Async EF queries; `AsNoTracking` for reads; projections to DTOs (no over-fetching)
- In-memory caching of read-mostly content (5-minute TTL)
- No unnecessary third-party JS; libraries vendored locally via LibMan (`libman.json`, pinned versions)

## 9. Frontend architecture

- Hand-rolled design system (`wwwroot/css/site.css`) on top of Bootstrap 5.3 grid/utilities — no theme kits
- Components: hero, product panels, fact strip, feature lists, steps, CTA band, cards, breadcrumbs, footer
- Progressive enhancement: reveal-on-scroll via IntersectionObserver (respects `prefers-reduced-motion`)
- `wwwroot/js/site.js` (nav toggle, scroll shadow, reveals) and `contact-form.js` (submit lock/spinner); jQuery used only where unobtrusive validation needs it
- jQuery 3.7.1, jQuery Validation 1.21.0, jQuery Unobtrusive Validation 4.0.0, Bootstrap bundle 5.3.3 — single copies each, all local

## 10. Deliberate non-goals (and why)

- **No admin/Identity area** — no verified requirement or credential scheme for a public corporate brochure; adding a hardcoded admin login would be worse than none. Admin capability is future work requiring real requirements.
- **No Redis/Kafka** — the removed Kafka/Redis integrations served the demo; the public site has no need for them. Caching is in-memory.
- **No team/careers/portfolio pages** — no verified public information exists; we do not fabricate.
- **No fake statistics, logos, testimonials or clients** — research explicitly forbids them.
- **No Swagger** — no public API surface.

## 11. Known limitations

- Contact inquiries are stored but not emailed (requires SMTP credentials; follow-up work).
- Seed data is static EF seeding; content edits require a new migration (adequate at this scale).
- Localization is English-only.
- Structured data uses a generic Organization type; no product schema until real product data is signed off.
