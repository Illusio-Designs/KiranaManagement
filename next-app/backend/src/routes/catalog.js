const express = require('express');
const { CatalogImage, Product } = require('../models');
const { requireRole } = require('../middleware/auth');
const { catalogKey, deriveImageName } = require('../util');

const router = express.Router();

// GET /api/catalog/image?name=  — master image for a product name (for auto-fill).
router.get('/image', async (req, res) => {
  const name = req.query.name;
  if (!name) return res.json(null);
  const m = await CatalogImage.findOne({ where: { nameKey: catalogKey(name) } });
  res.json(m ? { name: m.name, imageName: m.imageName, imageUrl: m.imageUrl } : null);
});

// GET /api/catalog/images  (SuperAdmin)
router.get('/images', requireRole('SuperAdmin'), async (req, res) => {
  const list = await CatalogImage.findAll({ order: [['name', 'ASC']] });
  res.json(list.map(c => ({ name: c.name, imageName: c.imageName, imageUrl: c.imageUrl })));
});

// PUT /api/catalog/image  (SuperAdmin) — add / replace a shared image, and
// propagate it to products that were using the shared image.
router.put('/image', requireRole('SuperAdmin'), async (req, res) => {
  const { name, imageUrl } = req.body || {};
  if (!name || !imageUrl) return res.status(400).json({ error: 'Name and image are required.' });
  const key = catalogKey(name);
  let m = await CatalogImage.findOne({ where: { nameKey: key } });
  const oldUrl = m ? m.imageUrl : null;
  if (!m) m = await CatalogImage.create({ nameKey: key, name: name.trim(), imageName: deriveImageName(name), imageUrl: imageUrl.trim() });
  else { m.name = name.trim(); m.imageUrl = imageUrl.trim(); await m.save(); }

  const products = await Product.findAll();
  for (const p of products)
    if (catalogKey(p.name) === key && (!p.imageUrl || p.imageUrl === oldUrl)) { p.imageUrl = m.imageUrl; await p.save(); }

  res.json({ name: m.name, imageName: m.imageName, imageUrl: m.imageUrl });
});

module.exports = router;
