// Kiểu dữ liệu khớp docs/openapi.yaml

DateTime? _date(dynamic v) => v == null ? null : DateTime.parse(v as String);

class AuthData {
  final String token;
  final DateTime expiresAtUtc;
  final String userId;
  final String email;
  final String fullName;
  final List<String> roles;

  AuthData({
    required this.token,
    required this.expiresAtUtc,
    required this.userId,
    required this.email,
    required this.fullName,
    required this.roles,
  });

  factory AuthData.fromJson(Map<String, dynamic> j) => AuthData(
        token: j['token'] as String,
        expiresAtUtc: DateTime.parse(j['expiresAtUtc'] as String),
        userId: j['userId'] as String,
        email: j['email'] as String,
        fullName: j['fullName'] as String,
        roles: (j['roles'] as List).map((e) => e as String).toList(),
      );

  Map<String, dynamic> toJson() => {
        'token': token,
        'expiresAtUtc': expiresAtUtc.toUtc().toIso8601String(),
        'userId': userId,
        'email': email,
        'fullName': fullName,
        'roles': roles,
      };
}

class Station {
  final String id;
  final String name;
  final double latitude;
  final double longitude;
  final String status;

  Station({
    required this.id,
    required this.name,
    required this.latitude,
    required this.longitude,
    required this.status,
  });

  factory Station.fromJson(Map<String, dynamic> j) => Station(
        id: j['id'] as String,
        name: j['name'] as String,
        latitude: (j['latitude'] as num).toDouble(),
        longitude: (j['longitude'] as num).toDouble(),
        status: j['status'] as String,
      );
}

class PackageInfo {
  final String id;
  final String? description;
  final double weightKg;
  final double lengthCm;
  final double widthCm;
  final double heightCm;
  final String? imageKey;

  PackageInfo({
    required this.id,
    required this.description,
    required this.weightKg,
    required this.lengthCm,
    required this.widthCm,
    required this.heightCm,
    required this.imageKey,
  });

  factory PackageInfo.fromJson(Map<String, dynamic> j) => PackageInfo(
        id: j['id'] as String,
        description: j['description'] as String?,
        weightKg: (j['weightKg'] as num).toDouble(),
        lengthCm: (j['lengthCm'] as num).toDouble(),
        widthCm: (j['widthCm'] as num).toDouble(),
        heightCm: (j['heightCm'] as num).toDouble(),
        imageKey: j['imageKey'] as String?,
      );
}

class Order {
  final String id;
  final String trackingCode;
  final String status;
  final String originStationName;
  final String destinationStationName;
  final PackageInfo package;
  final DateTime? eta;
  final DateTime? approvedAtUtc;
  final String? failureReason;
  final String? notes;
  final DateTime createdAtUtc;
  final String? recipientName;
  final String? recipientPhone;
  final String? originStationId;
  final String? destinationStationId;
  final String? deliveryAddressId;

  Order({
    required this.id,
    required this.trackingCode,
    required this.status,
    required this.originStationName,
    required this.destinationStationName,
    required this.package,
    required this.eta,
    required this.approvedAtUtc,
    required this.failureReason,
    required this.notes,
    required this.createdAtUtc,
    this.recipientName,
    this.recipientPhone,
    this.originStationId,
    this.destinationStationId,
    this.deliveryAddressId,
  });

  factory Order.fromJson(Map<String, dynamic> j) => Order(
        id: j['id'] as String,
        trackingCode: j['trackingCode'] as String,
        status: j['status'] as String,
        originStationName: (j['originStation'] as Map<String, dynamic>)['name'] as String,
        destinationStationName: (j['destinationStation'] as Map<String, dynamic>)['name'] as String,
        package: PackageInfo.fromJson(j['package'] as Map<String, dynamic>),
        eta: _date(j['eta']),
        approvedAtUtc: _date(j['approvedAtUtc']),
        failureReason: j['failureReason'] as String?,
        notes: j['notes'] as String?,
        createdAtUtc: DateTime.parse(j['createdAtUtc'] as String),
        recipientName: j['recipientName'] as String?,
        recipientPhone: j['recipientPhone'] as String?,
        originStationId: (j['originStation'] as Map<String, dynamic>)['id'] as String?,
        destinationStationId: (j['destinationStation'] as Map<String, dynamic>)['id'] as String?,
        deliveryAddressId: j['deliveryAddressId'] as String?,
      );
}

class OrderSummary {
  final String id;
  final String trackingCode;
  final String status;
  final String originStationName;
  final String destinationStationName;
  final DateTime? eta;
  final DateTime createdAtUtc;

  OrderSummary({
    required this.id,
    required this.trackingCode,
    required this.status,
    required this.originStationName,
    required this.destinationStationName,
    required this.eta,
    required this.createdAtUtc,
  });

  factory OrderSummary.fromJson(Map<String, dynamic> j) => OrderSummary(
        id: j['id'] as String,
        trackingCode: j['trackingCode'] as String,
        status: j['status'] as String,
        originStationName: j['originStationName'] as String,
        destinationStationName: j['destinationStationName'] as String,
        eta: _date(j['eta']),
        createdAtUtc: DateTime.parse(j['createdAtUtc'] as String),
      );
}

class OrderPage {
  final List<OrderSummary> items;
  final int page;
  final int pageSize;
  final int totalItems;

