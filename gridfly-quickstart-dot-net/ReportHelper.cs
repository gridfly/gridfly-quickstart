using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Gridfly.QuickStart
{
    public class ReportHelper
    {
        private static readonly List<string> MonthsList = new List<string>
        {
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };

        private readonly Dictionary<string, List<string>> _categories = new Dictionary<string, List<string>>();
        private readonly List<string> _periods;
        private readonly object _reportData;

        public ReportHelper(object reportData)
        {
            _reportData = reportData;
            _periods = new List<string>(MonthsList);
            _periods.Insert(3, "Q1");
            _periods.Insert(7, "Q2");
            _periods.Insert(11, "Q3");
            _periods.Insert(15, "Q4");
            _periods.Add("YTD");

            _categories.Add("Food", new List<string> { "Sandwich", "Bakery", "Salad", "Confectionary", "Fruit" });
            _categories.Add("Drink", new List<string> { "Tea", "Coffee", "Water", "Soft", "Beer" });
        }

        public object Data => _reportData;
        public List<string> Months => MonthsList;
        public List<string> Periods => _periods;

        public bool IsMonth(string period) => MonthsList.Contains(period);
        public bool IsQuarter(string period) => period.StartsWith("Q");
        public Dictionary<string, List<string>> Categories => _categories;

        public object GetData() => _reportData;
        public List<string> GetPeriods() => _periods;
        public List<string> GetMonths() => MonthsList;
        public Dictionary<string, List<string>> GetCategories() => _categories;

        public string GetSafeName(params string[] values)
        {
            return string.Join("_", values.Select(s => s.Replace(" ", "_")));
        }
    }
}
