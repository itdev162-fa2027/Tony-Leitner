const API_BASE_URL = "http://localhost:5281";

export const getProducts = async () => {
  try {
    const response = await fetch(`${API_BASE_URL}/products`);
    if (!response.ok) {
      throw new Error("Failed to fetch products");
    }
    return await response.json();
  } catch (error) {
    console.error("Error fetching products:", error);
    throw error;
  }
};

export const getProductById = async (id) => {
  try {
    const response = await fetch(`${API_BASE_URL}/products/${id}`);
    if (!response.ok) {
      throw new Error("Failed to fetch product");
    }
    return await response.json();
  } catch (error) {
    console.error("Error fetching product:", error);
    throw error;
  }
};

export const searchProducts = async (searchTerm) => {
  try {
    const url = searchTerm
      ? `${API_BASE_URL}/products/search?name=${encodeURIComponent(searchTerm)}`
      : `${API_BASE_URL}/products`;
    const response = await fetch(url);
    if (!response.ok) throw new Error("Failed to search products");
    return await response.json();
  } catch (error) {
    console.error("Error searching products:", error);
    throw error;
  }
};

export const createOrder = async (customerEmail, cartItems) => {
  try {
    const response = await fetch(`${API_BASE_URL}/api/orders`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        customerEmail,
        items: cartItems.map((item) => ({
          productId: item.product.id,
          quantity: item.quantity,
        })),
      }),
    });
    if (!response.ok) {
      const contentType = response.headers.get("Content-Type") || "";
      const isMessage =
        response.status === 400 && contentType.includes("text/plain");
      const message = isMessage ? await response.text() : "";
      throw new Error(message || "Failed to create order");
    }
    return await response.json();
  } catch (error) {
    console.error("Error creating order:", error);
    throw error;
  }
};

export const getOrderById = async (orderId) => {
  try {
    const response = await fetch(`${API_BASE_URL}/api/orders/${orderId}`);
    if (!response.ok) {
      throw new Error("Failed to fetch order");
    }
    return await response.json();
  } catch (error) {
    console.error("Error fetching order:", error);
    throw error;
  }
};
