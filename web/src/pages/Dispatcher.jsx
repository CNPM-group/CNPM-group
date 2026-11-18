import { useCallback, useEffect, useState } from 'react';
import { api } from '../api';
import { formatDate } from '../format';
import { useOrderEvents } from '../orderEvents';
import { useToast } from '../components/Toast';
import Dialog from '../components/Dialog';
import AiSummaryDialog from '../components/AiSummaryDialog';
import LiveDot from '../components/LiveDot';
import StatusBadge from '../components/StatusBadge';

const TABS = [
  { status: 'Pending', label: 'Chờ duyệt' },
  { status: 'Approved', label: 'Đã duyệt' },
  { status: 'Scheduled', label: 'Đã gán drone' },
  { status: 'InTransit', label: 'Đang bay' },
  { status: 'ArrivedAtStation', label: 'Tới trạm' },
  { status: 'Failed', label: 'Thất bại' },
  { status: 'Completed', label: 'Hoàn tất' },
  { status: '', label: 'Tất cả' },
];

export default function Dispatcher() {
  const [tab, setTab] = useState('Pending');
  const [search, setSearch] = useState('');
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [dialog, setDialog] = useState(null);
  const [busyId, setBusyId] = useState(null);
  const toast = useToast();

  const load = useCallback(async () => {
    try {
      const page = await api.orders({ status: tab, q: search.trim(), pageSize: 100 });
      setOrders(page.items);
      setError('');
    } catch (e) {
      setError(e.message);
    } finally {
      setLoading(false);
    }
  }, [tab, search]);

  // Tìm theo mã đơn (FR05): gõ xong 300 ms mới gọi API.
  useEffect(() => {
    setLoading(true);
    const t = setTimeout(load, 300);
    return () => clearTimeout(t);
  }, [load]);

  const live = useOrderEvents(() => load());

  async function run(order, action, doneText) {
    setBusyId(order.id);
    try {
      await action();
      toast.ok(`${order.trackingCode}: ${doneText}`);
      setDialog(null);
      await load();
    } catch (e) {
      toast.error(e.message);
    } finally {
      setBusyId(null);
    }
  }

  function actions(o) {
    const busy = busyId === o.id;
    switch (o.status) {
      case 'Pending':
        return (
          <>
            <button disabled={busy} onClick={() => run(o, () => api.approve(o.id), 'đã duyệt')}>
              Duyệt
            </button>
            <button className="danger" disabled={busy} onClick={() => setDialog({ kind: 'reject', order: o })}>
              Từ chối
            </button>
          </>
        );
      case 'Approved':
        return (
          <button disabled={busy} onClick={() => setDialog({ kind: 'schedule', order: o })}>
            Gán drone
          </button>
        );
      case 'Failed':
        return (
          <>
            <button disabled={busy} onClick={() => setDialog({ kind: 'schedule', order: o })}>
              Giao lại
            </button>
            <button className="danger" disabled={busy} onClick={() => setDialog({ kind: 'return', order: o })}>
              Hủy & hoàn
            </button>
          </>
        );
      case 'Scheduled':
        return (
          <button disabled={busy} onClick={() => run(o, () => api.start(o.id), 'đã cất cánh')}>
            Cất cánh
          </button>
        );
      case 'InTransit':
      case 'ArrivedAtStation':
        return (
          <button className="danger" disabled={busy} onClick={() => setDialog({ kind: 'fail', order: o })}>
            Báo lỗi
          </button>
        );
      default:
        return null;
    }
  }

  const close = () => setDialog(null);
  const dialogBusy = dialog && busyId === dialog.order.id;

  return (
    <section>
      <div className="page-head">
        <h2>Điều phối đơn hàng</h2>
        <input
          style={{ width: 240, marginTop: 0 }}
          placeholder="Tìm mã đơn, VD: SDD-7f3a"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
        <LiveDot live={live} />
      </div>

      <div className="tabs">
        {TABS.map((t) => (
          <button key={t.label} className={t.status === tab ? 'tab active' : 'tab'} onClick={() => setTab(t.status)}>
            {t.label}
          </button>
        ))}
      </div>

      {error && <div className="error">{error}</div>}

      <div className="card">
        <table>
          <thead>
            <tr>
              <th>Mã đơn</th>
              <th>Tuyến</th>
              <th>Trạng thái</th>
              <th>ETA</th>
              <th>Tạo lúc</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {orders.map((o) => (
              <tr key={o.id}>
                <td className="mono">{o.trackingCode}</td>
                <td>
                  {o.originStationName} → {o.destinationStationName}
                </td>
                <td>
                  <StatusBadge status={o.status} />
                </td>
                <td>{formatDate(o.eta)}</td>
                <td>{formatDate(o.createdAtUtc)}</td>
                <td className="actions">
                  {actions(o)}
                  <button className="ghost" title="AI tóm tắt chuyến giao" onClick={() => setDialog({ kind: 'ai', order: o })}>
                    Tóm tắt AI
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {!loading && orders.length === 0 && (
          <p className="empty">{search.trim() ? `Không có đơn nào khớp "${search.trim()}" ở mục này.` : 'Không có đơn nào ở mục này.'}</p>
        )}
        {loading && <p className="empty">Đang tải...</p>}
      </div>

      {dialog?.kind === 'reject' && (
        <ReasonDialog
          title={`Từ chối đơn ${dialog.order.trackingCode}`}
          busy={dialogBusy}
          onClose={close}
          onSubmit={(reason) => run(dialog.order, () => api.reject(dialog.order.id, reason), 'đã từ chối')}
        />
      )}
      {dialog?.kind === 'fail' && (
        <ReasonDialog
          title={`Báo giao thất bại – ${dialog.order.trackingCode}`}
          busy={dialogBusy}
          onClose={close}
          onSubmit={(reason) => run(dialog.order, () => api.fail(dialog.order.id, reason), 'đã chuyển sang thất bại')}
        />
      )}
      {dialog?.kind === 'return' && (
        <ReasonDialog
          title={`Hủy và hoàn trả đơn ${dialog.order.trackingCode}`}
          busy={dialogBusy}
          onClose={close}
          onSubmit={(reason) => run(dialog.order, () => api.returnOrder(dialog.order.id, reason), 'đã hủy, hoàn hàng về trạm gửi')}
        />
      )}
      {dialog?.kind === 'ai' && <AiSummaryDialog order={dialog.order} onClose={close} />}
      {dialog?.kind === 'schedule' && (
        <ScheduleDialog
          order={dialog.order}
          busy={dialogBusy}
          onClose={close}
          onSubmit={(droneId) => run(dialog.order, () => api.schedule(dialog.order.id, droneId), 'đã gán drone')}
        />
      )}

      {toast.view}
    </section>
  );
}

function ReasonDialog({ title, busy, onClose, onSubmit }) {
  const [reason, setReason] = useState('');

  return (
    <Dialog title={title} busy={busy} danger onClose={onClose} onSubmit={() => reason.trim() && onSubmit(reason.trim())}>
      <label>
        Lý do (bắt buộc)
        <textarea rows={3} value={reason} onChange={(e) => setReason(e.target.value)} required autoFocus />
      </label>
    </Dialog>
  );
}

function ScheduleDialog({ order, busy, onClose, onSubmit }) {
  const [drones, setDrones] = useState([]);
  const [weight, setWeight] = useState(null);
  const [droneId, setDroneId] = useState('');
  const [image, setImage] = useState(null);
  const [error, setError] = useState('');

  useEffect(() => {
    let url = null;
    Promise.all([api.drones(), api.order(order.id)])
      .then(([list, detail]) => {
        const kg = detail.package.weightKg;
        // Cùng luật với server (BR-12): rảnh, đủ tải, pin từ 30% trở lên. Tầm bay do server kiểm.
        const usable = list.filter((d) => d.status === 'Idle' && d.maxPayloadKg >= kg && d.batteryPct >= 30);
        setWeight(kg);
        setDrones(usable);
        setDroneId(usable[0]?.id ?? '');
        if (detail.package.imageKey) {
          api
            .packageImage(order.id)
            .then((blob) => {
              url = URL.createObjectURL(blob);
              setImage(url);
            })
            .catch(() => {});
        }
      })
      .catch((e) => setError(e.message));
    return () => url && URL.revokeObjectURL(url);
  }, [order.id]);

  return (
    <Dialog
      title={`Gán drone cho ${order.trackingCode}`}
      submitText="Gán drone"
      busy={busy || !droneId}
      onClose={onClose}
      onSubmit={() => droneId && onSubmit(droneId)}
    >
      <p className="muted">
        {order.originStationName} → {order.destinationStationName}
        {weight !== null && ` · kiện ${weight} kg`}
        {order.recipientName && ` · người nhận ${order.recipientName}${order.recipientPhone ? ` (${order.recipientPhone})` : ''}`}
      </p>
      {image && <img className="package-image" src={image} alt="Ảnh kiện hàng" />}
      {error && <div className="error">{error}</div>}
      {weight !== null && drones.length === 0 && (
        <div className="error">Không có drone nào đang rảnh, chở được {weight} kg và còn từ 30% pin.</div>
      )}
      {drones.length > 0 && (
        <label>
          Drone đang rảnh
          <select value={droneId} onChange={(e) => setDroneId(e.target.value)}>
            {drones.map((d) => (
              <option key={d.id} value={d.id}>
                {d.droneCode} – {d.model} – tải {d.maxPayloadKg} kg – pin {d.batteryPct}%
              </option>
            ))}
          </select>
        </label>
      )}
    </Dialog>
  );
}
