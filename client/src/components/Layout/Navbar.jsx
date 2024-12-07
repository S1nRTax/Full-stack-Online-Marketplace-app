import React from 'react';
import { Link } from 'react-router-dom'; 
import { useAuth } from '../../Context/AuthContext';

const Navbar = () => {
  const { isLoggedIn , logOut} = useAuth(); 

  return (
    <nav className="bg-gray-800 text-white p-4">
      <div className="container mx-auto flex justify-between items-center">
        <Link to="/" className="text-xl font-bold">
          Your Logo
        </Link>
        <div className="space-x-4">
          <Link to="/" className="hover:text-gray-300">Home</Link>
          {isLoggedIn ? (
            <>
              <Link to="/profile" className="hover:text-gray-300">Profile</Link>
              <button onClick={logOut}  className="text-red-500">
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
  );
};

export default Navbar;