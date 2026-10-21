import 'package:dio/dio.dart';

import 'auth_store.dart';
import 'config.dart';
import 'models.dart';

/// Lỗi đã được đổi sang câu tiếng Việt để hiện thẳng cho người dùng.
class ApiException implements Exception {
  final String message;
  ApiException(this.message);

  @override
  String toString() => message;
}

String _messageOf(Object e) {
  if (e is DioException) {
    final data = e.response?.data;
    if (data is Map) {
      final errors = data['errors'];
      if (errors is Map && errors.isNotEmpty) {
        return errors.values.expand((v) => v is List ? v : [v]).join(' ');
      }
      final detail = data['detail'];
      if (detail is String && detail.isNotEmpty) return detail;
      final title = data['title'];
      if (title is String && title.isNotEmpty) return title;
    }
    if (e.response == null) return 'Không kết nối được máy chủ. Backend đã chạy chưa?';
    if (e.response?.statusCode == 403) return 'Bạn không có quyền thực hiện thao tác này.';
  }
  return 'Có lỗi xảy ra, thử lại sau.';
}

class Api {
  Api._() {
    _dio.interceptors.add(
      InterceptorsWrapper(
        onRequest: (options, handler) {
          final token = AuthStore.instance.token;
          if (token != null) {
            options.headers['Authorization'] = 'Bearer $token';
          }
          handler.next(options);
        },
        onError: (e, handler) {
          // Token hết hạn hoặc sai: đăng xuất (trừ chính request đăng nhập/đăng ký)
          final isAuthCall = e.requestOptions.path.contains('/auth/');
          if (e.response?.statusCode == 401 && !isAuthCall) {
            AuthStore.instance.logout();
          }
          handler.next(e);
        },
      ),
    );
  }

  static final Api instance = Api._();

  final Dio _dio = Dio(
    BaseOptions(
      baseUrl: apiBaseUrl,
      connectTimeout: const Duration(seconds: 10),
      receiveTimeout: const Duration(seconds: 30),
    ),
  );

  Future<T> _guard<T>(Future<T> Function() action) async {
    try {
      return await action();
    } catch (e) {
      throw ApiException(_messageOf(e));
    }
  }

  Future<AuthData> login(String email, String password) => _guard(() async {
        final r = await _dio.post('/auth/login', data: {'email': email, 'password': password});
        return AuthData.fromJson(r.data as Map<String, dynamic>);
      });

  Future<AuthData> register(String email, String password, String fullName) => _guard(() async {
        final r = await _dio.post(
          '/auth/register',
          data: {'email': email, 'password': password, 'fullName': fullName},
        );
        return AuthData.fromJson(r.data as Map<String, dynamic>);
      });

  Future<List<Station>> getStations() => _guard(() async {
        final r = await _dio.get('/stations');
        return (r.data as List).map((e) => Station.fromJson(e as Map<String, dynamic>)).toList();
      });

  Future<OrderPage> getOrders({int page = 1, int pageSize = 50}) => _guard(() async {
        final r = await _dio.get('/orders', queryParameters: {'page': page, 'pageSize': pageSize});
        final d = r.data as Map<String, dynamic>;
        return OrderPage(
          items: (d['items'] as List).map((e) => OrderSummary.fromJson(e as Map<String, dynamic>)).toList(),
          page: d['page'] as int,
          pageSize: d['pageSize'] as int,
          totalItems: d['totalItems'] as int,
        );
      });

  Future<Order> getOrder(String id) => _guard(() async {
        final r = await _dio.get('/orders/$id');
        return Order.fromJson(r.data as Map<String, dynamic>);
      });

  Future<List<HistoryItem>> getHistory(String id) => _guard(() async {
        final r = await _dio.get('/orders/$id/history');
        return (r.data as List).map((e) => HistoryItem.fromJson(e as Map<String, dynamic>)).toList();
      });

  Future<Order> createOrder(Map<String, dynamic> body) => _guard(() async {
        final r = await _dio.post('/orders', data: body);
        return Order.fromJson(r.data as Map<String, dynamic>);
      });

  Future<Order> cancelOrder(String id, {String? reason}) => _guard(() async {
        final r = await _dio.post('/orders/$id/cancel', data: {'reason': reason});
        return Order.fromJson(r.data as Map<String, dynamic>);
      });

  /// Upload ảnh kiện, trả về imageKey để gửi kèm khi tạo đơn.
  Future<String> uploadImage(String path, String filename) => _guard(() async {
        final form = FormData.fromMap({
          'file': await MultipartFile.fromFile(path, filename: filename),
        });
        final r = await _dio.post('/packages/images', data: form);
        return (r.data as Map<String, dynamic>)['imageKey'] as String;
      });
}
