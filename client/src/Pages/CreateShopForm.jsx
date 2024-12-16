import React, { useState , useEffect } from 'react'
import { useVendor } from '../Context/TranstitionContext';
import { Navigate } from 'react-router-dom';


const CreateShopForm = () => {
  
  const [shopName , setShopName] = useState("");
  const [shopAddress , setShopAddress] = useState("");
  const [shopDescription , setShopDescription] = useState("");
  const { transitionToVendor, validateVendorStatus , isVendor ,setIsVendor } = useVendor();


        const handleBecomeVendor = async (e) => {
            e.preventDefault();
            const response = await transitionToVendor(shopName, shopAddress, shopDescription);
             console.log(isVendor);
             if(response.isVendor){
                setIsVendor(true);
             }
        };

        useEffect(() => {
            validateVendorStatus();
        }, [validateVendorStatus]);

        
        if (isVendor) {
            return <Navigate to="/profile" replace />;
        }

    return (
      <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8 bg-white p-8 rounded-lg shadow-md">
          {/** Title */}
          <div>
              <h2 className="text-center text-3xl font-extrabold text-gray-900 mb-6">Create Your Shop!</h2>
          </div>
          {/** Form */}
          <form className="w-full" onSubmit={handleBecomeVendor}>
              <div className="space-y-4">
                  {/* Shop Name Input */}
                  <div>
                      <label htmlFor="shop-name" className="block text-sm font-medium text-gray-700">Shop Name</label>
                      <input 
                          id="shop-name"
                          name="shop-name"
                          type="text" 
                          required 
                          className="mt-1 block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md" 
                          placeholder="Enter your shop name"
                          onChange={(e) => setShopName(e.target.value)} 
                      />
                  </div>
                  
                  {/** Shop Address */}
                  <div>
                      <label htmlFor="shop-address" className="block text-sm font-medium text-gray-700">Shop Address</label>
                      <input 
                          id="shop-address"
                          name="shop-address"
                          type="text" 
                          required 
                          className="mt-1 block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md" 
                          placeholder="Enter your shop address"
                          onChange={(e) => setShopAddress(e.target.value)} 
                      />
                  </div>
  
                  {/** Shop Description */}
                  <div>
                      <label htmlFor="shop-description" className="block text-sm font-medium text-gray-700">Shop Description</label>
                      <textarea 
                          id="shop-description"
                          name="shop-description"
                          required
                          rows={3}
                          className="mt-1 block w-full px-3 py-2 border border-gray-300 placeholder-gray-500 text-gray-900 focus:outline-none focus:ring-blue-500 focus:border-blue-500 sm:text-sm rounded-md" 
                          placeholder="Describe your shop"
                          onChange={(e) => setShopDescription(e.target.value)} 
                      />
                  </div>
  
                  {/* Submit button */}
                  <button type="submit" className="mt-6 w-full text-white bg-blue-700 hover:bg-blue-800 focus:ring-4 focus:outline-none focus:ring-blue-300 font-medium rounded-lg text-sm px-5 py-2.5" >
                      Create Shop
                  </button>
              </div>
          </form>
      </div>
  </div>
  )
}

export default CreateShopForm;