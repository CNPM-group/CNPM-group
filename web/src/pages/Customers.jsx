import { useCallback, useEffect, useState } from 'react';
import { api } from '../api';
import { useAuth } from '../auth';
import { formatDate } from '../format';
import { useToast } from '../components/Toast';
import Dialog from '../components/Dialog';
import StatusBadge from '../components/StatusBadge';

// FR03 (v0.11): nhân viên xem khách hàng, địa chỉ và lịch sử đơn của từng khách.
// Quản lý / Admin sửa được họ tên, số điện thoại của khách.
export default function Customers() {
  const { auth } = useAuth();
  const canEdit = auth.roles.some((r) => r === 'Manager' || r === 'Admin');
  const [customers, setCustomers] = useState([]);
  const [keyword, setKeyword] = useState('');
  const [error, setError] = useState('');
  const [detailId, setDetailId] = useState(null);
  const toast = useToast();

  const load = useCallback(async (q) => {
    try {
      setCustomers(await api.customers(q));
      setError('');
    } catch (e) {
      setError(e.message);
    }
  }, []);

  // Gõ xong 300 ms mới gọi API, tránh gọi mỗi phím.
  useEffect(() => {
    const t = setTimeout(() => load(keyword.trim()), 300);
    return () => clearTimeout(t);
  }, [keyword, load]);

  return (
    <section>
      <div className="page-head">
        <h2>Khách hàng</h2>
        <input
          style={{ width: 300, marginTop: 0 }}
          placeholder="Tìm theo tên, email, số điện thoại"
          value={keyword}
          onChange={(e) => setKeyword(e.target.value)}
        />
      </div>
      {error && <div className="error">{error}</div>}

      <div className="card">
        <table>
          <thead>
            <tr>
              <th>Họ tên</th>
              <th>Email</th>
              <th>Điện thoại</th>
              <th className="num">Tổng đơn</th>
              <th className="num">Hoàn tất</th>
              <th className="num">Đang xử lý</th>
              <th>Đơn gần nhất</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {customers.map((c) => (
              <tr key={c.id} className={c.isActive ? '' : 'inactive'}>
                <td>{c.fullName}</td>
                <td>{c.email}</td>
                <td>{c.phone ?? '-'}</td>
                <td className="num">{c.totalOrders}</td>
                <td className="num">{c.completedOrders}</td>
                <td className="num">{c.activeOrders}</td>
                <td>{formatDate(c.lastOrderAtUtc)}</td>
                <td className="actions">
                  <button className="ghost small" onClick={() => setDetailId(c.id)}>
                    Chi tiết
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {customers.length === 0 && !error && <p className="empty">Không có khách nào khớp.</p>}
      </div>

      {detailId && (
        <CustomerDialog
          id={detailId}
          canEdit={canEdit}
          onClose={() => setDetailId(null)}
          onSaved={(c) => {
            toast.ok(`Đã lưu thông tin ${c.fullName}`);
            load(keyword.trim());
          }}
        />
      )}
      {toast.view}
    </section>
  );
}

function CustomerDialog({ id, canEdit, onClose, onSaved }) {
  const [detail, setDetail] = useState(null);
  const [form, setForm] = useState(null);
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    api
      .customer(id)
      .then((d) => {
        setDetail(d);
        setForm({ fullName: d.customer.fullName, phone: d.customer.phone ?? '' });
      })
      .catch((e) => setError(e.message));
  }, [id]);

  async function save() {
    if (!canEdit) return onClose();
    setBusy(true);
    try {
      const saved = await api.updateCustomer(id, { fullName: form.fullName.trim(), phone: form.phone.trim() || null });
      onSaved(saved);
      onClose();
    } catch (e) {
      setError(e.message);
    } finally {
      setBusy(false);
    }
  }

  const c = detail?.customer;
  return (
    <Dialog
      title={c ? `Khách hàng – ${c.fullName}` : 'Khách hàng'}
      submitText={canEdit ? 'Lưu' : 'Đóng'}
      cancelText={canEdit ? 'Hủy' : 'Đóng'}
      busy={busy || !detail}
      onClose={onClose}
      onSubmit={save}
    >
      {error && <div className="error">{error}</div>}
      {detail && (
        <div className="customer-detail">
          <p className="muted">
            {c.email} · tạo lúc {formatDate(c.createdAtUtc)} · {c.totalOrders} đơn ({c.completedOrders} hoàn tất)
          </p>
          <label>
            Họ tên
            <input disabled={!canEdit} required value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} />
          </label>
          <label>
            Số điện thoại
            <input disabled={!canEdit} inputMode="tel" value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} />
          </label>

          <h4>Địa chỉ ({detail.addresses.length})</h4>
          {detail.addresses.length === 0 && <p className="muted">Chưa lưu địa chỉ nào.</p>}
          <ul className="plain-list">
            {detail.addresses.map((a) => (
              <li key={a.id}>
                <b>{a.label}</b>: {a.addressLine} {a.isDefault && <span className="role-chip">Mặc định</span>}
              </li>
            ))}
          </ul>

          <h4>Đơn gần đây ({detail.recentOrders.length})</h4>
          {detail.recentOrders.length === 0 && <p className="muted">Chưa có đơn.</p>}
          <table className="compact">
            <tbody>
              {detail.recentOrders.map((o) => (
                <tr key={o.id}>
                  <td className="mono">{o.trackingCode}</td>
                  <td>
                    {o.originStationName} → {o.destinationStationName}
                  </td>
                  <td>
                    <StatusBadge status={o.status} />
                  </td>
                  <td>{formatDate(o.createdAtUtc)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </Dialog>
  );
}
