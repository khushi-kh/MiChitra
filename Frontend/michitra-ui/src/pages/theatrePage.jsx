import { useEffect, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import api from "../api/axios";
import Navbar from "../components/navbar";
import Footer from "../components/footer";

const TheatrePage = () => {
    const isAuthenticated = !!localStorage.getItem("token");
    const [scrolled, setScrolled] = useState(false);
    const [theatres, setTheatres] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const itemsPerPage = 12;
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const city = searchParams.get("city");

    // Navbar scroll effect
    useEffect(() => {
        const handleScroll = () => {
            setScrolled(window.scrollY > 50);
        };
        window.addEventListener("scroll", handleScroll);
        return () => window.removeEventListener("scroll", handleScroll);
    }, []);

    // Fetch theatres
    useEffect(() => {
        const endpoint = city ? `/theatres/city/${city}` : "/theatres";
        api.get(endpoint)
            .then((res) => {
                setTheatres(res.data);
            })
            .catch((err) => console.error("Theatre API error:", err));
    }, [city]);

    return (
        <div className="container">
            <div className="bg-gradient" />
            <div className="noise-overlay" />

            {/* Navbar */}
            <Navbar isAuthenticated={isAuthenticated} scrolled={scrolled} />

            {/* Theatres Section */}
            <section
                className="theatre-showcase"
                id="theatres"
                style={{ paddingTop: "6rem" }}
            >
                <h2 className="showcase-title">{city ? `Theatres in ${city}` : "Available Theatres"}</h2>

                <div className="showcase-grid">
                    {theatres.length === 0 && (
                        <p style={{ color: "#aaa" }}>Loading theatres...</p>
                    )}

                    {theatres.slice((currentPage - 1) * itemsPerPage, currentPage * itemsPerPage).map((theatre) => (
                        <div
                            key={theatre.theatreId}
                            className="theatre-card"
                            onClick={() =>
                                navigate(`/theatres/${theatre.theatreId}`)
                            }
                        >
                            <div className="theatre-poster">
                                <img
                                    src="https://picsum.photos/300/400"
                                    alt="theatre"
                                />
                            </div>

                            <div className="theatre-info">
                                <div className="theatre-title">
                                    {theatre.name}
                                </div>
                                <div className="theatre-meta">
                                    {theatre.city}
                                </div>
                            </div>
                        </div>
                    ))}
                </div>

                {theatres.length > itemsPerPage && (
                    <div style={{ display: "flex", justifyContent: "center", gap: "10px", marginTop: "2rem" }}>
                        <button
                            onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
                            disabled={currentPage === 1}
                            style={{ padding: "10px 20px", cursor: currentPage === 1 ? "not-allowed" : "pointer", opacity: currentPage === 1 ? 0.5 : 1 }}
                        >
                            Previous
                        </button>
                        <span style={{ padding: "10px 20px", color: "#fff" }}>
                            Page {currentPage} of {Math.ceil(theatres.length / itemsPerPage)}
                        </span>
                        <button
                            onClick={() => setCurrentPage(p => Math.min(Math.ceil(theatres.length / itemsPerPage), p + 1))}
                            disabled={currentPage === Math.ceil(theatres.length / itemsPerPage)}
                            style={{ padding: "10px 20px", cursor: currentPage === Math.ceil(theatres.length / itemsPerPage) ? "not-allowed" : "pointer", opacity: currentPage === Math.ceil(theatres.length / itemsPerPage) ? 0.5 : 1 }}
                        >
                            Next
                        </button>
                    </div>
                )}
            </section>

            <Footer />
        </div>
    );
};

export default TheatrePage;