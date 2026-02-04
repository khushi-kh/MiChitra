import { useEffect, useState } from "react";
import api from "../api/axios";
import "../index.css";

const UserHomePage = () => {
    const [scrolled, setScrolled] = useState(false);
    const [user, setUser] = useState(null);
    const [movies, setMovies] = useState([]);

    // Navbar scroll effect
    useEffect(() => {
        const handleScroll = () => {
            setScrolled(window.scrollY > 50);
        };
        window.addEventListener("scroll", handleScroll);
        return () => window.removeEventListener("scroll", handleScroll);
    }, []);

    // Fetch user from backend
    useEffect(() => {
        const userId = localStorage.getItem("userId");
        if (!userId) return;

        api.get(`/users/${userId}`)
            .then(res => setUser(res.data))
            .catch(err => console.error("User API error:", err));
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
        <div className="movie-booking-container">
            {/* Background Effects */}
            <div className="bg-gradient" />
            <div className="noise-overlay" />

            {/* Navigation */}
            <nav
                className="nav"
                style={{
                    background: scrolled
                        ? "rgba(10, 10, 15, 0.95)"
                        : "rgba(10, 10, 15, 0.8)",
                }}
            >
                <div>
                    <a href="/" className="logo">MiChitra</a>
                </div>
                <ul className="nav-links">
                    <li><a href="#movies" className="nav-link">Movies</a></li>
                    <li><a href="#theatres" className="nav-link">Theatres</a></li>
                    <li><a href="#citiesdropdown" className="nav-link">Cities</a></li>
                    <li className="nav-link">{user?.fName}</li>
                </ul>
            </nav>

            { /* Main Content */}
            <section className="hero">
                <div className="hero-content">
                    <span className="hero-badge">✨ Premium Cinema Experience</span>
                    <h1 className="hero-title">Hello {user?.fName}</h1>
                    <p className="hero-text">
                        Discover the latest blockbusters, reserve premium seats, and enjoy
                        exclusive benefits. Your cinematic journey starts here.
                    </p>
                    <div className="hero-buttons">
                        <a href="#movies" className="btn-primary">Browse Movies</a>
                    </div>
                </div>
            </section>

            {/* Movie Section */}
            <section className="movie-showcase" id="movies">
                <h2 className="showcase-title">Now Showing</h2>
                <div className="showcase-grid">
                    {movies.length === 0 && (
                        <p style={{ color: "#aaa" }}>Loading movies...</p>
                    )}

                    {movies.map((movie) => (
                        <div key={movie.MovieId} className="movie-card">
                            <div className="movie-poster">
                                <span className="movie-poster-icon">🎬</span>
                            </div>
                            <div className="movie-info">
                                <div className="movie-title">{movie.movieName}</div>
                                <div className="movie-meta">
                                    {movie.language} • {movie.rating}
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            </section>

            {/* CTA Section */}
            <section className="cta-section">
                <div className="cta-container">
                    <div className="cta-content">
                        <h2 className="cta-title">Ready for the Show?</h2>
                        <p className="cta-subtitle">
                            Book your tickets now and immerse yourself in the ultimate movie experience.
                        </p>
                        <a href="#movies" className="btn-primary">Book Now</a>
                    </div>
                </div>
            </section>

            {/* Footer */}
            <footer className="footer">
                <div className="footer-content">
                    <div className="footer-brand">
                        <div className="footer-logo">MiChitra</div>
                        <p className="footer-desc">
                            Your premier destination for unforgettable movie experiences.
                            Book smarter, watch better.
                        </p>
                    </div>

                    <div>
                        <h4 className="footer-heading">Company</h4>
                        <ul className="footer-links">
                            <li><a href="#about" className="footer-link">About Us</a></li>
                            <li><a href="#careers" className="footer-link">Careers</a></li>
                            <li><a href="#press" className="footer-link">Press</a></li>
                            <li><a href="#blog" className="footer-link">Blog</a></li>
                        </ul>
                    </div>

                    <div>
                        <h4 className="footer-heading">Support</h4>
                        <ul className="footer-links">
                            <li><a href="#help" className="footer-link">Help Center</a></li>
                            <li><a href="#contact" className="footer-link">Contact Us</a></li>
                            <li><a href="#privacy" className="footer-link">Privacy Policy</a></li>
                            <li><a href="#terms" className="footer-link">Terms of Service</a></li>
                        </ul>
                    </div>

                    <div>
                        <h4 className="footer-heading">Connect</h4>
                        <ul className="footer-links">
                            <li><a href="#twitter" className="footer-link">Twitter</a></li>
                            <li><a href="#instagram" className="footer-link">Instagram</a></li>
                            <li><a href="#facebook" className="footer-link">Facebook</a></li>
                            <li><a href="#youtube" className="footer-link">YouTube</a></li>
                        </ul>
                    </div>
                </div>

                <div className="footer-bottom">
                    <p>© 2026 MiChitra. All rights reserved.</p>
                </div>
            </footer>
        </div>
          );
};

    export default UserHomePage;
