import { Link, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";
import api from "../api/axios"; // adjust path if needed

const Navbar = ({ isAuthenticated, scrolled }) => {
    const user = JSON.parse(localStorage.getItem("user") || "null");
    const [query, setQuery] = useState("");
    const [showAdminDropdown, setShowAdminDropdown] = useState(false);
    const [cities, setCities] = useState([]);
    const [showCitiesDropdown, setShowCitiesDropdown] = useState(false);
    const [menuOpen, setMenuOpen] = useState(false);
    const navigate = useNavigate();
    const isAdmin = user?.role === "Admin";


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
                        <div className="menu-dropdown">
                            <div className="menu-dropdown-header">Popular Cities</div>
                            <div className="menu-dropdown-columns">
                                <div className="menu-dropdown-column">
                                    {cities.slice(0, 5).map((city, index) => (
                                        <Link
                                            key={index}
                                            to={`/theatres?city=${city}`}
                                            className="menu-dropdown-item"
                                        >
                                            {city}
                                        </Link>
                                    ))}
                                    <Link to="/cities" className="menu-dropdown-item" style={{fontWeight: 600, color: '#e63946'}}>
                                        View All →
                                    </Link>
                                </div>
                            </div>
                        </div>
                    )}
                </li>

                {!isAuthenticated ? (
                    <>
                        <li className="nav-auth-group">
                            <Link to="/login" className="nav-login">Sign In</Link>
                            <Link to="/register" className="nav-register">Sign Up</Link>
                        </li>
                    </>
                ) : (
                    <>
                            {isAdmin? (
                                <li className="nav-dropdown" onMouseEnter={() => setShowAdminDropdown(true)}
                                    onMouseLeave={() => setShowAdminDropdown(false)}>

                                    <span className="nav-profile" style={{ color: "#e63946", cursor: "pointer" }}>
                                        {user?.fName || "Admin"}▼
                                    </span>
                                    {showAdminDropdown && (
                                        <div className="menu-dropdown">
                                            <Link to="/profile" className="menu-dropdown-item">Profile</Link>
                                            <Link to="/admin" className="menu-dropdown-item">Admin Dashboard</Link>
                                        </div>
                                    )}
                                </li> 
                            ) : (
                                    <li>
                                        <Link to="/profile" className="nav-profile" style={{ color: "#e63946" }}>
                                            {user?.fName || "Profile"}
                                        </Link>
                                </li>
                            )}
                        <li>
                            <button
                                className="nav-logout"
                                onClick={() => {
                                    localStorage.removeItem("token");
                                    localStorage.removeItem("user");
                                    window.location.href = "/";
                                }}
                            >
                                Logout
                            </button>
                        </li>
                    </>
                )}
            </ul>

            <button className="nav-hamburger" onClick={() => setMenuOpen(!menuOpen)} aria-label="Toggle menu">
                <span></span>
                <span></span>
                <span></span>
            </button>

            {menuOpen && (
                <div className="nav-mobile-menu">
                    <form className="nav-mobile-search" onSubmit={(e) => { handleSearch(e); setMenuOpen(false); }}>
                        <input
                            type="text"
                            placeholder="Search movies..."
                            value={query}
                            onChange={(e) => setQuery(e.target.value)}
                        />
                    </form>
                    <Link to="/movies" className="nav-mobile-link" onClick={() => setMenuOpen(false)}>Movies</Link>
                    <Link to="/theatres" className="nav-mobile-link" onClick={() => setMenuOpen(false)}>Theatres</Link>
                    <Link to="/cities" className="nav-mobile-link" onClick={() => setMenuOpen(false)}>Cities</Link>
                    {!isAuthenticated ? (
                        <>
                            <Link to="/login" className="nav-mobile-link" onClick={() => setMenuOpen(false)}>Sign In</Link>
                            <Link to="/register" className="nav-mobile-link nav-mobile-register" onClick={() => setMenuOpen(false)}>Sign Up</Link>
                        </>
                    ) : (
                        <>
                            <Link to="/profile" className="nav-mobile-link" onClick={() => setMenuOpen(false)}>{user?.fName || "Profile"}</Link>
                            <button
                                className="nav-mobile-link nav-mobile-logout"
                                onClick={() => {
                                    localStorage.removeItem("token");
                                    localStorage.removeItem("user");
                                    window.location.href = "/";
                                }}
                            >
                                Logout
                            </button>
                        </>
                    )}
                </div>
            )}
        </nav>
    );
};

export default Navbar;
