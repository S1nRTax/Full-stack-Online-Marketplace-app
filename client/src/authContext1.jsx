import { createContext, useState, useContext, useEffect } from 'react';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [isLoggedin, setIsLoggedin] = useState(false);
    const [user, setUser] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [errorMessage, setErrorMessage] = useState(null);
    // fucntion to handle login:
    const login = async (email, password) => {
        
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

            const text = await result.text(); 

            if (result.ok) {
                const userData = text ? JSON.parse(text) : {}; 
                setUser({
                    id: userData.userId,
                    username: userData.username,
                    email: userData.email,
                });
                setIsLoggedin(true);
                refreshTokenHandler();
                verifyAuth(); 
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
                //console.error("Validation failed:", await response.text());
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


    async function refreshTokenHandler() {
            try{
                    const response = await fetch('https://localhost:7262/api/auth/refresh-token',{
                        method: 'GET',
                        credentials: 'include'
                    });

                    if(response.ok){
                        console.warn("token has been refreshed!");
                        await verifyAuth();
                    }else{
                        console.error('Failed to refresh the token !');
                        setIsLoggedin(false);
                    }


            }catch(error){
                    console.error("Error refreshing access token :", error);
            }
    };


    // Use effect to verify authentication on initial mount
    useEffect(() => {
        console.log("Running initial authentication/refreshToken check...");
        verifyAuth(); 
        refreshTokenHandler();
    }, []);


    useEffect(() => {
        const intervalId = setInterval(() => {
            // Check token expiration logic here (you can customize this)
            refreshTokenHandler();
        }, 15 * 60 * 1000); // Refresh every 15 minutes

        return () => clearInterval(intervalId); // Cleanup on unmount
    }, []);

    return (
        <AuthContext.Provider value={{
            isLoggedin,
            setIsLoggedin,
            user,
            isLoading,
            setUser,
            verifyAuth,
            refreshTokenHandler,
            login,
        }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);
