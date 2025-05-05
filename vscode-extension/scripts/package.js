const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');

// 插件版本
const packageJson = JSON.parse(fs.readFileSync(path.resolve(__dirname, '../package.json'), 'utf8'));
const version = packageJson.version;

console.log(`正在打包 KoalaWiki VSCode 插件 v${version}...`);

try {
  // 清理之前的构建
  console.log('清理之前的构建...');
  if (fs.existsSync(path.resolve(__dirname, '../out'))) {
    fs.rmSync(path.resolve(__dirname, '../out'), { recursive: true, force: true });
  }

  // 编译TypeScript
  console.log('编译TypeScript...');
  execSync('npm run compile', { stdio: 'inherit' });

  // 打包VSIX
  console.log('打包VSIX...');
  execSync('npm run package', { stdio: 'inherit' });

  console.log(`✅ 打包完成！`);
  console.log(`📦 插件包: koalawiki-vscode-${version}.vsix`);
} catch (error) {
  console.error('❌ 打包失败:', error);
  process.exit(1);
} 