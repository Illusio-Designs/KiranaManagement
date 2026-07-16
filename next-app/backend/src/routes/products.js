const express = require('express');
const { Op } = require('sequelize');
const { Product, ProductVariant, Store, CatalogImage } = require('../models');
const { requireRole } = require('../middleware/auth');
const { catalogKey, deriveImageName } = require('../util');

const router = express.Router();

function dto(p, storeName) {
  return {
    id: p.id, storeId: p.storeId, storeName: storeName || '',
    name: p.name, description: p.description, brand: p.brand, category: p.category,
    imageUrl: p.imageUrl, isActive: p.isActive, status: p.status, rejectionReason: p.rejectionReason,
    variants: (p.variants || []).map(v => ({
      id: v.id, name: v.name, sku: v.sku, unit: v.unit, packSize: +v.packSize,
      mrp: +v.mrp, sellingPrice: +v.sellingPrice,
      discountAmount: +v.mrp - +v.sellingPrice,
      discountPercent: v.mrp > 0 ? Math.round(((v.mrp - v.sellingPrice) / v.mrp) * 100) : 0,
      taxRatePercent: +v.taxRatePercent, stockQuantity: v.stockQuantity, isActive: v.isActive
    }))
  };
}

// POST /api/products  (Owner/Manager) — created Pending, with master-image reuse.
router.post('/', requireRole('Owner', 'Manager'), async (req, res) => {
  const b = req.body || {};
  if (!b.name) return res.status(400).json({ error: 'Product name is required.' });
  if (!b.variants || !b.variants.length) return res.status(400).json({ error: 'A product needs at least one variant.' });

  const key = catalogKey(b.name);
  let imageUrl = (b.imageUrl || '').trim();
  if (imageUrl) {
    if (!(await CatalogImage.findOne({ where: { nameKey: key } })))
      await CatalogImage.create({ nameKey: key, name: b.name.trim(), imageName: deriveImageName(b.name), imageUrl });
  } else {
    const master = await CatalogImage.findOne({ where: { nameKey: key } });
    if (master) imageUrl = master.imageUrl;
  }

  const product = await Product.create({
    storeId: req.user.storeId, name: b.name.trim(), description: b.description,
    brand: b.brand, category: (b.category || '').trim(), imageUrl: imageUrl || null, status: 'Pending'
  });
  for (const v of b.variants)
    await ProductVariant.create({
      productId: product.id, storeId: req.user.storeId, name: (v.name || '').trim(), sku: v.sku, barcode: v.barcode,
      unit: v.unit || 'Piece', packSize: v.packSize > 0 ? v.packSize : 1, mrp: v.mrp, sellingPrice: v.sellingPrice,
      taxRatePercent: v.taxRatePercent || 0, stockQuantity: Math.max(0, v.stockQuantity || 0), reorderLevel: Math.max(0, v.reorderLevel || 0)
    });

  const full = await Product.findByPk(product.id, { include: [{ model: ProductVariant, as: 'variants' }] });
  res.json(dto(full));
});

// GET /api/products  (Owner/Manager) — this store's products.
router.get('/', requireRole('Owner', 'Manager'), async (req, res) => {
  const products = await Product.findAll({ where: { storeId: req.user.storeId }, include: [{ model: ProductVariant, as: 'variants' }], order: [['name', 'ASC']] });
  res.json(products.map(p => dto(p)));
});

// GET /api/products/pending-approval  (SuperAdmin)
router.get('/pending-approval', requireRole('SuperAdmin'), async (req, res) => {
  const stores = Object.fromEntries((await Store.findAll()).map(s => [s.id, s.name]));
  const products = await Product.findAll({ where: { status: 'Pending' }, include: [{ model: ProductVariant, as: 'variants' }], order: [['createdAt', 'ASC']] });
  res.json(products.map(p => dto(p, stores[p.storeId])));
});

// POST /api/products/:id/approve  (SuperAdmin)
router.post('/:id/approve', requireRole('SuperAdmin'), async (req, res) => {
  const p = await Product.findByPk(req.params.id);
  if (!p) return res.status(404).json({ error: 'Not found.' });
  p.status = 'Approved'; p.approvedAt = new Date(); p.rejectionReason = null; await p.save();
  res.json({ id: p.id, status: p.status });
});

// POST /api/products/:id/reject  (SuperAdmin)
router.post('/:id/reject', requireRole('SuperAdmin'), async (req, res) => {
  const p = await Product.findByPk(req.params.id);
  if (!p) return res.status(404).json({ error: 'Not found.' });
  p.status = 'Rejected'; p.rejectionReason = (req.body && req.body.reason) || 'Rejected by admin'; await p.save();
  res.json({ id: p.id, status: p.status });
});

// POST /api/products/variants/:id/adjust  (Owner/Manager)
router.post('/variants/:id/adjust', requireRole('Owner', 'Manager'), async (req, res) => {
  const v = await ProductVariant.findOne({ where: { id: req.params.id, storeId: req.user.storeId } });
  if (!v) return res.status(404).json({ error: 'Not found.' });
  const bal = v.stockQuantity + Number(req.body.changeQuantity || 0);
  if (bal < 0) return res.status(400).json({ error: 'Insufficient stock.' });
  v.stockQuantity = bal; await v.save();
  res.json({ variantId: v.id, stockQuantity: v.stockQuantity });
});

// GET /api/products/low-stock  (Owner/Manager)
router.get('/low-stock', requireRole('Owner', 'Manager'), async (req, res) => {
  const vs = await ProductVariant.findAll({ where: { storeId: req.user.storeId, isActive: true }, include: [Product] });
  const low = vs.filter(v => v.stockQuantity <= v.reorderLevel)
    .map(v => ({ variantId: v.id, productName: v.Product ? v.Product.name : '', variantName: v.name, stockQuantity: v.stockQuantity, reorderLevel: v.reorderLevel }));
  res.json(low);
});

module.exports = router;
