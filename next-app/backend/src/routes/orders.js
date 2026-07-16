const express = require('express');
const { ProductVariant, Product, Store, Order, OrderLine } = require('../models');
const { requireRole } = require('../middleware/auth');
const { distanceKm, etaMinutes } = require('../util');

const router = express.Router();

function dto(o) {
  return {
    id: o.id, orderNumber: o.orderNumber, status: o.status,
    customerName: o.customerName, phone: o.phone, address: o.address, city: o.city, pincode: o.pincode,
    latitude: o.latitude, longitude: o.longitude, distanceKm: o.distanceKm, etaMinutes: o.etaMinutes,
    subtotal: +o.subtotal, deliveryFee: +o.deliveryFee, grandTotal: +o.grandTotal, createdAt: o.createdAt,
    lines: (o.lines || []).map(l => ({ storeName: l.storeName, productName: l.productName, variantName: l.variantName, quantity: l.quantity, unitPrice: +l.unitPrice, lineTotal: +l.lineTotal }))
  };
}

// POST /api/orders  (Customer) — geo-aware delivery estimate.
router.post('/', requireRole('Customer'), async (req, res) => {
  const b = req.body || {};
  if (!b.lines || !b.lines.length) return res.status(400).json({ error: 'Your cart is empty.' });
  if (!b.customerName || !b.phone || !b.address) return res.status(400).json({ error: 'Name, phone and address are required.' });

  const stores = await Store.findAll();
  const storeMap = Object.fromEntries(stores.map(s => [s.id, s]));

  const order = await Order.create({
    customerId: req.user.id, orderNumber: 'ORD-' + Date.now(),
    customerName: b.customerName.trim(), phone: b.phone.trim(), address: b.address.trim(),
    city: (b.city || '').trim(), pincode: (b.pincode || '').trim(),
    latitude: b.latitude, longitude: b.longitude, status: 'Placed'
  });

  let subtotal = 0; const involved = new Set();
  for (const line of b.lines) {
    const v = await ProductVariant.findByPk(line.productVariantId, { include: [Product] });
    if (!v || !v.isActive) { await order.destroy(); return res.status(400).json({ error: 'A product in your cart is no longer available.' }); }
    if (v.stockQuantity < line.quantity) { await order.destroy(); return res.status(400).json({ error: `'${v.name}' has only ${v.stockQuantity} left.` }); }
    const lineTotal = +v.sellingPrice * line.quantity;
    await OrderLine.create({ orderId: order.id, storeId: v.storeId, storeName: storeMap[v.storeId] ? storeMap[v.storeId].name : '', productVariantId: v.id, productName: v.Product ? v.Product.name : '', variantName: v.name, quantity: line.quantity, unitPrice: v.sellingPrice, lineTotal });
    v.stockQuantity -= line.quantity; await v.save();
    subtotal += lineTotal; involved.add(v.storeId);
  }

  let deliveryFee = subtotal >= 500 ? 0 : 40, dist = null, eta = null;
  if (b.latitude != null && b.longitude != null) {
    for (const id of involved) {
      const s = storeMap[id];
      if (s && s.latitude != null && s.longitude != null) {
        const d = distanceKm(s.latitude, s.longitude, b.latitude, b.longitude);
        if (dist == null || d > dist) dist = d;
      }
    }
    if (dist != null) { dist = Math.round(dist * 100) / 100; eta = etaMinutes(dist); deliveryFee = dist <= 3 ? 0 : Math.min(120, Math.ceil((dist - 3) * 8)); }
  }
  order.subtotal = subtotal; order.deliveryFee = deliveryFee; order.grandTotal = subtotal + deliveryFee;
  order.distanceKm = dist; order.etaMinutes = eta; await order.save();

  const full = await Order.findByPk(order.id, { include: [{ model: OrderLine, as: 'lines' }] });
  res.json(dto(full));
});

// GET /api/orders/mine  (Customer)
router.get('/mine', requireRole('Customer'), async (req, res) => {
  const orders = await Order.findAll({ where: { customerId: req.user.id }, include: [{ model: OrderLine, as: 'lines' }], order: [['createdAt', 'DESC']] });
  res.json(orders.map(dto));
});

// GET /api/orders/:id  (Customer — own order only)
router.get('/:id', requireRole('Customer'), async (req, res) => {
  const o = await Order.findByPk(req.params.id, { include: [{ model: OrderLine, as: 'lines' }] });
  if (!o) return res.status(404).json({ error: 'Not found.' });
  if (o.customerId !== req.user.id) return res.status(403).json({ error: 'This order does not belong to you.' });
  res.json(dto(o));
});

module.exports = router;
