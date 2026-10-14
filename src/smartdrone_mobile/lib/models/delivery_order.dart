
class DeliveryOrder {
  final String id;
  final String pickupAddress;
  final String deliveryAddress;
  final String recipientName;
  final String status;
  final double weight;

  const DeliveryOrder({
    required this.id,
    required this.pickupAddress,
    required this.deliveryAddress,
    required this.recipientName,
    required this.status,
    required this.weight,
  });
}
