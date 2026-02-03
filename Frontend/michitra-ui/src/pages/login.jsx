import { useState } from "react";
import { Link } from "react-router-dom";
import "../styles/auth.css";

const Login = () => {
    const [formData, setFormData] = useState({
        email: "",
        password: ""
    });

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value
        });
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        // Handle login logic here
        console.log("Login data:", formData);
    };

    return (
        <div className="auth-container">
            <div className="bg-gradient" />
            <div className="noise-overlay" />
            
            <div className="auth-content">
                <div className="auth-card">
                    <div className="auth-header">
                        <Link to="/" className="auth-logo">MiChitra</Link>
                        <h1 className="auth-title">Welcome Back</h1>
                        <p className="auth-subtitle">Sign in to continue your cinematic journey</p>
                    </div>

                    <form className="auth-form" onSubmit={handleSubmit}>
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

                        <button type="submit" className="auth-button">
                            Sign In
                        </button>
                    </form>

                    <div className="auth-footer">
                        <p>Don't have an account? <Link to="/register" className="auth-link">Create Account</Link></p>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default Login;