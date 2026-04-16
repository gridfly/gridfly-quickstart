using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using RazorLight;
using System;

namespace Gridfly.QuickStart
{
    public class HtmlGenerator
    {
        private readonly string _templateName;
        private readonly ReportHelper _helper;
        private readonly IRazorLightEngine _engine;

        public HtmlGenerator(string templateName, ReportHelper helper)
        {
            _templateName = templateName;
            _helper = helper;
            
            _engine = new RazorLightEngineBuilder()
                .UseFileSystemProject(Path.Combine(AppContext.BaseDirectory, "Resources"))
                .UseMemoryCachingProvider()
                .Build();
        }

        public async Task<string> GenerateHtml()
        {
            // RazorLight expects .cshtml extension usually, but we can specify the template key
            return await _engine.CompileRenderAsync($"{_templateName}.cshtml", _helper);
        }
    }
}
