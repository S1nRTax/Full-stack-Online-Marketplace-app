import React from 'react';
import Navbar from './Navbar';
import Footer from './Footer';
import { Outlet } from 'react-router-dom';


const Layout = () => {
  return (
    <div className="flex flex-col min-h-screen bg-white">
      {/* Navigation bar / header */}
      <Navbar />


      {/* Main */}
      <main className="flex-grow p-4">
        <Outlet />
      </main>

      
      {/* Footer */}
      <Footer />
    </div>
  );
};

export default Layout;
