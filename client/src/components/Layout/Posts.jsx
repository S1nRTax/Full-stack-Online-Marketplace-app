import React, { useState } from 'react'
import { ArrowLeftToLine } from 'lucide-react'
import { AiOutlineLike, AiOutlineArrowRight } from 'react-icons/ai';
import { FiShoppingCart } from 'react-icons/fi';

const Posts = ({onToggleSidebar , isSidebarHidden}) => {
        

  return (
        <div className={`${isSidebarHidden ? 'col-span-5' : 'col-span-4'}`}>

                {/* Title + description section */}
                <div className='min-h-[150px] min-w-[350px] rounded-lg shadow bg-gray-100 mb-7'>
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
              </div>
                </div>

                
                    {/* Posts Section */}
                    
                    <div className={`${isSidebarHidden ? 'grid grid-col-1 sm:grid-cols-2 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-3' : 'grid grid-cols-1 lg:grid-cols-3 gap-3'}`}>
                        <div className="min-h-[500px] min-w-[350px] rounded-lg shadow bg-gray-100 mb-7">
                                {/* Head Section */}
                                <div className="flex items-center p-4 border-b border-gray-200">
                                    <img
                                    src=""
                                    alt="Profile Picture"
                                    className="w-12 h-12 rounded-full object-cover"
                                    />
                                    <div className="ml-4">
                                    <h2 className="font-bold text-lg">Username</h2>
                                    <p className="text-sm text-gray-600">Created 1 hour ago</p>
                                    </div>
                                    <div className="ml-auto text-center">
                                    <h2 className="font-bold text-lg">BWM M5 CS at sell best price no cap</h2>
                                    <p className="text-sm text-gray-600">1000+ sales</p>
                                    </div>
                                </div>

                                {/* Main Section */}
                                <div className="p-4">
                                    <img
                                    src=""
                                    alt="Post Image"
                                    className="w-full h-[300px] object-cover"
                                    />
                                </div>

                                {/* Footer Section */}
                                <div className="flex items-center p-4 border-t border-gray-200">
                                    <button className="flex items-center text-blue-600 hover:text-blue-800">
                                    <AiOutlineLike size={20} className="mr-2" />
                                    Upvote
                                    </button>
                                    <button className="flex items-center text-blue-600 hover:text-blue-800 mx-4">
                                    <AiOutlineArrowRight size={20} className="mr-2" />
                                    See More
                                    </button>
                                    <p className="text-lg font-bold ml-auto"> $10.99 </p>
                                </div>
                        </div>
                    </div>
      


        </div>
  )
}

export default Posts