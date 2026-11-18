import 'dart:async';

import 'package:flutter/material.dart';

import '../api.dart';
import '../auth_store.dart';
import '../format.dart';
import '../models.dart';
import '../services/signalr_service.dart';
import '../widgets.dart';
import 'create_order_page.dart';
import 'addresses_page.dart';
import 'chat_page.dart';
import 'notifications_page.dart';
import 'order_detail_page.dart';
import 'profile_page.dart';

class OrdersPage extends StatefulWidget {
  const OrdersPage({super.key});

  @override
  State<OrdersPage> createState() => _OrdersPageState();
}

class _OrdersPageState extends State<OrdersPage> {
  List<OrderSummary> _orders = [];
  bool _loading = true;
  String? _error;
  int _unread = 0;
  StreamSubscription<Map<String, dynamic>>? _sub;
  final _search = TextEditingController();
  Timer? _searchDebounce;

  @override
  void initState() {
    super.initState();
    _load();
    SignalRService.instance.connect();
    _sub = SignalRService.instance.updates.listen(_onUpdate);
  }

  @override
  void dispose() {
    _sub?.cancel();
    _searchDebounce?.cancel();
    _search.dispose();
    super.dispose();
  }

  void _onUpdate(Map<String, dynamic> event) {
    _load();
    final code = event['trackingCode'];
    final status = event['status'];
    if (code is String && status is String && mounted) {
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text('Đơn $code: ${statusLabel(status)}')),
      );
    }
  }

  Future<void> _loadUnread() async {
    try {
      final items = await Api.instance.getNotifications(unreadOnly: true);
      if (mounted) setState(() => _unread = items.length);
    } on ApiException {
      // badge không quan trọng, lỗi thì thôi
    }
  }

  Future<void> _openNotifications() async {
    await Navigator.of(context).push(
      MaterialPageRoute(builder: (_) => const NotificationsPage()),
    );
    _load();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    _loadUnread();
    try {
      final page = await Api.instance.getOrders(search: _search.text.trim());
      if (!mounted) return;
      setState(() => _orders = page.items);
    } on ApiException catch (e) {
      if (!mounted) return;
      setState(() => _error = e.message);
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  // Tìm theo mã đơn (v0.11): gõ xong 400 ms mới gọi API.
  void _onSearch(String _) {
    _searchDebounce?.cancel();
    _searchDebounce = Timer(const Duration(milliseconds: 400), _load);
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
      final searching = _search.text.trim().isNotEmpty;
      return ListView(
        children: [
          Padding(
            padding: const EdgeInsets.all(32),
            child: Text(
              searching ? 'Không có đơn nào khớp "${_search.text.trim()}".' : 'Chưa có đơn hàng nào. Bấm "Tạo đơn" để bắt đầu.',
              textAlign: TextAlign.center,
            ),
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
          IconButton(
            onPressed: _openNotifications,
            tooltip: 'Thông báo',
            icon: Badge(
              isLabelVisible: _unread > 0,
              label: Text('$_unread'),
              child: const Icon(Icons.notifications_outlined),
            ),
          ),
          PopupMenuButton<String>(
            tooltip: 'Thêm',
            onSelected: (v) => Navigator.of(context).push(
              MaterialPageRoute(
                builder: (_) => switch (v) {
                  'chat' => const ChatPage(),
                  'profile' => const ProfilePage(),
                  _ => const AddressesPage(),
                },
              ),
            ),
            itemBuilder: (_) => const [
              PopupMenuItem(value: 'chat', child: Text('Trợ lý AI')),
              PopupMenuItem(value: 'addresses', child: Text('Sổ địa chỉ')),
              PopupMenuItem(value: 'profile', child: Text('Hồ sơ của tôi')),
            ],
          ),
          IconButton(onPressed: _logout, icon: const Icon(Icons.logout), tooltip: 'Đăng xuất'),
        ],
      ),
      floatingActionButton: FloatingActionButton.extended(
        onPressed: _create,
        icon: const Icon(Icons.add),
        label: const Text('Tạo đơn'),
      ),
      body: Column(
        children: [
          Padding(
            padding: const EdgeInsets.fromLTRB(12, 8, 12, 4),
            child: TextField(
              controller: _search,
              onChanged: _onSearch,
              textInputAction: TextInputAction.search,
              decoration: const InputDecoration(
                prefixIcon: Icon(Icons.search),
                hintText: 'Tìm theo mã đơn',
                isDense: true,
                border: OutlineInputBorder(),
              ),
            ),
          ),
          Expanded(child: RefreshIndicator(onRefresh: _load, child: _body())),
        ],
      ),
    );
  }
}
