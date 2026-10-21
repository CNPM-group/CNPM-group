import 'package:flutter_test/flutter_test.dart';
import 'package:smart_drone_mobile/format.dart';

void main() {
  test('statusLabel trả về tiếng Việt, trạng thái lạ giữ nguyên', () {
    expect(statusLabel('Pending'), 'Chờ duyệt');
    expect(statusLabel('Completed'), 'Hoàn tất');
    expect(statusLabel('Abc'), 'Abc');
  });

  test('formatDate xử lý null', () {
    expect(formatDate(null), '-');
  });
}
