import requests
import json
'''
Save subscription key in a .txt file to avoid passing subscription key in command line every run.
Format:
<subscription_key>

Replace <subscription_key> with key provided.
E.g
asnk603nlk5f7
'''
def get_subscription_key():
    key_filepath = "src\sample-postman\subscription_key.txt"
    with open(key_filepath, "r") as f:
        subscription_key = f.read().strip()
    if not subscription_key:
        raise Exception(f"The subscription key file {key_filepath} cannot be empty.")
    return subscription_key
    

def get_request(query, header):
    base_url = "https://api.veracity.com/df/ec-api-hackaton/emissions-calculation"
    try:
        return requests.get(url=base_url+query, headers=header)
    except requests.exceptions.RequestException as err:
        print(err)
        return

def print_response(response):
    if isinstance(response, requests.Response) and response.status_code == requests.codes.ok:
        try:
            data = response.json()
            print(json.dumps(data, indent=2))
        except json.decoder.JSONDecodeError as e_info:
            print(f"Failed to process response as JSON because of: {e_info}")
            print(response.text)
    else:
        print("The server did not respond with an HTTP OK response.")
        print(f"Response status: {response.status_code} - {response.reason}")

def build_get_vessel_parameters(**kwargs):
    '''
    Example
    call:       build_get_vessel_parameters(imo=1234, duration_h=24, load_cond="ballast")
    returns:    ?imo=1234&duration_h=24&load_cond=ballast
    '''
    query_string = "?"
    for key, value in kwargs.items():
        query_string += f"&{key}={value}"
    return query_string


'''
You can choose to either ask for subscription in command line or read it from a .txt file
'''
subscription_key = input("Please enter the subscription key: ")
#subscription_key = get_subscription_key()

header = {'Ocp-Apim-Subscription-Key': subscription_key}

#======= Valid request =======#
'''
Response status: 200 - OK
'''
valid_query = build_get_vessel_parameters(imo=9936044, duration_h=24, load_cond="ballast", distance_nm=240)
valid_response = get_request(valid_query, header)
print_response(valid_response)


#======= Invalid request, incorrect IMO query =======#
'''
Response status: 404 - Not Found
'''
incorrect_imo_query = build_get_vessel_parameters(imo=1234567, duration_h=24, load_cond="ballast", distance_nm=240)
incorrect_imo_response = get_request(incorrect_imo_query, header)
print_response(incorrect_imo_response)


#======= Invalid request, missing IMO query =======#
'''
IMO number is required. Missing IMO number raises exception.
Response status: 404 - Resource Not Found
'''
missing_imo_query = build_get_vessel_parameters(duration_h=24, load_cond="ballast", distance_nm=240)
missing_imo_response = get_request(missing_imo_query, header)
print_response(missing_imo_response)


#======= Consume data =======#
'''
The API response is JSON containing the data. This is an example on how to acces the data.
'''
data = valid_response.json()
average_speed_knots = data["avg_speed_kn"]
print(f"The average speed is {average_speed_knots} knots")