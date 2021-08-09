﻿using Grand.Domain.Customers;
using Grand.Domain.Directory;
using Grand.Domain.Localization;
using Grand.Domain.Orders;
using Grand.Domain.Stores;
using Grand.Web.Models.Checkout;
using MediatR;
using System.Collections.Generic;

namespace Grand.Web.Features.Models.Checkout
{
    public class GetBillingAddress : IRequest<CheckoutBillingAddressModel>
    {
        public IList<ShoppingCartItem> Cart { get; set; }

        public string SelectedCountryId { get; set; } = null;
        public bool PrePopulateNewAddressWithCustomerFields { get; set; } = false;
        public string OverrideAttributesXml { get; set; } = "";
        public Customer Customer { get; set; }
        public Store Store { get; set; }
        public Currency Currency { get; set; }
        public Language Language { get; set; }
    }
}
