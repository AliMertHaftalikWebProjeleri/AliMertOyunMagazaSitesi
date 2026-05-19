const { chromium } = require('playwright');
const path = require('path');
const fs = require('fs');

async function takeScreenshots() {
    const browser = await chromium.launch();
    const page = await browser.newPage();
    
    const screenshotDir = path.join(__dirname, 'wwwroot', 'img', 'screenshots');
    if (!fs.existsSync(screenshotDir)) {
        fs.mkdirSync(screenshotDir, { recursive: true });
    }

    // Login first to access all pages
    console.log('Logging in as admin...');
    await page.goto('http://localhost:5000/Identity/Account/Login');
    await page.fill('input[name="Input.Email"]', 'admin@gmail.com');
    await page.fill('input[name="Input.Password"]', 'Admin123!');
    await page.click('button[type="submit"]');
    await page.waitForNavigation();

    // Home Page
    console.log('Taking home screenshot...');
    await page.goto('http://localhost:5000');
    await page.waitForTimeout(2000);
    await page.screenshot({ path: path.join(screenshotDir, 'home.png'), fullPage: false });

    // Catalog Page
    console.log('Taking catalog screenshot...');
    await page.goto('http://localhost:5000/Catalog');
    await page.waitForTimeout(2000);
    await page.screenshot({ path: path.join(screenshotDir, 'catalog.png'), fullPage: false });

    // Admin Page
    console.log('Taking admin screenshot...');
    await page.goto('http://localhost:5000/Admin');
    await page.waitForTimeout(2000);
    await page.screenshot({ path: path.join(screenshotDir, 'admin.png'), fullPage: false });

    // Wishlist Page
    console.log('Taking wishlist screenshot...');
    await page.goto('http://localhost:5000/Wishlist');
    await page.waitForTimeout(2000);
    await page.screenshot({ path: path.join(screenshotDir, 'wishlist.png'), fullPage: false });

    await browser.close();
    console.log('All screenshots taken successfully!');
}

takeScreenshots().catch(err => {
    console.error('Error taking screenshots:', err);
    process.exit(1);
});
