export const STATUS_LABELS = {
  Pending: 'Chờ duyệt',
  Approved: 'Đã duyệt',
  Rejected: 'Bị từ chối',
  Scheduled: 'Đã gán drone',
  InTransit: 'Đang bay',
  ArrivedAtStation: 'Đã tới trạm',
  Completed: 'Hoàn tất',
  Failed: 'Thất bại',
  Cancelled: 'Đã hủy',
};

export function statusLabel(status) {
  return STATUS_LABELS[status] ?? status;
}

export function formatDate(value) {
  if (!value) return '-';
  return new Date(value).toLocaleString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
  });
}
