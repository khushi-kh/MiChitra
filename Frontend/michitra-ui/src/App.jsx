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
import TheatrePage from "./pages/theatrePage";
import TheatreDetails from "./pages/theatreDetails";
import CitiesPage from "./pages/citiesPage";
import AdminDashboard from "./pages/adminDashboard";
import ManageMovies from "./pages/manageMovies";
import MovieForm from "./pages/movieForm";
import ManageTheatres from "./pages/manageTheatres";
import ManageShows from "./pages/manageShows";
import ManageBookings from "./pages/manageBookings";
import ManageUsers from "./pages/manageUsers";
import TheatreForm from "./pages/theatreForm";
import ShowForm from "./pages/showForm";
import ForgotPassword from "./pages/forgotPassword";
import ResetPassword from "./pages/resetPassword";
import ProtectedRoute from "./components/ProtectedRoute"; 


function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/register" element={<Register />} />
                <Route path="/login" element={<Login />} />
                <Route path="/forgot-password" element={<ForgotPassword />} />
                <Route path="/reset-password" element={<ResetPassword />} />
                <Route path="/movies" element={<MoviesPage />} />
                <Route path="/movies/:id" element={<MovieDetails />} />
                <Route path="/booking/:movieId" element={<Booking />} />
                <Route path="/booking-confirmation" element={<BookingConfirmation />} />
                <Route path="/profile" element={<Profile />} />
                <Route path="/edit-profile" element={<EditProfile />} />
                <Route path="/my-bookings" element={<MyBookings />} />
                <Route path="/theatres" element={<TheatrePage />} />
                <Route path="/theatres/:id" element={<TheatreDetails />} />
                <Route path="/cities" element={<CitiesPage />} />
                <Route path="/admin" element={
                    <ProtectedRoute requiredRole="Admin">
                        <AdminDashboard />
                    </ProtectedRoute>
                } />
                <Route path="/admin/movies" element={
                    <ProtectedRoute requiredRole="Admin">
                        <ManageMovies />
                    </ProtectedRoute>
                } />
                <Route path="/admin/movies/add" element={
                    <ProtectedRoute requiredRole="Admin">
                        <MovieForm />
                    </ProtectedRoute>
                } />
                <Route path="/admin/movies/edit/:id" element={
                    <ProtectedRoute requiredRole="Admin">
                        <MovieForm />
                    </ProtectedRoute>
                } />
                <Route path="/admin/theatres" element={
                    <ProtectedRoute requiredRole="Admin">
                        <ManageTheatres />
                    </ProtectedRoute>
                } />
                <Route path="/admin/theatres/add" element={
                    <ProtectedRoute requiredRole="Admin">
                        <TheatreForm />
                    </ProtectedRoute>
                } />
                <Route path="/admin/theatres/edit/:id" element={
                    <ProtectedRoute requiredRole="Admin">
                        <TheatreForm />
                    </ProtectedRoute>
                } />
                <Route path="/admin/shows" element={
                    <ProtectedRoute requiredRole="Admin">
                        <ManageShows />
                    </ProtectedRoute>
                } />
                <Route path="/admin/shows/add" element={
                    <ProtectedRoute requiredRole="Admin">
                        <ShowForm />
                    </ProtectedRoute>
                } />
                <Route path="/admin/shows/edit/:id" element={
                    <ProtectedRoute requiredRole="Admin">
                        <ShowForm />
                    </ProtectedRoute>
                } />
                <Route path="/admin/bookings" element={
                    <ProtectedRoute requiredRole="Admin">
                        <ManageBookings />
                    </ProtectedRoute>
                } />
                <Route path="/admin/users" element={
                    <ProtectedRoute requiredRole="Admin">
                        <ManageUsers />
                    </ProtectedRoute>
                } />
            </Routes>
        </BrowserRouter>
    );
}

export default App;
