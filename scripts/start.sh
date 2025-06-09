#!/bin/bash

# Trust the .NET development certificates
echo "Trust the .NET development certificates..."
dotnet dev-certs https --trust

# Check React app directory and offer to start the development server if it exists
if [ -d "/workspaces/purplemallard/src/web" ] && [ -f "/workspaces/purplemallard/src/web/package.json" ]; then
    echo "React web app detected. You can start the development server with:"
    echo "  cd /workspaces/purplemallard/src/web && npm start"
fi