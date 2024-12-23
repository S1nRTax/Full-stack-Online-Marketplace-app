import React from 'react'



const Home = () => {
  return (
    <div className=''>

    {/** Main section */}
    <div className='grid grid-cols-[300px_3fr] gap-4 min-h-screen p-4'>

      {/** Side Bar section */}
        <div className='row-span-2 '>
          <div className='min-h-full w-full rounded-lg shadow bg-gray-100'>
          {/* Sidebar content */}
          </div>
        </div>

      <div className=''>
          {/* Title + description section */}
          <div className='min-h-[150px] w-full rounded-lg shadow bg-gray-100 mb-7'>
            <div className='text-3xl md:text-5xl font-mono text-center text-gray-600 p-4'>
              My Online Marketplace
            </div>
            <div className='text-center text-base md:text-lg font-thin px-4 pb-4'>
            Discover a vibrant online marketplace where buyers and sellers connect seamlessly.
            </div>
          </div>

          {/** Posts grid section */}
          <div className='grid grid-cols-3 gap-4'>
            {/* Post 1 */}
              <div className='aspect-square rounded-lg shadow bg-gray-100'>
              POST NUM 1
              </div>
            {/* Post 2 */}
              <div className='aspect-square rounded-lg shadow bg-gray-100'>
              POST NUM 2
              </div>
            {/* Post 3 */}
              <div className='aspect-square rounded-lg shadow bg-gray-100'>
              POST NUM 3
              </div>
          </div>
      </div>

    </div>


</div>
  )
}

export default Home