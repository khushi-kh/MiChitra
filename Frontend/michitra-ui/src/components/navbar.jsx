import { Link, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import api from "../api/axios"; // adjust path if needed

const Navbar = ({ isAuthenticated, scrolled }) => {
    const user = JSON.parse(localStorage.getItem("user") || "null");
    const [query, setQuery] = useState("");
    const [cities, setCities] = useState([]);
    const [showCitiesDropdown, setShowCitiesDropdown] = useState(false);
    const navigate = useNavigate();


    useEffect(() => {
        const fetchCities = async () => {
            try {
                const response = await api.get('/theatres/cities');
                setCities(response.data);
            } catch (error) {
                console.error('Failed to fetch cities:', error);
            }
        };
        fetchCities();
    }, []);

    const handleSearch = async (e) => {
        e.preventDefault();
        console.log("Search submitted:", query);


        if (!query.trim()) return;

        try {
            const result = await api.get(`/movies/search?query=${query}`);
            const movies = result.data;

            if (movies && movies.length > 0) {
                navigate(`/movies/${movies[0].movieId}`);
                setQuery("");
            }
        } catch (err) {
            console.error("Movie not found", err);
        }
    };

    return (
        <nav
            className="nav"
            style={{
                background: scrolled
                    ? "rgba(10, 10, 15, 0.95)"
                    : "rgba(10, 10, 15, 0.8)",
            }}
        >
            {/* common navbar elements */ }
            <div>
                <a href="/" className="logo">MiChitra</a>
            </div>

            <form className="nav-search" onSubmit={handleSearch}>
                <input
                    type="text"
                    placeholder="Search movies..."
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                />
            </form>

            <ul className="nav-links">
                <li><Link to="/movies" className="nav-link">Movies</Link></li>
                <li><Link to="/theatres" className="nav-link">Theatres</Link></li>
                <li className="nav-dropdown" onMouseEnter={() => setShowCitiesDropdown(true)} onMouseLeave={() => setShowCitiesDropdown(false)}>
                    <span className="nav-link">
                        Cities ▼
                    </span>
                    {showCitiesDropdown && (
                        <div className="mega-dropdown">
                            <div className="mega-dropdown-header">Popular Cities</div>
                            <div className="mega-dropdown-columns">
                                <div className="mega-dropdown-column">
                                    {cities.slice(0, 5).map((city, index) => (
                                        <Link 
                                            key={index} 
                                            to={`/theatres?city=${city}`} 
                                            className="mega-dropdown-item"
                                        >
                                            {city}
                                        </Link>
                                    ))}
                                    <Link to="/cities" className="mega-dropdown-item" style={{fontWeight: 600, color: '#e63946'}}>
                                        View All →
                                    </Link>
                                </div>
                            </div>
                        </div>
                    )}
                </li>

                {/* Conditional rendering based on authentication status */}
                {!isAuthenticated ? (
                    <>
                        <li><Link to="/login" className="nav-login">Sign In</Link></li>
                        <li><Link to="/register" className="nav-register">Sign Up</Link></li>
                    </>
                ) : (
                    <>
                        <li>
                            <Link to="/profile" className="nav-profile">
                                {user?.fName || "Profile"}
                            </Link>
                        </li>
                        <li>
                            <button
                                className="nav-logout"
                                onClick={() => {
                                    localStorage.removeItem("token");
                                    window.location.href = "/";
                                }}
                            >
                                Logout
                            </button>
                        </li>
                    </>
                )}
            </ul>
        </nav>
    );
};

export default Navbar;
