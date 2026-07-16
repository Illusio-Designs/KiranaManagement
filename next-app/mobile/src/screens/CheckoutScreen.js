import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, ScrollView, Alert, StyleSheet } from 'react-native';
import AsyncStorage from '@react-native-async-storage/async-storage';
import api, { cart, money } from '../api';
import { C } from '../theme';

export default function CheckoutScreen({ navigation }) {
  const [f, setF] = useState({ customerName: '', phone: '', address: '', city: '', pincode: '' });
  const [busy, setBusy] = useState(false);
  const set = (k) => (v) => setF((p) => ({ ...p, [k]: v }));

  async function place() {
    const token = await AsyncStorage.getItem('kirana_token');
    const role = await AsyncStorage.getItem('kirana_role');
    if (!token || role !== 'Customer') { navigation.navigate('Login'); return; }
    if (!f.customerName || !f.phone || !f.address) { Alert.alert('Please fill name, phone and address.'); return; }
    const lines = (await cart.get()).map((x) => ({ productVariantId: x.variantId, quantity: x.qty }));
    if (!lines.length) { navigation.navigate('Home'); return; }
    setBusy(true);
    try {
      const { data } = await api.post('/api/orders', { ...f, lines });
      await cart.clear();
      Alert.alert('Order placed!', `${data.orderNumber} · ETA ${data.etaMinutes || '—'} min`, [
        { text: 'View orders', onPress: () => navigation.navigate('Orders') },
      ]);
    } catch (e) {
      Alert.alert('Could not place order', e?.response?.data?.error || 'Try again.');
    } finally { setBusy(false); }
  }

  const Field = ({ label, k, ...rest }) => (
    <View style={{ marginBottom: 10 }}>
      <Text style={s.lbl}>{label}</Text>
      <TextInput style={s.input} value={f[k]} onChangeText={set(k)} {...rest} />
    </View>
  );

  return (
    <ScrollView contentContainerStyle={{ padding: 16 }}>
      <Text style={s.h}>Delivery details</Text>
      <Field label="Full name" k="customerName" />
      <Field label="Phone" k="phone" keyboardType="phone-pad" />
      <Field label="Address" k="address" />
      <View style={{ flexDirection: 'row', gap: 10 }}>
        <View style={{ flex: 1 }}><Field label="City" k="city" /></View>
        <View style={{ flex: 1 }}><Field label="Pincode" k="pincode" keyboardType="number-pad" /></View>
      </View>
      <TouchableOpacity style={[s.btn, busy && { opacity: 0.6 }]} disabled={busy} onPress={place}>
        <Text style={s.btnTxt}>{busy ? 'Placing…' : 'Place Order'}</Text>
      </TouchableOpacity>
    </ScrollView>
  );
}

const s = StyleSheet.create({
  h: { fontSize: 18, fontWeight: '800', marginBottom: 12, color: C.ink },
  lbl: { fontSize: 12, fontWeight: '700', color: C.muted, marginBottom: 4 },
  input: { borderWidth: 1.5, borderColor: C.line, borderRadius: 12, paddingHorizontal: 14, paddingVertical: 10, backgroundColor: '#fff' },
  btn: { backgroundColor: C.green, borderRadius: 24, padding: 15, alignItems: 'center', marginTop: 12 }, btnTxt: { color: '#fff', fontWeight: '800', fontSize: 15 },
});
