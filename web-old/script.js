
const orders = [];
let nextOrderId = 1;

const statuses = [
  "Pending",
  "Approved",
  "InTransit",
  "Delivered"
];

function statusLabel(status) {
  const labels = {
    Pending: "Chờ duyệt",
    Approved: "Đã phê duyệt",
    InTransit: "Đang vận chuyển",
    Delivered: "Đã giao hàng"
  };
  return labels[status] || status;
}

function showPage(pageId) {
  document.querySelectorAll(".page").forEach(page => {
    page.classList.add("hidden");
  });

  document.getElementById(pageId).classList.remove("hidden");

  if (pageId === "orders") renderOrders();
  if (pageId === "status") {
    updateSelect("statusOrder");
    showStatus();
  }
  if (pageId === "confirm") {
    updateSelect("confirmOrder");
    showConfirmInfo();
  }
}

function login(event) {
  event.preventDefault();

  const email = document.getElementById("email").value;
  document.getElementById("loginMessage").textContent =
    "Đăng nhập demo thành công với tài khoản: " + email;

  showPage("orders");
}

function createOrder(event) {
  event.preventDefault();

  const order = {
    id: nextOrderId++,
    pickup: document.getElementById("pickup").value,
    delivery: document.getElementById("delivery").value,
    recipient: document.getElementById("recipient").value,
    phone: document.getElementById("phone").value,
    description: document.getElementById("description").value,
    weight: document.getElementById("weight").value,
    status: statuses[0]
  };

  orders.push(order);

  document.getElementById("createMessage").textContent =
    "Tạo đơn hàng thành công! Mã đơn: SD" + order.id;

  event.target.reset();
  renderOrders();
  updateSelect("statusOrder");
  updateSelect("confirmOrder");
}

function renderOrders() {
  const container = document.getElementById("orderList");

  if (orders.length === 0) {
    container.textContent = "Chưa có đơn hàng.";
    return;
  }

  container.replaceChildren();

  orders.forEach(order => {
    const card = document.createElement("div");
    card.style.cssText =
      "border:1px solid #cbd5e1;padding:14px;margin:12px 0;border-radius:8px";

    const title = document.createElement("h3");
    title.textContent = "Đơn SD" + order.id;

    const recipient = document.createElement("p");
    recipient.textContent = "Người nhận: " + order.recipient;

    const route = document.createElement("p");
    route.textContent = order.pickup + " → " + order.delivery;

    const status = document.createElement("p");
    status.textContent =
      "Trạng thái: " + statusLabel(order.status) +
      " (" + order.status + ")";

    card.append(title, recipient, route, status);
    container.appendChild(card);
  });
}

function updateSelect(selectId) {
  const select = document.getElementById(selectId);
  const previousValue = select.value;

  select.replaceChildren();

  if (orders.length === 0) {
    const option = document.createElement("option");
    option.textContent = "Chưa có đơn hàng";
    option.value = "";
    select.appendChild(option);
    return;
  }

  orders.forEach(order => {
    const option = document.createElement("option");
    option.value = order.id;
    option.textContent =
      "SD" + order.id + " - " + order.recipient;
    select.appendChild(option);
  });

  if (orders.some(order => String(order.id) === previousValue)) {
    select.value = previousValue;
  }
}

function showStatus() {
  const id = Number(document.getElementById("statusOrder").value);
  const order = orders.find(item => item.id === id);

  document.getElementById("statusInfo").textContent = order
    ? "Đơn SD" + order.id + ": " +
      statusLabel(order.status) + " (" + order.status + ")"
    : "Chưa có đơn hàng.";
}

function advanceStatus() {
  const id = Number(document.getElementById("statusOrder").value);
  const order = orders.find(item => item.id === id);

  if (!order) {
    document.getElementById("statusInfo").textContent =
      "Vui lòng tạo đơn hàng trước.";
    return;
  }

  const index = statuses.indexOf(order.status);

  if (index < statuses.length - 1) {
    order.status = statuses[index + 1];
  }

  showStatus();
  renderOrders();
  showConfirmInfo();
}

function showConfirmInfo() {
  const select = document.getElementById("confirmOrder");
  const info = document.getElementById("confirmInfo");
  const id = Number(select.value);
  const order = orders.find(item => item.id === id);

  if (!order) {
    info.textContent = "Vui lòng tạo đơn hàng trước.";
  } else if (order.status === "Delivered") {
    info.textContent = "Đơn hàng đã giao. Có thể xác nhận nhận hàng.";
  } else {
    info.textContent =
      "Trạng thái hiện tại: " + statusLabel(order.status);
  }
}

function confirmDelivery() {
  const id = Number(document.getElementById("confirmOrder").value);
  const order = orders.find(item => item.id === id);
  const info = document.getElementById("confirmInfo");

  if (!order) {
    info.textContent = "Vui lòng tạo đơn hàng trước.";
    return;
  }

  if (order.status !== "Delivered") {
    info.textContent =
      "Đơn hàng chưa giao xong. Vui lòng cập nhật trạng thái trước.";
    return;
  }

  info.textContent =
    "Đã xác nhận nhận hàng thành công cho đơn SD" + order.id + ".";
}

document.getElementById("statusOrder")
  .addEventListener("change", showStatus);

document.getElementById("confirmOrder")
  .addEventListener("change", showConfirmInfo);

showPage("login");

