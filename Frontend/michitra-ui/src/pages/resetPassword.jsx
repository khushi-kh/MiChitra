import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../styles/auth.css";

const PASSWORD_REGEX = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
const PASSWORD_MSG = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";

const ResetPassword = () => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({ currentPassword: "", newPassword: "" });
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const handleChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");

        if (!PASSWORD_REGEX.test(formData.newPassword)) {
            setError(PASSWORD_MSG);
            return;
        }

        setLoading(true);

        try {
            const token = localStorage.getItem("token");
            const response = await fetch("http://localhost:5267/api/users/reset-password", {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`
                },
                body: JSON.stringify(formData)
            });

            if (response.ok) {
                navigate("/profile");
            } else {
                const msg = await response.text();
                setError(msg || "Failed to reset password.");
            }
        } catch {
            setError("It's not you, it's us. Please try again after some time.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="auth-container">
            <div className="bg-gradient" />
            <div className="noise-overlay" />

            <div className="auth-content">
                <div className="auth-card">
                    <div className="auth-header">
                        <Link to="/" className="auth-logo">MiChitra</Link>
                        <h1 className="auth-title">Reset Password</h1>
                        <p className="auth-subtitle">Change your current password</p>
                    </div>

                    <form className="auth-form" onSubmit={handleSubmit}>
                        {error && (
                            <div style={{
                                backgroundColor: "#fee", border: "1px solid #f88", borderRadius: "8px",
                                padding: "12px 16px", marginBottom: "1rem", color: "#c33",
                                fontSize: "14px", fontWeight: "500", boxShadow: "0 2px 8px rgba(255,0,0,0.1)"
                            }}>
                                ⚠️ {error}
                            </div>
                        )}

                        <div className="form-group">
                            <label className="form-label">Current Password</label>
                            <input type="password" name="currentPassword" className="form-input"
                                value={formData.currentPassword} onChange={handleChange} required />
                        </div>

                        <div className="form-group">
                            <label className="form-label">New Password</label>
                            <input type="password" name="newPassword" className="form-input"
                                value={formData.newPassword} onChange={handleChange} required minLength={6} />
                        </div>

                        <button type="submit" className="auth-button" disabled={loading}>
                            {loading ? "Resetting..." : "Reset Password"}
                        </button>
                    </form>

                    <div className="auth-footer">
                        <p><Link to="/profile" className="auth-link">Back to Profile</Link></p>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ResetPassword;
