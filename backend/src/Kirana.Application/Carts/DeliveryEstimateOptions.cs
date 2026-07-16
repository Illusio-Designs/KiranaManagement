namespace Kirana.Application.Carts;

/// <summary>
/// Simple cart-level delivery-fee estimate (config section "Delivery"). This is
/// an indicative figure; the real quote comes from a 3PL partner at checkout
/// (see docs/DELIVERY_INTEGRATIONS.md).
/// </summary>
public class DeliveryEstimateOptions
{
    public const string SectionName = "Delivery";

    public decimal BaseFee { get; set; } = 40m;
    public decimal PerAdditionalStoreFee { get; set; } = 20m;
    public decimal FreeAboveOrderValue { get; set; } = 500m;
}
