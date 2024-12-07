import React from 'react'
import { useAuth } from '../Context/AuthContext';


const Home = () => {

  const { isLoggedin, user  } = useAuth();



  return (
    <div className="container mx-auto px-4 py-8">
      <h1 className="text-3xl font-bold mb-4">Welcome to Our Website</h1>
      
      {isLoggedin ? (
            
              <div>hello {user.username}, You're email : {user.email}</div>
            
          ) : (
            <div>
              Hello and welcome to my website please consider login.
            </div>
          )}
        
     
    </div>
  )
}

export default Home