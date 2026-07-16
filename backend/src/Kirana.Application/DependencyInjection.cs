using Kirana.Application.Auth;
using Kirana.Application.Carts;
using Kirana.Application.Catalog;
using Kirana.Application.Customers;
using Kirana.Application.Geo;
using Kirana.Application.Delivery;
using Kirana.Application.Inventory;
using Kirana.Application.Marketplace;
using Kirana.Application.Orders;
using Kirana.Application.Purchasing;
using Kirana.Application.Sales;
using Kirana.Application.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Kirana.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IStoreService, StoreService>();
        services.AddScoped<IStoreDocumentService, StoreDocumentService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGeoService, GeoService>();
        services.AddScoped<ICustomerAuthService, CustomerAuthService>();

        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IMarketplaceService, MarketplaceService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<ISalesService, SalesService>();
        services.AddScoped<IPurchaseService, PurchaseService>();
        services.AddScoped<ICheckoutService, CheckoutService>();
        services.AddScoped<IStoreOrderService, StoreOrderService>();
        services.AddScoped<IDeliveryService, DeliveryService>();

        return services;
    }
}
