# Instruções Rápidas para Executar Testes

## Backend (.NET)

```bash
# Navegar até o diretório de testes
cd ByteStoreAPI/ByteStoreAPI.Tests

# Restaurar pacotes NuGet
dotnet restore

# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test --collect:"XPlat Code Coverage"

# Executar apenas testes unitários
dotnet test --filter "FullyQualifiedName~Services"

# Executar apenas testes de integração
dotnet test --filter "FullyQualifiedName~Integration"
```

## Frontend (Vue.js)

```bash
# Instalar dependências (primeira vez)
npm install

# Executar testes unitários
npm test

# Executar testes em modo watch
npm test -- --watch

# Executar testes com interface gráfica
npm run test:ui

# Executar testes com cobertura
npm run test:coverage
```

## Testes E2E (Playwright)

```bash
# Instalar dependências do Playwright (primeira vez)
npx playwright install

# Executar testes E2E
npm run test:e2e

# Executar testes em modo UI (interativo)
npx playwright test --ui

# Executar testes em navegador específico
npx playwright test --project=chromium

# Executar testes em modo headed (ver o navegador)
npx playwright test --headed
```

## Estrutura de Testes

```
ByteStoreAPI/ByteStoreAPI.Tests/
├── Services/          # Testes unitários de serviços
├── Integration/       # Testes de integração

src/
├── services/__tests__/    # Testes de serviços Vue
├── components/__tests__/  # Testes de componentes Vue

e2e/                   # Testes end-to-end
├── auth.spec.js
└── products.spec.js
```

