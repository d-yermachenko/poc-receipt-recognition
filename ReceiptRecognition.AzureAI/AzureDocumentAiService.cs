using System.Globalization;
using Azure;
using Azure.AI.DocumentIntelligence;
using Microsoft.Extensions.Options;
using ReceiptRecognition.Core;


namespace ReceiptRecognition.AzureAI;


public class AzureDocumentAiService(IOptions<AzureOptions> azureOptions) : IReceiptRecognitionService
{
    private const string ReceiptModelName = "prebuilt-receipt";

    private AzureOptions AzureOptions => azureOptions.Value;
    public async Task<ReceiptDto?> GetReceiptData(Stream imageStream, string imageMimeType, CultureInfo receiptCulture, SupportedReceiptType _, CancellationToken cancellationToken)
    {
        AzureKeyCredential credential = new(AzureOptions.Key);
        DocumentIntelligenceClient client = new (new Uri(AzureOptions.Endpoint), credential);
        using MemoryStream memoryStream = new();
        await imageStream.CopyToAsync(memoryStream, cancellationToken);
        BinaryData binaryData = new (memoryStream.ToArray());
        var result = await client.AnalyzeDocumentAsync(WaitUntil.Completed, ReceiptModelName, binaryData, cancellationToken);
        var parsedResult = result.Value;
        //https://learn.microsoft.com/en-us/azure/ai-services/document-intelligence/how-to-guides/use-sdk-rest-api?view=doc-intel-4.0.0&tabs=windows&pivots=programming-language-csharp#use-the-receipt-model
        //https://github.com/Azure-Samples/document-intelligence-code-samples/blob/main/schema/2024-11-30-ga/receipt.md

        if (parsedResult.Documents.Count != 1)
            throw new ArgumentException("Only one receipt is allowed at one time", nameof(imageStream));

        var receiptDocument = parsedResult.Documents[0];
        ReceiptInfo principalInfo = GetPrincipalInfo(receiptDocument);

        IEnumerable<ReceiptItemInfo?> items = GetItemInfos(receiptDocument);
        if (items?.Any(x => x is null || x.Description is null)??true)
            throw new Exception("Receipt item/items undefined"); //TODO Make specialized exception

        IEnumerable<ReceiptItemDto> receiptItems = items.Select(x => new ReceiptItemDto(x!.Description, Convert.ToDecimal(x!.Quantity), x!.Price.Amount, Convert.ToDecimal(x!.TotalPrice?.Amount)));


        IEnumerable<TaxDetailInfo?> taxDetails = GetTaxDetails(receiptDocument) ;
        if (taxDetails?.Any(x => x is null || x.Amount is null) ?? true)
            throw new Exception("Receipt item/items undefined"); //TODO Make specialized exception
        IEnumerable<ReceiptTaxDetailDto> receiptTaxDetails = taxDetails.Select(x => new ReceiptTaxDetailDto(x!.Description, x.Rate, x.Amount));

        IEnumerable<PaymentDetailInfo?> paymentsDetails = GetPaymentDetails(receiptDocument);
        if (paymentsDetails?.Any(x => x is null || x.Amount is null) ?? true)
            throw new Exception("Receipt item/items undefined"); //TODO Make specialized exception
        IEnumerable<ReceiptPaymentDto> receiptPaymentsDetails = paymentsDetails.Select(x => new ReceiptPaymentDto(x!.Method, x!.Amount));

        return new ReceiptDto(
            principalInfo.MerchantName, 
            principalInfo.MerchantAddress,
            principalInfo.TransactionDate.ToString(),
            principalInfo.Total,
            principalInfo.TotalTax,
            principalInfo.Tip,
            [.. receiptItems],
            [.. receiptTaxDetails],
            [.. receiptPaymentsDetails]
         );
    }


    #region Receipt principal info
    public record ReceiptInfo(string MerchantName, string MerchantAddress, DateTime TransactionDate, Currency SubTotal, Currency? TotalTax, Currency Tip,  Currency Total);

