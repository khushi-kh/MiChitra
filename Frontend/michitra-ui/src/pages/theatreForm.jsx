import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import api from "../api/axios";
import "../styles/movieForm.css";

const TheatreForm = () => {
    const navigate = useNavigate();
    const { id } = useParams();
    const isEdit = !!id;

    const [formData, setFormData] = useState({
        name: "",
        city: ""
    });

    useEffect(() => {
        if (!isEdit) return;

        api.get(`/theatres/${id}`)
            .then((res) => {
                const theatre = res.data;
                setFormData({
                    name: theatre.name || "",
                    city: theatre.city || ""
                });
            })
            .catch((err) => console.error("Error fetching theatre:", err));
    }, [id, isEdit]);

    const handleSubmit = (e) => {
        e.preventDefault();

        const payload = {
            name: formData.name,
            city: formData.city
        };

        const request = isEdit
            ? api.put(`/theatres/${id}`, payload)
            : api.post("/theatres", payload);

        request
            .then(() => navigate("/admin/theatres"))
            .catch((err) => {
                console.error("Error saving theatre:", err);
                alert(err.response?.data || "Error saving theatre");
            });
    };

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
    };

    return (
        <div className="movie-form-page">
            <div className="form-nav">
                <h1>{isEdit ? "Edit Theatre" : "Add Theatre"}</h1>
                <button onClick={() => navigate("/admin/theatres")} className="btn-back">
                    Back
                </button>
            </div>

            <div className="form-content">
                <form onSubmit={handleSubmit} className="movie-form">
                    <input
                        type="text"
                        name="name"
                        placeholder="Theatre Name"
                        value={formData.name}
                        onChange={handleChange}
                        required
                    />
                    <input
                        type="text"
                        name="city"
                        placeholder="City"
                        value={formData.city}
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

export default TheatreForm;

