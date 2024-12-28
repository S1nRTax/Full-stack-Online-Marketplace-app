import React, { useState, useContext, useEffect } from 'react';
import { useVendor } from './TranstitionContext';

const AuthContext = React.createContext();

export function useAuth() {
    return useContext(AuthContext);
}

export function AuthProvider({ children }) {
    const [authUser, setAuthUser] = useState(null);
    const [isLoggedIn, setIsLoggedIn] = useState(false);
    const [errorMessage, setErrorMessage] = useState(null);
    const [isLoading , setIsloading] = useState(null);
    const API_URL = "https://localhost:7262/api";

    // Function to handle login
    const logIn = async (email, password) => {
        setIsloading(true); 
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
        } finally {
            setIsloading(false); 
        }
    };
    

    // Function to validate user
    const validateAuth = async () => {
        setIsloading(true); 
        try {
            const response = await fetch(`${API_URL}/auth/validate`, {
                method: 'GET',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
            });
    
            if (response.ok) {
                const userData = await response.json();
                setAuthUser(userData);
                window.localStorage.setItem('profile_data', JSON.stringify(userData)); // Store user data
                setIsLoggedIn(true); 
            } else {
                logOut(); // Log out if validation fails
            }
        } catch (error) {
            console.error('Validation error:', error);
            logOut(); 
        } finally {
            setIsloading(false); 
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
                console.log("deleting localstorage after logging out.");
                window.localStorage.removeItem('shop_data');
                window.localStorage.removeItem('profile_data');
                return true;
                
            } else {
                console.error('Logout failed');
                return false;
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
                setIsloading(true);
            } else {
                console.error('Refresh token failed');
                logOut();
            }
        } catch (error) {
            console.error('Refresh error:', error);
            logOut();
        }
        setIsloading(false);
    };

    // Periodic Authentification check:
    useEffect(()=>{
        validateAuth();
    },[])

    // Periodic token refresh logic
    useEffect(() => {
        const interval = setInterval(() => {
            if (isLoggedIn) {
                refreshAccessToken();
            }
        }, 14 * 60 * 1000); // Refresh every 14 minutes since the refresh token expires every 15 min.

        return () => clearInterval(interval);
    }, [isLoggedIn]);

    const value = { 
        authUser,
        validateAuth,
        setErrorMessage,
        isLoggedIn,
        isLoading,
        logIn,
        logOut,
        errorMessage,
    };

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
