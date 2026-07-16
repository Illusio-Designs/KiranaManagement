namespace Kirana.Application.Sales;

public interface ISalesService
{
    /// <summary>Rings up a POS sale: builds the invoice, extracts GST, and decrements stock.</summary>
    Task<SalesInvoiceDto> CreateSaleAsync(CreateSaleRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<SalesInvoiceDto>> GetSalesAsync(CancellationToken ct = default);
    Task<SalesInvoiceDto> GetSaleAsync(Guid invoiceId, CancellationToken ct = default);
}
