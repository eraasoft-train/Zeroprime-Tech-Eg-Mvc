# Project Structure

## Tree (source only)

```text
Zeroprime-Tech-Eg-Mvc/
├── .github/
│   ├── workflows/
│   │   ├── ci.yml                  # restore · build · test · format (main + feature/**)
│   │   └── docker.yml              # GHCR image build; push on main only
│   ├── ISSUE_TEMPLATE/             # bug_report.md, feature_request.md, config.yml
│   └── PULL_REQUEST_TEMPLATE.md
├── docs/
│   ├── architecture/
│   │   ├── REPOSITORY_AUDIT.md     # state before transformation + decisions
│   │   ├── MIGRATION_PLAN.md       # phased plan that was executed
│   │   └── SYSTEM_ARCHITECTURE.md  # the as-built architecture
│   ├── research/
│   │   └── ZEROPRIME_COMPANY_RESEARCH.md   # every content claim, with sources
│   ├── development/
│   │   ├── DEVELOPMENT_GUIDE.md
│   │   ├── TESTING_GUIDE.md
│   │   ├── DEPENDENCIES.md
│   │   └── PROJECT_STRUCTURE.md    # this file
│   └── deployment/
│       └── DEPLOYMENT_GUIDE.md
├── src/
│   ├── Zeroprime.Domain/
│   │   ├── Common/BaseEntity.cs    # Id, CreatedAt, UpdatedAt + SetUpdatedAt()
│   │   ├── Entities/               # CompanyProfile, Product, ContactInquiry,
│   │   │                           #   SocialMediaLink, SiteSetting
│   │   └── Enums/InquiryStatus.cs
│   ├── Zeroprime.Application/
│   │   ├── Abstractions/           # Repository ports (I*Repository)
│   │   ├── Common/CacheKeys.cs     # 5-minute TTL in-memory keys
│   │   ├── DependencyInjection/ApplicationServiceExtensions.cs
│   │   └── Features/
│   │       ├── Company/            # ICompanyProfileService + DTO + impl
│   │       ├── Contact/            # IContactInquiryService + request DTO + result + validator
│   │       ├── Products/           # IProductService + ProductDto + impl (JSON feature lists)
│   │       └── Settings/           # Site settings + social links services/DTOs
│   ├── Zeroprime.Infrastructure/
│   │   └── Persistence/
│   │       ├── Context/            # ZeroprimeDbContext + IDesignTimeDbContextFactory
│   │       ├── Configurations/     # IEntityTypeConfiguration<*> with Fluent API + HasData
│   │       ├── Migrations/         # EF migrations (InitialCreate)
│   │       └── Repositories/       # I-*Repository implementations (AsNoTracking reads)
│   └── Zeroprime.Web/
│       ├── Program.cs              # Pipeline: Serilog → security headers → status pages → static files
│       ├── Dockerfile              # multi-stage, non-root, port 8080
│       ├── libman.json             # vendored frontend libraries (pinned)
│       ├── Controllers/            # Home, About, Products, SmartCities, Contact, Sitemap, Robots
│       ├── ViewComponents/         # SocialLinks, OrganizationSchema (JSON-LD)
│       ├── ViewModels/             # Home/About/Products + StatusCodeViewModel, BreadcrumbItem
│       ├── Middleware/             # SecurityHeadersMiddleware
│       ├── Views/
│       │   ├── Shared/             # _Layout, _Breadcrumb, Error, components/
│       │   ├── Home|About|Products|SmartCities|Contact/
│       └── wwwroot/                # css (design system), js (site.js, contact-form.js),
│                                   #   images (logo.svg, logo-light.svg, canbank.svg, smart-cities.svg),
│                                   #   lib/ (vendored, unmodified licenses)
├── tests/
│   ├── Zeroprime.UnitTests/
│   ├── Zeroprime.ArchitectureTests/
│   └── Zeroprime.IntegrationTests/  # Testcontainers PostgreSQL
├── Zeroprime.sln
├── docker-compose.yml              # postgres (16-alpine) + web, dev defaults
├── CHANGELOG.md
├── CONTRIBUTING.md
└── README.md
```

## Conventions

- **Namespace = folder path.** File-scoped namespaces everywhere; `Nullable=enable` solution-wide.
- **Domain** entities: private parameterized factory (`Create`), invariants enforced there; `internal` mutation methods per use-case (`MarkAsResolved`). EF uses parameterless constructors marked `public` only where EF requires them (they carry no invariants).
- **Application** services return DTOs (`record`s). `CancellationToken` threaded through all public service methods.
- **Infrastructure** owns all SQL concerns (Fluent API, indexes, migrations, seeding). No EF attributes on domain entities.
- **Web** owns HTTP and rendering concerns only. Controllers map to services; views map to ViewModels (records).
- **Assets**: CSS/JS first-party under `wwwroot/css|js`; third-party under `wwwroot/lib/<name>` untouched from upstream distribution; images are hand-authored SVG (no photography because none is verified/licensed for reuse).

## Adding a new page

1. Domain? (usually not) → Application service + DTO → Infrastructure repository if new data → Web controller/action/view → integration test → documentation.
2. Keep the chain linear; never let a view call a repository or a controller re-implement a service.

## Adding content to the site

- Text edits to company content: update the seed data in the corresponding `*Configuration.cs` (EF `HasData`) and add a migration with `dotnet ef migrations add UpdateSeedContent --project src/Zeroprime.Infrastructure --startup-project src/Zeroprime.Web`.
- Re-verify against `docs/research/ZEROPRIME_COMPANY_RESEARCH.md` before changing facts. Do not add unverified claims.
