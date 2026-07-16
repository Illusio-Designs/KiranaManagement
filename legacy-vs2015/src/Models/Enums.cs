namespace Kirana.WebApi.Models
{
    // Store lifecycle
    public enum StoreStatus
    {
        Pending = 0,
        Active = 1,
        Suspended = 2,
        Rejected = 3,
        Closed = 4
    }

    public enum UserRole
    {
        SuperAdmin = 0,
        Owner = 1,
        Manager = 2,
        Cashier = 3,
        StockClerk = 4,
        Accountant = 5,
        Customer = 6
    }

    public enum UnitOfMeasure
    {
        Piece = 0,
        Pack = 1,
        Gram = 2,
        Kilogram = 3,
        Millilitre = 4,
        Litre = 5,
        Dozen = 6
    }

    public enum PaymentMode
    {
        Cash = 0,
        Upi = 1,
        Card = 2,
        Wallet = 3
    }

    public enum PurchaseStatus
    {
        Draft = 0,
        Received = 1,
        Cancelled = 2
    }

    // Marketplace order lifecycle (consumer + store view).
    public enum OrderStatus
    {
        Placed = 0,
        Accepted = 1,
        Packed = 2,
        OutForDelivery = 3,
        Delivered = 4,
        Cancelled = 5
    }

    // How a user authenticated / was created.
    public enum AuthProvider
    {
        Password = 0,
        Google = 1,
        Otp = 2
    }

    // A product must be approved by the platform before it goes live on the marketplace.
    public enum ProductStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }
}
