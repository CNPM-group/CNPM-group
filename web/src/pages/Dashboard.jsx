import { useCallback, useEffect, useState } from 'react';
import { api } from '../api';
import { statusLabel } from '../format';
import { useOrderEvents } from '../orderEvents';
import LiveDot from '../components/LiveDot';
import StatusBadge from '../components/StatusBadge';
import AiSource from '../components/AiSource';

export default function Dashboard() {
  const [report, setReport] = useState(null);
  const [byStation, setByStation] = useState([]);
  const [error, setError] = useState('');
  const [insights, setInsights] = useState(null);
  // Lọc khoảng ngày (v0.11). "to" của server là mốc loại trừ, nên gửi ngày kế tiếp để tính trọn ngày cuối.
  const [range, setRange] = useState({ from: '', to: '' });
  const params = {
    from: range.from || undefined,
    to: range.to ? nextDay(range.to) : undefined,
  };
  const paramsKey = `${params.from ?? ''}|${params.to ?? ''}`;
  const [thinking, setThinking] = useState(false);

  const load = useCallback(async () => {
    try {
      const [r, s] = await Promise.all([api.report(params), api.reportStations(params)]);
      setReport(r);
      setByStation(s);
      setError('');
    } catch (e) {
      setError(e.message);
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [paramsKey]);

  useEffect(() => {
    load();
  }, [load]);

  const live = useOrderEvents(() => load());

  async function downloadCsv() {
    try {
      const blob = await api.reportCsv(params);
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `don-hang-${new Date().toISOString().slice(0, 10)}.csv`;
      a.click();
      URL.revokeObjectURL(url);
    } catch (e) {
      setError(e.message);
    }
  }

  // Không tự gọi khi mở trang: mỗi lần phân tích là 1 lần gọi LLM (tốn phí), để quản lý tự bấm.
  async function analyze() {
    setThinking(true);
    try {
      setInsights(await api.aiInsights(params));
      setError('');
    } catch (e) {
      setError(e.message);
    } finally {
      setThinking(false);
    }
  }

  const maxDay = Math.max(1, ...(report?.byDay ?? []).map((d) => d.count));

  return (
    <section>
      <div className="page-head">
        <h2>Thống kê vận hành</h2>
        <div className="date-range">
          <label>
            Từ
            <input type="date" value={range.from} max={range.to || undefined} onChange={(e) => setRange({ ...range, from: e.target.value })} />
          </label>
          <label>
            Đến
            <input type="date" value={range.to} min={range.from || undefined} onChange={(e) => setRange({ ...range, to: e.target.value })} />
          </label>
          {(range.from || range.to) && (
            <button className="ghost small" onClick={() => setRange({ from: '', to: '' })}>
              30 ngày gần nhất
            </button>
          )}
        </div>
        <LiveDot live={live} />
        <button className="ghost" onClick={downloadCsv}>
          Tải CSV
        </button>
      </div>
      {error && <div className="error">{error}</div>}

      {report && (
        <>
          <div className="stats">
            <Stat label="Tổng đơn" value={report.totalOrders} />
            <Stat label="Hoàn tất" value={report.completedOrders} />
            <Stat label="Thất bại" value={report.failedOrders} />
            <Stat label="Tỷ lệ thành công" value={`${report.successRatePct.toFixed(1)}%`} />
            <Stat
              label="Thời gian giao TB"
              value={report.avgDeliveryMinutes == null ? '-' : `${report.avgDeliveryMinutes.toFixed(0)} phút`}
            />
          </div>

          <div className="two-col">
            <div className="card">
              <h3>Theo trạng thái</h3>
              <table>
                <tbody>
                  {report.byStatus.map((s) => (
                    <tr key={s.status}>
                      <td>
                        <StatusBadge status={s.status} />
                      </td>
                      <td className="num">{s.count}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
              {report.byStatus.length === 0 && <p className="empty">Chưa có đơn.</p>}
            </div>

            <div className="card">
              <h3>Số đơn theo ngày</h3>
              {report.byDay.map((d) => (
                <div key={d.day} className="bar-row" title={`${d.count} đơn`}>
                  <span className="bar-label">{d.day.slice(5).split('-').reverse().join('/')}</span>
                  <span className="bar" style={{ width: `${(d.count / maxDay) * 100}%` }} />
                  <span className="num">{d.count}</span>
                </div>
              ))}
              {report.byDay.length === 0 && <p className="empty">Chưa có đơn.</p>}
            </div>
          </div>

          <div className="card station-report">
            <h3>Hiệu quả theo trạm</h3>
            <table>
              <thead>
                <tr>
                  <th>Trạm</th>
                  <th className="num">Gửi đi</th>
                  <th className="num">Nhận về</th>
                  <th className="num">Hoàn tất</th>
                  <th className="num">Thất bại</th>
                  <th className="num">Tỷ lệ thành công</th>
                  <th className="num">Thời gian giao TB</th>
                </tr>
              </thead>
              <tbody>
                {byStation.map((s) => (
                  <tr key={s.station}>
                    <td>{s.station}</td>
                    <td className="num">{s.sent}</td>
                    <td className="num">{s.received}</td>
                    <td className="num">{s.completed}</td>
                    <td className="num">{s.failed}</td>
                    <td className="num">{s.completed + s.failed === 0 ? '-' : `${s.successRatePct.toFixed(1)}%`}</td>
                    <td className="num">{s.avgDeliveryMinutes == null ? '-' : `${s.avgDeliveryMinutes.toFixed(0)} phút`}</td>
                  </tr>
                ))}
              </tbody>
            </table>
            {byStation.length === 0 && <p className="empty">Chưa có đơn trong khoảng này.</p>}
            <p className="muted">Hoàn tất, thất bại, tỷ lệ và thời gian tính theo trạm nhận (nơi khách nhập OTP).</p>
          </div>

          <div className="card ai-card">
            <div className="ai-head">
              <h3>Nhận xét của AI</h3>
              <button disabled={thinking} onClick={analyze}>
                {thinking ? 'AI đang phân tích...' : insights ? 'Phân tích lại' : 'Phân tích bằng AI'}
              </button>
            </div>
            {!insights && !thinking && (
              <p className="empty">Bấm nút để AI đọc số liệu 30 ngày qua, chỉ ra điểm đáng chú ý và việc nên làm.</p>
            )}
            {insights && (
              <>
                <ul className="ai-points">
                  {insights.points.map((p, i) => (
                    <li key={i}>{p}</li>
                  ))}
                </ul>
                <AiSource source={insights.source} model={insights.model} note={insights.note} />
              </>
            )}
          </div>

          <p className="muted">
            Số liệu từ {new Date(report.fromUtc).toLocaleDateString('vi-VN')} đến{' '}
            {new Date(report.toUtc).toLocaleDateString('vi-VN')}. {statusLabel('Completed')} = khách đã nhập đúng OTP.
          </p>
        </>
      )}
    </section>
  );
}

function Stat({ label, value }) {
  return (
    <div className="card stat">
      <div className="muted">{label}</div>
      <div className="stat-value">{value}</div>
    </div>
  );
}

function nextDay(yyyyMmDd) {
  const d = new Date(`${yyyyMmDd}T00:00:00`);
  d.setDate(d.getDate() + 1);
  return `${d.getFullYear()}-${String(d.getMonth() + 1).padStart(2, '0')}-${String(d.getDate()).padStart(2, '0')}`;
}

