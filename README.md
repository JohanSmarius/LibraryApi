# Library API — Controller-Based Starting Solution

Starting point for the talk *"Refactoring an ASP.NET Core WebAPI Controller
based API to a minimal API"*. This is the **before** state — a working,
realistic, deliberately unlayered controller-based Web API. The live-coding
portion of the talk migrates this to a minimal API.

## What's here

- **.NET 10**, ASP.NET Core Web API with controllers
- **SQLite** via EF Core, with real migrations (see setup below)
- `LibraryDbContext` injected **directly into controllers** — no
  repository or service layer. This is intentional: most real-world
  controller APIs look like this, and introducing a cleanup step (e.g. an
  endpoint filter for validation) during the minimal API migration is part
  of the talk's narrative, not a starting assumption.
- **DTOs already in place** for requests/responses, so the talk stays
  focused on the routing/architecture migration and doesn't also become a
  talk about API contract design.
- **Swashbuckle** for OpenAPI/Swagger — deliberately swapped for the
  built-in `Microsoft.AspNetCore.OpenApi` package in the minimal API
  version, to show that migration too.

## Domain

- `Author` (Id, FirstName, LastName, Country) has many `Book`
- `Book` (Id, Title, PublicationYear, Isbn, AuthorId) belongs to one `Author`

## Endpoints

| Method | Route | Notes |
|---|---|---|
| GET | `/api/authors` | list |
| GET | `/api/authors/{id}` | single, 404 branch |
| POST | `/api/authors` | validated via data annotations |
| PUT | `/api/authors/{id}` | validated update |
| DELETE | `/api/authors/{id}` | cascade deletes the author's books |
| GET | `/api/authors/{authorId}/books` | **nested** |
| GET | `/api/authors/{authorId}/books/{bookId}` | **nested** |
| POST | `/api/authors/{authorId}/books` | **nested**, validated |
| GET | `/api/books` | **flat** list |
| GET | `/api/books/{id}` | **flat** single, 404 branch |
| POST | `/api/books` | **flat**, validated |
| PUT | `/api/books/{id}` | **flat**, validated |
| DELETE | `/api/books/{id}` | **flat** |

## Setup (do this once, before the talk)

This project was generated without running the .NET SDK, so the EF Core
migration has **not** been generated yet — generating one by hand risks it
not matching your exact installed EF Core version, which is worse than no
migration at all for a live demo. Run:

```bash
./scripts/setup.sh
```

This restores packages, runs `dotnet ef migrations add InitialCreate`
against your local EF Core version, and applies it to create `library.db`.

Then:

```bash
cd src/LibraryApi.Controllers
dotnet run
```

Swagger UI opens automatically at `/swagger`. The database is seeded
automatically on first run (three authors, seven books).
