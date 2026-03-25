import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api/axios";
import "../styles/manageBookings.css";

const ManageBookings = () => {
    const navigate = useNavigate();
    const [bookings, setBookings] = useState([]);

    useEffect(() => {
        fetchBookings();
    }, []);

    const fetchBookings = () => {
        api.get("/tickets")
            .then((res) => setBookings(res.data))
            .catch((err) => console.error("Error fetching bookings:", err));
    };

    const handleCancel = (ticketId) => {
        if (!window.confirm("Are you sure you want to cancel this ticket?")) return;

        api.put(`/tickets/cancel/${ticketId}`)
            .then(() => {
                setBookings((prev) =>
                    prev.map((b) =>
                        b.ticketId === ticketId ? { ...b, status: "Cancelled" } : b
                    )
                );
            })
            .catch((err) => {
                console.error("Error cancelling ticket:", err);
                alert(err.response?.data || "Error cancelling ticket");
            });
    };

    return (
        <div className="manage-bookings">
            <div className="manage-nav">
                <h1>View &amp; Manage Bookings</h1>
                <div className="nav-actions">
                    <button onClick={() => navigate("/admin")} className="btn-back">
                        Back
                    </button>
                </div>
            </div>

            <div className="manage-content">
                <div className="bookings-table">
                    {bookings.map((b) => (
                        <div key={b.ticketId} className="booking-row">
                            <div className="booking-info">
                                <div>
                                    <h3>{b.movieName}</h3>
                                    <p>
                                        {b.theatreName} • {b.city ?? "N/A"} •{" "}
                                        {new Date(b.showTime).toLocaleString()}
                                    </p>
                                    <p>
                                        Seats: {b.numberOfSeats} • Total: ₹{b.totalPrice}
                                    </p>
                                    <p>
                                        Ticket ID: #{b.ticketId} • User ID: {b.userId}
                                    </p>
                                    <p>
                                        Status: {b.status} • Booked on{" "}
                                        {new Date(b.bookingDate).toLocaleDateString()}
                                    </p>
                                </div>
                            </div>
                            <div className="booking-actions">
                                {b.status === "Booked" && (
                                    <button
                                        onClick={() => handleCancel(b.ticketId)}
                                        className="btn-delete"
                                    >
                                        Cancel Ticket
                                    </button>
                                )}
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default ManageBookings;

