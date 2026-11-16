# Deployment to MonsterASP.NET

**Architecture check (verified 2026-11-16):** MonsterASP.NET supports **PostgreSQL 18** (offered as *Beta*, including the **Free** tier — one free database, EU datacenter) and **Npgsql works through their PgBouncer pooler without code changes**. This app targets PostgreSQL, so it runs there unchanged.

Sources (MonsterASP.NET help center, reviewed 2026-11-16):
- Databases / PostgreSQL: `help.monsterasp.net/databases/postgresql/{create-database,connect}`
- Environment variables: `help.monsterasp.net/websites/configuration/environment-variables`
- Deploy (Visual Studio / FTP / ZIP / command line): `help.monsterasp.net/websites/deploy`
- Hosting models: `help.monsterasp.net/websites/configuration/hosting-model`

> ⚠️ **Security note — leaked credential:** the repository previously contained a real-looking MonsterASP.NET PostgreSQL string (`db80289.public.databaseasp.net`, `db80289`, password `Z=n37aB#G5-b`) in a local config file. It was **never committed** (verified across git history), and was removed from `appsettings.json` before the first commit of the new app. **Treat that credential as compromised: delete or rotate the `db80289` database and its password.** It does not belong to this codebase and is not used by it.

## What you need

1. A **Free** or **Premium** MonsterASP.NET account and a website (Control Panel → Websites → Create).
2. A **PostgreSQL** database (Control Panel → Databases → Add database → **PostgreSQL** → FREE database). Note: PostgreSQL is in Beta and uses the same wizard as other databases.
3. The database's **Local access** connection string (for a website hosted there) — Control Panel → Databases → your database → **Overview → Local access** → the Npgsql-format string:
   `Server=<local name>;Port=5432;Database=dbXXXX;User Id=dbXXXX;Password=…;`
4. .NET 9 enabled for the website (default on new websites — check **Scripting** / .NET version in the Control Panel).

## Steps

### 1. Prepare the production build

From Visual Studio 2022 (ASP.NET and web development workload):

- Right-click `src/Zeroprime.Web` → **Publish**
- Choose **Folder** or **Web Deploy** (below)
- Configuration **Release**, target .NET 9

Or from the command line:

```bash
dotnet publish src/Zeroprime.Web/Zeroprime.Web.csproj -c Release -o ./artifacts/publish
```

The published output includes `web.config` (auto-generated for IIS) and the self-contained wwwroot assets.

### 2. Publish to the website

Pick **one** deployment method:

| Method | How |
|---|---|
| **Visual Studio Web Deploy** (easiest) | Control Panel → website → **Deploy** → enable **WebDeploy** → **Download publish profile** → VS: right-click project → **Publish** → **Import profile** |
| **GitHub deploy** | Control Panel → website → **Git deploy** → connect repo `eraasoft-train/Zeroprime-Tech-Eg-Mvc`, set working dir to `src/Zeroprime.Web`, build command `dotnet publish -c Release` |
| **FTP/SFTP** | Control Panel → website → **Deploy** → FTP credentials → upload the *contents* of the publish folder into the site root |
| **ZIP upload** | Control Panel → website → **Deploy** → **Upload ZIP** → upload a zip of the publish folder |

### 3. Set the environment variables

Control Panel → website → **Scripting** → **Environment variables** → add:

| Name | Value |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__Postgres` | the **Local access** connection string from step 1 (contains the password — the panel masks it) |
| `Database__MigrateOnStartup` | `true` (optional; default applies migrations at startup) |

Then **Restart application** so they take effect. ASP.NET Core picks them up over `appsettings.json` automatically. Never store the password in the published files.

### 4. HTTPS

Control Panel → website → **Domains & HTTPS** → **Let's Encrypt** → enable. One click, free. Your site is served on `https://<site>.siteasp.net` or your own domain.

### 5. Verify

- `https://<your-site>/health` → `Healthy`
- `https://<your-site>/` → homepage renders
- Submit the contact form once with test data; confirm it redirects (302) and the row is stored (Control Panel → WebPGSQL → browse `ContactInquiries`)
- `https://<your-site>/sitemap.xml` and `/robots.txt` render

The app applies EF Core migrations + seed data automatically on first start (`Database:MigrateOnStartup=true`), so the schema (`CompanyProfiles`, `Products`, `ContactInquiries`, `SocialMediaLinks`, `SiteSettings`) is created and the verified company profile + CanBank product are seeded without manual SQL.

## Good to know / limits

- **Free plan**: 1 website, 5 GB site storage, **256 MB RAM**, 1 database (free slots are EU datacenter only), no custom domain (free subdomain), limited traffic. Perfectly adequate for this brochure site + contact form. The app is small (~40–60 MB under load).
- **Always-on**: an idle application is stopped after inactivity (free/premium default) and cold-starts on the next request. If you want it always warm: Control Panel → **AlwaysRunning** (premium) or a scheduled task pinging `/health`.
- **PgBouncer transaction pooling**: MonsterASP routes PostgreSQL through PgBouncer. Npgsql is fully compatible. Do not enable prepared-statement emulation (that's a PHP-only concern); no code changes required.
- **Connection string format for a website hosted there** is the **Local access** one — it never leaves the datacenter. Use **Remote access** only from your own machine for debugging, with SSL enabled.
- **Memory limit**: if the app keeps restarting on the free plan, check **Configuration → Increase the memory (RAM)** — 256 MB is the free ceiling; upgrade to Premium Single (512 MB, $1.95/mo first year) if needed.
- **Never** publish `appsettings.Development.json` with real data; the repo's committed `appsettings.json` contains only local-dev defaults and is overridden by the environment variables above.
- The **PostgreSQL Beta** status is MonsterASP's label; the hosted engine is PostgreSQL 18.

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| HTTP 500.30 on start | App crashed at boot, usually config/DB | Check **Application & Access logs** in Control Panel; confirm `ConnectionStrings__Postgres` is set and uses the **Local access** string |
| `Password authentication failed` (28P01) | Wrong string (remote vs local), or password typo | Local string goes in the site environment; remote string only from your machine |
| Site works, but contact form fails | Migrations not applied | Ensure `Database__MigrateOnStartup=true` and restart; or apply migrations from a local machine with `dotnet ef database update` against the Remote access string (enable remote access temporarily, then disable it) |
| SSL certificate errors | Let's Encrypt not yet enabled | One-click enable in Domains & HTTPS |
| 429 on contact form | Rate limit works (5 / 10 min / IP) | Expected; do not disable |
