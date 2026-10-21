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
