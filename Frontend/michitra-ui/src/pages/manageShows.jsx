import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api/axios";
import "../styles/manageShows.css";

const ManageShows = () => {
    const navigate = useNavigate();
    const [shows, setShows] = useState([]);

    useEffect(() => {
        fetchShows();
    }, []);

    const fetchShows = () => {
        api.get("/movieshows")
            .then((res) => setShows(res.data))
            .catch((err) => console.error("Error fetching shows:", err));
    };

    const handleDelete = (id) => {
        if (window.confirm("Are you sure you want to delete this show? Existing bookings may be affected.")) {
            api.delete(`/movieshows/${id}`)
                .then(() => {
                    fetchShows();
                })
                .catch((err) => {
                    console.error("Error deleting show:", err);
                    alert(err.response?.data || "Error deleting show");
                });
        }
    };

    return (
        <div className="manage-shows">
            <div className="manage-nav">
                <h1>Manage Shows</h1>
                <div className="nav-actions">
                    <button
                        onClick={() => navigate("/admin/shows/add")}
                        className="btn-add"
                    >
                        Add Show
                    </button>
                    <button onClick={() => navigate("/admin")} className="btn-back">
                        Back
                    </button>
                </div>
            </div>

            <div className="manage-content">
                <div className="shows-table">
                    {shows.map((show) => (
                        <div key={show.id} className="show-row">
                            <div className="show-info">
                                <div>
                                    <h3>{show.movieName}</h3>
                                    <p>
                                        {show.theatreName} • {show.city} •{" "}
                                        {new Date(show.showTime || show.startTime).toLocaleString()}
                                    </p>
                                </div>
                            </div>
                            <div className="show-actions">
                                {/* Hook this up to an edit page when you create it */}
                                {/* <button
                                    onClick={() => navigate(`/admin/shows/edit/${show.id}`)}
                                    className="btn-edit"
                                >
                                    Edit
                                </button> */}
                                <button
                                    onClick={() => handleDelete(show.id)}
                                    className="btn-delete"
                                >
                                    Delete
                                </button>
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default ManageShows;

