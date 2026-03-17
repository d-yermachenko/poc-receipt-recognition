using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ReceiptRecognition.OllamaSharp;

public class OllamaSharpOptions
{
    public string OllamaApiUrl { get; set; } = "http://192.168.1.1:11434";

    public string OllamaModelName { get; set; } = "llama3.2-vision:latest";

    public int TimeoutSeconds { get; set; } = 600;

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

    public string SuffixSchemaToPrompt(string schema)
    {
        return $"{Prompt}\n\nSchema:\n{schema}";
    }
}

public class OllamaSharpOptionsSetup : IConfigureOptions<OllamaSharpOptions>
{
    private readonly IConfiguration _configuration;
    public OllamaSharpOptionsSetup(IConfiguration configuration)
    {
        this._configuration = configuration;
    }
    public void Configure(OllamaSharpOptions options) => _configuration.GetSection(nameof(OllamaSharpOptions)).Bind(options);
}
