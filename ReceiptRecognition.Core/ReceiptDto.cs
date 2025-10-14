namespace ReceiptRecognition.Core;

public sealed record ReceiptDto(
    string? MerchantName,
    string? MerchantAddress,
    string? PurchaseDate,  // ISO 8601 date inferred
    Currency? Total,
    Currency? Taxes,
    Currency? Tip,
    IReadOnlyList<ReceiptItemDto> Items,
    IReadOnlyList<ReceiptTaxDetailDto> TaxDetails,
    IReadOnlyList<ReceiptPaymentDto> Payments
)
{
    public static readonly ReceiptDto Empty = new(null, null, null, null, null, null, [], [], []);
}

public sealed record ReceiptItemDto(
    string? Description = "",
    decimal? Quantity = null,
    Currency? UnitPrice = null,
    Currency? LineTotal = null
);

public sealed record ReceiptTaxDetailDto (
    string? Name = null,
    decimal? Rate = null,
    Currency? Amount = null
);

public sealed record ReceiptPaymentDto (
    string? Method = null,
    Currency? Amount = null
);
