import React, { useState } from 'react';
import { useAuth } from '../contexts/AuthContext';

export default function UserProfile() {
  const { user, logout } = useAuth();
  const [showDropdown, setShowDropdown] = useState(false);

  const toggleDropdown = () => {
    setShowDropdown(!showDropdown);
  };

  const handleLogout = async () => {
    await logout();
  };

  return (
    <div className="relative">
      <button
        onClick={toggleDropdown}
        className="flex items-center space-x-2 text-white hover:text-gray-300 focus:outline-none"
      >
        <span className="hidden md:inline-block">{user?.name || 'User'}</span>
        <svg
          xmlns="http://www.w3.org/2000/svg"
          className="h-5 w-5"
          viewBox="0 0 20 20"
          fill="currentColor"
        >
          <path
            fillRule="evenodd"
            d="M10 9a3 3 0 100-6 3 3 0 000 6zm-7 9a7 7 0 1114 0H3z"
            clipRule="evenodd"
          />
        </svg>
      </button>

      {showDropdown && (
        <div className="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg py-1 z-10">
          <div className="px-4 py-2 border-b">
            <p className="text-sm font-medium text-gray-900">{user?.name || 'User'}</p>
            <p className="text-xs text-gray-500 truncate">{user?.email || ''}</p>
          </div>
          <button
            onClick={handleLogout}
            className="block w-full text-left px-4 py-2 text-sm text-gray-700 hover:bg-gray-100"
          >
            Sign out
          </button>
        </div>
      )}
    </div>
  );
}
