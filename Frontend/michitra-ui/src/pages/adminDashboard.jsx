import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import "../styles/adminDashboard.css";

const AdminDashboard = () => {
    const navigate = useNavigate();
    const [user, setUser] = useState(null);

    useEffect(() => {
        const userData = JSON.parse(localStorage.getItem("user") || "{}");
        setUser(userData);
    }, []);

    const handleLogout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("user");
        navigate("/login");
    };

    return (
        <div className="admin-dashboard">
            <nav className="admin-nav">
                <a href="/" className="logo">MiChitra</a>
                <button onClick={handleLogout} className="logout-btn">Logout</button>
            </nav>
            
            <div className="admin-content">
                <div className="welcome-section">
                    <h2>Welcome, {user?.fName} {user?.lName}</h2>
                    <p>Admin Dashboard</p>
                </div>

                <div className="dashboard-grid">
                    <div className="dashboard-card" onClick={() => navigate("/admin/movies")}>
                        <h3>Manage Movies</h3>
                        <p>Add, edit, or remove movies</p>
                    </div>
                    
                    <div className="dashboard-card" onClick={() => navigate("/admin/theatres")}>
                        <h3>Manage Theatres</h3>
                        <p>Add, edit, or remove theatres</p>
                    </div>
                    
                    <div className="dashboard-card" onClick={() => navigate("/admin/shows")}>
                        <h3>Manage Shows</h3>
                        <p>Schedule and manage movie shows</p>
                    </div>
                    
                    <div className="dashboard-card" onClick={() => navigate("/admin/bookings")}>
                        <h3>View &amp; Manage Bookings</h3>
                        <p>Monitor and manage all ticket bookings</p>
                    </div>

                    <div className="dashboard-card" onClick={() => navigate("/admin/users")}>
                        <h3>Manage Users</h3>
                        <p>View, edit, or deactivate users</p>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AdminDashboard;
