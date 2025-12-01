import { describe, it, expect, vi, beforeEach } from 'vitest'
import { login, register, vendorLogin, vendorRegister } from '../authService'

// Mock do módulo api
vi.mock('../api', () => ({
  api: {
    post: vi.fn(),
    get: vi.fn()
  },
  setAuthToken: vi.fn(),
  setVendorToken: vi.fn()
}))

describe('authService', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('login', () => {
    it('deve fazer login com sucesso e retornar token', async () => {
      const { api, setAuthToken } = await import('../api')
      
      api.post.mockResolvedValue({
        data: 'token_jwt_aqui'
      })

      const result = await login('test@example.com', 'senha123')

      expect(result.success).toBe(true)
      expect(result.token).toBe('token_jwt_aqui')
      expect(setAuthToken).toHaveBeenCalledWith('token_jwt_aqui')
      expect(api.post).toHaveBeenCalledWith('/Auth/LoginComprador', {
        email: 'test@example.com',
        password: 'senha123'
      })
    })

    it('deve retornar erro quando login falha', async () => {
      const { api } = await import('../api')
      
      api.post.mockRejectedValue({
        response: {
          data: {
            message: 'Credenciais inválidas'
          }
        }
      })

      const result = await login('test@example.com', 'senhaerrada')

      expect(result.success).toBe(false)
      expect(result.error).toBe('Credenciais inválidas')
    })
  })

  describe('register', () => {
    it('deve registrar usuário com sucesso', async () => {
      const { api } = await import('../api')
      
      api.post.mockResolvedValue({
        data: { id: '123', nome: 'Test User', email: 'test@example.com' }
      })

      const userData = {
        name: 'Test User',
        email: 'test@example.com',
        password: 'senha123'
      }

      const result = await register(userData)

      expect(result.success).toBe(true)
      expect(api.post).toHaveBeenCalledWith('/Auth/RegistrarComprador', {
        Nome: 'Test User',
        Email: 'test@example.com',
        Password: 'senha123'
      })
    })

    it('deve retornar erro quando registro falha', async () => {
      const { api } = await import('../api')
      
      api.post.mockRejectedValue({
        response: {
          data: {
            message: 'Email já está em uso'
          }
        }
      })

      const result = await register({
        name: 'Test User',
        email: 'test@example.com',
        password: 'senha123'
      })

      expect(result.success).toBe(false)
      expect(result.error).toBe('Email já está em uso')
    })
  })

  describe('vendorLogin', () => {
    it('deve fazer login como vendedor com sucesso', async () => {
      const { api, setVendorToken } = await import('../api')
      
      api.post.mockResolvedValue({
        data: 'vendor_token_aqui'
      })

      const result = await vendorLogin('vendor@example.com', 'senha123')

      expect(result.success).toBe(true)
      expect(result.token).toBe('vendor_token_aqui')
      expect(setVendorToken).toHaveBeenCalledWith('vendor_token_aqui')
    })
  })

  describe('vendorRegister', () => {
    it('deve registrar vendedor com sucesso', async () => {
      const { api } = await import('../api')
      
      api.post.mockResolvedValue({
        data: { id: '456', nome: 'Vendor Store', email: 'vendor@example.com' }
      })

      const vendorData = {
        storeName: 'Vendor Store',
        email: 'vendor@example.com',
        password: 'senha123'
      }

      const result = await vendorRegister(vendorData)

      expect(result.success).toBe(true)
      expect(api.post).toHaveBeenCalledWith('/Auth/RegistrarVendedor', {
        Nome: 'Vendor Store',
        Email: 'vendor@example.com',
        Password: 'senha123'
      })
    })
  })
})

