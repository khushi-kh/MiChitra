import { useNavigate } from "react-router-dom";

const TheatreGrid = ({ theatres }) => {
    const navigate = useNavigate();
    return (
        <section className="theatre-showcase" id="theatres" style={{ paddingTop: '6rem' }}>
            <h2 className="showcase-title">Available Theatres</h2>
            <div className="showcase-grid">
                {theatres.length === 0 && (
                    <p style={{ color: "#aaa" }}>Loading theatres...</p>
                )}

                {theatres.map((theatre) => (
                    <div key={theatre.theatreId} className="theatre-card"
                        onClick={() => navigate(`/theatres/${theatre.theatreId}`)} >
                        <div className="theatre-poster">
                            <span className="theatre-poster-icon"><img src="https://picsum.photos/200" alt="theatre poster" />
                            </span>
                        </div>
                        <div className="theatre-info">
                            <div className="theatre-title">{theatre.name}</div>
                            <div className="theatre-meta">
                                {theatre.city}
                            </div>
                        </div>
                    </div>
                ))}
            </div>
        </section>
    );
}

export default TheatreGrid;