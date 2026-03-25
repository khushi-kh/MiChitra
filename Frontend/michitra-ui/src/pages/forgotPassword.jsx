import { useState } from "react";
import { Link } from "react-router-dom";
import "../styles/auth.css";

const PASSWORD_REGEX = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/;
const PASSWORD_MSG = "Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.";

const ForgotPassword = () => {
    const [formData, setFormData] = useState({ username: "", currentPassword: "", newPassword: "", confirmPassword: "" });
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);

    const handleChange = (e) => setFormData({ ...formData, [e.target.name]: e.target.value });

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");

        if (formData.newPassword !== formData.confirmPassword) {
            setError("Passwords do not match.");
            return;
        }

        if (!PASSWORD_REGEX.test(formData.newPassword)) {
            setError(PASSWORD_MSG);
            return;
        }

        setLoading(true);

        try {
            const response = await fetch("http://localhost:5267/api/users/forgot-password", {
                method: "PUT",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify(formData)
            });

            if (response.ok) {
                setSuccess(true);
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
                        {!success && <h1 className="auth-title">Forgot Password</h1>}
                        {!success && <p className="auth-subtitle">Enter your credentials to set a new password</p>}
                    </div>

                    {success ? (
                        <div style={{ textAlign: "center", padding: "1rem 0" }}>
                            <div style={{ fontSize: "2.5rem", marginBottom: "0.75rem" }}>✅</div>
                            <p style={{ color: "#fff", fontWeight: "600", marginBottom: "0.5rem" }}>Password updated successfully!</p>
                            <Link to="/login" className="auth-link">Back to Login</Link>
                        </div>
                    ) : (
                        <>
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
                                    <label className="form-label">Username</label>
                                    <input type="text" name="username" className="form-input"
                                        value={formData.username} onChange={handleChange} required />
                                </div>

                                <div className="form-group">
                                    <label className="form-label">Current Password</label>
                                    <input type="password" name="currentPassword" className="form-input"
                                        value={formData.currentPassword} onChange={handleChange} required />
                                </div>

                                <div className="form-group">
                                    <label className="form-label">New Password</label>
                                    <input type="password" name="newPassword" className="form-input"
                                        value={formData.newPassword} onChange={handleChange} required />
                                </div>

                                <div className="form-group">
                                    <label className="form-label">Confirm New Password</label>
                                    <input type="password" name="confirmPassword" className="form-input"
                                        value={formData.confirmPassword} onChange={handleChange} required />
                                </div>

                                <button type="submit" className="auth-button" disabled={loading}>
                                    {loading ? "Resetting..." : "Reset Password"}
                                </button>
                            </form>

                            <div className="auth-footer">
                                <p>Remembered it? <Link to="/login" className="auth-link">Back to Login</Link></p>
                            </div>
                        </>
                    )}
                </div>
            </div>
        </div>
    );
};

export default ForgotPassword;
