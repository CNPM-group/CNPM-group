import 'package:flutter/material.dart';

String formatDate(DateTime? d) {
  if (d == null) return '-';
  final l = d.toLocal();
  String two(int n) => n.toString().padLeft(2, '0');
  return '${two(l.day)}/${two(l.month)}/${l.year} ${two(l.hour)}:${two(l.minute)}';
}

const Map<String, String> _statusLabels = {
  'Pending': 'Chờ duyệt',
  'Approved': 'Đã duyệt',
  'Rejected': 'Bị từ chối',
  'Scheduled': 'Đã lên lịch',
  'InTransit': 'Đang giao',
  'ArrivedAtStation': 'Đã tới trạm',
  'Completed': 'Hoàn tất',
  'Failed': 'Giao thất bại',
  'Cancelled': 'Đã hủy',
};

String statusLabel(String status) => _statusLabels[status] ?? status;

Color statusColor(String status) {
  switch (status) {
    case 'Pending':
      return Colors.orange;
    case 'Approved':
    case 'ArrivedAtStation':
      return Colors.blue;
    case 'Scheduled':
    case 'InTransit':
      return Colors.deepPurple;
    case 'Completed':
      return Colors.green;
    case 'Rejected':
    case 'Failed':
      return Colors.red;
    default:
      return Colors.grey;
  }
}
