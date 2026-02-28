import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Navbar from "../components/navbar";
import api from "../api/axios";
import "../styles/myBookings.css";

const MyBookings = () => {
    const isAuthenticated = !!localStorage.getItem("token");
    const navigate = useNavigate();
    const [scrolled, setScrolled] = useState(false);
    const [bookings, setBookings] = useState([]);
    const [loading, setLoading] = useState(true);
    const [filter, setFilter] = useState("all");

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
        const fetchBookings = async () => {
            try {
                const res = await api.get("/bookings/user");
                setBookings(res.data);
            } catch (error) {
                console.error("Error fetching bookings:", error);
            } finally {
                setLoading(false);
            }
        };

        if (isAuthenticated) fetchBookings();
    }, [isAuthenticated]);

    const handleCancelTicket = async (ticketId) => {
        if (!window.confirm("Are you sure you want to cancel this booking?")) return;
        
        try {
            await api.put(`/tickets/cancel/${ticketId}`);
            alert("Ticket cancelled successfully");
            setBookings(bookings.map(b => b.ticketId === ticketId ? {...b, status: "Cancelled"} : b));
        } catch (error) {
            alert(error.response?.data || "Failed to cancel ticket");
        }
    };

    const getStatusClass = (status) => {
        switch (status) {
            case "Booked":
            case "Completed":
                return "status-confirmed";
            case "Cancelled":
                return "status-cancelled";
            case "Reserved":
                return "status-reserved";
            default:
                return "status-expired";
        }
    };

    const filteredBookings = bookings.filter((booking) => {
        if (filter === "all") return true;
        if (filter === "confirmed") return booking.status === "Booked" || booking.status === "Completed";
        if (filter === "cancelled") return booking.status === "Cancelled";
        return true;
    });

    return (
        <div className="container">
            <div className="bg-gradient" />
            <div className="noise-overlay" />
            <Navbar isAuthenticated={isAuthenticated} scrolled={scrolled} />

            <div className="bookings-container">
                <div className="bookings-header">
                    <h1 className="bookings-title">My Bookings</h1>
                    <p className="bookings-subtitle">View and manage your ticket bookings</p>
                </div>

                <div className="filter-tabs">
                    <button
                        className={`filter-tab ${filter === "all" ? "active" : ""}`}
                        onClick={() => setFilter("all")}
                    >
                        All ({bookings.length})
                    </button>
                    <button
                        className={`filter-tab ${filter === "confirmed" ? "active" : ""}`}
                        onClick={() => setFilter("confirmed")}
                    >
                        Confirmed ({bookings.filter(b => b.status === "Booked" || b.status === "Completed").length})
                    </button>
                    <button
                        className={`filter-tab ${filter === "cancelled" ? "active" : ""}`}
                        onClick={() => setFilter("cancelled")}
                    >
                        Cancelled ({bookings.filter(b => b.status === "Cancelled").length})
                    </button>
                </div>

                {loading ? (
                    <p className="loading-text">Loading bookings...</p>
                ) : filteredBookings.length === 0 ? (
                    <div className="empty-state">
                        <p className="empty-text">No bookings found</p>
                        <button className="browse-btn" onClick={() => navigate("/")}>
                            Browse Movies
                        </button>
                    </div>
                ) : (
                    <div className="bookings-grid">
                        {filteredBookings.map((booking) => (
                            <div key={booking.ticketId} className="booking-card">
                                <div className="booking-header-section">
                                    <h3 className="booking-movie">{booking.movieName}</h3>
                                    <span className={`booking-status ${getStatusClass(booking.status)}`}>
                                        {booking.status}
                                    </span>
                                </div>
                                <div className="booking-details">
                                    <div className="booking-detail">
                                        <span className="detail-label">Theatre</span>
                                        <span className="detail-value">{booking.theatreName}</span>
                                    </div>
                                    <div className="booking-detail">
                                        <span className="detail-label">Show Time</span>
                                        <span className="detail-value">
                                            {new Date(booking.showTime).toLocaleString()}
                                        </span>
                                    </div>
                                    <div className="booking-detail">
                                        <span className="detail-label">Seats</span>
                                        <span className="detail-value">{booking.numberOfSeats}</span>
                                    </div>
                                    <div className="booking-detail">
                                        <span className="detail-label">Total Price</span>
                                        <span className="detail-value">₹{booking.totalPrice}</span>
                                    </div>
                                    <div className="booking-detail">
                                        <span className="detail-label">Transaction ID</span>
                                        <span className="detail-value">{booking.transactionId || "N/A"}</span>
                                    </div>
                                    <div className="booking-detail">
                                        <span className="detail-label">Booking Date</span>
                                        <span className="detail-value">
                                            {new Date(booking.bookingDate).toLocaleDateString()}
                                        </span>
                                    </div>
                                    <div className="booking-detail">
                                        <span className="detail-label">Ticket ID</span>
                                        <span className="detail-value">#{booking.ticketId}</span>
                                    </div>
                                </div>
                                {booking.status === "Booked" && new Date(booking.showTime) > new Date() && (
                                    <button className="cancel-btn" onClick={() => handleCancelTicket(booking.ticketId)}>
                                        Cancel Ticket
                                    </button>
                                )}
                            </div>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
};

export default MyBookings;
