import { Link, useNavigate } from "react-router-dom";
import { useState } from "react";
import api from "../api/axios"; // adjust path if needed

const Navbar = ({ isAuthenticated, scrolled }) => {
    const user = JSON.parse(localStorage.getItem("user") || "null");
    const [query, setQuery] = useState("");
    const navigate = useNavigate();

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
                <li><Link to="/theatre" className="nav-link">Theatre</Link></li>
                <li><Link to="/cities" className="nav-link">Cities</Link></li>

                {/* Conditional rendering based on authentication status */}
                {!isAuthenticated ? (
                    <>
                        <li><Link to="/login" className="nav-login">Sign In</Link></li>
                        <li><Link to="/register" className="nav-register">Sign Up</Link></li>
                    </>
                ) : (
                    <>
                        <li>
                            <Link to="/home" className="nav-profile">
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
