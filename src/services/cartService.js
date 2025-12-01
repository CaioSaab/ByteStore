const getCartKey = () => {
  const email = localStorage.getItem('currentUserEmail');
  return email ? `cart:${email}` : 'cart:guest';
};

const readCart = () => {
  try {
    const raw = localStorage.getItem(getCartKey());
    const items = raw ? JSON.parse(raw) : [];
    return Array.isArray(items) ? items : [];
  } catch {
    return [];
  }
};

const writeCart = (items) => {
  localStorage.setItem(getCartKey(), JSON.stringify(items));
};

const getCart = async () => {
  const items = readCart();
  return { success: true, data: { items } };
};

const addToCart = async (arg1, details) => {
  const items = readCart();

  let payload;
  if (typeof arg1 === 'object' && arg1 && arg1.productId) {
    payload = {
      productId: arg1.productId,
      variationId: arg1.variationId,
      name: arg1.name,
      price: Number(arg1.price) || 0,
      image: arg1.image,
      color: arg1.color,
      size: arg1.size,
      quantity: arg1.quantity || 1,
    };
  } else {
    // Forma antiga: (productId, details)
    payload = {
      productId: arg1,
      variationId: details?.variationId,
      name: details?.name,
      price: Number(details?.price) || 0,
      image: details?.image,
      color: details?.color,
      size: details?.size,
      quantity: details?.quantity || 1,
    };
  }

  if (!payload.variationId) {
    return { success: false, error: 'Variação do produto não informada.' };
  }

  const existing = items.find(i => i.variationId === payload.variationId);
  if (existing) {
    existing.quantity += payload.quantity;
  } else {
    items.push({
      cartItemId: payload.variationId,
      id: payload.productId,
      variationId: payload.variationId,
      name: payload.name,
      image: payload.image,
      color: payload.color,
      size: payload.size,
      price: payload.price,
      quantity: payload.quantity,
    });
  }

  writeCart(items);
  return { success: true, data: { items } };
};

const updateCartItem = async (cartItemId, quantity) => {
  const items = readCart();
  const idx = items.findIndex(i => i.cartItemId === cartItemId);
  if (idx === -1) return { success: false, error: 'Item não encontrado no carrinho' };
  if (quantity <= 0) {
    items.splice(idx, 1);
  } else {
    items[idx].quantity = quantity;
  }
  writeCart(items);
  return { success: true, data: { items } };
};

const removeFromCart = async (cartItemId) => {
  const items = readCart();
  const next = items.filter(i => i.cartItemId !== cartItemId);
  writeCart(next);
  return { success: true, data: { items: next } };
};

const clearCart = async () => {
  writeCart([]);
  return { success: true, data: { items: [] } };
};

export {
  getCart,
  addToCart,
  updateCartItem,
  removeFromCart,
  clearCart,
};

