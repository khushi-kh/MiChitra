import { BrowserRouter, Routes, Route } from "react-router-dom";
import Movies from "./pages/landingpage";
import Register from "./pages/Register";
import Login from "./pages/Login";
import UserHomePage from "./pages/userHome";


function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Movies />} />
                <Route path="/register" element={<Register />} />
                <Route path="/login" element={<Login />} />
                <Route path="/home" element={<UserHomePage />} />

            </Routes>
        </BrowserRouter>
    );
}

export default App;
