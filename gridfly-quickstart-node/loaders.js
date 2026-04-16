import fs from 'fs-extra';
import path from 'path';

export async function loadConfiguration(configPath) {
    if (!await fs.pathExists(configPath)) {
        throw new Error(`Configuration file not found at ${configPath}`);
    }
    return fs.readJson(configPath);
}

export async function loadData(dataPath) {
    if (!await fs.pathExists(dataPath)) {
        throw new Error(`Data file not found at ${dataPath}`);
    }
    return fs.readJson(dataPath);
}
