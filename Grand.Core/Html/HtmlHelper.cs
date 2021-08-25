using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace Grand.Core.Html
{
    /// <summary>
    /// Represents a HTML helper
    /// </summary>
    public static class HtmlHelper
    {
        #region Fields
        private readonly static Regex paragraphStartRegex = new Regex("<p>", RegexOptions.IgnoreCase);
        private readonly static Regex paragraphEndRegex = new Regex("</p>", RegexOptions.IgnoreCase);

        #endregion

        #region Utilities

        public static string EnsureOnlyAllowedHtml(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            var allowedTags = "br,hr,b,i,u,a,div,ol,ul,li,blockquote,img,span,p,em,strong,font,pre,h1,h2,h3,h4,h5,h6,address,cite".Split(',');

            return Regex.Matches(text, "<.*?>", RegexOptions.IgnoreCase)
                .Aggregate(text, (acc, match) =>
                {
                    string tag = acc.Substring(match.Index + 1, match.Length - 1).Trim().ToLower();

                    if (!IsValidTag(tag, allowedTags))
                    {
                        acc = acc.Remove(match.Index, match.Length);
                    }
                    return acc;
                });
        }

        private static bool IsValidTag(string tag, string[] allowedTags)
        {
            if (tag.IndexOf("javascript") >= 0) return false;
            if (tag.IndexOf("vbscript") >= 0) return false;
            if (tag.IndexOf("onclick") >= 0) return false;

            return allowedTags.Any(x => x == GetTag(tag));
        }

        private static string GetTag(string tag)
        {
            var pos = GetPos(tag);
            if (pos > 0) tag = tag.Substring(0, pos);
            if (tag[0] == '/') tag = tag.Substring(1);
            return tag;
        }

        private static int GetPos(string tag)
        {
            var endchars = new[] { ' ', '>', '/', '\t' };
            return tag.IndexOfAny(endchars, 1);
        }
        #endregion

        #region Methods

        public static string FormatText(string text)
        {
            text = WebUtility.HtmlEncode(text);
            text = ConvertPlainTextToHtml(text);
            return text;
        }

       
        /// <summary>
        /// Converts plain text to HTML
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string ConvertPlainTextToHtml(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            text = text.Replace("\r\n", "<br />");
            text = text.Replace("\r", "<br />");
            text = text.Replace("\n", "<br />");
            text = text.Replace("\t", "&nbsp;&nbsp;");
            text = text.Replace("  ", "&nbsp;&nbsp;");

            return text;
        }

        #endregion
    }
}
