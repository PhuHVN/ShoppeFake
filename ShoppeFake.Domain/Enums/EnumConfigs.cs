namespace ShoppeFake.Domain.Enums
{
    public enum StatusEnum
    {
        Inactive = 0,
        Active = 1,
        Pending = 2,
    }
    public enum RoleEnum
    {
        Customer = 0,
        Admin = 1,
    }

    public enum OrderStatus
    {
        Pending = 0,
        Confirmed = 1,
        Shipping = 2,
        Completed = 3,
        Cancelled = 4
    }

    public enum PaymentMethod
    {
        COD = 0,
        Online = 1
    }

    public enum PaymentStatus
    {
        Pending = 0,
        Paid = 1,
        Failed = 2
    }
    public enum WebhookProcessStatus
    {
        Success,
        Ignore,
        Retry,
        InvalidSignature,
        OutOfStock,
        AmountMismatch
    }
}
