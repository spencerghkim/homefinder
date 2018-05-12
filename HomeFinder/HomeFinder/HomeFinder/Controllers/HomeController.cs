

namespace HomeFinder.Controllers
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using Newtonsoft.Json;
    using System.Net;
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        private static readonly string ACCOUNT_NAME = "homefinder";
        private static readonly string ACCESS_KEY = "+Ekr3DjSPY90U5AzEBYF3kNuLJiBqUyfI3Z4ET1u+CBSAkR0RuJNy0dKer93CHF7Ur9leIBL1boerG0NhOjUGA==";
        private static readonly string TABLE_NAME = "LocationData";
        private static readonly string[] PARTITIONS = { "gym", "connectors", "apartment" };
        private static readonly string STORAGE_CONFIGURATION_KEY =
            "DefaultEndpointsProtocol=https;AccountName=homefinder;AccountKey=+Ekr3DjSPY90U5AzEBYF3kNuLJiBqUyfI3Z4ET1u+CBSAkR0RuJNy0dKer93CHF7Ur9leIBL1boerG0NhOjUGA==;EndpointSuffix=core.windows.net";

        public ActionResult Index()
        {
            return View();
        }

        // 
        // GET: /Home/QueryLocations/ 
        public string QueryLocations(string type)
        {
            if (type == null)
            {
                Dictionary<string, List<ApartmentEntity>> allLocations = new Dictionary<string, List<ApartmentEntity>>();
                foreach (string LocationType in PARTITIONS)
                {
                    allLocations[LocationType] = RequestResource(TABLE_NAME, LocationType);
                }
                return JsonConvert.SerializeObject(allLocations);
            } else
            {
                return JsonConvert.SerializeObject(RequestResource(TABLE_NAME, type));
            }
        }

        // 
        // GET: /Home/QueryApartments/
        // http://localhost:3437/home/QueryApartments?maxWalkingDistanceToConnector=1
        // http://localhost:3437/home/QueryApartments?maxWalkingDistanceToGym=1
        // http://localhost:3437/home/QueryApartments?maxWalkingDistanceToConnector=1&maxWalkingDistanceToGym=1
        public string QueryApartments(int maxWalkingDistanceToConnector = int.MaxValue, int maxWalkingDistanceToGym = int.MaxValue)
        {
            List<ApartmentEntity> apartments = RequestResource(TABLE_NAME, "apartment");
            List<ApartmentEntity> filteredApartments = new List<ApartmentEntity>();

            foreach (var a in apartments)
            {
                if (a.DistanceToClosestConnector <= maxWalkingDistanceToConnector
                    && a.DistanceToClosestGym <= maxWalkingDistanceToGym)
                {
                    filteredApartments.Add(a);
                }
            }
            return JsonConvert.SerializeObject(filteredApartments);
        }

        public string Test()
        {
            string Jsondata =
                "[{\"name\":\"Wanna walk to everywhere? You have to Live Here First!\", \"area\":\"572ft2\", \"url\":\"http://seattle.craigslist.org/see/apa/d/wanna-walk-to-everywhere/6234302475.html\", \"where\":\"Seattle, Wa\", \"price\":\"$2138\", \"bedrooms\":\"1\", \"geotag\": \"(47.65092, -122.34242)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:08\", \"has_map\": true, \"id\":\"6234302475\"},{\"name\":\"Think tank conference room & theater, Two lush courtyards, Solarium\", \"area\":\"566ft2\", \"url\":\"http://seattle.craigslist.org/see/apa/d/think-tank-conference-room/6216442179.html\", \"where\":\"South Lake Union\", \"price\":\"$2035\", \"bedrooms\": \"0\", \"geotag\": \"(47.62313, -122.331442)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:06\", \"has_map\": true, \"id\":\"6216442179\"},{\"name\":\"Wood Floors, Quiet, Quartz, Easy to Downtown, Lite Rail, 1 Mo Free\", \"area\": \"0\", \"url\":\"http://seattle.craigslist.org/see/apa/d/wood-floors-quiet-quartz/6234299811.html\", \"where\":\"Capitol Hill\", \"price\":\"$1150\", \"bedrooms\": \"0\", \"geotag\": \"(47.616116, -122.315168)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:06\", \"has_map\": true, \"id\":\"6234299811\"},{\"name\":\"Micro/convection oven, City and water views , Package valet kiosk\", \"area\":\"252ft2\", \"url\":\"http://seattle.craigslist.org/see/apa/d/micro-convection-oven-city/6234285819.html\", \"where\":\"1731 NW 57th Street, Seattle, WA 98107\", \"price\":\"$1270\", \"bedrooms\": \"0\", \"geotag\": \"(47.67012, -122.379151)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:06\", \"has_map\": true, \"id\":\"6234285819\"},{\"name\":\"Top floor luxury apartment on Upper Queen Anne\", \"area\":\"550ft2\", \"url\":\"http://seattle.craigslist.org/see/apa/d/top-floor-luxury-apartment/6234297783.html\", \"where\":\"Queen Anne\", \"price\":\"$1995\", \"bedrooms\":\"1\", \"geotag\": \"(47.63832, -122.352967)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:05\", \"has_map\": true, \"id\":\"6234297783\"},{\"name\":\"1 Month Free Rent!! Top Floor Studio!!\", \"area\":\"252ft2\", \"url\":\"http://seattle.craigslist.org/see/apa/d/month-free-rent-top-floor/6231444482.html\", \"where\":\"1731 NW 57th Street, Seattle, WA 98107\", \"price\":\"$1270\", \"bedrooms\": \"0\", \"geotag\": \"(47.67012, -122.379151)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:05\", \"has_map\": true, \"id\":\"6231444482\"},{\"name\":\"Rare top floor corner unit on top of Queen Anne\", \"area\":\"550ft2\", \"url\":\"http://seattle.craigslist.org/see/apa/d/rare-top-floor-corner-unit/6234297183.html\", \"where\":\"Queen Anne\", \"price\":\"$1995\", \"bedrooms\":\"1\", \"geotag\": \"(47.63832, -122.352967)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:05\", \"has_map\": true, \"id\":\"6234297183\"},{\"name\":\"City views , Wood style flooring, Washer & dryer in every home\", \"area\":\"642ft2\", \"url\":\"http://seattle.craigslist.org/see/apa/d/city-views-wood-style/6234297002.html\", \"where\":\"165 17th Avenue, Seattle, WA 98122\", \"price\":\"$1730\", \"bedrooms\":\"1\", \"geotag\": \"(47.602982, -122.310136)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:05\", \"has_map\": true, \"id\":\"6234297002\"},{\"name\":\"Urban 1 Bedroom 1 Bath For This Rate- Apply Today!\", \"area\":\"586ft2\", \"url\":\"http://seattle.craigslist.org/see/apa/d/urban-bedroom-bath-for-this/6234295936.html\", \"where\":\"Seattle\", \"price\":\"$1843\", \"bedrooms\":\"1\", \"geotag\": \"(47.625593, -122.344938)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:04\", \"has_map\": true, \"id\":\"6234295936\"},{\"name\":\"Beautiful View From This One Bedroom Plus Den\", \"area\":\"790ft2\", \"url\":\"http://seattle.craigslist.org/see/apa/d/beautiful-view-from-this/6234295090.html\", \"where\":\"Seattle\", \"price\":\"$2640\", \"bedrooms\":\"1\", \"geotag\": \"(47.62041, -122.330437)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:03\", \"has_map\": true, \"id\":\"6234295090\"},{\"name\":\"Green Urban Living @ Greenhouse Apartments! Prelease your home NOW *\", \"area\":\"528ft2\", \"url\":\"http://seattle.craigslist.org/see/apa/d/green-urban-living/6234294235.html\", \"where\":\"3701 S. Hudson St., Seattle, WA 98118\", \"price\":\"$1729\", \"bedrooms\":\"1\", \"geotag\": \"(47.556896, -122.28812)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:03\", \"has_map\": true, \"id\":\"6234294235\"},{\"name\":\"TWO UNITS AVAIL...  Great Neighborhood, Close to UW 1 bed 1 bath,\", \"area\":\"925ft2\", \"url\":\"http://seattle.craigslist.org/see/apa/d/two-units-avail-great/6218624114.html\", \"where\":\"MONTLAKE\", \"price\":\"$1450\", \"bedrooms\":\"1\", \"geotag\": \"0\", \"has_image\": true, \"datetime\":\"2017-07-24 15:02\", \"has_map\": true, \"id\":\"6218624114\"},{\"name\":\"Northgate/ Pinehurst 1 br apt.\", \"area\":\"630ft2\", \"url\":\"http://seattle.craigslist.org/see/apa/d/northgate-pinehurst-br-apt/6226670232.html\", \"where\":\"11745 15th Ave. N.E., Northgate\", \"price\":\"$1350\", \"bedrooms\":\"1\", \"geotag\": \"(47.714959, -122.313023)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:02\", \"has_map\": true, \"id\":\"6226670232\"},{\"name\":\"Wood floors, Quiet, Quartz, Easy to Downtown, Lite Rail, 1 Mo free\", \"area\": \"0\", \"url\":\"http://seattle.craigslist.org/see/apa/d/wood-floors-quiet-quartz/6234292883.html\", \"where\":\"Capitol Hill\", \"price\":\"$1150\", \"bedrooms\": \"0\", \"geotag\": \"(47.616144, -122.315254)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:02\", \"has_map\": true, \"id\":\"6234292883\"},{\"name\":\"Eastlake 2 br 2 ba Apt.\", \"area\": \"0\", \"url\":\"http://seattle.craigslist.org/see/apa/d/eastlake-br-ba-apt/6196786975.html\", \"where\":\"2010 Franklin Ave. E., Eastlake\", \"price\":\"$1800\", \"bedrooms\":\"2\", \"geotag\": \"(47.63832, -122.324637)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:02\", \"has_map\": true, \"id\":\"6196786975\"},{\"name\":\"Seattle 3 br 2 ba house\", \"area\": \"0\", \"url\":\"http://seattle.craigslist.org/see/apa/d/seattle-br-ba-house/6218296710.html\", \"where\":\"9732 45th Ave. N.E.\", \"price\":\"$2100\", \"bedrooms\":\"3\", \"geotag\": \"(47.6849, -122.2968)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:02\", \"has_map\": true, \"id\":\"6218296710\"},{\"name\":\"Shoreline 2 br house\", \"area\": \"0\", \"url\":\"http://seattle.craigslist.org/see/apa/d/shoreline-br-house/6218246278.html\", \"where\":\"16041 27th Ave. N.E.\", \"price\":\"$1850\", \"bedrooms\": \"0\", \"geotag\": \"(47.7559, -122.3003)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:01\", \"has_map\": true, \"id\":\"6218246278\"},{\"name\":\"City Living and the Best Price! Studio Home Waiting for YOU!\", \"area\":\"400ft2\", \"url\":\"http://seattle.craigslist.org/see/apa/d/city-living-and-the-best/6234291650.html\", \"where\":\"first hill\", \"price\":\"$1250\", \"bedrooms\": \"0\", \"geotag\": \"(47.608934, -122.328949)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:01\", \"has_map\": true, \"id\":\"6234291650\"},{\"name\":\"Traditional Studio. Pet friendly. Come tour today!!\", \"area\":\"440ft2\", \"url\":\"http://seattle.craigslist.org/see/apa/d/traditional-studio-pet/6234291511.html\", \"where\":\"5240 University Way NE Seattle, WA\", \"price\":\"$1495\", \"bedrooms\": \"0\", \"geotag\": \"(47.666515, -122.312937)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:01\", \"has_map\": true, \"id\":\"6234291511\"},{\"name\":\"West Seattle water view at the Tracy Ann\", \"area\":\"800ft2\", \"url\":\"http://seattle.craigslist.org/see/apa/d/west-seattle-water-view-at/6234291155.html\", \"where\":\"Seattle\", \"price\":\"$1325\", \"bedrooms\":\"1\", \"geotag\": \"(47.542055, -122.393606)\", \"has_image\": true, \"datetime\":\"2017-07-24 15:01\", \"has_map\": true, \"id\":\"6234291155\"}]";
            List<Apartment> parsed = JsonConvert.DeserializeObject<List<Apartment>>(Jsondata);
            foreach (Apartment apartment in parsed)
            {
                string geotag = apartment.geotag;
                if (geotag.IndexOf("(") < 0)
                {
                    continue;
                }
                string lat = geotag.Substring(1, geotag.IndexOf(",") - 1);
                int a = geotag.IndexOf(", ") + 2;
                int b = geotag.Length - 1;
                string log = geotag.Substring(a, b - a);
                string price = apartment.price;
                if (price.IndexOf("$") > -1)
                {
                    price = price.Substring(price.IndexOf("$") + 1);
                }
                int priceInt = Int32.Parse(price);
                UpdateLocation(
                    "apartment",
                    lat,
                    log,
                    apartment.where,
                    apartment.name,
                    priceInt,
                    0.0,
                    0.0);
            }
            return parsed[0].name;
            //string JsonData =
            //    "[{\"ID\":53,\"Name\":\"108TH AVE NE & NE 6TH ST\",\"Address\":\"580 108th Ave NE\",\"City\":\"BELLEVUE\",\"Latitude\":47.615197,\"LatitudeWaypoint\":null,\"Longitude\":-122.196252,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98004\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":true,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":68,\"Name\":\"16TH AVE. E. & E. THOMAS ST.\",\"Address\":\"16th Ave E & E Thomas St\",\"City\":\"SEATTLE\",\"Latitude\":47.620376,\"LatitudeWaypoint\":null,\"Longitude\":-122.311398,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98112\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":73,\"Name\":\"18TH AVE & E CHERRY STREET\",\"Address\":\"18TH AVE & E CHERRY STREET\",\"City\":\"SEATTLE\",\"Latitude\":47.60802,\"LatitudeWaypoint\":null,\"Longitude\":-122.30893,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98122\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":69,\"Name\":\"19TH AVE E & E HARRISON ST\",\"Address\":\"19th Ave E & E Harrison St\",\"City\":\"SEATTLE\",\"Latitude\":47.621934,\"LatitudeWaypoint\":null,\"Longitude\":-122.307357,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98112\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":52,\"Name\":\"1ST AVE & CEDAR ST\",\"Address\":\"1st Ave & Cedar St\",\"City\":\"SEATTLE\",\"Latitude\":47.615984,\"LatitudeWaypoint\":null,\"Longitude\":-122.350761,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98121\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":66,\"Name\":\"1ST AVE N & JOHN ST\",\"Address\":\"1st Ave N & John St\",\"City\":\"SEATTLE\",\"Latitude\":47.619972,\"LatitudeWaypoint\":null,\"Longitude\":-122.35543,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98109\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":62,\"Name\":\"2490 N 65th ST (PM)\",\"Address\":\"2490 N 65th St\",\"City\":\"SEATTLE\",\"Latitude\":47.675942,\"LatitudeWaypoint\":null,\"Longitude\":-122.328439,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98103\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":21,\"Name\":\"24TH AVE NW & NW 67TH ST\",\"Address\":\"6556 24th Ave NW\",\"City\":\"SEATTLE\",\"Latitude\":47.677554,\"LatitudeWaypoint\":null,\"Longitude\":-122.387435,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98117\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":22,\"Name\":\"24TH AVE NW & NW 77TH ST\",\"Address\":\"7704 24th Ave NW\",\"City\":\"SEATTLE\",\"Latitude\":47.685156,\"LatitudeWaypoint\":null,\"Longitude\":-122.387483,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98117\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":3,\"Name\":\"2ND AVE & CEDAR ST\",\"Address\":\"2701 2nd Ave\",\"City\":\"SEATTLE\",\"Latitude\":47.616622,\"LatitudeWaypoint\":null,\"Longitude\":-122.350024,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98121\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":13,\"Name\":\"35TH AVE NE & NE 73RD ST\",\"Address\":\"7050 35th Ave NE\",\"City\":\"SEATTLE\",\"Latitude\":47.681023,\"LatitudeWaypoint\":null,\"Longitude\":-122.290318,\"LongitudeWaypoint\":null,\"StopRadius\":650,\"Description\":null,\"ZipCode\":\"98115\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":14,\"Name\":\"35TH AVE NE & NE 80TH ST\",\"Address\":\"8008 35th Ave NE\",\"City\":\"SEATTLE\",\"Latitude\":47.68649,\"LatitudeWaypoint\":null,\"Longitude\":-122.290487,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98115\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":11,\"Name\":\"40TH AVE NE & NE 55TH ST\",\"Address\":\"5250 40th Ave NE\",\"City\":\"SEATTLE\",\"Latitude\":47.668341,\"LatitudeWaypoint\":null,\"Longitude\":-122.28472,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98105\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":51,\"Name\":\"9 W MCGRAW\",\"Address\":\"9 W McGraw St\",\"City\":\"SEATTLE\",\"Latitude\":47.639545,\"LatitudeWaypoint\":null,\"Longitude\":-122.357003,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98109\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":6,\"Name\":\"BOTHELL STAPLES\",\"Address\":\"18438 120th Ave NE\",\"City\":\"BOTHELL\",\"Latitude\":47.76083205,\"LatitudeWaypoint\":null,\"Longitude\":-122.17577814,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98011\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":81,\"Name\":\"CALIFORNIA AVE SW & SW SPOKANE ST (SHARED)\",\"Address\":\"California Ave SW & SW Spokane St\",\"City\":\"SEATTLE\",\"Latitude\":47.571801,\"LatitudeWaypoint\":null,\"Longitude\":-122.386825,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98116\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":36,\"Name\":\"CALIFORNIA AVE SW @ ADMIRAL THEATER\",\"Address\":\"2343 California Ave SW\",\"City\":\"SEATTLE\",\"Latitude\":47.581726,\"LatitudeWaypoint\":null,\"Longitude\":-122.386676,\"LongitudeWaypoint\":null,\"StopRadius\":650,\"Description\":null,\"ZipCode\":\"98116\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":39,\"Name\":\"CEDAR PARK CRESCENT NE AND NE 100TH WAY\",\"Address\":\"Cedar Park Cres NE & NE 100th Way\",\"City\":\"REDMOND\",\"Latitude\":47.687832,\"LatitudeWaypoint\":null,\"Longitude\":-122.041605,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98053\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":35,\"Name\":\"CHRISTIAN FAITH CENTER\",\"Address\":\"13000 21st Dr SE\",\"City\":\"EVERETT\",\"Latitude\":47.87887481,\"LatitudeWaypoint\":null,\"Longitude\":-122.20385643,\"LongitudeWaypoint\":null,\"StopRadius\":1000,\"Description\":null,\"ZipCode\":\"98208\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":10,\"Name\":\"COMMONS TRANSPORTATION CENTER\",\"Address\":\"150th Ave NE\",\"City\":\"REDMOND\",\"Latitude\":47.64306987,\"LatitudeWaypoint\":null,\"Longitude\":-122.13992611,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98052\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":true,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":38,\"Name\":\"COVINGTON CORNERSTONE CHURCH\",\"Address\":\"20730 SE 272nd St\",\"City\":\"Kent\",\"Latitude\":47.358409,\"LatitudeWaypoint\":null,\"Longitude\":-122.06376,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98042\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":74,\"Name\":\"E CHERRY ST & 16TH AVE\",\"Address\":\"E CHERRY ST & 16TH AVE\",\"City\":\"SEATTLE\",\"Latitude\":47.60804,\"LatitudeWaypoint\":null,\"Longitude\":-122.31155,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98122\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":58,\"Name\":\"E MADISION ST & 23RD AVE (AM)\",\"Address\":\"E Madison St & 23rd Ave\",\"City\":\"SEATTLE\",\"Latitude\":47.619365,\"LatitudeWaypoint\":null,\"Longitude\":-122.302149,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98112\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":59,\"Name\":\"E MADISION ST & 25TH AVE (PM)\",\"Address\":\"E Madison St & 25rd Ave\",\"City\":\"SEATTLE\",\"Latitude\":47.620834,\"LatitudeWaypoint\":null,\"Longitude\":-122.300261,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98112\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":45,\"Name\":\"E MADISON ST @ CITY PEOPLE`S NURSERY (AM)\",\"Address\":\"2925 E Madison St\",\"City\":\"SEATTLE\",\"Latitude\":47.624349,\"LatitudeWaypoint\":null,\"Longitude\":-122.294792,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98112\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":44,\"Name\":\"E UNION ST AND 33TH AVE (PM)\",\"Address\":\"E Union St & 33rd Ave\",\"City\":\"SEATTLE\",\"Latitude\":47.612872,\"LatitudeWaypoint\":null,\"Longitude\":-122.290495,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98122\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":43,\"Name\":\"E UNION ST AND 34TH AVE (AM)\",\"Address\":\"E Union St & 34th Ave\",\"City\":\"SEATTLE\",\"Latitude\":47.613129,\"LatitudeWaypoint\":null,\"Longitude\":-122.290111,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98122\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":67,\"Name\":\"E. PIKE ST. & SUMMIT AVE.\",\"Address\":\"E Pike St & Summit Ave\",\"City\":\"SEATTLE\",\"Latitude\":47.614092,\"LatitudeWaypoint\":null,\"Longitude\":-122.32567,\"LongitudeWaypoint\":null,\"StopRadius\":700,\"Description\":null,\"ZipCode\":\"98122\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":57,\"Name\":\"EASTRIDGE DR NE & POWERLINES\",\"Address\":\"Eastridge Dr NE & Powerlines\",\"City\":\"REDMOND\",\"Latitude\":47.697198,\"LatitudeWaypoint\":null,\"Longitude\":-122.018875,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98053\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":33,\"Name\":\"EVERGREEN STATE FAIRGROUNDS\",\"Address\":\"14405 179th Ave SE\",\"City\":\"MONROE\",\"Latitude\":47.86687254,\"LatitudeWaypoint\":null,\"Longitude\":-121.99006901,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98272\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":15,\"Name\":\"GREENWOOD AVE N & N 73RD ST\",\"Address\":\"7307 Greenwood Ave N\",\"City\":\"SEATTLE\",\"Latitude\":47.682113,\"LatitudeWaypoint\":null,\"Longitude\":-122.35541,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98103\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":20,\"Name\":\"GREENWOOD AVE N & N 75TH ST\",\"Address\":\"Greenwood Ave N & N 75th St\",\"City\":\"SEATTLE\",\"Latitude\":47.683497,\"LatitudeWaypoint\":null,\"Longitude\":-122.355152,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98103\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":54,\"Name\":\"HIGHLANDS COMMUNITY CHURCH\",\"Address\":\"3031 NE 10th St\",\"City\":\"RENTON\",\"Latitude\":47.497344,\"LatitudeWaypoint\":null,\"Longitude\":-122.17861,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98056\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":9,\"Name\":\"HOLY INNOCENTS CATHOLIC CHURCH\",\"Address\":\"26526 NE Cherry Valley Rd\",\"City\":\"DUVALL\",\"Latitude\":47.747887,\"LatitudeWaypoint\":null,\"Longitude\":-121.982896,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98019\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":64,\"Name\":\"ISSAQUAH HIGHLANDS\",\"Address\":\"10th Ave NE\",\"City\":\"ISSAQUAH\",\"Latitude\":47.541288,\"LatitudeWaypoint\":null,\"Longitude\":-122.015946,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98029\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":41,\"Name\":\"KENT SEVENTH DAY ADVENTIST CHURCH\",\"Address\":\"25213 116th Ave SE\",\"City\":\"KENT\",\"Latitude\":47.375639,\"LatitudeWaypoint\":null,\"Longitude\":-122.188209,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98030\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":50,\"Name\":\"LAKE WASHINGTON BLVD E AND E MADISON ST (PM)\",\"Address\":\"Lake Washington Blvd E & E Madison St\",\"City\":\"SEATTLE\",\"Latitude\":47.625855,\"LatitudeWaypoint\":null,\"Longitude\":-122.29285,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98112\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":42,\"Name\":\"LESCHI PARK (PARKING LOT)\",\"Address\":\"201 Lakeside Ave S\",\"City\":\"SEATTLE\",\"Latitude\":47.601114,\"LatitudeWaypoint\":null,\"Longitude\":-122.285348,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98144\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":55,\"Name\":\"MARS HILL CHURCH\",\"Address\":\"120 228th Ave NE\",\"City\":\"SAMMAMISH\",\"Latitude\":47.61022841,\"LatitudeWaypoint\":null,\"Longitude\":-122.03460699,\"LongitudeWaypoint\":null,\"StopRadius\":650,\"Description\":null,\"ZipCode\":\"98074\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":5,\"Name\":\"MILLCREEK FOURSQUARE CHURCH\",\"Address\":\"1415 164th St SW\",\"City\":\"LYNNWOOD\",\"Latitude\":47.85046537,\"LatitudeWaypoint\":null,\"Longitude\":-122.25343971,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98087\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":49,\"Name\":\"MT RAINIER DR S AND S MCCLELLAN ST (PM)\",\"Address\":\"Mount Rainier Dr S & S McClellan St\",\"City\":\"SEATTLE\",\"Latitude\":47.577825,\"LatitudeWaypoint\":null,\"Longitude\":-122.287617,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98144\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":25,\"Name\":\"N 35TH ST & WOODLAND PARK AVE N (AM)\",\"Address\":\"1051 N 35th AVE\",\"City\":\"SEATTLE\",\"Latitude\":47.649653,\"LatitudeWaypoint\":null,\"Longitude\":-122.344348,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98103\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":32,\"Name\":\"N 35TH ST & WOODLAND PARK AVE N (PM)\",\"Address\":\"Woodland Park Ave N & N 35th St\",\"City\":\"SEATTLE\",\"Latitude\":47.649682,\"LatitudeWaypoint\":null,\"Longitude\":-122.343723,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98103\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":24,\"Name\":\"N 36TH ST & PHINNEY AVE N\",\"Address\":\"401 N 36th ST\",\"City\":\"SEATTLE\",\"Latitude\":47.652109,\"LatitudeWaypoint\":null,\"Longitude\":-122.354074,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98103\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":28,\"Name\":\"N 45TH ST & BAGLEY AVE N\",\"Address\":\"2115 N 45th ST\",\"City\":\"SEATTLE\",\"Latitude\":47.661327,\"LatitudeWaypoint\":null,\"Longitude\":-122.333,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98103\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":29,\"Name\":\"N 45TH ST & MERIDIAN AVE N\",\"Address\":\"Meridian Ave N & N 45th St\",\"City\":\"SEATTLE\",\"Latitude\":47.661451,\"LatitudeWaypoint\":null,\"Longitude\":-122.333316,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98103\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":82,\"Name\":\"NE 65TH AVE AND 39TH AVE NE (SHARED)\",\"Address\":\"NE 65th St & 39th Ave NE\",\"City\":\"SEATTLE\",\"Latitude\":47.675807,\"LatitudeWaypoint\":null,\"Longitude\":-122.285824,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98115\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":17,\"Name\":\"NE 65TH ST & LATONA AVE NE - (AM)\",\"Address\":\"6411 Latona Ave NE\",\"City\":\"SEATTLE\",\"Latitude\":47.675867,\"LatitudeWaypoint\":null,\"Longitude\":-122.325645,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98115\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":56,\"Name\":\"NE CEDAR PARK CRESCENT & EASTRIDGE DR NE\",\"Address\":\"NE Cedar Park Crescent & Eastridge Dr NE\",\"City\":\"REDMOND\",\"Latitude\":47.691234,\"LatitudeWaypoint\":null,\"Longitude\":-122.017663,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98053\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":80,\"Name\":\"NW MARKET ST & 20TH AVE NW (SHARED)\",\"Address\":\"NW Market St & 20th Ave NW\",\"City\":\"SEATTLE\",\"Latitude\":47.668705,\"LatitudeWaypoint\":null,\"Longitude\":-122.382389,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98107\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":23,\"Name\":\"OUR REDEEMERS LUTHERAN CHURCH\",\"Address\":\"2400 NW 85th St\",\"City\":\"SEATTLE\",\"Latitude\":47.691183,\"LatitudeWaypoint\":null,\"Longitude\":-122.389294,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98117\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":1,\"Name\":\"OVERLAKE TRANSIT CENTER\",\"Address\":\"156TH AVE NE & NE 40TH ST\",\"City\":\"REDMOND\",\"Latitude\":47.644307,\"LatitudeWaypoint\":null,\"Longitude\":-122.132564,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98052\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":true,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":84,\"Name\":\"OVERLAKE TRANSIT CENTER.\",\"Address\":\"156TH AVE NE & NE 40TH ST\",\"City\":\"REDMOND\",\"Latitude\":47.64456,\"LatitudeWaypoint\":null,\"Longitude\":-122.133755,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98052\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":19,\"Name\":\"PHINNEY AVE N & N 55TH ST\",\"Address\":\"Phinney Ave N & N 55th St\",\"City\":\"SEATTLE\",\"Latitude\":47.668456,\"LatitudeWaypoint\":null,\"Longitude\":-122.354315,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98103\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":16,\"Name\":\"PHINNEY AVE N & N ARGLE PL\",\"Address\":\"5555 Phinney Ave N\",\"City\":\"SEATTLE\",\"Latitude\":47.669303,\"LatitudeWaypoint\":null,\"Longitude\":-122.354543,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98103\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":75,\"Name\":\"QUEEN ANNE AVE & W. COMSTOCK (AM)\",\"Address\":\"QUEEN ANNE AVE & W. COMSTOCK\",\"City\":\"SEATTLE\",\"Latitude\":47.630425,\"LatitudeWaypoint\":null,\"Longitude\":-122.356571,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98109\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":76,\"Name\":\"QUEEN ANNE AVE & W. COMSTOCK (PM)\",\"Address\":\"QUEEN ANNE AVE & W. COMSTOCK\",\"City\":\"SEATTLE\",\"Latitude\":47.630177,\"LatitudeWaypoint\":null,\"Longitude\":-122.356654,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98109\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":78,\"Name\":\"QUEEN ANNE AVE N  & W HARRISON ST (SHARED)\",\"Address\":\"Queen Anne Ave N & W Harrison St\",\"City\":\"SEATTLE\",\"Latitude\":47.621877,\"LatitudeWaypoint\":null,\"Longitude\":-122.356768,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98109\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":2,\"Name\":\"QUEEN ANNE AVE N & CROCKETT ST\",\"Address\":\"2100 QUEEN ANNE AVE N\",\"City\":\"SEATTLE\",\"Latitude\":47.637203,\"LatitudeWaypoint\":null,\"Longitude\":-122.356954,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98109\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":40,\"Name\":\"REDMOND RIDGE CARPOOL LOT\",\"Address\":\"NE Cedar Park Cres & Redmond Ridge Trail\",\"City\":\"REDMOND\",\"Latitude\":47.685765,\"LatitudeWaypoint\":null,\"Longitude\":-122.033426,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98053\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":47,\"Name\":\"S GENESEE ST AND 43RD AVE S (GENESEE PARK)\",\"Address\":\"4316 S Genesee St\",\"City\":\"SEATTLE\",\"Latitude\":47.563815,\"LatitudeWaypoint\":null,\"Longitude\":-122.279484,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98118\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":48,\"Name\":\"S MCCLELLAN ST AND S MT BAKER BLVD (AM)\",\"Address\":\"3600 S McClellan St\",\"City\":\"SEATTLE\",\"Latitude\":47.578438,\"LatitudeWaypoint\":null,\"Longitude\":-122.288985,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98144\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":46,\"Name\":\"S ORCAS ST AND 52ND AVE S (SDA CHURCH)\",\"Address\":\"5200 S Orcas St\",\"City\":\"SEATTLE\",\"Latitude\":47.551227,\"LatitudeWaypoint\":null,\"Longitude\":-122.266897,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98118\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":34,\"Name\":\"SNOHOMISH HOPE FOURSQUARE CHURCH\",\"Address\":\"5002 Bickford Ave\",\"City\":\"SNOHOMISH\",\"Latitude\":47.950616,\"LatitudeWaypoint\":null,\"Longitude\":-122.118294,\"LongitudeWaypoint\":null,\"StopRadius\":650,\"Description\":null,\"ZipCode\":\"98290\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":27,\"Name\":\"STONE WAY N & N 45TH ST\",\"Address\":\"4468 Stone Way N\",\"City\":\"SEATTLE\",\"Latitude\":47.661178,\"LatitudeWaypoint\":null,\"Longitude\":-122.342023,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98103\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":8,\"Name\":\"TIMES SQUARE COMPLEX\",\"Address\":\"500 SW 39th St\",\"City\":\"RENTON\",\"Latitude\":47.447296,\"LatitudeWaypoint\":null,\"Longitude\":-122.224284,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98055\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":7,\"Name\":\"TRAILSIDE BUILDING\",\"Address\":\"35161 SE Douglas St\",\"City\":\"SNOQUALMIE\",\"Latitude\":47.52546393,\"LatitudeWaypoint\":null,\"Longitude\":-121.87141939,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98065\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":31,\"Name\":\"WALLINGFORD AVE N & 34TH ST (PM)\",\"Address\":\"Wallingford Ave N & N 34th St\",\"City\":\"SEATTLE\",\"Latitude\":47.64824,\"LatitudeWaypoint\":null,\"Longitude\":-122.336428,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98103\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":26,\"Name\":\"WALLINGFORD AVE N & N 34TH ST (AM)\",\"Address\":\"3400 Wallingford Ave N\",\"City\":\"SEATTLE\",\"Latitude\":47.648235,\"LatitudeWaypoint\":null,\"Longitude\":-122.33628,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98103\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":30,\"Name\":\"WALLINGFORD AVE N & N 43RD ST\",\"Address\":\"Wallingford Ave N & N 43rd St\",\"City\":\"SEATTLE\",\"Latitude\":47.658931,\"LatitudeWaypoint\":null,\"Longitude\":-122.336256,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98103\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]},{\"ID\":63,\"Name\":\"WEST SEATTLE CHRISTIAN CHURCH\",\"Address\":\"4415 41st Ave SW\",\"City\":\"SEATTLE\",\"Latitude\":47.56435,\"LatitudeWaypoint\":null,\"Longitude\":-122.384444,\"LongitudeWaypoint\":null,\"StopRadius\":500,\"Description\":null,\"ZipCode\":\"98116\",\"IsPick\":false,\"IsDrop\":false,\"IsHub\":false,\"DistanceFromStop\":0,\"SecondsFromStop\":0,\"RoutesServiced\":[]}]";
            //List<Connector> parsed = JsonConvert.DeserializeObject<List<Connector>>(JsonData);
            //foreach (Connector connector in parsed)
            //{
            //    UpdateLocation(
            //        "connectors",
            //        Convert.ToString(connector.Latitude),
            //        Convert.ToString(connector.Longitude),
            //        connector.Address,
            //        0,
            //        connector.Name);
            //}
            //return parsed[0].Name;
        }

        public class Apartment
        {
            public string name;
            public string area;
            public string url;
            public string where;
            public string price;
            public int bedrooms;
            public string geotag;
        }

        public class Connector
        {
            public int ID;
            public string Name;
            public string Address;
            public string City;
            public double Latitude;
            public double Longitude;
            public int StopRadius;
            public string Description;
            public string ZipCode;
            public bool IsPick;
            public bool IsDrop;
            public bool IsHub;
        }

        //
        // POST: /Home/UpdateLocation
        [AcceptVerbs(HttpVerbs.Post)]
        public string UpdateLocation(string type, 
                                string lattitude, 
                                string longitude, 
                                string address, 
                                string name, 
                                int price, 
                                double distanceToClosestConnector,
                                double distanceToClosestGym)
        {
            if (type == null || lattitude == null || longitude == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return "type, lattitude and longitude cannot be null";
            }
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(STORAGE_CONFIGURATION_KEY);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(TABLE_NAME);

            // Create a location entity.
            LocationEntity loc;
            if (type == "apartment")
            {
                loc = new ApartmentEntity(address, lattitude, longitude, name, price, distanceToClosestConnector, distanceToClosestGym);
            } else
            {
                loc = new LocationEntity(type, address, lattitude, longitude, name);
            }
            table.Execute(TableOperation.InsertOrReplace(loc));
            return null;
        }

        private List<ApartmentEntity> RequestResource(
            string tableName,
            string partition)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(STORAGE_CONFIGURATION_KEY);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);
            TableQuery<ApartmentEntity> query = new TableQuery<ApartmentEntity>().Where(
                TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partition));


            List<ApartmentEntity> locations = new List<ApartmentEntity>();
            foreach (ApartmentEntity entity in table.ExecuteQuery(query))
            {
                locations.Add(entity);
            }

            return locations;
        }

        public class LocationEntity : TableEntity
        {
            public LocationEntity(string type, string address, string latitude, string longitude, string name)
            {
                this.PartitionKey = type;
                this.RowKey = latitude + longitude;
                this.Address = address;
                this.Longitude = longitude;
                this.Latitude = latitude;
                this.Name = name;
            }

            public LocationEntity() { }

            public string Address { get; set; }

            public string Longitude { get; set; }

            public string Latitude { get; set; }

            public string Name { get; set; }
        }

        public class ApartmentEntity : LocationEntity
        {
            public ApartmentEntity() { }

            public ApartmentEntity(string address, 
                            string longitude, 
                            string latitude, 
                            string name,
                            int price, 
                            double distanceToClosestConnector, 
                            double distanceToClosestGym) 
                : base("apartment", address, longitude, latitude, name)
            {
                this.Price = price;
                this.DistanceToClosestConnector = distanceToClosestConnector;
                this.DistanceToClosestGym = distanceToClosestGym;
            }

            public int Price { get; set; }
            public double DistanceToClosestConnector { get; set; }
            public double DistanceToClosestGym { get; set; }
        }

    }
}
