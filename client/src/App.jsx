import { BrowserRouter, Routes, Route } from "react-router-dom";
import Register from "./Pages/Register"
import Login from "./Pages/Login";
import Layout from "./components/Layout/Layout";
import Home from "./Pages/Home";
import NotFound from "./Pages/NotFound";

function App() {


  return (
    <div>
          <BrowserRouter>
                <Routes>
                           {/* Public routes : */}
                    <Route path="/" element={<Layout />}>
                        <Route index element={<Home />} />
                        <Route path="/register" element={< Register />} />
                        <Route path="/login" element={< Login />} />
                          {/* 404 PAGE : */}
                        <Route path="*" element={<NotFound />} />
                    </Route>
                </Routes>
            </BrowserRouter>
    </div>
    
        
   
  )
}

export default App
