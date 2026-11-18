export default function LiveDot({ live }) {
  return (
    <span className={`live ${live ? 'on' : ''}`} title={live ? 'Đang nhận cập nhật realtime' : 'Tự làm mới mỗi 10 giây'}>
      {live ? 'Realtime' : 'Tự làm mới 10s'}
    </span>
  );
}
