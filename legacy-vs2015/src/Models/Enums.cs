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
        Accountant = 5
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
}
