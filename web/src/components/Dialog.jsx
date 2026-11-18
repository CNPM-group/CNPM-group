export default function Dialog({ title, children, onClose, onSubmit, submitText = 'Xác nhận', cancelText = 'Hủy', busy, danger }) {
  function submit(e) {
    e.preventDefault();
    onSubmit();
  }

  return (
    <div className="overlay" onMouseDown={onClose}>
      <form className="dialog" onMouseDown={(e) => e.stopPropagation()} onSubmit={submit}>
        <h3>{title}</h3>
        {children}
        <div className="dialog-actions">
          <button type="button" className="ghost" onClick={onClose}>
            {cancelText}
          </button>
          <button type="submit" className={danger ? 'danger' : ''} disabled={busy}>
            {busy ? 'Đang xử lý...' : submitText}
          </button>
        </div>
      </form>
    </div>
  );
}
