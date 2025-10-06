export const products = [
    // Eletrônicos
    {
        id: 1,
        name: 'Smartphone Pro X',
        category: 'Eletrônicos',
        price: 3299.90,
        image: 'https://images.unsplash.com/photo-1523206489230-c012c64b2b48?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['#2c3e50', '#ecf0f1', '#3498db'], sizes: ['128GB', '256GB'] }
    },
    {
        id: 2,
        name: 'Headset com Cancelamento de Ruído',
        category: 'Eletrônicos',
        price: 899.00,
        image: 'https://images.unsplash.com/photo-1505740420928-5e560c06d30e?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['#ffffff', '#000000', '#7f8c8d'], sizes: ['Único'] }
    },
    {
        id: 3,
        name: 'Notebook Ultrafino i7',
        category: 'Eletrônicos',
        price: 6800.50,
        image: 'https://images.unsplash.com/photo-1496181133206-80ce9b88a853?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['#bdc3c7', '#2c3e50'], sizes: ['8GB RAM', '16GB RAM'] }
    },
    {
        id: 4,
        name: 'Smartwatch Geração 5',
        category: 'Eletrônicos',
        price: 1850.00,
        image: 'https://images.unsplash.com/photo-1546868871-7041f2a55e12?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['#000000', '#f1c40f', '#ecf0f1'], sizes: ['40mm', '44mm'] }
    },
    {
        id: 5,
        name: 'Tablet 10 Polegadas',
        category: 'Eletrônicos',
        price: 2100.00,
        image: 'https://images.unsplash.com/photo-1561154464-82e9adf32764?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['#000000', '#ffffff'], sizes: ['64GB', '128GB'] }
    },
    {
        id: 6,
        name: 'Caixa de Som Bluetooth',
        category: 'Eletrônicos',
        price: 450.00,
        image: 'https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSqzII4oSzzaF_Fb_asTGslpP-iHUW2ZW44zA&s    ',
        variations: { colors: ['#000000', '#e74c3c', '#3498db'], sizes: ['Único'] }
    },

    // Roupas
    {
        id: 7,
        name: 'Camiseta Básica Premium',
        category: 'Roupas',
        price: 99.90,
        image: 'https://images.unsplash.com/photo-1521572163474-6864f9cf17ab?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['#ffffff', '#000000', '#2c3e50'], sizes: ['P', 'M', 'G', 'GG'] }
    },
    {
        id: 8,
        name: 'Calça Jeans Reta',
        category: 'Roupas',
        price: 220.00,
        image: 'https://img.freepik.com/fotos-gratis/calcas-de-ganga_1203-8094.jpg',
        variations: { colors: ['#34495e', '#2980b9'], sizes: ['38', '40', '42', '44'] }
    },
    {
        id: 9,
        name: 'Jaqueta de Couro Sintético',
        category: 'Roupas',
        price: 480.00,
        image: 'https://img.freepik.com/fotos-gratis/jovem-mulher-vestindo-uma-jaqueta-encostada-em-uma-parede-branca_181624-40808.jpg?ga=GA1.1.2086848425.1749424456&semt=ais_hybrid&w=740&q=80',
        variations: { colors: ['#000000'], sizes: ['P', 'M', 'G'] }
    },
    {
        id: 10,
        name: 'Tênis de Corrida Performance',
        category: 'Roupas',
        price: 550.00,
        image: 'https://images.unsplash.com/photo-1542291026-7eec264c27ff?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['#ecf0f1', '#2c3e50', '#e74c3c'], sizes: ['39', '40', '41', '42'] }
    },
    {
        id: 11,
        name: 'Moletom com Capuz',
        category: 'Roupas',
        price: 280.00,
        image: 'https://img.freepik.com/fotos-gratis/homem-com-capuz-amarelo-e-roupas-masculinas-streetwear_53876-105536.jpg?ga=GA1.1.2086848425.1749424456&semt=ais_hybrid&w=740&q=80',
        variations: { colors: ['#7f8c8d', '#000000', '#c0392b'], sizes: ['P', 'M', 'G', 'GG'] }
    },
    {
        id: 12,
        name: 'Vestido Floral de Verão',
        category: 'Roupas',
        price: 190.00,
        image: 'https://img.freepik.com/fotos-gratis/mulher-vestindo-vestido-de-verao_23-2150388811.jpg?ga=GA1.1.2086848425.1749424456&semt=ais_hybrid&w=740&q=80',
        variations: { colors: ['#f1c40f', '#3498db'], sizes: ['P', 'M', 'G'] }
    },

    // Casa
    {
        id: 13,
        name: 'Cafeteira de Cápsulas',
        category: 'Casa',
        price: 399.90,
        image: 'https://images.unsplash.com/photo-1563227812-0ea4c22e6cc8?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['#c0392b', '#000000', '#ffffff'], sizes: ['Único'] }
    },
    {
        id: 14,
        name: 'Conjunto de Facas do Chef',
        category: 'Casa',
        price: 350.00,
        image: 'https://img.freepik.com/fotos-premium/conjunto-de-talheres_170153-1474.jpg?ga=GA1.1.2086848425.1749424456&semt=ais_hybrid&w=740&q=80',
        variations: { colors: ['#bdc3c7'], sizes: ['8 peças'] }
    },
    {
        id: 15,
        name: 'AirFryer Elétrica 5L',
        category: 'Casa',
        price: 580.00,
        image: 'https://img.freepik.com/fotos-gratis/pao-fresco-cafe-quente-refeicao-caseira-rustica-gerada-por-ia_188544-26893.jpg?ga=GA1.1.2086848425.1749424456&semt=ais_hybrid&w=740&q=80',
        variations: { colors: ['#000000', '#ffffff'], sizes: ['5L'] }
    },
    {
        id: 16,
        name: 'Aspirador de Pó Robô Inteligente',
        category: 'Casa',
        price: 1999.00,
        image: 'https://img.freepik.com/fotos-gratis/homem-usando-um-aspirador-de-robo-na-sala-de-estar_23-2149036870.jpg?ga=GA1.1.2086848425.1749424456&semt=ais_hybrid&w=740&q=80',
        variations: { colors: ['#000000'], sizes: ['Único'] }
    },
    {
        id: 17,
        name: 'Liquidificador de Alta Potência',
        category: 'Casa',
        price: 250.00,
        image: 'https://img.freepik.com/fotos-premium/um-misturador-preto-e-prateado-com-um-molho-de-tomate-vermelho-dentro_1064589-18622.jpg?ga=GA1.1.2086848425.1749424456&semt=ais_hybrid&w=740&q=80',
        variations: { colors: ['#e74c3c', '#ffffff'], sizes: ['1.5L'] }
    },

    // Livros
    {
        id: 18,
        name: 'Duna - Frank Herbert',
        category: 'Livros',
        price: 89.90,
        image: 'https://images.unsplash.com/photo-1544947950-fa07a98d237f?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['N/A'], sizes: ['Capa Dura'] }
    },
    {
        id: 19,
        name: 'Box O Senhor dos Anéis',
        category: 'Livros',
        price: 250.00,
        image: 'https://images.unsplash.com/photo-1524995997946-a1c2e315a42f?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['N/A'], sizes: ['3 Volumes'] }
    },

    // Esportes e Lazer
    {
        id: 20,
        name: 'Tapete de Yoga',
        category: 'Esportes e Lazer',
        price: 120.00,
        image: 'https://images.unsplash.com/photo-1544367567-0f2fcb009e0b?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['#9b59b6', '#3498db', '#1abc9c'], sizes: ['Único'] }
    },
    {
        id: 21,
        name: 'Bicicleta Aro 29',
        category: 'Esportes e Lazer',
        price: 2300.00,
        image: 'https://images.unsplash.com/photo-1485965120184-e220f721d03e?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['#000000', '#e74c3c'], sizes: ['M', 'L'] }
    },
    {
        id: 22,
        name: 'Kit Halteres Reguláveis',
        category: 'Esportes e Lazer',
        price: 600.00,
        image: 'https://images.unsplash.com/photo-1584735935682-2f2b69dff9d2?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['#2c3e50'], sizes: ['24kg'] }
    },
    {
        id: 23,
        name: 'Barraca de Camping',
        category: 'Esportes e Lazer',
        price: 450.00,
        image: 'https://img.freepik.com/fotos-premium/tenda-na-floresta_1048944-26092540.jpg?ga=GA1.1.2086848425.1749424456&semt=ais_hybrid&w=740&q=80',
        variations: { colors: ['#27ae60', '#f39c12'], sizes: ['4 Pessoas', '6 Pessoas'] }
    },
    {
        id: 24,
        name: 'Bola de Futebol Oficial',
        category: 'Esportes e Lazer',
        price: 150.00,
        image: 'https://img.freepik.com/fotos-gratis/vista-frontal-de-futebol-e-forma-hexagonal_23-2148796889.jpg?ga=GA1.1.2086848425.1749424456&semt=ais_hybrid&w=740&q=80',
        variations: { colors: ['#ffffff'], sizes: ['Único'] }
    },

    // Beleza e Cuidados
    {
        id: 25,
        name: 'Kit Skincare Facial',
        category: 'Beleza e Cuidados',
        price: 180.00,
        image: 'https://images.unsplash.com/photo-1598440947619-2c35fc9aa908?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['N/A'], sizes: ['3 passos'] }
    },
    {
        id: 26,
        name: 'Perfume Amadeirado 100ml',
        category: 'Beleza e Cuidados',
        price: 420.00,
        image: 'https://images.unsplash.com/photo-1541643600914-78b084683601?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['N/A'], sizes: ['100ml'] }
    },
    {
        id: 27,
        name: 'Paleta de Sombras',
        category: 'Beleza e Cuidados',
        price: 95.00,
        image: 'https://img.freepik.com/fotos-premium/vista-de-angulo-alto-da-paleta-de-sombra-de-olhos-na-mesa_1048944-5915037.jpg?ga=GA1.1.2086848425.1749424456&semt=ais_hybrid&w=740&q=80',
        variations: { colors: ['#c0392b', '#8e44ad', '#f1c40f'], sizes: ['12 cores'] }
    },
    {
        id: 28,
        name: 'Shampoo Hidratante',
        category: 'Beleza e Cuidados',
        price: 55.00,
        image: 'https://img.freepik.com/psd-premium/maquete-de-embalagem-de-shampoo-para-cabelos-cacheados_23-2151102044.jpg?ga=GA1.1.2086848425.1749424456&semt=ais_hybrid&w=740&q=80',
        variations: { colors: ['N/A'], sizes: ['500ml'] }
    },
    {
        id: 29,
        name: 'Máquina de Cortar Cabelo',
        category: 'Beleza e Cuidados',
        price: 230.00,
        image: 'https://images.unsplash.com/photo-1621607512214-68297480165e?auto=format&fit=crop&w=400&q=80',
        variations: { colors: ['#bdc3c7', '#000000'], sizes: ['10 pentes'] }
    },
    {
        id: 30,
        name: 'Escova de Dentes Elétrica',
        category: 'Eletrônicos',
        price: 290.00,
        image: 'https://img.freepik.com/fotos-premium/close-up-de-mao-humana-segurando-oculos_1048944-30907224.jpg?ga=GA1.1.2086848425.1749424456&semt=ais_hybrid&w=740&q=80',
        variations: { colors: ['#ffffff', '#000000'], sizes: ['Único'] }
    },
];