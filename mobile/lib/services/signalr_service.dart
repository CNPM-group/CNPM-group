import 'dart:async';

import 'package:signalr_netcore/signalr_client.dart';

import '../auth_store.dart';
import 'signalr_config.dart';

/// Nhận event đổi trạng thái đơn từ hub. Hub chưa chạy hoặc mất kết nối thì
/// cứ 10 giây phát một event rỗng để các màn tự tải lại (polling dự phòng).
class SignalRService {
  SignalRService._();
  static final SignalRService instance = SignalRService._();

  static const _pollEvery = Duration(seconds: 10);

  HubConnection? _connection;
  Timer? _poll;

  final StreamController<Map<String, dynamic>> _updates =
      StreamController<Map<String, dynamic>>.broadcast();

  Stream<Map<String, dynamic>> get updates => _updates.stream;

  final StreamController<Map<String, dynamic>> _drone =
      StreamController<Map<String, dynamic>>.broadcast();

  /// Vị trí drone đang bay (event DroneLocation), phát mỗi vài giây khi đơn đang giao.
  Stream<Map<String, dynamic>> get drone => _drone.stream;

  bool get isLive => _connection?.state == HubConnectionState.Connected;

  Future<void> connect() async {
    if (_connection != null) return;

    final connection = HubConnectionBuilder()
        .withUrl(
          SignalRConfig.hubUrl,
          options: HttpConnectionOptions(
            accessTokenFactory: () async => AuthStore.instance.token ?? '',
          ),
        )
        .withAutomaticReconnect()
        .build();

    connection.on(SignalRConfig.eventName, (arguments) {
      final data = arguments?.isNotEmpty == true ? arguments!.first : null;
      if (data is Map) _updates.add(Map<String, dynamic>.from(data));
    });
    connection.on(SignalRConfig.droneEventName, (arguments) {
      final data = arguments?.isNotEmpty == true ? arguments!.first : null;
      if (data is Map) _drone.add(Map<String, dynamic>.from(data));
    });
    connection.onreconnected(({connectionId}) => _stopPolling());
    connection.onclose(({error}) {
      if (_connection != null) _startPolling();
    });

    _connection = connection;
    try {
      await connection.start();
      _stopPolling();
    } catch (_) {
      _startPolling();
    }
  }

  Future<void> disconnect() async {
    _stopPolling();
    final connection = _connection;
    _connection = null;
    try {
      await connection?.stop();
    } catch (_) {
      // đang tắt thì bỏ qua lỗi
    }
  }

  void _startPolling() {
    _poll ??= Timer.periodic(_pollEvery, (_) => _updates.add(<String, dynamic>{}));
  }

  void _stopPolling() {
    _poll?.cancel();
    _poll = null;
  }
}
