import type { Route } from './+types/home';
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
      <div className="max-w-3xl mx-auto bg-white shadow-lg rounded-lg p-6 border border-gray-200">
        <div className="bg-orange-50 p-4 rounded-lg border border-orange-200">
          <h3 className="text-lg font-semibold text-orange-800 mb-2">🤖 Product Assistant</h3>
          <p className="mb-3">
            Get help with product descriptions, feature analysis, marketing strategies, and UX
            improvements from our AI-powered Product Assistant.
          </p>
          <Link
            to="/product-assistant/new"
            className="inline-block px-4 py-2 bg-orange-600 text-white rounded hover:bg-orange-700 transition-colors"
          >
            Start Chat with Product Assistant
          </Link>
        </div>
      </div>
    </div>
  );
}
