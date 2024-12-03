import React, { useState } from 'react';
import { Navigate } from 'react-router-dom';

const Register = () => {
    const [name, setName] = useState("");
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [errorMessage, setErrorMessage] = useState(null); 
    const [isRegistered, setIsRegistered] = useState(false);
    const userType = "Customer";

    async function handleRegisterSubmit(e) {
        e.preventDefault();

        if (password !== confirmPassword) {
            setErrorMessage({
                message: "Passwords do not match.",
                errors: [],
            });
            return;
        }

        try {
            const item = { name, username, email, password, userType };
            const result = await fetch('https://localhost:7262/api/auth/register', {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(item),
            });
            
            if (result.ok) {
                setIsRegistered(true);
            } else {
                const errorData = await result.json();
                setErrorMessage({
                    message: errorData.message,
                    errors: errorData.errors || [],
                });
                return;
            }

            const data = await result.json();
            console.log(data);

        } catch (error) {
            console.error('Error during fetch:', error);
            setErrorMessage({
                message: "Something went wrong. Please try again.",
                errors: [],
            });
        }   
    }

    if (isRegistered) {
        return <Navigate to="/login" replace />;
    }

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
            <div className="max-w-md w-full space-y-8 bg-white p-8 rounded-lg shadow-md">
                <div>
                    <h2 className="text-center text-3xl font-extrabold text-gray-900">
                        Sign up
                    </h2>
                </div>
                <form className="space-y-7" onSubmit={handleRegisterSubmit}> 
                    <div className="rounded-md shadow-sm -space-y-px">
                        {/* Full Name Input */}
                        <div className="">
                            <label htmlFor="full-name" className="sr-only">Full Name</label>
                            <input 
                                id="full-name"
                                name="name"
                                type="text" 
                                required 
                                className="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-blue-500 focus:border-blue-500 focus:z-10 sm:text-sm" 
                                placeholder="Full Name"
                                value={name}
                                onChange={(e) => setName(e.target.value)} 
                            />
                        </div>

                        {/* Username Input */}
                        <div className="mb-4">
                            <label htmlFor="username" className="sr-only">Username</label>
                            <input 
                                id="username"
                                name="username"
                                type="text" 
                                required 
                                className="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-blue-500 focus:border-blue-500 focus:z-10 sm:text-sm" 
                                placeholder="Username"
                                value={username}
                                onChange={(e) => setUsername(e.target.value)} 
                            />
                        </div>

                        {/* Email Input */}
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

                        {/* Password Input */}
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

                        {/* Confirm Password Input */}
                        <div>
                            <label htmlFor="confirm-password" className="sr-only">Confirm Password</label>
                            <input 
                                id="confirm-password"
                                name="confirm-password"
                                type="password" 
                                required 
                                className="appearance-none rounded-none relative block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 rounded-b-md focus:outline-none focus:ring-blue-500 focus:border-blue-500 focus:z-10 sm:text-sm" 
                                placeholder="Confirm Password"
                                value={confirmPassword}
                                onChange={(e) => setConfirmPassword(e.target.value)} 
                            />
                        </div>
                    </div>

                    {/* Submit Button */}
                    <div>
                        <button 
                            type="submit" 
                            className="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                        >
                            Register
                        </button>
                    </div>
                </form>

                {/* Error Messages */}
                {errorMessage && (
                    <div className="mt-4 text-center">
                        {errorMessage.errors.length > 0 && (
                            <ul className="text-red-600">
                                {errorMessage.errors.map((error, index) => (
                                    <li key={index} className="mb-1">{error}</li>
                                ))}
                            </ul>
                        )}
                        {errorMessage.message && (
                            <p className="text-red-600">{errorMessage.message}</p>
                        )}
                    </div>
                )}
            </div>
        </div>
    );
};

export default Register;