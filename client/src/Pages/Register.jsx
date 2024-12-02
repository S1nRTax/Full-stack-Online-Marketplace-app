import React, { useState } from 'react'
import  { Navigate } from 'react-router-dom'


const Register = () => {
    const [name, setName] = useState("");
    const [username, setUsername] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [errorMessage, setErrorMessage] = useState(null); 
    const [isRegistered, setIsRegistered] = useState(false);
    const userType = "Customer";

    async function handleRegisterSubmit() {
        try {
            const item = { name, username, email, password, userType };
            const result = await fetch('https://localhost:7262/api/auth/register', {
                method: 'POST',
                headers: {
                    'Accept': 'application/json',
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(item),
            });
            
            if(result.ok){
                setIsRegistered(true);
            }else{
                const errorData = await result.json();
                setErrorMessage({
                    message: errorData.message,
                    errors: errorData.errors || [],
                });
                return;
            }

            const data = await result.json();
            console.log(data);

                
        } catch (error) {
            console.error('Error during fetch:', error);
            setErrorMessage({
                message: "Something went wrong. Please try again.",
                errors: [],
            });
        }   
    }

    if (isRegistered) {
        return <Navigate to="/login" replace />;
    }

    return (
        <div>
            <div>
                    <div>
                        <h2>Register</h2>
                    </div>
                    <div>
                        <label>Name:</label>
                        <input type="text" placeholder="name" onChange={(e) => setName(e.target.value)} /> <br />
                        <label>Username:</label>
                        <input type="text" placeholder="username" onChange={(e) => setUsername(e.target.value)} /><br />
                        <label>Email:</label>
                        <input type="text" placeholder="email" onChange={(e) => setEmail(e.target.value)} /><br />
                        <label>Password:</label>
                        <input type="password" placeholder="password" onChange={(e) => setPassword(e.target.value)} /><br />
                        <input type="submit" onClick={handleRegisterSubmit} />
                    </div>
                

                {/* Display error messages */}
                {errorMessage && (
                    <div style={{ color: 'red', marginTop: '10px' }}>
                        {errorMessage.errors.length > 0 && (
                            <ul>
                                {errorMessage.errors.map((error, index) => (
                                    <li key={index}>{error}</li>
                                ))}
                            </ul>
                        )}
                    </div>
                )}
            </div>
        </div>
    );
};

export default Register;
