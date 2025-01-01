import React, { useState } from 'react'
import Sidebar from '../components/Layout/sidebar'
import Posts from '../components/Layout/Posts'

const Home = () => {
      const [shouldHideSideBar , setShouldHideSideBar] = useState(false);
    

  return (
    <div className=''>

      {/** Main section */}
        <div className='grid grid-cols-5 gap-4 min-h-screen '>

        {!shouldHideSideBar && <Sidebar />}
        <Posts onToggleSidebar={() => setShouldHideSideBar(!shouldHideSideBar)} 
                isSidebarHidden={shouldHideSideBar}
          />
            

        </div>


  </div>
  )
}

export default Home