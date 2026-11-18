import { useCallback, useEffect, useState } from 'react';
import { api } from '../api';
import { formatDate } from '../format';

// UC-29 (v0.11): Admin xem ai đã thêm / sửa / xóa gì. Server tự ghi khi lưu dữ liệu (AuditInterceptor).
const ENTITIES = {
  '': 'Tất cả',
  DeliveryOrder: 'Đơn hàng',
  Package: 'Kiện hàng',
  Drone: 'Drone',
  DroneFlight: 'Chuyến bay',
  LandingStation: 'Trạm',
  DeliveryConfirmation: 'Xác nhận OTP',
  CustomerAddress: 'Địa chỉ',
  AppUser: 'Tài khoản',
  'IdentityUserRole`1': 'Vai trò tài khoản',
};

const ACTIONS = { Added: 'Thêm', Modified: 'Sửa', Deleted: 'Xóa' };
const PAGE_SIZE = 50;

export default function Audit() {
  const [entity, setEntity] = useState('');
  const [page, setPage] = useState(1);
  const [data, setData] = useState(null);
  const [error, setError] = useState('');

  const load = useCallback(async () => {
    try {
      setData(await api.auditLogs({ entity, page, pageSize: PAGE_SIZE }));
      setError('');
    } catch (e) {
      setError(e.message);
    }
  }, [entity, page]);

  useEffect(() => {
    load();
  }, [load]);

  const pages = data ? Math.max(1, Math.ceil(data.totalItems / PAGE_SIZE)) : 1;

  return (
    <section>
      <div className="page-head">
        <h2>Nhật ký thao tác</h2>
        <select
          style={{ width: 220, marginTop: 0 }}
          value={entity}
          onChange={(e) => {
            setEntity(e.target.value);
            setPage(1);
          }}
        >
          {Object.entries(ENTITIES).map(([value, label]) => (
            <option key={value} value={value}>
              {label}
            </option>
          ))}
        </select>
      </div>
      {error && <div className="error">{error}</div>}

      <div className="card">
        <table className="audit">
          <thead>
            <tr>
              <th>Lúc</th>
              <th>Người làm</th>
              <th>Thao tác</th>
              <th>Dữ liệu</th>
              <th>Thay đổi</th>
            </tr>
          </thead>
          <tbody>
            {data?.items.map((a) => (
              <tr key={a.id}>
                <td>{formatDate(a.createdAtUtc)}</td>
                <td>{a.userEmail ?? '-'}</td>
                <td>
                  <span className={`audit-action audit-${a.action}`}>{ACTIONS[a.action] ?? a.action}</span>
                </td>
                <td>
                  {ENTITIES[a.entityName] ?? a.entityName}
                  <div className="muted mono small-id">{a.entityId}</div>
                </td>
                <td>
                  <Changes oldJson={a.oldValues} newJson={a.newValues} />
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {data && data.items.length === 0 && <p className="empty">Chưa có nhật ký.</p>}
        <div className="pager">
          <button className="ghost small" disabled={page <= 1} onClick={() => setPage(page - 1)}>
            ‹ Trước
          </button>
          <span className="muted">
            Trang {page}/{pages} · {data?.totalItems ?? 0} dòng
          </span>
          <button className="ghost small" disabled={page >= pages} onClick={() => setPage(page + 1)}>
            Sau ›
          </button>
        </div>
      </div>
    </section>
  );
}

// Sửa: hiện "trường: cũ → mới". Thêm / xóa: hiện vài trường đầu cho gọn.
function Changes({ oldJson, newJson }) {
  const before = parse(oldJson);
  const after = parse(newJson);
  const keys = Object.keys(after ?? before ?? {});
  if (keys.length === 0) return null;
  const shown = before && after ? keys : keys.filter((k) => !/At(Utc)?$|^Id$/.test(k)).slice(0, 4);
  return (
    <ul className="changes">
      {shown.map((k) => (
        <li key={k}>
          <b>{k}</b>: {before && after ? `${show(before[k])} → ${show(after[k])}` : show((after ?? before)[k])}
        </li>
      ))}
    </ul>
  );
}

function parse(json) {
  try {
    return json ? JSON.parse(json) : null;
  } catch {
    return null;
  }
}

function show(v) {
  if (v === null || v === undefined) return '∅';
  const s = typeof v === 'object' ? JSON.stringify(v) : String(v);
  return s.length > 60 ? `${s.slice(0, 60)}…` : s;
}
