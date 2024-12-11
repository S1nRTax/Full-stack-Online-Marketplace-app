import React, { useState, useContext, useCallback } from 'react';
import { useAuth } from './AuthContext';

const TransitionContext = React.createContext();

export function useVendor() {
    const context = useContext(TransitionContext);
    if (context === undefined) {
        throw new Error('useVendor must be used within a TransitionProvider');
    }
    return context;
}

export const TransitionProvider = ({ children }) => {
    const [vendorData, setVendorData] = useState(null);
    const [isVendor, setIsVendor] = useState(false);
    const { authUser, setErrorMessage } = useAuth(); 
    const API_URL = "https://localhost:7262/api";

    // function to transition a normal user to a vendor.
    const transitionToVendor = useCallback(async (shopName, shopAddress, shopDescription) => {
        // Validate inputs
        if (!shopName || !shopAddress) {
            setErrorMessage('Shop name and address are required');
            return false;
        }

        // Ensure userId exists
        if (!authUser?.id) {
            setErrorMessage('User authentication failed');
            return false;
        }

        const items = { shopName, shopAddress, shopDescription };
        const userId = authUser.id;

        try {
            const response = await fetch(`${API_URL}/transition/become-vendor/${userId}`, {
                method: 'POST',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json',
                    // Consider adding authorization header if needed
                    // 'Authorization': `Bearer ${authUser.token}`
                },
                body: JSON.stringify(items),
            });

            // More comprehensive error handling
            if (response.ok) {
                const vendorData = await response.json();
                setVendorData(vendorData);
                setIsVendor(true);
                
                // Use setErrorMessage to clear any previous errors
                setErrorMessage('');
                
                console.info("User has been transitioned successfully!");
                return true;
            } else {
                // More robust error parsing
                const errorData = await response.json().catch(() => ({
                    message: 'An unexpected error occurred'
                }));
                
                // Use the provided setErrorMessage from AuthContext
                setErrorMessage(errorData.message || 'Failed to transition to vendor');
                
                return false;
            }
        } catch (error) {
            // More informative error handling
            const errorMessage = error instanceof Error 
                ? error.message 
                : 'An unexpected network error occurred';
            
            setErrorMessage(errorMessage);
            console.error('Vendor transition error:', error);
            
            return false;
        }
    }, [authUser, setErrorMessage, API_URL]);

    const value = {
        transitionToVendor,
        vendorData,
        isVendor
    };

    return (
        <TransitionContext.Provider value={value}>
            {children}
        </TransitionContext.Provider>
    );
};