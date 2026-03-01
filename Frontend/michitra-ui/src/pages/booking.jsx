import { useEffect, useState } from "react";
import { useParams, useSearchParams } from "react-router-dom";
import api from "../api/axios";
import Navbar from "../components/navbar";
import SeatSelection from "../components/SeatSelection";
import "../styles/booking.css";

const Booking = () => {
    const isAuthenticated = !!localStorage.getItem("token");
    const [scrolled, setScrolled] = useState(false);
    const { movieId } = useParams();
    const [searchParams] = useSearchParams();
    const theatreIdFromUrl = searchParams.get("theatreId");

    const [theatres, setTheatres] = useState([]);
    const [selectedTheatre, setSelectedTheatre] = useState(null);
    const [shows, setShows] = useState([]);
    const [loading, setLoading] = useState(true);

    const [showSeatModal, setShowSeatModal] = useState(false);
    const [selectedShow, setSelectedShow] = useState(null);

    // Navbar scroll effect
    useEffect(() => {
        const handleScroll = () => setScrolled(window.scrollY > 50);
        window.addEventListener("scroll", handleScroll);
        return () => window.removeEventListener("scroll", handleScroll);
    }, []);

    // Fetch theatres
    useEffect(() => {
        setLoading(true);
        api.get(`/movieshows/movie/${movieId}/theatres`)
            .then((res) => {
                setTheatres(res.data);
                if (theatreIdFromUrl) {
                    const theatre = res.data.find(t => t.theatreId === parseInt(theatreIdFromUrl));
                    if (theatre) handleTheatreClick(theatre);
                }
                setLoading(false);
            })
            .catch(() => setLoading(false));
    }, [movieId, theatreIdFromUrl]);

    // Check if show is expired
    const isShowExpired = (showTime) => {
        const now = new Date();
        const showDate = new Date(showTime);
        return showDate < now;
    };

    // Handle theatre click
    const handleTheatreClick = (theatre) => {
        setSelectedTheatre(theatre);
        setLoading(true);

        api.get(`/movieshows/movie/${movieId}/theatre/${theatre.theatreId}`)
            .then((res) => {
                // Sort shows (upcoming first)
                const sortedShows = res.data.sort(
                    (a, b) => new Date(a.showTime) - new Date(b.showTime)
                );
                setShows(sortedShows);
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

                {/* Theatre Selection */}
                {!selectedTheatre ? (
                    <>
                        {loading ? (
                            <p className="loading-text">Loading theatres...</p>
                        ) : (
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
                        <button
                            className="back-button"
                            onClick={() => {
                                setSelectedTheatre(null);
                                setShows([]);
                            }}
                        >
                            ← Back to Theatres
                        </button>

                        <div className="theatre-header">
                            <h2 className="theatre-name">
                                {selectedTheatre.theatreName}
                            </h2>
                        </div>

                        {/* Shows */}
                        {loading ? (
                            <p className="loading-text">Loading shows...</p>
                        ) : (
                            <div className="shows-grid">
                                {shows.map((show) => {
                                    const expired = isShowExpired(show.showTime);
                                    const soldOut = show.availableSeats === 0;

                                    return (
                                        <div
                                            key={show.id}
                                            className={`show-card ${expired ? "expired-show" : ""
                                                }`}
                                        >
                                            <div className="show-info">
                                                <p className="show-time">
                                                    {new Date(
                                                        show.showTime
                                                    ).toLocaleString()}
                                                </p>

                                                <p
                                                    className={`show-seats ${show.availableSeats < 10 &&
                                                            show.availableSeats > 0
                                                            ? "low-seats"
                                                            : ""
                                                        }`}
                                                >
                                                    {expired
                                                        ? "Show Ended"
                                                        : soldOut
                                                            ? "Sold Out"
                                                            : `${show.availableSeats} seats available`}
                                                </p>
                                            </div>

                                            <button
                                                className="book-show-button"
                                                disabled={soldOut || expired}
                                                onClick={() => {
                                                    if (!expired && !soldOut) {
                                                        setSelectedShow(show);
                                                        setShowSeatModal(true);
                                                    }
                                                }}
                                            >
                                                {expired
                                                    ? "Show Ended"
                                                    : soldOut
                                                        ? "Sold Out"
                                                        : "Book Now"}
                                            </button>
                                        </div>
                                    );
                                })}
                            </div>
                        )}
                    </>
                )}
            </div>

            {/* Seat Modal */}
            {showSeatModal && selectedShow && (
                <SeatSelection
                    show={{
                        ...selectedShow,
                        movieName: selectedShow.movieName,
                        theatreName: selectedTheatre.theatreName,
                    }}
                    onClose={() => setShowSeatModal(false)}
                />
            )}
        </div>
    );
};

export default Booking;