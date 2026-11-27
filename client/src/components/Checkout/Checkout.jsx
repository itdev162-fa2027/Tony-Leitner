import { useState, startTransition } from "react";
import { useNavigate } from "react-router-dom";
import { createOrder } from "../../services/api";
import "./Checkout.css";

function Checkout({ cartItems, cartTotal, clearCart }) {
  const navigate = useNavigate();
  const [email, setEmail] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  if (cartItems.length === 0) {
    return (
      <div className="checkout-container">
        <div className="empty-cart">
          <h2>Your cart is empty</h2>
          <p>Add some products before checking out.</p>
          <button onClick={() => navigate("/")} className="continue-button">
            Continue Shopping
          </button>
        </div>
      </div>
    );
  }

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    try {
      const order = await createOrder(email, cartItems);
      startTransition(() => {
        clearCart();
        navigate(`/order/success?orderId=${order.id}`);
      });
    } catch (err) {
      console.error("Order creation error:", err);
      setError(err.message || "Failed to create order. Please try again.");
      setLoading(false);
    }
  };

  return (
    <div className="checkout-container">
      <div className="checkout-content">
        <div className="order-summary">
          <h2>Order Summary</h2>
          <div className="summary-items">
            {cartItems.map((item) => {
              const price = item.product.isOnSale
                ? item.product.salePrice
                : item.product.price;
              const subtotal = price * item.quantity;
              return (
                <div key={item.product.id} className="summary-item">
                  <img
                    src={
                      item.product.imageUrl ||
                      "https://placehold.co/60x60/e0e0e0/666?text=No+Image"
                    }
                    alt={item.product.name}
                    className="summary-item-image"
                  />
                  <div className="summary-item-details">
                    <h4>{item.product.name}</h4>
                    <p>
                      ${price.toFixed(2)} {"\u00D7"} {item.quantity}
                    </p>
                  </div>
                  <div className="summary-item-total">
                    ${subtotal.toFixed(2)}
                  </div>
                </div>
              );
            })}
          </div>
          <div className="summary-total">
            <span>Total:</span>
            <span className="total-amount">${cartTotal.toFixed(2)}</span>
          </div>
        </div>
        <div className="checkout-form-section">
          <h2>Checkout</h2>
          <form onSubmit={handleSubmit} className="checkout-form">
            <div className="form-group">
              <label htmlFor="email">Email Address</label>
              <input
                type="email"
                id="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                required
                placeholder="your@email.com"
                disabled={loading}
              />
              <p className="form-help">
                We'll save this email with your order
              </p>
            </div>
            {error && <div className="error-message">{error}</div>}
            <button
              type="submit"
              className="place-order-button"
              disabled={loading}
            >
              {loading ? "Processing..." : "Place Order"}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}

export default Checkout;
