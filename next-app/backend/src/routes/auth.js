const express = require('express');
const bcrypt = require('bcryptjs');
const { Op } = require('sequelize');
const { User, Store, OtpCode } = require('../models');
const { token, otpCode } = require('../util');

const router = express.Router();
const lower = e => (e || '').trim().toLowerCase();

function loginResponse(u) {
  return { token: u.sessionToken, role: u.role, fullName: u.fullName, email: u.email, storeId: u.storeId };
}

// POST /api/auth/register-customer
router.post('/register-customer', async (req, res) => {
  const { fullName, email, password } = req.body || {};
  if (!email || !password) return res.status(400).json({ error: 'Name, email and password are required.' });
  const em = lower(email);
  if (await User.findOne({ where: { email: em } }))
    return res.status(409).json({ error: 'An account with this email already exists.' });
  await User.create({ email: em, fullName: (fullName || '').trim(), passwordHash: bcrypt.hashSync(password, 10), role: 'Customer' });
  res.json({ email: em });
});

// POST /api/auth/login
router.post('/login', async (req, res) => {
  const { email, password } = req.body || {};
  if (!email || !password) return res.status(400).json({ error: 'Email and password are required.' });
  const u = await User.findOne({ where: { email: lower(email) } });
  if (!u || !u.passwordHash || !bcrypt.compareSync(password, u.passwordHash))
    return res.status(401).json({ error: 'Invalid email or password.' });
  if (!u.isActive) return res.status(401).json({ error: 'This account is disabled.' });
  if (u.role !== 'SuperAdmin' && u.storeId) {
    const store = await Store.findByPk(u.storeId);
    if (!store || store.status !== 'Active')
      return res.status(401).json({ error: 'Your store is not active yet. Please wait for admin approval.' });
  }
  u.sessionToken = token(); await u.save();
  res.json(loginResponse(u));
});

// GET /api/auth/me
router.get('/me', async (req, res) => {
  if (!req.user) return res.status(401).json({ error: 'Not signed in.' });
  res.json(loginResponse(req.user));
});

// POST /api/auth/google  (store owner)
router.post('/google', async (req, res) => {
  const { idToken } = req.body || {};
  if (!idToken) return res.status(400).json({ error: 'Google token is required.' });
  try {
    const r = await fetch('https://oauth2.googleapis.com/tokeninfo?id_token=' + encodeURIComponent(idToken));
    if (!r.ok) return res.status(401).json({ error: 'Google sign-in could not be verified.' });
    const g = await r.json();
    const clientId = process.env.GOOGLE_CLIENT_ID;
    if (clientId && g.aud !== clientId) return res.status(401).json({ error: 'Google sign-in could not be verified.' });
    if (g.email_verified !== 'true' && g.email_verified !== true)
      return res.status(401).json({ error: 'Google email is not verified.' });

    const u = await User.findOne({ where: { email: lower(g.email) } });
    if (!u) return res.status(401).json({ error: 'No account is registered with this Google email. Please register your store first.' });
    if (!u.isActive) return res.status(401).json({ error: 'This account is disabled.' });
    u.provider = 'Google'; u.sessionToken = token(); await u.save();
    res.json(loginResponse(u));
  } catch (e) {
    res.status(401).json({ error: 'Google sign-in failed.' });
  }
});

// POST /api/auth/request-otp  (consumer)
router.post('/request-otp', async (req, res) => {
  const phone = (req.body && req.body.phone || '').trim();
  if (phone.length < 8) return res.status(400).json({ error: 'Please enter a valid phone number.' });
  await OtpCode.update({ consumed: true }, { where: { phone, consumed: false } });
  const code = otpCode();
  await OtpCode.create({ phone, code, expiresAt: new Date(Date.now() + 5 * 60000) });
  const dev = (process.env.OTP_DEV_MODE || 'true').toLowerCase() === 'true';
  res.json(dev ? { sent: true, devMode: true, code } : { sent: true, devMode: false });
});

// POST /api/auth/verify-otp  (consumer)
router.post('/verify-otp', async (req, res) => {
  const { phone, code, fullName } = req.body || {};
  const p = (phone || '').trim();
  if (!p || !code) return res.status(400).json({ error: 'Phone and code are required.' });
  const otp = await OtpCode.findOne({ where: { phone: p, consumed: false }, order: [['createdAt', 'DESC']] });
  if (!otp || otp.expiresAt < new Date() || otp.attempts >= 5) return res.status(401).json({ error: 'Invalid or expired code.' });
  otp.attempts += 1;
  if (otp.code !== String(code).trim()) { await otp.save(); return res.status(401).json({ error: 'Invalid or expired code.' }); }
  otp.consumed = true; await otp.save();

  let u = await User.findOne({ where: { phone: p, role: 'Customer' } });
  if (!u) u = await User.create({ email: 'otp+' + p + '@kirana.local', phone: p, fullName: (fullName || '').trim(), role: 'Customer', provider: 'Otp' });
  u.provider = 'Otp'; u.sessionToken = token(); await u.save();
  res.json(loginResponse(u));
});

module.exports = router;
