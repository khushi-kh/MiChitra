import { BrowserRouter, Routes, Route } from "react-router-dom";
import Register from "./pages/Register";
import Login from "./pages/Login";
import HomePage from "./pages/HomePage";
import MoviesPage from "./pages/moviesPage";
import MovieDetails from "./pages/movieDetails";
import Booking from "./pages/booking";
import BookingConfirmation from "./pages/bookingConfirmation";
import Profile from "./pages/profile";
import EditProfile from "./pages/editProfile";
import MyBookings from "./pages/myBookings";


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
                <Route path="/booking-confirmation" element={<BookingConfirmation />} />
                <Route path="/profile" element={<Profile />} />
                <Route path="/edit-profile" element={<EditProfile />} />
                <Route path="/my-bookings" element={<MyBookings />} />
            </Routes>
        </BrowserRouter>
    );
}

export default App;
