const express = require('express');
const { Order, OrderLine } = require('../models');
const { requireRole } = require('../middleware/auth');

const router = express.Router();

// Masked store view — NO consumer name / phone / full address.
function storeDto(o, storeId) {
  const mine = (o.lines || []).filter(l => l.storeId === storeId);
  const area = [o.city, o.pincode].filter(Boolean).join(' ') || 'Delivery area not specified';
  return {
    id: o.id, orderNumber: o.orderNumber, status: o.status, deliveryArea: area,
    distanceKm: o.distanceKm, etaMinutes: o.etaMinutes,
    itemCount: mine.reduce((s, l) => s + l.quantity, 0),
    storeTotal: mine.reduce((s, l) => s + +l.lineTotal, 0),
    createdAt: o.createdAt,
    lines: mine.map(l => ({ productName: l.productName, variantName: l.variantName, quantity: l.quantity, unitPrice: +l.unitPrice, lineTotal: +l.lineTotal }))
  };
}

// GET /api/store/orders  (Owner/Manager)
router.get('/', requireRole('Owner', 'Manager'), async (req, res) => {
  const orders = await Order.findAll({ include: [{ model: OrderLine, as: 'lines' }], order: [['createdAt', 'DESC']] });
  const mine = orders.filter(o => (o.lines || []).some(l => l.storeId === req.user.storeId));
  res.json(mine.map(o => storeDto(o, req.user.storeId)));
});

// PUT /api/store/orders/:id/status  (Owner/Manager)
router.put('/:id/status', requireRole('Owner', 'Manager'), async (req, res) => {
  const valid = ['Placed', 'Accepted', 'Packed', 'OutForDelivery', 'Delivered', 'Cancelled'];
  const status = (req.body && req.body.status) || '';
  if (!valid.includes(status)) return res.status(400).json({ error: 'Unknown status.' });
  const o = await Order.findByPk(req.params.id, { include: [{ model: OrderLine, as: 'lines' }] });
  if (!o || !(o.lines || []).some(l => l.storeId === req.user.storeId)) return res.status(404).json({ error: 'Not found.' });
  o.status = status; await o.save();
  res.json(storeDto(o, req.user.storeId));
});

module.exports = router;
