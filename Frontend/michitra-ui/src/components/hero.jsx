const Hero = ({isAuthenticated  }) => {
    const user = JSON.parse(localStorage.getItem("user") || null);

    return (
        <section className="hero">

            <div className="hero-content">
                <span className="hero-badge">✨ Premium Cinema Experience</span>

                {!isAuthenticated ? (
                    <h1 className="hero-title">Book Your Perfect Movie Night</h1>
                ) : (
                    <h1 className="hero-title">Hello {user?.fName}</h1>
                )}
                <p className="hero-text">
                    Discover the latest blockbusters, reserve premium seats, and enjoy exclusive benefits. Your cinematic journey starts here.
                </p>
                <div className="hero-buttons">
                    <a href="#browse" className="btn-primary">Browse Movies</a>
                    {!isAuthenticated && (
                        <a href="#learn" className="btn-secondary">Learn More</a>
                    )}
                </div>
            </div>
        </section>      

    );
}

export default Hero;