import { test, expect } from '@playwright/test';

test.describe('Produtos', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/login');
  });

  test('deve exibir lista de produtos na página inicial', async ({ page }) => {
     // -- LOGIN --
    await expect(page).toHaveURL(/.*login/);
    await page.fill('input[type="email"], input[name*="email"]', 'cliente@cliente.com');
    await page.fill('input[type="password"], input[name*="password"]', '123456');
    await page.click('button[type="submit"], button:has-text("Entrar"), button:has-text("Login")');
    await page.waitForTimeout(4000);
    await expect(page).toHaveURL(/.*home|.*\/$/);
    // ---

    // Verificar se há produtos na página
    const productCards = page.locator('[data-testid="product-card"], .product-card, article, .bg-\\[\\#1a1a2e\\]');
    await expect(productCards.first()).toBeVisible({ timeout: 10000 });
  });

  test('deve permitir visualizar detalhes de um produto', async ({ page }) => {
    // -- LOGIN --
    await expect(page).toHaveURL(/.*login/);
    await page.fill('input[type="email"], input[name*="email"]', 'cliente@cliente.com');
    await page.fill('input[type="password"], input[name*="password"]', '123456');
    await page.click('button[type="submit"], button:has-text("Entrar"), button:has-text("Login")');
    await page.waitForTimeout(4000);
    await expect(page).toHaveURL(/.*home|.*\/$/);
    // ---

    // Clicar no primeiro produto
    const firstProduct = page.locator('[data-testid="productCard"], a[href*="/product/"], router-link-stub').first();
    await firstProduct.click();

    // Verificar se é os detalhes
    await expect(page).toHaveURL(/.*\/product\/.*/, { timeout: 5000 });
  });

  test('deve permitir buscar produtos', async ({ page }) => {
    // -- LOGIN --
    await expect(page).toHaveURL(/.*login/);
    await page.fill('input[type="email"], input[name*="email"]', 'cliente@cliente.com');
    await page.fill('input[type="password"], input[name*="password"]', '123456');
    await page.click('button[type="submit"], button:has-text("Entrar"), button:has-text("Login")');
    await page.waitForTimeout(2000);
    await expect(page).toHaveURL(/.*home|.*\/$/);
    // ---

    // Procurar campo de busca
    const searchInput = page.locator('input[type="text"], input[placeholder*="Procurar..." i], input[name*="search"]');
    
    if (await searchInput.count() > 0) {
      await searchInput.fill('teste');
      await searchInput.press('Enter');
      
      await page.waitForTimeout(2000);
    }
  });

  test('deve permitir filtrar produtos por categoria', async ({ page }) => {
    // -- LOGIN
    await expect(page).toHaveURL(/.*login/);
    await page.fill('input[type="email"], input[name*="email"]', 'cliente@cliente.com');
    await page.fill('input[type="password"], input[name*="password"]', '123456');
    await page.click('button[type="submit"], button:has-text("Entrar"), button:has-text("Login")');
    await page.waitForTimeout(2000);
    await expect(page).toHaveURL(/.*home|.*\/$/);
    // ---

    // Procurar filtros de categoria
    const categoryFilter = page.locator('button:has-text("Categoria"), select, [role="button"]');
    
    if (await categoryFilter.count() > 0) {
      await categoryFilter.first().click();
      await page.waitForTimeout(1000);
    }
  });
});

