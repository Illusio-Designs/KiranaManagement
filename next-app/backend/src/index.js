require('dotenv').config();
const express = require('express');
const cors = require('cors');
const path = require('path');
const { sequelize } = require('./models');
const { loadUser } = require('./middleware/auth');
const { seed } = require('./seed');

const app = express();
app.use(cors());
app.use(express.json());
app.use('/uploads', express.static(path.join(__dirname, '..', 'uploads')));
app.use(loadUser);

app.get('/', (req, res) => res.json({ name: 'Kirana API (Node + MySQL)', ok: true }));
app.use('/api/auth', require('./routes/auth'));
app.use('/api/stores', require('./routes/stores'));
app.use('/api/products', require('./routes/products'));
app.use('/api/marketplace', require('./routes/marketplace'));
app.use('/api/orders', require('./routes/orders'));
app.use('/api/store/orders', require('./routes/storeorders'));
app.use('/api/catalog', require('./routes/catalog'));
app.use('/api/uploads', require('./routes/uploads'));

app.use((err, req, res, next) => {
  console.error(err);
  res.status(500).json({ error: err.message || 'Server error.' });
});

const PORT = process.env.PORT || 4000;
(async () => {
  await sequelize.authenticate();
  await sequelize.sync({ alter: true });
  await seed();
  app.listen(PORT, () => console.log('Kirana API on http://localhost:' + PORT));
})().catch(e => { console.error('Startup failed:', e.message); process.exit(1); });
