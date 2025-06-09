# Purple Mallard

A full-stack application using .NET Aspire and React.

## Development Environment

This project uses a Visual Studio Code Dev Container for development. The container provides:

- .NET SDK 9.0
- .NET Aspire templates
- Node.js with npm
- React development tools

### Getting Started

1. Open the project in VS Code with the Dev Containers extension installed
2. Click "Reopen in Container" when prompted
3. Wait for the container to build and initialize

### Running the Application

#### .NET Backend:

```bash
cd /workspaces/purplemallard/src
dotnet run --project api/PurpleMallard.Api
```

#### React Frontend:

```bash
cd /workspaces/purplemallard/src/web
npm start
```

## Features

- .NET Aspire for cloud-ready applications
- React for frontend development
- Docker support for containerization
