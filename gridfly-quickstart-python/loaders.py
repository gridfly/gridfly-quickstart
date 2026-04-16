import json
import os

def load_configuration(path="../config.json"):
    with open(path, "r") as f:
        return json.load(f)

def load_data(path):
    with open(path, "r") as f:
        return json.load(f)
