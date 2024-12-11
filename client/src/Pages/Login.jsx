import React, { useState, useEffect } from 'react';
import { useAuth } from '../Context/AuthContext';
import { Navigate } from 'react-router-dom';

const Login = () => {

    const { isLoggedIn, logIn ,setErrorMessage , errorMessage}  = useAuth();
    const [email , setEmail] = useState("");
    const [password , setPassword] = useState("");

        
    const handleLoginSubmit = async (e) => {
        e.preventDefault();
        try {

          await logIn(email, password);
    
        } catch (error) {
          console.error(error);
          setErrorMessage(error.message);
        }
      };

      if(isLoggedIn){
        return <Navigate to="/" replace />
      }
    

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
            <div className="max-w-md w-full space-y-8 bg-white p-8 rounded-lg shadow-md">
                <div>
                    <h2 className="text-center text-3xl font-extrabold text-gray-900 mb-6">Sign in</h2>
                    <form className="space-y-6" onSubmit={handleLoginSubmit}>
                        <div className="rounded-md shadow-sm -space-y-px">
                            <div className="mb-4">
                                <label htmlFor="email" className="sr-only">Email address</label>
                                <input
                                    id="email"
                                    name="email"
                                    type="email"
                                    required
                                    className="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-blue-500 focus:border-blue-500 focus:z-10 sm:text-sm"
                                    placeholder="Email address"
                                    value={email}
                                    onChange={(e) => setEmail(e.target.value)}
                                />
                            </div>
                            <div className="mb-4">
                                <label htmlFor="password" className="sr-only">Password</label>
                                <input
                                    id="password"
                                    name="password"
                                    type="password"
                                    required
                                    className="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-blue-500 focus:border-blue-500 focus:z-10 sm:text-sm"
                                    placeholder="Password"
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                />
                            </div>
                        </div>
                        <div>
                            <button
                                type="submit"
                                className="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                            >
                                Sign in
                            </button>
                        </div>
                    </form>
                </div>
            </div>

            {errorMessage && (
                <div style={{ color: 'red', marginTop: '10px' }}>
                    <p>{errorMessage.message}</p>
                </div>
            )}
        </div>
    );
};

export default Login;
