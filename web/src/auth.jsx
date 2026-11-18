import { createContext, useContext, useEffect, useState } from 'react';
import { api, clearAuth, loadAuth, saveAuth, setUnauthorizedHandler } from './api';

const PAGES = [
  { path: '/dispatch', label: 'Điều phối', roles: ['Dispatcher', 'Manager', 'Admin'] },
  { path: '/station', label: 'Trạm', roles: ['StationOperator', 'Dispatcher', 'Manager', 'Admin'] },
  { path: '/customers', label: 'Khách hàng', roles: ['Dispatcher', 'Manager', 'Admin'] },
  { path: '/dashboard', label: 'Thống kê', roles: ['Manager', 'Admin'] },
  { path: '/fleet', label: 'Drone & trạm', roles: ['Manager', 'Admin'] },
  { path: '/users', label: 'Tài khoản', roles: ['Admin'] },
  { path: '/audit', label: 'Nhật ký', roles: ['Admin'] },
];

export function pagesFor(roles = []) {
  return PAGES.filter((p) => p.roles.some((r) => roles.includes(r)));
}

export function canOpen(path, roles = []) {
  return pagesFor(roles).some((p) => p.path === path);
}

export function homePath(roles = []) {
  if (roles.includes('Manager') || roles.includes('Admin')) return '/dashboard';
  return pagesFor(roles)[0]?.path ?? '/login';
}

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [auth, setAuth] = useState(loadAuth);

  useEffect(() => {
    setUnauthorizedHandler(() => setAuth(null));
  }, []);

  async function login(email, password) {
    const data = await api.login(email, password);
    if (pagesFor(data.roles).length === 0) {
      throw new Error('Tài khoản khách hàng dùng app mobile, web chỉ dành cho nhân viên.');
    }
    saveAuth(data);
    setAuth(data);
    return data;
  }

  function logout() {
    clearAuth();
    setAuth(null);
  }

  // Sau khi sửa hồ sơ: cập nhật tên đang hiện trên thanh menu, token giữ nguyên.
  function updateProfile(profile) {
    const next = { ...auth, fullName: profile.fullName, phone: profile.phone };
    saveAuth(next);
    setAuth(next);
  }

  return <AuthContext.Provider value={{ auth, login, logout, updateProfile }}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  return useContext(AuthContext);
}
