import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import api from "../api/axios";
import "../styles/manageUsers.css";

const ManageUsers = () => {
    const navigate = useNavigate();
    const [users, setUsers] = useState([]);

    useEffect(() => {
        fetchUsers();
    }, []);

    const fetchUsers = () => {
        api.get("/users")
            .then((res) => setUsers(res.data))
            .catch((err) => {
                console.error("Error fetching users:", err);
                if (err.response?.status === 403) {
                    alert("You are not authorized to view users. Please ensure you are logged in.");
                }
            });
    };

    const handleDeactivate = (id) => {
        if (!window.confirm("Are you sure you want to deactivate this user?")) return;

        api.put(`/users/deactivate/${id}`)
            .then(() => {
                setUsers((prev) =>
                    prev.map((u) =>
                        u.userId === id ? { ...u, isActive: false } : u
                    )
                );
            })
            .catch((err) => {
                console.error("Error deactivating user:", err);
                alert(err.response?.data || "Error deactivating user");
            });
    };

    const handleChangeRole = (user, newRole) => {
        api.put(`/users/${user.userId}/role`, { role: newRole })
            .then(() => {
                setUsers((prev) =>
                    prev.map((u) =>
                        u.userId === user.userId ? { ...u, role: newRole } : u
                    )
                );
            })
            .catch((err) => {
                console.error("Error updating user role:", err);
                alert(err.response?.data || "Error updating user role");
            });
    };

    return (
        <div className="manage-users">
            <div className="manage-nav">
                <h1>Manage Users</h1>
                <div className="nav-actions">
                    <button onClick={() => navigate("/register")} className="btn-add">
                        Add User
                    </button>
                    <button onClick={() => navigate("/admin")} className="btn-back">
                        Back
                    </button>
                </div>
            </div>

            <div className="manage-content">
                <div className="users-table">
                    {users.map((u) => (
                        <div key={u.userId} className="user-row">
                            <div className="user-info">
                                <div>
                                    <h3>{u.fName} {u.lName}</h3>
                                    <p>{u.email} • Username: {u.username}</p>
                                    <p>Joined: {new Date(u.createdAt).toLocaleDateString()}</p>
                                    <p>Role: {u.role}</p>
                                    <p>Status: {u.isActive ? "Active" : "Inactive"}</p>
                                </div>
                            </div>
                            <div className="user-actions">
                                <select
                                    value={u.role}
                                    onChange={(e) => handleChangeRole(u, e.target.value)}
                                    className="btn-edit"
                                >
                                    <option value="Admin">Admin</option>
                                    <option value="User">User</option>
                                </select>
                                <button
                                    onClick={() => handleDeactivate(u.userId)}
                                    className="btn-delete"
                                    disabled={!u.isActive}
                                >
                                    {u.isActive ? "Deactivate" : "Deactivated"}
                                </button>
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default ManageUsers;

