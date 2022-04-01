import requests

'''
Save subscription key in a .txt file to avoid passing subscription key in command line every run.
Format:
<subscription_key>
Replace <subscription_key> with key provided.
E.g
asnk603nlk5f7
'''


def read_subscription_key():
    key_filepath = "../subscription_key.txt"
    with open(key_filepath, "r") as f:
        subscription_key = f.read().strip()
    if not subscription_key:
        raise Exception(f"The subscription key file {key_filepath} cannot be empty.")
    return subscription_key


def send_http_get_request(query, header):
    base_url = "https://api.veracity.com/df/ec-api-hackaton/emissions-calculation"
    try:
        response = requests.get(url=base_url + query, headers=header)
        if response.status_code == requests.codes.ok:
            content = response.json()
            return content
        else:
            print("The server did not respond with an HTTP OK response.")
            print(f"Response status: {response.status_code} - {response.reason}")
    except Exception as err:
        print(f"Unable to fetch vessel metrics: {err}")


def build_get_vessel_parameters(**kwargs):
    """
    Example
    call:       build_get_vessel_parameters(imo=1234, duration_h=24, load_cond="ballast")
    returns:    ?imo=1234&duration_h=24&load_cond=ballast
    """
    query_string = "?"
    for key, value in kwargs.items():
        query_string += f"&{key}={value}"
    return query_string


'''
You can choose to either ask for subscription in command line or read it from a .txt file
'''
subscription_key = input("Please enter the subscription key: ")
# subscription_key = read_subscription_key()

header = {'Ocp-Apim-Subscription-Key': subscription_key}

# ======= Valid request =======#
'''
Response status: 200 - OK
'''
valid_query = build_get_vessel_parameters(imo=9936044, duration_h=24, load_cond="ballast", distance_nm=240)
valid_data = send_http_get_request(valid_query, header)
print(valid_data)

# Consume data
average_speed_knots = valid_data["avg_speed_kn"]
print(f"The average speed is {average_speed_knots} knots")

# ======= Invalid request, incorrect IMO query =======#
'''
Response status: 404 - Not Found
'''
incorrect_imo_query = build_get_vessel_parameters(imo=1234567, duration_h=24, load_cond="ballast", distance_nm=240)
incorrect_imo_data = send_http_get_request(incorrect_imo_query, header)
print(incorrect_imo_data)

# ======= Invalid request, missing IMO query =======#
'''
IMO number is required. Missing IMO number raises exception.
Response status: 404 - Resource Not Found
'''
missing_imo_query = build_get_vessel_parameters(duration_h=24, load_cond="ballast", distance_nm=240)
missing_imo_data = send_http_get_request(missing_imo_query, header)
print(missing_imo_data)
