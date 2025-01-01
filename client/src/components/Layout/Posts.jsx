import React, { useState } from 'react'
import { ArrowLeftToLine } from 'lucide-react'
import { AiOutlineLike, AiOutlineArrowRight } from 'react-icons/ai';
import { ImageIcon, UploadCloud, X } from 'lucide-react';

const Posts = ({ onToggleSidebar, isSidebarHidden }) => {
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [imagePreview, setImagePreview] = useState(null);
    const [dragActive, setDragActive] = useState(false);
    const [Title , setTitle] = useState("");
    const [Description, setDescription] = useState("");
    const [Price, setPrice] = useState("");
    const [PostImage, setPostImage] = useState();
    const [postData , setPostData] = useState();

    const API_URL = "https://localhost:7262/api";

    const handleImageChange = (e) => {
        const file = e.target.files[0];
        setPostImage(file);
        if (file) {
            const reader = new FileReader();
            reader.onloadend = () => {
                setImagePreview(reader.result);
            };
            reader.readAsDataURL(file);
        }
    };

    const handleDrag = (e) => {
        e.preventDefault();
        e.stopPropagation();
        if (e.type === "dragenter" || e.type === "dragover") {
            setDragActive(true);
        } else if (e.type === "dragleave") {
            setDragActive(false);
        }
    };

    const handleDrop = (e) => {
        e.preventDefault();
        e.stopPropagation();
        setDragActive(false);

        const file = e.dataTransfer.files[0];
        if (file && file.type.startsWith('image/')) {
            const reader = new FileReader();
            reader.onloadend = () => {
                setImagePreview(reader.result);
            };
            reader.readAsDataURL(file);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
    
        // Ensure all required fields are present
        if (!Title || !Description || !Price || !PostImage) {
            alert("Please fill in all fields and upload an image.");
            return;
        }
    
        // Create a FormData object
        const formData = new FormData();
        formData.append("Title", Title);
        formData.append("Description", Description);
        formData.append("Price", Price);
        formData.append("PostImage", PostImage);
    
        try {
            // Make the API request
            const response = await fetch(`${API_URL}/post/create-post`, {
                method: "POST",
                credentials: "include",
                body: formData, 
            });
    
            if (response.ok) {
                const responseData = await response.json();
                setPostData(responseData);
                console.log(responseData);
                setIsModalOpen(false);
            } else {
                const error = await response.json();
                console.error(error);
                alert("Failed to create post.");
            }
        } catch (error) {
            console.error("Error:", error);
            alert("An unexpected error occurred.");
        }
    };
    

    return (
        <div className={`${isSidebarHidden ? 'col-span-5' : 'col-span-4'}`}>
            {/* Title + description section */}
            <div className='min-h-[200px] min-w-[350px] rounded-lg shadow bg-gray-100 mb-7'>
                <div onClick={onToggleSidebar}>
                    <ArrowLeftToLine />
                </div>
                <div className='text-2xl md:text- font-mono text-center text-gray-600 p-4'>
                    My Online Marketplace
                </div>

                <div className='text-center text-base md:text-lg font-thin px-4 pb-4'>
                    Discover a vibrant online marketplace where buyers and sellers connect seamlessly.
                </div>

                <div className='text-center'>
                    <button
                        onClick={() => setIsModalOpen(true)}
                        className='bg-blue-600 text-white px-6 py-2.5 rounded-lg hover:bg-blue-700 transition-colors duration-200 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2'
                    >
                        Create a post right now!
                    </button>
                </div>
            </div>

            {/* Modal Overlay */}
            {isModalOpen && (
                <div className="fixed inset-0 z-50 overflow-y-auto">
                    {/* Backdrop */}
                    <div
                        className="fixed inset-0 bg-black bg-opacity-50 backdrop-blur-sm transition-opacity"
                        onClick={() => setIsModalOpen(false)}
                    />

                    {/* Modal Content */}
                    <div className="relative min-h-screen flex items-center justify-center p-4">
                        <div className="relative bg-white rounded-xl shadow-xl max-w-xl w-full mx-auto">
                            {/* Close Button */}
                            <button
                                onClick={() => setIsModalOpen(false)}
                                className="absolute right-4 top-4 text-gray-500 hover:text-gray-700 focus:outline-none"
                            >
                                <X className="w-6 h-6" />
                            </button>

                            {/* Form Header */}
                            <div className="p-6 border-b border-gray-200">
                                <h2 className="text-2xl font-semibold text-gray-800">Create New Post</h2>
                            </div>

                            {/* Form Content */}
                            <form onSubmit={handleSubmit} className="p-6 space-y-6">
                                <div className="space-y-6">
                                    <div className="relative">
                                        <label
                                            htmlFor="postTitle"
                                            className="block text-sm font-medium text-gray-700 mb-2"
                                        >
                                            Title
                                        </label>
                                        <input
                                            id="postTitle"
                                            type="text"
                                            className="w-full px-4 py-2.5 rounded-lg border border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors duration-200"
                                            placeholder="Enter title"
                                            required
                                            value={Title}
                                            onChange={(e)=> setTitle(e.target.value)}
                                        />
                                    </div>

                                    <div className="relative">
                                        <label
                                            htmlFor="postDescription"
                                            className="block text-sm font-medium text-gray-700 mb-2"
                                        >
                                            Description
                                        </label>
                                        <textarea
                                            id="postDescription"
                                            rows={4}
                                            className="w-full px-4 py-2.5 rounded-lg border border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors duration-200"
                                            placeholder="Enter description"
                                            required
                                            value={Description}
                                            onChange={(e)=> setDescription(e.target.value)}
                                        />
                                    </div>

                                    <div className="relative">
                                        <label
                                            htmlFor="postPrice"
                                            className="block text-sm font-medium text-gray-700 mb-2"
                                        >
                                            Price
                                        </label>
                                        <div className="relative">
                                            <input
                                                id="postPrice"
                                                type="number"
                                                min="0"
                                                step="0.01"
                                                placeholder="0.00"
                                                className="w-full pl-4 pr-12 py-2.5 rounded-lg border border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-colors duration-200"
                                                required
                                                value={Price}
                                                onChange={(e) => setPrice(e.target.value)}
                                            />
                                            <div className="absolute inset-y-0 right-0 flex items-center pr-4 pointer-events-none">
                                                <span className="text-gray-500">Dh</span>
                                            </div>
                                        </div>
                                    </div>

                                    {/* Image Upload Section */}
                                    <div className="relative">
                                        <label
                                            htmlFor="imageUpload"
                                            className="block text-sm font-medium text-gray-700 mb-2"
                                        >
                                            Image
                                        </label>
                                        <div
                                            className={`relative border-2 border-dashed rounded-lg p-4 text-center ${dragActive ? 'border-blue-500 bg-blue-50' : 'border-gray-300'
                                                }`}
                                            onDragEnter={handleDrag}
                                            onDragLeave={handleDrag}
                                            onDragOver={handleDrag}
                                            onDrop={handleDrop}
                                        >
                                            <input
                                                id="imageUpload"
                                                type="file"
                                                accept="image/*"
                                                onChange={handleImageChange}
                                                className="hidden"
                                                required
                                            />

                                            {imagePreview ? (
                                                <div className="relative group">
                                                    <img
                                                        src={imagePreview}
                                                        alt="Preview"
                                                        className="mx-auto max-h-48 rounded-lg object-cover"
                                                    />
                                                    <label
                                                        htmlFor="imageUpload"
                                                        className="absolute inset-0 flex items-center justify-center bg-black bg-opacity-50 text-white opacity-0 group-hover:opacity-100 transition-opacity duration-200 rounded-lg cursor-pointer"
                                                    >
                                                        Click to change
                                                    </label>
                                                </div>
                                            ) : (
                                                <label
                                                    htmlFor="imageUpload"
                                                    className="flex flex-col items-center cursor-pointer p-4"
                                                >
                                                    <UploadCloud className="h-12 w-12 text-gray-400 mb-3" />
                                                    <span className="text-sm text-gray-600">
                                                        Drop your image here, or{' '}
                                                        <span className="text-blue-500 hover:text-blue-600">browse</span>
                                                    </span>
                                                    <span className="text-xs text-gray-500 mt-2">
                                                        PNG, JPG, GIF up to 10MB
                                                    </span>
                                                </label>
                                            )}
                                        </div>
                                    </div>
                                </div>

                                <div className="flex justify-end gap-4 pt-4">
                                    <button
                                        type="button"
                                        onClick={() => setIsModalOpen(false)}
                                        className="px-4 py-2 text-sm font-medium text-gray-700 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
                                    >
                                        Cancel
                                    </button>
                                    <button
                                        type="submit"
                                        className="bg-blue-600 text-white py-2 px-4 rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-colors duration-200"
                                    >
                                        Create Post
                                    </button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            )}

            {/* Posts Section */}
            <div className={`${isSidebarHidden ? 'grid grid-col-1 sm:grid-cols-2 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-3' : 'grid grid-cols-1 lg:grid-cols-3 gap-3'}`}>
                {/* Your existing post cards here */}
            </div>
        </div>
    )
}

export default Posts