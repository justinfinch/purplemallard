# ASP.NET Core SpaProxy with React Integration

This project demonstrates integration between a React Single Page Application (SPA) and an ASP.NET Core backend API using the modern SpaProxy approach.

## Architecture Overview

- **Modern approach**: Using `Microsoft.AspNetCore.SpaProxy` instead of the legacy `Microsoft.AspNetCore.SpaServices.Extensions`
- **React Router v7** for frontend routing
- **Vite** as the frontend build tool and development server
- **ASP.NET Core 9.0** for the backend API

## How SpaProxy Works

SpaProxy is the recommended way to integrate a SPA with ASP.NET Core starting from .NET 6. It's simpler and more efficient than the older SpaServices.Extensions approach.

During development:

1. When you run the ASP.NET Core app, SpaProxy automatically:
   - Starts the frontend development server (Vite)
   - Redirects requests to the SPA development server
   - Provides hot module reloading (HMR)

In production:

1. The React app is built and the generated files are placed in the `wwwroot` folder
2. ASP.NET Core serves these static files and the API from the same origin
3. Uses a fallback route to direct SPA navigation to `index.html`

## Key Configuration Files

### ASP.NET Core Side

- `PurpleMallard.Spa.Host.csproj`: Configures SpaProxy settings
- `Program.cs`: Sets up the middleware for serving the SPA
- `launchSettings.json`: Configures the hosting startup assembly for SpaProxy

### React/Vite Side

- `vite.config.ts`: Configures the development server and build settings
- `.env.production`: Sets environment variables for production builds

## Development Workflow

You can run the app using either:

1. **VS Code Tasks**:

   - "Start API" task: Automatically starts both the ASP.NET Core API and the Vite development server

2. **Manual startup**:
   - In `src/web/PurpleMallard.Spa.Host`: Run `dotnet run`
   - The SpaProxy will automatically start the Vite dev server

## Production Build

To build for production:

1. The ASP.NET Core project handles building both the backend and frontend through MSBuild targets
2. Run `dotnet publish -c Release` in the `PurpleMallard.Spa.Host` directory
3. The frontend is built and copied to the appropriate location automatically

## Troubleshooting

- If assets aren't loading in production, check the Vite build configuration and BASE_URL setting
- If API calls fail, ensure the API endpoints are mapped correctly with `/api` prefix
- For HTTPS development issues, check the development certificates in `Properties/launchSettings.json`

## References

- [ASP.NET Core SPA documentation](https://learn.microsoft.com/en-us/aspnet/core/client-side/spa)
- [Vite Backend Integration](https://vitejs.dev/guide/backend-integration)
