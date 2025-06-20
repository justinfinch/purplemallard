import {
  isRouteErrorResponse,
  Links,
  Link,
  Meta,
  Outlet,
  Scripts,
  ScrollRestoration,
  useNavigate,
} from 'react-router';
import { AuthProvider, useAuth, ProtectedRoute, UserProfile, AuthLoading } from './modules/auth';

import type { Route } from './+types/root';
import './app.css';

export const links: Route.LinksFunction = () => [
  { rel: 'preconnect', href: 'https://fonts.googleapis.com' },
  {
    rel: 'preconnect',
    href: 'https://fonts.gstatic.com',
    crossOrigin: 'anonymous',
  },
  {
    rel: 'stylesheet',
    href: 'https://fonts.googleapis.com/css2?family=Inter:ital,opsz,wght@0,14..32,100..900;1,14..32,100..900&display=swap',
  },
];

export function Layout({ children }: { children: React.ReactNode }) {
  return (
    <html lang="en">
      <head>
        <meta charSet="utf-8" />
        <meta name="viewport" content="width=device-width, initial-scale=1" />
        <Meta />
        <Links />
      </head>
      <body>
        {children}
        <ScrollRestoration />
        <Scripts />
      </body>
    </html>
  );
}

function AppContent() {
  const { isAuthenticated, isLoading } = useAuth();

  // If still loading authentication status, show loading spinner
  if (isLoading) {
    return <AuthLoading />;
  }

  return (
    <>
      <header className="bg-gray-800 text-white p-4">
        <div className="container mx-auto flex justify-between items-center">
          <h1 className="text-xl font-bold">Purple Mallard</h1>
          <div className="flex items-center space-x-6">
            <nav>
              <ul className="flex space-x-4">
                <li>
                  <Link to="/" className="hover:text-gray-300">
                    Home
                  </Link>
                </li>
                <li>
                  <Link to="/product-assistant/new" className="hover:text-gray-300">
                    Product Assistant
                  </Link>
                </li>
              </ul>
            </nav>
            {isAuthenticated && <UserProfile />}
          </div>
        </div>
      </header>
      <main className="container mx-auto p-4">
        {/* Use the ProtectedRoute component to wrap all routes */}
        {isLoading ? (
          <AuthLoading />
        ) : (
          <ProtectedRoute>
            <Outlet />
          </ProtectedRoute>
        )}
      </main>
      <footer className="bg-gray-800 text-white p-4 mt-8">
        <div className="container mx-auto text-center">
          <p>© {new Date().getFullYear()} Purple Mallard SPA - ASP.NET Core Integration Example</p>
        </div>
      </footer>
    </>
  );
}

export default function App() {
  return (
    <AuthProvider>
      <AppContent />
    </AuthProvider>
  );
}

export function ErrorBoundary({ error }: Route.ErrorBoundaryProps) {
  let message = 'Oops!';
  let details = 'An unexpected error occurred.';
  let stack: string | undefined;

  if (isRouteErrorResponse(error)) {
    message = error.status === 404 ? '404' : 'Error';
    details =
      error.status === 404 ? 'The requested page could not be found.' : error.statusText || details;
  } else if (error && error instanceof Error) {
    details = error.message;
    stack = error.stack;
  }

  return (
    <main className="pt-16 p-4 container mx-auto">
      <h1>{message}</h1>
      <p>{details}</p>
      {stack && (
        <pre className="w-full p-4 overflow-x-auto">
          <code>{stack}</code>
        </pre>
      )}
    </main>
  );
}
