import { useState } from "react";
import api from "../api/axios";
import "../styles/paymentModal.css";

const METHODS = [
    { id: "CreditCard", label: "Credit Card", icon: "💳" },
    { id: "DebitCard",  label: "Debit Card",  icon: "🏧" },
    { id: "UPI",        label: "UPI",         icon: "📱" },
    { id: "NetBanking", label: "Net Banking", icon: "🏦" },
];

const BANKS = ["SBI", "HDFC", "ICICI", "Axis", "Kotak", "PNB", "BOB"];

const isValidLuhn = (number) => {
    const digits = number.replace(/\s/g, "");
    if (!/^\d{16}$/.test(digits)) return false;
    let sum = 0, alt = false;
    for (let i = digits.length - 1; i >= 0; i--) {
        let n = parseInt(digits[i]);
        if (alt) { n *= 2; if (n > 9) n -= 9; }
        sum += n;
        alt = !alt;
    }
    return sum % 10 === 0;
};

const isValidExpiry = (expiry) => {
    if (!/^\d{2}\/\d{2}$/.test(expiry)) return false;
    const [month, year] = expiry.split("/").map(Number);
    if (month < 1 || month > 12) return false;
    const now = new Date();
    const cy = now.getFullYear() % 100, cm = now.getMonth() + 1;
    return year > cy || (year === cy && month >= cm);
};

const isValidUpi = (upi) => /^[\w.\-]{2,256}@[a-zA-Z]{2,64}$/.test(upi);

const formatCardNumber = (val) =>
    val.replace(/\D/g, "").slice(0, 16).replace(/(.{4})/g, "$1 ").trim();

const formatExpiry = (val) => {
    const digits = val.replace(/\D/g, "").slice(0, 4);
    return digits.length > 2 ? `${digits.slice(0, 2)}/${digits.slice(2)}` : digits;
};

const PaymentModal = ({ ticketId, amount, onClose, onSuccess }) => {
    const [method, setMethod] = useState("CreditCard");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    const [card, setCard] = useState({ number: "", name: "", expiry: "", cvv: "" });
    const [upi, setUpi] = useState("");
    const [bank, setBank] = useState("");

    const validate = () => {
        if (method === "CreditCard" || method === "DebitCard") {
            if (!isValidLuhn(card.number.replace(/\s/g, ""))) return "Invalid card number.";
            if (!card.name.trim()) return "Name on card is required.";
            if (!isValidExpiry(card.expiry)) return "Invalid or expired expiry date.";
            if (!/^\d{3}$/.test(card.cvv)) return "CVV must be 3 digits.";
        }
        if (method === "UPI") {
            if (!isValidUpi(upi)) return "Enter a valid UPI ID (e.g. name@upi).";
        }
        if (method === "NetBanking") {
            if (!bank) return "Please select a bank.";
        }
        return null;
    };

    const handlePay = async () => {
        const validationError = validate();
        if (validationError) { setError(validationError); return; }

        setError("");
        setLoading(true);
        try {
            const res = await api.post("/payment/process", {
                ticketId,
                amount,
                paymentMethod: method,
                cardNumber: card.number.replace(/\s/g, ""),
                cardHolderName: card.name,
                expiry: card.expiry,
                cvv: card.cvv,
                upiId: upi,
                bankName: bank,
            });
            onSuccess(res.data);
        } catch (err) {
            setError(err.response?.data?.message || err.response?.data || "Payment failed. Please try again.");
        } finally {
            setLoading(false);
        }
    };

    const isCard = method === "CreditCard" || method === "DebitCard";

    return (
        <div className="payment-modal-overlay" onClick={(e) => e.target === e.currentTarget && onClose()}>
            <div className="payment-modal">
                <div className="payment-modal-header">
                    <h2>Complete Payment</h2>
                    <button className="payment-modal-close" onClick={onClose}>✕</button>
                </div>

                <div className="payment-amount-box">
                    <span className="payment-amount-label">Total Amount</span>
                    <span className="payment-amount-value">₹{amount}</span>
                </div>

                <div className="payment-methods">
                    {METHODS.map(({ id, label, icon }) => (
                        <button
                            key={id}
                            className={`payment-method-btn ${method === id ? "active" : ""}`}
                            onClick={() => { setMethod(id); setError(""); }}
                        >
                            <span className="method-icon">{icon}</span>
                            <span className="method-label">{label}</span>
                        </button>
                    ))}
                </div>

                <div className="payment-form">
                    {isCard && (
                        <>
                            <div className="form-group">
                                <label>Card Number</label>
                                <input
                                    placeholder="1234 5678 9012 3456"
                                    value={card.number}
                                    onChange={(e) => setCard({ ...card, number: formatCardNumber(e.target.value) })}
                                    maxLength={19}
                                />
                            </div>
                            <div className="form-group">
                                <label>Name on Card</label>
                                <input
                                    placeholder="John Doe"
                                    value={card.name}
                                    onChange={(e) => setCard({ ...card, name: e.target.value })}
                                />
                            </div>
                            <div className="form-row">
                                <div className="form-group">
                                    <label>Expiry</label>
                                    <input
                                        placeholder="MM/YY"
                                        value={card.expiry}
                                        onChange={(e) => setCard({ ...card, expiry: formatExpiry(e.target.value) })}
                                        maxLength={5}
                                    />
                                </div>
                                <div className="form-group">
                                    <label>CVV</label>
                                    <input
                                        placeholder="•••"
                                        type="password"
                                        value={card.cvv}
                                        maxLength={3}
                                        onChange={(e) => setCard({ ...card, cvv: e.target.value.replace(/\D/g, "").slice(0, 3) })}
                                    />
                                </div>
                            </div>
                        </>
                    )}

                    {method === "UPI" && (
                        <div className="form-group">
                            <label>UPI ID</label>
                            <input
                                placeholder="yourname@upi"
                                value={upi}
                                onChange={(e) => setUpi(e.target.value)}
                            />
                        </div>
                    )}

                    {method === "NetBanking" && (
                        <div className="form-group">
                            <label>Select Bank</label>
                            <select value={bank} onChange={(e) => setBank(e.target.value)}>
                                <option value="">-- Choose your bank --</option>
                                {BANKS.map((b) => <option key={b} value={b}>{b}</option>)}
                            </select>
                        </div>
                    )}
                </div>

                {error && <p className="payment-error">{error}</p>}

                <div className="payment-actions">
                    <button className="btn-cancel" onClick={onClose} disabled={loading}>Cancel</button>
                    <button className="btn-pay" disabled={loading} onClick={handlePay}>
                        {loading ? <span className="paying-spinner">Processing...</span> : `Pay ₹${amount}`}
                    </button>
                </div>
            </div>
        </div>
    );
};

export default PaymentModal;
