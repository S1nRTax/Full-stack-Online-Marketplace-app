import React from 'react'
import { Link, Navigate, useNavigate } from 'react-router-dom'
import { useAuth } from '../../authContext';

const Navbar = () => {
  const { isLoggedin , setIsLoggedin } = useAuth();
  const navigate = useNavigate();

  const handleLogout = async () => {
    try {
      const response = await fetch('https://localhost:7262/api/auth/logout', {
        method: 'POST',
        credentials: 'include'
      });
  
      if (response.ok) {
        setIsLoggedin(false);
         navigate("/" , {replace : true});
      }
    } catch (error) {
      console.error('Logout failed', error);
    }
  }

  return (
    <nav className="bg-gray-800 text-white p-4">
      <div className="container mx-auto flex justify-between items-center">
        <Link to="/" className="text-xl font-bold">
          Your Logo
        </Link>
        <div className="space-x-4">
          <Link to="/" className="hover:text-gray-300">Home</Link>
          {isLoggedin ? (
            <>
              <Link to="/profile" className="hover:text-gray-300">Profile</Link>
              <button onClick={handleLogout} className="text-red-500">
                Logout
              </button>
            </>
          ) : (
            <>
              <Link to="/login" className="hover:text-gray-300">Login</Link>
              <Link to="/register" className="hover:text-gray-300">Register</Link>
            </>
          )}
        </div>
      </div>
    </nav>
  )
}

export default Navbar;