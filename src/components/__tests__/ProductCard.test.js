import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import { createRouter, createWebHistory } from 'vue-router'
import ProductCard from '../ProductCard.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/products/:id',
      name: 'product-detail',
      component: { template: '<div>Product Detail</div>' }
    }
  ]
})

describe('ProductCard', () => {
  const mockProduct = {
    id: '1',
    name: 'Produto Teste',
    price: 99.99,
    category: 'Eletrônicos',
    image: 'https://example.com/image.jpg'
  }

  it('deve renderizar informações do produto corretamente', () => {
    const wrapper = mount(ProductCard, {
      props: {
        product: mockProduct
      },
      global: {
        plugins: [router]
      }
    })

    expect(wrapper.text()).toContain('Produto Teste')
    expect(wrapper.text()).toContain('R$99.99')
    expect(wrapper.text()).toContain('Eletrônicos')
  })

  it('deve exibir imagem do produto quando disponível', () => {
    const wrapper = mount(ProductCard, {
      props: {
        product: mockProduct
      },
      global: {
        plugins: [router]
      }
    })

    const img = wrapper.find('img')
    expect(img.exists()).toBe(true)
    expect(img.attributes('src')).toBe(mockProduct.image)
    expect(img.attributes('alt')).toBe(mockProduct.name)
  })

  it('deve ter link para detalhes do produto', () => {
    const wrapper = mount(ProductCard, {
      props: {
        product: mockProduct
      },
      global: {
        plugins: [router]
      }
    })

    // Tenta encontrar o router-link de diferentes formas
    // O Vue Test Utils pode criar um stub 'router-link-stub' ou 'RouterLink'
    let link = wrapper.find('router-link-stub')
    
    if (!link.exists()) {
      link = wrapper.findComponent({ name: 'RouterLink' })
    }
    
    if (!link.exists()) {
      // Tenta encontrar qualquer elemento que possa ser o link
      link = wrapper.find('[to]')
    }

    expect(link.exists()).toBe(true)
    
    // Verifica o atributo 'to' - pode ser string ou objeto
    const toAttr = link.attributes('to')
    if (toAttr) {
      // Se for string, verifica se contém o ID
      if (typeof toAttr === 'string') {
        expect(toAttr).toContain('1')
      }
    } else {
      // Se não tiver atributo 'to', verifica a prop
      const toProp = link.props('to')
      if (toProp && typeof toProp === 'object' && toProp.params) {
        expect(toProp.params.id).toBe('1')
      } else if (toProp && typeof toProp === 'object' && toProp.name) {
        expect(toProp.name).toBe('product-detail')
      }
    }
  })
})

