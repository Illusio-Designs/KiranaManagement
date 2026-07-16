const bcrypt = require('bcryptjs');
const { sequelize, Store, User, Product, ProductVariant, CatalogImage } = require('./models');
const { catalogKey, deriveImageName } = require('./util');

async function addImage(name, url) {
  const key = catalogKey(name);
  if (!(await CatalogImage.findOne({ where: { nameKey: key } })))
    await CatalogImage.create({ nameKey: key, name, imageName: deriveImageName(name), imageUrl: url });
}

async function makeProduct(storeId, name, brand, category, variant) {
  const p = await Product.create({ storeId, name, brand, category, status: 'Approved', approvedAt: new Date() });
  await ProductVariant.create({ productId: p.id, storeId, ...variant, taxRatePercent: 5, reorderLevel: 10 });
}

// Idempotent seed: only inserts when rows are missing.
async function seed() {
  if (!(await User.findOne({ where: { role: 'SuperAdmin' } })))
    await User.create({ email: 'superadmin@kirana.local', fullName: 'Platform Super Admin', role: 'SuperAdmin', passwordHash: bcrypt.hashSync('Admin@12345', 10) });

  if (!(await Store.findOne())) {
    const store = await Store.create({ name: 'Demo Kirana Store', ownerName: 'Demo Owner', email: 'demo@store.local', phone: '+919999999999', city: 'Ahmedabad', gstin: '24ABCDE1234F1Z5', pan: 'ABCDE1234F', status: 'Active', approvedAt: new Date(), latitude: 23.0225, longitude: 72.5714 });
    await User.create({ email: 'demo@store.local', fullName: 'Demo Owner', role: 'Owner', storeId: store.id, passwordHash: bcrypt.hashSync('Demo@12345', 10) });

    await makeProduct(store.id, 'Aashirvaad Atta', 'Aashirvaad', 'Pulses & Grains', { name: '5 kg', sku: 'ATTA-5KG', unit: 'Kilogram', packSize: 5, mrp: 280, sellingPrice: 260, stockQuantity: 40 });
    await makeProduct(store.id, 'Fresh Apples', 'Farm', 'Fruits', { name: '1 kg', sku: 'APPLE-1KG', unit: 'Kilogram', packSize: 1, mrp: 159, sellingPrice: 119, stockQuantity: 80 });
    await makeProduct(store.id, 'Amul Milk', 'Amul', 'Dairy & Eggs', { name: '1 L', sku: 'MILK-1L', unit: 'Litre', packSize: 1, mrp: 62, sellingPrice: 54, stockQuantity: 60 });
    await makeProduct(store.id, 'Tata Salt', 'Tata', 'Cooking Essentials', { name: '1 kg', sku: 'SALT-1KG', unit: 'Kilogram', packSize: 1, mrp: 28, sellingPrice: 25, stockQuantity: 120 });
    await makeProduct(store.id, "Lay's Chips", "Lay's", 'Snacks', { name: '52 g', sku: 'LAYS-52G', unit: 'Gram', packSize: 52, mrp: 20, sellingPrice: 18, stockQuantity: 200 });
    await makeProduct(store.id, 'Fresh Tomatoes', 'Farm', 'Vegetables', { name: '1 kg', sku: 'TOM-1KG', unit: 'Kilogram', packSize: 1, mrp: 59, sellingPrice: 39, stockQuantity: 90 });
  }

  await addImage('Fresh Apples', 'https://placehold.co/300x300/ffe6ef/ff4d8d?text=Apples');
  await addImage('Amul Milk', 'https://placehold.co/300x300/e3f2ff/2ea7ff?text=Milk');
  await addImage('Tata Salt', 'https://placehold.co/300x300/eef1ee/6b7a6e?text=Salt');
  await addImage("Lay's Chips", 'https://placehold.co/300x300/fff3d6/b8860b?text=Chips');
  await addImage('Fresh Tomatoes', 'https://placehold.co/300x300/fdecec/e5484d?text=Tomatoes');
  await addImage('Aashirvaad Atta', 'https://placehold.co/300x300/e9f8ec/1f9d38?text=Atta');
}

module.exports = { seed };

// Allow "npm run seed" to run standalone.
if (require.main === module) {
  (async () => {
    await sequelize.authenticate();
    await sequelize.sync({ alter: true });
    await seed();
    console.log('Seed complete.');
    process.exit(0);
  })().catch(e => { console.error(e); process.exit(1); });
}
