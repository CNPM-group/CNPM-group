import { useCallback, useEffect, useState } from 'react';
import { api } from '../api';
import { useToast } from '../components/Toast';
import Dialog from '../components/Dialog';

const DRONE_STATUS = {
  Idle: 'Rảnh',
  Assigned: 'Đã gán đơn',
  InFlight: 'Đang bay',
  Charging: 'Đang sạc',
  Maintenance: 'Bảo trì',
};

const STATION_STATUS = {
  Available: 'Hoạt động',
  Occupied: 'Đang bận',
  Maintenance: 'Bảo trì',
};

const EMPTY_DRONE = {
  droneCode: '',
  model: '',
  maxPayloadKg: 2,
  maxRangeKm: 10,
  cruiseSpeedKmh: 40,
  batteryPct: 100,
  status: 'Idle',
  currentStationId: '',
};

const EMPTY_STATION = { name: '', latitude: 10.8, longitude: 106.7, status: 'Available', capacity: 4 };

export default function Fleet() {
  const [drones, setDrones] = useState([]);
  const [stations, setStations] = useState([]);
  const [load, setLoad] = useState({});
  const [error, setError] = useState('');
  const [dialog, setDialog] = useState(null);
  const [busy, setBusy] = useState(false);
  const toast = useToast();

  const reload = useCallback(async () => {
    try {
      const [d, s, l] = await Promise.all([api.drones(), api.stations(), api.stationLoad()]);
      setDrones(d);
      setStations(s);
      setLoad(Object.fromEntries(l.map((x) => [x.stationId, x])));
      setError('');
    } catch (e) {
      setError(e.message);
    }
  }, []);

  useEffect(() => {
    reload();
  }, [reload]);

  const stationName = (id) => stations.find((s) => s.id === id)?.name ?? '-';

  async function save(action, doneText, { closeOnError = false } = {}) {
    setBusy(true);
    try {
      await action();
      toast.ok(doneText);
      setDialog(null);
      await reload();
    } catch (e) {
      toast.error(e.message);
      if (closeOnError) setDialog(null);
    } finally {
      setBusy(false);
    }
  }

  function saveDrone(form) {
    const body = {
      droneCode: form.droneCode.trim(),
      model: form.model.trim(),
      maxPayloadKg: Number(form.maxPayloadKg),
      maxRangeKm: Number(form.maxRangeKm),
      cruiseSpeedKmh: Number(form.cruiseSpeedKmh),
      batteryPct: Number(form.batteryPct),
      status: form.status,
      currentStationId: form.currentStationId || null,
    };
    return form.id
      ? save(() => api.updateDrone(form.id, body), `Đã cập nhật ${body.droneCode}`)
      : save(() => api.createDrone(body), `Đã thêm ${body.droneCode}`);
  }

  function saveStation(form) {
    const body = {
      name: form.name.trim(),
      latitude: Number(form.latitude),
      longitude: Number(form.longitude),
      status: form.status,
      capacity: Number(form.capacity),
    };
    return form.id
      ? save(() => api.updateStation(form.id, body), `Đã cập nhật ${body.name}`)
      : save(() => api.createStation(body), `Đã thêm ${body.name}`);
  }

  return (
    <section>
      <div className="page-head">
        <h2>Drone & trạm</h2>
      </div>
      {error && <div className="error">{error}</div>}

      <div className="section-head">
        <h3>Drone ({drones.length})</h3>
        <button onClick={() => setDialog({ kind: 'drone', form: EMPTY_DRONE })}>+ Thêm drone</button>
      </div>
      <div className="card">
        <table>
          <thead>
            <tr>
              <th>Mã</th>
              <th>Model</th>
              <th className="num">Tải (kg)</th>
              <th className="num">Tầm bay (km)</th>
              <th className="num">Pin</th>
              <th>Trạng thái</th>
              <th>Đang ở trạm</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {drones.map((d) => (
              <tr key={d.id}>
                <td className="mono">{d.droneCode}</td>
                <td>{d.model}</td>
                <td className="num">{d.maxPayloadKg}</td>
                <td className="num">{d.maxRangeKm}</td>
                <td className="num">{d.batteryPct}%</td>
                <td>
                  <span className={`badge drone-${d.status}`}>{DRONE_STATUS[d.status] ?? d.status}</span>
                </td>
                <td>{stationName(d.currentStationId)}</td>
                <td className="actions">
                  <button
                    className="ghost small"
                    onClick={() =>
                      setDialog({ kind: 'drone', form: { ...d, currentStationId: d.currentStationId ?? '' } })
                    }
                  >
                    Sửa
                  </button>
                  <button
                    className="danger small"
                    onClick={() => setDialog({ kind: 'delete', label: d.droneCode, run: () => api.deleteDrone(d.id) })}
                  >
                    Xóa
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {drones.length === 0 && <p className="empty">Chưa có drone.</p>}
      </div>

      <div className="section-head">
        <h3>Trạm hạ cánh ({stations.length})</h3>
        <button onClick={() => setDialog({ kind: 'station', form: EMPTY_STATION })}>+ Thêm trạm</button>
      </div>
      <div className="card">
        <table>
          <thead>
            <tr>
              <th>Tên trạm</th>
              <th className="num">Vĩ độ</th>
              <th className="num">Kinh độ</th>
              <th>Trạng thái</th>
              <th className="num" title="Đơn đang bay tới + kiện chờ khách nhận / sức chứa">Đang tới + chờ / sức chứa</th>
              <th className="num" title="Đơn sắp cất cánh từ trạm này">Sắp đi</th>
              <th></th>
            </tr>
          </thead>
          <tbody>
            {stations.map((s) => (
              <tr key={s.id}>
                <td>{s.name}</td>
                <td className="num">{s.latitude}</td>
                <td className="num">{s.longitude}</td>
                <td>
                  <span className={`badge station-${s.status}`}>{STATION_STATUS[s.status] ?? s.status}</span>
                </td>
                <StationLoadCells station={s} load={load[s.id]} />
                <td className="actions">
                  <button className="ghost small" onClick={() => setDialog({ kind: 'station', form: s })}>
                    Sửa
                  </button>
                  <button
                    className="danger small"
                    onClick={() => setDialog({ kind: 'delete', label: s.name, run: () => api.deleteStation(s.id) })}
                  >
                    Xóa
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
        {stations.length === 0 && <p className="empty">Chưa có trạm.</p>}
      </div>

      {dialog?.kind === 'drone' && (
        <DroneDialog
          initial={dialog.form}
          stations={stations}
          busy={busy}
          onClose={() => setDialog(null)}
          onSubmit={saveDrone}
        />
      )}
      {dialog?.kind === 'station' && (
        <StationDialog initial={dialog.form} busy={busy} onClose={() => setDialog(null)} onSubmit={saveStation} />
      )}
      {dialog?.kind === 'delete' && (
        <Dialog
          title={`Xóa ${dialog.label}?`}
          submitText="Xóa"
          danger
          busy={busy}
          onClose={() => setDialog(null)}
          onSubmit={() => save(dialog.run, `Đã xóa ${dialog.label}`, { closeOnError: true })}
        >
          <p className="muted">Không xóa được nếu drone/trạm đã có đơn hàng hoặc chuyến bay dùng tới.</p>
        </Dialog>
      )}

      {toast.view}
    </section>
  );
}

function useForm(initial) {
  const [form, setForm] = useState(initial);
  const bind = (name) => ({
    value: form[name],
    onChange: (e) => setForm((f) => ({ ...f, [name]: e.target.value })),
  });
  return [form, bind];
}

function DroneDialog({ initial, stations, busy, onClose, onSubmit }) {
  const [form, bind] = useForm(initial);

  return (
    <Dialog
      title={form.id ? `Sửa ${initial.droneCode}` : 'Thêm drone'}
      submitText="Lưu"
      busy={busy}
      onClose={onClose}
      onSubmit={() => onSubmit(form)}
    >
      <div className="form-grid">
        <label>
          Mã drone
          <input required maxLength={30} {...bind('droneCode')} />
        </label>
        <label>
          Model
          <input required maxLength={100} {...bind('model')} />
        </label>
        <label>
          Tải tối đa (kg)
          <input type="number" step="0.1" min="0.1" max="50" required {...bind('maxPayloadKg')} />
        </label>
        <label>
          Tầm bay (km)
          <input type="number" step="0.1" min="0.1" max="500" required {...bind('maxRangeKm')} />
        </label>
        <label>
          Tốc độ bay (km/h)
          <input type="number" step="1" min="1" max="200" required {...bind('cruiseSpeedKmh')} />
        </label>
        <label>
          Pin (%)
          <input type="number" step="1" min="0" max="100" required {...bind('batteryPct')} />
        </label>
        <label>
          Trạng thái
          <select {...bind('status')}>
            {Object.entries(DRONE_STATUS).map(([value, label]) => (
              <option key={value} value={value}>
                {label}
              </option>
            ))}
          </select>
        </label>
        <label>
          Đang ở trạm
          <select {...bind('currentStationId')}>
            <option value="">(không ở trạm nào)</option>
            {stations.map((s) => (
              <option key={s.id} value={s.id}>
                {s.name}
              </option>
            ))}
          </select>
        </label>
      </div>
    </Dialog>
  );
}

// FR07 (v0.11): trạm đầy thì điều phối không gán drone tới được (server trả 409).
function StationLoadCells({ station, load }) {
  const used = (load?.incoming ?? 0) + (load?.waiting ?? 0);
  const full = used >= station.capacity;
  return (
    <>
      <td className={full ? 'num load-full' : 'num'} title={`${load?.incoming ?? 0} đang tới, ${load?.waiting ?? 0} chờ khách nhận`}>
        {used} / {station.capacity}
        {full && ' · đầy'}
      </td>
      <td className="num">{load?.outgoing ?? 0}</td>
    </>
  );
}

function StationDialog({ initial, busy, onClose, onSubmit }) {
  const [form, bind] = useForm(initial);

  return (
    <Dialog
      title={form.id ? `Sửa ${initial.name}` : 'Thêm trạm'}
      submitText="Lưu"
      busy={busy}
      onClose={onClose}
      onSubmit={() => onSubmit(form)}
    >
      <label>
        Tên trạm
        <input required maxLength={200} {...bind('name')} />
      </label>
      <div className="form-grid">
        <label>
          Vĩ độ
          <input type="number" step="0.000001" min="-90" max="90" required {...bind('latitude')} />
        </label>
        <label>
          Kinh độ
          <input type="number" step="0.000001" min="-180" max="180" required {...bind('longitude')} />
        </label>
      </div>
      <div className="form-grid">
        <label>
          Trạng thái
          <select {...bind('status')}>
            {Object.entries(STATION_STATUS).map(([value, label]) => (
              <option key={value} value={value}>
                {label}
              </option>
            ))}
          </select>
        </label>
        <label>
          Sức chứa (số kiện cùng lúc)
          <input type="number" min="1" max="50" step="1" required {...bind('capacity')} />
        </label>
      </div>
    </Dialog>
  );
}
