import { useEffect, useState } from "react";
import api from "../api/axios";
import Navbar from "../components/navbar";
import TheatreGrid from "../components/theatreGrid";
import Footer from "../components/footer";

const TheatrePage = () => {
    const isAuthenticated = !!localStorage.getItem("token");
    const [scrolled, setScrolled] = useState(false);
    const [theatres, setTheatres] = useState([]);

    // Navbar scroll effect
    useEffect(() => {
        const handleScroll = () => {
            setScrolled(window.scrollY > 50);
        };
        window.addEventListener("scroll", handleScroll);
        return () => window.removeEventListener("scroll", handleScroll);
    }, []);

    // Fetch theatres from backend
    useEffect(() => {
        api
            .get("/theatres")
            .then((res) => {
                setTheatres(res.data);
            })
            .catch((err) => console.error("Theatre API error:", err));
    }, []);

    return (
        <div className="container">
            <div className="bg-gradient" />
            <div className="noise-overlay" />

            {/* Navigation */}
            <Navbar isAuthenticated={isAuthenticated} scrolled={scrolled} />

            {/* Movies Grid */}
            <TheatreGrid theatres={theatres} />

            {/* Footer */}
            <Footer />

        </div>
    );
};

export default TheatrePage;