import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api/axios";
import Navbar from "../components/navbar";
import Footer from "../components/footer";

const CitiesPage = () => {
    const isAuthenticated = !!localStorage.getItem("token");
    const [scrolled, setScrolled] = useState(false);
    const [cities, setCities] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const itemsPerPage = 12;
    const navigate = useNavigate();

    useEffect(() => {
        const handleScroll = () => {
            setScrolled(window.scrollY > 50);
        };
        window.addEventListener("scroll", handleScroll);
        return () => window.removeEventListener("scroll", handleScroll);
    }, []);

    useEffect(() => {
        api.get("/theatres/cities")
            .then((res) => setCities(res.data))
            .catch((err) => console.error("Cities API error:", err));
    }, []);

    return (
        <div className="container">
            <div className="bg-gradient" />
            <div className="noise-overlay" />

            <Navbar isAuthenticated={isAuthenticated} scrolled={scrolled} />

            <section className="theatre-showcase" id="cities" style={{ paddingTop: "6rem" }}>
                <h2 className="showcase-title">Select Your City</h2>

                <div className="showcase-grid">
                    {cities.length === 0 && <p style={{ color: "#aaa" }}>Loading cities...</p>}

                    {cities.slice((currentPage - 1) * itemsPerPage, currentPage * itemsPerPage).map((city) => (
                        <div key={city} className="theatre-card" onClick={() => navigate(`/theatres?city=${city}`)}>
                            <div className="theatre-poster" style={{ height: "280px" }}>
                                <img src="https://picsum.photos/300/400" alt={city} />
                            </div>
                            <div className="theatre-info">
                                <div className="theatre-title">{city}</div>
                            </div>
                        </div>
                    ))}
                </div>

                {cities.length > itemsPerPage && (
                    <div style={{ display: "flex", justifyContent: "center", gap: "10px", marginTop: "2rem" }}>
                        <button
                            onClick={() => setCurrentPage(p => Math.max(1, p - 1))}
                            disabled={currentPage === 1}
                            style={{ padding: "10px 20px", cursor: currentPage === 1 ? "not-allowed" : "pointer", opacity: currentPage === 1 ? 0.5 : 1 }}
                        >
                            Previous
                        </button>
                        <span style={{ padding: "10px 20px", color: "#fff" }}>
                            Page {currentPage} of {Math.ceil(cities.length / itemsPerPage)}
                        </span>
                        <button
                            onClick={() => setCurrentPage(p => Math.min(Math.ceil(cities.length / itemsPerPage), p + 1))}
                            disabled={currentPage === Math.ceil(cities.length / itemsPerPage)}
                            style={{ padding: "10px 20px", cursor: currentPage === Math.ceil(cities.length / itemsPerPage) ? "not-allowed" : "pointer", opacity: currentPage === Math.ceil(cities.length / itemsPerPage) ? 0.5 : 1 }}
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

export default CitiesPage;
