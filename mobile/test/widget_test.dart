import 'package:flutter_test/flutter_test.dart';
import 'package:smart_drone_mobile/format.dart';
import 'package:smart_drone_mobile/models.dart';

void main() {
  test('statusLabel trả về tiếng Việt, trạng thái lạ giữ nguyên', () {
    expect(statusLabel('Pending'), 'Chờ duyệt');
    expect(statusLabel('Completed'), 'Hoàn tất');
    expect(statusLabel('Abc'), 'Abc');
  });

  test('formatDate xử lý null', () {
    expect(formatDate(null), '-');
  });

  test('TrackingInfo đọc JSON và cập nhật vị trí từ event DroneLocation', () {
    final t = TrackingInfo.fromJson({
      'orderId': 'o1',
      'status': 'InTransit',
      'droneCode': 'DRONE-001',
      'latitude': 10.8,
      'longitude': 106.7,
      'progressPct': 0,
      'origin': {'name': 'Trạm A', 'latitude': 10.8, 'longitude': 106.7},
      'destination': {'name': 'Trạm B', 'latitude': 10.9, 'longitude': 106.8},
    });
    expect(t.progressPct, 0.0);
    expect(t.destination.name, 'Trạm B');

    final m = t.moved({'latitude': 10.85, 'longitude': 106.75, 'progressPct': 50});
    expect(m.progressPct, 50.0);
    expect(m.latitude, 10.85);
    expect(m.droneCode, 'DRONE-001'); // event không có droneCode thì giữ mã cũ
    expect(m.origin.name, 'Trạm A');
  });

  test('AiSummary phân biệt AI viết và bản tự ghép', () {
    final ai = AiSummary.fromJson({'text': 'Đơn giao xong.', 'source': 'llm', 'model': 'gpt-4o-mini'});
    expect(ai.byLlm, true);
    expect(ai.model, 'gpt-4o-mini');

    final rule = AiSummary.fromJson({'text': 'Đơn giao xong.', 'source': 'rule', 'note': 'Chưa cấu hình LLM API'});
    expect(rule.byLlm, false);
    expect(rule.note, isNotNull);
  });

  test('UserProfile đọc hồ sơ, số điện thoại có thể trống', () {
    final p = UserProfile.fromJson({'id': 'u1', 'email': 'customer@drone.local', 'fullName': 'Khách Demo', 'phone': null});
    expect(p.fullName, 'Khách Demo');
    expect(p.phone, isNull);
  });
}
