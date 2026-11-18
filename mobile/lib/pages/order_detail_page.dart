import 'dart:async';

import 'package:flutter/material.dart';

import '../api.dart';
import '../auth_store.dart';
import '../config.dart';
import '../format.dart';
import '../models.dart';
import '../tracking_map.dart';
import '../services/signalr_service.dart';
import '../widgets.dart';
import 'create_order_page.dart';

class OrderDetailPage extends StatefulWidget {
  final String orderId;
  const OrderDetailPage({super.key, required this.orderId});

  @override
  State<OrderDetailPage> createState() => _OrderDetailPageState();
}

class _OrderDetailPageState extends State<OrderDetailPage> {
  Order? _order;
  List<HistoryItem> _history = [];
  bool _loading = true;
  bool _busy = false;
  String? _error;

  StreamSubscription<Map<String, dynamic>>? _sub;
  StreamSubscription<Map<String, dynamic>>? _droneSub;
  TrackingInfo? _tracking;

  static const _trackedStatuses = {'InTransit', 'ArrivedAtStation'};
  // Đơn đã kết thúc mới có gì để tóm tắt.
  static const _finishedStatuses = {'Completed', 'Failed', 'Cancelled', 'Rejected'};

  AiSummary? _summary;
  bool _summarizing = false;

  @override
  void initState() {
    super.initState();
    _load();
    _sub = SignalRService.instance.updates.listen((event) {
      final id = event['orderId'];
      if (id == null || id == widget.orderId) _load();
    });
    _droneSub = SignalRService.instance.drone.listen((event) {
      final t = _tracking;
      if (t == null || event['orderId'] != widget.orderId || !mounted) return;
      setState(() => _tracking = t.moved(event));
    });
  }

  @override
  void dispose() {
    _sub?.cancel();
    _droneSub?.cancel();
    super.dispose();
  }

  Future<void> _load() async {
    setState(() {
      _loading = true;
      _error = null;
    });
    try {
      final results = await Future.wait([
        Api.instance.getOrder(widget.orderId),
        Api.instance.getHistory(widget.orderId),
      ]);
      if (!mounted) return;
      final order = results[0] as Order;
      TrackingInfo? tracking;
      if (_trackedStatuses.contains(order.status)) {
        try {
          tracking = await Api.instance.getTracking(widget.orderId);
        } on ApiException {
          tracking = null;
        }
      }
      if (!mounted) return;
      setState(() {
        _order = order;
        _history = results[1] as List<HistoryItem>;
        _tracking = tracking;
      });
    } on ApiException catch (e) {
      if (!mounted) return;
      setState(() => _error = e.message);
    } finally {
      if (mounted) setState(() => _loading = false);
    }
  }

