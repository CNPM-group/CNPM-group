import 'package:flutter/material.dart';

import '../api.dart';
import '../auth_store.dart';
import '../format.dart';
import '../models.dart';
import '../widgets.dart';
import 'create_order_page.dart';
import 'order_detail_page.dart';

class OrdersPage extends StatefulWidget {
  const OrdersPage({super.key});

  @override
  State<OrdersPage> createState() => _OrdersPageState();
}

class _OrdersPageState extends State<OrdersPage> {
  List<OrderSummary> _orders = [];
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final page = await Api.instance.getOrders();
      if (!mounted) return;
      setState(() => _orders = page.items);
    } on ApiException catch (e) {
      if (!mounted) return;
      setState(() => _error = e.message);
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  Future<void> _create() async {
    final created = await Navigator.of(context).push<bool>(
      MaterialPageRoute(builder: (_) => const CreateOrderPage()),
    );
    if (created == true) _load();
  }

  Future<void> _open(OrderSummary order) async {
    await Navigator.of(context).push(
      MaterialPageRoute(builder: (_) => OrderDetailPage(orderId: order.id)),
    );
    _load();
  }

  Future<void> _logout() async {
    final ok = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Đăng xuất?'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Hủy')),
          FilledButton(onPressed: () => Navigator.pop(ctx, true), child: const Text('Đăng xuất')),
        ],
      ),
    );
    if (ok == true) await AuthStore.instance.logout();
  }

  Widget _body() {
    if (_loading && _orders.isEmpty) {
      return const Center(child: CircularProgressIndicator());
    }
    if (_error != null) {
      return ListView(
        children: [
          Padding(
            padding: const EdgeInsets.all(24),
            child: Text(_error!, style: TextStyle(color: Theme.of(context).colorScheme.error)),
          ),
          TextButton(onPressed: _load, child: const Text('Thử lại')),
        ],
      );
    }
    if (_orders.isEmpty) {
      return ListView(
        children: const [
          Padding(
            padding: EdgeInsets.all(32),
            child: Text('Chưa có đơn hàng nào. Bấm "Tạo đơn" để bắt đầu.', textAlign: TextAlign.center),
          ),
        ],
      );
    }
    return ListView.separated(
      padding: const EdgeInsets.only(bottom: 88),
      itemCount: _orders.length,
      separatorBuilder: (_, __) => const Divider(height: 1),
      itemBuilder: (context, i) {
        final o = _orders[i];
        return ListTile(
          title: Text(o.trackingCode, style: const TextStyle(fontWeight: FontWeight.w600)),
          subtitle: Text('${o.originStationName} → ${o.destinationStationName}\n${formatDate(o.createdAtUtc)}'),
          isThreeLine: true,
          trailing: StatusChip(o.status),
          onTap: () => _open(o),
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('Đơn của tôi'),
        actions: [
          IconButton(onPressed: _logout, icon: const Icon(Icons.logout), tooltip: 'Đăng xuất'),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: _create,
        icon: const Icon(Icons.add),
        label: const Text('Tạo đơn'),
      ),
      body: RefreshIndicator(onRefresh: _load, child: _body()),
    );
  }
}
