using System.Text.Json;
using ByteStoreAPI.DTOs;
using ByteStoreAPI.Entity;
using ByteStoreAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ByteStoreAPI.Service
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IVendorRepository _vendorRepository;
        private readonly IFileUploadService _fileUploadService;

        public ProductService(IProductRepository productRepository,IVendorRepository vendorRepository,IFileUploadService fileUploadService)
        {
            _productRepository = productRepository;
            _vendorRepository = vendorRepository;
            _fileUploadService = fileUploadService;
        }

        public async Task<ProductResponseDTO> CreateProductAsync(CreateProductDTO dto, Guid vendorId)
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<CreateProductDataDTO>(dto.Data, options);

            if (data == null)
                throw new ArgumentException("Dados JSON inválidos.");

            var vendor = await _vendorRepository.GetByIdAsync(vendorId)
                ?? throw new InvalidOperationException("Vendedor não encontrado.");

            var product = new Product(vendor, data.Nome, data.Descricao, data.Categoria);

            if (data.Variations == null || data.Variations.Count == 0)
                throw new ArgumentException("O produto deve ter pelo menos uma variação.");

            foreach (var variationDto in data.Variations)
            {
                var variation = new ProductVariation(
                    product,
                    variationDto.Preco,
                    variationDto.Estoque,
                    variationDto.Cor,
                    variationDto.Tamanho
                );

                if (dto.Files != null && variationDto.FileIndexes?.Count > 0)
                {
                    foreach (var fileIndex in variationDto.FileIndexes)
                    {
                        if (fileIndex >= 0 && fileIndex < dto.Files.Count)
                        {
                            var file = dto.Files[fileIndex];
                            var imageUrl = await _fileUploadService.UploadFileAsync(file);
                            var productImage = new ProductImage(imageUrl);
                            productImage.ProductVariationId = variation.Id;
                            variation.Imagens.Add(productImage);
                        }
                    }
                }
                product.Variations.Add(variation);
            }
            var savedProduct = await _productRepository.AddAsync(product);
            
            var productWithIncludes = await _productRepository.GetByIdAsync(savedProduct.Id);
            
            return MapToDTO(productWithIncludes);
        }

        public async Task<ProductResponseDTO?> GetProductByIdAsync(Guid id)
        {
            var product = await _productRepository.GetByIdAsync(id);

            return product == null ? null : MapToDTO(product);
        }

        public async Task<PageResponse<ProductResponseDTO>> GetAllProductsAsync(int pageNumber, int pageSize)
        {
            var (products, totalCount) = await _productRepository.GetAllAsync(pageNumber, pageSize);

            var dtos = products.Select(MapToDTO).ToList();

            return new PageResponse<ProductResponseDTO>(dtos, totalCount, pageNumber, pageSize);
        }

        public async Task<bool> DeleteProductAsync(Guid productId, Guid vendedorId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return false;

            if (product.VendorId != vendedorId)
                throw new UnauthorizedAccessException("Você não tem permissão para deletar este produto.");

            await _productRepository.DeleteAsync(productId);
            return true;
        }

        public async Task<List<VendorProductListDTO>> GetProductsByVendorIdAsync(Guid vendorId)
        {
            var products = await _productRepository.GetByVendorIdAsync(vendorId);

            return products.Select(p => new VendorProductListDTO
            {
                Id = p.Id,
                Name = p.Nome,
                Category = p.Categoria,
                Active = p.Status == ProductStatus.Ativo,

                Price = p.Variations.Any() ? p.Variations.Min(v => v.Preco) : 0,

                Image = p.Variations.FirstOrDefault()?
                           .Imagens.FirstOrDefault()?
                           .ImageUrl ?? "" 
            }).ToList();
        }

        public async Task SetProductStatusAsync(Guid productId, Guid vendorId, bool newStatus)
        {
            var product = await _productRepository.FindByIdAsync(productId);

            if (product == null)
                throw new KeyNotFoundException("Produto não encontrado.");

            if (product.VendorId != vendorId)
                throw new UnauthorizedAccessException("Você não tem permissão para editar este produto.");

            product.Status = newStatus ? ProductStatus.Ativo : ProductStatus.Inativo;
            await _productRepository.UpdateAsync(product);
        }

        public async Task<bool> UpdateVariationStockAsync(Guid productId, Guid vendorId, Guid variationId, int estoque)
        {
            var product = await _productRepository.GetByIdAsync(productId)
                ?? throw new KeyNotFoundException("Produto não encontrado.");

            if (product.VendorId != vendorId)
                throw new UnauthorizedAccessException("Você não tem permissão para editar este produto.");

            var variation = product.Variations?.FirstOrDefault(v => v.Id == variationId)
                ?? throw new KeyNotFoundException("Variação não encontrada.");

            variation.Estoque = estoque;
            await _productRepository.UpdateAsync(product);
            return true;
        }
        private static ProductResponseDTO MapToDTO(Product product)
        {
            return new ProductResponseDTO
            {
                Id = product.Id,
                Nome = product.Nome,
                Descricao = product.Descricao,
                Categoria = product.Categoria,
                VendedorId = product.VendorId,
                VendedorNome = product.Vendor?.Nome ?? "N/A",

                Variations = product.Variations?.Select(v => new VariationResponseDTO
                {
                    Id = v.Id,
                    Preco = v.Preco,
                    Estoque = v.Estoque,
                    Cor = v.Cor,
                    Tamanho = v.Tamanho,
                    ImageUrls = v.Imagens?.Select(img => img.ImageUrl).ToList() ?? new List<string>()
                }).ToList() ?? new List<VariationResponseDTO>()
            };
        }
    }
}
