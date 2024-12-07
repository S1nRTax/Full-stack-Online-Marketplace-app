import { BrowserRouter, Routes, Route } from "react-router-dom";
import Register from "./Pages/Register";
import Login from "./Pages/Login";
import Layout from "./components/Layout/Layout";
import Home from "./Pages/Home";
import NotFound from "./Pages/NotFound";
import CreateShopForm from "./Pages/createShopForm";
import { AuthProvider, useAuth } from "./Context/AuthContext";

function App() {
  return (
    <AuthProvider>
      <div className="flex flex-col min-h-screen">
        <BrowserRouter>
          <AppRoutes />
        </BrowserRouter>
      </div>
    </AuthProvider>
  );
}

function AppRoutes() {
  const { isLoading } = useAuth();

  // Display a loading spinner while the authentication state is being resolved
  if (isLoading) {
    return (
      <div className="flex justify-center items-center min-h-screen">
        <div className="spinner">Loading...</div>
      </div>
    );
  }

  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<Home />} />
        <Route path="/register" element={<Register />} />
        <Route path="/login" element={<Login />} />
        <Route path="/create-shop" element={<CreateShopForm />} />
        <Route path="*" element={<NotFound />} />
      </Route>
    </Routes>
  );
}

export default App;
