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
        internal class France
        {
            public const string Kiabi = "Shops/France/Kiabi.jpg";
            public const string Aldi = "Shops/France/Aldi.jpg";
            public const string AlpesBureau = "Shops/France/AlpesBureau.jpg";
        }

        internal class USA
        {
            public const string GrosseryDepot = "Shops/USA/GrosseryDepot.jpg";
            public const string LaV = "Shops/USA/LaV.jpg";
            public const string MegaBigBox  = "Shops/USA/MegaBigBox.png";
            public const string StapleStore  = "Shops/USA/StapleStore.jpg";
            public const string Target = "Shops/USA/Target.jpg";
            public const string Target2016 = "Shops/USA/Target2016.jpeg";
            public const string Wallmart = "Shops/USA/Wallmart.jpg";
            public const string Wallmart2020 = "Shops/USA/Wallmart2020.png";
            public const string WholeFoods = "Shops/USA/WholeFoods.jpeg";
        }
    }

    internal class GasStations
    {
        public const string IntermarcheResized = "GasStation/Intermarche.jpg";
        public const string IntermarcheRaw = "GasStation/Intermarche-2.jpg";
    }

    internal class Parking
    {
        public const string HopitalSavoie = "Parking/HS.jpeg";
    }


    internal class Restaurant
    {
        internal class USA
        {
            public const string AmericanFood = "Restaurants/USA/AmericanFood.jpg";
            public const string AyaBeach = "Restaurants/USA/AyaBeach.jpg";
            public const string BarneysBeanery = "Restaurants/USA/BarneysBeanery.jpg";
            public const string BreackfastClub = "Restaurants/USA/BreackfastClub.jpg";
            public const string ElChalan = "Restaurants/USA/ElChalan.jpg";
            public const string ErrinSnug ="Restaurants/USA/ErrinSnug";
            public const string GoodsRestaurant = "Restaurants/USA/GoodsRestaurant.jpg";
            public const string HotelRestaurant = "Restaurants/USA/HotelRestaurant.jpg";
            public const string Lolo = "Restaurants/USA/Lolo.jpg";
            public const string MontanaRestaurant = "Restaurants/USA/MontanaRestaurant.png";
            public const string NexxtCafe ="Restaurants/USA/NexxtCafe.jpg";
            public const string PasificaRestaurant = "Restaurants/USA/PasificaRestaurant.jpg";
            public const string PlaceToEat ="Restaurants/USA/PlaceToEat.jpg";
            public const string SeasideSushiHouse = "Restaurants/USA/SeasideSushiHouse.jpg";
            public const string Station5 = "Restaurants/USA/Station5.jpg";
            public const string WaterView = "Restaurants/USA/WaterView.jpg";
        }

        internal class France
        {
            public const string BillTheButcher = "Restaurants/France/Bill-The-Butcher.jpg";
            public const string BistrotDuMarche = "Restaurants/France/bistrot-du-marche.jpg";
            public const string BrasserieLaissac = "Restaurants/France/Brasserie Laissac.jpg";
            public const string HiBoutique = "Restaurants/France/HiBoutique.png";
            public const string HiBoutique2 = "Restaurants/France/HiBoutique2.png";
            public const string LaGardere = "Restaurants/France/LaGardere.jpg";
            public const string Lilois = "Restaurants/France/Lilois.jpg";
            public const string McDonalds = "Restaurants/France/McDonalds.jpg";
            public const string NoNameMarchant = "Restaurants/France/NoNameMarchant.jpg";
            public const string SetteHolding = "Restaurants/France/SetteHolding.jpg";
            public const string Ticket = "Restaurants/France/ticket.gif";
        }

        internal class Swiss
        {
            public const string CashFlow = "Restaurants/Swiss/CashFlow.jpg";
        }
    }

    static string BasePath => $"Receipts{Path.DirectorySeparatorChar}";

    #pragma warning disable CA1822, IDE0079 // Mark members as static
    public Stream GetReceiptByName(string receiptPath)
    #pragma warning restore CA1822, IDE0079 // Mark members as static
    {
        receiptPath = receiptPath.Replace('/', Path.DirectorySeparatorChar);
    ///C: \Users\dyerm\source\ReceiptRecognition\ReceiptRecognition.Tests\bin\Debug\net9.0\Receipts\Shops
        string path = Path.Combine(Directory.GetCurrentDirectory(), "../../../", BasePath, receiptPath);
        if (!File.Exists(path))
            Assert.Fail($"File {path} not exists");

        FileStream fstream = new (path, FileMode.Open, FileAccess.Read);
        return fstream;
    }
}
