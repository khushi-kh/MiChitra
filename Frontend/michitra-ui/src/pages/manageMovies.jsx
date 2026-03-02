import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api/axios";
import "../styles/manageMovies.css";

const ManageMovies = () => {
    const navigate = useNavigate();
    const [movies, setMovies] = useState([]);

    useEffect(() => {
        fetchMovies();
    }, []);

    const fetchMovies = () => {
        api.get("/movies")
            .then((res) => setMovies(res.data))
            .catch((err) => {
                console.error("Error fetching movies:", err);
                if (err.response?.status === 401) {
                    alert("You are not authorized to view movies. Please log in again as an admin.");
                }
            });
    };

    const handleDelete = (id) => {
        if (!window.confirm("Are you sure you want to delete this movie?")) return;

        api.delete(`/movies/${id}`)
            .then(() => {
                fetchMovies();
            })
            .catch((err) => {
                console.error("Error deleting movie:", err);
                if (err.response?.status === 401) {
                    alert("You are not authorized to delete movies. Please log in again as an admin.");
                } else {
                    alert(err.response?.data || "Error deleting movie");
                }
            });
    };

    return (
        <div className="manage-movies">
            <div className="manage-nav">
                <h1>Manage Movies</h1>
                <div className="nav-actions">
                    <button onClick={() => navigate("/admin/movies/add")} className="btn-add">Add Movie</button>
                    <button onClick={() => navigate("/admin")} className="btn-back">Back</button>
                </div>
            </div>

            <div className="manage-content">
                <div className="movies-table">
                    {movies.length === 0 ? (
                        <p style={{ color: "#b8b8c8" }}>No movies found.</p>
                    ) : (
                        movies.map((movie) => (
                            <div key={movie.movieId} className="movie-row">
                                <div className="movie-info">
                                    <div>
                                        <h3>{movie.movieName}</h3>
                                        <p>
                                            {movie.language} • Rating {movie.rating ?? 0}/10
                                        </p>
                                    </div>
                                </div>
                                <div className="movie-actions">
                                    <button
                                        onClick={() => navigate(`/admin/movies/edit/${movie.movieId}`)}
                                        className="btn-edit"
                                    >
                                        Edit
                                    </button>
                                    <button
                                        onClick={() => handleDelete(movie.movieId)}
                                        className="btn-delete"
                                    >
                                        Delete
                                    </button>
                                </div>
                            </div>
                        ))
                    )}
                </div>
            </div>
        </div>
    );
};

export default ManageMovies;
