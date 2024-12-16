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

            // Log response details for debugging
            if (!response.ok) {
                const errorText = await response.text();
                console.error('Error Response:', {
                    status: response.status,
                    statusText: response.statusText,
                    body: errorText,
                });
            }

            if (response.ok) {
                await validateVendorStatus();
                console.info('User has been transitioned successfully!');
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

    const validateVendorStatus = useCallback(async () => {
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
                setIsVendor(vendorData.isVendor); // Set state based on backend response
                setErrorMessage('');
                return true;
            } else {
                const errorData = await response.json().catch(() => ({
                    message: 'An unexpected error occurred',
                }));
                setErrorMessage(errorData.message || 'Failed to validate vendor status');
                setIsVendor(false);
                setVendorData(null);
                return false;
            }
        } catch (error) {
            const errorMessage = error instanceof Error 
                ? error.message 
                : 'An unexpected network error occurred';

            setErrorMessage(errorMessage);
            console.error('Vendor status validation error:', error);
            setIsVendor(false);
            setVendorData(null);
            return false;
        }
    }, [API_URL, setErrorMessage]);

    const value = {
        transitionToVendor,
        validateVendorStatus,
        vendorData,
        isVendor,
        setIsVendor,
    };

    return (
        <TransitionContext.Provider value={value}>
            {children}
        </TransitionContext.Provider>
    );
};
