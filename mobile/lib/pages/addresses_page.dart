import 'package:flutter/material.dart';

import '../api.dart';
import '../models.dart';

class AddressesPage extends StatefulWidget {
  const AddressesPage({super.key});

  @override
  State<AddressesPage> createState() => _AddressesPageState();
}

class _AddressesPageState extends State<AddressesPage> {
  List<CustomerAddress> _items = [];
  bool _loading = true;
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  Future<void> _load() async {
    try {
      final items = await Api.instance.getAddresses();
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

  void _toast(String text) {
    if (!mounted) return;
    ScaffoldMessenger.of(context).showSnackBar(SnackBar(content: Text(text)));
  }

  Future<void> _edit([CustomerAddress? current]) async {
    final saved = await showDialog<bool>(
      context: context,
      builder: (_) => _AddressDialog(current: current),
    );
    if (saved == true) {
      _toast(current == null ? 'Đã thêm địa chỉ' : 'Đã cập nhật địa chỉ');
      _load();
    }
  }

  Future<void> _delete(CustomerAddress a) async {
    final ok = await showDialog<bool>(
      context: context,
      builder: (ctx) => AlertDialog(
        title: Text('Xóa "${a.label}"?'),
        actions: [
          TextButton(onPressed: () => Navigator.pop(ctx, false), child: const Text('Hủy')),
          FilledButton(onPressed: () => Navigator.pop(ctx, true), child: const Text('Xóa')),
        ],
      ),
    );
    if (ok != true) return;
    try {
      await Api.instance.deleteAddress(a.id);
      _toast('Đã xóa địa chỉ');
      _load();
    } on ApiException catch (e) {
      _toast(e.message);
    }
  }

  Future<void> _makeDefault(CustomerAddress a) async {
    try {
      await Api.instance.saveAddress(a.id, {
        'label': a.label,
        'addressLine': a.addressLine,
        'latitude': a.latitude,
        'longitude': a.longitude,
        'isDefault': true,
      });
      _load();
    } on ApiException catch (e) {
      _toast(e.message);
    }
  }

  Widget _body() {
    if (_loading) return const Center(child: CircularProgressIndicator());
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
            child: Text('Chưa có địa chỉ nào. Bấm + để thêm.', textAlign: TextAlign.center),
          ),
        ],
      );
    }
    return ListView.separated(
      itemCount: _items.length,
      separatorBuilder: (_, __) => const Divider(height: 1),
      itemBuilder: (_, i) {
        final a = _items[i];
        return ListTile(
          leading: Icon(a.isDefault ? Icons.star : Icons.place_outlined,
              color: a.isDefault ? Colors.amber : null),
          title: Text(a.label),
          subtitle: Text(a.addressLine),
          onTap: () => _edit(a),
          trailing: PopupMenuButton<String>(
            onSelected: (v) => v == 'default' ? _makeDefault(a) : _delete(a),
            itemBuilder: (_) => [
              if (!a.isDefault) const PopupMenuItem(value: 'default', child: Text('Đặt làm mặc định')),
              const PopupMenuItem(value: 'delete', child: Text('Xóa')),
            ],
          ),
        );
      },
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: const Text('Sổ địa chỉ')),
      floatingActionButton: FloatingActionButton(
        onPressed: () => _edit(),
        child: const Icon(Icons.add),
      ),
      body: RefreshIndicator(onRefresh: _load, child: _body()),
    );
  }
}

class _AddressDialog extends StatefulWidget {
  final CustomerAddress? current;

  const _AddressDialog({this.current});

  @override
  State<_AddressDialog> createState() => _AddressDialogState();
}

class _AddressDialogState extends State<_AddressDialog> {
  final _formKey = GlobalKey<FormState>();
  late final _label = TextEditingController(text: widget.current?.label ?? '');
  late final _line = TextEditingController(text: widget.current?.addressLine ?? '');
  late bool _isDefault = widget.current?.isDefault ?? false;
  bool _saving = false;
  String? _error;

  @override
  void dispose() {
    _label.dispose();
    _line.dispose();
    super.dispose();
  }

  Future<void> _save() async {
    if (!_formKey.currentState!.validate()) return;
    setState(() {
      _saving = true;
      _error = null;
    });
    try {
      await Api.instance.saveAddress(widget.current?.id, {
        'label': _label.text.trim(),
        'addressLine': _line.text.trim(),
        'latitude': widget.current?.latitude,
        'longitude': widget.current?.longitude,
        'isDefault': _isDefault,
      });
      if (mounted) Navigator.pop(context, true);
    } on ApiException catch (e) {
      if (mounted) setState(() => _error = e.message);
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    return AlertDialog(
      title: Text(widget.current == null ? 'Thêm địa chỉ' : 'Sửa địa chỉ'),
      content: Form(
        key: _formKey,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            TextFormField(
              controller: _label,
              maxLength: 50,
              decoration: const InputDecoration(labelText: 'Tên gợi nhớ (Nhà, Phòng trọ...)'),
              validator: (v) => (v == null || v.trim().isEmpty) ? 'Nhập tên gợi nhớ' : null,
            ),
            TextFormField(
              controller: _line,
              maxLength: 255,
              minLines: 1,
              maxLines: 3,
              decoration: const InputDecoration(labelText: 'Địa chỉ'),
              validator: (v) => (v == null || v.trim().isEmpty) ? 'Nhập địa chỉ' : null,
            ),
            CheckboxListTile(
              contentPadding: EdgeInsets.zero,
              value: _isDefault,
              onChanged: (v) => setState(() => _isDefault = v ?? false),
              title: const Text('Đặt làm mặc định'),
            ),
            if (_error != null)
              Text(_error!, style: TextStyle(color: Theme.of(context).colorScheme.error)),
          ],
        ),
      ),
      actions: [
        TextButton(onPressed: () => Navigator.pop(context, false), child: const Text('Hủy')),
        FilledButton(onPressed: _saving ? null : _save, child: const Text('Lưu')),
      ],
    );
  }
}
