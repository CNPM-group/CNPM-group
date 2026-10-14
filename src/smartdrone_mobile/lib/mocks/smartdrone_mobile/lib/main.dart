
import 'package:flutter/material.dart';

import 'mocks/mock_delivery_repository.dart';

void main() {
  runApp(const SmartDroneApp());
}

class SmartDroneApp extends StatelessWidget {
  const SmartDroneApp({super.key});

  @override
  Widget build(BuildContext context) {
    final repository = MockDeliveryRepository();

    return MaterialApp(
      title: 'SmartDroneDelivery',
      debugShowCheckedModeBanner: false,
      home: Scaffold(
        appBar: AppBar(
          title: const Text('SmartDroneDelivery'),
        ),
        body: FutureBuilder(
          future: repository.getOrders(),
          builder: (context, snapshot) {
            if (!snapshot.hasData) {
              return const Center(
                child: CircularProgressIndicator(),
              );
            }

            final orders = snapshot.data!;

            return ListView.builder(
              itemCount: orders.length,
              itemBuilder: (context, index) {
                final order = orders[index];

                return ListTile(
                  leading: const Icon(Icons.delivery_dining),
                  title: Text('Order ${order.id}'),
                  subtitle: Text(
                    '${order.pickupAddress} → '
                    '${order.deliveryAddress}\n'
                    'Recipient: ${order.recipientName}',
                  ),
                  trailing: Text(order.status),
                );
              },
            );
          },
        ),
      ),
    );
  }
}
