# web/src/components

Khối giao diện dùng lại ở nhiều trang.

## Layout.jsx

Khung trang sau đăng nhập: thanh xanh trên cùng gồm tên app, menu (chỉ hiện trang mà role được mở, lấy từ `pagesFor`), họ tên + role và nút Đăng xuất. Trang con nằm trong `<main>`. Từ v0.11 bấm vào họ tên mở `ProfileDialog` để sửa hồ sơ.

## Dialog.jsx

Hộp thoại nổi giữa màn hình. Nhận `title`, nội dung bên trong, `onSubmit`, `onClose`, `submitText`, `cancelText` (mặc định "Hủy", v0.10), `busy` (khóa nút khi đang gửi), `danger` (nút đỏ).

- Là một `<form>` nên bấm Enter cũng gửi.
- Bấm ra ngoài vùng tối thì đóng; bấm bên trong thì không (chặn bằng `stopPropagation`).

`Dispatcher.jsx` dùng nó cho hộp thoại từ chối, báo lỗi, gán drone, hủy và hoàn; `AiSummaryDialog.jsx` dùng cho tóm tắt AI.

## Toast.jsx

Hook `useToast()` trả `{ view, ok(text), error(text) }`. Đặt `{toast.view}` ở cuối trang, gọi `toast.ok('...')` hoặc `toast.error('...')`. Thông báo nổi ở góc phải dưới, tự tắt sau 4 giây.

## StatusBadge.jsx

Nhãn tròn hiện trạng thái đơn bằng tiếng Việt. Màu theo class `.badge-<Trạng thái>` trong `styles.css`: cam = chờ duyệt, xanh dương = đã duyệt/tới trạm, tím = đã gán/đang bay, xanh lá = hoàn tất, đỏ = từ chối/thất bại, xám = đã hủy.

## LiveDot.jsx

Nhãn nhỏ cạnh tiêu đề trang cho biết đang nhận cập nhật kiểu gì: **Realtime** (nối được SignalR) hoặc **Tự làm mới 10s** (hub chưa chạy, xem `orderEvents.md`).

## AiSummaryDialog.jsx (v0.10)

Hộp thoại AI tóm tắt chuyến giao của 1 đơn. Chi tiết: [AiSummaryDialog.md](AiSummaryDialog.md).

## AiSource.jsx (v0.10)

Nhãn "AI viết · model" hoặc "Tự ghép từ dữ liệu" dưới nội dung AI. Chi tiết: [AiSource.md](AiSource.md).

## ProfileDialog.jsx (v0.11)

Hộp thoại sửa họ tên, số điện thoại của chính mình. Chi tiết: [ProfileDialog.md](ProfileDialog.md).

