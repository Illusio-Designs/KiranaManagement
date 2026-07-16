import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createNativeStackNavigator } from '@react-navigation/native-stack';
import { C } from './src/theme';
import Home from './src/screens/HomeScreen';
import Cart from './src/screens/CartScreen';
import Checkout from './src/screens/CheckoutScreen';
import Orders from './src/screens/OrdersScreen';
import Login from './src/screens/LoginScreen';

const Stack = createNativeStackNavigator();

export default function App() {
  return (
    <NavigationContainer>
      <Stack.Navigator
        screenOptions={{
          headerStyle: { backgroundColor: C.green },
          headerTintColor: '#fff',
          headerTitleStyle: { fontWeight: '800' },
          contentStyle: { backgroundColor: C.bg },
        }}
      >
        <Stack.Screen name="Home" component={Home} options={{ title: 'Kirana' }} />
        <Stack.Screen name="Cart" component={Cart} options={{ title: 'Your Cart' }} />
        <Stack.Screen name="Checkout" component={Checkout} options={{ title: 'Checkout' }} />
        <Stack.Screen name="Orders" component={Orders} options={{ title: 'My Orders' }} />
        <Stack.Screen name="Login" component={Login} options={{ title: 'Sign in' }} />
      </Stack.Navigator>
    </NavigationContainer>
  );
}
