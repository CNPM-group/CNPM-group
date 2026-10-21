import 'package:flutter/material.dart';

import 'auth_store.dart';
import 'nav.dart';
import 'pages/login_page.dart';
import 'pages/orders_page.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  runApp(const App());
}

class App extends StatelessWidget {
  const App({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'SmartDroneDelivery',
      navigatorKey: navigatorKey,
      theme: ThemeData(colorSchemeSeed: const Color(0xFF1F3864), useMaterial3: true),
      home: const Gate(),
    );
  }
}

/// Cổng vào: chưa đăng nhập thì hiện trang đăng nhập, rồi mới tới danh sách đơn.
class Gate extends StatefulWidget {
  const Gate({super.key});

  @override
  State<Gate> createState() => _GateState();
}

class _GateState extends State<Gate> {
  @override
  void initState() {
    super.initState();
    AuthStore.instance.load();
  }

  @override
  Widget build(BuildContext context) {
    return ListenableBuilder(
      listenable: AuthStore.instance,
      builder: (context, _) {
        final store = AuthStore.instance;
        if (!store.loaded) {
          return const Scaffold(body: Center(child: CircularProgressIndicator()));
        }
        return store.isLoggedIn ? const OrdersPage() : const LoginPage();
      },
    );
  }
}
