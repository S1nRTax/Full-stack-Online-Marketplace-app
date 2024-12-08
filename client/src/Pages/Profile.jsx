import React from 'react'
import { useAuth } from '../Context/authContext';
import { User, Mail } from 'lucide-react';

const Profile = () => {
    const { isLoggedIn, authUser } = useAuth();

    return (
        <div className="min-h-screen bg-gray-100 flex items-center justify-center p-4">
            <div className="bg-white shadow-xl rounded-lg w-full max-w-md p-6 space-y-6">
                <div className="text-center">
                    <h2 className="text-2xl font-bold text-gray-800 mb-4">Profile</h2>
                    
                    {isLoggedIn ? (
                        <div className="space-y-4">
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
                        </div>
                    ) : (
                        <div className="text-center text-gray-500">
                            Please log in to view your profile
                        </div>
                    )}
                </div>
            </div>
        </div>
    )
}

export default Profile