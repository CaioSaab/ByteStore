# Configuração da API

Este projeto foi desmocado e está pronto para receber dados do backend.

## Configuração

1. Crie um arquivo `.env` na raiz do projeto baseado no `.env.example`:
   ```
   VITE_API_BASE_URL=http://localhost:3000/api
   ```

2. Configure a URL base da sua API backend no arquivo `.env`

## Estrutura de Serviços

O projeto utiliza uma arquitetura de serviços separados por funcionalidade:

- **`src/services/api.js`** - Configuração do axios com interceptors para tokens
- **`src/services/authService.js`** - Autenticação (login, register, logout)
- **`src/services/productService.js`** - Gerenciamento de produtos
- **`src/services/cartService.js`** - Gerenciamento do carrinho
- **`src/services/shippingService.js`** - Cálculo de frete
- **`src/services/couponService.js`** - Validação de cupons
- **`src/services/checkoutService.js`** - Finalização de pedidos

## Tokens

Todos os tokens são armazenados como strings no localStorage:
- `authToken` - Token do usuário cliente
- `vendorToken` - Token do vendedor

Os tokens são automaticamente incluídos no header `Authorization` de todas as requisições:
```
Authorization: Bearer <token>
```

## Endpoints Esperados

### Autenticação
- `POST /auth/login` - Login do cliente
- `POST /auth/register` - Registro do cliente
- `POST /auth/vendor/login` - Login do vendedor
- `POST /auth/vendor/register` - Registro do vendedor
- `GET /auth/me` - Dados do usuário atual
- `GET /auth/vendor/me` - Dados do vendedor atual

### Produtos
- `GET /products` - Listar produtos
- `GET /products/:id` - Detalhes do produto
- `GET /products/search` - Buscar produtos
- `GET /products/featured` - Listar produtos em destaque
- `POST /products` - Criar produto (vendedor)
- `POST /products/:id/questions` - Adicionar pergunta ao produto
- `GET /products/:id/questions` - Listar perguntas do produto

### Produtos - Vendedor
- `GET /products/vendor/my-products` - Listar produtos do vendedor autenticado (requer autenticação de vendedor)
- `PATCH /products/:id/status` - Ativar/desativar produto (requer autenticação de vendedor)
  - Body: `{ "active": true/false }`

### Carrinho
- `GET /cart` - Obter carrinho
- `POST /cart` - Adicionar item ao carrinho
- `PUT /cart/:id` - Atualizar item do carrinho
- `DELETE /cart/:id` - Remover item do carrinho
- `DELETE /cart` - Limpar carrinho

### Checkout
- `POST /shipping/calculate` - Calcular frete
- `POST /coupons/validate` - Validar cupom
- `POST /orders` - Criar pedido

## Clean Code

O código foi refatorado seguindo princípios de clean code:
- Funções pequenas e bem definidas
- Separação de responsabilidades
- Reutilização de código
- Tratamento de erros consistente

