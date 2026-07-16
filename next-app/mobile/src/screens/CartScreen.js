import React, { useState, useCallback } from 'react';
import { View, Text, TouchableOpacity, FlatList, StyleSheet } from 'react-native';
import { useFocusEffect } from '@react-navigation/native';
import { cart, money } from '../api';
import { C } from '../theme';

export default function CartScreen({ navigation }) {
  const [items, setItems] = useState([]);
  const load = useCallback(async () => setItems(await cart.get()), []);
  useFocusEffect(useCallback(() => { load(); }, [load]));

  async function chg(id, qty) { await cart.setQty(id, qty); load(); }
  const sub = items.reduce((s, x) => s + x.price * x.qty, 0);
  const del = sub >= 500 ? 0 : 40;

  if (!items.length)
    return <View style={s.center}><Text style={{ color: C.muted }}>Your cart is empty.</Text>
      <TouchableOpacity style={s.btn} onPress={() => navigation.navigate('Home')}><Text style={s.btnTxt}>Shop now</Text></TouchableOpacity></View>;

  return (
    <View style={{ flex: 1 }}>
      <FlatList data={items} keyExtractor={(x) => x.variantId} contentContainerStyle={{ padding: 12 }}
        renderItem={({ item: x }) => (
          <View style={s.row}>
            <View style={{ flex: 1 }}>
              <Text style={s.name}>{x.productName}</Text>
              <Text style={s.muted}>{x.variantName} · {x.storeName}</Text>
            </View>
            <View style={s.qty}>
              <TouchableOpacity onPress={() => chg(x.variantId, x.qty - 1)}><Text style={s.qtyBtn}>−</Text></TouchableOpacity>
              <Text style={s.qtyNum}>{x.qty}</Text>
              <TouchableOpacity onPress={() => chg(x.variantId, x.qty + 1)}><Text style={s.qtyBtn}>+</Text></TouchableOpacity>
            </View>
            <Text style={s.total}>{money(x.price * x.qty)}</Text>
          </View>
        )} />
      <View style={s.summary}>
        <View style={s.sumRow}><Text>Subtotal</Text><Text style={{ fontWeight: '800' }}>{money(sub)}</Text></View>
        <View style={s.sumRow}><Text>Delivery</Text><Text style={{ fontWeight: '800' }}>{del === 0 ? 'FREE' : money(del)}</Text></View>
        <View style={[s.sumRow, { borderTopWidth: 1, borderColor: C.line, paddingTop: 10 }]}>
          <Text style={{ fontWeight: '900', fontSize: 16 }}>Total</Text><Text style={{ fontWeight: '900', fontSize: 16 }}>{money(sub + del)}</Text></View>
        <TouchableOpacity style={s.btn} onPress={() => navigation.navigate('Checkout')}><Text style={s.btnTxt}>Checkout →</Text></TouchableOpacity>
      </View>
    </View>
  );
}

const s = StyleSheet.create({
  center: { flex: 1, alignItems: 'center', justifyContent: 'center', gap: 14 },
  row: { flexDirection: 'row', alignItems: 'center', backgroundColor: '#fff', borderRadius: 14, padding: 12, marginBottom: 10, borderWidth: 1, borderColor: C.line },
  name: { fontWeight: '800', color: C.ink }, muted: { color: C.muted, fontSize: 12 },
  qty: { flexDirection: 'row', alignItems: 'center', borderWidth: 1, borderColor: C.line, borderRadius: 20, marginHorizontal: 8 },
  qtyBtn: { paddingHorizontal: 12, fontSize: 18, color: C.green2, fontWeight: '800' }, qtyNum: { fontWeight: '800', minWidth: 20, textAlign: 'center' },
  total: { fontWeight: '900', width: 70, textAlign: 'right' },
  summary: { backgroundColor: '#fff', padding: 16, borderTopWidth: 1, borderColor: C.line },
  sumRow: { flexDirection: 'row', justifyContent: 'space-between', paddingVertical: 6 },
  btn: { backgroundColor: C.green, borderRadius: 24, padding: 14, alignItems: 'center', marginTop: 12 }, btnTxt: { color: '#fff', fontWeight: '800', fontSize: 15 },
});
