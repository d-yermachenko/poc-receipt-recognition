using System.Net;
using System.Runtime.InteropServices.Marshalling;
using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.Extensions.Options;
using ReceiptRecognition.Core;

namespace ReceiptRecognition.AzureAI;


public class AzureDocumentAiService(IOptions<AzureOptions> azureOptions) : IReceiptRecognitionService
{
    private const string ReceiptModelName = "prebuilt-receipt";

    private AzureOptions AzureOptions => azureOptions.Value;
    public async Task<ReceiptDto?> GetReceiptData(Stream imageStream, string imageMimeType, CancellationToken cancellationToken)
    {
        AzureKeyCredential credential = new AzureKeyCredential(AzureOptions.Key);
        DocumentIntelligenceClient client = new DocumentIntelligenceClient(new Uri(AzureOptions.Endpoint), credential);
        using MemoryStream memoryStream = new();
        await imageStream.CopyToAsync(memoryStream, cancellationToken);
        BinaryData binaryData = new BinaryData(memoryStream.ToArray());
        var result = await client.AnalyzeDocumentAsync(WaitUntil.Completed, ReceiptModelName, binaryData, cancellationToken);
        var parsedResult = result.Value;

        //https://learn.microsoft.com/en-us/azure/ai-services/document-intelligence/how-to-guides/use-sdk-rest-api?view=doc-intel-4.0.0&tabs=windows&pivots=programming-language-csharp#use-the-receipt-model

        return null;
    }

    private (string MerchantName, string MerchantAddress, DateTime TransactionDate, DateTime TransactionTime, Decimal SubTotal, Decimal Total) 
        GetPrincipalInfo(AnalyzedDocument receipt)
    {

        throw new NotImplementedException();
    }
}
