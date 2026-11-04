import '../config.dart';

class SignalRConfig {
  static String get hubUrl => '${apiBaseUrl.replaceFirst('/api/v1', '')}/hubs/delivery';
  static const String eventName = 'OrderStatusUpdated';
}
