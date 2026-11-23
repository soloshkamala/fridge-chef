# 🥗 FridgeChef

FridgeChef is a team-developed web application for managing food stored at home, exploring recipes, maintaining shopping lists, and calculating calorie and macronutrient targets. It consists of a **Blazor WebAssembly** client and an **ASP.NET Core Web API** backend, both targeting **.NET 8**.

> **Development status:** FridgeChef is under active development. This README describes the functionality and implementation in `feature/fridgechef-auth-localization`, not necessarily the current `main` branch. Some features and authentication flows are still being refined.

## Features

| Area | Functionality |
| --- | --- |
| Smart fridge | View, add, update, and remove fridge items; manage available ingredients. |
| Recipes | Browse recipes, inspect ingredients, explore recipes based on available food, and work with saved recipe collections. |
| Shopping list | Create, view, update, and delete items; track purchased and fridge status. Products, categories, and measurement units are validated against the catalog. |
| Nutrition | Calculate estimated daily calorie and protein/fat/carbohydrate targets using user-provided parameters. |
| Profiles | View and manage user profile information. |
| Authentication | Sign in/out through Microsoft Entra ID with MSAL; authenticated client requests use access tokens. |
| Localization | English (`en-US`) and Ukrainian (`uk-UA`) login resources. Do not assume every page is fully translated. |

## Technology stack

| Layer | Technology |
| --- | --- |
| Client | Blazor WebAssembly, C#, .NET 8, MudBlazor |
| Server | ASP.NET Core Web API, Minimal APIs, C#, .NET 8 |
| Database | PostgreSQL, Entity Framework Core, migrations |
| Identity | Microsoft Entra ID, MSAL, Microsoft Identity Web, JWT Bearer |
| API documentation | Swagger / OpenAPI (when enabled) |
| Tests | xUnit, ASP.NET Core integration testing, Testcontainers for PostgreSQL; frontend test project |
| Collaboration | Git, GitHub, feature branches, pull requests |

## Repository layout

```text
fridge-chef/
├── FridgeChef/                  # Blazor WebAssembly client
│   ├── Components/
│   ├── Layout/
│   ├── Models/
│   ├── Pages/
│   ├── Resources/              # Localized login resources
│   ├── Services/
│   └── wwwroot/
├── FridgeChef.Api/              # ASP.NET Core API
│   ├── Data/                   # DbContext and entities
│   ├── Features/               # Feature-oriented endpoints and handlers
│   ├── Migrations/
│   ├── Middleware/
│   ├── Services/               # Including CurrentUserService
│   └── Program.cs
├── Server.Tests/                # Backend unit/integration tests
├── FridgeChef.Tests/            # Frontend tests
└── README.md
```

## Local development

### Prerequisites

- .NET SDK 8
- Git
- PostgreSQL (locally installed or via Docker Desktop)
- Docker Desktop running **when executing Testcontainers integration tests**
- Access to the appropriate Microsoft Entra ID application registrations to exercise Microsoft sign-in
- Optional: `dotnet-ef` CLI for applying migrations

### 1. Clone and restore

```powershell
git clone https://github.com/soloshkamala/fridge-chef.git
cd fridge-chef
git switch feature/fridgechef-auth-localization
dotnet restore
```

The branch switch is needed only while these changes have not been merged into `main`.

### 2. Configure PostgreSQL

Run a PostgreSQL server and configure the API's `ConnectionStrings:DefaultConnection`. For example, use .NET user secrets from the repository root:

```powershell
dotnet user-secrets init --project .\FridgeChef.Api\FridgeChef.Api.csproj
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=fridgechef;Username=postgres;Password=YOUR_LOCAL_PASSWORD" --project .\FridgeChef.Api\FridgeChef.Api.csproj
```

Replace `YOUR_LOCAL_PASSWORD` with your local database password. Ensure port `5432` is actually served by the intended PostgreSQL instance; a Windows PostgreSQL service and a Docker container cannot both bind the same host port simultaneously.

Install the EF CLI if needed and apply the existing migrations:

```powershell
dotnet tool install --global dotnet-ef --version 8.0.11
dotnet ef database update --project .\FridgeChef.Api --startup-project .\FridgeChef.Api
```

If `dotnet-ef` is already installed, skip installation or update it to a compatible EF Core 8 version. Never commit passwords, tokens, or client secrets.

### 3. Configure Microsoft Entra ID

The Microsoft login flow requires **separate client (SPA) and API registrations**, configured consistently:

1. Register the Blazor SPA and configure its local redirect URI(s).
2. Register the backend API and expose the delegated scope `access_as_user`.
3. Configure the SPA to request that API scope and use the correct tenant/client identifiers.
4. Configure the API to validate tokens intended for **this API** and the correct issuer/tenant.
5. Define and assign the `User` and/or `Admin` application roles to appropriate test accounts, where required by protected operations.
6. Match the actual configuration keys and callback routes used in the client and API project settings. Do not copy placeholder IDs into production settings.

`ClientId` and `TenantId` are identifiers, not client secrets. Do not put a confidential client secret in the browser application.

### 4. Run the API and client

Start each process in a separate terminal:

```powershell
# Terminal 1 — API
dotnet run --project .\FridgeChef.Api --urls http://localhost:5039
```

```powershell
# Terminal 2 — client
dotnet run --project .\FridgeChef --urls http://localhost:5050
```

