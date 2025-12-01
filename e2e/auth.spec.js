import { test, expect } from '@playwright/test';

test.describe('Autenticação', () => {
  test.beforeEach(async ({ page }) => {
    await page.goto('/');
  });

  test('deve permitir registro de novo usuário', async ({ page }) => {
    await page.click('text=Crie uma agora');
    await expect(page).toHaveURL(/.*register/);

    // Preencher formulário de registro
    await page.fill('input[type="text"], input[name*="name"], input[placeholder*="nome" i]', 'Teste Usuario');
    await page.fill('input[type="email"], input[name*="email"]', `teste_${Date.now()}@example.com`);
    await page.fill('input[type="password"], input[name*="password"]', 'senha123456');
    await page.fill('#confirmPassword', 'senha123456');

    await page.click('button[type="submit"], button:has-text("Registrar"), button:has-text("Cadastrar")');

    await expect(page.locator('text=Cadastro realizado com sucesso! Você já pode fazer login.')).toBeVisible({ timeout: 5000 });
  });

  test('deve fazer login com credenciais válidas', async ({ page }) => {
    await expect(page).toHaveURL(/.*login/);

    await page.fill('input[type="email"], input[name*="email"]', 'cliente@cliente.com');
    await page.fill('input[type="password"], input[name*="password"]', '123456');

    await page.click('button[type="submit"], button:has-text("Entrar"), button:has-text("Login")');

    await expect(page).toHaveURL(/.*home|.*\/$/);
  });

  test('deve exibir erro com credenciais inválidas', async ({ page }) => {
    await page.goto('/login');

    await page.fill('input[type="email"], input[name*="email"]', 'inexistente@example.com');
    await page.fill('input[type="password"], input[name*="password"]', 'senhaerrada');
    await page.click('button[type="submit"], button:has-text("Entrar")');

    await expect(page.locator('text=E-mail ou senha inválidos.')).toBeVisible({ timeout: 5000 });
  });
});

