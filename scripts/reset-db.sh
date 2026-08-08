#!/usr/bin/env bash
# Your on-stage "reset button". Deletes the SQLite file and any WAL/SHM
# sidecar files, then re-applies migrations and reseeds on the next
# `dotnet run`. Use this if the demo state gets messy or the file locks up.
set -e

cd "$(dirname "$0")/../src/LibraryApi.Controllers"

rm -f library.db library.db-shm library.db-wal

echo "library.db removed. It will be recreated and reseeded automatically on the next 'dotnet run'."
