import { useEffect, useState } from "react";
import api from "../api/axios";

function Movies() {
    const [movies, setMovies] = useState([]);

    useEffect(() => {
        api.get("/movies")
            .then(res => setMovies(res.data))
            .catch(err => console.error(err));
    }, []);

    return (
        <>
            <h1>MiChitra- A Movie Booking App</h1>
            <div>
                <h2>Now Showing</h2>
                {movies.map(movie => (
                    <div key={movie.movieId}>
                        <h3>{movie.movieName}</h3>
                        <p>{movie.description}</p>
                    </div>
                ))}
            </div>
        </>
        
    );
}

export default Movies;
