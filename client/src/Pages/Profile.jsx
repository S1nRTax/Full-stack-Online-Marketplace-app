import React, { useEffect, useState, useRef } from 'react';
import { useAuth } from '../Context/authContext';
import { Link } from 'react-router-dom';
import { User, Mail, Camera, SmilePlus } from 'lucide-react';
import { useVendor } from '../Context/TranstitionContext';

const Profile = () => {
    const { isLoggedIn, authUser, validateAuth  } = useAuth();
    const { vendorData, validateVendorStatus } = useVendor();
    const [userData, setUserData] = useState(null);
    const [shopData, setShopData] = useState(null);
    const fileInputRef = useRef(null);  // To trigger the file input

    const API_BASE_URL = import.meta.env.VITE_API_URL.replace('/api', '');

    const uploadProfilePicture = async (e) => {
        const file = e.target.files[0];
        const formData = new FormData();
        formData.append('profilePicture', file);

        try {
            const response = await fetch(`https://localhost:7262/api/auth/update-profile-picture`, {
                method: 'PATCH',
                credentials: 'include',
                body: formData,
            });

            if (!response.ok) {
                throw new Error('Upload failed');
            }
            
            const data = await response.json();
            console.log('Profile picture uploaded successfully:', data);

        } catch (error) {
            console.error("Upload failed", error);
        }
    };

    const handleButtonClick = () => {
        fileInputRef.current.click(); // Trigger the hidden file input
    };

    const getProfilePictureSrc = () => {
        return userData?.profilePicture
            ? `${API_BASE_URL}${userData.profilePicture}`
            : `${API_BASE_URL}/images/user.png`; // Default profile picture
    };

    const getVendorState = () => {
        return shopData?.isVendor ? true : false;
    };

    useEffect(() => {
        const profileData = window.localStorage.getItem('profile_data');
        const shopInfo = window.localStorage.getItem('shop_data');
        
        if (profileData) {
            setUserData(JSON.parse(profileData));
        }

        if (shopInfo) {
            setShopData(JSON.parse(shopInfo));
        }
    }, []);

    useEffect(() => {
        if (authUser) {
            window.localStorage.setItem('profile_data', JSON.stringify(authUser));
            setUserData(authUser);
        }
        if (vendorData) {
            window.localStorage.setItem('shop_data', JSON.stringify(vendorData));
            setShopData(vendorData);
        }
    }, [authUser, vendorData]);

    useEffect(() => {
        validateVendorStatus();
        validateAuth();
        getVendorState();
    }, []);

    if (!authUser) {
        return <div className="text-center">Loading...</div>;
    }

    return (
        <div className="min-h-screen bg-gray-100 flex items-center justify-center p-4">
            <div className="bg-white shadow-xl rounded-lg w-full max-w-md p-6 space-y-6">
                <div className="text-center">
                    <h2 className="text-2xl font-bold text-gray-800 mb-4">Profile</h2>

                    {isLoggedIn ? (
                        <div className="space-y-6">
                            {/* Profile Image Section */}
                            <div className="relative mx-auto w-32 h-32">
                                <div className="w-full h-full rounded-full border-4 border-gray-200 overflow-hidden">
                                    <img 
                                        src={getProfilePictureSrc()} 
                                        alt="Profile" 
                                        className="w-full h-full object-cover"
                                    />
                                </div>
                                {/* Camera icon button */}
                                <div 
                                    onClick={handleButtonClick} 
                                    className="absolute bottom-0 right-0 bg-blue-500 rounded-full p-2 cursor-pointer"
                                >
                                    <Camera className="w-4 h-4 text-white" />
                                </div>
                            </div>

                            {/* Hidden file input */}
                            <input 
                                ref={fileInputRef}
                                type="file"
                                className="hidden"
                                onChange={uploadProfilePicture}
                            />

                            {/* User Details Section */}
                            <div className="space-y-4">
                                <div className="flex items-center bg-gray-50 p-3 rounded-lg">
                                    <SmilePlus className="w-6 h-6 text-blue-500 mr-3" />
                                    <div>
                                        <p className="text-sm text-gray-600">Fullname</p>
                                        <p className="font-semibold text-gray-800">{authUser.name}</p>
                                    </div>
                                </div>

                                <div className="flex items-center bg-gray-50 p-3 rounded-lg">
                                    <User className="w-6 h-6 text-blue-500 mr-3" />
                                    <div>
                                        <p className="text-sm text-gray-600">Username</p>
                                        <p className="font-semibold text-gray-800">{authUser.username}</p>
                                    </div>
                                </div>

                                <div className="flex items-center bg-gray-50 p-3 rounded-lg">
                                    <Mail className="w-6 h-6 text-green-500 mr-3" />
                                    <div>
                                        <p className="text-sm text-gray-600">Email</p>
                                        <p className="font-semibold text-gray-800">{authUser.email}</p>
                                    </div>
                                </div>

                                {/* Vendor Section */}
                                {getVendorState() ? (
                                    <div className="bg-gray-50 p-4 rounded-lg space-y-2">
                                        <h3 className="text-xl font-bold text-gray-800">{shopData.vendorDetails.shopName}</h3>
                                        <p className="text-sm text-gray-600">{shopData.vendorDetails.shopDescription}</p>
                                        <p className="text-sm text-gray-600 font-semibold">Address: {shopData.vendorDetails.shopAddress}</p>
                                    </div>
                                ) : (
                                    <div className='flex items-center p-3 rounded-lg'>
                                        <Link to="/create-shop" className="text-white bg-gradient-to-r from-blue-500 via-blue-600 to-blue-700 hover:bg-gradient-to-br focus:ring-4 focus:outline-none focus:ring-blue-300 dark:focus:ring-blue-800 shadow-lg shadow-blue-500/50 dark:shadow-lg dark:shadow-blue-800/80 font-medium rounded-lg text-sm px-5 py-2.5 text-center me-2 mb-2"> 
                                            Become a Vendor? 
                                        </Link>                                  
                                    </div>
                                )}
                            </div>
                        </div>
                    ) : (
                        <div className="text-center text-gray-500">
                            Please log in to view your profile
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
};

export default Profile;
