# Purple Mallard SPA with ASP.NET Core Backend

This project demonstrates integration between a React Single Page Application (SPA) and an ASP.NET Core backend API.

## Project Structure

- `src/web/purple-mallard-spa/` - React SPA frontend
- `src/web/PurpleMallard.Spa.Host/` - ASP.NET Core backend API hosting the SPA

## Development Workflow

### Running the Application

You can run both the frontend and backend together using the VS Code tasks:

1. Press `Ctrl+Shift+P` (or `Cmd+Shift+P` on Mac) to open the command palette
2. Type "Tasks: Run Task" and select it
3. Choose "Start Full Stack" to start both the API and SPA servers

Alternatively, you can run them separately:

- **API Only**: Run the "Start API" task
- **SPA Only**: Run the "Start Purple Mallard SPA" task

### Development Ports

- React SPA Development Server: http://localhost:5173
- ASP.NET Core API: http://localhost:5023

### API Endpoints

- `/api/weatherforecast` - Returns weather forecast data (example endpoint)

## Production Deployment

In production, the ASP.NET Core application serves both the API endpoints and the static files for the React SPA from a single server.

### Building for Production

1. Build the React SPA:

   ```bash
   cd src/web/purple-mallard-spa
   npm run build
   ```

2. Build the ASP.NET Core app:
   ```bash
   cd src/web/PurpleMallard.Spa.Host
   dotnet publish -c Release
   ```

## Adding New Features

### Adding New API Endpoints

1. Create a new controller or endpoint in the ASP.NET Core project
2. Use the convention `/api/*` for API endpoints to distinguish them from the SPA routes

### Adding New Frontend Routes

1. Create a new React component in the `purple-mallard-spa/app/routes/` folder
2. Add the route to the `routes.ts` file
3. Update navigation as needed

## Architecture Overview

The solution uses:

- **React Router v7** for frontend routing
- **Fetch API** for making HTTP requests to the backend
- **ASP.NET Core 9.0** for the backend API
- **SpaProxy** for integrating the React app with ASP.NET Core (modern approach)
- **Vite** as the frontend build tool and development server

## How It Works

- In development, SpaProxy automatically starts the Vite dev server and proxies requests to it
- The ASP.NET Core backend provides API endpoints with `/api` prefix
- In production, ASP.NET Core serves both the API and the static SPA files from a single origin
- The fallback route redirects SPA navigation requests to `index.html`

## Troubleshooting

- If API calls fail, check the CORS configuration and ensure you're using the correct API base URL
- If the SPA doesn't load in production, make sure the build folder is correctly configured in Program.cs
