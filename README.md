# Car Stock API

A car stock management system for dealers, built with C# (FastEndpoints) and Svelte.

## Tech Stack

### Backend

- .NET 10 / C#
- FastEndpoints
- Dapper (raw SQL queries)
- SQLite
- JWT Authentication (FastEndpoint.Security)
- BCrypt password hashing

### Frontend

- Svelte 5
- Vite
- TailwindCSS v4

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js v20+

### Running the Backend

```bash
cd CarDealerApi
dotnet run
```

The API will start at `http://localhost:5150`.
The SQLite database is created and seeded automatically on first run.

### Running the Frontend

```bash
cd CarDealerApp
npm install
npm run dev
```

The frontent will start at `http://localhost:5173`

---

## Test Accounts

Two dealer accounts are seeded automatically:

| DealerName | Username | Password |
| --- | --- | --- |
| Melbourne Auto Group | melbourne | dealer123 |
| Sydney Car World | sydney | dealer123 |

Each account has 5 cars pre-loaded. Log in as both to verify dealer isolation.
Each dealer can only see and manage their own inventory.

---

All car endpoints require a Bearer token obtained from `/auth/login`.

### Auth

| Method | Route | Description |
| --- | --- | --- |
| POST | `/auth/register` | Register a new dealer account |
| POST | `/auth/login` | Login and receive a JWT token |

### Cars

| Method | Route | Description |
| --- | --- | --- |
| GET | `/cars` | List all cars for the authenticated dealer |
| GET | `/cars/{id}` | Get a single car by ID |
| POST | `/cars` | Add a new car |
| PATCH | `/cars/{id}/stock` | Update stock level for a car |
| DELETE | /cars/{id} | Delete a car |
| GET | /cars/search | Search cars by make and/or model |

#### Search example

```
GET /cars/search?make=toyota&model=corolla
```

Both parameters are optional. Supports partial matches. For example, searching `toy` will match `Toyota`.

---

## How It Works

### Authentication & Dealer Isolation

Each dealer registers an account and logs in to receive a JWT token. The token contains the dealer's ID as a claim. Every car endpoint reads this claim to scope all database queries so a dealer can only read or modify their own cars. Attempting to access another dealer's car returns a `404`.

### Database

SQLite is used with a local database file at `Data/database.db`. The database is initialised and seeded automatically on startup via DBInitializer. Dapper is used for all queries with raw SQL.

### Validation

Request validation is handled by FastEndpoints' built-in FluentValidation integration. Invalid requests return a `400` with a descriptive error message per field.

### Password Security

Passwords are hashed with BCrypt before storage. Plain text passwords are never stored.

---

## Notes & Design Decisions

- **Update endpoint is scoped to stock level only** as per the requirements. A full car update endpoint could easily be added if needed.
- **`GET /cars/{id}`** is implemented on the backend as a REST practice, though the frontend uses the list endpoint for the dashboard view.
- **Pagination** was considered but not implemented as it was not in the requirements, but could be added to `GET /cars` using `LIMIT` and `OFFSET` in the SQL query.
- **Swagger UI** is available at `https://localhost:5150/swagger` for testing the API directly.
