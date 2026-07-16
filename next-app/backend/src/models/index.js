const { DataTypes } = require('sequelize');
const { sequelize } = require('../db');

// ---------------- Models ----------------

const Store = sequelize.define('Store', {
  id: { type: DataTypes.UUID, defaultValue: DataTypes.UUIDV4, primaryKey: true },
  name: { type: DataTypes.STRING, allowNull: false },
  ownerName: DataTypes.STRING,
  email: { type: DataTypes.STRING, allowNull: false },
  phone: DataTypes.STRING,
  addressLine: DataTypes.STRING,
  city: DataTypes.STRING,
  gstin: DataTypes.STRING,
  pan: DataTypes.STRING,
  status: { type: DataTypes.ENUM('Pending', 'Active', 'Suspended', 'Rejected', 'Closed'), defaultValue: 'Pending' },
  latitude: DataTypes.DOUBLE,
  longitude: DataTypes.DOUBLE,
  approvedAt: DataTypes.DATE,
  rejectionReason: DataTypes.STRING
});

const User = sequelize.define('User', {
  id: { type: DataTypes.UUID, defaultValue: DataTypes.UUIDV4, primaryKey: true },
  email: { type: DataTypes.STRING, allowNull: false },
  passwordHash: DataTypes.STRING,           // null for Google / OTP accounts
  fullName: DataTypes.STRING,
  phone: DataTypes.STRING,
  provider: { type: DataTypes.ENUM('Password', 'Google', 'Otp'), defaultValue: 'Password' },
  role: { type: DataTypes.ENUM('SuperAdmin', 'Owner', 'Manager', 'Customer'), defaultValue: 'Customer' },
  storeId: DataTypes.UUID,
  isActive: { type: DataTypes.BOOLEAN, defaultValue: true },
  sessionToken: DataTypes.STRING
});

const Product = sequelize.define('Product', {
  id: { type: DataTypes.UUID, defaultValue: DataTypes.UUIDV4, primaryKey: true },
  storeId: { type: DataTypes.UUID, allowNull: false },
  name: { type: DataTypes.STRING, allowNull: false },
  description: DataTypes.STRING,
  brand: DataTypes.STRING,
  category: DataTypes.STRING,
  imageUrl: { type: DataTypes.STRING(1000) },
  isActive: { type: DataTypes.BOOLEAN, defaultValue: true },
  status: { type: DataTypes.ENUM('Pending', 'Approved', 'Rejected'), defaultValue: 'Pending' },
  rejectionReason: DataTypes.STRING,
  approvedAt: DataTypes.DATE
});

const ProductVariant = sequelize.define('ProductVariant', {
  id: { type: DataTypes.UUID, defaultValue: DataTypes.UUIDV4, primaryKey: true },
  productId: { type: DataTypes.UUID, allowNull: false },
  storeId: { type: DataTypes.UUID, allowNull: false },
  name: DataTypes.STRING,
  sku: DataTypes.STRING,
  barcode: DataTypes.STRING,
  unit: { type: DataTypes.STRING, defaultValue: 'Piece' },
  packSize: { type: DataTypes.DECIMAL(12, 3), defaultValue: 1 },
  mrp: { type: DataTypes.DECIMAL(18, 2), defaultValue: 0 },
  sellingPrice: { type: DataTypes.DECIMAL(18, 2), defaultValue: 0 },
  taxRatePercent: { type: DataTypes.DECIMAL(5, 2), defaultValue: 0 },
  stockQuantity: { type: DataTypes.INTEGER, defaultValue: 0 },
  reorderLevel: { type: DataTypes.INTEGER, defaultValue: 0 },
  isActive: { type: DataTypes.BOOLEAN, defaultValue: true }
});

const CatalogImage = sequelize.define('CatalogImage', {
  id: { type: DataTypes.UUID, defaultValue: DataTypes.UUIDV4, primaryKey: true },
  nameKey: { type: DataTypes.STRING, allowNull: false, unique: true },
  name: DataTypes.STRING,
  imageName: DataTypes.STRING,
  imageUrl: { type: DataTypes.STRING(1000), allowNull: false }
});

const Order = sequelize.define('Order', {
  id: { type: DataTypes.UUID, defaultValue: DataTypes.UUIDV4, primaryKey: true },
  customerId: DataTypes.UUID,
  orderNumber: DataTypes.STRING,
  customerName: DataTypes.STRING,
  phone: DataTypes.STRING,
  address: DataTypes.STRING(1000),
  city: DataTypes.STRING,
  pincode: DataTypes.STRING,
  latitude: DataTypes.DOUBLE,
  longitude: DataTypes.DOUBLE,
  distanceKm: DataTypes.DOUBLE,
  etaMinutes: DataTypes.INTEGER,
  status: { type: DataTypes.ENUM('Placed', 'Accepted', 'Packed', 'OutForDelivery', 'Delivered', 'Cancelled'), defaultValue: 'Placed' },
  subtotal: { type: DataTypes.DECIMAL(18, 2), defaultValue: 0 },
  deliveryFee: { type: DataTypes.DECIMAL(18, 2), defaultValue: 0 },
  grandTotal: { type: DataTypes.DECIMAL(18, 2), defaultValue: 0 }
});

const OrderLine = sequelize.define('OrderLine', {
  id: { type: DataTypes.UUID, defaultValue: DataTypes.UUIDV4, primaryKey: true },
  orderId: { type: DataTypes.UUID, allowNull: false },
  storeId: DataTypes.UUID,
  storeName: DataTypes.STRING,
  productVariantId: DataTypes.UUID,
  productName: DataTypes.STRING,
  variantName: DataTypes.STRING,
  quantity: DataTypes.INTEGER,
  unitPrice: { type: DataTypes.DECIMAL(18, 2), defaultValue: 0 },
  lineTotal: { type: DataTypes.DECIMAL(18, 2), defaultValue: 0 }
});

const OtpCode = sequelize.define('OtpCode', {
  id: { type: DataTypes.UUID, defaultValue: DataTypes.UUIDV4, primaryKey: true },
  phone: { type: DataTypes.STRING, allowNull: false },
  code: { type: DataTypes.STRING, allowNull: false },
  expiresAt: DataTypes.DATE,
  consumed: { type: DataTypes.BOOLEAN, defaultValue: false },
  attempts: { type: DataTypes.INTEGER, defaultValue: 0 }
});

// ---------------- Associations ----------------
Product.hasMany(ProductVariant, { as: 'variants', foreignKey: 'productId' });
ProductVariant.belongsTo(Product, { foreignKey: 'productId' });
Order.hasMany(OrderLine, { as: 'lines', foreignKey: 'orderId' });
OrderLine.belongsTo(Order, { foreignKey: 'orderId' });

module.exports = {
  sequelize,
  Store, User, Product, ProductVariant, CatalogImage, Order, OrderLine, OtpCode
};
