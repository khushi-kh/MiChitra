import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import "../styles/auth.css";

const Register = () => {
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        firstName: "",
        lastName: "",
        username: "",
        email: "",
        password: "",
        confirmPassword: ""
    });
    const [error, setError] = useState("");
    const [loading, setLoading] = useState(false);

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        
        if (formData.password !== formData.confirmPassword) {
            setError("Passwords do not match");
            return;
        }
        
        setLoading(true);
        
        try {
            const response = await fetch("http://localhost:5267/api/auth/register", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    username: formData.username,
                    email: formData.email,
                    password: formData.password,
                    fName: formData.firstName,
                    lName: formData.lastName
                })
            });
            
            if (response.ok) {
                const data = await response.json();
                localStorage.setItem("token", data.token);
                localStorage.setItem("user", JSON.stringify(data.user));
                navigate("/");
            } else {
                const errorText = await response.text();
                try {
                    const errorJson = JSON.parse(errorText);
                    if (errorJson.errors) {
                        const firstError = Object.values(errorJson.errors).flat()[0];
                        setError(firstError);
                    } else if (errorText.includes("Username already exists")) {
                        setError("Username is not available.");
                    } else if (errorText.includes("Email already exists")) {
                        setError("Email is already registered.");
                    } else {
                        setError(errorJson.title || errorText || "Registration failed");
                    }
                } catch {
                    if (errorText.includes("Username already exists")) {
                        setError("Username is not available.");
                    } else if (errorText.includes("Email already exists")) {
                        setError("Email is already registered.");
                    } else {
                        setError(errorText || "Registration failed");
                    }
                }
            }
        } catch (err) {
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
                        <h1 className="auth-title">Join the Experience</h1>
                        <p className="auth-subtitle">Create your account and start your cinematic journey</p>
                    </div>

                    <form className="auth-form" onSubmit={handleSubmit}>
                        {error && (
                            <div style={{
                                backgroundColor: "#fee",
                                border: "1px solid #f88",
                                borderRadius: "8px",
                                padding: "12px 16px",
                                marginBottom: "1rem",
                                color: "#c33",
                                fontSize: "14px",
                                fontWeight: "500",
                                boxShadow: "0 2px 8px rgba(255, 0, 0, 0.1)"
                            }}>
                                ⚠️ {error}
                            </div>
                        )}
                        <div className="form-row">
                            <div className="form-group">
                                <label className="form-label">First Name</label>
                                <input
                                    type="text"
                                    name="firstName"
                                    className="form-input"
                                    value={formData.firstName}
                                    onChange={handleChange}
                                    required
                                />
                            </div>
                            <div className="form-group">
                                <label className="form-label">Last Name</label>
                                <input
                                    type="text"
                                    name="lastName"
                                    className="form-input"
                                    value={formData.lastName}
                                    onChange={handleChange}
                                    required
                                />
                            </div>
                        </div>

                        <div className="form-group">
                            <label className="form-label">Username</label>
                            <input
                                type="text"
                                name="username"
                                className="form-input"
                                value={formData.username}
                                onChange={handleChange}
                                required
                            />
                        </div>

                        <div className="form-group">
                            <label className="form-label">Email Address</label>
                            <input
                                type="email"
                                name="email"
                                className="form-input"
                                value={formData.email}
                                onChange={handleChange}
                                required
                            />
                        </div>

                        <div className="form-group">
                            <label className="form-label">Password</label>
                            <input
                                type="password"
                                name="password"
                                className="form-input"
                                value={formData.password}
                                onChange={handleChange}
                                required
                            />
                        </div>

                        <div className="form-group">
                            <label className="form-label">Confirm Password</label>
                            <input
                                type="password"
                                name="confirmPassword"
                                className="form-input"
                                value={formData.confirmPassword}
                                onChange={handleChange}
                                required
                            />
                        </div>

                        <button type="submit" className="auth-button" disabled={loading}>
                            {loading ? "Creating Account..." : "Create Account"}
                        </button>
                    </form>

                    <div className="auth-footer">
                        <p>Already have an account? <Link to="/login" className="auth-link">Sign In</Link></p>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Register;