import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import Navbar from "../components/navbar";
import api from "../api/axios";
import "../styles/editProfile.css";

const EditProfile = () => {
    const isAuthenticated = !!localStorage.getItem("token");
    const navigate = useNavigate();
    const [scrolled, setScrolled] = useState(false);
    const [loading, setLoading] = useState(true);
    const [saving, setSaving] = useState(false);
    const [userId, setUserId] = useState(null);
    const [formData, setFormData] = useState({
        fName: "",
        lName: "",
        email: "",
        contactNumber: ""
    });
    const [error, setError] = useState("");
    const [success, setSuccess] = useState("");

    useEffect(() => {
        if (!isAuthenticated) {
            navigate("/login");
            return;
        }

        const handleScroll = () => setScrolled(window.scrollY > 50);
        window.addEventListener("scroll", handleScroll);
        return () => window.removeEventListener("scroll", handleScroll);
    }, [isAuthenticated, navigate]);

    useEffect(() => {
        const fetchUserData = async () => {
            try {
                const res = await api.get("/users/profile");
                setUserId(res.data.userId);
                setFormData({
                    fName: res.data.fName || "",
                    lName: res.data.lName || "",
                    email: res.data.email || "",
                    contactNumber: res.data.contactNumber || ""
                });
            } catch (err) {
                setError("Failed to load profile data");
            } finally {
                setLoading(false);
            }
        };

        if (isAuthenticated) fetchUserData();
    }, [isAuthenticated]);

    const handleChange = (e) => {
        setFormData({ ...formData, [e.target.name]: e.target.value });
        setError("");
        setSuccess("");
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setSaving(true);
        setError("");
        setSuccess("");

        try {
            await api.put(`/users/${userId}`, formData);
            setSuccess("Profile updated successfully!");
            setTimeout(() => navigate("/profile"), 1500);
        } catch (err) {
            setError(err.response?.data?.message || "Failed to update profile");
        } finally {
            setSaving(false);
        }
    };

    return (
        <div className="container">
            <div className="bg-gradient" />
            <div className="noise-overlay" />
            <Navbar isAuthenticated={isAuthenticated} scrolled={scrolled} />

            <div className="edit-profile-container">
                {loading ? (
                    <p className="loading-text">Loading...</p>
                ) : (
                    <div className="edit-profile-card">
                        <div className="edit-profile-header">
                            <h1 className="edit-profile-title">Edit Profile</h1>
                            <p className="edit-profile-subtitle">Update your personal information</p>
                        </div>

                        <form onSubmit={handleSubmit} className="edit-profile-form">
                            <div className="form-row">
                                <div className="form-group">
                                    <label className="form-label">First Name</label>
                                    <input
                                        type="text"
                                        name="fName"
                                        value={formData.fName}
                                        onChange={handleChange}
                                        className="form-input"
                                        required
                                    />
                                </div>
                                <div className="form-group">
                                    <label className="form-label">Last Name</label>
                                    <input
                                        type="text"
                                        name="lName"
                                        value={formData.lName}
                                        onChange={handleChange}
                                        className="form-input"
                                        required
                                    />
                                </div>
                            </div>

                            <div className="form-group">
                                <label className="form-label">Email</label>
                                <input
                                    type="email"
                                    name="email"
                                    value={formData.email}
                                    onChange={handleChange}
                                    className="form-input"
                                    required
                                />
                            </div>

                            <div className="form-group">
                                <label className="form-label">Contact Number</label>
                                <input
                                    type="tel"
                                    name="contactNumber"
                                    value={formData.contactNumber}
                                    onChange={handleChange}
                                    className="form-input"
                                    required
                                />
                            </div>

                            {error && <div className="error-message">{error}</div>}
                            {success && <div className="success-message">{success}</div>}

                            <div className="form-actions">
                                <button
                                    type="button"
                                    onClick={() => navigate("/profile")}
                                    className="cancel-btn"
                                >
                                    Cancel
                                </button>
                                <button
                                    type="submit"
                                    disabled={saving}
                                    className="save-btn"
                                >
                                    {saving ? "Saving..." : "Save Changes"}
                                </button>
                            </div>
                        </form>
                    </div>
                )}
            </div>
        </div>
    );
};

export default EditProfile;
