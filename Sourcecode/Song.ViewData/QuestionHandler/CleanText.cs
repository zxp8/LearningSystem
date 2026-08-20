using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Song.ViewData.QuestionHandler
{
    /// <summary>
    /// 清理试题的文本
    /// </summary>
    public class CleanText
    {
        /// <summary>
        /// 试题标签的清理，清除html标签与js
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Title(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;            
            text = JsFunction(text);
            text = Html(text);
            return text;
        }
        /// <summary>
        /// 清理试题内容，清理脚本，保留html标签
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Content(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            text = JsFunction(text); 
            return text;
        }
        /// <summary>
        /// 清理脚本
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string JsFunction(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            RegexOptions option = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace;
            //删除脚本
            text = Regex.Replace(text, @"<script[^>]+?>[\s\S]*?</script>", "", option);
            text = Regex.Replace(text, @"<script[^>]*>[\s\S]*?</script>", "", option);
            text = Regex.Replace(text, @"//\(function\(\)[\s\S]+?}\)\(\);", "", option);
            return text;
        }
        /// <summary>
        /// 清理HTML标签
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string Html(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            RegexOptions option = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace;
            //删除HTML           
            text = Regex.Replace(text, @"([\r\n])[\s]+", "", option);
            //text = Regex.Replace(text, @"<(.[^>]*)>", "", option);
            text = ViewData.Html.ClearAttr(text, "p", "div", "font", "span", "a");
            text = Regex.Replace(text, @"-->", "", option);
            text = Regex.Replace(text, @"<!--.*", "", option);
            return text;
        }
    }
}
