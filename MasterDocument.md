# DELMONDES CODELABS - GESTÃO COMERCIAL & PDV
## MASTER CONTEXT & ARCHITECTURE DOCUMENT

### 1. SYSTEM OVERVIEW
* **Client Niche:** Commercial Management and Point of Sale (POS), initially modelled for an "Adega" (Liquor Store/Convenience), requiring robust inventory tracking, service compositions (e.g., Hookah/Narguilé), and operational expense tracking (e.g., Pool Table rentals).
* **Core Philosophy:** High performance, transactional safety, Mobile-First UI, and strict architectural boundaries.
* **Current Status:** Fase 9.4 (1 de 4) - PWA Manifest setup & Docker Tunneling.
* **Version:** Site version refers to May 2026 (incorporating new pages and deprecating legacy ones).

### 2. ARCHITECTURE & TECH STACK
* **Backend:** C# / .NET 10.0.
* **Frontend:** Blazor Web App (Interactive Server) + MudBlazor (Mobile-First UI, strict Material Design).
* **Database:** PostgreSQL orchestrated via Docker.
* **Data Access (CQRS Pattern):**
  * **Commands (Writes):** Entity Framework Core (v10.0.10). Strictly used for Inserts, Updates, and Deletes to guarantee transactional integrity and constraint validation.
  * **Queries (Reads):** Dapper (v2.1.79). Strictly used for high-performance data retrieval using pure SQL scripts, avoiding memory overhead.
* **Environment & Infrastructure:** 
  * Docker v4.86.0.
  * Development and mobile testing are exposed via **Dockerized Tunnels** (Cloudflare Dev Tunnels and Ngrok as fallback).
  * Production Domain: `adega.delmondescodelabs.dpdns.org`.
  * Email Routing: `contato@delmondescodelabs.dpdns.org` and `suporte@delmondescodelabs.dpdns.org`.
  * The system implements a `manifest.json` and icons to allow "Add to Home Screen" native-like PWA behaviour (requires an active connection as it relies on SSR).
* **Quality Assurance & Testing Stack:**
  * **Framework:** xUnit.
  * **Assertions:** FluentAssertions (Strict adherence to the Arrange-Act-Assert pattern with fluent syntax).
  * **Mocking & Fakes:** NSubstitute for service mocking; Bogus for relational data seeding (Users, Sales, Products).
  * **E2E & UI Testing:** Microsoft Playwright. Tests must target the local Docker port (`localhost:8080`) to validate Blazor Server interactivity and MudBlazor DOM rendering.

