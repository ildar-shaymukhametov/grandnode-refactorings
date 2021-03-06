using Grand.Domain.Vendors;
using Grand.Core.Html;
using System;

namespace Grand.Services.Vendors
{
    public static class VendorExtensions
    {
        /// <summary>
        /// Formats the vendor note text
        /// </summary>
        /// <param name="vendorNote">Vendor note</param>
        /// <returns>Formatted text</returns>
        public static string FormatVendorNoteText(this VendorNote vendorNote)
        {
            if (vendorNote == null)
                throw new ArgumentNullException("vendorNote");

            string text = vendorNote.Note;

            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = HtmlHelper.FormatText(text);

            return text;
        }
    }
}
