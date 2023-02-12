using Slim.Data.Entity;

namespace Slim.Shared.Interfaces.Serv;

public interface ICartService
{
    /// <summary>
    /// Get all cart items for a user
    /// </summary>
    /// <param name="loggedInUser"></param>
    /// <param name="defaultSessionUser"></param>
    /// <returns></returns>
    List<ShoppingCart> GetCartItemsForUser(string loggedInUser, string defaultSessionUser);

    /// <summary>
    /// Get the total price of all items in cart
    /// </summary>
    /// <param name="loggedInUser"></param>
    /// <param name="defaultSessionUser"></param>
    /// <returns></returns>
    decimal GetTotalCartPrice(string loggedInUser, string defaultSessionUser);

    /// <summary>
    /// Get the price for this product
    /// </summary>
    /// <param name="standardPrice"></param>
    /// <param name="salesPrice"></param>
    /// <returns></returns>
    (int StandardWholePrice, string StandardPriceRoundUp, int SalesWholePrice, string SalesPriceRoundUp) GetPriceForProduct(decimal standardPrice, decimal salesPrice);

    /// <summary>
    /// Get the Products in the cart
    /// </summary>
    /// <param name="products"></param>
    /// <param name="loggedInUser"></param>
    /// <param name="defaultSessionUser"></param>
    /// <returns></returns>
    List<Product> GetProductsWithInCartCheck(IEnumerable<Product> products, string loggedInUser, string defaultSessionUser);

    /// <summary>
    /// Get the type of product that it is
    /// </summary>
    /// <param name="razorPageId"></param>
    /// <returns></returns>
    string GetProductType(int razorPageId);
}