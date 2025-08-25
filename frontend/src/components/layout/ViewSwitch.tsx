// src/components/ViewSwitch.tsx
import React from 'react';

const ViewSwitch: React.FC = () => {
  const switchToRazor = () => {
    document.cookie = 'prefersReact=false; path=/; max-age=0';
    window.location.href = '/Home/Index?react=false';
  };

  return (
    <div className="view-switch">
      <button 
        onClick={switchToRazor}
        className="px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600"
      >
        Switch to Classic View
      </button>
    </div>
  );
};

export default ViewSwitch;