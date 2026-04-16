export default class ReportHelper {
    constructor(reportData) {
        this.months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
        this.report_data = reportData;
        this.periods = [...this.months];

        this.periods.splice(3, 0, "Q1");
        this.periods.splice(7, 0, "Q2");
        this.periods.splice(11, 0, "Q3");
        this.periods.splice(15, 0, "Q4");
        this.periods.push("YTD");

        this.categories = {
            "Food": ["Sandwich", "Bakery", "Salad", "Confectionary", "Fruit"],
            "Drink": ["Tea", "Coffee", "Water", "Soft", "Beer"]
        };
    }

    get_data() {
        return this.report_data;
    }

    get data() {
        return this.report_data;
    }

    get_months() {
        return this.months;
    }

    get_periods() {
        return this.periods;
    }

    is_month(period) {
        return this.months.includes(period);
    }

    is_quarter(period) {
        return period.startsWith("Q");
    }

    get_categories() {
        return this.categories;
    }

    get_safe_name(...values) {
        return values
            .filter(v => v)
            .map(v => String(v).replace(/ /g, "_"))
            .join("_");
    }
}
