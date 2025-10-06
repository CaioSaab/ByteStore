document.addEventListener('DOMContentLoaded', () => {
    // Banco de dados de produtos em memória
    const products = [
        // As URLs das imagens foram trocadas para um serviço mais estável (picsum.photos)
        { id: 1, name: 'Kiroshi Optics V.2', category: 'cyberwear', price: 750.00, image: 'https://picsum.photos/seed/1/400/400', description: 'Advanced optical visor with environmental scanner and direct neural interface. Enhance your perception and stay ahead.' },
        { id: 2, name: 'Ares Prosthetic Arm', category: 'augmentation', price: 4500.00, image: 'https://picsum.photos/seed/2/400/400', description: 'Military-grade cybernetic arm implant. Offers superhuman strength, precision, and optional weapon integration.' },
        { id: 3, name: 'Specter Surveillance Drone', category: 'drone', price: 1200.00, image: 'https://picsum.photos/seed/3/400/400', description: 'Stealth drone for reconnaissance and security. Equipped with thermal camera, long-range microphones, and silent propulsion.' },
        { id: 4, name: 'Thermoregulating Jacket', category: 'cyberwear', price: 980.00, image: 'https://picsum.photos/seed/4/400/400', description: 'Stylish jacket that adapts to any climate, maintaining optimal body temperature. Integrated communication system.' },
        { id: 5, name: 'MnemOS Memory Implant', category: 'augmentation', price: 8300.00, image: 'https://picsum.photos/seed/5/400/400', description: 'Expand your memory capacity with this neural co-processor. Never forget a detail again, perfect for data brokers.' },
        { id: 6, name: 'Nekomata Robotic Cat', category: 'drone', price: 2100.00, image: 'https://picsum.photos/seed/6/400/400', description: 'Agile and intelligent robotic companion, perfect for infiltration missions or as a high-tech pet. Customizable personality.' },
        { id: 7, name: 'AI Contact Lenses', category: 'cyberwear', price: 550.00, image: 'https://picsum.photos/seed/7/400/400', description: 'Lenses that project an augmented reality interface directly onto your retina. Real-time data overlay and communication.' },
        { id: 8, name: 'Adrenaline Neural System', category: 'augmentation', price: 6200.00, image: 'https://picsum.photos/seed/8/400/400', description: 'Enhance your reflexes and speed with this system that optimizes neural responses. Gain the edge in combat or competition.' },
    ];

    // Lógica do carrinho
    let cart = JSON.parse(localStorage.getItem('cart')) || [];

    const updateCartCount = () => {
        const cartCount = document.getElementById('cart-count');
        if (cartCount) {
            cartCount.textContent = cart.reduce((sum, item) => sum + item.quantity, 0);
        }
    };

    const saveCart = () => {
        localStorage.setItem('cart', JSON.stringify(cart));
        updateCartCount();
    };
    
    const addToCart = (productId) => {
        const product = products.find(p => p.id === productId);
        if (product) {
            const cartItem = cart.find(item => item.id === productId);
            if (cartItem) {
                cartItem.quantity++;
            } else {
                cart.push({ ...product, quantity: 1 });
            }
            saveCart();
            alert(`${product.name} has been added to your cart!`);
        }
    };
    
    // Função para renderizar produtos na página principal
    const displayProducts = (filteredProducts) => {
        const productGrid = document.getElementById('product-grid');
        if (!productGrid) return;
        
        productGrid.innerHTML = filteredProducts.map(product => `
            <div class="bg-gray-900/70 border border-gray-800 rounded-xl overflow-hidden shadow-lg transform transition-all duration-300 product-card-hover relative group">
                <a href="product.html?id=${product.id}" class="block">
                    <img src="${product.image}" alt="${product.name}" class="w-full h-52 object-cover transition-transform duration-300 group-hover:scale-105">
                    <div class="p-5">
                        <h3 class="text-xl font-semibold font-montserrat text-blue-400 mb-2 truncate">${product.name}</h3>
                        <p class="text-gray-400 text-sm mb-4 capitalize">${product.category}</p>
                        <span class="text-3xl font-montserrat font-bold text-white">$${product.price.toFixed(2)}</span>
                    </div>
                </a>
                <button data-product-id="${product.id}" class="add-to-cart-btn absolute bottom-5 right-5 w-12 h-12 rounded-full gradient-btn text-white text-xl flex items-center justify-center shadow-lg opacity-0 group-hover:opacity-100 transform translate-y-2 group-hover:translate-y-0 transition-all duration-300">
                    <i class="fas fa-plus"></i>
                </button>
            </div>
        `).join('');
    };
    
    // Função para renderizar detalhes na página de produto
    const displayProductDetails = () => {
        const productDetailContent = document.getElementById('product-detail-content');
        if (!productDetailContent) return;

        const urlParams = new URLSearchParams(window.location.search);
        const productId = parseInt(urlParams.get('id'));
        const product = products.find(p => p.id === productId);

        if (product) {
            // Usamos uma resolução maior para a página de detalhes
            const imageUrl = product.image.replace('/400/400', '/800/800');
            productDetailContent.innerHTML = `
                <div class="grid grid-cols-1 md:grid-cols-2 gap-10 items-center bg-gray-900/70 p-8 md:p-12 rounded-xl border border-gray-800 shadow-2xl">
                    <div class="flex justify-center items-center p-4 bg-gray-800 rounded-lg">
                        <img src="${imageUrl}" alt="${product.name}" class="w-full max-w-md h-auto rounded-lg object-cover shadow-xl">
                    </div>
                    <div>
                        <h1 class="text-5xl font-montserrat font-bold text-gradient mb-5">${product.name}</h1>
                        <p class="text-gray-300 text-lg leading-relaxed mb-8">${product.description}</p>
                        <p class="text-5xl font-montserrat font-bold text-white mb-10">$${product.price.toFixed(2)}</p>
                        <button data-product-id="${product.id}" class="add-to-cart-btn-detail w-full max-w-xs gradient-btn text-white font-bold py-4 px-8 rounded-full text-xl shadow-lg hover:shadow-2xl focus:outline-none focus:ring-4 focus:ring-blue-500/50">
                            <i class="fas fa-shopping-cart mr-3"></i> Add to Cart
                        </button>
                    </div>
                </div>
            `;
        } else {
            productDetailContent.innerHTML = `<p class="text-center text-3xl text-red-500 font-montserrat">Product not found.</p>`;
        }
    };
    
    // Roteamento simples baseado na página atual
    if (document.getElementById('product-grid')) {
        // --- LÓGICA DA PÁGINA PRINCIPAL ---
        const searchInput = document.getElementById('searchInput');
        const filterBtns = document.querySelectorAll('.filter-btn');

        displayProducts(products); // Exibe todos os produtos inicialmente

        searchInput.addEventListener('input', (e) => {
            const searchTerm = e.target.value.toLowerCase();
            const filtered = products.filter(p => p.name.toLowerCase().includes(searchTerm));
            displayProducts(filtered);
        });

        filterBtns.forEach(btn => {
            btn.addEventListener('click', () => {
                filterBtns.forEach(b => {
                    b.classList.remove('active', 'bg-blue-900/50', 'ring-2', 'ring-blue-500');
                    b.classList.add('bg-gray-800');
                });
                btn.classList.add('active', 'bg-blue-900/50', 'ring-2', 'ring-blue-500');
                btn.classList.remove('bg-gray-800'); // Remove default background if active

                const category = btn.dataset.category;
                const filtered = category === 'all' 
                    ? products 
                    : products.filter(p => p.category === category);
                displayProducts(filtered);
            });
        });
        // Set initial active state for "All Products" filter button
        document.querySelector('.filter-btn[data-category="all"]').classList.add('active', 'bg-blue-900/50', 'ring-2', 'ring-blue-500');
    }

    if (document.getElementById('product-detail-content')) {
        // --- LÓGICA DA PÁGINA DE DETALHES ---
        displayProductDetails();
    }

    // Event delegation para botões "Adicionar ao Carrinho"
    document.body.addEventListener('click', (e) => {
        const button = e.target.closest('.add-to-cart-btn, .add-to-cart-btn-detail');
        if (button) {
            const productId = parseInt(button.dataset.productId);
            addToCart(productId);
        }
    });

    // Atualiza a contagem do carrinho em ambas as páginas ao carregar
    updateCartCount();
});