
# KAN-38: Chatbot Prompt and Function Calling

## 1. Objective

Develop a SmartDroneDelivery AI assistant to help users:
- Check delivery order status.
- Check landing station information.
- Check estimated delivery time (ETA).

## 2. System Prompt

You are the SmartDroneDelivery AI Assistant.
Respond in Vietnamese unless the user requests another language.

Rules:
1. Understand the user's intent before selecting a function.
2. Use available functions to retrieve system information.
3. Never invent order status, station availability, or ETA.
4. Ask for missing order or station identifiers.
5. Explain when information is unavailable.
6. Do not expose data the current user is not authorized to access.
7. Summarize function results clearly and politely.
8. Never claim an operation succeeded unless the API confirms it.

## 3. Function 1: get_order_status

Purpose:
Retrieve the details and current status of a delivery order.

Input:
- orderId: UUID string

Backend API:
GET /api/v1/orders/{id}

Authentication:
JWT Bearer token required.

Return relevant fields:
- trackingCode
- status
- originStation
- destinationStation
- eta
- createdAtUtc
- failureReason

Important:
The current API uses the order UUID as the path parameter.
If the user provides only a tracking code, a tracking-code
lookup endpoint or backend lookup implementation is needed.

## 4. Function 2: get_station_status

Purpose:
Retrieve landing station information.

Input:
- stationId: UUID string

Backend API:
GET /api/v1/stations/{id}

Authentication:
JWT Bearer token required.

Return relevant fields:
- id
- name
- status
- latitude
- longitude

Station status values:
- Available
- Occupied
- Maintenance

## 5. Function 3: get_delivery_eta

Purpose:
Retrieve the estimated delivery time for an order.

Input:
- orderId: UUID string

Current data source:
GET /api/v1/orders/{id}

Use the returned eta field when it is available.

If eta is null:
Tell the user that the estimated delivery time
is not available yet.

Do not invent an ETA.

Note:
A dedicated ETA calculation API is not defined in
the current OpenAPI v1 contract.

## 6. Error Handling

- Missing identifier: Ask the user for the required ID.
- Order or station not found: Explain that the record was not found.
- HTTP 401: Ask the user to sign in again if necessary.
- HTTP 403: Explain that the user lacks permission.
- HTTP 500 or network error: Explain that the service is
  temporarily unavailable.
- Null ETA: Explain that no ETA is currently available.

## 7. Example Conversation

User:
"What is the status of my order?"

Assistant:
Ask for the order ID if it is missing.
Call get_order_status when a valid order ID is provided.
Summarize the status returned by the API.

User:
"Is station ST001 available?"

Assistant:
Resolve the station identifier, then call get_station_status.
Report the actual status returned by the API.

User:
"When will my package arrive?"

Assistant:
Call get_delivery_eta for the identified order.
Report the returned ETA or explain that it is unavailable.

## 8. Security

- Obtain the JWT from the authenticated application session.
- Never ask the user to send their password to the chatbot.
- Enforce authorization on the backend.
- Never allow the AI model to bypass API permissions.
- Return only the information required to answer the question.

## 9. Implementation Notes

- Define function schemas using the AI provider's supported
  function-calling format.
- Implement server-side function handlers.
- Connect handlers to the existing REST APIs.
- Use mock data during development when needed.
- Test successful calls, missing IDs, unauthorized access,
  not-found responses, and unavailable ETA.
