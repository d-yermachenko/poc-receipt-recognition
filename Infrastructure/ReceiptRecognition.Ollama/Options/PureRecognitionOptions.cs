using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ReceiptRecognition.Ollama.Options;

public sealed class PureRecognitionOptions
{
    public string Prompt { get; set; } = """
        You are a strict receipt extraction engine. Extract fields and return ONLY compact JSON with no extra text.

        Rules:
        - Parse: MerchantName, MerchantAddress, PurchaseDate (ISO 8601), Currency (ISO 4217),
          Tax, Total, PaymentMethod, and Items (Description, Quantity, UnitPrice, LineTotal).
        - Amounts are numbers; use null if missing. Do not invent data.
        - If both date and time present, include date part in PurchaseDate; normalize to ISO 8601.
        - Respond with ONLY JSON.
        - Use null for unknown fields. Currency should be an ISO 4217 code when present.
        - Infer Quantity/UnitPrice/LineTotal when clearly stated; otherwise leave null.
        - Quantity is usually near the item, can be in the column or have a suffix or prefix "X"
        - List of items usually follows the header of receipt and preceeds taxes
        """;

    public string Model { get; set; } = AIModelsDictionary.Qwen;

}

public sealed class PureRecognitionOptionsSetup(IConfiguration configuration) : IConfigureOptions<PureRecognitionOptions>
{
    public void Configure(PureRecognitionOptions options) => configuration.GetSection(nameof(PureRecognitionOptions)).Bind(options);
}


