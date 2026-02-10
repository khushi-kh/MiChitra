import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import api from "../api/axios";
import Navbar from "../components/navbar";
import SeatSelection from "../components/SeatSelection";
import "../styles/booking.css";

const Booking = () => {
    const isAuthenticated = !!localStorage.getItem("token");
    const [scrolled, setScrolled] = useState(false);
    const { movieId } = useParams();
    const [theatres, setTheatres] = useState([]);
    const [selectedTheatre, setSelectedTheatre] = useState(null);
    const [shows, setShows] = useState([]);
    const [loading, setLoading] = useState(true);

    const [showSeatModal, setShowSeatModal] = useState(false);
    const [selectedShow, setSelectedShow] = useState(null);

    useEffect(() => {
        const handleScroll = () => setScrolled(window.scrollY > 50);
        window.addEventListener("scroll", handleScroll);
        return () => window.removeEventListener("scroll", handleScroll);
    }, []);

    useEffect(() => {
        api.get(`/movieshows/movie/${movieId}/theatres`)
            .then((res) => {
                setTheatres(res.data);
                setLoading(false);
            })
            .catch(() => setLoading(false));
    }, [movieId]);

    const handleTheatreClick = (theatre) => {
        setSelectedTheatre(theatre);
        setLoading(true);
        api.get(`/movieshows/movie/${movieId}/theatre/${theatre.theatreId}`)
            .then((res) => {
                setShows(res.data);
                setLoading(false);
            })
            .catch(() => setLoading(false));
    };

    return (
        <div className="container">
            <div className="bg-gradient" />
            <div className="noise-overlay" />
            <Navbar isAuthenticated={isAuthenticated} scrolled={scrolled} />

            <div className="booking-container">
                <div className="booking-header">
                    <h1 className="booking-title">Book Your Tickets</h1>
                    <p className="booking-subtitle">Select a theatre and showtime</p>
                </div>

                {!selectedTheatre ? (
                    <>
                        {loading ? <p className="loading-text">Loading theatres...</p> : (
                            <div className="theatres-grid">
                                {theatres.map((theatre) => (
                                    <div
                                        key={theatre.theatreId}
                                        className="theatre-card"
                                        onClick={() => handleTheatreClick(theatre)}
                                    >
                                        <h3>{theatre.theatreName}</h3>
                                        <p>{theatre.city}</p>
                                    </div>
                                ))}
                            </div>
                        )}
                    </>
                ) : (
                    <>
                        <button className="back-button" onClick={() => { setSelectedTheatre(null); setShows([]); }}>← Back to Theatres</button>
                        <div className="theatre-header">
                            <h2 className="theatre-name">{selectedTheatre.theatreName}</h2>
                        </div>

                        {loading ? <p className="loading-text">Loading shows...</p> : (
                            <div className="shows-grid">
                                {shows.map((show) => (
                                    <div key={show.id} className="show-card">
                                        <div className="show-info">
                                            <p className="show-time">{new Date(show.showTime).toLocaleString()}</p>
                                            <p className={`show-seats ${show.availableSeats < 10 ? 'low-seats' : ''}`}>
                                                {show.availableSeats > 0 ? `${show.availableSeats} seats available` : 'Sold Out'}
                                            </p>
                                        </div>
                                        <button
                                            className="book-show-button"
                                            disabled={show.availableSeats === 0}
                                            onClick={() => {
                                                setSelectedShow(show);
                                                setShowSeatModal(true);
                                            }}
                                        >
                                            {show.availableSeats === 0 ? "Sold Out" : "Book Now"}
                                        </button>
                                    </div>
                                ))}
                            </div>
                        )}
                    </>
                )}
            </div>

            {showSeatModal && selectedShow && (
                <SeatSelection
                    show={selectedShow}
                    onClose={() => setShowSeatModal(false)}
                />
            )}
        </div>
    );
};

export default Booking;
