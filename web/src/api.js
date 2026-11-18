export const API_ROOT = (import.meta.env.VITE_API_URL || 'http://localhost:5080').replace(/\/$/, '');

const BASE = `${API_ROOT}/api/v1`;
const AUTH_KEY = 'sd_auth';

export function loadAuth() {
  try {
    const raw = localStorage.getItem(AUTH_KEY);
    if (!raw) return null;
    const auth = JSON.parse(raw);
    if (new Date(auth.expiresAtUtc) <= new Date()) {
      localStorage.removeItem(AUTH_KEY);
      return null;
    }
    return auth;
  } catch {
    return null;
  }
}

export function saveAuth(auth) {
  try {
    localStorage.setItem(AUTH_KEY, JSON.stringify(auth));
  } catch {
    // trình duyệt chặn localStorage thì chỉ mất đăng nhập khi F5
  }
}

export function clearAuth() {
  try {
    localStorage.removeItem(AUTH_KEY);
  } catch {
    // bỏ qua
  }
}

let onUnauthorized = () => {};
export function setUnauthorizedHandler(fn) {
  onUnauthorized = fn;
}

export class ApiError extends Error {
  constructor(message, status) {
    super(message);
    this.status = status;
  }
}

async function errorMessage(res) {
  try {
    const data = await res.json();
    if (data.errors) return Object.values(data.errors).flat().join(' ');
    if (data.detail) return data.detail;
    if (data.title) return data.title;
  } catch {
    // body không phải JSON
  }
  if (res.status === 403) return 'Bạn không có quyền thực hiện thao tác này.';
  return `Lỗi ${res.status}`;
}

async function request(method, path, body, { raw = false } = {}) {
  const headers = {};
  const token = loadAuth()?.token;
  if (token) headers.Authorization = `Bearer ${token}`;
  if (body !== undefined) headers['Content-Type'] = 'application/json';

  let res;
  try {
    res = await fetch(BASE + path, {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
    });
  } catch {
    throw new ApiError('Không kết nối được máy chủ. Backend đã chạy chưa?', 0);
  }

  if (res.status === 401 && !path.startsWith('/auth/')) {
    clearAuth();
    onUnauthorized();
  }
  if (!res.ok) throw new ApiError(await errorMessage(res), res.status);
  if (raw) return res;

  const text = await res.text();
  return text ? JSON.parse(text) : null;
}

function query(params = {}) {
  const entries = Object.entries(params).filter(([, v]) => v !== undefined && v !== null && v !== '');
  const s = new URLSearchParams(entries).toString();
  return s ? `?${s}` : '';
}

export const api = {
  login: (email, password) => request('POST', '/auth/login', { email, password }),
  me: () => request('GET', '/auth/me'),
  updateMe: (body) => request('PUT', '/auth/me', body),
  changePassword: (currentPassword, newPassword) => request('POST', '/auth/me/password', { currentPassword, newPassword }),
  auditLogs: (params) => request('GET', `/audit-logs${query(params)}`),

  customers: (q) => request('GET', `/customers${query({ q })}`),
  customer: (id) => request('GET', `/customers/${id}`),
  updateCustomer: (id, body) => request('PUT', `/customers/${id}`, body),

  orders: (params) => request('GET', `/orders${query(params)}`),
  order: (id) => request('GET', `/orders/${id}`),
  history: (id) => request('GET', `/orders/${id}/history`),
  tracking: (id) => request('GET', `/orders/${id}/tracking`),

  approve: (id, note) => request('POST', `/orders/${id}/approve`, { note: note || null }),
  reject: (id, reason) => request('POST', `/orders/${id}/reject`, { reason }),
  schedule: (id, droneId) => request('POST', `/orders/${id}/schedule`, { droneId }),
  start: (id) => request('POST', `/orders/${id}/start`),
  fail: (id, reason) => request('POST', `/orders/${id}/fail`, { reason }),
  returnOrder: (id, reason) => request('POST', `/orders/${id}/return`, { reason }),
  arrive: (id) => request('POST', `/orders/${id}/arrive`),
  confirm: (id, otp) => request('POST', `/orders/${id}/confirm`, { otp }),
  aiSummary: (id) => request('POST', `/orders/${id}/ai-summary`),
  packageImage: async (id) => (await request('GET', `/orders/${id}/package-image`, undefined, { raw: true })).blob(),

  drones: () => request('GET', '/drones'),
  createDrone: (body) => request('POST', '/drones', body),
  updateDrone: (id, body) => request('PUT', `/drones/${id}`, body),
  deleteDrone: (id) => request('DELETE', `/drones/${id}`),

  stations: () => request('GET', '/stations'),
  stationLoad: () => request('GET', '/stations/load'),
  createStation: (body) => request('POST', '/stations', body),
  updateStation: (id, body) => request('PUT', `/stations/${id}`, body),
  deleteStation: (id) => request('DELETE', `/stations/${id}`),

  users: () => request('GET', '/users'),
  createUser: (body) => request('POST', '/users', body),
  setRoles: (id, roles) => request('PUT', `/users/${id}/roles`, { roles }),
  setActive: (id, isActive) => request('PUT', `/users/${id}/active`, { isActive }),

  // range = { from, to } (yyyy-mm-dd), trống = 30 ngày gần nhất (v0.11)
  report: (range) => request('GET', `/reports/orders${query(range)}`),
  reportStations: (range) => request('GET', `/reports/stations${query(range)}`),
  aiInsights: (range) => request('GET', `/reports/ai-insights${query(range)}`),
  reportCsv: async (range) => (await request('GET', `/reports/orders.csv${query(range)}`, undefined, { raw: true })).blob(),
};
