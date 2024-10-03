using ShopSiloAppFSD.Enums;
using ShopSiloAppFSD.Models;

namespace ShopSiloAppFSD.Interfaces
{
    public interface IShippingDetailRepository
    {
        Task AddShippingDetailsAsync(ShippingDetail shippingDetails);
        Task UpdateShippingDetailsAsync(ShippingDetail shippingDetails);
        Task DeleteShippingDetailsAsync(int shippingId);
        Task<ShippingDetail?> GetShippingDetailsByIdAsync(int shippingId);
        Task<ShippingDetail?> GetShippingDetailsByOrderIdAsync(int orderId);
        Task UpdateShippingStatusAsync(int shippingId, ShippingStatus status);
        Task<ShippingDetail?> TrackShipmentAsync(string trackingNumber);
    }

}
