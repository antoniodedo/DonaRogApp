/**
 * Replicates 'abp install-libs' by reading abp.resourcemapping.js from each
 * @abp/* npm package and copying files to wwwroot/libs accordingly.
 */
const fs = require('fs');
const path = require('path');

const NODE_MODULES = path.resolve('node_modules');
const LIBS_DIR = path.resolve('wwwroot', 'libs');

function resolveAlias(p) {
    if (p.startsWith('@node_modules/')) return path.join(NODE_MODULES, p.slice('@node_modules/'.length));
    if (p.startsWith('@libs/')) return path.join(LIBS_DIR, p.slice('@libs/'.length));
    return p;
}

function ensureDir(dir) {
    if (!fs.existsSync(dir)) fs.mkdirSync(dir, { recursive: true });
}

function copyFileToDir(src, destDir) {
    if (!fs.existsSync(src)) { console.warn(`  SKIP (not found): ${src}`); return; }
    ensureDir(destDir);
    fs.copyFileSync(src, path.join(destDir, path.basename(src)));
}

function copyDirRecursive(src, dest) {
    if (!fs.existsSync(src)) { console.warn(`  SKIP (not found): ${src}`); return; }
    ensureDir(dest);
    for (const entry of fs.readdirSync(src)) {
        const s = path.join(src, entry), d = path.join(dest, entry);
        fs.lstatSync(s).isDirectory() ? copyDirRecursive(s, d) : fs.copyFileSync(s, d);
    }
}

function applyMapping(rawSrc, rawDest) {
    const hasStar = rawSrc.includes('*');
    if (!hasStar) {
        copyFileToDir(resolveAlias(rawSrc), resolveAlias(rawDest));
        return;
    }

    const lastSlash = rawSrc.lastIndexOf('/');
    const dir = resolveAlias(rawSrc.substring(0, lastSlash));
    const pattern = rawSrc.substring(lastSlash + 1); // e.g. "*.*", "*.js", "*"
    const dest = resolveAlias(rawDest);

    if (!fs.existsSync(dir)) { console.warn(`  SKIP dir (not found): ${dir}`); return; }

    const isRecursive = pattern === '*' || !pattern.includes('.');
    if (isRecursive) {
        copyDirRecursive(dir, dest);
        return;
    }

    // wildcard like "*.*" or "*.min.js" — match all files in directory
    const extFilter = pattern === '*.*' ? null : pattern.replace('*', '');
    for (const file of fs.readdirSync(dir)) {
        const filePath = path.join(dir, file);
        if (fs.lstatSync(filePath).isDirectory()) continue;
        if (extFilter === null || file.endsWith(extFilter)) {
            ensureDir(dest);
            fs.copyFileSync(filePath, path.join(dest, file));
        }
    }
}

function applyMappingFile(mappingFile) {
    // Use eval to avoid require() caching issues across different paths
    const content = fs.readFileSync(mappingFile, 'utf8');
    let mapping;
    try {
        mapping = eval(content.replace('module.exports =', '').trim().replace(/;$/, ''));
    } catch {
        return;
    }
    if (!mapping || !mapping.mappings) return;
    for (const [src, dest] of Object.entries(mapping.mappings)) {
        applyMapping(src, dest);
    }
}

// Process all @abp/* packages
const abpDir = path.join(NODE_MODULES, '@abp');
if (!fs.existsSync(abpDir)) {
    console.error('node_modules/@abp not found — run npm/yarn install first');
    process.exit(1);
}

let pkgCount = 0;
for (const pkg of fs.readdirSync(abpDir)) {
    const mappingFile = path.join(abpDir, pkg, 'abp.resourcemapping.js');
    if (!fs.existsSync(mappingFile)) continue;
    console.log(`  @abp/${pkg}`);
    applyMappingFile(mappingFile);
    pkgCount++;
}

// Also apply the project's own abp.resourcemapping.js
const projectMapping = path.resolve('abp.resourcemapping.js');
if (fs.existsSync(projectMapping)) {
    const content = fs.readFileSync(projectMapping, 'utf8');
    if (content.includes('mappings')) {
        console.log('  project abp.resourcemapping.js');
        applyMappingFile(projectMapping);
        pkgCount++;
    }
}

console.log(`\nDone — processed ${pkgCount} resource mapping files.`);
console.log(`Libs output: ${LIBS_DIR}`);
