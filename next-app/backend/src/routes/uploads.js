const express = require('express');
const multer = require('multer');
const path = require('path');
const fs = require('fs');
const crypto = require('crypto');
const { requireRole } = require('../middleware/auth');

const router = express.Router();
const dir = path.join(__dirname, '..', '..', 'uploads', 'products');
fs.mkdirSync(dir, { recursive: true });

const storage = multer.diskStorage({
  destination: (req, file, cb) => cb(null, dir),
  filename: (req, file, cb) => {
    const ext = (path.extname(file.originalname) || '.jpg').toLowerCase();
    cb(null, crypto.randomBytes(16).toString('hex') + ext);
  }
});
const allowed = ['.jpg', '.jpeg', '.png', '.gif', '.webp'];
const upload = multer({
  storage,
  limits: { fileSize: 5 * 1024 * 1024 },
  fileFilter: (req, file, cb) => cb(null, allowed.includes((path.extname(file.originalname) || '').toLowerCase()))
});

// POST /api/uploads/image  (Owner/Manager/SuperAdmin) — field name: "file"
router.post('/image', requireRole('Owner', 'Manager', 'SuperAdmin'), upload.single('file'), (req, res) => {
  if (!req.file) return res.status(400).json({ error: 'No image file uploaded.' });
  const base = process.env.PUBLIC_URL || '';
  res.json({ url: `${base}/uploads/products/${req.file.filename}` });
});

module.exports = router;
