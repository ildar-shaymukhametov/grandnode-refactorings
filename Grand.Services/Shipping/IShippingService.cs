using Grand.Domain.Common;
using Grand.Domain.Customers;
using Grand.Domain.Orders;
using Grand.Domain.Shipping;
using Grand.Domain.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grand.Services.Shipping
{
    /// <summary>
    /// Shipping service interface
    /// </summary>
    public partial interface IShippingService
    {
        /// <summary>
        /// Load active shipping rate computation methods
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass "" to load all records</param>
        /// <returns>Shipping rate computation methods</returns>
        Task<IList<IShippingRateComputationMethod>> LoadActiveShippingRateComputationMethods(string storeId = "", IList<ShoppingCartItem> cart = null);

        /// <summary>
        /// Load shipping rate computation method by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found Shipping rate computation method</returns>
        IShippingRateComputationMethod LoadShippingRateComputationMethodBySystemName(string systemName);

        /// <summary>
        /// Load all shipping rate computation methods
        /// </summary>
        /// <param name="storeId">Load records allowed only in a specified store; pass "" to load all records</param>
        /// <returns>Shipping rate computation methods</returns>
        IList<IShippingRateComputationMethod> LoadAllShippingRateComputationMethods(string storeId = "");
        
        /// <summary>
        /// Gets shopping cart item weight (of one item)
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns>Shopping cart item weight</returns>
        Task<decimal> GetShoppingCartItemWeight(ShoppingCartItem shoppingCartItem);

        /// <summary>
        /// Gets shopping cart weight
        /// </summary>
        /// <param name="request">Request</param>
        /// <param name="includeCheckoutAttributes">A value indicating whether we should calculate weights of selected checkotu attributes</param>
        /// <returns>Total weight</returns>
        Task<decimal> GetTotalWeight(GetShippingOptionRequest request, bool includeCheckoutAttributes = true);

        /// <summary>
        /// Get dimensions of associated products (for quantity 1)
        /// </summary>
        /// <param name="shoppingCartItem">Shopping cart item</param>
        /// <returns name="width">Width</returns>
        /// <returns name="length">Length</returns>
        /// <returns name="height">Height</returns>
        Task<(decimal width, decimal length, decimal height)> GetAssociatedProductDimensions(ShoppingCartItem shoppingCartItem);

        /// <summary>
        /// Get total dimensions
        /// </summary>
        /// <param name="packageItems">Package items</param>
        /// <param name="width">Width</param>
        /// <param name="length">Length</param>
        /// <param name="height">Height</param>
        Task<(decimal width, decimal length, decimal height)> GetDimensions(IList<GetShippingOptionRequest.PackageItem> packageItems);

        /// <summary>
        /// Get the nearest warehouse for the specified address
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="warehouses">List of warehouses, if null all warehouses are used.</param>
        /// <returns></returns>
        Task<Warehouse> GetNearestWarehouse(Address address, IList<Warehouse> warehouses = null);

        /// <summary>
        /// Create shipment packages (requests) from shopping cart
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass "" to load all records</param>
        /// <returns>Shipment packages (requests)</returns>
        /// <param name="shippingFromMultipleLocations">Value indicating whether shipping is done from multiple locations (warehouses)</param>
        Task<(IList<GetShippingOptionRequest> shippingOptionRequest, bool shippingFromMultipleLocations)> CreateShippingOptionRequests(Customer customer,
            IList<ShoppingCartItem> cart, Address shippingAddress, Store store);

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <param name="shippingAddress">Shipping address</param>
        /// <param name="allowedShippingRateComputationMethodSystemName">Filter by shipping rate computation method identifier; null to load shipping options of all shipping rate computation methods</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass "" to load all records</param>
        /// <returns>Shipping options</returns>
        Task<GetShippingOptionResponse> GetShippingOptions(Customer customer, IList<ShoppingCartItem> cart, Address shippingAddress,
            string allowedShippingRateComputationMethodSystemName = "", Store store = null);
    }
}
