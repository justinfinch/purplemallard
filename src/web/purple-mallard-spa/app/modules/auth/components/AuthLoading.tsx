import React from 'react';
import Spinner from './Spinner';

export default function AuthLoading() {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen">
      <div className="text-center">
        <Spinner size="large" className="mb-4" />
        <h2 className="text-xl font-semibold text-gray-700">Checking authentication...</h2>
        <p className="text-gray-500 mt-2">Please wait while we verify your login status</p>
      </div>
    </div>
  );
}