Local addresses used in development:

- Client: `http://localhost:5050`
- API: `http://localhost:5039`
- Swagger: `http://localhost:5039/swagger` **if enabled in the current environment**

The client API base URL, Entra redirect URI, and backend CORS allowlist must match the actual local addresses. Starting both processes does not by itself configure Microsoft Entra ID.

## Authentication and authorization

### Microsoft Entra ID flow

1. A user signs in through Microsoft Entra ID in the Blazor client.
2. MSAL obtains an **access token for the FridgeChef API**.
3. The client sends that token as `Authorization: Bearer <access_token>` to API requests requiring authentication. Tokens should not be attached to unrelated third-party requests.
4. The API validates the JWT and applies the endpoint's authorization policy.
5. `GET /api/profiles/me` can resolve the signed-in Microsoft identity and create a corresponding PostgreSQL user record on first use.
6. `CurrentUserService` maps the validated identity to the application's internal user ID for user-specific operations.

The database user is associated with `EntraObjectId` and `EntraTenantId`. The `AddEntraIdentityToUsers` migration adds these nullable fields and a unique index for their combination, preserving compatibility with existing user rows.

### API policies

| Policy | Requirement |
| --- | --- |
| `ApiRead` | Authenticated request with delegated scope `access_as_user` |
| `ApiWrite` | `access_as_user` plus the `User` or `Admin` app role |
| `ApiAdmin` | `access_as_user` plus the `Admin` app role |

The current branch updates authorization on endpoints in the users, profiles, courses, products, fridge items, recipes, and shopping-list feature areas. **Authorization must be checked per endpoint:** defining a policy does not automatically protect every route. Role and scope checks are distinct; a valid login alone does not grant write/admin access.

### Legacy authentication

Earlier email/password and cookie-session routes remain in the codebase for compatibility. They are separate from the Microsoft Entra JWT flow and should not be treated as interchangeable:

| Method | Route | Purpose |
| --- | --- | --- |
| `POST` | `/api/users` | Legacy registration route |
| `POST` | `/api/auth/login` | Legacy email/password login |
| `GET` | `/api/auth/session` | Legacy session check |
| `POST` | `/api/auth/logout` | Legacy logout |

Verify the authorization requirements of each legacy route before using it in a deployment. The long-term relationship between the two sign-in mechanisms remains an area for cleanup.

## Database and catalog validation

The API uses `FridgeChefDbContext`, PostgreSQL, and EF Core migrations. Shopping-list creation and updates resolve a product by **product name and category**, resolve a measurement unit by name or short name, and require a matching product–unit association in `ProductUnits`. A product name not present in the catalog, an unknown unit, or a disallowed product/unit combination cannot be saved as a shopping item.

Shopping-list records are associated with the authenticated application's user ID. The shopping-list API also uses SignalR notifications for changes; the corresponding hub and user-group behavior should be considered when testing real-time UI updates.

## Localization

Login-page localization resources are maintained in:

```text
FridgeChef/Resources/LoginResource.cs
FridgeChef/Resources/LoginResource.en-US.resx
FridgeChef/Resources/LoginResource.uk-UA.resx
```

The branch includes updates to the login page and its styling. Language coverage elsewhere in the application may be partial; do not assume full localization of every component.

## Tests

Run all backend tests:

```powershell
dotnet test .\Server.Tests\Server.Tests.csproj --no-restore
```

For a first-time checkout, run `dotnet restore` first, or omit `--no-restore`. Integration tests use **Testcontainers PostgreSQL**, so Docker Desktop must be available and running. Tests use an isolated temporary database and a test authentication setup rather than requiring an interactive Microsoft login.

To run only the shopping-list lifecycle test:

```powershell
dotnet test .\Server.Tests\Server.Tests.csproj --filter "FullyQualifiedName~ShoppingItem_CreateGetUpdateDelete_Works"
```

**Latest verified local result (23 November 2026):** `Server.Tests` — **33 passed, 0 failed, 0 skipped** (approximately 40 seconds). This is a local test result for the feature branch, not a claim that CI has passed or that every project in the repository has been tested.

The shopping-list integration test exercises **create → get → list → update → delete → not found**. Its test database seeds a user identity, sample category, two sample products, a unit, and valid product–unit associations so the test respects the real catalog rules.

Additional tests cover authentication/authorization, profiles, users, and course operations. Run the full solution test suite when making cross-project changes:

```powershell
dotnet test
```

## Security and development notes

- Keep connection strings containing passwords, secrets, and tokens out of Git.
- Access tokens must target the API; tokens issued for another audience must be rejected.
- Protected writes require the configured role as well as the required delegated scope.
- Ensure operations on user-specific resources use the authenticated user's identity, not an arbitrary client-supplied user ID.
- CORS origins, Entra redirect URIs, and API addresses must be aligned for each environment.
- Temporary files (`*.backup`, `*.failed`) and local diagnostic pages should not be included in pull requests.

## Next steps

- Audit remaining endpoints and legacy authentication flows for consistent policy and ownership enforcement.
- Expand localization beyond the login page.
- Continue improving recipe discovery and saved-recipe experiences.
- Add automated CI checks for build and tests, if not already configured.
- Extend coverage for access-control edge cases and user-specific data isolation.

## Contributing

The project is developed collaboratively. Changes are made in feature branches, reviewed through GitHub pull requests, and merged into `main` after review and testing.
