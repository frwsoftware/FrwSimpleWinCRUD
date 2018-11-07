/**********************************************************************************
 *   FrwSimpleWinCRUD   https://github.com/frwsoftware/FrwSimpleWinCRUD
 *   The Open-Source Library for most quick  WinForm CRUD application creation
 *   MIT License Copyright (c) 2016 FrwSoftware
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 **********************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace FrwSoftware
{
    public class HtmlUtils
    {
        static public string HTML_TEMPLATE = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01//EN\" \"http://www.w3.org/TR/html4/strict.dtd\"><html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=UTF-8\"><title>{0}</title><link rel=\"stylesheet\" type=\"text/css\" href=\"{1}\">{2}</head><body>{3}</body></html>";
        static public string MakeHtmlFromTemplate(string title, string css, string additional, string body)
        {
            return string.Format(HTML_TEMPLATE, title, css, additional, body);
        }

        //slow
        static public bool CheckIsHtmlAnyDoubleTags(string text)
        {
            //The following will match any matching set of tags.i.e. < b > this </ b >
            Regex tagRegex = new Regex(@"<\s*([^ >]+)[^>]*>.*?<\s*/\s *\1\s*>");
            return tagRegex.IsMatch(text);
        }
        //fastest
        static public bool CheckIsHtmlAnySingleTags(string text)
        {
            //The following will match any single tag.i.e. <b> (it doesn't have to be closed).
            Regex tagRegex = new Regex(@"<[^>]+>");
            return tagRegex.IsMatch(text);
        }
        //too slow
        static public bool CheckIsHtmlSpecialTags(string text)
        {
            return CheckIsHtmlTagsTipicalList(text);
        }
        //fast
        static public bool CheckIsHtmlFull(string text)
        {

            if (text == null) return false;
            if (text.IndexOfAny(new char[] { '«', '»' }) > -1)// if present HttpUtility.HtmlEncode thinks that html 
            {
                return CheckIsHtmlAnySingleTags(text);
            }
            else
            {
                bool containsHTML = (text != HttpUtility.HtmlEncode(text));//System.Web
                return containsHTML;
            }
        }
        public static bool CheckIsHtmlSingleTags(string text, string tag)
        {
            var pattern = @"<\s*" + tag + @"\s*\/?>";
            return Regex.IsMatch(text, pattern, RegexOptions.IgnoreCase);
        }
        public static bool CheckIsHtmlTagsFromList(string text, string tags)
        {
            var ba = tags.Split('|').Select(x => new { tag = x, hastag = CheckIsHtmlSingleTags(text, x) }).Where(x => x.hastag);

            return ba.Count() > 0;
        }

        public static bool CheckIsHtmlTagsTipicalList(string text)
        {
            return
                CheckIsHtmlTagsFromList(text,
                    "a|abbr|acronym|address|area|b|base|bdo|big|blockquote|body|br|button|caption|cite|code|col|colgroup|dd|del|dfn|div|dl|DOCTYPE|dt|em|fieldset|form|h1|h2|h3|h4|h5|h6|head|html|hr|i|img|input|ins|kbd|label|legend|li|link|map|meta|noscript|object|ol|optgroup|option|p|param|pre|q|samp|script|select|small|span|strong|style|sub|sup|table|tbody|td|textarea|tfoot|th|thead|title|tr|tt|ul|var");
        }

        public static string ConvertHtmlToPlainTextRegexp(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);


            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //frw replace <P> with NewLine
            text = text.Replace("<p>", Environment.NewLine).Replace("<P>", Environment.NewLine).Replace("</p>", "").Replace("</P>", ""); 
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);
            //frw remove first NewLine
            if (text.StartsWith(Environment.NewLine)) text = text.Substring(Environment.NewLine.Length);
            return text;
        }


        public static void AppendBr(StringBuilder str)
        {
            str.Append("<br/>");
        }
        public static void AppendDosBr(StringBuilder str)
        {
            str.Append("\n\r");
        }
        static public bool CheckUrlIsValid(string url)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }

        static public IList<string> SpliteTextIntoParagraph(string text)
        {
            return text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }
        static public string CreateHtmlBRTextFromText(string text)
        {
            StringBuilder sb = new StringBuilder();
            IList<string> strings = SpliteTextIntoParagraph(text);
            foreach (var str in strings)
            {
                sb.Append(str);
                sb.Append("<br/>");
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
        static public IList<HtmlNode> CreateHtmlParagraphsFromText(string text, HtmlDocument doc)
        {
            List<HtmlNode> nodes = new List<HtmlNode>();
            IList<string> strings = SpliteTextIntoParagraph(text);
            foreach (var str in strings)
            {
                HtmlNode p = doc.CreateElement("p");
                p.InnerHtml = str;
                nodes.Add(p);
            }
            return nodes;
        }
        static public string CreateHtmlParagraphsFromText(string text)
        {
            StringBuilder sb = new StringBuilder();
            IList<string> strings = SpliteTextIntoParagraph(text);
            foreach (var str in strings)
            {
                sb.Append("<p>");
                sb.Append(str);
                sb.Append("</p>");
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }
    }
}
