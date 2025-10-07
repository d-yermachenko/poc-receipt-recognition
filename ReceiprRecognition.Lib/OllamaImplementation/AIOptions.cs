using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace ReceiptRecognition.Lib.OllamaImplementation;

public sealed class ConnectionOptions 
{
    public string OllamaUri { get; set; } = "";
}

public sealed class ImageInputOptions
{
    public string SystemPrompt { get; set; } = """
        You are a strict receipt extraction engine. Extract fields and return ONLY compact JSON with no extra text.
        SCHEMA:
        ---
        {
          "MerchantName": string|null,
          "MerchantAddress": string|null,
          "PurchaseDate": string|null,        // ISO 8601
          "Currency": string|null,            // ISO 4217
          "Tax": number|null,
          "Total": number|null,
          "PaymentMethod": string|null,
          "Items": [
            {
              "Description": string|null,
              "Quantity": number|null,
              "UnitPrice": number|null,
              "LineTotal": number|null
            }
          ]
        }
        Rules:
        - Respond with ONLY JSON.
        - Use null for unknown fields. Currency should be an ISO 4217 code when present.
        - Infer Quantity/UnitPrice/LineTotal when clearly stated; otherwise leave null.
        - Quantity is usually near the item, can be in the column or have a suffix or prefix "X"
        - List of items usually follows the header of receipt and preceeds taxes

        ---
        Rules:
        - Parse: MerchantName, MerchantAddress, PurchaseDate (ISO 8601), Currency (ISO 4217),
          Tax, Total, PaymentMethod, and Items (Description, Quantity, UnitPrice, LineTotal).
        - Amounts are numbers; use null if missing. Do not invent data.
        - If both date and time present, include date part in PurchaseDate; normalize to ISO 8601.
        """;
    public string UserPrompt { get; set; } = """
        Extract the receipt data as JSON matching the target schema.
        """;

    public string Model { get; set; } = "gemma3:4b";

}


public sealed class OcrInputOptions
{
    public string OCRModelName { get; set; } = "gemma3:4b";

    public string OCRSystemPrompt { get; set; } = "You will receive the image of receipt. Recognize text on it. Use tabs ans spaces for keeping formatting. Ignore images and barcodes";

    public string OCRUserPrompt { get; set; }

    public string JsonParserModelName { get; set; } = "deepseek-r1:8b";

    public string JsonParserSystemPrompt { get; set; } = """
        You are a strict receipt extraction engine. Extract fields and return ONLY compact JSON with no extra text.
        SCHEMA:
        ---
        {
          "MerchantName": string|null,
          "MerchantAddress": string|null,
          "PurchaseDate": string|null,        // ISO 8601
          "Currency": string|null,            // ISO 4217
          "Tax": number|null,
          "Total": number|null,
          "PaymentMethod": string|null,
          "Items": [
            {
              "Description": string|null,
              "Quantity": number|null,
              "UnitPrice": number|null,
              "LineTotal": number|null
            }
          ]
        }
        Rules:
        - Respond with ONLY JSON.
        - Use null for unknown fields. Currency should be an ISO 4217 code when present.
        - Infer Quantity/UnitPrice/LineTotal when clearly stated; otherwise leave null.
        - Quantity is usually near the item, can be in the column or have a suffix or prefix "X"
        - List of items usually follows the header of receipt and preceeds taxes

        ---
        Rules:
        - Parse: MerchantName, MerchantAddress, PurchaseDate (ISO 8601), Currency (ISO 4217),
          Tax, Total, PaymentMethod, and Items (Description, Quantity, UnitPrice, LineTotal).
        - Amounts are numbers; use null if missing. Do not invent data.
        - If both date and time present, include date part in PurchaseDate; normalize to ISO 8601.
        """;

    public string JsonParserUserPrompt { get; set; } = """
        Extract the receipt data as JSON matching the target schema.
        """;
}


public sealed class ConnectionOptionsSetup(IConfiguration configuration) : IConfigureOptions<ConnectionOptions>
{
    public void Configure(ConnectionOptions options) => configuration.GetSection(nameof(ConnectionOptions)).Bind(options);
}

public sealed class ImageInputOptionsSetup(IConfiguration configuration) : IConfigureOptions<ImageInputOptions>
{
    public void Configure(ImageInputOptions options) => configuration.GetSection(nameof(ImageInputOptions)).Bind(options);
}

public sealed class OcrInputOptionsSetup(IConfiguration configuration) : IConfigureOptions<OcrInputOptions>
{
    public void Configure(OcrInputOptions options) => configuration.GetSection(nameof(OcrInputOptions)).Bind(options);
}
