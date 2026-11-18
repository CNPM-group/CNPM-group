import { useState } from 'react';
import { NavLink } from 'react-router-dom';
import { pagesFor, useAuth } from '../auth';
import ProfileDialog from './ProfileDialog';

export default function Layout({ children }) {
  const { auth, logout } = useAuth();
  const [profileOpen, setProfileOpen] = useState(false);

  return (
    <div className="app">
      <header className="topbar">
        <div className="brand">SmartDrone</div>
        <nav>
          {pagesFor(auth.roles).map((p) => (
            <NavLink key={p.path} to={p.path} className={({ isActive }) => (isActive ? 'active' : '')}>
              {p.label}
            </NavLink>
          ))}
        </nav>
        <div className="user">
          <button className="link-button" title="Sửa họ tên, số điện thoại" onClick={() => setProfileOpen(true)}>
            {auth.fullName} · {auth.roles.join(', ')}
          </button>
          <button className="ghost" onClick={logout}>
            Đăng xuất
          </button>
        </div>
      </header>
      <main className="content">{children}</main>
      {profileOpen && <ProfileDialog onClose={() => setProfileOpen(false)} />}
    </div>
  );
}
