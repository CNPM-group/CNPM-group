import 'package:flutter/material.dart';

import '../api.dart';
import '../format.dart';
import '../models.dart';
import '../widgets.dart';

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
      final results = await Future.wait([
        Api.instance.getOrder(widget.orderId),
        Api.instance.getHistory(widget.orderId),
      ]);
      if (!mounted) return;
      setState(() {
        _order = results[0] as Order;
        _history = results[1] as List<HistoryItem>;
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
        Card(
          child: Padding(
            padding: const EdgeInsets.all(12),
            child: Column(
              children: [
                _row('Trạm đi', o.originStationName),
                _row('Trạm đến', o.destinationStationName),
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
              ],
            ),
          ),
        ),
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
