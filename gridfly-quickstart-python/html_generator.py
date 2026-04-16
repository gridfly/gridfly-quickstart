from jinja2 import Environment, FileSystemLoader, select_autoescape
from report_helper import ReportHelper
import os

class HtmlGenerator:
    def __init__(self, template_dir="templates"):
        self.env = Environment(
            loader=FileSystemLoader(template_dir),
            autoescape=select_autoescape(['html', 'xml'])
        )

    def generate_html(self, template_name, data):
        template = self.env.get_template(template_name)
        report_helper = ReportHelper(data)
        return template.render(report=report_helper)
