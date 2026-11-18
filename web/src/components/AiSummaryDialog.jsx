import { useCallback, useEffect, useState } from 'react';
import { api } from '../api';
import Dialog from './Dialog';
import AiSource from './AiSource';

// Hộp thoại "Tóm tắt AI" của 1 đơn (KAN-62). Mở lên là gọi API ngay, bấm "Viết lại" để gọi lần nữa.
export default function AiSummaryDialog({ order, onClose }) {
  const [result, setResult] = useState(null);
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(true);

  const load = useCallback(async () => {
    setBusy(true);
    setError('');
    try {
      setResult(await api.aiSummary(order.id));
    } catch (e) {
      setError(e.message);
    } finally {
      setBusy(false);
    }
  }, [order.id]);

  useEffect(() => {
    load();
  }, [load]);

  return (
    <Dialog title={`Tóm tắt chuyến giao – ${order.trackingCode}`} submitText="Viết lại" cancelText="Đóng" busy={busy} onClose={onClose} onSubmit={load}>
      {busy && !result && <p className="muted">AI đang đọc dữ liệu đơn và viết tóm tắt...</p>}
      {error && <div className="error">{error}</div>}
      {result && (
        <div className="ai-box">
          <p className="ai-text">{result.text}</p>
          <AiSource source={result.source} model={result.model} note={result.note} />
        </div>
      )}
    </Dialog>
  );
}
