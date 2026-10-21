import 'dart:io';

import 'package:flutter/material.dart';
import 'package:image_picker/image_picker.dart';

import '../api.dart';
import '../models.dart';

class CreateOrderPage extends StatefulWidget {
  const CreateOrderPage({super.key});

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

  List<Station> _stations = [];
  String? _origin;
  String? _destination;
  XFile? _image;
  bool _loading = true;
  bool _saving = false;
  String? _error;

  @override
  void initState() {
    super.initState();
    _loadStations();
  }

  @override
  void dispose() {
    for (final c in [_weight, _length, _width, _height, _description, _notes]) {
      c.dispose();
    }
    super.dispose();
  }

  Future<void> _loadStations() async {
    try {
      final stations = await Api.instance.getStations();
      if (!mounted) return;
      setState(() {
        _stations = stations;
        _loading = false;
      });
    } on ApiException catch (e) {
      if (!mounted) return;
      setState(() {
        _error = e.message;
        _loading = false;
      });
    }
  }

  Future<void> _pick(ImageSource source) async {
    final file = await ImagePicker().pickImage(source: source, maxWidth: 1600, imageQuality: 85);
    if (file != null && mounted) setState(() => _image = file);
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
      String? imageKey;
      if (_image != null) {
        imageKey = await Api.instance.uploadImage(_image!.path, _image!.name);
      }
      final body = <String, dynamic>{
        'originStationId': _origin,
        'destinationStationId': _destination,
        'notes': _notes.text.trim().isEmpty ? null : _notes.text.trim(),
        'package': {
          'description': _description.text.trim().isEmpty ? null : _description.text.trim(),
          'weightKg': _num(_weight.text),
          'lengthCm': _num(_length.text),
          'widthCm': _num(_width.text),
          'heightCm': _num(_height.text),
          'imageKey': imageKey,
        },
      };
      await Api.instance.createOrder(body);
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
      appBar: AppBar(title: const Text('Tạo đơn hàng')),
      body: _loading
          ? const Center(child: CircularProgressIndicator())
          : Form(
              key: _formKey,
              child: ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  _stationDropdown('Trạm đi', _origin, (v) => setState(() => _origin = v)),
                  const SizedBox(height: 12),
                  _stationDropdown('Trạm đến', _destination, (v) => setState(() => _destination = v)),
                  const SizedBox(height: 20),
                  Text('Thông tin kiện hàng', style: Theme.of(context).textTheme.titleMedium),
                  const SizedBox(height: 12),
                  TextFormField(
                    controller: _description,
                    decoration: const InputDecoration(labelText: 'Mô tả (không bắt buộc)', border: OutlineInputBorder()),
                  ),
                  const SizedBox(height: 12),
                  _numberField(_weight, 'Cân nặng (kg)', max: 50),
                  const SizedBox(height: 12),
                  Row(
                    children: [
                      Expanded(child: _numberField(_length, 'Dài (cm)')),
                      const SizedBox(width: 8),
                      Expanded(child: _numberField(_width, 'Rộng (cm)')),
                      const SizedBox(width: 8),
                      Expanded(child: _numberField(_height, 'Cao (cm)')),
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
                    child: Text(_saving ? 'Đang gửi...' : 'Tạo đơn'),
                  ),
                ],
              ),
            ),
    );
  }
}
