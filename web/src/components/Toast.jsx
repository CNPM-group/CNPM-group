import { useEffect, useState } from 'react';

export function useToast() {
  const [toast, setToast] = useState(null);

  useEffect(() => {
    if (!toast) return;
    const t = setTimeout(() => setToast(null), 4000);
    return () => clearTimeout(t);
  }, [toast]);

  return {
    view: toast ? <div className={`toast toast-${toast.type}`}>{toast.text}</div> : null,
    ok: (text) => setToast({ type: 'ok', text }),
    error: (text) => setToast({ type: 'error', text }),
  };
}
