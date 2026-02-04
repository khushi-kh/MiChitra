import { useEffect, useState } from "react";
import api from "../api/axios";
import Navbar from "../components/navbar";
import MovieGrid from "../components/moviegrid";
import Footer from "../components/footer";

const MoviesPage = () => {
    const isAuthenticated = !!localStorage.getItem("token");
    const [scrolled, setScrolled] = useState(false);
    const [movies, setMovies] = useState([]);

    // Navbar scroll effect
    useEffect(() => {
        const handleScroll = () => {
            setScrolled(window.scrollY > 50);
        };
        window.addEventListener("scroll", handleScroll);
        return () => window.removeEventListener("scroll", handleScroll);
    }, []);

    // Fetch movies from backend
    useEffect(() => {
        api
            .get("/movies")
            .then((res) => {
                setMovies(res.data);
            })
            .catch((err) => console.error("Movie API error:", err));
    }, []);

    return (
        <div className="container">
            <div className="bg-gradient" />
            <div className="noise-overlay" />

            {/* Navigation */}
            <Navbar isAuthenticated={isAuthenticated} scrolled={scrolled} />

            {/* Movies Grid */}
            <MovieGrid movies={movies} />

            {/* Footer */}
            <Footer />

        </div>    
    );
};

export default MoviesPage;