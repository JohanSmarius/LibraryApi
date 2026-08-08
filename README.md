# Library API - .NET 10 Minimal API

A .NET 10 Minimal API for managing authors and books with SQLite and
Entity Framework Core.

## Architecture

- Feature-based endpoint modules in `Endpoints/` handle HTTP concerns and
  delegate application and data operations to scoped services
- Route groups for author, nested author-book, and flat book routes
- Author and book services use `LibraryDbContext` directly
- DataAnnotations request validation through .NET 10 `AddValidation()`
- Strongly typed HTTP results for accurate OpenAPI response metadata
- Built-in `Microsoft.AspNetCore.OpenApi` generation using OpenAPI 3.1
- Scalar interactive API documentation in Development
- EF Core migrations and deterministic seed data applied on startup

The project and root namespace retain the `LibraryApi.Controllers` name to
avoid an unrelated project rename, but the application no longer uses MVC
controllers.

## Domain

- `Author` (Id, FirstName, LastName, Country) has many `Book`
- `Book` (Id, Title, PublicationYear, Isbn, AuthorId) belongs to one `Author`

Deleting an author cascade deletes all books that belong to that author.

## Endpoints

| Method | Route | Notes |
|---|---|---|
| GET | `/api/authors` | list |
| GET | `/api/authors/{id}` | single, 404 branch |
| POST | `/api/authors` | validated |
| PUT | `/api/authors/{id}` | validated update |
| DELETE | `/api/authors/{id}` | cascade deletes the author's books |
| GET | `/api/authors/{authorId}/books` | nested list |
| GET | `/api/authors/{authorId}/books/{bookId}` | nested single |
| POST | `/api/authors/{authorId}/books` | nested, validated |
| GET | `/api/books` | flat list |
| GET | `/api/books/{id}` | flat single, 404 branch |
| POST | `/api/books` | flat, validated |
| PUT | `/api/books/{id}` | flat, validated |
| DELETE | `/api/books/{id}` | flat |

## Run locally

Requires the .NET 10 SDK.

```bash
dotnet restore
dotnet run --project src/LibraryApi.Controllers
```

In Development:

- Scalar UI: `/scalar`
- OpenAPI document: `/openapi/v1.json`

The SQLite database is migrated and seeded automatically on startup with
three authors and seven books.

## Reset the local database

```bash
./scripts/reset-db.sh
```

The next application start recreates, migrates, and seeds `library.db`.
