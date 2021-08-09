using Grand.Domain.Orders;
using Grand.Core.Plugins;
using Grand.Services.Shipping.Tracking;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grand.Services.Shipping
{
    /// <summary>
    /// Provides an interface of shipping rate computation method
    /// </summary>
    public partial interface IShippingRateComputationMethod : IPlugin
    {
        /// <summary>
        /// Gets a shipping rate computation method type
        /// </summary>
        ShippingRateComputationMethodType ShippingRateComputationMethodType { get; }

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Represents a response of getting shipping rate options</returns>
        Task<GetShippingOptionResponse> GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest);

        /// <summary>
        /// Returns a value indicating whether shipping methods should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shoping cart</param>
        /// <returns>true - hide; false - display.</returns>
        Task<bool> HideShipmentMethods(IList<ShoppingCartItem> cart);

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
        Task<decimal?> GetFixedRate(GetShippingOptionRequest getShippingOptionRequest);

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        IShipmentTracker ShipmentTracker { get; }


        Task<IList<string>> ValidateShippingForm(IFormCollection form);

        /// <summary>
        /// Gets a view component for displaying plugin in public store ("shipping" checkout step)
        /// </summary>
        /// <param name="viewComponentName">View component name</param>
        void GetPublicViewComponent(out string viewComponentName);

    }
}
