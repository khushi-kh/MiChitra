import { useEffect, useState } from "react";
import { useParams, Link } from "react-router-dom";
import Navbar from "../components/navbar";
import api from "../api/axios";
import "../styles/theatreDetails.css";
import Footer from "../components/footer";

const TheatreDetails = () => {
    const isAuthenticated = !!localStorage.getItem("token");
    const [scrolled, setScrolled] = useState(false);
    const [theatre, setTheatre] = useState(null);
    const [movieShows, setMovieShows] = useState([]);
    const { id } = useParams();

    useEffect(() => {
        const handleScroll = () => {
            setScrolled(window.scrollY > 50);
        };
        window.addEventListener("scroll", handleScroll);
        return () => window.removeEventListener("scroll", handleScroll);
    }, []);

    useEffect(() => {
        api.get(`/theatres/${id}`)
            .then((res) => setTheatre(res.data))
            .catch((err) => console.error("Theatre API error:", err));

        api.get(`/movieshows/theatre/${id}`)
            .then((res) => {
                const now = new Date();
                const activeShows = res.data.filter(show => new Date(show.showTime) > now);
                const uniqueMovies = activeShows.reduce((acc, show) => {
                    if (!acc.find(m => m.movieId === show.movieId)) {
                        acc.push(show);
                    }
                    return acc;
                }, []);
                setMovieShows(uniqueMovies);
            })
            .catch((err) => console.error("Movie shows API error:", err));
    }, [id]);

    return (
        <div className="theatre-details-container">
            <div className="bg-gradient" />
            <div className="noise-overlay" />

            <Navbar isAuthenticated={isAuthenticated} scrolled={scrolled} />

            <div className="theatre-details-content">
                {theatre && (
                    <div className="theatre-header">
                        <h1>{theatre.name}</h1>
                        <p className="theatre-location">{theatre.city}</p>
                    </div>
                )}

                <div className="movies-section">
                    <h2>Available Movies</h2>
                    {movieShows.length > 0 ? (
                        <div className="theatre-movies-grid">
                            {movieShows.map((show) => (
                                <Link to={`/booking/${show.movieId}?theatreId=${id}`} key={show.movieId} className="movie-card">
                                    <img src="https://picsum.photos/300/400" alt={show.movieName} />
                                    <div className="movie-card-info">
                                        <h3>{show.movieName}</h3>
                                        <p className="movie-price">From ₹{show.pricePerSeat}</p>
                                    </div>
                                </Link>
                            ))}
                        </div>
                    ) : (
                        <p className="no-movies">No movies currently showing at this theatre.</p>
                    )}
                </div>
            </div>

            <Footer />
        </div>
    );
};

export default TheatreDetails;
