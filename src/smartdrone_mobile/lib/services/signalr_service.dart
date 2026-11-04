
import 'dart:async';
import 'package:signalr_netcore/signalr_client.dart';

class SignalRService {
  HubConnection? _connection;

  final StreamController<Map<String, dynamic>> _updates =
      StreamController<Map<String, dynamic>>.broadcast();

  Stream<Map<String, dynamic>> get updates => _updates.stream;

  Future<void> connect({
    required String hubUrl,
    required String eventName,
  }) async {
    _connection = HubConnectionBuilder()
        .withUrl(hubUrl)
        .withAutomaticReconnect()
        .build();

    _connection!.on(eventName, (arguments) {
      if (arguments != null && arguments.isNotEmpty) {
        final data = arguments.first;

        if (data is Map) {
          _updates.add(Map<String, dynamic>.from(data));
        }
      }
    });

    await _connection!.start();
  }

  Future<void> disconnect() async {
    await _connection?.stop();
    _connection = null;
  }

  Future<void> dispose() async {
    await disconnect();
    await _updates.close();
  }
}
