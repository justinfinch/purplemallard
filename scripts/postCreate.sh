#!/bin/bash

# Install .NET Aspire project templates
echo "Installing .NET Aspire project templates..."
dotnet new install Aspire.ProjectTemplates::9.3.0 --force

# Setup for React development
echo "Setting up Node.js and npm tools for React development..."
npm install -g create-react-app
npm install -g eslint
npm install -g prettier

# Check if the web directory has a package.json file
if [ -f "/workspaces/purplemallard/src/web/purple-mallard-spa/package.json" ]; then
    echo "Installing Node dependencies in web project..."
    cd /workspaces/purplemallard/src/web/purple-mallard-spa
    npm install
fi

echo "Development container setup complete!"