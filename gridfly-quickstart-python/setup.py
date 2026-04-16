import subprocess
import sys
import os

def run_command(command, shell=True):
    print(f"Running: {command}")
    process = subprocess.run(command, shell=shell, check=True, capture_output=True, text=True)
    if process.stdout:
        print(process.stdout)
    if process.stderr:
        print(process.stderr)

def main():
    print("=== Gridfly Quickstart Setup ===")
    
    # 1. Install dependencies
    print("\n1. Installing dependencies...")
    run_command([sys.executable, "-m", "pip", "install", "-r", "requirements.txt"], shell=False)
    run_command([sys.executable, "-m", "pip", "install", "openapi-python-client", "httpx", "attrs", "python-dateutil"], shell=False)

    # 2. Generate API Client
    print("\n2. Generating API Client...")
    spec_url = "https://dev.api.gridfly.io/v1/apidocs/json"
    output_path = "gridfly_api_client"
    
    # We use --meta none to get just the package without extra project files
    # The command might fail if the tool is not in PATH, so we try running it via python -m if possible, 
    # but openapi-python-client is usually a script.
    try:
        run_command(f"openapi-python-client generate --url {spec_url} --output-path {output_path} --overwrite --meta none")
    except Exception as e:
        print(f"Error during generation: {e}")
        print("Attempting to run via 'python -m openapi_python_client'...")
        run_command(f"{sys.executable} -m openapi_python_client generate --url {spec_url} --output-path {output_path} --overwrite --meta none")

    print("\n=== Setup Complete ===")
    print("You can now run the examples using: python main.py")

if __name__ == "__main__":
    main()
