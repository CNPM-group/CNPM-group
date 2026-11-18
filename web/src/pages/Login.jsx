import { useState } from 'react';
import { Navigate, useNavigate } from 'react-router-dom';
import { homePath, useAuth } from '../auth';

const DEMO_PASSWORD = 'Demo@12345';
const DEMO_ACCOUNTS = [
  { email: 'dispatcher@drone.local', label: 'Điều phối viên' },
  { email: 'stationoperator@drone.local', label: 'Nhân viên trạm' },
  { email: 'manager@drone.local', label: 'Quản lý' },
  { email: 'admin@drone.local', label: 'Admin' },
];

export default function Login() {
  const { auth, login } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [busy, setBusy] = useState(false);

  if (auth) return <Navigate to={homePath(auth.roles)} replace />;

  async function submit(e) {
    e.preventDefault();
    setBusy(true);
    setError('');
    try {
      const data = await login(email.trim(), password);
      navigate(homePath(data.roles), { replace: true });
    } catch (err) {
      setError(err.message);
    } finally {
      setBusy(false);
    }
  }

  return (
    <div className="login-page">
      <form className="login-card" onSubmit={submit}>
        <h1>SmartDrone</h1>
        <p className="muted">Trang dành cho nhân viên điều phối, trạm và quản lý</p>

        <label>
          Email
          <input type="email" value={email} onChange={(e) => setEmail(e.target.value)} required autoFocus />
        </label>
        <label>
          Mật khẩu
          <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
        </label>

        {error && <div className="error">{error}</div>}

        <button type="submit" disabled={busy}>
          {busy ? 'Đang đăng nhập...' : 'Đăng nhập'}
        </button>

        <div className="demo">
          <span className="muted">Tài khoản demo:</span>
          {DEMO_ACCOUNTS.map((a) => (
            <button
              key={a.email}
              type="button"
              className="ghost small"
              onClick={() => {
                setEmail(a.email);
                setPassword(DEMO_PASSWORD);
              }}
            >
              {a.label}
            </button>
          ))}
        </div>
      </form>
    </div>
  );
}
