import React, { useState, useContext, useEffect } from 'react';

const AuthContext = React.createContext();

export function useAuth() {
    return useContext(AuthContext);
}

export function AuthProvider({ children }) {
    const [authUser, setAuthUser] = useState(null);
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [errorMessage, setErrorMessage] = useState(null);

    const API_URL = "https://localhost:7262/api";

    // Function to handle login
    const logIn = async (email, password) => {
        try {
            const response = await fetch(`${API_URL}/auth/login`, {
                method: 'POST',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, password }),
            });

            if (response.ok) {
                await validateAuth(); 
                setErrorMessage(null);
            } else {
                const { message } = await response.json();
                setErrorMessage(message || "Login failed");
            }
        } catch (error) {
            console.error('Login error:', error);
            setErrorMessage("Something went wrong. Please try again.");
        }
    };

    // Function to validate user
    const validateAuth = async () => {
        try {
            const response = await fetch(`${API_URL}/auth/validate`, {
                method: 'GET',
                credentials: 'include', 
                headers: { 
                    'Content-Type': 'application/json'
                }
            });
    
            if (response.ok) {
                const userData = await response.json();
                setAuthUser(userData);
                setIsLoggedIn(true);
            } else {
                console.error('Validation failed');
                logOut();
            }
        } catch (error) {
            console.error('Validation error:', error);
            logOut();
        }
    };

    // Function to handle logout
    const logOut = async () => {
        try {
            const response = await fetch(`${API_URL}/auth/logout`, {
                method: 'POST',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
            });

            if (response.ok) {
                setAuthUser(null);
                setIsLoggedIn(false);
            } else {
                console.error('Logout failed');
            }
        } catch (error) {
            console.error('Logout error:', error);
        }
    };

    // Function to refresh the access token
    const refreshAccessToken = async () => {
        try {
            const response = await fetch(`${API_URL}/auth/refreshToken`, {
                method: 'GET',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
            });

            if (response.ok) {
                await validateAuth(); 
            } else {
                console.error('Refresh token failed');
                logOut();
            }
        } catch (error) {
            console.error('Refresh error:', error);
            logOut();
        }
    };

    // Periodic Authentification check:
    useEffect(()=>{
        validateAuth();
    },[])

    // Periodic token refresh logic
    useEffect(() => {
        const interval = setInterval(() => {
            if (isLoggedIn) {
                console.warn("token refreshed!");
                refreshAccessToken();
            }
        }, 1 * 60 * 1000); // Refresh every 15 minutes

        return () => clearInterval(interval);
    }, [isLoggedIn]);

    const value = {
        authUser,
        isLoggedIn,
        logIn,
        logOut,
        errorMessage,
    };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
