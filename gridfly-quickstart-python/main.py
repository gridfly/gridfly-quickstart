import os
import gzip
import tempfile
from loaders import load_configuration, load_data
from html_generator import HtmlGenerator
from client import GridflyClient

def gzip_compress(data):
    fd, path = tempfile.mkstemp(prefix="gridfly-quickstart", suffix=".gz")
    with os.fdopen(fd, 'wb') as f:
        with gzip.GzipFile(fileobj=f, mode='wb') as gz:
            gz.write(data.encode('utf-8'))
    return path

def main():
    examples = ["example-1", "example-2", "example-3"]
    
    configuration = load_configuration("../config.json")
    client_id = configuration.get("client_id")
    client_secret = configuration.get("client_secret")
    mode = configuration.get("mode")
    output_directory = configuration.get("output_directory")

    html_generator = HtmlGenerator()
    client = GridflyClient()
    client.authenticate(client_id, client_secret)

    for example in examples:
        print(f"Beginning Excel generation for {example}")
        data_path = f"../data/{example}.json"
        data = load_data(data_path)
        
        # Wrap data in a simple object-like structure to support dot notation if necessary,
        # but Jinja handles dicts fine with both dot and bracket notation.
        # We'll pass the data dict directly as it was loaded.
        from types import SimpleNamespace
        def dict_to_obj(d):
            if isinstance(d, list):
                return [dict_to_obj(x) for x in d]
            if isinstance(d, dict):
                return SimpleNamespace(**{k: dict_to_obj(v) for k, v in d.items()})
            return d
        
        data_obj = dict_to_obj(data)
        
        template_name = f"{example}.html.j2"
        try:
            html = html_generator.generate_html(template_name, data_obj)
        except Exception as e:
            print(f"Error generating HTML for {example}: {e}")
            continue
        
        gzipped_path = gzip_compress(html)
        
        try:
            if mode == "async":
                client.generate_excel_async(gzipped_path)
            else:
                if not os.path.exists(output_directory):
                    os.makedirs(output_directory)
                client.generate_excel_sync(gzipped_path, output_directory)
        finally:
            if os.path.exists(gzipped_path):
                os.remove(gzipped_path)

if __name__ == "__main__":
    main()
