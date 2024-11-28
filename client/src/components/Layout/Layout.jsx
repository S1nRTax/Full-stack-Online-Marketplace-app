import React from 'react'
import Navbar from './Navbar';
import Footer from './Footer';
import { Outlet } from 'react-router-dom';


const Layout = () => {
  return (
      <div>
            {/* Navigation bar / header */}
                <Navbar />
            {/* Main  */}
            <main>
                <Outlet />
            </main>
            {/* Footer : */}
                <Footer />
      </div>
  )
}

export default Layout;