import { Link } from "react-router-dom";

const Navbar = ({ isAuthenticated, scrolled }) => {
    const user = JSON.parse(localStorage.getItem("user") || null);
    console.log("Navbar user:", user);
    console.log("Is Authenticated:", isAuthenticated);

    return (
        <nav className="nav"
            style={{
                background: scrolled
                    ? "rgba(10, 10, 15, 0.95)"
                    : "rgba(10, 10, 15, 0.8)",
            }}
        >
            {/* common navigation items */}
            <div>
                <a href="/" className="logo">MiChitra</a>
            </div>
            <ul className="nav-links">
                <li><Link to="/movies" className="nav-link">Movies</Link></li>
                <li><Link to="/theatre" className="nav-link">Theatre</Link></li>
                <li><Link to="/cities" className="nav-link">Cities</Link></li>
            

            {/* if user is not authenticated */}
            {!isAuthenticated ? (
                <>
                    <li><Link to="/login" className="nav-login">Sign In</Link></li>
                    <li><Link to="/register" className="nav-register">Sign Up</Link></li>
                </>
            ) : (
                // if user is authenticated
                <>
                            <li><Link to="/home" className="nav-profile">{user?.fName || "Profile"}</Link></li>
                            <li>
                                <button className="nav-logout" onClick={() => {
                                    localStorage.removeItem("token");
                                    window.location.href = "/";
                                }}>
                                    Logout
                                </button>
                                </li>
                </>
            )
                }
            </ul>
        </nav>
    );
};

export default Navbar;