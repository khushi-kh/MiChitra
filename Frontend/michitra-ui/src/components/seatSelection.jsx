import { useState } from "react";
import "../styles/seatSelection.css";

const SeatSelection = ({ show, onClose }) => {
    const rows = 5;
    const cols = 8;

    const [selectedSeats, setSelectedSeats] = useState([]);

    const toggleSeat = (seat) => {
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

                                return (
                                    <div
                                        key={seatId}
                                        className={`seat ${isSelected ? "selected" : "available"}`}
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

                <div className="seat-actions">
                    <button className="secondary" onClick={onClose}>Cancel</button>
                    <button className="primary" disabled={!selectedSeats.length}>
                        Proceed to Pay
                    </button>
                </div>
            </div>
        </div>
    );
};

export default SeatSelection;
