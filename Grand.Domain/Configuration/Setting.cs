using Grand.Domain.Localization;
using System.Collections.Generic;

namespace Grand.Domain.Configuration
{
    /// <summary>
    /// Represents a setting
    /// </summary>
    public partial class Setting : BaseEntity, ILocalizedEntity
    {
        public Setting() {
            Locales = new List<LocalizedProperty>();
        }
        
        public Setting(string name, string value, string storeId = "") {
            Name = name;
            Value = value;
            StoreId = storeId;
        }
        
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the store for which this setting is valid. 0 is set when the setting is for all stores
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Gets or sets the collection of locales
        /// </summary>
        public IList<LocalizedProperty> Locales { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
