import 'dart:math' as math;

import 'package:flutter/material.dart';

import 'models.dart';

/// Bản đồ mini tự vẽ (không cần gói bản đồ, không cần mạng): trạm đi, trạm đến, vị trí drone.
class TrackingMap extends StatelessWidget {
  final TrackingInfo info;

  const TrackingMap({super.key, required this.info});

  @override
  Widget build(BuildContext context) {
    final scheme = Theme.of(context).colorScheme;
    final pct = info.progressPct.round();
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        ClipRRect(
          borderRadius: BorderRadius.circular(12),
          child: SizedBox(
            height: 180,
            child: CustomPaint(
              painter: _MapPainter(
                info: info,
                route: scheme.outline,
                origin: scheme.primary,
                destination: Colors.orange.shade700,
                drone: scheme.tertiary,
                background: scheme.surfaceVariant,
                text: scheme.onSurfaceVariant,
              ),
            ),
          ),
        ),
        const SizedBox(height: 10),
        LinearProgressIndicator(value: info.progressPct / 100, minHeight: 8, borderRadius: BorderRadius.circular(4)),
        const SizedBox(height: 6),
        Text(
          info.status == 'InTransit'
              ? '${info.droneCode ?? 'Drone'} đã bay $pct%${pct >= 100 ? ' – đang chờ hạ cánh' : ''}'
              : 'Drone đã tới ${info.destination.name}',
        ),
      ],
    );
  }
}

class _MapPainter extends CustomPainter {
  final TrackingInfo info;
  final Color route, origin, destination, drone, background, text;

  _MapPainter({
    required this.info,
    required this.route,
    required this.origin,
    required this.destination,
    required this.drone,
    required this.background,
    required this.text,
  });

  @override
  void paint(Canvas canvas, Size size) {
    canvas.drawRect(Offset.zero & size, Paint()..color = background);

    const pad = 36.0;
    final a = info.origin, b = info.destination;
    final minLat = math.min(a.latitude, b.latitude), maxLat = math.max(a.latitude, b.latitude);
    final minLng = math.min(a.longitude, b.longitude), maxLng = math.max(a.longitude, b.longitude);
    // Giữ đúng tỉ lệ ngang/dọc để tuyến bay không bị méo.
    final span = math.max(math.max(maxLat - minLat, maxLng - minLng), 1e-6);
    final scale = math.min(size.width - 2 * pad, size.height - 2 * pad) / span;
    final cx = size.width / 2, cy = size.height / 2;
    final midLat = (minLat + maxLat) / 2, midLng = (minLng + maxLng) / 2;

    Offset project(double lat, double lng) =>
        Offset(cx + (lng - midLng) * scale, cy - (lat - midLat) * scale);

    final pa = project(a.latitude, a.longitude);
    final pb = project(b.latitude, b.longitude);
    final pd = project(info.latitude, info.longitude);

    canvas.drawLine(pa, pb, Paint()
      ..color = route
      ..strokeWidth = 3
      ..strokeCap = StrokeCap.round);
    canvas.drawLine(pa, pd, Paint()
      ..color = drone
      ..strokeWidth = 4
      ..strokeCap = StrokeCap.round);

    canvas.drawCircle(pa, 9, Paint()..color = origin);
    canvas.drawCircle(pb, 9, Paint()..color = destination);
    canvas.drawCircle(pd, 14, Paint()..color = drone.withOpacity(0.25));
    canvas.drawCircle(pd, 8, Paint()..color = drone);

    _label(canvas, size, a.name, pa);
    _label(canvas, size, b.name, pb);
  }

  void _label(Canvas canvas, Size size, String name, Offset at) {
    final tp = TextPainter(
      text: TextSpan(text: name, style: TextStyle(color: text, fontSize: 12, fontWeight: FontWeight.w600)),
      textDirection: TextDirection.ltr,
      maxLines: 1,
      ellipsis: '…',
    )..layout(maxWidth: size.width / 2);
    final dx = (at.dx - tp.width / 2).clamp(4.0, size.width - tp.width - 4).toDouble();
    final dy = at.dy + 14 + tp.height > size.height ? at.dy - 14 - tp.height : at.dy + 14;
    tp.paint(canvas, Offset(dx, dy));
  }

  @override
  bool shouldRepaint(covariant _MapPainter old) =>
      old.info.latitude != info.latitude || old.info.longitude != info.longitude || old.info.status != info.status;
}
