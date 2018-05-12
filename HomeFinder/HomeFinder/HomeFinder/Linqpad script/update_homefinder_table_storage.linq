<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.Build.Framework.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.Build.Tasks.v4.0.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Microsoft.Build.Utilities.v4.0.dll</Reference>
  <Reference>&lt;ProgramFilesX64&gt;\Microsoft SDKs\Azure\.NET SDK\v2.9\bin\plugins\Diagnostics\Microsoft.WindowsAzure.Storage.dll</Reference>
  <Reference>&lt;ProgramFilesX64&gt;\Microsoft SDKs\Azure\.NET SDK\v2.9\bin\plugins\Diagnostics\Newtonsoft.Json.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.ComponentModel.DataAnnotations.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Design.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.DirectoryServices.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.DirectoryServices.Protocols.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.EnterpriseServices.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.Caching.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Security.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.ServiceProcess.dll</Reference>
  <Reference>&lt;ProgramFilesX86&gt;\Microsoft WCF Data Services\5.6.4\bin\.NETFramework\System.Spatial.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.ApplicationServices.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.RegularExpressions.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Web.Services.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
  <Namespace>Microsoft.WindowsAzure.Storage</Namespace>
  <Namespace>Microsoft.WindowsAzure.Storage.Table</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Web</Namespace>
</Query>

//string filePath = @"c:\homefinderjsons\apartmentjson_updatedWithDistances.txt";
//string partition = "apartment";

//string filePath = @"c:\homefinderjsons\connectorsjson.txt";
//string partition = "connector";

string filePath = @"c:\homefinderjsons\gymjson.txt";
string partition = "gym";

/////////

private static readonly string TABLE_NAME = "LocationData";
private static readonly string STORAGE_CONFIGURATION_KEY =
	"DefaultEndpointsProtocol=https;AccountName=homefinder;AccountKey=+Ekr3DjSPY90U5AzEBYF3kNuLJiBqUyfI3Z4ET1u+CBSAkR0RuJNy0dKer93CHF7Ur9leIBL1boerG0NhOjUGA==;EndpointSuffix=core.windows.net";

void Main()
{
	List<ApartmentEntity> apartments = JsonConvert.DeserializeObject<List<ApartmentEntity>>(File.ReadAllText(filePath));
	foreach (var a in apartments)
	{
		UpdateLocation(partition, 
				Convert.ToString(a.Latitude), 
				Convert.ToString(a.Longitude), 
				a.Address, 
				a.Name, 
				0, 
				a.DistanceToClosestConnector, 
				a.DistanceToClosestGym);
	}

}

// Define other methods and classes here
public string UpdateLocation(string type,
						string lattitude,
						string longitude,
						string address,
						string name,
						int price = 0,
						double distanceToClosestConnector = 0.0,
						double distanceToClosestGym = 0.0)
{
	if (type == null || lattitude == null || longitude == null)
	{
		HttpContext hc = new HttpContext(null, null);
		hc.Response.StatusCode = (int)HttpStatusCode.BadRequest;
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
	}
	else
	{
		loc = new LocationEntity(type, address, lattitude, longitude, name);
	}
	table.Execute(TableOperation.InsertOrReplace(loc));
	return null;
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
		this.Type = type;
	}

	public LocationEntity() { }

	public string Address { get; set; }

	public string Longitude { get; set; }

	public string Latitude { get; set; }

	public string Name { get; set; }
	public string Type { get; set; }
}

public class ApartmentEntity : LocationEntity
{
	public ApartmentEntity(string address, string longitude, string latitude, string name,
		int price, double distanceToClosestConnector, double distanceToClosestGym)
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
