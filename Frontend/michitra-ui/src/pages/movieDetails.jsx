import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import Navbar from "../components/navbar";
import api from "../api/axios";
import "../styles/movieDetails.css";
import Footer from "../components/footer";

const MovieDetails = () => {
    const isAuthenticated = !!localStorage.getItem("token");
    const [scrolled, setScrolled] = useState(false);
    const [movie, setMovie] = useState(null);
    const { id } = useParams();

    // Navbar scroll effect
    useEffect(() => {
        const handleScroll = () => {
            setScrolled(window.scrollY > 50);
        };
        window.addEventListener("scroll", handleScroll);
        return () => window.removeEventListener("scroll", handleScroll);
    }, [id]);

    // Fetch movie details from backend
    useEffect(() => {
        api
            .get(`/movies/${id}`)
            .then((res) => {
                setMovie(res.data);
            })
            .catch((err) => console.error("Movie API error:", err));
    }, [id]);


    return (
        <div className="movie-details-container">
            <div className="bg-gradient" />
            <div className="noise-overlay" />
        
            {/* Navigation */ }
            <Navbar isAuthenticated={isAuthenticated} scrolled={scrolled} />

            { /* Movie Details */}
            <div className="movie-details-page-content">
            {movie ? (
                <div className="movie-details">
                    <img src="https://picsum.photos/350/500" alt="movie poster" className="movie-img" />
                    <div className="movie-info">
                        <h1>{movie.movieName}</h1>
                        <p>{movie.description}</p>
                        <div className="movie-meta">
                            <div className="meta-item">
                                <span className="meta-label">Language</span>
                                <span className="meta-value">{movie.language}</span>
                            </div>
                            <div className="meta-item">
                                <span className="meta-label">Rating</span>
                                <span className="meta-value">{movie.rating}</span>
                            </div>
                        </div>
                        <button className="book-button">Book Now</button>
                    </div>
                </div>
            ) : (
                <p>Loading movie details...</p>
            )}

            </div>

            { /* Footer */}
            <Footer />
        </div>
    )

}

export default MovieDetails;