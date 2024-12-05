import React, { useState, useEffect } from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../authContext';

const Login = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const { isLoggedin, setIsLoggedin, setUser, verifyAuth } = useAuth(); // Destructure verifyAuth here
    const [errorMessage, setErrorMessage] = useState(null);

    async function handleLoginSubmit(event) {
        event.preventDefault();
        try {
            const item = { email, password };
            const result = await fetch('https://localhost:7262/api/auth/login', {
                method: 'POST',
                credentials: 'include',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(item),
            });

            const text = await result.text(); // Get text first

            if (result.ok) {
                const userData = text ? JSON.parse(text) : {}; // Check for empty response
                setUser({
                    id: userData.userId,
                    username: userData.username,
                    email: userData.email,
                });
                setIsLoggedin(true);

                // Call verifyAuth after successful login
                verifyAuth(); // Trigger token verification
                setErrorMessage(null);
            } else {
                const errorData = text ? JSON.parse(text) : { message: 'Login failed' };
                setErrorMessage({ message: errorData.message || 'Login failed' });
            }
        } catch (error) {
            console.error('Error during fetch:', error);
            setErrorMessage({ message: "Something went wrong. Please try again!" });
        }
    }

    if (isLoggedin) {
        return <Navigate to="/" replace />;
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
