import { useEffect, useState } from "react";
import api from "../api/axios";
import Navbar from "../components/navbar";
import Hero from "../components/hero";
import MovieGrid from "../components/moviegrid";
import Footer from "../components/footer";
import "../styles/homePage.css"

const HomePage = () => {
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

    // Featurs Map
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
            <div className="container">
                <div className="bg-gradient" />
                <div className="noise-overlay" />

                {/* Navigation */}
                <Navbar isAuthenticated={isAuthenticated} scrolled={scrolled} />

                {/* Hero Content */}
                <Hero isAuthenticated={isAuthenticated} />

                {/*Features Section (only if user is not authenticated */}
                {!isAuthenticated && (
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
                            {features.map((feature) => (
                                <div key={feature.title} className="feature-card">
                                    <div className="feature-icon">{feature.icon}</div>
                                    <h3 className="feature-title">{feature.title}</h3>
                                    <p className="feature-desc">{feature.description}</p>
                                </div>
                            ))}
                        </div>
                    </section>
                )}

                {/* Movies Grid */}
                <MovieGrid movies={movies} />

                {/* CTA Section */}
                <section className="cta-section">
                    <div className="cta-container">
                        <div className="cta-content">
                            <h2 className="cta-title">Ready for the Show?</h2>

                            {!isAuthenticated ? (
                                <>
                                    <p className="cta-subtitle">
                                        Join thousands of movie lovers and start booking your tickets
                                        today. Get exclusive early access to blockbusters.
                                    </p>
                                    <a href="/register" className="btn-primary">
                                        Get Started Now
                                    </a>
                                </>
                            ) : (
                                <>
                                    <p className="cta-subtitle">
                                        Book your tickets now and immerse yourself in the ultimate movie experience.
                                    </p>
                                    <a href="/movies" className="btn-primary">
                                        Explore Movies
                                    </a>
                                </>
                            )}
                        </div>
                    </div>
                </section>

                {/* Footer */}
                <Footer /> 

            </div>
        );
    };

export default HomePage;