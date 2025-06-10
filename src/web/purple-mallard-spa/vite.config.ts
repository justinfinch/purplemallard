import { fileURLToPath, URL } from 'node:url';

import { defineConfig } from 'vite';
import { reactRouter } from '@react-router/dev/vite';
import tailwindcss from '@tailwindcss/vite';
import tsconfigPaths from 'vite-tsconfig-paths';
import fs from 'fs';
import path from 'path';
import child_process from 'child_process';
import { env } from 'process';

const baseFolder =
  env.APPDATA !== undefined && env.APPDATA !== ''
    ? `${env.APPDATA}/ASP.NET/https`
    : `${env.HOME}/.aspnet/https`;

const certificateArg = process.argv
  .map(arg => arg.match(/--name=(?<value>.+)/i))
  .filter(Boolean)[0];
const certificateName = certificateArg?.groups ? certificateArg.groups.value : 'reactapp1.client';

if (!certificateName) {
  console.error(
    'Invalid certificate name. Run this script in the context of an npm/yarn script or pass --name=<<app>> explicitly.'
  );
  process.exit(-1);
}

const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

// Create the certificate directory if it doesn't exist
// This is required in .NET 9 as 'dotnet dev-certs' no longer creates the directory automatically
if (!fs.existsSync(baseFolder)) {
  try {
    fs.mkdirSync(baseFolder, { recursive: true });
    console.log(`Created certificate directory: ${baseFolder}`);
  } catch (error) {
    console.error(`Failed to create certificate directory: ${baseFolder}`, error);
    throw new Error('Could not create certificate directory.');
  }
}

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
  if (
    0 !==
    child_process.spawnSync(
      'dotnet',
      ['dev-certs', 'https', '--export-path', certFilePath, '--format', 'Pem', '--no-password'],
      { stdio: 'inherit' }
    ).status
  ) {
    throw new Error('Could not create certificate.');
  }
}

const target = env.ASPNETCORE_HTTPS_PORT
  ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
  : env.ASPNETCORE_URLS
  ? env.ASPNETCORE_URLS.split(';')[0]
  : 'https://localhost:7205';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [reactRouter(), tailwindcss(), tsconfigPaths()],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url)),
    },
  },
  server: {
    proxy: {
      '/api/': {
        target,
        secure: false,
      },
      '^/bff': {
        target,
        secure: false,
      },
      '^/signin-oidc': {
        target,
        secure: false,
      },
      '^/signout-callback-oidc': {
        target,
        secure: false,
      },
    },
    port: 5173,
    https: {
      key: fs.readFileSync(keyFilePath),
      cert: fs.readFileSync(certFilePath),
    },
  },
});
