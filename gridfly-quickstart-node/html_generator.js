import nunjucks from 'nunjucks';
import path from 'path';

export default class HtmlGenerator {
    constructor(templatesPath = 'templates') {
        this.env = nunjucks.configure(templatesPath, {
            autoescape: true,
            noCache: true
        });
    }

    generateHtml(templateName, data) {
        return this.env.render(templateName, data);
    }
}
