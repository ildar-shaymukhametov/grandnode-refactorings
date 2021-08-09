using Grand.Domain.Customers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grand.Services.Authentication.External
{
    /// <summary>
    /// External authentication service
    /// </summary>
    public partial interface IExternalAuthenticationService
    {
        #region External authentication methods

        /// <summary>
        /// Load active external authentication methods
        /// </summary>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass "" to load all records</param>
        /// <returns>Payment methods</returns>
        IList<IExternalAuthenticationMethod> LoadActiveExternalAuthenticationMethods(Customer customer = null, string storeId = "");

        /// <summary>
        /// Load external authentication method by system name
        /// </summary>
        /// <param name="systemName">System name</param>
        /// <returns>Found external authentication method</returns>
        IExternalAuthenticationMethod LoadExternalAuthenticationMethodBySystemName(string systemName);

        /// <summary>
        /// Load all external authentication methods
        /// </summary>
        /// <param name="customer">Load records allowed only to a specified customer; pass null to ignore ACL permissions</param>
        /// <param name="storeId">Load records allowed only in a specified store; pass "" to load all records</param>
        /// <returns>External authentication methods</returns>
        IList<IExternalAuthenticationMethod> LoadAllExternalAuthenticationMethods(Customer customer = null, string storeId = "");

        /// <summary>
        /// Check whether authentication by the passed external authentication method is available 
        /// </summary>
        /// <param name="systemName">System name of the external authentication method</param>
        /// <returns>True if authentication is available; otherwise false</returns>
        bool ExternalAuthenticationMethodIsAvailable(string systemName);

        #endregion

        #region Authentication

        /// <summary>
        /// Authenticate user by passed parameters
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        /// <param name="returnUrl">URL to which the user will return after authentication</param>
        /// <returns>Result of an authentication</returns>
        Task<IActionResult> Authenticate(ExternalAuthenticationParameters parameters, string returnUrl = null);

        #endregion

        /// <summary>
        /// Accociate external account with customer
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="parameters">External authentication parameters</param>
        Task AssociateExternalAccountWithUser(Customer customer, ExternalAuthenticationParameters parameters);

        /// <summary>
        /// Get the particular user with specified parameters
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        /// <returns>Customer</returns>
        Task<Customer> GetUserByExternalAuthenticationParameters(ExternalAuthenticationParameters parameters);

        Task<IList<ExternalAuthenticationRecord>> GetExternalIdentifiersFor(Customer customer);
        /// <summary>
        /// Remove the association
        /// </summary>
        /// <param name="parameters">External authentication parameters</param>
        Task RemoveAssociation(ExternalAuthenticationParameters parameters);

        /// <summary>
        /// Delete the external authentication record
        /// </summary>
        /// <param name="externalAuthenticationRecord">External authentication record</param>
        Task DeleteExternalAuthenticationRecord(ExternalAuthenticationRecord externalAuthenticationRecord);
    }
}