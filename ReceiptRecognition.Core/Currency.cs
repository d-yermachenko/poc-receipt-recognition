using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiptRecognition.Core;

public record Currency(decimal Amount, string? CurrencyCode)
{
    public static Currency Empty => new(0, "");

    public static implicit operator Currency(decimal value) => new (value, "");

    public static Currency Create(double amount, string? currencyCode) => new(Convert.ToDecimal(amount), currencyCode);
}
