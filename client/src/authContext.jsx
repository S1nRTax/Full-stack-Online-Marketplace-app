import { createContext, useState, useContext, useEffect } from 'react';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [isLoggedin, setIsLoggedin] = useState(false);
    const [user, setUser] = useState(null);
    const [isLoading, setIsLoading] = useState(true);

    // Function to verify authentication
    async function verifyAuth() {
        try {
            const response = await fetch('https://localhost:7262/api/auth/validate', {
                method: 'GET',
                credentials: 'include', 
            });

            if (response.ok) {
                const userData = await response.json();
                console.log("User validated successfully:", userData);
                setUser(userData);
                setIsLoggedin(true);
            } else {
                console.error("Validation failed:", await response.text());
                setIsLoggedin(false);
                setUser(null);
            }
        } catch (error) {
            console.error("Authentication verification failed:", error);
            setIsLoggedin(false);
            setUser(null);
        } finally {
            setIsLoading(false);
        }
    }

    // Use effect to verify authentication on initial mount
    useEffect(() => {
        console.log("Running initial authentication check...");
        verifyAuth(); // Call verifyAuth directly on mount
    }, []);

    return (
        <AuthContext.Provider value={{
            isLoggedin,
            setIsLoggedin,
            user,
            isLoading,
            setUser,
            verifyAuth,
        }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);
