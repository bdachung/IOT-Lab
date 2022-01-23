import paho.mqtt.client as mqttclient
import time
import json
import math

from selenium import webdriver
from selenium.webdriver.firefox.options import Options
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC

print("Hello ThingBoard")

FOLDER = "C:/Users/asus/AppData/Roaming/Mozilla/Firefox/Profiles"
PROFILE_NAME = "pmr27fcv.default-1642918215471"
driver = None


def setup():
    global driver
    profile = FOLDER + "/" + PROFILE_NAME
    fp = webdriver.FirefoxProfile(profile)
    options = Options()
    options.headless = True
    driver = webdriver.Firefox(firefox_profile=fp, options=options)
    driver.get("https://www.maps.ie/coordinates.html")
    findbtn = WebDriverWait(driver, 10).until(EC.presence_of_element_located((By.ID, "find-loc")))
    findbtn.click()
    print("Set up : Done. Start sending data")


def get_latlng(lat_default, lng_default):
    global driver
    try:
        findbtn = WebDriverWait(driver, 10).until(EC.presence_of_element_located((By.ID, "find-loc")))
        findbtn.click()
        WebDriverWait(driver, 10).until(EC.presence_of_element_located((By.ID, "find-loc")))
    except:
        driver.get("https://www.maps.ie/coordinates.html")
        return lat_default, lng_default
    lat = driver.find_element(By.ID, "marker-lat").get_attribute("value")
    lng = driver.find_element(By.ID, "marker-lng").get_attribute("value")
    return lat, lng


BROKER_ADDRESS = "demo.thingsboard.io"
PORT = 1883
THINGS_BOARD_ACCESS_TOKEN = "iX5SMb4L6Zup3nmv8KnA"


def subscribed(client, userdata, mid, granted_qos):
    print("Subscribed...")


def recv_message(client, userdata, message):
    print("Received: ", message.payload.decode("utf-8"))
    temp_data = {'value': True}
    try:
        jsonobj = json.loads(message.payload)
        if jsonobj['method'] == "setValue":
            temp_data['value'] = jsonobj['params']
            client.publish('v1/devices/me/attributes', json.dumps(temp_data), 1)
    except:
        pass


def connected(client, usedata, flags, rc):
    if rc == 0:
        print("Thingsboard connected successfully!!")
        client.subscribe("v1/devices/me/rpc/request/+")
    else:
        print("Connection is failed")


client = mqttclient.Client("Gateway_Thingsboard")
client.username_pw_set(THINGS_BOARD_ACCESS_TOKEN)

client.on_connect = connected
client.connect(BROKER_ADDRESS, 1883)
client.loop_start()

client.on_subscribe = subscribed
client.on_message = recv_message

setup()
temp = 30
humidity = 50
light_intensity = 0
latitude, longitude = get_latlng(10.8231, 106.6297)
counter = 0


def func(x):
    return (x-1)*(x-1) + (x-3)*(x-3) - 25


while True:
    collect_data = {'temperature': temp, 'humidity': humidity, 'light': func(light_intensity),
                    'longitude': longitude, 'latitude': latitude}
    temp += 1
    humidity += 1
    light_intensity += 5
    latitude, longitude = get_latlng(latitude, longitude)
    client.publish('v1/devices/me/telemetry', json.dumps(collect_data), 1)
    time.sleep(5)