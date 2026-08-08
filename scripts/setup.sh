#!/usr/bin/env bash
# Run this once after cloning, before your talk. Requires the .NET 10 SDK
# and `dotnet-ef` tool (dotnet tool install --global dotnet-ef).
set -e

cd "$(dirname "$0")/../src/LibraryApi.Controllers"

echo "Restoring packages..."
dotnet restore

echo "Adding initial EF Core migration (generated locally against your installed EF Core version)..."
dotnet ef migrations add InitialCreate

echo "Applying migration and creating library.db (Program.cs also does this on startup, this just verifies it works)..."
dotnet ef database update

echo "Done. Run 'dotnet run' from src/LibraryApi.Controllers to start the API."
