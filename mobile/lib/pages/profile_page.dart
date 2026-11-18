import 'package:flutter/material.dart';

import '../api.dart';
import '../models.dart';

/// Hồ sơ của tôi (v0.11, FR01): sửa họ tên và số điện thoại. Email không đổi được.
/// Số điện thoại ở đây là số mặc định cho người nhận khi tạo đơn mà bỏ trống ô người nhận.
class ProfilePage extends StatefulWidget {
  const ProfilePage({super.key});

  @override
  State<ProfilePage> createState() => _ProfilePageState();
}

class _ProfilePageState extends State<ProfilePage> {
  final _form = GlobalKey<FormState>();
  final _name = TextEditingController();
  final _phone = TextEditingController();
  final _currentPw = TextEditingController();
  final _newPw = TextEditingController();
  UserProfile? _profile;
  bool _saving = false;
  String? _error;

  @override
  void initState() {
    super.initState();
    _load();
  }

  @override
  void dispose() {
    _name.dispose();
    _phone.dispose();
    _currentPw.dispose();
    _newPw.dispose();
    super.dispose();
  }

  Future<void> _load() async {
    try {
      final p = await Api.instance.getMe();
      if (!mounted) return;
      setState(() {
        _profile = p;
        _name.text = p.fullName;
        _phone.text = p.phone ?? '';
      });
    } on ApiException catch (e) {
      if (mounted) setState(() => _error = e.message);
    }
  }

  Future<void> _save() async {
    if (!_form.currentState!.validate()) return;
    setState(() {
      _saving = true;
      _error = null;
    });
    try {
      final phone = _phone.text.trim();
      final p = await Api.instance.updateMe(_name.text.trim(), phone.isEmpty ? null : phone);
      final changePw = _newPw.text.isNotEmpty;
      if (changePw) await Api.instance.changePassword(_currentPw.text, _newPw.text);
      if (!mounted) return;
      setState(() => _profile = p);
      _currentPw.clear();
      _newPw.clear();
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(content: Text(changePw ? 'Đã lưu hồ sơ và đổi mật khẩu' : 'Đã lưu hồ sơ')),
      );
    } on ApiException catch (e) {
      if (mounted) setState(() => _error = e.message);
    } finally {
      if (mounted) setState(() => _saving = false);
    }
  }

  @override
  Widget build(BuildContext context) {
    final p = _profile;
    return Scaffold(
      appBar: AppBar(title: const Text('Hồ sơ của tôi')),
      body: p == null && _error == null
          ? const Center(child: CircularProgressIndicator())
          : Form(
              key: _form,
              child: ListView(
                padding: const EdgeInsets.all(16),
                children: [
                  if (p != null) ...[
                    Text(p.email, style: Theme.of(context).textTheme.titleMedium),
                    const SizedBox(height: 16),
                  ],
                  TextFormField(
                    controller: _name,
                    maxLength: 100,
                    decoration: const InputDecoration(labelText: 'Họ tên', border: OutlineInputBorder()),
                    validator: (v) => (v ?? '').trim().length < 2 ? 'Họ tên ít nhất 2 ký tự' : null,
                  ),
                  const SizedBox(height: 8),
                  TextFormField(
                    controller: _phone,
                    keyboardType: TextInputType.phone,
                    maxLength: 20,
                    decoration: const InputDecoration(labelText: 'Số điện thoại', border: OutlineInputBorder()),
                    validator: (v) {
                      final t = (v ?? '').trim();
                      if (t.isEmpty) return null;
                      return RegExp(r'^\+?[0-9 .]{8,15}$').hasMatch(t) ? null : 'Số điện thoại 8–15 chữ số';
                    },
                  ),
                  const SizedBox(height: 8),
                  Text('Đổi mật khẩu (bỏ trống nếu không đổi)', style: Theme.of(context).textTheme.titleSmall),
                  const SizedBox(height: 8),
                  TextFormField(
                    controller: _currentPw,
                    obscureText: true,
                    decoration: const InputDecoration(labelText: 'Mật khẩu hiện tại', border: OutlineInputBorder()),
                    validator: (v) => _newPw.text.isNotEmpty && (v ?? '').isEmpty ? 'Nhập mật khẩu hiện tại' : null,
                  ),
                  const SizedBox(height: 8),
                  TextFormField(
                    controller: _newPw,
                    obscureText: true,
                    decoration: const InputDecoration(labelText: 'Mật khẩu mới (ít nhất 8 ký tự)', border: OutlineInputBorder()),
                    validator: (v) => (v ?? '').isNotEmpty && v!.length < 8 ? 'Ít nhất 8 ký tự' : null,
                  ),
                  if (_error != null)
                    Padding(
                      padding: const EdgeInsets.symmetric(vertical: 8),
                      child: Text(_error!, style: TextStyle(color: Theme.of(context).colorScheme.error)),
                    ),
                  const SizedBox(height: 8),
                  FilledButton(
                    onPressed: _saving || p == null ? null : _save,
                    child: Text(_saving ? 'Đang lưu...' : 'Lưu'),
                  ),
                ],
              ),
            ),
    );
  }
}
