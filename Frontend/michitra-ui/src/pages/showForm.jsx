import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import api from "../api/axios";
import "../styles/movieForm.css";

const ShowForm = () => {
    const navigate = useNavigate();
    const { id } = useParams();
    const isEdit = !!id;

    const [movies, setMovies] = useState([]);
    const [theatres, setTheatres] = useState([]);

    const [formData, setFormData] = useState({
        movieId: "",
        theatreId: "",
        showTime: "",
        totalSeats: "",
        pricePerSeat: ""
    });

    useEffect(() => {
        api.get("/movies").then((res) => setMovies(res.data));
        api.get("/theatres").then((res) =>
            setTheatres(res.data.filter((t) => t.isActive !== false))
        );
    }, []);

    useEffect(() => {
        if (!isEdit) return;

        api.get(`/movieshows/${id}`)
            .then((res) => {
                const show = res.data;
                setFormData({
                    movieId: show.movieId?.toString() ?? "",
                    theatreId: show.theatreId?.toString() ?? "",
                    showTime: show.showTime?.split("Z")[0] ?? "",
                    totalSeats: show.totalSeats?.toString() ?? "",
                    pricePerSeat: show.pricePerSeat?.toString() ?? ""
                });
            })
            .catch((err) => console.error("Error fetching show:", err));
    }, [id, isEdit]);

    const handleSubmit = (e) => {
        e.preventDefault();

        const payload = {
            movieId: parseInt(formData.movieId),
            theatreId: parseInt(formData.theatreId),
            showTime: new Date(formData.showTime),
            totalSeats: parseInt(formData.totalSeats),
            pricePerSeat: parseFloat(formData.pricePerSeat)
        };

        const request = isEdit
            ? api.put(`/movieshows/${id}`, {
                  showTime: payload.showTime,
                  totalSeats: payload.totalSeats,
                  availableSeats: payload.totalSeats,
                  pricePerSeat: payload.pricePerSeat
              })
            : api.post("/movieshows", payload);

        request
            .then(() => navigate("/admin/shows"))
            .catch((err) => {
                console.error("Error saving show:", err);
                alert(err.response?.data || "Error saving show");
            });
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    return (
        <div className="movie-form-page">
            <div className="form-nav">
                <h1>{isEdit ? "Edit Show" : "Add Show"}</h1>
                <button onClick={() => navigate("/admin/shows")} className="btn-back">
                    Back
                </button>
            </div>

            <div className="form-content">
                <form onSubmit={handleSubmit} className="movie-form">
                    <select
                        name="movieId"
                        value={formData.movieId}
                        onChange={handleChange}
                        required
                    >
                        <option value="">Select Movie</option>
                        {movies.map((m) => (
                            <option key={m.movieId} value={m.movieId}>
                                {m.movieName}
                            </option>
                        ))}
                    </select>

                    <select
                        name="theatreId"
                        value={formData.theatreId}
                        onChange={handleChange}
                        required
                    >
                        <option value="">Select Theatre</option>
                        {theatres.map((t) => (
                            <option key={t.theatreId} value={t.theatreId}>
                                {t.name} - {t.city}
                            </option>
                        ))}
                    </select>

                    <input
                        type="datetime-local"
                        name="showTime"
                        value={formData.showTime}
                        onChange={handleChange}
                        required
                    />

                    <input
                        type="number"
                        name="totalSeats"
                        placeholder="Total Seats"
                        value={formData.totalSeats}
                        onChange={handleChange}
                        required
                    />

                    <input
                        type="number"
                        name="pricePerSeat"
                        placeholder="Price Per Seat"
                        value={formData.pricePerSeat}
                        onChange={handleChange}
                        required
                    />

                    <button type="submit" className="btn-submit">
                        {isEdit ? "Update" : "Create"}
                    </button>
                </form>
            </div>
        </div>
    );
};

export default ShowForm;

