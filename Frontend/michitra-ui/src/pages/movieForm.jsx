import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import api from "../api/axios";
import "../styles/movieForm.css";

const MovieForm = () => {
    const navigate = useNavigate();
    const { id } = useParams();
    const isEdit = !!id;

    const [formData, setFormData] = useState({
        movieName: "",
        description: "",
        language: "",
        rating: ""
    });

    useEffect(() => {
        if (!isEdit) return;

        api.get(`/movies/${id}`)
            .then((res) => {
                const movie = res.data;
                setFormData({
                    movieName: movie.movieName || "",
                    description: movie.description || "",
                    language: movie.language || "",
                    rating: movie.rating?.toString() ?? ""
                });
            })
            .catch((err) => {
                console.error("Error fetching movie:", err);
                if (err.response?.status === 401) {
                    alert("You are not authorized to view this movie. Please log in again as an admin.");
                }
            });
    }, [id, isEdit]);

    const handleSubmit = (e) => {
        e.preventDefault();

        const payload = {
            movieName: formData.movieName,
            description: formData.description,
            language: formData.language,
            rating: parseFloat(formData.rating) || 0
        };

        const request = isEdit
            ? api.put(`/movies/${id}`, payload)
            : api.post("/movies", payload);

        request
            .then(() => navigate("/admin/movies"))
            .catch((err) => {
                console.error("Error saving movie:", err);
                if (err.response?.status === 401) {
                    alert("You are not authorized to modify movies. Please log in again as an admin.");
                } else {
                    alert(err.response?.data || "Error saving movie");
                }
            });
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    return (
        <div className="movie-form-page">
            <div className="form-nav">
                <h1>{isEdit ? "Edit Movie" : "Add Movie"}</h1>
                <button onClick={() => navigate("/admin/movies")} className="btn-back">Back</button>
            </div>

            <div className="form-content">
                <form onSubmit={handleSubmit} className="movie-form">
                    <input
                        type="text"
                        name="movieName"
                        placeholder="Title"
                        value={formData.movieName}
                        onChange={handleChange}
                        required
                    />
                    <textarea
                        name="description"
                        placeholder="Description"
                        value={formData.description}
                        onChange={handleChange}
                        required
                    />
                    <input
                        type="text"
                        name="language"
                        placeholder="Language"
                        value={formData.language}
                        onChange={handleChange}
                        required
                    />
                    <input
                        type="number"
                        name="rating"
                        placeholder="Rating (0-10)"
                        min="0"
                        max="10"
                        step="0.1"
                        value={formData.rating}
                        onChange={handleChange}
                    />
                    <button type="submit" className="btn-submit">
                        {isEdit ? "Update" : "Create"}
                    </button>
                </form>
            </div>
        </div>
    );
};

export default MovieForm;