    private static ReceiptInfo GetPrincipalInfo(AnalyzedDocument receipt)
    {
        string? merchantName = null;
        var receiptFields = receipt.Fields;
        if(receiptFields.TryGetValue("MerchantName", out DocumentField merchantNameDF))
            merchantName = merchantNameDF.FieldType == DocumentFieldType.String ? merchantNameDF.ValueString : null;

        string? merchantAddress = null;
        if (receiptFields.TryGetValue("MerchantAddress", out DocumentField merchantAddressDF))
            merchantAddress = merchantAddressDF.FieldType == DocumentFieldType.String ? merchantAddressDF.ValueString : null;

        DateTimeOffset? transactionDate = null;
        if (receiptFields.TryGetValue("TransactionDate", out DocumentField transactionDateDF))
            transactionDate = transactionDateDF.FieldType == DocumentFieldType.Date ? transactionDateDF.ValueDate : null;

        TimeSpan? transactionTime = null;
        if (receiptFields.TryGetValue("TransactionTime", out DocumentField transactionTimeDF))
            transactionTime = transactionTimeDF.FieldType == DocumentFieldType.Time ? transactionTimeDF.ValueTime : null;

        DateTime? transactionDateTime = GetDateTime(transactionDate, transactionTime);

        Currency? subTotal = null;
        if (receiptFields.TryGetValue("Subtotal", out DocumentField subtotalDF))
        {
            CurrencyValue? subTotalC = subtotalDF.FieldType == DocumentFieldType.Currency ? subtotalDF.ValueCurrency : null;
            subTotal = subTotalC is not null ? Currency.Create(subTotalC.Amount, subTotalC.CurrencyCode) : null; 
        }

        Currency? totalTax = null;
        if (receiptFields.TryGetValue("TotalTax", out DocumentField totalTaxDF))
        {
            CurrencyValue? totalTaxC = totalTaxDF.FieldType == DocumentFieldType.Currency ? totalTaxDF.ValueCurrency : null;
            totalTax = totalTaxC is not null ? Currency.Create(totalTaxC.Amount, totalTaxC.CurrencyCode) : null;
        }

        Currency? tip = null;
        if (receiptFields.TryGetValue("TotalTax", out DocumentField tipDF))
        {
            CurrencyValue? tipC = tipDF.FieldType == DocumentFieldType.Currency ? tipDF.ValueCurrency : null;
            totalTax = tipC is not null ? Currency.Create(tipC.Amount, tipC.CurrencyCode) : null;
        }


        Currency? total = null;
        if(receiptFields.TryGetValue("Total", out DocumentField totalDF))
        {
            CurrencyValue? totalC = totalDF.FieldType == DocumentFieldType.Currency ? totalDF.ValueCurrency : null;
            total = totalC is not null ? Currency.Create(totalC.Amount, totalC.CurrencyCode) : null;
        }
        return new(merchantName ?? String.Empty, merchantAddress ?? String.Empty, transactionDateTime ?? DateTime.MinValue, subTotal??0M, totalTax??0M, tip??0M, total??0M);
    }
    #endregion

    #region Receipt items
    public record ReceiptItemInfo(string Description, double Quantity, string QuantityUnit, Currency Price, string ProductCode, Currency TotalPrice);
    private static IEnumerable<ReceiptItemInfo?>  GetItemInfos(AnalyzedDocument receipt)
    {
        if (!receipt.Fields.TryGetValue("Items", out DocumentField itemsFieldDF))
            return [];

        if (itemsFieldDF.FieldType != DocumentFieldType.List)
            return [];

        return itemsFieldDF.ValueList.Select(x => GetItemInfo(x));
    }
    
    private static ReceiptItemInfo GetItemInfo(DocumentField itemField)
    {
        if (itemField.FieldType != DocumentFieldType.Dictionary)
            throw new ArgumentException("Unexpected field type for items. Expected field type is DocumentFieldType.Dictionary", nameof(itemField));

        var itemFields = itemField.ValueDictionary;

        string? description = null;
        if (itemFields.TryGetValue("Description", out DocumentField? descriptionDF))
            description = descriptionDF?.FieldType == DocumentFieldType.String ? descriptionDF.ValueString : null;

        Currency? price = null;
        if(itemFields.TryGetValue("Price", out DocumentField? priceDF))
        {
            CurrencyValue? priceValue = priceDF?.FieldType == DocumentFieldType.Currency ? priceDF.ValueCurrency : null;
            price = priceValue is not null ? Currency.Create(priceValue.Amount, priceValue.CurrencyCode) : null;
        }

        double? quantity = null;
        if (itemFields.TryGetValue("Quantity", out DocumentField? quantityDF))
            quantity = quantityDF?.FieldType == DocumentFieldType.Double ? quantityDF.ValueDouble : null;

        string? quantityUnit = null;
        if (itemFields.TryGetValue("QuantityUnit", out DocumentField? quantityUnitDF))
            quantityUnit = quantityUnitDF?.FieldType == DocumentFieldType.String ? quantityUnitDF.ValueString : null;

        string? productCode = null;
        if (itemFields.TryGetValue("ProductCode", out DocumentField? productCodeDF))
            productCode = productCodeDF?.FieldType == DocumentFieldType.String ? productCodeDF.ValueString : null;

        Currency? totalPrice = null;
        if(itemFields.TryGetValue("TotalPrice", out DocumentField? totalPriceDF))
        {
            CurrencyValue? totalPriceC = totalPriceDF.FieldType == DocumentFieldType.Currency ? totalPriceDF.ValueCurrency : null;
            totalPrice = totalPriceC is not null ? Currency.Create(totalPriceC.Amount, totalPriceC.CurrencyCode) : null;
        }

        return new(description ?? String.Empty, quantity ?? 0, quantityUnit ?? String.Empty, price ?? Currency.Empty, productCode ?? String.Empty, totalPrice ?? Currency.Empty);
    }
    #endregion

