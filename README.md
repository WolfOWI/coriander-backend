# CoriCore API

A .NET Core Web API project with integrated Swagger UI documentation.

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

## Setting up the project

1. Clone the repository & navigate to the CoriCore directory:

```bash
git clone https://github.com/WolfOWI/coriander-backend.git
cd coriander-backend/CoriCore
```

2. Install dependencies:

```bash
dotnet restore
```

3. Create a `.env` file by copying the `.env.template` file:

```bash
cp .env.template .env
```

4. Edit the `.env` file with your own values.

```bash
PGHOST=your-host.aivencloud.com
PGPORT=your-port
PGUSER=your-username
PGPASSWORD=your-password
PGDATABASE=your-database
ASPNETCORE_ENVIRONMENT=Development
```

5. Create a `appsettings.Development.json` file by copying the `appsettings.Development.json.template` file:

```bash
cp appsettings.Development.json.template appsettings.Development.json
```

6. Edit the `ConnectionStrings` section in the `appsettings.Development.json` file with your own values.

```bash
"ConnectionStrings": {
    "DefaultConnection": "Host=your-host;Port=your-port;Username=your-username;Password=your-password;Database=your-database;SSL Mode=Require"
  },
```

7. Run the application:

```bash
dotnet run
```

The application will automatically start the API server and open your default browser to the Swagger UI interface at `http://localhost:5121`.

If the browser doesn't open automatically, manually navigate to:

- HTTP: `http://localhost:5121`

```

```
