import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Navbar from "../components/navbar";
import api from "../api/axios";
import "../styles/profile.css";

const Profile = () => {
    const isAuthenticated = !!localStorage.getItem("token");
    const navigate = useNavigate();
    const [scrolled, setScrolled] = useState(false);
    const [user, setUser] = useState(null);
    const [bookingStats, setBookingStats] = useState({ confirmed: 0, cancelled: 0 });
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        if (!isAuthenticated) {
            navigate("/login");
            return;
        }

        const handleScroll = () => setScrolled(window.scrollY > 50);
        window.addEventListener("scroll", handleScroll);
        return () => window.removeEventListener("scroll", handleScroll);
    }, [isAuthenticated, navigate]);

    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const userRes = await api.get("/users/profile");

                setUser(userRes.data);

                const bookingsRes = await api.get("/bookings/user");

                // Backend returns TicketStatus enum as strings: Reserved, Booked, Completed, Cancelled, Expired
                const confirmed = bookingsRes.data.filter(
                    (b) => b.status === "Booked" || b.status === "Completed"
                ).length;
                const cancelled = bookingsRes.data.filter(
                    (b) => b.status === "Cancelled"
                ).length;
                setBookingStats({ confirmed, cancelled });
            } catch (error) {
                console.error("Error fetching profile data:", error);
            } finally {
                setLoading(false);
            }
        };

        if (isAuthenticated) fetchUserData();
    }, [isAuthenticated]);

    const total = bookingStats.confirmed + bookingStats.cancelled;
    const confirmedPercent = total > 0 ? (bookingStats.confirmed / total) * 100 : 0;
    const cancelledPercent = total > 0 ? (bookingStats.cancelled / total) * 100 : 0;

    return (
        <div className="container">
            <div className="bg-gradient" />
            <div className="noise-overlay" />
            <Navbar isAuthenticated={isAuthenticated} scrolled={scrolled} />

            <div className="profile-container">
                {loading ? (
                    <p className="loading-text">Loading profile...</p>
                ) : (
                    <div className="profile-grid">
                        {/* Left Card - User Info */}
                        <div className="profile-card">
                            <div className="profile-avatar">
                                {user?.fName?.charAt(0).toUpperCase() || "U"}
                            </div>
                            <h2 className="profile-name">{user?.fName + " " + user?.lName|| "User"}</h2>
                            <p className="profile-username">@{user?.username || "username"}</p>
                            <div className="profile-details">
                                <div className="profile-detail-item">
                                    <span className="detail-icon">📞</span>
                                    <span className="detail-text">{user?.contactNumber || "N/A"}</span>
                                </div>
                                <div className="profile-detail-item">
                                    <span className="detail-icon">✉️</span>
                                    <span className="detail-text">{user?.email || "N/A"}</span>
                                </div>
                            </div>
                            <button className="edit-profile-btn" onClick={() => navigate("/edit-profile")}>Edit Profile</button>
                        </div>

                        {/* Right Card - Booking Analytics */}
                        <div className="analytics-card">
                            <h3 className="analytics-title">Booking Overview</h3>
                            <div className="chart-container">
                                <svg viewBox="0 0 200 200" className="pie-chart">
                                    <circle
                                        cx="100"
                                        cy="100"
                                        r="80"
                                        fill="none"
                                        stroke="#22c55e"
                                        strokeWidth="40"
                                        strokeDasharray={`${confirmedPercent * 5.03} 502`}
                                        transform="rotate(-90 100 100)"
                                    />
                                    <circle
                                        cx="100"
                                        cy="100"
                                        r="80"
                                        fill="none"
                                        stroke="#ef4444"
                                        strokeWidth="40"
                                        strokeDasharray={`${cancelledPercent * 5.03} 502`}
                                        strokeDashoffset={`-${confirmedPercent * 5.03}`}
                                        transform="rotate(-90 100 100)"
                                    />
                                    <text x="100" y="85" textAnchor="middle" className="chart-total-label">Total</text>
                                    <text x="100" y="115" textAnchor="middle" className="chart-total-value">{total}</text>
                                </svg>
                            </div>
                            <div className="chart-legend">
                                <div className="legend-item">
                                    <span className="legend-color confirmed"></span>
                                    <span className="legend-label">Confirmed</span>
                                    <span className="legend-value">{bookingStats.confirmed}</span>
                                </div>
                                <div className="legend-item">
                                    <span className="legend-color cancelled"></span>
                                    <span className="legend-label">Cancelled</span>
                                    <span className="legend-value">{bookingStats.cancelled}</span>
                                </div>
                            </div>
                            <button className="view-bookings-btn" onClick={() => navigate("/my-bookings")}>
                                View All Bookings
                            </button>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
};

export default Profile;
