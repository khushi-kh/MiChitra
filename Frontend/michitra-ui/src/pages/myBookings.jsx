import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Navbar from "../components/navbar";
import PaymentModal from "../components/paymentModal";
import api from "../api/axios";
import "../styles/myBookings.css";

const MyBookings = () => {
    const isAuthenticated = !!localStorage.getItem("token");
    const navigate = useNavigate();
    const [scrolled, setScrolled] = useState(false);
    const [bookings, setBookings] = useState([]);
    const [loading, setLoading] = useState(true);
    const [filter, setFilter] = useState("all");
    const [paymentModal, setPaymentModal] = useState(null);
    const [currentPage, setCurrentPage] = useState(1);
    const itemsPerPage = 6;

    useEffect(() => {
        if (!isAuthenticated) {
            navigate("/login");
            return;
        }

        const handleScroll = () => setScrolled(window.scrollY > 50);
        window.addEventListener("scroll", handleScroll);

        const timer = setInterval(() => {
            setBookings(prev => [...prev]);
        }, 1000);

        return () => {
            window.removeEventListener("scroll", handleScroll);
            clearInterval(timer);
        };
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

    const handlePaymentSuccess = (paymentData) => {
        setPaymentModal(null);
        setBookings(bookings.map(b => 
            b.ticketId === paymentData.ticketId ? {...b, status: "Booked", transactionId: paymentData.transactionId} : b
        ));
        alert("Payment successful!");
    };

    const getTimeRemaining = (expiry) => {
        const now = new Date();
        const expiryDate = new Date(expiry);
        const diff = expiryDate - now;
        if (diff <= 0) return "Expired";
        const minutes = Math.floor(diff / 60000);
        const seconds = Math.floor((diff % 60000) / 1000);
        return `${minutes}:${seconds.toString().padStart(2, '0')}`;
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

    const totalPages = Math.ceil(filteredBookings.length / itemsPerPage);
    const paginatedBookings = filteredBookings.slice(
        (currentPage - 1) * itemsPerPage,
        currentPage * itemsPerPage
    );

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
                        onClick={() => { setFilter("all"); setCurrentPage(1); }}
                    >
                        All ({bookings.length})
                    </button>
                    <button
                        className={`filter-tab ${filter === "confirmed" ? "active" : ""}`}
                        onClick={() => { setFilter("confirmed"); setCurrentPage(1); }}
                    >
                        Confirmed ({bookings.filter(b => b.status === "Booked" || b.status === "Completed").length})
                    </button>
                    <button
                        className={`filter-tab ${filter === "cancelled" ? "active" : ""}`}
                        onClick={() => { setFilter("cancelled"); setCurrentPage(1); }}
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
                    <>
                    <div className="bookings-grid">
                        {paginatedBookings.map((booking) => (
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
                                        <span className="detail-label">City</span>
                                        <span className="detail-value">{booking.city ?? "N/A"}</span>
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
                                    {booking.status === "Reserved" && booking.reservationExpiry && (
                                        <div className="booking-detail">
                                            <span className="detail-label">Expires in</span>
                                            <span className="detail-value expires-timer">
                                                {getTimeRemaining(booking.reservationExpiry)}
                                            </span>
                                        </div>
                                    )}
                                </div>
                                {booking.status === "Reserved" && (
                                    <button className="pay-now-btn" onClick={() => setPaymentModal(booking)}>
                                        Pay Now
                                    </button>
                                )}
                                {booking.status === "Booked" && new Date(booking.showTime) > new Date() && (
                                    <button className="cancel-btn" onClick={() => handleCancelTicket(booking.ticketId)}>
                                        Cancel Ticket
                                    </button>
                                )}
                            </div>
                        ))}
                    </div>
                    {totalPages > 1 && (
                        <div className="pagination">
                            <button
                                className="pagination-btn"
                                onClick={() => setCurrentPage(prev => Math.max(1, prev - 1))}
                                disabled={currentPage === 1}
                            >
                                Previous
                            </button>
                            <span className="pagination-info">
                                Page {currentPage} of {totalPages}
                            </span>
                            <button
                                className="pagination-btn"
                                onClick={() => setCurrentPage(prev => Math.min(totalPages, prev + 1))}
                                disabled={currentPage === totalPages}
                            >
                                Next
                            </button>
                        </div>
                    )}
                    </>
                )}
            </div>
            {paymentModal && (
                <PaymentModal
                    ticketId={paymentModal.ticketId}
                    amount={paymentModal.totalPrice}
                    onClose={() => setPaymentModal(null)}
                    onSuccess={handlePaymentSuccess}
                />
            )}
        </div>
    );
};

export default MyBookings;
