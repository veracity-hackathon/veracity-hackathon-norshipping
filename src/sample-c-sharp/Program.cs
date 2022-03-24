var client = new HttpClient();
var baseUrl = "https://api.veracity.com/df/ec-api-hackaton/emissions-calculation";

var jsonOptions = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };

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
await PrintHttpResponse(validResponse);

// ======= Invalid request, incorrect IMO number =======
/*
Response status: 404 - Not Found
*/
//var invalidImoQueryParameters = BuildGetVesselsParameters("ballast", "1234567", 24, 240);
var invalidImoQueryParameters = BuildGetVesselsParameters(loadCondition: "ballast", imo: 1234567, durationInHours: 24, distanceInNauticalMiles: 240);
var invalidImoResponse = await SendHttpGetRequest(invalidImoQueryParameters);
await PrintHttpResponse(invalidImoResponse);


// ======= Invalid request, missing IMO number =======
/*
IMO number is required. Missing IMO number raises exception.
Response status: 404 - Resource Not Found
*/
//var invalidQueryParameters = BuildGetVesselsParameters("ballast", "", 24, 240);
var missingImoQueryParameters = BuildGetVesselsParameters(loadCondition: "ballast", durationInHours: 24, distanceInNauticalMiles: 240);
var missingImoResponse = await SendHttpGetRequest(missingImoQueryParameters);
await PrintHttpResponse(missingImoResponse);


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
    var keyFilePath = "src/subscription_key.txt";
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

async Task<HttpResponseMessage> SendHttpGetRequest(string requestUri)
{
    var response = await client.GetAsync(requestUri);
    return response;
}

async Task PrintHttpResponse(HttpResponseMessage response)
{
    if (response.StatusCode == System.Net.HttpStatusCode.OK)
    {
        var content = await response.Content.ReadAsStringAsync();
        var contentJson = System.Text.Json.Nodes.JsonNode.Parse(content);
        Console.WriteLine(contentJson.ToJsonString(jsonOptions));
    }
    else
    {
        Console.WriteLine("The server did not respond with an HTTP OK response.");
        Console.WriteLine($"Response status code: {((int)response.StatusCode)} - {response.ReasonPhrase}");
    }
}