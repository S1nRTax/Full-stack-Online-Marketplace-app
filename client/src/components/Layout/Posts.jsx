import React, { useState } from 'react'


const Posts = ({onToggleSidebar , isSidebarHidden}) => {
        

  return (
        <div className={`${isSidebarHidden ? 'col-span-5' : 'col-span-4'}`}>

                {/* Title + description section */}
                <div className='min-h-[150px] w-full rounded-lg shadow bg-gray-100 mb-7'>
                    <div className='text-2xl md:text- font-mono text-center text-gray-600 p-4'>
                        My Online Marketplace
                    </div>

                    <div className='text-center text-base md:text-lg font-thin px-4 pb-4'>
                        Discover a vibrant online marketplace where buyers and sellers connect seamlessly.
                    </div>
                    <div className='text-center'>
                    <button onClick={onToggleSidebar}>
                        hide sidebar
                    </button>
              </div>
                </div>

                
                    {/* Posts Section */}
                     <div className='aspect-square rounded-lg shadow bg-gray-100'>
                        {/** Post header section */}
                        <div className='w-full h-[15%] bg-gray-500'>
                            {/** Profile picture */}
                                <div className='relative mx-auto w-32 h-32'>
                                    <img src="" alt="" className='w-10 h-10 rounded-full border-4 border-gray-200 overflow-hidden' />
                                </div>
                        </div>
                    </div>
      
        </div>
  )
}

export default Posts