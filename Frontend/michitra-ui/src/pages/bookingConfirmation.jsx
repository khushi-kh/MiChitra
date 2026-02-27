import { useLocation, useNavigate } from "react-router-dom";
import { useEffect } from "react";
import "../styles/bookingConfirmation.css";

const BookingConfirmation = () => {
    const location = useLocation();
    const navigate = useNavigate();
    const bookingData = location.state;

    useEffect(() => {
        if (!bookingData) {
            navigate("/movies");
        }
    }, [bookingData, navigate]);

    if (!bookingData) return null;

    return (
        <div className="confirmation-container">
            <div className="confirmation-card">
                <div className="success-icon">✓</div>
                <h1>Booking Confirmed!</h1>
                
                <div className="booking-details">
                    <div className="detail-row">
                        <span className="label">Movie:</span>
                        <span className="value">{bookingData.movieName}</span>
                    </div>
                    <div className="detail-row">
                        <span className="label">Theatre:</span>
                        <span className="value">{bookingData.theatreName}</span>
                    </div>
                    <div className="detail-row">
                        <span className="label">Show Time:</span>
                        <span className="value">{new Date(bookingData.showTime).toLocaleString()}</span>
                    </div>
                    <div className="detail-row">
                        <span className="label">Seats:</span>
                        <span className="value">{bookingData.seats.join(", ")}</span>
                    </div>
                    <div className="detail-row">
                        <span className="label">Amount Paid:</span>
                        <span className="value">₹{bookingData.amount}</span>
                    </div>
                    <div className="detail-row">
                        <span className="label">Transaction ID:</span>
                        <span className="value">{bookingData.transactionId}</span>
                    </div>
                </div>

                <button className="home-button" onClick={() => navigate("/movies")}>
                    Back to Movies
                </button>
            </div>
        </div>
    );
};

export default BookingConfirmation;
