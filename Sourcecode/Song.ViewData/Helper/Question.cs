using Newtonsoft.Json.Linq;
using Song.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeiSha.Core;

namespace Song.ViewData.Helper
{
    /// <summary>
    /// 试题相关的处理方法
    /// </summary>
    public class Question
    {
        /// <summary>
        /// 转换试题中的文本
        /// </summary>
        /// <param name="ques"></param>
        /// <returns></returns>
        public static Song.Entities.Questions Transform(Song.Entities.Questions ques)
        {
            return ques;

            //if (ques == null) return ques;
            //ques.Qus_Title = _tranText(ques.Qus_Title);
            //ques.Qus_Answer = _tranText(ques.Qus_Answer);
            //ques.Qus_Explain = _tranText(ques.Qus_Explain);   

            //return ques;
        }
        /// <summary>
        /// 将试题的答题选项(Json)转换为数组
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static List<Song.Entities.QuesAnswer> AnswerToItems(Song.Entities.Questions entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Qus_Items)) return null;
            List<Song.Entities.QuesAnswer> items = new List<QuesAnswer>();
            JArray jaryy = JArray.Parse(entity.Qus_Items);
            if (jaryy != null)
            {
                for (int i = 0; i < jaryy.Count; i++)
                {
                    JToken jt = jaryy[i];
                    try
                    {
                        Song.Entities.QuesAnswer obj = ExecuteMethod.ValueToEntity<Song.Entities.QuesAnswer>(null, jt.ToString());
                        if (string.IsNullOrWhiteSpace(obj.Ans_Context)) continue;
                        //生成答案项的id
                        if (obj.Ans_ID <= 0) obj.Ans_ID = WeiSha.Core.Request.SnowID();
                        //填空题，每项都是正确的
                        if (entity.Qus_Type == 5)
                        {
                            obj.Ans_IsCorrect = true;
                            obj.Ans_Context = HTML.ClearTag(obj.Ans_Context);
                        }
                        items.Add(obj);
                    }
                    catch { }
                }
            }
            return items;
        }

        /// <summary>
        /// 处理试题中的文本
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static string TransformText(string txt)
        {
            if (string.IsNullOrWhiteSpace(txt)) return string.Empty;

            txt = txt.Replace("&lt;", "<");
            txt = txt.Replace("&gt;", ">");
            txt = txt.Replace("\n", "<br/>");
            txt = Html.ClearScript(txt);
            txt = Html.ClearAttr(txt, "p", "div", "font", "span", "a");
            txt = TransformImagePath(txt);
            txt = txt.Replace("&nbsp;", " ");
            return txt;
        }
        /// <summary>
        /// 处理试题中的图片
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string TransformImagePath(string text)
        {
            RegexOptions options = RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase;
            //将超链接处理为相对于模版页的路径
            string linkExpr = @"<(img)[^>]+>";
            foreach (Match match in new Regex(linkExpr, options).Matches(text))
            {
                string tagName = match.Groups[1].Value.Trim();      //标签名称
                string tagContent = match.Groups[0].Value.Trim();   //标签内容
                string expr = @"(?<=\s+)(?<key>src[^=""']*)=([""'])?(?<value>[^'"">]*)\1?";
                foreach (Match m in new Regex(expr, options).Matches(tagContent))
                {
                    string key = m.Groups["key"].Value.Trim();      //属性名称
                    string val = m.Groups["value"].Value.Trim();    //属性值    
                    if (val.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) continue;
                    if (val.StartsWith("https://", StringComparison.OrdinalIgnoreCase)) continue;
                    if (val.StartsWith("data:image/", StringComparison.OrdinalIgnoreCase)) continue;

                    val = val.Replace("&apos;", "");
                    if (val.EndsWith("/")) val = val.Substring(0, val.Length - 1);
                    val = m.Groups[2].Value + "=\"" + val + "\"";
                    val = Regex.Replace(val, @"//", "/");

                    tagContent = tagContent.Replace(m.Value, val);
                }
                text = text.Replace(match.Groups[0].Value.Trim(), tagContent);
            }
            return text;
        }
    }
}