  OrderPage({required this.items, required this.page, required this.pageSize, required this.totalItems});
}

class HistoryItem {
  final String? fromStatus;
  final String toStatus;
  final String? changedByName;
  final String? note;
  final DateTime changedAtUtc;

  HistoryItem({
    required this.fromStatus,
    required this.toStatus,
    required this.changedByName,
    required this.note,
    required this.changedAtUtc,
  });

  factory HistoryItem.fromJson(Map<String, dynamic> j) => HistoryItem(
        fromStatus: j['fromStatus'] as String?,
        toStatus: j['toStatus'] as String,
        changedByName: j['changedByName'] as String?,
        note: j['note'] as String?,
        changedAtUtc: DateTime.parse(j['changedAtUtc'] as String),
      );
}

class AppNotification {
  final String id;
  final String? orderId;
  final String title;
  final String? body;
  final bool isRead;
  final DateTime createdAtUtc;

  AppNotification({
    required this.id,
    required this.orderId,
    required this.title,
    required this.body,
    required this.isRead,
    required this.createdAtUtc,
  });

  factory AppNotification.fromJson(Map<String, dynamic> j) => AppNotification(
        id: j['id'] as String,
        orderId: j['orderId'] as String?,
        title: j['title'] as String,
        body: j['body'] as String?,
        isRead: j['isRead'] as bool,
        createdAtUtc: DateTime.parse(j['createdAtUtc'] as String),
      );

  /// Mã OTP 6 số nằm trong nội dung thông báo "Hàng đã tới trạm".
  String? get otp => RegExp(r'(?<!\w)\d{6}(?!\w)').firstMatch(body ?? '')?.group(0);
}

class CustomerAddress {
  final String id;
  final String label;
  final String addressLine;
  final double? latitude;
  final double? longitude;
  final bool isDefault;

  CustomerAddress({
    required this.id,
    required this.label,
    required this.addressLine,
    required this.latitude,
    required this.longitude,
    required this.isDefault,
  });

  factory CustomerAddress.fromJson(Map<String, dynamic> j) => CustomerAddress(
        id: j['id'] as String,
        label: j['label'] as String,
        addressLine: j['addressLine'] as String,
        latitude: (j['latitude'] as num?)?.toDouble(),
        longitude: (j['longitude'] as num?)?.toDouble(),
        isDefault: j['isDefault'] as bool,
      );
}

class StationPoint {
  final String name;
  final double latitude;
  final double longitude;

  StationPoint({required this.name, required this.latitude, required this.longitude});

  factory StationPoint.fromJson(Map<String, dynamic> j) => StationPoint(
        name: j['name'] as String,
        latitude: (j['latitude'] as num).toDouble(),
        longitude: (j['longitude'] as num).toDouble(),
      );
}

class TrackingInfo {
  final String orderId;
  final String status;
  final String? droneCode;
  final double latitude;
  final double longitude;
  final double progressPct;
  final StationPoint origin;
  final StationPoint destination;

  TrackingInfo({
    required this.orderId,
    required this.status,
    required this.droneCode,
    required this.latitude,
    required this.longitude,
    required this.progressPct,
    required this.origin,
    required this.destination,
  });

  factory TrackingInfo.fromJson(Map<String, dynamic> j) => TrackingInfo(
        orderId: j['orderId'] as String,
        status: j['status'] as String,
        droneCode: j['droneCode'] as String?,
        latitude: (j['latitude'] as num).toDouble(),
        longitude: (j['longitude'] as num).toDouble(),
        progressPct: (j['progressPct'] as num).toDouble(),
        origin: StationPoint.fromJson(j['origin'] as Map<String, dynamic>),
        destination: StationPoint.fromJson(j['destination'] as Map<String, dynamic>),
      );

  /// Áp vị trí mới từ event SignalR DroneLocation.
  TrackingInfo moved(Map<String, dynamic> e) => TrackingInfo(
        orderId: orderId,
        status: status,
        droneCode: (e['droneCode'] as String?) ?? droneCode,
        latitude: (e['latitude'] as num).toDouble(),
        longitude: (e['longitude'] as num).toDouble(),
        progressPct: (e['progressPct'] as num).toDouble(),
        origin: origin,
        destination: destination,
      );
}

/// Tóm tắt chuyến giao do AI viết (v0.10). source = 'llm' (AI viết) hoặc 'rule' (server tự ghép khi chưa có LLM).
class AiSummary {
  final String text;
  final String source;
  final String? model;
  final String? note;

  AiSummary({required this.text, required this.source, this.model, this.note});

  bool get byLlm => source == 'llm';

  factory AiSummary.fromJson(Map<String, dynamic> j) => AiSummary(
        text: (j['text'] as String?) ?? '',
        source: (j['source'] as String?) ?? 'rule',
        model: j['model'] as String?,
        note: j['note'] as String?,
      );
}

/// Hồ sơ của người đang đăng nhập (v0.11, GET/PUT /auth/me).
class UserProfile {
  final String id;
  final String email;
  final String fullName;
  final String? phone;

  UserProfile({required this.id, required this.email, required this.fullName, this.phone});

  factory UserProfile.fromJson(Map<String, dynamic> j) => UserProfile(
        id: j['id'] as String,
        email: j['email'] as String,
        fullName: j['fullName'] as String,
        phone: j['phone'] as String?,
      );
}
