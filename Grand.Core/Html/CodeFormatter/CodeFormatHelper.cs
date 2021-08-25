using System;
using System.Text.RegularExpressions;
using System.Net;

namespace Grand.Core.Html.CodeFormatter
{
    /// <summary>
    /// Represents a code format helper
    /// </summary>
    public static class CodeFormatHelper
    {
        #region Fields
        private readonly static Regex regexCode2 = new Regex(@"\[code\](?<inner>(.*?))\[/code\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        #endregion

        #region Utilities

        /// <summary>
        /// Code evaluator method
        /// </summary>
        /// <param name="match">Match</param>
        /// <returns>Formatted text</returns>
        private static string CodeEvaluator(Match match)
        {
            if (!match.Success)
                return match.Value;

            string result = match.Value.Replace(match.Groups["begin"].Value, "");
            result = result.Replace(match.Groups["end"].Value, "");
            result = Highlight(GetOptions(match), result);
            return result;

            static HighlightOptions GetOptions(Match match)
            {
                var options = new HighlightOptions();

                options.Language = match.Groups["lang"].Value;
                options.Code = match.Groups["code"].Value;
                options.DisplayLineNumbers = match.Groups["linenumbers"].Value == "on";
                options.Title = match.Groups["title"].Value;
                options.AlternateLineNumbers = match.Groups["altlinenumbers"].Value == "on";
                return options;
            }
        }


        /// <summary>
        /// Code evaluator method
        /// </summary>
        /// <param name="match">Match</param>
        /// <returns>Formatted text</returns>
        private static string CodeEvaluatorSimple(Match match)
        {
            if (!match.Success)
                return match.Value;

            string result = match.Value;
            result = Highlight(GetOptions(match), result);
            return result;

            static HighlightOptions GetOptions(Match match)
            {
                var options = new HighlightOptions();

                options.Language = "c#";
                options.Code = match.Groups["inner"].Value;
                options.DisplayLineNumbers = false;
                options.Title = string.Empty;
                options.AlternateLineNumbers = false;
                return options;
            }
        }

        /// <summary>
        /// Returns the formatted text.
        /// </summary>
        /// <param name="options">Whatever options were set in the regex groups.</param>
        /// <param name="text">Send the e.body so it can get formatted.</param>
        /// <returns>The formatted string of the match.</returns>
        private static string Highlight(HighlightOptions options, string text)
        {
            var formatter = new FormatterFactory(options).Create();
            if (formatter == null)
            {
                return string.Empty;
            }

            return formatter.Format(text);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Formats the text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string FormatTextSimple(string text)
        {
            if (String.IsNullOrEmpty(text))
                return string.Empty;

            if (!text.Contains("[/code]"))
                return text;

            text = regexCode2.Replace(text, new MatchEvaluator(CodeEvaluatorSimple));
            text = regexCode2.Replace(text, "$1");
            return text;
        }

        #endregion
    }

    public class FormatterFactory
    {
        private readonly Regex htmlRegex;
        private readonly HighlightOptions options;

        public FormatterFactory(HighlightOptions options)
        {
            this.htmlRegex = new Regex("<[^>]*>", RegexOptions.Compiled);
            this.options = options;
        }

        public Formatter Create()
        {
            Formatter result = null;
            switch (options.Language)
            {
                case "c#":
                    result = new CSharpFormatter(options);
                    break;
                case "vb":
                    result = new VisualBasicFormatter(options);
                    break;
                case "js":
                    result = new JavaScriptFormatter(options);
                    break;
                case "html":
                    result = new HtmlFormatter(options, htmlRegex);
                    break;
                case "xml":
                    result = new HtmlFormatter(options, htmlRegex);
                    break;
                case "msh":
                    result = new MshFormatter(options);
                    break;
            }
            return result;
        }
    }

    public abstract class Formatter
    {
        protected readonly HighlightOptions options;
        public Formatter(HighlightOptions options)
        {
            this.options = options;

        }
        public abstract string Format(string text);
    }

    public class CSharpFormatter : Formatter
    {
        public CSharpFormatter(HighlightOptions options) : base(options) { }

        public override string Format(string text)
        {
            var csf = new CSharpFormat();
            csf.LineNumbers = options.DisplayLineNumbers;
            csf.Alternate = options.AlternateLineNumbers;
            return WebUtility.HtmlDecode(csf.FormatCode(text));
        }
    }

    public class VisualBasicFormatter : Formatter
    {
        public VisualBasicFormatter(HighlightOptions options) : base(options) { }

        public override string Format(string text)
        {
            var vbf = new VisualBasicFormat();
            vbf.LineNumbers = options.DisplayLineNumbers;
            vbf.Alternate = options.AlternateLineNumbers;
            return vbf.FormatCode(text);
        }
    }

    public class JavaScriptFormatter : Formatter
    {
        public JavaScriptFormatter(HighlightOptions options) : base(options) { }

        public override string Format(string text)
        {
            var jsf = new JavaScriptFormat();
            jsf.LineNumbers = options.DisplayLineNumbers;
            jsf.Alternate = options.AlternateLineNumbers;
            return WebUtility.HtmlDecode(jsf.FormatCode(text));
        }
    }

    public class HtmlFormatter : Formatter
    {
        private readonly Regex regexHtml;

        public HtmlFormatter(HighlightOptions options, Regex regexHtml) : base(options)
        {
            this.regexHtml = regexHtml;
        }

        public override string Format(string text)
        {
            var htmlf = new HtmlFormat();
            htmlf.LineNumbers = options.DisplayLineNumbers;
            htmlf.Alternate = options.AlternateLineNumbers;
            text = regexHtml.Replace(text, string.Empty).Trim();
            string code = htmlf.FormatCode(WebUtility.HtmlDecode(text)).Trim();
            return code.Replace("\r\n", "<br />").Replace("\n", "<br />");
        }
    }

    public class XmlFormatter : Formatter
    {
        private readonly Regex regexHtml;

        public XmlFormatter(HighlightOptions options, Regex regexHtml) : base(options)
        {
            this.regexHtml = regexHtml;
        }

        public override string Format(string text)
        {
            var xmlf = new HtmlFormat();
            xmlf.LineNumbers = options.DisplayLineNumbers;
            xmlf.Alternate = options.AlternateLineNumbers;
            text = text.Replace("<br />", "\r\n");
            text = regexHtml.Replace(text, string.Empty).Trim();
            string xml = xmlf.FormatCode(WebUtility.HtmlDecode(text)).Trim();
            return xml.Replace("\r\n", "<br />").Replace("\n", "<br />");
        }
    }

    public class MshFormatter : Formatter
    {
        public MshFormatter(HighlightOptions options) : base(options) { }

        public override string Format(string text)
        {
            var mshf = new MshFormat();
            mshf.LineNumbers = options.DisplayLineNumbers;
            mshf.Alternate = options.AlternateLineNumbers;
            return WebUtility.HtmlDecode(mshf.FormatCode(text));
        }
    }
}

