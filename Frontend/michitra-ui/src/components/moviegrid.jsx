import { useNavigate } from "react-router-dom";

const MovieGrid = ({ movies }) => {
    const navigate = useNavigate();
    return (
        <section className="movie-showcase" id="movies">
            <h2 className="showcase-title">Now Showing</h2>
            <div className="showcase-grid">
                {movies.length === 0 && (
                    <p style={{ color: "#aaa" }}>Loading movies...</p>
                )}

                {movies.map((movie) => (
                    <div key={movie.MovieId} className="movie-card"
                        onClick={() => navigate(`/movies/${movie.movieId}`)} >
                        <div className="movie-poster">
                            <span className="movie-poster-icon">🎬</span>
                        </div>
                        <div className="movie-info">
                            <div className="movie-title">{movie.movieName}</div>
                            <div className="movie-meta">
                                {movie.language} • {movie.rating}
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </section>
    );
}

export default MovieGrid;