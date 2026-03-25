using Core.Abstracts.IRepositories;
using Utils.Responses;

namespace Core.Abstracts
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IProductRepository ProductRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductReviewRepository ProductReviewRepository { get; }
        IProductAttributeRepository ProductAttributeRepository { get; }
        IProductImageRepository ProductImageRepository { get; }
        IBrandRepository BrandRepository { get; }
        ICartRepository CartRepository { get; }
        ICartItemRepository CartItemRepository { get; }
        ISubCategoryRepository SubCategoryRepository { get; }
        Task <IResult> CommitAsync();
    }
}
