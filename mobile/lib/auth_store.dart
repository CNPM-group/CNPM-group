import 'dart:convert';

import 'package:flutter/foundation.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

import 'api.dart';
import 'models.dart';
import 'nav.dart';

/// Giữ trạng thái đăng nhập, token lưu trong bộ nhớ an toàn của máy.
class AuthStore extends ChangeNotifier {
  AuthStore._();
  static final AuthStore instance = AuthStore._();

  static const _key = 'sd_auth';
  final FlutterSecureStorage _storage = const FlutterSecureStorage();

  AuthData? _auth;
  bool _loaded = false;

  AuthData? get auth => _auth;
  String? get token => _auth?.token;
  bool get loaded => _loaded;
  bool get isLoggedIn => _auth != null;

  Future<void> load() async {
    try {
      final raw = await _storage.read(key: _key);
      if (raw != null) {
        final data = AuthData.fromJson(jsonDecode(raw) as Map<String, dynamic>);
        if (data.expiresAtUtc.isAfter(DateTime.now().toUtc())) {
          _auth = data;
        } else {
          await _storage.delete(key: _key);
        }
      }
    } catch (_) {
      _auth = null;
    }
    _loaded = true;
    notifyListeners();
  }

  Future<void> login(String email, String password) async {
    await _save(await Api.instance.login(email, password));
  }

  Future<void> register(String email, String password, String fullName) async {
    await _save(await Api.instance.register(email, password, fullName));
  }

  Future<void> _save(AuthData data) async {
    _auth = data;
    await _storage.write(key: _key, value: jsonEncode(data.toJson()));
    notifyListeners();
  }

  Future<void> logout() async {
    _auth = null;
    await _storage.delete(key: _key);
    // Đóng hết các màn đang mở, về màn gốc (Gate sẽ hiện trang đăng nhập)
    navigatorKey.currentState?.popUntil((route) => route.isFirst);
    notifyListeners();
  }
}
