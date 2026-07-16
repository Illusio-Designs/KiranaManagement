import React, { useState } from 'react';
import { View, Text, TextInput, TouchableOpacity, Alert, StyleSheet, ScrollView } from 'react-native';
import api, { setSession } from '../api';
import { C } from '../theme';

export default function LoginScreen({ navigation }) {
  const [tab, setTab] = useState('otp'); // 'otp' | 'email'
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [phone, setPhone] = useState('');
  const [code, setCode] = useState('');
  const [sent, setSent] = useState(false);
  const [dev, setDev] = useState('');

  async function emailLogin() {
    try { const { data } = await api.post('/api/auth/login', { email, password }); await setSession(data); navigation.navigate('Home'); }
    catch (e) { Alert.alert('Sign in failed', e?.response?.data?.error || 'Try again.'); }
  }
  async function requestOtp() {
    try { const { data } = await api.post('/api/auth/request-otp', { phone }); setSent(true); if (data.devMode && data.code) setDev('Dev code: ' + data.code); }
    catch (e) { Alert.alert('Could not send OTP', e?.response?.data?.error || 'Try again.'); }
  }
  async function verifyOtp() {
    try { const { data } = await api.post('/api/auth/verify-otp', { phone, code }); await setSession(data); navigation.navigate('Home'); }
    catch (e) { Alert.alert('Invalid code', e?.response?.data?.error || 'Try again.'); }
  }

  return (
    <ScrollView contentContainerStyle={{ padding: 20 }}>
      <Text style={s.h}>Sign in to Kirana</Text>
      <View style={s.tabs}>
        <TouchableOpacity onPress={() => setTab('otp')} style={[s.tab, tab === 'otp' && s.tabOn]}><Text style={tab === 'otp' ? s.tabTxtOn : s.tabTxt}>OTP</Text></TouchableOpacity>
        <TouchableOpacity onPress={() => setTab('email')} style={[s.tab, tab === 'email' && s.tabOn]}><Text style={tab === 'email' ? s.tabTxtOn : s.tabTxt}>Email</Text></TouchableOpacity>
      </View>

      {tab === 'email' ? (
        <View>
          <Text style={s.lbl}>Email</Text><TextInput style={s.input} autoCapitalize="none" value={email} onChangeText={setEmail} />
          <Text style={s.lbl}>Password</Text><TextInput style={s.input} secureTextEntry value={password} onChangeText={setPassword} />
          <TouchableOpacity style={s.btn} onPress={emailLogin}><Text style={s.btnTxt}>Sign In</Text></TouchableOpacity>
        </View>
      ) : (
        <View>
          <Text style={s.lbl}>Mobile number</Text>
          <TextInput style={s.input} keyboardType="phone-pad" value={phone} onChangeText={setPhone} placeholder="+91 98765 43210" />
          {!sent ? (
            <TouchableOpacity style={s.btnOutline} onPress={requestOtp}><Text style={s.btnOutlineTxt}>Send OTP</Text></TouchableOpacity>
          ) : (
            <View>
              <Text style={s.lbl}>Enter the 6-digit code</Text>
              <TextInput style={s.input} keyboardType="number-pad" maxLength={6} value={code} onChangeText={setCode} />
              {!!dev && <Text style={{ color: C.muted, marginBottom: 8 }}>{dev}</Text>}
              <TouchableOpacity style={s.btn} onPress={verifyOtp}><Text style={s.btnTxt}>Verify & continue</Text></TouchableOpacity>
            </View>
          )}
        </View>
      )}
    </ScrollView>
  );
}

const s = StyleSheet.create({
  h: { fontSize: 22, fontWeight: '800', textAlign: 'center', marginBottom: 16, color: C.ink },
  tabs: { flexDirection: 'row', justifyContent: 'center', marginBottom: 16, gap: 8 },
  tab: { paddingHorizontal: 18, paddingVertical: 8, borderRadius: 20 }, tabOn: { backgroundColor: C.greenSoft },
  tabTxt: { color: C.muted, fontWeight: '700' }, tabTxtOn: { color: C.green2, fontWeight: '700' },
  lbl: { fontSize: 12, fontWeight: '700', color: C.muted, marginBottom: 4, marginTop: 8 },
  input: { borderWidth: 1.5, borderColor: C.line, borderRadius: 12, paddingHorizontal: 14, paddingVertical: 10, backgroundColor: '#fff' },
  btn: { backgroundColor: C.green, borderRadius: 24, padding: 14, alignItems: 'center', marginTop: 14 }, btnTxt: { color: '#fff', fontWeight: '800' },
  btnOutline: { borderWidth: 1.5, borderColor: C.line, borderRadius: 24, padding: 13, alignItems: 'center', marginTop: 14, backgroundColor: '#fff' }, btnOutlineTxt: { color: C.ink, fontWeight: '800' },
});
