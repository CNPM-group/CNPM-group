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

  /// [search]: tìm theo mã đơn (v0.11), trống = lấy hết.
  Future<OrderPage> getOrders({int page = 1, int pageSize = 50, String? search}) => _guard(() async {
        final r = await _dio.get('/orders', queryParameters: {
          'page': page,
          'pageSize': pageSize,
          if (search != null && search.isNotEmpty) 'q': search,
        });
        final d = r.data as Map<String, dynamic>;
        return OrderPage(
          items: (d['items'] as List).map((e) => OrderSummary.fromJson(e as Map<String, dynamic>)).toList(),
          page: d['page'] as int,
          pageSize: d['pageSize'] as int,
          totalItems: d['totalItems'] as int,
        );
      });

  /// Hồ sơ cá nhân (v0.11).
  Future<UserProfile> getMe() => _guard(() async {
        final r = await _dio.get('/auth/me');
        return UserProfile.fromJson(r.data as Map<String, dynamic>);
      });

  Future<UserProfile> updateMe(String fullName, String? phone) => _guard(() async {
        final r = await _dio.put('/auth/me', data: {'fullName': fullName, 'phone': phone});
        return UserProfile.fromJson(r.data as Map<String, dynamic>);
      });

  /// Đổi mật khẩu (v0.11), sai mật khẩu cũ → ApiException "Mật khẩu hiện tại không đúng."
  Future<void> changePassword(String currentPassword, String newPassword) => _guard(() async {
        await _dio.post('/auth/me/password', data: {'currentPassword': currentPassword, 'newPassword': newPassword});
      });

  Future<Order> getOrder(String id) => _guard(() async {
        final r = await _dio.get('/orders/$id');
        return Order.fromJson(r.data as Map<String, dynamic>);
      });

  Future<List<HistoryItem>> getHistory(String id) => _guard(() async {
        final r = await _dio.get('/orders/$id/history');
        return (r.data as List).map((e) => HistoryItem.fromJson(e as Map<String, dynamic>)).toList();
      });

  /// Trả về id đơn vừa tạo. API tạo/hủy chỉ trả bản tóm tắt, chi tiết đầy đủ lấy bằng getOrder.
  Future<String> createOrder(Map<String, dynamic> body) => _guard(() async {
        final r = await _dio.post('/orders', data: body);
        return (r.data as Map<String, dynamic>)['id'] as String;
      });

  /// Sửa đơn khi còn Pending (v0.11, FR05). Body giống tạo đơn.
  Future<void> updateOrder(String id, Map<String, dynamic> body) => _guard(() async {
        await _dio.put('/orders/$id', data: body);
      });

  Future<void> cancelOrder(String id, {String? reason}) => _guard(() async {
        await _dio.post('/orders/$id/cancel', data: {'reason': reason});
      });

  Future<List<AppNotification>> getNotifications({bool unreadOnly = false}) => _guard(() async {
        final r = await _dio.get('/notifications', queryParameters: {'unreadOnly': unreadOnly});
        return (r.data as List).map((e) => AppNotification.fromJson(e as Map<String, dynamic>)).toList();
      });

  Future<void> markRead(String id) => _guard(() async {
        await _dio.post('/notifications/$id/read');
      });

  /// Vị trí drone của đơn (v0.9).
  Future<TrackingInfo> getTracking(String orderId) => _guard(() async {
        final r = await _dio.get('/orders/$orderId/tracking');
        return TrackingInfo.fromJson(r.data as Map<String, dynamic>);
      });

  /// Ước tính trước khi đặt đơn: khoảng cách đường chim bay và số phút bay.
  Future<({double km, double minutes})> estimateEta(String originId, String destinationId, double weightKg) =>
      _guard(() async {
        final r = await _dio.get('/orders/eta', queryParameters: {
          'originStationId': originId,
          'destinationStationId': destinationId,
          'weightKg': weightKg,
        });
        final j = r.data as Map<String, dynamic>;
        return (km: (j['distanceKm'] as num).toDouble(), minutes: (j['estimatedMinutes'] as num).toDouble());
      });

  /// AI tóm tắt chuyến giao (v0.10, KAN-62). Chờ tối đa 60 giây vì phải đợi LLM viết.
  Future<AiSummary> getAiSummary(String orderId) => _guard(() async {
        final r = await _dio.post(
          '/orders/$orderId/ai-summary',
          options: Options(receiveTimeout: const Duration(seconds: 60)),
        );
        return AiSummary.fromJson(r.data as Map<String, dynamic>);
      });

  Future<String> chat(String message) => _guard(() async {
        final r = await _dio.post(
          '/ai/chat',
          data: {'message': message},
          options: Options(receiveTimeout: const Duration(seconds: 60)),
        );
        return ((r.data as Map<String, dynamic>)['answer'] as String?) ?? '';
      });

  Future<List<CustomerAddress>> getAddresses() => _guard(() async {
        final r = await _dio.get('/customers/me/addresses');
        return (r.data as List).map((e) => CustomerAddress.fromJson(e as Map<String, dynamic>)).toList();
      });

  /// id = null thì thêm mới, có id thì sửa.
  Future<void> saveAddress(String? id, Map<String, dynamic> body) => _guard(() async {
        if (id == null) {
          await _dio.post('/customers/me/addresses', data: body);
        } else {
          await _dio.put('/customers/me/addresses/$id', data: body);
        }
      });

  Future<void> deleteAddress(String id) => _guard(() async {
        await _dio.delete('/customers/me/addresses/$id');
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