  Future<void> _cancel() async {
    final ok = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: const Text('Hủy đơn này?'),
        content: const Text('Chỉ hủy được khi đơn chưa được duyệt.'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Không')),
          FilledButton(onPressed: () => Navigator.pop(ctx, true), child: const Text('Hủy đơn')),
        ],
      ),
    );
    if (ok != true) return;
    setState(() => _busy = true);
    try {
      await Api.instance.cancelOrder(widget.orderId);
      await _load();
    } on ApiException catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(e.message)));
    } finally {
      if (mounted) setState(() => _busy = false);
    }
  }

  Future<void> _summarize() async {
    setState(() => _summarizing = true);
    try {
      final s = await Api.instance.getAiSummary(widget.orderId);
      if (mounted) setState(() => _summary = s);
    } on ApiException catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(e.message)));
    } finally {
      if (mounted) setState(() => _summarizing = false);
    }
  }

  Widget _summaryCard() {
    final s = _summary;
    return Card(
      child: Padding(
        padding: const EdgeInsets.all(12),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              children: [
                const Icon(Icons.auto_awesome, size: 18),
                const SizedBox(width: 6),
                Expanded(child: Text('Tóm tắt chuyến giao', style: Theme.of(context).textTheme.titleMedium)),
                if (s != null)
                  TextButton(onPressed: _summarizing ? null : _summarize, child: const Text('Viết lại')),
              ],
            ),
            const SizedBox(height: 8),
            if (s == null)
              FilledButton.tonal(
                onPressed: _summarizing ? null : _summarize,
                child: Text(_summarizing ? 'AI đang viết...' : 'Nhờ AI tóm tắt'),
              )
            else ...[
              Text(s.text, style: const TextStyle(height: 1.4)),
              const SizedBox(height: 6),
              Text(
                s.byLlm ? 'AI viết${s.model != null ? ' · ${s.model}' : ''}' : 'Tự ghép từ dữ liệu (chưa dùng AI)',
                style: const TextStyle(fontSize: 12, color: Colors.grey),
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _row(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(width: 120, child: Text(label, style: const TextStyle(color: Colors.grey))),
          Expanded(child: Text(value)),
        ],
      ),
    );
  }

  Widget _content(Order o) {
    final p = o.package;
    return ListView(
      padding: const EdgeInsets.all(16),
      children: [
        Row(
          children: [
            Expanded(
              child: Text(o.trackingCode, style: Theme.of(context).textTheme.titleLarge),
            ),
            StatusChip(o.status),
          ],
        ),
        const SizedBox(height: 12),
        if (_tracking != null)
          Card(
            child: Padding(
              padding: const EdgeInsets.all(12),
              child: Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('Theo dõi drone', style: Theme.of(context).textTheme.titleMedium),
                  const SizedBox(height: 10),
                  TrackingMap(info: _tracking!),
                ],
              ),
            ),
          ),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(12),
            child: Column(
              children: [
                _row('Trạm đi', o.originStationName),
                _row('Trạm đến', o.destinationStationName),
                if (o.recipientName != null)
                  _row('Người nhận', '${o.recipientName}${o.recipientPhone != null ? ' · ${o.recipientPhone}' : ''}'),
                _row('ETA', formatDate(o.eta)),
                _row('Ngày tạo', formatDate(o.createdAtUtc)),
                if (o.notes != null) _row('Ghi chú', o.notes!),
                if (o.failureReason != null) _row('Lý do thất bại', o.failureReason!),
              ],
            ),
          ),
        ),
        Card(
          child: Padding(
            padding: const EdgeInsets.all(12),
            child: Column(
              children: [
                _row('Mô tả', p.description ?? '-'),
                _row('Cân nặng', '${p.weightKg} kg'),
                _row('Kích thước', '${p.lengthCm} x ${p.widthCm} x ${p.heightCm} cm'),
                if (p.imageKey != null)
                  Padding(
                    padding: const EdgeInsets.only(top: 8),
                    child: ClipRRect(
                      borderRadius: BorderRadius.circular(8),
                      child: Image.network(
                        '$apiBaseUrl/orders/${o.id}/package-image',
                        headers: {'Authorization': 'Bearer ${AuthStore.instance.token ?? ''}'},
                        height: 180,
                        width: double.infinity,
                        fit: BoxFit.cover,
                        errorBuilder: (_, __, ___) => const Text('Không tải được ảnh kiện hàng'),
                      ),
                    ),
                  ),
              ],
            ),
          ),
        ),
        if (_finishedStatuses.contains(o.status)) _summaryCard(),
        const SizedBox(height: 8),
        Text('Lịch sử trạng thái', style: Theme.of(context).textTheme.titleMedium),
        const SizedBox(height: 4),
        for (final h in _history)
          ListTile(
            contentPadding: EdgeInsets.zero,
            leading: Icon(Icons.circle, size: 14, color: statusColor(h.toStatus)),
            title: Text(statusLabel(h.toStatus)),
            subtitle: Text(
              '${formatDate(h.changedAtUtc)}'
              '${h.changedByName != null ? ' · ${h.changedByName}' : ''}'
              '${h.note != null ? '\n"${h.note}"' : ''}',
            ),
          ),
        if (o.status == 'Pending') ...[
          const SizedBox(height: 16),
          FilledButton.tonalIcon(
            onPressed: _busy
                ? null
                : () async {
                    final saved = await Navigator.of(context).push<bool>(
                      MaterialPageRoute(builder: (_) => CreateOrderPage(editing: o)),
                    );
                    if (saved == true) _load();
                  },
            icon: const Icon(Icons.edit_outlined),
            label: const Text('Sửa đơn'),
          ),
          const SizedBox(height: 8),
          OutlinedButton.icon(
            onPressed: _busy ? null : _cancel,
            icon: const Icon(Icons.cancel_outlined),
            label: const Text('Hủy đơn'),
          ),
        ],
      ],
    );
  }

  @override
  Widget build(BuildContext context) {
    final order = _order;
    Widget body;
    if (_loading && order == null) {
      body = const Center(child: CircularProgressIndicator());
    } else if (_error != null) {
      body = ListView(
        children: [
          Padding(
            padding: const EdgeInsets.all(24),
            child: Text(_error!, style: TextStyle(color: Theme.of(context).colorScheme.error)),
          ),
          TextButton(onPressed: _load, child: const Text('Thử lại')),
        ],
      );
    } else if (order == null) {
      body = const SizedBox.shrink();
    } else {
      body = _content(order);
    }
    return Scaffold(
      appBar: AppBar(title: const Text('Chi tiết đơn')),
      body: RefreshIndicator(onRefresh: _load, child: body),
    );
  }
}
