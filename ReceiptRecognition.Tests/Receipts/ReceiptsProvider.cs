using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReceiptRecognition.Tests.Receipts;

internal class ReceiptsProvider
{
    internal class Shops { 
        public const string Kiabi = "Shops/Kiabi.jpg";
        public const string Aldi = "Shops/Aldi.jpg";
        public const string AlpesBureau = "AlpesBureau.jpg";
    }

    const string BasePath = "./Receipts/";

    public Stream GetReceiptByName(string receiptPath)
    {
        string path = Path.Combine(BasePath, receiptPath);
        if (!File.Exists(path))
            Assert.Fail("File not exists");

        FileStream fstream = new FileStream(path, FileMode.Open, FileAccess.Read);
        return fstream;
    } 


}
