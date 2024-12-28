import React, { useState, useContext, useEffect } from 'react';
import { useAuth } from './authContext';

const TransitionContext = React.createContext();

export function useVendor() {
    const context = useContext(TransitionContext);
    if (!context) {
        throw new Error('useVendor must be used within a TransitionProvider');
    }
    return context;
}

export const TransitionProvider = ({ children }) => {
    const [vendorData, setVendorData] = useState(() => {
        const savedData = window.localStorage.getItem('shop_data');
        return savedData ? JSON.parse(savedData) : null;
    });
    const { authUser, setErrorMessage } = useAuth();
    const API_URL = "https://localhost:7262/api";

    useEffect(() => {
        if (vendorData) {
            window.localStorage.setItem('shop_data', JSON.stringify(vendorData));
        } else {
            window.localStorage.removeItem('shop_data');
        }
    }, [vendorData]);

    const transitionToVendor = async (shopName, shopAddress, shopDescription) => {
        if (!shopName || !shopAddress) {
            setErrorMessage('Shop name and address are required');
            return false;
        }

        if (!authUser?.id) {
            setErrorMessage('User authentication failed');
            return false;
        }

        const vendorDetails = { shopName, shopAddress, shopDescription };
        try {
            const response = await fetch(`${API_URL}/transition/become-vendor/${authUser.id}`, {
                method: 'POST',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(vendorDetails),
            });

            if (response.ok) {
                const data = await response.json();
                setVendorData(data);
                setErrorMessage(null);
                return true;
            } else {
                const errorData = await response.json();
                setErrorMessage(errorData.message || 'Failed to transition to vendor');
                return false;
            }
        } catch (error) {
            setErrorMessage('An unexpected network error occurred');
            console.error('Vendor transition error:', error);
            return false;
        }
    };

    const validateVendorStatus = async () => {
        try {
            const response = await fetch(`${API_URL}/transition/validateVendor`, {
                method: 'GET',
                credentials: 'include',
                headers: { 'Content-Type': 'application/json' },
            });

            if (response.ok) {
                const data = await response.json();
                setVendorData(data);
                setErrorMessage(null);
                return true;
            } else {
                const errorData = await response.json();
                setErrorMessage(errorData.message || 'Failed to validate vendor status');
                setVendorData(null);
                return false;
            }
        } catch (error) {
            setErrorMessage('An unexpected network error occurred');
            setVendorData(null);
            console.error('Validation error:', error);
            return false;
        }
    };

    const value = {
        transitionToVendor,
        validateVendorStatus,
        vendorData,
    };

    return (
        <TransitionContext.Provider value={value}>
            {children}
        </TransitionContext.Provider>
    );
};
