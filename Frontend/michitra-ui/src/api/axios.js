import axios from "axios";

const api = axios.create({
    baseURL: "http://localhost:5267/api", // backend API base URL
});

api.interceptors.request.use(config => {
    const token = localStorage.getItem("token");
    console.log("JWT Token being sent:", token); 

    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

export default api;
