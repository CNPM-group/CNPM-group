import 'dart:io';

import 'dart:async';

import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';

import '../api.dart';
import '../models.dart';

/// Tạo đơn mới, hoặc sửa đơn còn Pending khi truyền [editing] (v0.11).
class CreateOrderPage extends StatefulWidget {
  final Order? editing;
  const CreateOrderPage({super.key, this.editing});

  @override
  State<CreateOrderPage> createState() => _CreateOrderPageState();
}

class _CreateOrderPageState extends State<CreateOrderPage> {
  final _formKey = GlobalKey<FormState>();
  final _weight = TextEditingController();
  final _length = TextEditingController();
  final _width = TextEditingController();
  final _height = TextEditingController();
  final _description = TextEditingController();
  final _notes = TextEditingController();
  final _recipientName = TextEditingController();
  final _recipientPhone = TextEditingController();

  List<Station> _stations = [];
  List<CustomerAddress> _addresses = [];
  String? _origin;
  String? _destination;
  String? _addressId;
  XFile? _image;
  bool _loading = true;
  bool _saving = false;
  String? _error;
  ({double km, double minutes})? _eta;
  String? _etaError;
  Timer? _etaDebounce;
  String? _existingImageKey;

  bool get _isEdit => widget.editing != null;

  @override
  void initState() {
    super.initState();
    _loadStations();
    _weight.addListener(_scheduleEta);
  }

  @override
  void dispose() {
    _etaDebounce?.cancel();
    for (final c in [_weight, _length, _width, _height, _description, _notes, _recipientName, _recipientPhone]) {
      c.dispose();
    }
    super.dispose();
  }

  Future<void> _loadStations() async {
    try {
      final stations = await Api.instance.getStations();
      final addresses = await Api.instance.getAddresses().catchError((_) => <CustomerAddress>[]);
      if (!mounted) return;
      setState(() {
        _stations = stations;
        _addresses = addresses;
        _addressId = addresses.where((a) => a.isDefault).map((a) => a.id).firstOrNull;
        _loading = false;
      });
      final o = widget.editing;
      if (o != null) _prefill(o);
    } on ApiException catch (e) {
      if (!mounted) return;
      setState(() {
        _error = e.message;
        _loading = false;
      });
    }
  }

  // Sửa đơn: điền sẵn dữ liệu cũ. Không chọn ảnh mới thì giữ ảnh cũ.
  void _prefill(Order o) {
    final p = o.package;
    setState(() {
      _origin = _stations.any((s) => s.id == o.originStationId) ? o.originStationId : null;
      _destination = _stations.any((s) => s.id == o.destinationStationId) ? o.destinationStationId : null;
      _addressId = _addresses.any((a) => a.id == o.deliveryAddressId) ? o.deliveryAddressId : null;
      _weight.text = _fmt(p.weightKg);
      _length.text = _fmt(p.lengthCm);
      _width.text = _fmt(p.widthCm);
      _height.text = _fmt(p.heightCm);
      _description.text = p.description ?? '';
      _notes.text = o.notes ?? '';
      _recipientName.text = o.recipientName ?? '';
      _recipientPhone.text = o.recipientPhone ?? '';
      _existingImageKey = p.imageKey;
    });
    _refreshEta();
  }

  String _fmt(double v) => v == v.roundToDouble() ? v.toStringAsFixed(0) : v.toString();

  Future<void> _pick(ImageSource source) async {
    final file = await ImagePicker().pickImage(source: source, maxWidth: 1600, imageQuality: 85);
    if (file != null && mounted) setState(() => _image = file);
  }

  void _scheduleEta() {
    _etaDebounce?.cancel();
    _etaDebounce = Timer(const Duration(milliseconds: 500), _refreshEta);
  }

  Future<void> _refreshEta() async {
    final weight = _num(_weight.text);
    if (_origin == null || _destination == null || _origin == _destination || weight == null || weight <= 0) {
      if (mounted) setState(() => _eta = null);
      return;
    }
    try {
      final eta = await Api.instance.estimateEta(_origin!, _destination!, weight);
      if (!mounted) return;
      setState(() {
        _eta = eta;
        _etaError = null;
      });
    } on ApiException catch (e) {
      if (!mounted) return;
      setState(() {
        _eta = null;
        _etaError = e.message;
      });
    }
  }

