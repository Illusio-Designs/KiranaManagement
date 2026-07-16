const { User } = require('../models');

// Resolves the logged-in user from the X-Auth-Token header and attaches
// req.user. Never blocks — routes decide what roles they require.
async function loadUser(req, res, next) {
  const token = req.headers['x-auth-token'];
  req.user = null;
  if (token) {
    try {
      const u = await User.findOne({ where: { sessionToken: token, isActive: true } });
      if (u) req.user = u;
    } catch (e) { /* ignore */ }
  }
  next();
}

function requireRole(...roles) {
  return (req, res, next) => {
    if (!req.user || !roles.includes(req.user.role))
      return res.status(401).json({ error: 'Login required.' });
    next();
  };
}

module.exports = { loadUser, requireRole };
