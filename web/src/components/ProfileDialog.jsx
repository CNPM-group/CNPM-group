import { useEffect, useState } from 'react';
import { api } from '../api';
import { useAuth } from '../auth';
import Dialog from './Dialog';

// FR01 (v0.11): tự sửa họ tên, số điện thoại. Mở từ tên người dùng trên thanh menu.
export default function ProfileDialog({ onClose }) {
  const { updateProfile } = useAuth();
  const [form, setForm] = useState(null);
  const [pw, setPw] = useState({ current: '', next: '' });
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    api
      .me()
      .then((me) => setForm({ email: me.email, fullName: me.fullName, phone: me.phone ?? '' }))
      .catch((e) => setError(e.message));
  }, []);

  async function save() {
    setBusy(true);
    try {
      const saved = await api.updateMe({ fullName: form.fullName.trim(), phone: form.phone.trim() || null });
      updateProfile(saved);
      // Đổi mật khẩu chỉ khi có nhập mật khẩu mới (UC-03).
      if (pw.next) await api.changePassword(pw.current, pw.next);
      onClose();
    } catch (e) {
      setError(e.message);
    } finally {
      setBusy(false);
    }
  }

  return (
    <Dialog title="Hồ sơ của tôi" submitText="Lưu" busy={busy || !form} onClose={onClose} onSubmit={save}>
      {error && <div className="error">{error}</div>}
      {form && (
        <>
          <label>
            Email (không đổi được)
            <input value={form.email} disabled />
          </label>
          <label>
            Họ tên
            <input required minLength={2} maxLength={100} value={form.fullName} onChange={(e) => setForm({ ...form, fullName: e.target.value })} />
          </label>
          <label>
            Số điện thoại
            <input
              inputMode="tel"
              maxLength={20}
              placeholder="VD: 0901234567"
              value={form.phone}
              onChange={(e) => setForm({ ...form, phone: e.target.value })}
            />
          </label>
          <h4 className="dialog-sub">Đổi mật khẩu (bỏ trống nếu không đổi)</h4>
          <div className="form-grid">
            <label>
              Mật khẩu hiện tại
              <input type="password" autoComplete="current-password" required={!!pw.next} value={pw.current} onChange={(e) => setPw({ ...pw, current: e.target.value })} />
            </label>
            <label>
              Mật khẩu mới (≥ 8 ký tự)
              <input type="password" autoComplete="new-password" minLength={8} value={pw.next} onChange={(e) => setPw({ ...pw, next: e.target.value })} />
            </label>
          </div>
        </>
      )}
    </Dialog>
  );
}
