import React, { useState } from 'react'
import { Navigate, useNavigate } from 'react-router-dom';

const Login = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [isLoggedin , setIsLoggedin] = useState(false);
    const [errorMessage , setErrorMessage] = useState(null);

    async function handleLoginSubmit(){
        try {
        const item = {email , password}
        const result =  await fetch('https://localhost:7262/api/auth/login' , 
            {
                method: 'POST',
                headers: {
                    'Accept':'application/json',
                    'Content-Type':'application/json',
                },
                body: JSON.stringify(item)
            });
        
            if(result.ok){
                // well store the user token and user info here in the future
                setIsLoggedin(true);
            }else{
                const errorData = await result.json();
                setErrorMessage({
                    message: errorData.message || "login failed",
                });
                return;
            }
        }catch(error){
            console.log("Error during fetch :", error);
            setErrorMessage({
                message: "Something wen't wrong please try again !",
            });
        }
    }

    if(isLoggedin){
        return <Navigate to="/" replace />;
    }

  return (
    <div>
        <p>Login page : </p>
        <label> Email</label>
        <input type="text" placeholder='email' onChange={(e)=>setEmail(e.target.value)} />
        <label>Password</label>
        <input type="password" placeholder='password' onChange={(e) => setPassword(e.target.value)} />
        <input type="submit" onClick={handleLoginSubmit} />

        {/* Diplaying the errors :  */}
        {errorMessage && ( 
        <div style={{ color: 'red', marginTop: '10px' }}>
                <p>{errorMessage.message}</p>
            </div>
            )}
    </div>
    
  )
}

export default Login;