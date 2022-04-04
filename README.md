
This repository contains code samples and documentation for **Emissions Prediction Calculator API (EPC)**.

The code samples are intended to allow a quick start and show how you can query API in **Python** and **C#**. Additionally sample **Postman** collection file was provided to allow querying the API via GUI.

You can also use `epc.openapi.json` file to get the full API specification. This file follows Open API standard and can be imported to tools such as Postman.

Repository content:

    .
    ├── doc
    │   ├── epc.openapi.json                        # Open API specification for the EPC API
    |   ├── NorShippin-Hackaton-Appendix-A.xlsx     # Extra information about specific ships
    ├── src                                         # Source code with samples
    │   ├── sample-c-sharp                          # Code samples for API query in C#
    │   ├── sample-postman                          # Sample project file that can be impoted into Postman
    │   ├── sample-python                           # Code samples for API query in Python
    │   ├── subscription_key.txt                    # Textfile to save subscription_key if desirable
    └── README.md                                   # This file

Code samples summary:
1.	Sample project in C#: Codesamples for accessing API and query data. If you choose to save subscription_key in the text file make sure the path in function `ReadSubscriptionKey` is correct, this will depend on how you run the code. To build queries, pass parameter-names and the value to `BuildGetVesselParameters`. List of parameter-names can be found in API documentation. IMO is the only required parameter. 

    There is a difference between input parameter names to function `BuildGetVesselParameters` and parameter names in the query for API. This is due to naming convention in C#. Have a look at `BuildGetVesselParameters` to see the mapping and valid parameter names. 
    
    The API response is stored in a record `VesselMetricsDto` to consume the data. This is an example, if you need to access other data then please add more fields to the record. You can access data like this:
    ```
    var response = await SendHttpGetRequest(query);
    var averageSpeedInKnots = response.AverageSpeedInKn
    ```

2.	Sample project in Python: Codesamples for accessing API and query data. If you choose to save subscription_key in the text file make sure the path in function `get_subscription_key` is correct, this will depend on how you run the code. To build queries, pass parameter-names and the value to `build_get_vessel_parameters`. List of parameter-names can be found in API documentation. IMO is the only required parameter. You need to install `requests` by enter following command to terminal: `pip3 install requests`
3.	Sample collection file in Postman:  
You can import the collection file into Postman `(File -> Import)`. Once imported a new collection **EPC API Hackaton** is created and can be used to query the API. You need to replace header parameter `{{subscribtion-key}}` with a subscription key you have received.  You can get Postman form here https://www.postman.com/

