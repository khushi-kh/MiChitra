import { useNavigate } from "react-router-dom";
import { useState } from "react";

const MovieGrid = ({ movies, limit, showViewAll }) => {
    const navigate = useNavigate();
    const [currentPage, setCurrentPage] = useState(1);
    const itemsPerPage = 12;
    
    const displayMovies = limit ? movies.slice(0, limit) : movies.slice((currentPage - 1) * itemsPerPage, currentPage * itemsPerPage);
    return (
        <section className="movie-showcase" id="movies">
            <div className="showcase-header">
                <h2 className="showcase-title">Now Showing</h2>
                {showViewAll && movies.length > limit && (
                    <button className="btn-view-all" onClick={() => navigate('/movies')}>View All →</button>
                )}
            </div>
            <div className="showcase-grid">
                {movies.length === 0 && (
                    <p style={{ color: "#aaa" }}>Loading movies...</p>
                )}

                {displayMovies.map((movie) => (
                    <div key={movie.MovieId} className="movie-card"
                        onClick={() => navigate(`/movies/${movie.movieId}`)} >
                        <div className="movie-poster">
                            <span className="movie-poster-icon"><img src="https://picsum.photos/200" alt="movie poster" />
</span>
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
            {!limit && movies.length > itemsPerPage && (
                <div className="pagination">
                    <button onClick={() => setCurrentPage(p => Math.max(1, p - 1))} disabled={currentPage === 1}>←</button>
                    <span>Page {currentPage} of {Math.ceil(movies.length / itemsPerPage)}</span>
                    <button onClick={() => setCurrentPage(p => Math.min(Math.ceil(movies.length / itemsPerPage), p + 1))} disabled={currentPage === Math.ceil(movies.length / itemsPerPage)}>→</button>
                </div>
            )}
        </section>
    );
}

export default MovieGrid;