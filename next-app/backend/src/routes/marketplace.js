const express = require('express');
const { Op, fn, col } = require('sequelize');
const { Product, ProductVariant, Store, CatalogImage } = require('../models');
const { catalogKey } = require('../util');

const router = express.Router();

// GET /api/marketplace/products?q=&category=&sort=
router.get('/products', async (req, res) => {
  const { q, category, sort } = req.query;
  const stores = await Store.findAll({ where: { status: 'Active' } });
  const storeIds = stores.map(s => s.id);
  const storeNames = Object.fromEntries(stores.map(s => [s.id, s.name]));

  const where = { isActive: true, status: 'Approved', storeId: { [Op.in]: storeIds } };
  if (category) where.category = category;
  if (q) where[Op.or] = [
    { name: { [Op.like]: `%${q}%` } },
    { brand: { [Op.like]: `%${q}%` } },
    { category: { [Op.like]: `%${q}%` } }
  ];

  const products = await Product.findAll({ where, include: [{ model: ProductVariant, as: 'variants' }] });
  const images = Object.fromEntries((await CatalogImage.findAll()).map(c => [c.nameKey, c.imageUrl]));

  let result = products.map(p => {
    const variants = (p.variants || []).filter(v => v.isActive).map(v => ({
      id: v.id, name: v.name, mrp: +v.mrp, sellingPrice: +v.sellingPrice,
      discountPercent: v.mrp > 0 ? Math.round(((v.mrp - v.sellingPrice) / v.mrp) * 100) : 0,
      stockQuantity: v.stockQuantity
    }));
    if (!variants.length) return null;
    const key = catalogKey(p.name);
    return {
      id: p.id, storeId: p.storeId, storeName: storeNames[p.storeId] || '',
      name: p.name, brand: p.brand, category: p.category,
      imageUrl: p.imageUrl || images[key] || null,
      minPrice: Math.min(...variants.map(v => v.sellingPrice)),
      maxDiscountPercent: Math.max(...variants.map(v => v.discountPercent)),
      variants
    };
  }).filter(Boolean);

  switch ((sort || '').toLowerCase()) {
    case 'price_asc': result.sort((a, b) => a.minPrice - b.minPrice); break;
    case 'price_desc': result.sort((a, b) => b.minPrice - a.minPrice); break;
    case 'discount': result.sort((a, b) => b.maxDiscountPercent - a.maxDiscountPercent); break;
    default: result.sort((a, b) => a.name.localeCompare(b.name));
  }
  res.json(result);
});

// GET /api/marketplace/categories
router.get('/categories', async (req, res) => {
  const stores = await Store.findAll({ where: { status: 'Active' } });
  const storeIds = stores.map(s => s.id);
  const rows = await Product.findAll({
    where: { isActive: true, status: 'Approved', storeId: { [Op.in]: storeIds }, category: { [Op.ne]: null } },
    attributes: ['category', [fn('COUNT', col('id')), 'count']],
    group: ['category']
  });
  res.json(rows.map(r => ({ name: r.category, count: Number(r.get('count')) })).filter(c => c.name).sort((a, b) => a.name.localeCompare(b.name)));
});

module.exports = router;
