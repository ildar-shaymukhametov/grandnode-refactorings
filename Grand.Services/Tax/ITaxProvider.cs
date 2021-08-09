using Grand.Core.Plugins;
using System.Threading.Tasks;

namespace Grand.Services.Tax
{
    /// <summary>
    /// Provides an interface for creating tax providers
    /// </summary>
    public partial interface ITaxProvider : IPlugin
    {
        /// <summary>
        /// Gets tax rate
        /// </summary>
        /// <param name="calculateTaxRequest">Tax calculation request</param>
        /// <returns>Tax</returns>
        Task<CalculateTaxResult> GetTaxRate(CalculateTaxRequest calculateTaxRequest);

    }
}