    #region Tax details
    public record TaxDetailInfo(string Description, decimal? Rate = null, Currency? Amount = null);
    private static IEnumerable<TaxDetailInfo> GetTaxDetails(AnalyzedDocument receipt)
    {
        if (!receipt.Fields.TryGetValue("TaxDetails", out DocumentField taxDetailsDF))
            return [];

        if (taxDetailsDF.FieldType != DocumentFieldType.List)
            return [];

        return taxDetailsDF.ValueList.Select(x => GetTaxDetail(x));
    }

    private static TaxDetailInfo GetTaxDetail(DocumentField taxField)
    {
        if (taxField.FieldType != DocumentFieldType.Dictionary)
            throw new ArgumentException("Unexpected field type for tax details. Expected field type is DocumentFieldType.Dictionary", nameof(taxField));

        var taxFields = taxField.ValueDictionary;

        string description = string.Empty;
        if (taxFields.TryGetValue("Description", out DocumentField? descriptionDF))
            description = descriptionDF?.FieldType == DocumentFieldType.String ? descriptionDF.ValueString : string.Empty;

        Currency? amount = null; 
        if(taxFields.TryGetValue("Amount", out DocumentField? amountDF))
        {
            CurrencyValue? amountValue = amountDF?.FieldType == DocumentFieldType.Currency ? amountDF.ValueCurrency : null;
            amount = amountValue is null ? null : Currency.Create(amountValue.Amount, amountValue.CurrencyCode); 
        }

        decimal? rate = null;
        if (taxFields.TryGetValue("Rate", out DocumentField? rateDF))
            rate = rateDF?.FieldType == DocumentFieldType.Double ? Convert.ToDecimal(rateDF.ValueDouble) : null;
        return new(description, rate, amount);

    }
    #endregion

    #region Payment details
    public record PaymentDetailInfo(string? Method, Currency? Amount);

    private static PaymentDetailInfo GetPaymentDetail(DocumentField paymentField)
    {
        if (paymentField.FieldType != DocumentFieldType.Dictionary)
            throw new ArgumentException("Unexpected field type for tax details. Expected field type is DocumentFieldType.Dictionary", nameof(paymentField));

        var paymentFields = paymentField.ValueDictionary;

        string? method = string.Empty;
        if (paymentFields.TryGetValue("Method", out DocumentField? descriptionDF))
            method = descriptionDF?.FieldType == DocumentFieldType.String ? descriptionDF.ValueString : string.Empty;

        Currency? amount = null;
        if (paymentFields.TryGetValue("Amount", out DocumentField? amountDF))
        {
            CurrencyValue? amountValue = amountDF?.FieldType == DocumentFieldType.Currency ? amountDF.ValueCurrency : null;
            amount = amountValue is null ? null : Currency.Create(amountValue.Amount, amountValue.CurrencyCode);
        }

        return new(method, amount??Currency.Empty);
    }

    private static IEnumerable<PaymentDetailInfo> GetPaymentDetails(AnalyzedDocument receipt)
    {
        if (!receipt.Fields.TryGetValue("Payments", out DocumentField taxDetailsDF))
            return [];

        if (taxDetailsDF.FieldType != DocumentFieldType.List)
            return [];

        return taxDetailsDF.ValueList.Select(x => GetPaymentDetail(x));
    }

    #endregion

    private static DateTime? GetDateTime(DateTimeOffset? date, TimeSpan? time)
    {
        if (date is null && time is null)
            return null;

        int year = date?.Year ?? 0;
        int month = date?.Month ?? 0;
        int day = date?.Day ?? 0;

        int hour = time?.Hours ?? date?.Hour ?? 0;
        int minute = time?.Minutes ?? date?.Minute ?? 0;
        int second = time?.Seconds ?? date?.Second ?? 0;

        return DateTime.MinValue.AddYears(year).AddMonths(month).AddDays(day).AddHours(hour).AddMinutes(minute).AddSeconds(second);

    }
}
