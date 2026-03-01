import { useState } from "react";
import api from "../api/axios";
import "../styles/paymentModal.css";

const PaymentModal = ({ ticketId, amount, onClose, onSuccess }) => {
    const [method, setMethod] = useState("CreditCard");
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");

    const [card, setCard] = useState({
        number: "",
        name: "",
        expiry: "",
        cvv: ""
    });

    const handlePay = async () => {
        setError("");

        if ((method === "CreditCard" || method === "DebitCard") && !isValidLuhn(card.number)) {
            setError("Invalid card number");
            return;
        }

        setLoading(true);

        try {
            const res = await api.post("/payment/process", {
                ticketId,
                amount,
                paymentMethod: method,
                cardNumber: card.number,
                cardHolderName: card.name,
                expiry: card.expiry,
                cvv: card.cvv
            });

            onSuccess(res.data);
        } catch (err) {
            const errorMsg = err.response?.data?.message || err.response?.data || "Payment failed";
            setError(errorMsg);
        } finally {
            setLoading(false);
        }
    };

    const isValidLuhn = (cardNumber) => {
        if (!cardNumber) return false;
        const digits = cardNumber.replace(/\s|-/g, "");
        if (!/^\d+$/.test(digits)) return false;

        let sum = 0;
        let alternate = false;
        for (let i = digits.length - 1; i >= 0; i--) {
            let n = parseInt(digits[i]);
            if (alternate) {
                n *= 2;
                if (n > 9) n -= 9;
            }
            sum += n;
            alternate = !alternate;
        }
        return sum % 10 === 0;
    };

    return (
        <div className="payment-modal-overlay">
            <div className="payment-modal">
                <h2>Payment</h2>
                <p className="amount">₹{amount}</p>

                <div className="payment-methods">
                    {["CreditCard", "DebitCard", "UPI", "NetBanking"].map((m) => (
                        <button
                            key={m}
                            className={method === m ? "active" : ""}
                            onClick={() => setMethod(m)}
                        >
                            {m}
                        </button>
                    ))}
                </div>

                {(method === "CreditCard" || method === "DebitCard") && (
                    <div className="card-form">
                        <input
                            placeholder="Card Number"
                            value={card.number}
                            onChange={(e) => setCard({ ...card, number: e.target.value })}
                        />
                        <input
                            placeholder="Name on Card"
                            value={card.name}
                            onChange={(e) => setCard({ ...card, name: e.target.value })}
                        />
                        <div className="row">
                            <input
                                placeholder="MM/YY"
                                value={card.expiry}
                                onChange={(e) => setCard({ ...card, expiry: e.target.value })}
                            />
                            <input
                                placeholder="CVV"
                                type="password"
                                value={card.cvv}
                                onChange={(e) => setCard({ ...card, cvv: e.target.value })}
                            />
                        </div>
                    </div>
                )}

                {method === "UPI" && (
                    <input placeholder="UPI ID (example@upi)" />
                )}

                {method === "NetBanking" && (
                    <select>
                        <option>Select Bank</option>
                        <option>SBI</option>
                        <option>HDFC</option>
                        <option>ICICI</option>
                    </select>
                )}

                {error && <p className="error">{error}</p>}

                <div className="payment-actions">
                    <button className="secondary" onClick={onClose}>Cancel</button>
                    <button className="primary" disabled={loading} onClick={handlePay}>
                        {loading ? "Processing..." : "Pay Now"}
                    </button>
                </div>
            </div>
        </div>
    );
};

export default PaymentModal;
