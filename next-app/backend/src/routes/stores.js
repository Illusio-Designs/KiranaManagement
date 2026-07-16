const express = require('express');
const bcrypt = require('bcryptjs');
const { Store, User } = require('../models');
const { requireRole } = require('../middleware/auth');

const router = express.Router();
const dto = s => ({ id: s.id, name: s.name, ownerName: s.ownerName, email: s.email, phone: s.phone, city: s.city, gstin: s.gstin, pan: s.pan, status: s.status, createdAt: s.createdAt, approvedAt: s.approvedAt, rejectionReason: s.rejectionReason });

// POST /api/stores/register  (public)
router.post('/register', async (req, res) => {
  const b = req.body || {};
  const email = (b.email || '').trim().toLowerCase();
  if (!email || !b.password) return res.status(400).json({ error: 'Email and password are required.' });
  if (await User.findOne({ where: { email } })) return res.status(409).json({ error: 'An account with this email already exists.' });
  const store = await Store.create({ name: (b.storeName || '').trim(), ownerName: (b.ownerName || '').trim(), email, phone: b.phone, addressLine: b.addressLine, city: b.city, gstin: b.gstin, pan: b.pan });
  await User.create({ email, fullName: store.ownerName, passwordHash: bcrypt.hashSync(b.password, 10), role: 'Owner', storeId: store.id });
  res.json(dto(store));
});

// GET /api/stores/pending  (SuperAdmin)
router.get('/pending', requireRole('SuperAdmin'), async (req, res) => {
  const stores = await Store.findAll({ where: { status: 'Pending' }, order: [['createdAt', 'ASC']] });
  res.json(stores.map(dto));
});

// GET /api/stores  (SuperAdmin)
router.get('/', requireRole('SuperAdmin'), async (req, res) => {
  const stores = await Store.findAll({ order: [['createdAt', 'DESC']] });
  res.json(stores.map(dto));
});

// POST /api/stores/:id/approve
router.post('/:id/approve', requireRole('SuperAdmin'), async (req, res) => {
  const s = await Store.findByPk(req.params.id);
  if (!s) return res.status(404).json({ error: 'Not found.' });
  s.status = 'Active'; s.approvedAt = new Date(); s.rejectionReason = null; await s.save();
  res.json(dto(s));
});

// POST /api/stores/:id/reject
router.post('/:id/reject', requireRole('SuperAdmin'), async (req, res) => {
  const s = await Store.findByPk(req.params.id);
  if (!s) return res.status(404).json({ error: 'Not found.' });
  s.status = 'Rejected'; s.rejectionReason = (req.body && req.body.reason) || 'Rejected by admin'; await s.save();
  res.json(dto(s));
});

module.exports = router;
