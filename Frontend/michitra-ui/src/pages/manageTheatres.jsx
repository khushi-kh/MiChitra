import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api/axios";
import "../styles/manageTheatres.css";

const ManageTheatres = () => {
    const navigate = useNavigate();
    const [theatres, setTheatres] = useState([]);

    useEffect(() => {
        fetchTheatres();
    }, []);

    const fetchTheatres = () => {
        api.get("/theatres")
            .then((res) => setTheatres(res.data))
            .catch((err) => console.error("Error fetching theatres:", err));
    };

    const handleDeactivate = (id) => {
        if (window.confirm("Are you sure you want to deactivate this theatre?")) {
            api.put(`/theatres/deactivate/${id}`)
                .then(() => {
                    fetchTheatres();
                })
                .catch((err) => {
                    console.error("Error deactivating theatre:", err);
                    alert(err.response?.data || "Error deactivating theatre");
                });
        }
    };

    return (
        <div className="manage-theatres">
            <div className="manage-nav">
                <h1>Manage Theatres</h1>
                <div className="nav-actions">
                    <button
                        onClick={() => navigate("/admin/theatres/add")}
                        className="btn-add"
                    >
                        Add Theatre
                    </button>
                    <button onClick={() => navigate("/admin")} className="btn-back">
                        Back
                    </button>
                </div>
            </div>

            <div className="manage-content">
                <div className="theatres-table">
                    {theatres.map((theatre) => (
                        <div key={theatre.theatreId} className="theatre-row">
                            <div className="theatre-info">
                                <div>
                                    <h3>{theatre.name}</h3>
                                    <p>{theatre.city}</p>
                                    <p style={{ fontSize: "0.8rem", color: theatre.isActive ? "#4ade80" : "#f87171" }}>
                                        {theatre.isActive ? "Active" : "Inactive"}
                                    </p>
                                </div>
                            </div>
                            <div className="theatre-actions">
                                <button
                                    onClick={() => navigate(`/admin/theatres/edit/${theatre.theatreId}`)}
                                    className="btn-edit"
                                >
                                    Edit
                                </button>
                                {theatre.isActive ? (
                                    <button
                                        onClick={() => handleDeactivate(theatre.theatreId)}
                                        className="btn-delete"
                                    >
                                        Deactivate
                                    </button>
                                ) : (
                                    <button
                                        onClick={() => api.put(`/theatres/activate/${theatre.theatreId}`).then(fetchTheatres)}
                                        className="btn-add"
                                    >
                                        Activate
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

export default ManageTheatres;

