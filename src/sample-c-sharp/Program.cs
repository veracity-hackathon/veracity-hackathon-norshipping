using System.Net.Http.Json;
using System.Text.Json.Serialization;

var client = new HttpClient();
var baseUrl = "https://api.veracity.com/df/ec-api-hackaton/emissions-calculation";

/*
You can choose to either ask for subscription in command line or read it from a .txt file
*/
Console.WriteLine("Please enter subscription key: ");
var subscriptionKey = Console.ReadLine();
//var subscriptionKey = ReadSubscriptionKey();

client.BaseAddress = new Uri(baseUrl);
client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

// ======= Valid request =======
/*
Response status: 200 - OK
*/
var validQueryParameters = BuildGetVesselsParameters(loadCondition: "ballast", imo: 9936044, durationInHours: 24, distanceInNauticalMiles: 240);
var validResponse = await SendHttpGetRequest(validQueryParameters);
Console.WriteLine(validResponse);

// ======= Invalid request, incorrect IMO number =======
/*
Response status: 404 - Not Found
*/
var invalidImoQueryParameters = BuildGetVesselsParameters(loadCondition: "ballast", imo: 1234567, durationInHours: 24, distanceInNauticalMiles: 240);
var invalidImoResponse = await SendHttpGetRequest(invalidImoQueryParameters);
Console.WriteLine(invalidImoResponse);


// ======= Invalid request, missing IMO number =======
/*
IMO number is required. Missing IMO number raises exception.
Response status: 404 - Resource Not Found
*/
var missingImoQueryParameters = BuildGetVesselsParameters(loadCondition: "ballast", durationInHours: 24, distanceInNauticalMiles: 240);
var missingImoResponse = await SendHttpGetRequest(invalidImoQueryParameters);
Console.WriteLine(missingImoResponse);

/*
Save subscription key in a .txt file to avoid passing subscription key in command line every run.
Format:
key=<subscription_key>
Replace <subscription_key> with key provided.
E.g
key=asnk603nlk5f7
*/
string ReadSubscriptionKey()
{
    // You may have to change the path for it to work, depending on how you run your code.
    var keyFilePath = "../subscription_key.txt";
    var key = File.ReadAllText(keyFilePath).Trim();

    if (string.IsNullOrWhiteSpace(key))
    {
        throw new Exception("The subscription key file {keyFilePath} cannot be empty.");
    }

    return key;
}

string BuildGetVesselsParameters(
    string? loadCondition = null, 
    int? imo = null, 
    int? durationInHours = null, 
    int? distanceInNauticalMiles = null, 
    int? averageSpeedInKnots = null, 
    string? cargoUnit = null,
    int? cargoAmount = null, 
    string? mainEngineFuelType = null, 
    string? auxilaryEngineFuelType = null, 
    string? boilerEngineFuelType = null)
    {
        var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);
        if (imo.HasValue)
        {
            queryString.Add("imo", $"{imo}");
        }
        if (!string.IsNullOrEmpty(loadCondition))
        {
            queryString.Add("load_cond", loadCondition);
        }
        if (durationInHours.HasValue)
        {
            queryString.Add("duration_h", $"{durationInHours}");
        }
        if (distanceInNauticalMiles.HasValue)
        {
            queryString.Add("distance_nm", $"{distanceInNauticalMiles}");
        }
        if (averageSpeedInKnots.HasValue)
        {
            queryString.Add("avg_speed_kn", $"{averageSpeedInKnots}");
        }
        if (cargoAmount.HasValue)
        {
            queryString.Add("cargo_amt", $"{cargoAmount}");
        }
        if (!string.IsNullOrEmpty(cargoUnit))
        {
            queryString.Add("cargo_unit", cargoUnit);
        }
        if (!string.IsNullOrEmpty(mainEngineFuelType))
        {
            queryString.Add("me_fuel_type", mainEngineFuelType);
        }
        if (!string.IsNullOrEmpty(auxilaryEngineFuelType))
        {
            queryString.Add("au_fuel_type", auxilaryEngineFuelType);
        }
        if (!string.IsNullOrEmpty(boilerEngineFuelType))
        {
            queryString.Add("boiler_fuel_type", boilerEngineFuelType);
        }
        return $"?{queryString}";
    }

async Task<VesselMetricsDto?> SendHttpGetRequest(string requestUri)
{
    try
    {
        var vesselMetrics = await client.GetFromJsonAsync<VesselMetricsDto>(requestUri);
        return vesselMetrics;
    }
    catch (Exception e)
    {
        Console.WriteLine("Unable to fetch vessel metrics:");
        Console.Write(e.ToString());
        return null;
    }
}

record VesselMetricsDto
{
    [JsonPropertyName("avg_speed_kn")]
    public float AverageSpeedInKn { get; init; }

    [JsonPropertyName("estimated_distance_nm")]
    public float EstimatedDistanceInNm { get; init; }

    [JsonPropertyName("duration_h")]
    public float DurationInHours { get; init; }

    [JsonPropertyName("me_fuel_cons_metric_metric_tons")]
    public float MainEngineFuelConsumptionInTons { get; init; }

    [JsonPropertyName("ae_fuel_cons_metric_metric_tons")]
    public float AuxilaryEnginefuelFuelConsumptionInTons { get; init; }

    [JsonPropertyName("boiler_fuel_cons_metric_metric_tons")]
    public float BoilerEngineFuelConsumptionInTons { get; init; }

    [JsonPropertyName("total_co2_emission_metric_tons")]
    public float TotalCo2EmissionMetricInTons { get; init; }
}