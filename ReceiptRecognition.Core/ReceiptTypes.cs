using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ReceiptRecognition.Core;

public enum SupportedReceiptType
{
    Unknown,
    Retail,
    RetailMeal,
    Restaurant,
    Gas,
    TallRoad,
    EventOrTransportation,
    BankCard
}
