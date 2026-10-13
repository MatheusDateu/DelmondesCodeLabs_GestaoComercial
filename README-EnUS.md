# Delmondes CodeLabs - Commercial Management & POS

<p align="center">
  <b>High-Performance System for Commercial Management, POS, and Inventory Control</b><br>
  <i>Developed by Delmondes CodeLabs</i>
</p>

---

## 1. System Overview
The **Delmondes CodeLabs - Commercial Management & POS** is a high-performance system initially modeled for the niche of beverage stores (adeGAS), convenience stores, and related retail businesses. The platform offers robust inventory control, service compositions (such as hookah sessions with automatic deduction of supplies), operational expense tracking, and an agile Point of Sale (POS) supporting open tabs.

---

## 2. Architecture and Tech Stack
* **Backend:** C# / .NET 10.0.
* **Frontend:** Blazor Web App (Interactive Server) + MudBlazor (Mobile-First UI in strict Material Design).
* **Database:** PostgreSQL orchestrated via Docker.
* **Data Access Pattern (CQRS):**
  * **Commands (Writes):** Entity Framework Core (v10.0.10) strictly used for inserts, updates, and deletes to guarantee transactional integrity.
  * **Queries (Reads):** Dapper (v2.1.79) strictly used for high-performance data retrieval using pure SQL, avoiding memory overhead.

---

## 3. Core Modules and Business Rules
* **Product Catalog:** Management of categories, unique SKUs, barcodes, cost/selling prices, and minimum stock alerts.
* **Inventory & Composition Engine (BOM):** Absolute traceability through immutable stock movements (`StockMovements`). Selling services (such as Hookah) automatically deducts associated physical components (essence, coal, foil).
* **Financial Module:** Tracking of fixed and variable expenses (rent, utility bills, maintenance) for accurate Net Profit calculation in the BI Dashboard.
* **Pricing & Promotions:** Support for percentage and fixed monetary value discounts linked to specific products or categories.
* **Point of Sale (POS):** Counter operations and open tabs. Items added to tabs deduct stock immediately; cancellations return items to physical stock, while damages log losses for managerial reports.
* **Business Intelligence (BI):** Dynamic dashboards analyzing sales ABC curves, gross revenue, COGS (CMV), paid expenses, and net profit margins.

---

## 4. Role-Based Access Control (RBAC)
The system implements four hierarchical security levels:
* **SuperUser:** Supreme owner with full access and exclusivity to manage administrative roles.
* **Admin:** Store manager with full operational access (BI, Inventory, Finance, POS, Catalog, and Promotions), without permission to manage SuperUsers.
* **Cashier:** Full access to the POS terminal and sales history for closing accounts.
* **Waiter:** Restricted access to add products into open tabs, without permission to receive payments or delete history.

---

## 5. Prerequisites for Local Execution
To run the system in your development environment, make sure you have the following software installed:
1. **Docker Desktop** (with Linux containers support enabled).
2. **.NET 10.0 SDK** (or higher).
3. A compatible IDE (Visual Studio 2026 / Visual Studio Code / Rider).

---

## 6. Local Environment Configuration
The system uses environment variables injected via Docker Compose. Follow the steps below to configure your environment:

1. At the root of the project (where the `docker-compose.yml` file is located), create a file named **`.env`**.
2. Paste the following pre-defined configuration content into the `.env` file:

```env
POSTGRES_USER=root
POSTGRES_PASSWORD=S3nh4.Development@3nv1r0nm3nt
DB_USER=root
DB_PASSWORD=S3nh4.Development@3nv1r0nm3nt
CLOUDFLARE_TOKEN=eyJhIjoiYjM0ZmFiOTFmODY5NmI5ODFlMDEwMjNhNTU2ZDVhMzMiLCJ0IjoiNGYzOTJkYjMtNzQyYi00OWZjLTkwNzAtODk3OTMzZmFkZWY5IiwicyI6IlpXTmxZek5sWlRBdE16SXlZaTAwT0RNeExXRXdNMkl0TldaaFlUUXpOVEEyTUdFeSJ9
NGROK_TOKEN=3HkuvqDAz9mQUnSZB6bBcnDhwlb_7ghwwLTzyA8vmY5rHUXr9
```

---

## 7. How to Run the System via Command Line (CLI)

With Docker Desktop open and the `.env` file configured at the root, run the commands below in your terminal (PowerShell, Bash, or CMD):

### To build and start the application and database in the background:
```bash
docker compose up -d --build
```

### To monitor execution logs:
```bash
docker compose logs -f delmondescodelabs_gestaocomercial
```

### To stop containers while preserving database data:
```bash
docker compose down
```

### To stop containers and completely wipe data volumes (General Reset):
```bash
docker compose down -v
```

Once the containers are running, access the application in your browser through the mapped port or the configured tunnel at:
* `https://localhost:8081` or `http://localhost:8080`

---

## 8. Licensing and Terms of Use

This project is distributed under the terms of the **Apache License 2.0** (refer to the [LICENSE.txt](LICENSE.txt) file for further details).

* **What is permitted:** You have complete freedom to fork, study the source code, modify, adapt, and run the application locally for testing, evaluation, interviews, or internal operational use.
* **What is prohibited:** It is strictly prohibited to **commercialize, resell, re-license under third-party brands, or directly distribute paid commercial versions** of the software or derivative works without the express authorization of **Delmondes CodeLabs**.