import React, { useState } from 'react'
import Sidebar from '../components/Layout/sidebar'
import Posts from '../components/Layout/Posts'

const Home = () => {
      const [shouldHideSideBar , setShouldHideSideBar] = useState(false);
    

  return (
    <div className=''>

      {/** Main section grid-cols-[100px_3fr] sm:grid-cols-[300px_3fr] */}
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