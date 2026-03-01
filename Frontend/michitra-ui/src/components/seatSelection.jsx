import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import "../styles/seatSelection.css";
import PaymentModal from "./paymentModal";
import api from "../api/axios";

const SeatSelection = ({ show, onClose }) => {
    const navigate = useNavigate();
    const [showPayment, setShowPayment] = useState(false);
    const [ticketId, setTicketId] = useState(null);
    const [bookingError, setBookingError] = useState("");
    const rows = 5;
    const cols = 8;

    const [selectedSeats, setSelectedSeats] = useState([]);
    const [bookedSeats, setBookedSeats] = useState([]);

    useEffect(() => {
        const fetchBookedSeats = async () => {
            try {
                const response = await api.get(`/tickets/booked-seats/${show.id}`);
                setBookedSeats(response.data);
            } catch (error) {
                console.error("Failed to fetch booked seats:", error);
            }
        };
        fetchBookedSeats();
    }, [show.id]);

    const handleProceedToPay = async () => {
        setBookingError("");
        try {
            const token = localStorage.getItem("token");
            if (!token) {
                navigate('/login');
            }
            
            const userId = JSON.parse(atob(token.split(".")[1])).sub;
            const response = await api.post("/Tickets/book", {
                userId: parseInt(userId),
                movieShowId: show.id,
                numberOfSeats: selectedSeats.length,
                seatNumbers: selectedSeats
            });
            console.log("Booking response:", response.data);
            const id = response.data.ticketId || response.data.TicketId;
            console.log("Extracted ticketId:", id);
            setTicketId(id);
            setShowPayment(true);
        } catch (error) {
            console.error("Booking error:", error);
            const errorMsg = error.response?.data?.message || error.response?.data || "Failed to book ticket";
            setBookingError(errorMsg);
        }
    };

    const toggleSeat = (seat) => {
        if (bookedSeats.includes(seat)) return;
        setSelectedSeats((prev) =>
            prev.includes(seat)
                ? prev.filter((s) => s !== seat)
                : [...prev, seat]
        );
    };

    return (
        <div className="seat-modal-overlay">
            <div className="seat-modal">
                <h2>Select Seats</h2>
                <p>{new Date(show.showTime).toLocaleString()}</p>

                <div className="screen">SCREEN THIS WAY</div>

                <div className="seats">
                    {Array.from({ length: rows }).map((_, r) => (
                        <div key={r} className="seat-row">
                            {Array.from({ length: cols }).map((_, c) => {
                                const seatId = `${r}-${c}`;
                                const isSelected = selectedSeats.includes(seatId);
                                const isBooked = bookedSeats.includes(seatId);

                                return (
                                    <div
                                        key={seatId}
                                        className={`seat ${isBooked ? "booked" : isSelected ? "selected" : "available"}`}
                                        onClick={() => toggleSeat(seatId)}
                                    >
                                        {r + 1}-{c + 1}
                                    </div>
                                );
                            })}
                        </div>
                    ))}
                </div>

                <div className="seat-summary">
                    Selected Seats: {selectedSeats.length}
                    Total: ₹{selectedSeats.length * show.pricePerSeat}
                </div>

                {bookingError && <p className="error">{bookingError}</p>}

                <div className="seat-actions">
                    <button className="secondary" onClick={onClose}>Cancel</button>
                    <button
                        className="primary"
                        disabled={!selectedSeats.length}
                        onClick={handleProceedToPay}
                    >
                        Proceed to Pay
                    </button>
                </div>
            </div>

            {showPayment && ticketId && (
                <PaymentModal
                    ticketId={ticketId}
                    amount={selectedSeats.length * show.pricePerSeat}
                    onClose={() => setShowPayment(false)}
                    onSuccess={(paymentResult) => {
                        navigate("/booking-confirmation", {
                            state: {
                                ticketId: ticketId,
                                movieName: show.movieName,
                                theatreName: show.theatreName,
                                showTime: show.showTime,
                                seats: selectedSeats,
                                amount: selectedSeats.length * show.pricePerSeat,
                                transactionId: paymentResult.transactionId
                            }
                        });
                    }}
                />
            )}
        </div>
    );
};

export default SeatSelection;
