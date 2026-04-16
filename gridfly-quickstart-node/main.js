import fs from 'fs-extra';
import path from 'path';
import zlib from 'zlib';
import tmp from 'tmp';
import { loadConfiguration, loadData } from './loaders';
import HtmlGenerator from './html_generator';
import GridflyClient from './client';
import ReportHelper from './report_helper';

function gzipCompress(data) {
    return new Promise((resolve, reject) => {
        tmp.file({ prefix: 'gridfly-quickstart-', postfix: '.gz' }, (err, tmpPath, fd, cleanupCallback) => {
            if (err) return reject(err);
            
            const buffer = Buffer.from(data, 'utf-8');
            zlib.gzip(buffer, (err, gzipped) => {
                if (err) return reject(err);
                
                fs.writeFile(tmpPath, gzipped, (err) => {
                    if (err) return reject(err);
                    resolve(tmpPath);
                });
            });
        });
    });
}

async function main() {
    const examples = ["example-1", "example-2", "example-3"];
    
    try {
        const configuration = await loadConfiguration("../config.json");
        const { client_id, client_secret, mode, output_directory } = configuration;

        const htmlGenerator = new HtmlGenerator();
        const client = new GridflyClient();
        await client.authenticate(client_id, client_secret);

        for (const example of examples) {
            console.log(`Beginning Excel generation for ${example}`);
            const dataPath = path.join("..", "data", `${example}.json`);
            const data = await loadData(dataPath);
            const reportHelper = new ReportHelper(data);
            
            const templateName = `${example}.html.njk`;
            let html;
            try {
                html = htmlGenerator.generateHtml(templateName, { report: reportHelper });
            } catch (e) {
                console.error(`Error generating HTML for ${example}: ${e.message}`);
                continue;
            }
            
            const gzippedPath = await gzipCompress(html);
            
            try {
                if (mode === "async") {
                    await client.generateExcelAsync(gzippedPath);
                } else {
                    await client.generateExcelSync(gzippedPath, output_directory);
                }
            } catch (e) {
                console.error(`Error processing ${example}: ${e.message}`);
            } finally {
                if (await fs.pathExists(gzippedPath)) {
                    await fs.remove(gzippedPath);
                }
            }
        }
    } catch (e) {
        console.error(`Main execution error: ${e.message}`);
    }
}

if (require.main === module || process.argv[1] && process.argv[1].endsWith('index.js')) {
    main();
}
