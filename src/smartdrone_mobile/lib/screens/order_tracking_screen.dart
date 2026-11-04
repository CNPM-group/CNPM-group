
import 'package:flutter/material.dart';

class OrderTrackingScreen extends StatelessWidget {
  const OrderTrackingScreen({super.key});

  final List<String> statuses = const [
    'Đã tiếp nhận đơn hàng',
    'Đang chuẩn bị giao hàng',
    'Đang vận chuyển',
    'Đã giao hàng',
  ];

  @override
  Widget build(BuildContext context) {
    const currentStatus = 2;

    return Scaffold(
      appBar: AppBar(
        title: const Text('Theo dõi đơn hàng'),
      ),
      body: Padding(
        padding: const EdgeInsets.all(20),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Mã đơn hàng: SD001',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 24),
            const Text('Tiến trình giao hàng'),
            const SizedBox(height: 16),
            ...List.generate(statuses.length, (index) {
              final completed = index <= currentStatus;

              return ListTile(
                leading: Icon(
                  completed
                      ? Icons.check_circle
                      : Icons.radio_button_unchecked,
                  color: completed ? Colors.green : Colors.grey,
                ),
                title: Text(statuses[index]),
              );
            }),
            const SizedBox(height: 20),
            const Text('Trạng thái: Đang vận chuyển'),
          ],
        ),
      ),
    );
  }
}
