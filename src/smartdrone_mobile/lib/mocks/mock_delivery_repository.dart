
import '../models/delivery_order.dart';
import '../repositories/delivery_repository.dart';

class MockDeliveryRepository implements DeliveryRepository {
  @override
  Future<List<DeliveryOrder>> getOrders() async {
    return const [
      DeliveryOrder(
        id: 'SD001',
        pickupAddress: 'KTX Khu A',
        deliveryAddress: 'Thư viện',
        recipientName: 'Nguyen Van A',
        status: 'In Transit',
        weight: 1.5,
      ),
      DeliveryOrder(
        id: 'SD002',
        pickupAddress: 'Cang tin',
        deliveryAddress: 'Nha dieu hanh',
        recipientName: 'Tran Thi B',
        status: 'Delivered',
        weight: 2.0,
      ),
    ];
  }
}
