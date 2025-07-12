# 🐳 SQL Server in Docker with Automatic Database and Table Creation

This documentation explains how to set up Microsoft SQL Server 2022 in a Docker container and automatically create a database (`Auctions`) with tables and seed data using Docker Compose and an init script.

---

## 📁 Project Structure
```
project-folder/
├── docker-compose.yml
└── init-auctions.sql
```

---

## 📄 `init-auctions.sql`
This script creates the `Auctions` database, a sample table, and inserts seed data.

```sql
-- Create the Auctions database if it doesn't exist
IF DB_ID('Auctions') IS NULL
BEGIN
    CREATE DATABASE Auctions;
END
GO

-- Switch to the Auctions database
USE Auctions;
GO

-- Create a sample table
IF OBJECT_ID('Auctions', 'U') IS NULL
BEGIN
    CREATE TABLE Auctions (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(100),
        StartTime DATETIME,
        EndTime DATETIME,
        IsActive BIT
    );
END
GO

-- Seed sample data
INSERT INTO Auctions (Title, StartTime, EndTime, IsActive)
VALUES
('Auction #1', GETDATE(), DATEADD(day, 7, GETDATE()), 1),
('Auction #2', GETDATE(), DATEADD(day, 14, GETDATE()), 0);
GO
```

---

## 🐋 `docker-compose.yml`
This defines two services:
- `sqlserver`: runs the SQL Server instance
- `sql-init`: a one-time container that executes the init script using `sqlcmd`

```yaml
version: '3.8'

services:
  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: sqlserver
    environment:
      ACCEPT_EULA: "Y"
      MSSQL_SA_PASSWORD: "MsSql123!"
    ports:
      - "1433:1433"
    volumes:
      - mssql_data:/var/opt/mssql

  sql-init:
    image: mcr.microsoft.com/mssql-tools
    depends_on:
      - sqlserver
    entrypoint: >
      /bin/bash -c "
      sleep 20 &&
      /opt/mssql-tools/bin/sqlcmd -S sqlserver -U sa -P MsSql123! -i /tmp/init-auctions.sql
      "
    volumes:
      - ./init-auctions.sql:/tmp/init-auctions.sql:ro

volumes:
  mssql_data:
```

---

## 🚀 Running the Setup
From the terminal in the same directory as the files, run:

```bash
docker compose up -d --build
```

This will:
- Start SQL Server
- Wait ~20 seconds for it to initialize
- Automatically create the database, table, and seed data

---

## 🧪 Verify the Database
Use `sqlcmd` to confirm everything was created:

```bash
sqlcmd -S localhost,1433 -U sa -P "MsSql123!" -d Auctions -Q "SELECT * FROM Auctions"
```

Or view it in **Visual Studio**:
- Open **SQL Server Object Explorer**
- Add a new connection: `localhost,1433` with SQL Auth (`sa` / `MsSql123!`)
- Expand databases → You should see `Auctions`

---

## ✅ Notes
- Password `MsSql123!` must meet SQL Server security requirements
- The `sql-init` container runs once to create data, then stops
- You can customize the SQL script to add more tables or seed logic

---

Let me know if you want to:
- Add Entity Framework support
- Auto-generate EF models (`Scaffold-DbContext`)
- Extend with stored procedures or relationships



## 1. Stop and Remove Everything (Containers + Volumes + Networks)
```docker compose down -v --remove-orphans```
This will:

- Stop all services in your docker-compose.yml
- Remove the mssql_data volume (database files)
- Remove unused containers/networks from previous runs

## 2. (Optional) Remove Any Dangling Volumes or Images
```
docker volume prune -f
docker image prune -f
```
This clears unused Docker volumes and images if disk space is an issue.

## 3. Force Rebuild and Start Fresh
```
docker compose build --no-cache
docker compose up -d
```
This ensures:
- All services are built fresh
- Environment variables like MSSQL_SA_PASSWORD are re-applied cleanly

## 4. After ~20–30 seconds, test your connection
```
sqlcmd -S localhost,1444 -U sa -P "StrongP@ssw0rd1!" -Q "SELECT name FROM sys.databases"
```