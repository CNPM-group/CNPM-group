import { useEffect, useRef, useState } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { API_ROOT, loadAuth } from './api';

const POLL_MS = 10000;

// Nghe event đổi trạng thái đơn từ hub. Hub chưa chạy thì tự chuyển sang hỏi lại server mỗi 10 giây.
// onDrone (không bắt buộc) nhận vị trí drone đang bay từ event DroneLocation.
export function useOrderEvents(onChange, onDrone) {
  const handler = useRef(onChange);
  handler.current = onChange;
  const droneHandler = useRef(onDrone);
  droneHandler.current = onDrone;
  const [live, setLive] = useState(false);

  useEffect(() => {
    let stopped = false;
    let timer = null;

    const poll = () => {
      if (!timer) timer = setInterval(() => handler.current(null), POLL_MS);
    };
    const stopPoll = () => {
      clearInterval(timer);
      timer = null;
    };

    const conn = new HubConnectionBuilder()
      .withUrl(`${API_ROOT}/hubs/delivery`, { accessTokenFactory: () => loadAuth()?.token ?? '' })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.None)
      .build();

    conn.on('OrderStatusUpdated', (event) => handler.current(event));
    conn.on('DroneLocation', (event) => droneHandler.current?.(event));
    conn.onreconnecting(() => setLive(false));
    conn.onreconnected(() => {
      setLive(true);
      stopPoll();
    });
    conn.onclose(() => {
      if (stopped) return;
      setLive(false);
      poll();
    });

    conn
      .start()
      .then(() => {
        if (stopped) return conn.stop();
        setLive(true);
      })
      .catch(() => {
        if (!stopped) poll();
      });

    return () => {
      stopped = true;
      stopPoll();
      conn.stop();
    };
  }, []);

  return live;
}
