package io.gridfly.quickstart;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

public class ReportHelper {
  private final List<String> months =
      List.of("Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec");

  private final Map<String, List<String>> categories = new LinkedHashMap<>();

  private final List<String> periods;
  private final Map<String, Object> reportData;

  public ReportHelper(Map<String, Object> reportData) {
    this.reportData = reportData;
    this.periods = new ArrayList<>(months);

    periods.add(3, "Q1");
    periods.add(7, "Q2");
    periods.add(11, "Q3");
    periods.add(15, "Q4");
    periods.add(16, "YTD");

    categories.put("Food", List.of("Sandwich", "Bakery", "Salad", "Confectionary", "Fruit"));
    categories.put("Drink", List.of("Tea", "Coffee", "Water", "Soft", "Beer"));
  }

  public Object getData() {
    return reportData;
  }

  public List<String> getMonths() {
    return months;
  }

  public List<String> getPeriods() {
    return periods;
  }

  public boolean isMonth(String period) {
    return months.contains(period);
  }

  public boolean isQuarter(String period) {
    return period.startsWith("Q");
  }

  public Map<String, List<String>> getCategories() {
    return categories;
  }

  public String getSafeName(String... values) {
    return Arrays.stream(values).map(s -> s.replaceAll(" ", "_")).collect(Collectors.joining("_"));
  }
}
