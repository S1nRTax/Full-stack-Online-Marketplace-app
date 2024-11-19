import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'; // Import Routes and Route
import Login from './components/auth/Login';
import Register from './components/auth/Register';

const App = () => {
  return (
    <Router>
      <Routes>  
        <Route path="/login" element={<Login />} />  
        <Route path="/register" element={<Register />} />  
        {/* Add other routes like dashboard */}
      </Routes>
    </Router>
  );
};

export default App;
