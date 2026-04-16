import re

class ReportHelper:
    def __init__(self, report_data):
        self.months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
        self.report_data = report_data
        self.periods = list(self.months)

        self.periods.insert(3, "Q1")
        self.periods.insert(7, "Q2")
        self.periods.insert(11, "Q3")
        self.periods.insert(15, "Q4")
        self.periods.append("YTD")

        self.categories = {
            "Food": ["Sandwich", "Bakery", "Salad", "Confectionary", "Fruit"],
            "Drink": ["Tea", "Coffee", "Water", "Soft", "Beer"]
        }

    def get_data(self):
        return self.report_data

    @property
    def data(self):
        return self.report_data

    def get_months(self):
        return self.months

    def get_periods(self):
        return self.periods

    def is_month(self, period):
        return period in self.months

    def is_quarter(self, period):
        return period.startswith("Q")

    def get_categories(self):
        return self.categories

    def get_safe_name(self, *values):
        return "_".join(str(v).replace(" ", "_") for v in values if v)
