import { useCallback, useEffect, useState } from 'react';
import { api } from '../api';
import { formatDate } from '../format';
import { useOrderEvents } from '../orderEvents';
import { useToast } from '../components/Toast';
import LiveDot from '../components/LiveDot';
import StatusBadge from '../components/StatusBadge';

export default function Station() {
  const [flying, setFlying] = useState([]);
  const [arrived, setArrived] = useState([]);
  const [otp, setOtp] = useState({});
  const [track, setTrack] = useState({});
  const [stations, setStations] = useState([]);
  const [busyId, setBusyId] = useState(null);
  const [error, setError] = useState('');
  const toast = useToast();

  const load = useCallback(async () => {
    try {
      const [a, b, list, load] = await Promise.all([
        api.orders({ status: 'InTransit', pageSize: 100 }),
        api.orders({ status: 'ArrivedAtStation', pageSize: 100 }),
        api.stations(),
        api.stationLoad(),
      ]);
      const byId = Object.fromEntries(load.map((l) => [l.stationId, l]));
      setStations(list.map((s) => ({ ...s, load: byId[s.id] })));
      setFlying(a.items);
      setArrived(b.items);
      setError('');
      const snapshots = await Promise.all(a.items.map((o) => api.tracking(o.id).catch(() => null)));
      setTrack(Object.fromEntries(snapshots.filter(Boolean).map((t) => [t.orderId, t])));
    } catch (e) {
      setError(e.message);
    }
  }, []);

  useEffect(() => {
    load();
  }, [load]);

  const live = useOrderEvents(
    () => load(),
    (e) => setTrack((prev) => ({ ...prev, [e.orderId]: { ...prev[e.orderId], ...e } })),
  );

  async function run(order, action, doneText) {
    setBusyId(order.id);
    try {
      await action();
      toast.ok(`${order.trackingCode}: ${doneText}`);
      setOtp((prev) => ({ ...prev, [order.id]: '' }));
      await load();
    } catch (e) {
      toast.error(e.message);
    } finally {
      setBusyId(null);
    }
  }

  return (
    <section>
      <div className="page-head">
        <h2>Nhân viên trạm</h2>
        <LiveDot live={live} />
      </div>
      {error && <div className="error">{error}</div>}

      <StationLoadStrip stations={stations} />

      <h3>Đang bay tới ({flying.length})</h3>
      <div className="grid">
        {flying.map((o) => (
          <div key={o.id} className="card order-card">
            <OrderHead order={o} />
            <p className="muted">Dự kiến tới: {formatDate(o.eta)}</p>
            <FlightProgress track={track[o.id]} />
            <button disabled={busyId === o.id} onClick={() => run(o, () => api.arrive(o.id), 'đã tới trạm, OTP đã gửi khách')}>
              Drone đã tới trạm
            </button>
          </div>
        ))}
        {flying.length === 0 && <p className="empty">Không có drone nào đang bay.</p>}
      </div>

      <h3>Chờ khách nhận ({arrived.length})</h3>
      <div className="grid">
        {arrived.map((o) => {
          const code = otp[o.id] ?? '';
          return (
            <form
              key={o.id}
              className="card order-card"
              onSubmit={(e) => {
                e.preventDefault();
                run(o, () => api.confirm(o.id, code), 'giao thành công');
              }}
            >
              <OrderHead order={o} />
              <label>
                Mã OTP khách đọc
                <input
                  className="otp"
                  inputMode="numeric"
                  placeholder="______"
                  value={code}
                  onChange={(e) =>
                    setOtp((prev) => ({ ...prev, [o.id]: e.target.value.replace(/\D/g, '').slice(0, 6) }))
                  }
                />
              </label>
              <button type="submit" disabled={busyId === o.id || code.length !== 6}>
                Xác nhận giao
              </button>
            </form>
          );
        })}
        {arrived.length === 0 && <p className="empty">Không có đơn nào chờ nhận.</p>}
      </div>

      {toast.view}
    </section>
  );
}

// UC-18 (v0.11): tình trạng các trạm cho nhân viên trạm: kiện chờ nhận + đang bay tới so với sức chứa.
function StationLoadStrip({ stations }) {
  if (stations.length === 0) return null;
  return (
    <div className="load-strip">
      {stations.map((s) => {
        const used = (s.load?.incoming ?? 0) + (s.load?.waiting ?? 0);
        const full = used >= s.capacity;
        return (
          <div key={s.id} className={full ? 'load-chip full' : 'load-chip'} title={`${s.load?.incoming ?? 0} đang tới, ${s.load?.waiting ?? 0} chờ khách nhận`}>
            <b>{s.name}</b>
            <span>
              {s.status === 'Maintenance' ? 'Bảo trì' : `${used}/${s.capacity}${full ? ' · đầy' : ''}`}
            </span>
          </div>
        );
      })}
    </div>
  );
}

function OrderHead({ order }) {
  return (
    <>
      <div className="order-card-head">
        <span className="mono">{order.trackingCode}</span>
        <StatusBadge status={order.status} />
      </div>
      <p>
        {order.originStationName} → <b>{order.destinationStationName}</b>
      </p>
      {order.recipientName && (
        <p className="muted recipient">
          Người nhận: <b>{order.recipientName}</b>
          {order.recipientPhone && (
            <>
              {' · '}
              <a href={`tel:${order.recipientPhone}`}>{order.recipientPhone}</a>
            </>
          )}
        </p>
      )}
    </>
  );
}

function FlightProgress({ track }) {
  if (!track) return null;
  const pct = Math.round(track.progressPct ?? 0);
  return (
    <div className="flight">
      <div className="progress" title={`${pct}%`}>
        <span style={{ width: `${pct}%` }} />
      </div>
      <p className="muted">
        {track.droneCode ?? 'Drone'} đã bay {pct}%{pct >= 100 ? ', đang chờ hạ cánh' : ''}
      </p>
    </div>
  );
}
