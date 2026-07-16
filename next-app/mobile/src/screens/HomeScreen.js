import React, { useEffect, useState, useCallback } from 'react';
import { View, Text, TextInput, FlatList, TouchableOpacity, Image, StyleSheet } from 'react-native';
import { useFocusEffect } from '@react-navigation/native';
import api, { cart, money } from '../api';
import { C } from '../theme';

export default function HomeScreen({ navigation }) {
  const [products, setProducts] = useState([]);
  const [cats, setCats] = useState([]);
  const [active, setActive] = useState('');
  const [q, setQ] = useState('');
  const [count, setCount] = useState(0);

  async function loadCats() { try { const { data } = await api.get('/api/marketplace/categories'); setCats(data); } catch {} }
  async function load() {
    try {
      let url = '/api/marketplace/products?sort=popular';
      if (q) url += '&q=' + encodeURIComponent(q);
      if (active) url += '&category=' + encodeURIComponent(active);
      const { data } = await api.get(url);
      setProducts(data);
    } catch { setProducts([]); }
  }
  async function refreshCount() { setCount((await cart.get()).reduce((s, x) => s + x.qty, 0)); }

  useEffect(() => { loadCats(); }, []);
  useEffect(() => { load(); }, [active]);
  useFocusEffect(useCallback(() => { refreshCount(); }, []));

  useEffect(() => {
    navigation.setOptions({
      headerRight: () => (
        <View style={{ flexDirection: 'row' }}>
          <TouchableOpacity onPress={() => navigation.navigate('Orders')} style={{ marginRight: 16 }}>
            <Text style={{ color: '#fff', fontWeight: '700' }}>Orders</Text></TouchableOpacity>
          <TouchableOpacity onPress={() => navigation.navigate('Cart')}>
            <Text style={{ color: '#fff', fontWeight: '800' }}>Cart ({count})</Text></TouchableOpacity>
        </View>
      ),
    });
  }, [count, navigation]);

  async function add(p, v) {
    await cart.add({ variantId: v.id, storeId: p.storeId, storeName: p.storeName, productName: p.name, variantName: v.name, mrp: v.mrp, price: v.sellingPrice, qty: 1 });
    refreshCount();
  }

  const renderProduct = ({ item: p }) => {
    const v = p.variants[0]; if (!v) return null;
    return (
      <View style={s.card}>
        <View style={s.thumb}>
          {p.imageUrl ? <Image source={{ uri: p.imageUrl }} style={{ width: 64, height: 64, borderRadius: 12 }} /> : <Text style={{ fontSize: 30 }}>🧺</Text>}
        </View>
        <View style={{ flex: 1 }}>
          <Text style={s.name}>{p.name}</Text>
          <Text style={s.muted}>{v.name} · {p.storeName}</Text>
          <View style={{ flexDirection: 'row', alignItems: 'center', marginTop: 4 }}>
            <Text style={s.price}>{money(v.sellingPrice)}</Text>
            {v.mrp > v.sellingPrice && <Text style={s.mrp}>{money(v.mrp)}</Text>}
            {v.discountPercent > 0 && <Text style={s.off}>  {v.discountPercent}% OFF</Text>}
          </View>
        </View>
        <TouchableOpacity style={s.addBtn} onPress={() => add(p, v)}><Text style={s.addTxt}>Add</Text></TouchableOpacity>
      </View>
    );
  };

  return (
    <View style={{ flex: 1 }}>
      <View style={s.searchWrap}>
        <TextInput style={s.search} placeholder="Search for products..." value={q}
          onChangeText={setQ} onSubmitEditing={load} returnKeyType="search" />
      </View>
      <FlatList horizontal showsHorizontalScrollIndicator={false} style={s.chips} data={[{ name: '', count: 0 }, ...cats]}
        keyExtractor={(c) => c.name || 'all'}
        renderItem={({ item: c }) => (
          <TouchableOpacity onPress={() => setActive(c.name)} style={[s.chip, active === c.name && s.chipOn]}>
            <Text style={[s.chipTxt, active === c.name && s.chipTxtOn]}>{c.name ? `${c.name} (${c.count})` : 'All'}</Text>
          </TouchableOpacity>
        )} />
      <FlatList data={products} keyExtractor={(p) => p.id} renderItem={renderProduct}
        contentContainerStyle={{ padding: 12 }}
        ListEmptyComponent={<Text style={[s.muted, { textAlign: 'center', marginTop: 40 }]}>No products found.</Text>} />
    </View>
  );
}

const s = StyleSheet.create({
  searchWrap: { padding: 12, backgroundColor: '#fff', borderBottomWidth: 1, borderColor: C.line },
  search: { backgroundColor: C.bg, borderRadius: 24, paddingHorizontal: 16, paddingVertical: 10, fontSize: 15 },
  chips: { maxHeight: 52, backgroundColor: '#fff', paddingHorizontal: 8, borderBottomWidth: 1, borderColor: C.line },
  chip: { paddingHorizontal: 14, paddingVertical: 8, marginHorizontal: 4, borderRadius: 20, height: 36, justifyContent: 'center' },
  chipOn: { backgroundColor: C.greenSoft },
  chipTxt: { color: C.muted, fontWeight: '700' }, chipTxtOn: { color: C.green2 },
  card: { flexDirection: 'row', alignItems: 'center', backgroundColor: '#fff', borderRadius: 16, padding: 12, marginBottom: 10, borderWidth: 1, borderColor: C.line },
  thumb: { width: 64, height: 64, borderRadius: 14, backgroundColor: C.greenSoft, alignItems: 'center', justifyContent: 'center', marginRight: 12 },
  name: { fontWeight: '800', fontSize: 15, color: C.ink }, muted: { color: C.muted, fontSize: 12 },
  price: { fontWeight: '900', fontSize: 16, color: C.green3 }, mrp: { color: C.muted, textDecorationLine: 'line-through', marginLeft: 6 },
  off: { color: C.green2, fontWeight: '800', fontSize: 12 },
  addBtn: { backgroundColor: C.green, borderRadius: 12, paddingHorizontal: 16, paddingVertical: 10 }, addTxt: { color: '#fff', fontWeight: '800' },
});
