using ECommerceApp.ViewModels;

namespace ECommerceApp.Services
{
    public interface ICartService
    {
        void AddItem(int productId, int quantity);
        void UpdateQuantity(int productId, int quantity);
        void RemoveItem(int productId);
        List<CartItemViewModel> GetCartItems();
        decimal GetTotal();
        void ClearCart();
        int GetCartItemCount();
    }
}

