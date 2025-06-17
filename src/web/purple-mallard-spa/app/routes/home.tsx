import type { Route } from './+types/home';
import { Welcome } from '../welcome/welcome';
import { Link } from 'react-router';

export function meta({}: Route.MetaArgs) {
  return [
    { title: 'Purple Mallard SPA' },
    { name: 'description', content: 'React SPA with ASP.NET Core Backend Integration' },
  ];
}

export default function Home() {
  return (
    <div className="space-y-8">
      <Welcome />

      <div className="max-w-3xl mx-auto bg-white shadow-lg rounded-lg p-6 border border-gray-200">
        <h2 className="text-2xl font-bold mb-4">React + ASP.NET Core Integration</h2>
        <p className="mb-4">
          This application demonstrates the integration between a React SPA frontend and an ASP.NET
          Core backend API.
        </p>
        <p className="mb-4">
          The frontend is built with React Router and uses modern JavaScript techniques to
          communicate with the backend.
        </p>
        <div className="bg-blue-50 p-4 rounded-lg border border-blue-200">
          <h3 className="text-lg font-semibold text-blue-800 mb-2">
            Try the Weather Forecast feature
          </h3>
          <p className="mb-3">
            Check out the Weather Forecast page to see real-time data fetched from the ASP.NET Core
            backend API.
          </p>
          <Link
            to="/weather"
            className="inline-block px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors"
          >
            View Weather Forecast
          </Link>
        </div>
      </div>
    </div>
  );
}
