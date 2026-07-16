import React, { useState, useCallback } from 'react';
import { View, Text, FlatList, StyleSheet, TouchableOpacity } from 'react-native';
import { useFocusEffect } from '@react-navigation/native';
import AsyncStorage from '@react-native-async-storage/async-storage';
import api, { money } from '../api';
import { C } from '../theme';

const pillColor = { Delivered: C.green2, OutForDelivery: C.amber, Cancelled: C.danger };

export default function OrdersScreen({ navigation }) {
  const [orders, setOrders] = useState([]);
  const [msg, setMsg] = useState('');

  const load = useCallback(async () => {
    const role = await AsyncStorage.getItem('kirana_role');
    if (role !== 'Customer') { setMsg('Please sign in to see your orders.'); setOrders([]); return; }
    try { const { data } = await api.get('/api/orders/mine'); setOrders(data); setMsg(data.length ? '' : 'No orders yet.'); }
    catch { setMsg('Please sign in to see your orders.'); }
  }, []);
  useFocusEffect(useCallback(() => { load(); }, [load]));

  if (msg)
    return <View style={s.center}><Text style={{ color: C.muted }}>{msg}</Text>
      <TouchableOpacity style={s.btn} onPress={() => navigation.navigate('Login')}><Text style={s.btnTxt}>Sign in</Text></TouchableOpacity></View>;

  return (
    <FlatList data={orders} keyExtractor={(o) => o.id} contentContainerStyle={{ padding: 12 }}
      renderItem={({ item: o }) => (
        <View style={s.card}>
          <View style={s.rowBetween}>
            <Text style={{ fontWeight: '900' }}>{o.orderNumber}</Text>
            <Text style={{ fontWeight: '800', color: pillColor[o.status] || C.muted }}>{o.status}</Text>
          </View>
          <Text style={s.muted}>{new Date(o.createdAt).toLocaleString()}</Text>
          {o.lines.map((l, i) => <Text key={i} style={s.line}>{l.productName} · {l.variantName} × {l.quantity}</Text>)}
          <View style={[s.rowBetween, { marginTop: 6 }]}>
            <Text style={s.muted}>{o.etaMinutes ? `ETA ${o.etaMinutes} min` : ''}</Text>
            <Text style={{ fontWeight: '900' }}>{money(o.grandTotal)}</Text>
          </View>
        </View>
      )} />
  );
}

const s = StyleSheet.create({
  center: { flex: 1, alignItems: 'center', justifyContent: 'center', gap: 14 },
  card: { backgroundColor: '#fff', borderRadius: 14, padding: 14, marginBottom: 10, borderWidth: 1, borderColor: C.line },
  rowBetween: { flexDirection: 'row', justifyContent: 'space-between', alignItems: 'center' },
  muted: { color: C.muted, fontSize: 12 }, line: { marginTop: 4, color: C.ink },
  btn: { backgroundColor: C.green, borderRadius: 24, paddingHorizontal: 24, padding: 12 }, btnTxt: { color: '#fff', fontWeight: '800' },
});
