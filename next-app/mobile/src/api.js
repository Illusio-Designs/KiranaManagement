import axios from 'axios';
import AsyncStorage from '@react-native-async-storage/async-storage';

// Point this at your running Node backend.
// Android emulator uses 10.0.2.2 to reach the host machine; iOS/web use localhost.
import { Platform } from 'react-native';
export const BASE_URL =
  Platform.OS === 'android' ? 'http://10.0.2.2:4000' : 'http://localhost:4000';

const api = axios.create({ baseURL: BASE_URL });

api.interceptors.request.use(async (config) => {
  const token = await AsyncStorage.getItem('kirana_token');
  if (token) config.headers['X-Auth-Token'] = token;
  return config;
});

export async function setSession(d) {
  await AsyncStorage.multiSet([
    ['kirana_token', d.token || ''],
    ['kirana_role', d.role || ''],
    ['kirana_name', d.fullName || ''],
  ]);
}
export async function getRole() { return AsyncStorage.getItem('kirana_role'); }
export async function getName() { return AsyncStorage.getItem('kirana_name'); }
export async function logout() {
  await AsyncStorage.multiRemove(['kirana_token', 'kirana_role', 'kirana_name']);
}

// ---- cart (AsyncStorage) ----
export const cart = {
  async get() { try { return JSON.parse(await AsyncStorage.getItem('kirana_cart')) || []; } catch { return []; } },
  async save(c) { await AsyncStorage.setItem('kirana_cart', JSON.stringify(c)); },
  async add(item) {
    const c = await cart.get();
    const f = c.find((x) => x.variantId === item.variantId);
    if (f) f.qty += item.qty; else c.push(item);
    await cart.save(c);
  },
  async setQty(id, qty) {
    let c = await cart.get();
    c.forEach((x) => { if (x.variantId === id) x.qty = qty; });
    await cart.save(c.filter((x) => x.qty > 0));
  },
  async clear() { await cart.save([]); },
  async subtotal() { return (await cart.get()).reduce((s, x) => s + x.price * x.qty, 0); },
};

export const money = (n) => '₹' + (Number(n) || 0).toFixed(2);
export default api;
