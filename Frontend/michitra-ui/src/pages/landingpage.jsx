import { useEffect, useState } from "react";
import api from "../api/axios";
import "../styles/landingpage.css";

const MovieBookingLanding = () => {
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

    const features = [
        {
            icon: "🎟️",
            title: "Instant Booking",
            description:
                "Reserve your seats in seconds with our lightning-fast booking system. No queues, no hassle.",
        },
        {
            icon: "💺",
            title: "Premium Seats",
            description:
                "Choose from luxury recliners, VIP lounges, and premium seating options for ultimate comfort.",
        },
        {
            icon: "🍿",
            title: "Food & Beverages",
            description:
                "Pre-order gourmet snacks and drinks delivered right to your seat before the show starts.",
        },
    ];

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
                <div className="logo">MiChitra</div>
                <ul className="nav-links">
                    <li><a href="#movies" className="nav-link">Movies</a></li>
                    <li><a href="#theatres" className="nav-link">Theatres</a></li>
                    <li><a href="#cities" className="nav-link">Cities</a></li>
                    <li><a href="/register" className="nav-cta">Sign Up</a></li>
                </ul>
            </nav>

            {/* Hero Section */}
            <section className="hero">
                <div className="hero-content">
                    <span className="hero-badge">✨ Premium Cinema Experience</span>
                    <h1 className="hero-title">Book Your Perfect Movie Night</h1>
                    <p className="hero-text">
                        Discover the latest blockbusters, reserve premium seats, and enjoy
                        exclusive benefits. Your cinematic journey starts here.
                    </p>
                    <div className="hero-buttons">
                        <a href="#browse" className="btn-primary">Browse Movies</a>
                        <a href="#learn" className="btn-secondary">Learn More</a>
                    </div>
                </div>
            </section>

            {/* Movie Showcase */}
            <section className="movie-showcase" id="movies">
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

            {/* Features Section */}
            <section className="features">
                <div className="section-header">
                    <span className="section-badge">Why Choose MiChitra</span>
                    <h2 className="section-title">Premium Features</h2>
                    <p className="section-subtitle">
                        Experience cinema like never before with our cutting-edge technology
                        and exclusive perks.
                    </p>
                </div>

                <div className="features-grid">
                    {features.map((feature, index) => (
                        <div key={index} className="feature-card">
                            <div className="feature-icon">{feature.icon}</div>
                            <h3 className="feature-title">{feature.title}</h3>
                            <p className="feature-desc">{feature.description}</p>
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
                            Join thousands of movie lovers and start booking your tickets
                            today. Get exclusive early access to blockbusters.
                        </p>
                        <a href="/register" className="btn-primary">Get Started Now</a>
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

export default MovieBookingLanding;
