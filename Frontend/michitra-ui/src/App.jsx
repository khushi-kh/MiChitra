import { BrowserRouter, Routes, Route } from "react-router-dom";
import Movies from "./pages/Movies";
import Register from "./pages/Register";

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<Movies />} />
                <Route path="/register" element={<Register />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;
