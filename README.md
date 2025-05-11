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

3. Create a `.env` file by copying the `.env.example` file:

```bash
cp .env.example .env
```

4. Configure your environment variables in the `.env` file:

### Required Environment Variables

#### Application Configuration

```env
ASPNETCORE_ENVIRONMENT=Development  # Use 'Production' for production environment
```

#### Database Configuration

```env
PGHOST=your-host.aivencloud.com    # PostgreSQL host
PGPORT=your-port                   # PostgreSQL port (typically 5432)
PGUSER=your-username              # PostgreSQL username
PGPASSWORD=your-password          # PostgreSQL password
PGDATABASE=your-database          # PostgreSQL database name
```

#### Authentication

```env
JWT_SECRET=your-secure-jwt-secret  # Use a strong, random string
```

#### Email Service

```env
EMAIL_FROM=your-email@example.com   # Sender email address
EMAIL_USERNAME=your-email-username  # SMTP username
EMAIL_PASSWORD=your-email-password  # SMTP password
SMTP_HOST=smtp.example.com         # SMTP server host
SMTP_PORT=587                      # SMTP server port
```

#### Google Services

##### Google Authentication (for login)

```env
GOOGLE_CLIENT_ID=your-google-auth-client-id        # OAuth client ID for login
GOOGLE_CLIENT_SECRET=your-google-auth-client-secret # OAuth client secret for login
```

##### Google Meet Integration

```env
GMEET_CLIENT_ID=your-google-meet-client-id         # OAuth client ID for Google Meet
GMEET_CLIENT_SECRET=your-google-meet-client-secret # OAuth client secret for Google Meet
GMEET_SCOPE=https://www.googleapis.com/auth/calendar.events # Required OAuth scope
GMEET_REDIRECT_URL=http://localhost:5121/api/GMeetAuth/callback # OAuth redirect URL
```

5. Run the application:

```bash
dotnet run
```

The application will automatically start the API server and open your default browser to the Swagger UI interface at `http://localhost:5121`.

If the browser doesn't open automatically, manually navigate to:

- HTTP: `http://localhost:5121`

## Security Notes

1. Never commit the `.env` file to version control
2. Use strong, unique values for all secrets and passwords
3. Keep your Google client secrets secure
4. Regularly rotate the JWT secret in production
5. Use environment-specific settings for development and production
6. Ensure all environment variables are set before running in production
7. Use different OAuth credentials for development and production environments

```

```
