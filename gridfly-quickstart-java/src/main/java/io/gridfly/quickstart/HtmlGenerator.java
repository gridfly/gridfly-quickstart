package io.gridfly.quickstart;

import java.io.StringWriter;
import java.io.Writer;
import java.util.Map;
import java.util.Properties;
import org.apache.velocity.Template;
import org.apache.velocity.VelocityContext;
import org.apache.velocity.app.VelocityEngine;

public class HtmlGenerator {

  private final VelocityEngine velocityEngine;

  public HtmlGenerator() {
    Properties props = new Properties();
    props.put("resource.loaders", "class");
    props.put("resource.loader.class.cache", false);
    props.put(
        "resource.loader.class.class",
        "org.apache.velocity.runtime.resource.loader.ClasspathResourceLoader");
    velocityEngine = new VelocityEngine(props);
    velocityEngine.init();
  }

  public String generateHTML(String templateName, Map<String, Object> data) {
    Template template = velocityEngine.getTemplate(templateName);
    VelocityContext context = new VelocityContext();

    context.put("report", new ReportHelper(data));
    Writer writer = new StringWriter();
    template.merge(context, writer);

    return writer.toString();
  }
}
