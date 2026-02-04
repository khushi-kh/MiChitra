const Footer = () => {
    return (
        <footer className="footer">
            <div className="footer-content">
                <div className="footer-brand">
                    <div className="footer-logo">MiChitra</div>
                    <p className="footer-desc">
                        Your premier destination for unforgettable movie experiences.
                        Book smarter, watch better.
                    </p>
                </div>

                <div>
                    <h4 className="footer-heading">Company</h4>
                    <ul className="footer-links">
                        <li><a href="#about" className="footer-link">About Us</a></li>
                        <li><a href="#careers" className="footer-link">Careers</a></li>
                        <li><a href="#press" className="footer-link">Press</a></li>
                        <li><a href="#blog" className="footer-link">Blog</a></li>
                    </ul>
                </div>

                <div>
                    <h4 className="footer-heading">Support</h4>
                    <ul className="footer-links">
                        <li><a href="#help" className="footer-link">Help Center</a></li>
                        <li><a href="#contact" className="footer-link">Contact Us</a></li>
                        <li><a href="#privacy" className="footer-link">Privacy Policy</a></li>
                        <li><a href="#terms" className="footer-link">Terms of Service</a></li>
                    </ul>
                </div>

                <div>
                    <h4 className="footer-heading">Connect</h4>
                    <ul className="footer-links">
                        <li><a href="#twitter" className="footer-link">Twitter</a></li>
                        <li><a href="#instagram" className="footer-link">Instagram</a></li>
                        <li><a href="#facebook" className="footer-link">Facebook</a></li>
                        <li><a href="#youtube" className="footer-link">YouTube</a></li>
                    </ul>
                </div>
            </div>

            <div className="footer-bottom">
                <p>© 2026 MiChitra. All rights reserved.</p>
            </div>
        </footer>
    )
};
export default Footer;