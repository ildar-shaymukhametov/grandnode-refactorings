#region Copyright � 2001-2003 Jean-Claude Manoli [jc@manoli.net]
/*
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the author(s) be held liable for any damages arising from
 * the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 *   1. The origin of this software must not be misrepresented; you must not
 *      claim that you wrote the original software. If you use this software
 *      in a product, an acknowledgment in the product documentation would be
 *      appreciated but is not required.
 * 
 *   2. Altered source versions must be plainly marked as such, and must not
 *      be misrepresented as being the original software.
 * 
 *   3. This notice may not be removed or altered from any source distribution.
 */ 
#endregion

using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Grand.Core.Html.CodeFormatter
{
	/// <summary>
	/// Provides a base class for formatting most programming languages.
	/// </summary>
    public abstract partial class CodeFormat : SourceFormat
	{
		/// <summary>
		/// Must be overridden to provide a list of keywords defined in 
		/// each language.
		/// </summary>
		/// <remarks>
		/// Keywords must be separated with spaces.
		/// </remarks>
		protected abstract string Keywords 
		{
			get;
		}

		/// <summary>
		/// Can be overridden to provide a list of preprocessors defined in 
		/// each language.
		/// </summary>
		/// <remarks>
		/// Preprocessors must be separated with spaces.
		/// </remarks>
		protected virtual string Preprocessors
		{
			get { return ""; }
		}

		/// <summary>
		/// Must be overridden to provide a regular expression string
		/// to match strings literals. 
		/// </summary>
		protected abstract string StringRegex
		{
			get;
		}

		/// <summary>
		/// Must be overridden to provide a regular expression string
		/// to match comments. 
		/// </summary>
		protected abstract string CommentRegex
		{
			get;
		}

		/// <summary>
		/// Determines if the language is case sensitive.
		/// </summary>
		/// <value><b>true</b> if the language is case sensitive, <b>false</b> 
		/// otherwise. The default is true.</value>
		/// <remarks>
		/// A case-insensitive language formatter must override this 
		/// property to return false.
		/// </remarks>
		public virtual bool CaseSensitive
		{
			get { return true; }
		}

		/// <summary/>
		protected CodeFormat()
        {
            //generate the keyword and preprocessor regexes from the keyword lists
            var r = new Regex(@"\w+|-\w+|#\w+|@@\w+|#(?:\\(?:s|w)(?:\*|\+)?\w+)+|@\\w\*+");
            var r2 = new Regex(@" +");
            var regKeyword = GetKeywordRegex(r, r2);
            var regPreproc = GetPreprocessorRegex(r, r2);

            var regAll = BuildMasterRegex(regKeyword, regPreproc);

            RegexOptions caseInsensitive = CaseSensitive ? 0 : RegexOptions.IgnoreCase;
            CodeRegex = new Regex(regAll.ToString(), RegexOptions.Singleline | caseInsensitive);
        }

        private string GetKeywordRegex(Regex r, Regex r2)
        {
            string result = r.Replace(Keywords, @"(?<=^|\W)$0(?=\W)");
            result = r2.Replace(result, @"|");
            return result;
        }

        private string GetPreprocessorRegex(Regex r, Regex r2)
        {
            string result = r.Replace(Preprocessors, @"(?<=^|\s)$0(?=\s|$)");
            result = r2.Replace(result, @"|");

            if (result.Length == 0)
            {
                result = "(?!.*)_{37}(?<!.*)"; //use something quite impossible...
            }

            return result;
        }

        private StringBuilder BuildMasterRegex(string regKeyword, string regPreproc)
        {
            var result = new StringBuilder();
            result.Append("(");
            result.Append(CommentRegex);
            result.Append(")|(");
            result.Append(StringRegex);
            if (regPreproc.Length > 0)
            {
                result.Append(")|(");
                result.Append(regPreproc);
            }
            result.Append(")|(");
            result.Append(regKeyword);
            result.Append(")");
            return result;
        }

        /// <summary>
        /// Called to evaluate the HTML fragment corresponding to each 
        /// matching token in the code.
        /// </summary>
        /// <param name="match">The <see cref="Match"/> resulting from a 
        /// single regular expression match.</param>
        /// <returns>A string containing the HTML code fragment.</returns>
        protected override string MatchEval(Match match)
		{
			var result = "";
			if(match.Groups[1].Success)
            {
                result = GenerateCommentFragment(match);
            }
            if (match.Groups[2].Success) //string literal
            {
				result = "<span class=\"str\">" + match.ToString() + "</span>";
			}
			if(match.Groups[3].Success) //preprocessor keyword
			{
				result = "<span class=\"preproc\">" + match.ToString() + "</span>";
			}
			if(match.Groups[4].Success) //keyword
			{
				result = "<span class=\"kwrd\">" + match.ToString() + "</span>";
			}
			System.Diagnostics.Debug.Assert(false, "None of the above!");
			return result; //none of the above
		}

        private static string GenerateCommentFragment(Match match)
        {
            var reader = new StringReader(match.ToString());
            string line;
            var result = new StringBuilder();
            while ((line = reader.ReadLine()) != null)
            {
                if (result.Length > 0)
                {
                    result.Append("\n");
                }
                result.Append("<span class=\"rem\">");
                result.Append(line);
                result.Append("</span>");
            }
            return result.ToString();
        }
    }
}

