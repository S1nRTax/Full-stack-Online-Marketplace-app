import React, { useState, useContext } from 'react';
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
    const { authUser, setErrorMessage , logOut} = useAuth(); 
    const API_URL = "https://localhost:7262/api";

  

    const transitionToVendor = async (shopName, shopAddress, shopDescription) => {
        // Input validation
        if (!shopName || !shopAddress) {
            setErrorMessage('Shop name and address are required');
            console.log("im here");
            return false;
        }

        // Authentication check
        if (!authUser?.id) {
            setErrorMessage('User authentication failed');
            console.log("im here");
            return false;
        }

        const vendorDetails = { shopName, shopAddress, shopDescription };
        const userId = authUser.id;
        console.log(vendorData);

        try {
            const response = await fetch(`${API_URL}/transition/become-vendor/${userId}`, {
                method: 'POST',
                mode: 'cors',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(vendorDetails),
            });

            if (response.ok) {
                await validateVendorStatus();
                window.localStorage.setItem('shop_data', JSON.stringify(vendorData));
                return true;
            } else {
                const errorData = await response.json().catch(() => ({
                    message: 'An unexpected error occurred',
                }));
                setErrorMessage(errorData.message || 'Failed to transition to vendor');
                return false;
            }
        } catch (error) {
            const errorMessage = error instanceof Error 
                ? error.message 
                : 'An unexpected network error occurred';

            setErrorMessage(errorMessage);
            console.error('Vendor transition error:', error);
            return false;
        }
    };

    const validateVendorStatus = async () => {
        try {
            const response = await fetch(`${API_URL}/transition/validateVendor`, {
                method: 'GET', 
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json',
                },
            });

            if (response.ok) {
                const vendorData = await response.json();
                setVendorData(vendorData);
                setErrorMessage('');
                return true;
            } else {
                const errorData = await response.json().catch(() => ({
                    message: 'An unexpected error occurred',
                }));
                setErrorMessage(errorData.message || 'Failed to validate vendor status');
                window.localStorage.removeItem('shop_data');
                setVendorData(null);
                return false;
            }
        } catch (error) {
            const errorMessage = error instanceof Error 
                ? error.message 
                : 'An unexpected network error occurred';
            setErrorMessage(errorMessage);
            setVendorData(null);
            return false;
        }
    };

    const value = {
        transitionToVendor,
        validateVendorStatus,
        vendorData,
        setVendorData
    };

    return (
        <TransitionContext.Provider value={value}>
            {children}
        </TransitionContext.Provider>
    );
};
