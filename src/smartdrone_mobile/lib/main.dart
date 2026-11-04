
import 'package:flutter/material.dart';

void main() {
  runApp(const SmartDroneApp());
}

class SmartDroneApp extends StatelessWidget {
  const SmartDroneApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      debugShowCheckedModeBanner: false,
      title: 'SmartDroneDelivery',
      home: Scaffold(
        appBar: AppBar(
          title: const Text('SmartDroneDelivery'),
        ),
        body: const Center(
          child: Text('Theo dõi đơn hàng'),
        ),
      ),
    );
  }
}