### 3. DELMONDES CODELABS STRICT CODING GUIDELINES
* **Language:** Explanations in Portuguese. Code and comments strictly in **British English**.
* **Structure (C#):** C# code must ALWAYS be divided into `#region` blocks with short, self-explanatory names. **Crucial:** The region name MUST be replicated at the end of the block (e.g., `#region Properties` ... `#endregion Properties`). 
* **Startup/DI:** `Program.cs` MUST be kept lean. All service registrations, pipeline configurations, and dependency injections must be delegated to `Setup.cs`.
* **Structure (Razor):** NEVER use `#region` blocks inside the HTML markup of Razor components. Regions are exclusively allowed within the `@code` block.
* **Typing:** Strong typing only. Never use `dynamic` in Blazor/C# or `any` in frontend contexts.
* **Comments:** XML documentation comments `///` are mandatory for classes, interfaces, and methods. NEVER use inline comments `//` or `--` inside code or SQL strings (especially Supabase).
* **Blazor Forms:** Every `<EditForm>` MUST include a `FormName` attribute with a unique value to prevent .NET routing/postback errors during WebSocket disconnections.
* **Entity Framework in Blazor Server:** To prevent tracking collisions and memory leaks inherent to long-lived SignalR circuits, all Write Repositories MUST use `IDbContextFactory<T>` to create short-lived contexts (`await using var context = await _factory.CreateDbContextAsync()`).

### 4. CRITICAL INFRASTRUCTURE & SSR TRAPS (DO NOT IGNORE)
* **Identity & Static SSR:** The Identity area (`/Account`) runs strictly in **Static SSR** mode. MudBlazor input components (`MudTextField`, `MudCheckBox`, `MudButton`) fail silently, lose interactions, or overlap labels in SSR. You MUST use native HTML inputs (`<InputText>`, `<input type="checkbox">`, `<button type="submit">`) styled with MudBlazor CSS variables (`var(--mud-palette...)`).
* **Antiforgery Tokens in SSR (The Golden Rule):** 
  * **If using `<EditForm method="post">`:** Blazor automatically generates and injects the token. **NEVER** manually add `<AntiforgeryToken />` inside it. Doing so duplicates the hidden input (`__RequestVerificationToken`), resulting in a comma-separated string that fails backend decryption and causes crash errors.
  * **If using native HTML `<form method="post">`:** (e.g., Logout, ExternalLogins) You MUST manually include `<AntiforgeryToken />`, add `data-enhance="false"`, and use a native `<button type="submit">`.
* **Docker & Data Protection ("Remember Me" Bug):** To ensure authentication cookies survive Docker container restarts, the application MUST use `AddDataProtection().PersistKeysToFileSystem()`. The Docker Compose file MUST map a persistent volume (e.g., `dcl-keys:/app/keys`), and the `Dockerfile` MUST grant folder ownership to the non-root `$APP_UID` before execution to avoid `UnauthorizedAccessException`.

### 5. LOCALISATION RULES
* Always inject `@inject IStringLocalizer<DelmondesCodeLabs_GestaoComercial.Resources.App> StrLocalizer` in Razor pages.
* The system defaults strictly to `pt-BR`. Automatic browser culture fallback is disabled.
* **Translation Casing (CRUCIAL):** 
  * `Key` (and `en-US`): Can be Pascal Case or Title Case (e.g., `Reset Authenticator Key`).
  * `Neutral Value` / `pt-BR`: MUST strictly follow Portuguese grammar rules for **Sentence case** (e.g., `Redefinir chave do autenticador`). NEVER use Title Case for Portuguese values.
* **Translation Table Format:** Document new strings in a 4-column table: `Key` | `Neutral Value (pt-BR)` | `en-US` | `pt-BR`.

### 6. DOMAIN ENTITIES, ENUMS & BUSINESS RULES (THE MAP)
*All entities inherit from `AuditableEntity` (Id, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted, Version).*

#### A. Core Catalog
* **`ProductType` (Enum):** `Standard = 1`, `Service = 2`, `Component = 3`.
* **`Category`**: `Name`, `Description`, `IsActive`.
* **`Product`**: `Sku`, `Barcode`, `Name`, `Description`, `CostPrice`, `SellingPrice`, `CurrentStock`, `MinimumStockAlert`, `IsActive`, `Type` (ProductType), `CategoryId` (FK to Category).
  * *Rule:* Profit margins are calculated dynamically on the UI. `MinimumStockAlert` triggers BI warnings.

#### B. Inventory & BOM Engine
* **`MovementType` (Enum):** `Entry = 1`, `Sale = 2`, `Loss = 3`, `InternalConsumption = 4`.
* **`ProductComposition`**: `ParentProductId` (FK to Service Product), `ChildProductId` (FK to Component Product), `Quantity`.
  * *Rule:* "Hookah/Narguilé" mechanic. A service sold at the POS automatically deducts components (coal, foil, essence) from inventory based on this BOM.
* **`Supplier`**: `Name`, `Cnpj`, `ContactEmail`, `ContactPhone`.
* **`StockEntry`**: `SupplierId` (FK), `InvoiceNumber`, `EntryDate`, `TotalCost`.
* **`StockEntryItem`**: `StockEntryId` (FK), `ProductId` (FK), `Quantity`, `UnitCost`.
* **`StockMovement`**: `ProductId` (FK), `Quantity`, `Type` (MovementType), `Reason`, `MovementDate`.
  * *Rule:* Absolute traceability. `Product.CurrentStock` is merely a cached projection of these immutable movements.

#### C. Financial Engine
* **`ExpenseCategory` (Enum):** `Operational = 1`, `Rent = 2`, `Utilities = 3`, `Payroll = 4`, `Maintenance = 5`, `Taxes = 6`, `Other = 99`.
* **`Expense`**: `Description`, `Amount`, `DueDate`, `IsPaid`, `PaymentDate`, `Category` (ExpenseCategory).
  * *Rule:* "Pool Table/Sinuca" mechanic. Tracks fixed/variable costs so the BI Dashboard can calculate True Net Profit.

#### D. Pricing & Promos
* **`DiscountType` (Enum):** `Percentage = 0`, `FixedValue = 1`.
* **`Promotion`**: `Name`, `StartDate`, `EndDate`, `DiscountType`, `DiscountValue`, `IsActive`. 
* **`PromotionItem`**: `PromotionId` (FK), `ProductId` (Nullable FK), `CategoryId` (Nullable FK).
  * *Rule:* Automates POS discounts. Supports percentage and fixed monetary value options.

#### E. Point of Sale (POS)
* **`PaymentMethod` (Enum):** `Cash = 1`, `Pix = 2`, `CreditCard = 3`, `DebitCard = 4`, `Pending = 99`.
* **`SaleStatus` (Enum):** `Pending = 1` (Comanda Aberta), `Completed = 2`, `Cancelled = 3`.
* **`SaleItemStatus` (Enum):** `Active = 1`, `Canceled = 2`, `Damaged = 3` (Avaria).
* **`Sale`**: `ReceiptNumber`, `CustomerIdentifier`, `SaleDate`, `SubTotal`, `DiscountAmount`, `TotalAmount`, `PaymentMethod`, `Status`.
* **`SaleItem`**: `SaleId` (FK), `ProductId` (FK), `Quantity`, `UnitPrice`, `DiscountAmount`, `Total`, `Status` (SaleItemStatus).
  * *Rule (Open Tab):* Stock (`MovementType.Sale`) is deducted IMMEDIATELY when an item is saved to an open tab (Pending Sale).
  * *Rule (Exceptions):* If a `SaleItem` is marked as `Canceled`, stock is physically returned (`MovementType.Entry`). If marked as `Damaged`, it remains out of stock but is logged as `MovementType.Loss` for BI reporting. This applies recursively to BOM components (Hookah essence, coal, etc.).

#### F. Access Control (RBAC)
* **`SuperUser`**: The ultimate owner. Full system access. The *only* role authorized to grant or revoke `Admin` access to other users.
* **`Admin`**: Store Manager. Full operational access (BI, Inventory, Finance, POS, Catalog, Promos). Cannot manage SuperUser/Admin roles.
* **`Cashier`**: Full access to the POS domain (Terminal and Sales History). Authorized to settle tabs and process payments.
* **`Waiter`**: Restricted POS access. Authorized to launch products into pending tabs and view history for reference. Strictly prohibited from settling tabs (receiving payments) or deleting historical sales.

### 7. GRANULAR ROADMAP & PHASES
* [x] **Phase 1: Foundation & Setup**
* [x] **Phase 2: Security & Access**
* [x] **Phase 3: Core Catalog**
* [x] **Phase 4: Inventory Engine**
* [x] **Phase 5: Financial Engine**
* [x] **Phase 6: Pricing & Promotions**
* [x] **Phase 7: Point of Sale (POS)**
* [x] **Phase 8: BI Dashboards**
* [x] **Phase 9: Security Polish & Infrastructure (RBAC)**
  * [x] 9.1 Database Seeder for Roles (SuperUser, Admin, Cashier, Waiter).
  * [x] 9.2 Route & UI Protection (`[Authorize]` attributes and `<AuthorizeView>`).
  * [x] 9.3 Translate scaffolded Identity pages to Portuguese (MudBlazor UI + SSR Native Form Bypass).
  * [x] 9.4 PWA Manifest setup & Docker Tunneling (Ngrok/Cloudflare).
  
* [x] **Phase 10: Quality Assurance (QA) & Stress Testing**
  * [x] **10.1 Data Seeding Engine (Bogus):** Robust mock data generator populating Tier 1 through 6.
  * [x] **10.2 In-Memory Transaction Integrity:** Validated CQRS deductions, returns, and losses.
  * [x] **10.3 Docker E2E Database Injection:** Automated population of the local PostgreSQL container.
  * [ ] **10.4 BI & POS Manual Stress Test:** Validate UI responsiveness, charting speed, and Dapper query execution times with thousands of records loaded in the Dev environment.

* [ ] **Phase 11: SaaS Security Lockdown & Team Management**
  * [x] **11.1 Public Registration Lockdown:** Restrict or entirely remove the public `/Account/Register` route to prevent unauthorised account creation.
  * [x] **11.2 SuperUser Team Dashboard (`/admin/team`):** Build a dedicated RBAC management interface.
    * *SuperUser Privilege:* Can create, edit, and revoke access for Admin, Cashier, and Waiter roles.
    * *Admin Privilege:* Can strictly manage Cashier and Waiter roles.
    * Implement temporary password generation for new staff.
  * [-] **11.3 External SSO Providers:** Integrate Google and Microsoft Authentication in `Setup.cs` for frictionless login, restricted to approved domains/emails if necessary.[Cancelled]

* [ ] **Phase 12: Commercial Landing Page & Legal Hub**
  * [x] **12.1 Home Page Revamp (`Index.razor`):** Transform the blank welcome page into a high-converting SaaS Landing Page.[Cancelled - already added in Admin/Plans page]
    * Add Hero Section with value proposition.
    * Add highly visual Pricing/Plan Cards using MudBlazor (`MudCard`), detailing included features per tier.
  * [x] **12.2 Contact & Support Routing:** Implement "Contact Us" components linking directly to `mailto:contato@delmondescodelabs.dpdns.org` and `mailto:suporte@delmondescodelabs.dpdns.org`.
  * [ ] **12.3 Legal & Compliance Documents:** Create professional static pages for:
    * Software License Agreement.
    * Terms of Use.
    * Security & Privacy Policies.
  * [x] **12.4 Domain Fallback Protocol:** Document the load-balancing strategy (Cloudflare -> Ngrok -> Localhost) for end-users to ensure high availability during presentations.

* [ ] **Phase 13: Production Communication Engine**
  * [ ] **13.1 SMTP Integration:** Replace the `IdentityNoOpEmailSender` with a real SMTP provider implementation (e.g., MailKit) to handle password resets and email confirmations.
  * [ ] **13.2 Internal Help Desk (Optional):** Build a lightweight ticketing system inside the app for users to report bugs or request assistance directly to the SuperUser.