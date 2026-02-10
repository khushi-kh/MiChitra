import { BrowserRouter, Routes, Route } from "react-router-dom";
import Register from "./pages/Register";
import Login from "./pages/Login";
import HomePage from "./pages/HomePage";
import MoviesPage from "./pages/moviesPage";
import MovieDetails from "./pages/movieDetails";
import Booking from "./pages/booking"


function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/register" element={<Register />} />
                <Route path="/login" element={<Login />} />
                <Route path="/movies" element={<MoviesPage />} />
                <Route path="/movies/:id" element={<MovieDetails />} />
                <Route path="/booking/:movieId" element={<Booking />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;
