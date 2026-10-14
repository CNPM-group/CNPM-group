
import '../models/delivery_order.dart';

abstract class DeliveryRepository {
  Future<List<DeliveryOrder>> getOrders();
}
