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

    const totalSeats = show?.totalSeats ?? 40;
    const cols = Math.ceil(Math.sqrt(totalSeats));
    const rows = Math.ceil(totalSeats / cols);

    const [selectedSeats, setSelectedSeats] = useState([]);
    const [bookedSeats, setBookedSeats] = useState([]);

    useEffect(() => {
        const fetchBookedSeats = async () => {
            try {
                const response = await api.get(`/tickets/booked-seats/${show.id}`);
             
                const normalized = (response.data || []).map((s) => {
                    if (!s) return s;
                    const seat = s.trim();
                    if (/^\d+\-\d+$/.test(seat)) {
                        const [rStr, cStr] = seat.split("-");
                        const rNum = Number(rStr);
                        const cNum = Number(cStr);
                        if (!Number.isNaN(rNum) && !Number.isNaN(cNum)) {
                            return `${String.fromCharCode(97 + rNum)}${cNum + 1}`;
                        }
                    }
                    const m = seat.match(/^([A-Za-z])(\d+)$/);
                    if (m) return `${m[1].toLowerCase()}${m[2]}`;
                    return seat.toLowerCase();
                });
                setBookedSeats(normalized);
            } catch (error) {
                console.error("Failed to fetch booked seats:", error);
            }
        };
        if (show?.id) fetchBookedSeats();
    }, [show?.id]);

    const handleProceedToPay = async () => {
        setBookingError("");
        try {
            const token = localStorage.getItem("token");
            if (!token) {
                navigate('/login');
                return;
            }

            const userId = JSON.parse(atob(token.split(".")[1])).sub;
            console.log("Booking payload about to send:", {
                userId: JSON.parse(atob(localStorage.getItem("token").split(".")[1])).sub,
                movieShowId: show.id,
                numberOfSeats: selectedSeats.length,
                seatNumbers: selectedSeats
            });
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

    // inline CSS vars to allow responsive sizing in the stylesheet
    const cssVars = {
        ["--cols"]: cols,
        ["--rows"]: rows
    };

    return (
        <div className="seat-modal-overlay">
            <div className="seat-modal" style={cssVars}>

                {/* Header */}
                <div className="seat-modal-header">
                    <h2>Select Seats</h2>
                    <p>{show.movieName} &mdash; {new Date(show.showTime).toLocaleString()}</p>
                </div>

                {/* Seating area */}
                <div className="seat-modal-seating">
                    <div className="screen">SCREEN</div>

                    <div className="seats">
                        {Array.from({ length: rows }).map((_, r) => (
                            <div key={r} className="seat-row">
                                {Array.from({ length: cols }).map((_, c) => {
                                    const seatIndex = r * cols + c;
                                    if (seatIndex >= totalSeats) {
                                        // render an invisible placeholder to preserve grid geometry
                                        return (
                                            <div
                                                key={`empty-${r}-${c}`}
                                                className="seat empty"
                                                style={{ visibility: "hidden", pointerEvents: "none", border: "none", background: "transparent" }}
                                            />
                                        );
                                    }
                                    const seatId = `${String.fromCharCode(97 + r)}${c + 1}`; // a1, b2, ...
                                    const isSelected = selectedSeats.includes(seatId);
                                    const isBooked = bookedSeats.includes(seatId);
                                    return (
                                        <div
                                            key={seatId}
                                            className={`seat ${isBooked ? "booked" : isSelected ? "selected" : "available"}`}
                                            onClick={() => toggleSeat(seatId)}
                                        >
                                            {seatId}
                                        </div>
                                    );
                                })}
                            </div>
                        ))}
                    </div>

                    <div className="seat-legend">
                        <div className="legend-item"><div className="legend-dot available"></div>Available</div>
                        <div className="legend-item"><div className="legend-dot selected"></div>Selected</div>
                        <div className="legend-item"><div className="legend-dot booked"></div>Booked</div>
                    </div>
                </div>

                {/* Sidebar: summary + actions */}
                <div className="seat-modal-sidebar">
                    <span className="sidebar-title">Booking Summary</span>

                    <div className="seat-summary">
                        <div className="seat-summary-row">
                            <span>Seats</span>
                            <span>{selectedSeats.length}</span>
                        </div>
                        <div className="seat-summary-row">
                            <span>Total</span>
                            <span className="seat-summary-total">₹{selectedSeats.length * show.pricePerSeat}</span>
                        </div>
                    </div>

                    {bookingError && <p className="error">{bookingError}</p>}

                    <div className="seat-actions">
                        <button className="primary" disabled={!selectedSeats.length} onClick={handleProceedToPay}>
                            Proceed to Pay
                        </button>
                        <button className="secondary" onClick={onClose}>Cancel</button>
                    </div>
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
