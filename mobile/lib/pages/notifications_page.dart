import 'dart:async';

import 'package:flutter/material.dart';

import '../api.dart';
import '../format.dart';
import '../models.dart';
import '../services/signalr_service.dart';
import 'order_detail_page.dart';

class NotificationsPage extends StatefulWidget {
  const NotificationsPage({super.key});

  @override
  State<NotificationsPage> createState() => _NotificationsPageState();
}

class _NotificationsPageState extends State<NotificationsPage> {
  List<AppNotification> _items = [];
  bool _loading = true;
  String? _error;
  StreamSubscription<Map<String, dynamic>>? _sub;

  @override
  void initState() {
    super.initState();
    _load();
    _sub = SignalRService.instance.updates.listen((_) => _load());
  }

  @override
  void dispose() {
    _sub?.cancel();
    super.dispose();
  }

  Future<void> _load() async {
    try {
      final items = await Api.instance.getNotifications();
      if (!mounted) return;
      setState(() {
        _items = items;
        _error = null;
      });
    } on ApiException catch (e) {
      if (!mounted) return;
      setState(() => _error = e.message);
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  Future<void> _open(AppNotification n) async {
    if (!n.isRead) {
      try {
        await Api.instance.markRead(n.id);
      } on ApiException {
        // không đánh dấu được thì vẫn cho mở đơn
      }
    }
    if (!mounted) return;
    if (n.orderId != null) {
      await Navigator.of(context).push(
        MaterialPageRoute(builder: (_) => OrderDetailPage(orderId: n.orderId!)),
      );
    }
    _load();
  }

  Widget _otpBox(String otp) {
    final scheme = Theme.of(context).colorScheme;
    return Container(
      margin: const EdgeInsets.only(top: 8),
      padding: const EdgeInsets.symmetric(vertical: 10),
      width: double.infinity,
      decoration: BoxDecoration(
        color: scheme.primaryContainer,
        borderRadius: BorderRadius.circular(10),
      ),
      child: Column(
        children: [
          const Text('Mã nhận hàng – đọc cho nhân viên trạm'),
          const SizedBox(height: 4),
          SelectableText(
            otp,
            style: TextStyle(
              fontSize: 34,
              fontWeight: FontWeight.bold,
              letterSpacing: 8,
              color: scheme.onPrimaryContainer,
            ),
          ),
        ],
      ),
    );
  }

  Widget _tile(AppNotification n) {
    final otp = n.otp;
    return InkWell(
      onTap: () => _open(n),
      child: Container(
        color: n.isRead ? null : Theme.of(context).colorScheme.primary.withOpacity(0.06),
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                if (!n.isRead)
                  const Padding(
                    padding: EdgeInsets.only(right: 8),
                    child: Icon(Icons.circle, size: 10, color: Colors.blue),
                  ),
                Expanded(
                  child: Text(
                    n.title,
                    style: TextStyle(fontWeight: n.isRead ? FontWeight.normal : FontWeight.bold),
                  ),
                ),
                Text(formatDate(n.createdAtUtc), style: const TextStyle(fontSize: 12, color: Colors.grey)),
              ],
            ),
            if (n.body != null) ...[
              const SizedBox(height: 4),
              Text(n.body!),
            ],
            if (otp != null) _otpBox(otp),
          ],
        ),
      ),
    );
  }

  Widget _body() {
    if (_loading && _items.isEmpty) {
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
    if (_items.isEmpty) {
      return ListView(
        children: const [
          Padding(
            padding: EdgeInsets.all(32),
            child: Text('Chưa có thông báo nào.', textAlign: TextAlign.center),
          ),
        ],
      );
    }
    return ListView.separated(
      itemCount: _items.length,
      separatorBuilder: (_, __) => const Divider(height: 1),
      itemBuilder: (_, i) => _tile(_items[i]),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Thông báo')),
      body: RefreshIndicator(onRefresh: _load, child: _body()),
    );
  }
}
