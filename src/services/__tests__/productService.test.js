import { describe, it, expect, vi, beforeEach } from 'vitest'
import { getAllProducts, getProductById, searchProducts } from '../productService'

// Mock do módulo api
vi.mock('../api', () => ({
  api: {
    get: vi.fn(),
    post: vi.fn(),
    delete: vi.fn(),
    patch: vi.fn(),
    put: vi.fn()
  }
}))

describe('productService', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  describe('getAllProducts', () => {
    it('deve buscar todos os produtos com sucesso', async () => {
      const { api } = await import('../api')
      
      const mockProducts = {
        items: [
          { id: '1', nome: 'Produto 1', preco: 100 },
          { id: '2', nome: 'Produto 2', preco: 200 }
        ],
        totalCount: 2
      }

      api.get.mockResolvedValue({
        data: mockProducts
      })

      const result = await getAllProducts()

      expect(result.success).toBe(true)
      expect(result.data).toEqual(mockProducts)
      expect(api.get).toHaveBeenCalledWith('/Product', { params: {} })
    })

    it('deve retornar erro quando busca falha', async () => {
      const { api } = await import('../api')
      
      api.get.mockRejectedValue({
        response: {
          data: {
            message: 'Erro ao buscar produtos'
          }
        }
      })

      const result = await getAllProducts()

      expect(result.success).toBe(false)
      expect(result.error).toBe('Erro ao buscar produtos')
    })
  })

  describe('getProductById', () => {
    it('deve buscar produto por ID com sucesso', async () => {
      const { api } = await import('../api')
      
      const mockProduct = {
        id: '1',
        nome: 'Produto 1',
        preco: 100,
        descricao: 'Descrição do produto'
      }

      api.get.mockResolvedValue({
        data: mockProduct
      })

      const result = await getProductById('1')

      expect(result.success).toBe(true)
      expect(result.data).toEqual(mockProduct)
      expect(api.get).toHaveBeenCalledWith('/Product/1')
    })

    it('deve retornar erro quando produto não é encontrado', async () => {
      const { api } = await import('../api')
      
      api.get.mockRejectedValue({
        response: {
          status: 404,
          data: {
            message: 'Produto não encontrado'
          }
        }
      })

      const result = await getProductById('999')

      expect(result.success).toBe(false)
      expect(result.error).toBe('Produto não encontrado')
    })
  })

  describe('searchProducts', () => {
    it('deve buscar produtos com termo de busca', async () => {
      const { api } = await import('../api')
      
      const mockProducts = {
        items: [
          { id: '1', nome: 'Produto Teste', preco: 100 }
        ],
        totalCount: 1
      }

      api.get.mockResolvedValue({
        data: mockProducts
      })

      const result = await searchProducts('Teste')

      expect(result.success).toBe(true)
      expect(api.get).toHaveBeenCalledWith('/Product', {
        params: { search: 'Teste' }
      })
    })

    it('deve buscar produtos com categoria', async () => {
      const { api } = await import('../api')
      
      api.get.mockResolvedValue({
        data: { items: [], totalCount: 0 }
      })

      await searchProducts('', 'Eletrônicos')

      expect(api.get).toHaveBeenCalledWith('/Product', {
        params: { category: 'Eletrônicos' }
      })
    })
  })
})