  Widget _etaCard() {
    if (_eta == null && _etaError == null) return const SizedBox.shrink();
    final scheme = Theme.of(context).colorScheme;
    final eta = _eta;
    return Card(
      color: eta == null ? scheme.errorContainer : scheme.secondaryContainer,
      margin: const EdgeInsets.only(top: 12),
      child: ListTile(
        leading: Icon(eta == null ? Icons.error_outline : Icons.schedule),
        title: Text(eta == null
            ? _etaError!
            : 'Bay khoảng ${eta.minutes.round()} phút · ${eta.km.toStringAsFixed(1)} km'),
        subtitle: eta == null ? null : const Text('Tính từ lúc drone cất cánh, chưa gồm thời gian chờ duyệt'),
      ),
    );
  }

  double? _num(String v) => double.tryParse(v.trim().replaceAll(',', '.'));

  String? _positive(String? v, {double? max}) {
    final n = _num(v ?? '');
    if (n == null || n <= 0) return 'Nhập số lớn hơn 0';
    if (max != null && n > max) return 'Tối đa $max';
    return null;
  }

  Future<void> _submit() async {
    if (!_formKey.currentState!.validate()) return;
    if (_origin == _destination) {
      setState(() => _error = 'Trạm đi và trạm đến phải khác nhau.');
      return;
    }
    setState(() {
      _saving = true;
      _error = null;
    });
    try {
      String? imageKey = _existingImageKey;
      if (_image != null) {
        try {
          imageKey = await Api.instance.uploadImage(_image!.path, _image!.name);
        } on ApiException catch (e) {
          if (mounted) {
            ScaffoldMessenger.of(context).showSnackBar(
              SnackBar(content: Text('Không tải được ảnh (${e.message}), đơn vẫn được tạo không kèm ảnh.')),
            );
          }
        }
      }
      final body = <String, dynamic>{
        'originStationId': _origin,
        'destinationStationId': _destination,
        'deliveryAddressId': _addressId,
        'notes': _notes.text.trim().isEmpty ? null : _notes.text.trim(),
        // Bỏ trống thì server lấy tên + số điện thoại của chính khách (v0.11).
        'recipientName': _recipientName.text.trim().isEmpty ? null : _recipientName.text.trim(),
        'recipientPhone': _recipientPhone.text.trim().isEmpty ? null : _recipientPhone.text.trim(),
        'package': {
          'description': _description.text.trim().isEmpty ? null : _description.text.trim(),
          'weightKg': _num(_weight.text),
          'lengthCm': _num(_length.text),
          'widthCm': _num(_width.text),
          'heightCm': _num(_height.text),
          'imageKey': imageKey,
        },
      };
      if (_isEdit) {
        await Api.instance.updateOrder(widget.editing!.id, body);
      } else {
        await Api.instance.createOrder(body);
      }
      if (!mounted) return;
      Navigator.of(context).pop(true);
    } on ApiException catch (e) {
      if (!mounted) return;
      setState(() => _error = e.message);
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  Widget _stationDropdown(String label, String? value, ValueChanged<String?> onChanged) {
    return DropdownButtonFormField<String>(
      value: value,
      isExpanded: true,
      decoration: InputDecoration(labelText: label, border: const OutlineInputBorder()),
      items: _stations.map((s) => DropdownMenuItem(value: s.id, child: Text(s.name))).toList(),
      onChanged: onChanged,
      validator: (v) => v == null ? 'Chọn $label' : null,
    );
  }

  Widget _numberField(TextEditingController c, String label, {double? max}) {
    return TextFormField(
      controller: c,
      keyboardType: const TextInputType.numberWithOptions(decimal: true),
      decoration: InputDecoration(labelText: label, border: const OutlineInputBorder()),
      validator: (v) => _positive(v, max: max),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(title: Text(_isEdit ? 'Sửa đơn ${widget.editing!.trackingCode}' : 'Tạo đơn hàng')),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : Form(
              key: _formKey,
              child: ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  _stationDropdown('Trạm đi', _origin, (v) {
                    setState(() => _origin = v);
                    _refreshEta();
                  }),
                  const SizedBox(height: 12),
                  _stationDropdown('Trạm đến', _destination, (v) {
                    setState(() => _destination = v);
                    _refreshEta();
                  }),
                  if (_addresses.isNotEmpty) ...[
                    const SizedBox(height: 12),
                    DropdownButtonFormField<String?>(
                      value: _addressId,
                      isExpanded: true,
                      decoration: const InputDecoration(
                        labelText: 'Địa chỉ nhận (không bắt buộc)',
                        border: OutlineInputBorder(),
                      ),
                      items: [
                        const DropdownMenuItem<String?>(value: null, child: Text('Không chọn')),
                        ..._addresses.map(
                          (a) => DropdownMenuItem<String?>(
                            value: a.id,
                            child: Text('${a.label} – ${a.addressLine}', overflow: TextOverflow.ellipsis),
                          ),
                        ),
                      ],
                      onChanged: (v) => setState(() => _addressId = v),
                    ),
                  ],
                  const SizedBox(height: 20),
                  Text('Thông tin kiện hàng', style: Theme.of(context).textTheme.titleMedium),
                  const SizedBox(height: 12),
                  TextFormField(
                    controller: _description,
                    decoration: const InputDecoration(labelText: 'Mô tả (không bắt buộc)', border: OutlineInputBorder()),
                  ),
                  const SizedBox(height: 12),
                  _numberField(_weight, 'Cân nặng (kg)', max: 50),
                  _etaCard(),
                  const SizedBox(height: 12),
                  Row(
                    children: [
                      Expanded(child: _numberField(_length, 'Dài (cm)', max: 100)),
                      const SizedBox(width: 8),
                      Expanded(child: _numberField(_width, 'Rộng (cm)', max: 100)),
                      const SizedBox(width: 8),
                      Expanded(child: _numberField(_height, 'Cao (cm)', max: 100)),
                    ],
                  ),
                  const SizedBox(height: 12),
                  Row(
                    children: [
                      OutlinedButton.icon(
                        onPressed: _saving ? null : () => _pick(ImageSource.gallery),
                        icon: const Icon(Icons.photo_library),
                        label: const Text('Chọn ảnh'),
                      ),
                      const SizedBox(width: 8),
                      OutlinedButton.icon(
                        onPressed: _saving ? null : () => _pick(ImageSource.camera),
                        icon: const Icon(Icons.photo_camera),
                        label: const Text('Chụp ảnh'),
                      ),
                    ],
                  ),
                  if (_image != null) ...[
                    const SizedBox(height: 12),
                    ClipRRect(
                      borderRadius: BorderRadius.circular(8),
                      child: Image.file(File(_image!.path), height: 160, fit: BoxFit.cover),
                    ),
                  ],
                  const SizedBox(height: 12),
                  Text('Người nhận (bỏ trống nếu bạn tự nhận)', style: Theme.of(context).textTheme.titleSmall),
                  const SizedBox(height: 8),
                  TextFormField(
                    controller: _recipientName,
                    maxLength: 100,
                    decoration: const InputDecoration(labelText: 'Tên người nhận', border: OutlineInputBorder()),
                  ),
                  const SizedBox(height: 4),
                  TextFormField(
                    controller: _recipientPhone,
                    keyboardType: TextInputType.phone,
                    maxLength: 20,
                    decoration: const InputDecoration(labelText: 'SĐT người nhận', border: OutlineInputBorder()),
                    validator: (v) {
                      final t = (v ?? '').trim();
                      if (t.isEmpty) return null;
                      return RegExp(r'^\+?[0-9 .]{8,15}$').hasMatch(t) ? null : 'Số điện thoại 8–15 chữ số';
                    },
                  ),
                  const SizedBox(height: 4),
                  TextFormField(
                    controller: _notes,
                    maxLines: 2,
                    decoration: const InputDecoration(labelText: 'Ghi chú (không bắt buộc)', border: OutlineInputBorder()),
                  ),
                  const SizedBox(height: 12),
                  if (_error != null)
                    Padding(
                      padding: const EdgeInsets.only(bottom: 12),
                      child: Text(_error!, style: TextStyle(color: Theme.of(context).colorScheme.error)),
                    ),
                  FilledButton(
                    onPressed: _saving ? null : _submit,
                    child: Text(_saving ? 'Đang gửi...' : (_isEdit ? 'Lưu thay đổi' : 'Tạo đơn')),
                  ),
                ],
              ),
            ),
    );
  }
}
