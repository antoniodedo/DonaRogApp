const fs = require('fs');
const path = require('path');

function copyRecursive(src, dest) {
    if (!fs.existsSync(src)) return;
    const stat = fs.lstatSync(src);
    if (stat.isDirectory()) {
        if (!fs.existsSync(dest)) fs.mkdirSync(dest, { recursive: true });
        for (const entry of fs.readdirSync(src)) {
            if (entry === 'node_modules') continue;
            copyRecursive(path.join(src, entry), path.join(dest, entry));
        }
    } else {
        fs.copyFileSync(src, dest);
    }
}

const abpDir = path.join('node_modules', '@abp');
const libsDir = path.join('wwwroot', 'libs', '@abp');

if (!fs.existsSync(abpDir)) {
    console.log('No @abp packages found in node_modules, skipping.');
    process.exit(0);
}

for (const pkg of fs.readdirSync(abpDir)) {
    const src = path.join(abpDir, pkg);
    const dest = path.join(libsDir, pkg);
    console.log(`Installing @abp/${pkg}...`);
    copyRecursive(src, dest);
}

console.log('ABP libs installed successfully.');
