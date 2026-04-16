const fs = require('fs');
const path = require('path');

function walk(dir, callback) {
    fs.readdirSync(dir).forEach( f => {
        let dirPath = path.join(dir, f);
        let isDirectory = fs.statSync(dirPath).isDirectory();
        isDirectory ? walk(dirPath, callback) : callback(path.join(dir, f));
    });
};

const clientDir = path.join(__dirname, 'gridfly_api_client');
const srcDir = path.join(clientDir, 'src');

// 1. Fix imports in src/
if (fs.existsSync(srcDir)) {
    walk(srcDir, (filePath) => {
        if (filePath.endsWith('.js')) {
            let content = fs.readFileSync(filePath, 'utf8');
            const newContent = content.replace(/(from|import|export) (['"])(\.\.?\/[^'"]+)(?<!\.js)\2/g, '$1 $2$3.js$2');
            if (content !== newContent) {
                fs.writeFileSync(filePath, newContent, 'utf8');
            }
        }
    });
}

// 2. Set "type": "module" in gridfly_api_client/package.json to avoid Node.js warnings
const packageJsonPath = path.join(clientDir, 'package.json');
if (fs.existsSync(packageJsonPath)) {
    let pkg = JSON.parse(fs.readFileSync(packageJsonPath, 'utf8'));
    pkg.type = "module";
    fs.writeFileSync(packageJsonPath, JSON.stringify(pkg, null, 2), 'utf8');
}