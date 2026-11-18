import { useCallback, useEffect, useState } from 'react';
import { api } from '../api';
import { useAuth } from '../auth';
import { formatDate } from '../format';
import { useToast } from '../components/Toast';
import Dialog from '../components/Dialog';

const ROLES = {
  Customer: 'Khách hàng',
  Dispatcher: 'Điều phối viên',
  StationOperator: 'Nhân viên trạm',
  Manager: 'Quản lý',
  Admin: 'Admin',
};

export default function Users() {
  const { auth } = useAuth();
  const [users, setUsers] = useState([]);
  const [filter, setFilter] = useState('');
  const [error, setError] = useState('');
  const [dialog, setDialog] = useState(null);
  const [busy, setBusy] = useState(false);
  const toast = useToast();

  const load = useCallback(async () => {
    try {
      setUsers(await api.users());
      setError('');
    } catch (e) {
      setError(e.message);
    }
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  async function save(action, doneText) {
    setBusy(true);
    try {
      await action();
      toast.ok(doneText);
      setDialog(null);
      await load();
    } catch (e) {
      toast.error(e.message);
    } finally {
      setBusy(false);
    }
  }

  const keyword = filter.trim().toLowerCase();
  const shown = users.filter(
    (u) => !keyword || u.email.toLowerCase().includes(keyword) || u.fullName.toLowerCase().includes(keyword),
  );

  return (
    <section>
      <div className="page-head">
        <h2>Tài khoản</h2>
        <input
          style={{ width: 280, marginTop: 0 }}
          placeholder="Tìm theo email hoặc tên"
          value={filter}
          onChange={(e) => setFilter(e.target.value)}
        />
        <button onClick={() => setDialog({ kind: 'create' })}>+ Tạo tài khoản</button>
      </div>
      {error && <div className="error">{error}</div>}

      <div className="card">
        <table>
          <thead>
            <tr>
              <th>Email</th>
              <th>Họ tên</th>
              <th>Vai trò</th>
              <th>Tạo lúc</th>
              <th>Trạng thái</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {shown.map((u) => {
              const self = u.id === auth.userId;
              return (
                <tr key={u.id} className={u.isActive ? '' : 'inactive'}>
                  <td>{u.email}</td>
                  <td>{u.fullName}</td>
                  <td>
                    <div className="role-list">
                      {u.roles.map((r) => (
                        <span key={r} className="role-chip">
                          {ROLES[r] ?? r}
                        </span>
                      ))}
                    </div>
                  </td>
                  <td>{formatDate(u.createdAtUtc)}</td>
                  <td>{u.isActive ? 'Đang dùng' : 'Đã khóa'}</td>
                  <td className="actions">
                    <button className="ghost small" disabled={self} onClick={() => setDialog({ kind: 'roles', user: u })}>
                      Đổi vai trò
                    </button>
                    <button
                      className={u.isActive ? 'danger small' : 'small'}
                      disabled={self || busy}
                      title={self ? 'Không tự khóa tài khoản đang đăng nhập' : ''}
                      onClick={() =>
                        save(
                          () => api.setActive(u.id, !u.isActive),
                          `${u.email}: ${u.isActive ? 'đã khóa' : 'đã mở lại'}`,
                        )
                      }
                    >
                      {u.isActive ? 'Khóa' : 'Mở khóa'}
                    </button>
                  </td>
                </tr>
              );
            })}
          </tbody>
        </table>
        {shown.length === 0 && <p className="empty">Không có tài khoản nào.</p>}
      </div>

      {dialog?.kind === 'create' && (
        <CreateDialog
          busy={busy}
          onClose={() => setDialog(null)}
          onSubmit={(body) => save(() => api.createUser(body), `Đã tạo ${body.email}`)}
        />
      )}
      {dialog?.kind === 'roles' && (
        <RolesDialog
          user={dialog.user}
          busy={busy}
          onClose={() => setDialog(null)}
          onSubmit={(roles) => save(() => api.setRoles(dialog.user.id, roles), `Đã đổi vai trò ${dialog.user.email}`)}
        />
      )}

      {toast.view}
    </section>
  );
}

function CreateDialog({ busy, onClose, onSubmit }) {
  const [form, setForm] = useState({ email: '', fullName: '', password: '', role: 'Dispatcher' });
  const bind = (name) => ({
    value: form[name],
    onChange: (e) => setForm((f) => ({ ...f, [name]: e.target.value })),
  });

  return (
    <Dialog
      title="Tạo tài khoản"
      submitText="Tạo"
      busy={busy}
      onClose={onClose}
      onSubmit={() => onSubmit({ ...form, email: form.email.trim(), fullName: form.fullName.trim() })}
    >
      <label>
        Email
        <input type="email" required maxLength={255} {...bind('email')} />
      </label>
      <label>
        Họ tên
        <input required maxLength={100} {...bind('fullName')} />
      </label>
      <label>
        Mật khẩu (ít nhất 8 ký tự)
        <input type="password" required minLength={8} {...bind('password')} />
      </label>
      <label>
        Vai trò
        <select {...bind('role')}>
          {Object.entries(ROLES).map(([value, label]) => (
            <option key={value} value={value}>
              {label}
            </option>
          ))}
        </select>
      </label>
    </Dialog>
  );
}

function RolesDialog({ user, busy, onClose, onSubmit }) {
  const [roles, setRoles] = useState(user.roles);

  function toggle(role) {
    setRoles((prev) => (prev.includes(role) ? prev.filter((r) => r !== role) : [...prev, role]));
  }

  return (
    <Dialog
      title={`Vai trò của ${user.email}`}
      submitText="Lưu"
      busy={busy || roles.length === 0}
      onClose={onClose}
      onSubmit={() => roles.length > 0 && onSubmit(roles)}
    >
      <div className="check-list">
        {Object.entries(ROLES).map(([value, label]) => (
          <label key={value}>
            <input type="checkbox" checked={roles.includes(value)} onChange={() => toggle(value)} />
            {label}
          </label>
        ))}
      </div>
      {roles.length === 0 && <p className="muted">Phải chọn ít nhất 1 vai trò.</p>}
    </Dialog>
  );
}
