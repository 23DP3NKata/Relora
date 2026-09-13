# Relora

Relora is a fashion/resale auction marketplace. Sellers publish lots, buyers bid in real time, and the backend creates an order after a completed auction.

## Stack

- Backend: .NET 8, ASP.NET Core, Entity Framework Core, PostgreSQL, MediatR, SignalR
- Storefront: Vue 3, TypeScript, Vite
- Media: Cloudflare R2 (S3-compatible API)
- Payments: Stripe

## Repository layout

```text
backend/
  host/Relora.Host/             API entry point, middleware, background jobs and Docker Compose
  modules/                      Business modules: identity, items, auctions, bids, orders, admin, support
  persistance/                  EF Core DbContext, configurations and migrations
  realtime/                     SignalR hub and notifications
  shared/                       Shared abstractions and infrastructure
  tests/                        Domain tests
frontend/
  relora-frontend/              Public Vue storefront
  relora-admin/                 Vue administration interface
```

## Prerequisites

- .NET SDK 8
- Node.js 20 or newer
- Docker Desktop

## Local setup

### 1. Configure PostgreSQL

```powershell
cd backend/host/Relora.Host
Copy-Item .env.example .env
```

Set a local `POSTGRES_PASSWORD` in `.env`, then start PostgreSQL:

```powershell
docker compose up -d
```

### 2. Configure and run the API

```powershell
Copy-Item appsettings.Development.example.json appsettings.Development.json
```

Keep only non-secret local settings in `appsettings.Development.json`. Configure credentials with .NET User Secrets; `user-secrets.example.json` lists all required secret keys:

```powershell
dotnet user-secrets set "Jwt:Secret" "replace-with-a-long-random-secret"
dotnet user-secrets set "ConnectionStrings:Postgres" "Host=localhost;Port=5433;Database=relora;Username=postgres;Password=change-me"
dotnet user-secrets set "R2:AccessKeyId" "replace-with-r2-access-key-id"
dotnet user-secrets set "R2:SecretAccessKey" "replace-with-r2-secret-access-key"
dotnet user-secrets set "Stripe:SecretKey" "sk_test_replace_me"
dotnet user-secrets set "Stripe:WebhookSecret" "whsec_replace_me"
dotnet user-secrets set "Email:Username" "replace-me"
dotnet user-secrets set "Email:Password" "replace-me"
```

Use the same PostgreSQL password as `.env`, generate a long random JWT secret, and configure a real R2 endpoint. The R2 `ServiceUrl` must be exactly:

```text
https://<CLOUDFLARE_ACCOUNT_ID>.r2.cloudflarestorage.com
```

Apply migrations and run the API:

```powershell
dotnet ef database update --project ../../persistance/Relora.Persistance/Relora.Persistance.csproj --startup-project Relora.Host.csproj
dotnet run --project Relora.Host.csproj
```

The API is available on the URLs reported by ASP.NET Core; Swagger is enabled in Development.

### 3. Run the storefront

```powershell
cd ../../../frontend/relora-frontend
Copy-Item .env.example .env
npm ci
npm run dev
```

Set `VITE_API_BASE_URL` to the backend URL, `VITE_SITE_URL` to the storefront URL, and `VITE_MEDIA_PUBLIC_BASE_URL` to the public R2/custom media URL. Values prefixed with `VITE_` are public and must never contain secrets.

### 4. Run the admin interface

```powershell
cd ../relora-admin
Copy-Item .env.example .env
npm ci
npm run dev
```

## Configuration and secrets

Local `.env` files, real `appsettings` files, certificates, Node modules, and build output are excluded by `.gitignore`. Docker also excludes real `appsettings` files so credentials cannot be baked into an image. Commit only the supplied `*.example` files. Use your deployment platform's secret store or runtime environment variables for production credentials.

Required external configuration:

- Cloudflare R2: bucket, account ID, S3 access keys, public media URL
- Stripe: API secret key, webhook secret, publishable key, checkout URLs
- SMTP: host and credentials for transactional email
- PostgreSQL: connection string and database credentials

## Verification

```powershell
dotnet build backend/Relora.sln --no-restore
dotnet test backend/tests/Relora.Domain.Tests/Relora.Domain.Tests.csproj --no-build --no-restore
cd frontend/relora-frontend; npm run build
cd ../relora-admin; npm run build
```

## License

Proprietary. See [LICENSE](LICENSE).
