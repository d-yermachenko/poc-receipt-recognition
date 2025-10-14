using System;
using System.Data.Common;
using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Xml.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Polly.Caching;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ReceiptRecognition.Ollama.Options;

internal class PromptsNotebook
{

}

public sealed class OcrInputOptions
{

    public string OCRModelName { get; set; } = AIModelsDictionary.LlamaVision; 

    public string OCRPromptInEnglish { get; set; } = """
        Recognize the receipt in the provided image and output ONLY the visible text as plain text.

        Rules:
        - Do not invent or infer missing text.If a character is unreadable, replace only that character with?.
        - Preserve reading order (top-to-bottom, left-to-right).
        - Preserve line breaks; keep each line as it appears on the receipt.
        - Keep numbers, currency symbols, punctuation, and casing exactly as seen.Do not translate or reformat dates or numbers.
        - Ignore barcodes/QR codes, logos, decorative slogans, and unrelated boilerplate.
        - Include merchant name/address, date/time, item lines, quantities, unit prices, line totals, subtotal, tax, total, payment method, and store/terminal IDs where visible.
        - Use spaces to reflect column alignment when helpful; do not add any labels or JSON.

        Output:
        - Return only the plain text block with no extra commentary, XML/JSON, quotes, or Markdown.
        """;

    /*public string OCRUserPrompt { get; set; } = """
        Recognize given image as text of receipt.
        Rules:
        - Do not invent data
        - Use tabs and spaces for formatting. 
        - Ignore barcodes and logos
        - Return result as plain text
        - In json response, use field "receipt" : string for recognized text.
        """;*/


    public string TextParserModelName { get; set; } = AIModelsDictionary.Gemma.B12;

    public string TextParserPromptInEnglish { get; set; } = """
        You are a strict receipt extraction engine. Extract fields and return ONLY compact JSON with no extra text. Do not invent data. Use null for unknown values.
        """;
}

public sealed class OcrInputOptionsSetup(IConfiguration configuration) : IConfigureOptions<OcrInputOptions>
{
    public void Configure(OcrInputOptions options) => configuration.GetSection(nameof(OcrInputOptions)).Bind(options);
}
