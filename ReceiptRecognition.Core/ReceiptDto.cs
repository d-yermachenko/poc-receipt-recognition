namespace ReceiptRecognition.Core;

public sealed record ReceiptDto(
    string? MerchantName,
    string? MerchantAddress,
    string? PurchaseDate,          // ISO 8601 date inferred
    string? Currency,                // ISO 4217 e.g. "USD", "EUR"
    decimal? Tax,
    decimal? Total,
    string? PaymentMethod,           // e.g. "VISA", "Cash"
    IReadOnlyList<ReceiptItemDto> Items
)
{
    public static readonly ReceiptDto Empty = new(
        null, null, null,  null, null, null, null, Array.Empty<ReceiptItemDto>());
}

public sealed record ReceiptItemDto(
    string? Description,
    decimal? Quantity,
    decimal? UnitPrice,
    decimal? LineTotal
);