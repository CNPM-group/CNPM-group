import { Navigate, Route, Routes, useLocation } from 'react-router-dom';
import { canOpen, homePath, useAuth } from './auth';
import Layout from './components/Layout';
import Dashboard from './pages/Dashboard';
import Dispatcher from './pages/Dispatcher';
import Fleet from './pages/Fleet';
import Login from './pages/Login';
import Station from './pages/Station';
import Users from './pages/Users';
import Customers from './pages/Customers';
import Audit from './pages/Audit';

function Guard({ children }) {
  const { auth } = useAuth();
  const { pathname } = useLocation();
  if (!auth) return <Navigate to="/login" replace />;
  if (!canOpen(pathname, auth.roles)) return <Navigate to={homePath(auth.roles)} replace />;
  return <Layout>{children}</Layout>;
}

export default function App() {
  const { auth } = useAuth();

  return (
    <Routes>
      <Route path="/login" element={<Login />} />
      <Route path="/dispatch" element={<Guard><Dispatcher /></Guard>} />
      <Route path="/station" element={<Guard><Station /></Guard>} />
      <Route path="/dashboard" element={<Guard><Dashboard /></Guard>} />
      <Route path="/fleet" element={<Guard><Fleet /></Guard>} />
      <Route path="/users" element={<Guard><Users /></Guard>} />
      <Route path="/customers" element={<Guard><Customers /></Guard>} />
      <Route path="/audit" element={<Guard><Audit /></Guard>} />
      <Route path="*" element={<Navigate to={auth ? homePath(auth.roles) : '/login'} replace />} />
    </Routes>
  );
}
