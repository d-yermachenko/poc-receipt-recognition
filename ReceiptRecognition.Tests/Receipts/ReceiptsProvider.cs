using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiptRecognition.Tests.Receipts;

internal class ReceiptsProvider
{
    internal class Shops { 
        public const string Kiabi = "Shops/Kiabi.jpg";
        public const string Aldi = "Shops/Aldi.jpg";
        public const string AlpesBureau = "Shops/AlpesBureau.jpg";
    }

    static string BasePath => $"Receipts{Path.DirectorySeparatorChar}";

#pragma warning disable CA1822, IDE0079 // Mark members as static
    public Stream GetReceiptByName(string receiptPath)
#pragma warning restore CA1822, IDE0079 // Mark members as static
    {
        receiptPath = receiptPath.Replace('/', Path.DirectorySeparatorChar);
    ///C: \Users\dyerm\source\ReceiptRecognition\ReceiptRecognition.Tests\bin\Debug\net9.0\Receipts\Shops
        string path = Path.Combine(Directory.GetCurrentDirectory(), BasePath, receiptPath);
        if (!File.Exists(path))
            Assert.Fail("File not exists");

        FileStream fstream = new (path, FileMode.Open, FileAccess.Read);
        return fstream;
    }
}
