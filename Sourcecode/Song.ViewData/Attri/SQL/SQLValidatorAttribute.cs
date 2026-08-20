using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Song.ViewData.Attri
{
    /// <summary>
    /// SQL注入的敏感字符过滤
    /// </summary>
    public class SQLValidatorAttribute : WeishaAttr
    {    
        /// <summary>
        /// 不校验的参数名，多个用逗号隔开
        /// </summary>
        public string Not { get; set; }
        public SQLValidatorAttribute()
        {

        }
        /// <summary>
        /// 清理SQL注入的敏感字符
        /// </summary>
        /// <param name="method">执行的方法</param>
        /// <param name="letter">客户端传来的信息</param>
        /// <returns></returns>
        public static SQLValidatorAttribute Clear(MethodInfo method, Letter letter)
        {
            SQLValidatorAttribute attr = WeishaAttr.GetAttr<SQLValidatorAttribute>(method);
            string[] nots = new string[] { };
            if (attr != null) nots = attr.Not.Split(',');

            foreach (ParameterInfo param in method.GetParameters())
            {
                if (param.ParameterType != typeof(string)) continue;    //不是字符串类型的跳过
                                                                        //明确不检测的跳过
                bool exists = nots.Any(s => s.IndexOf(param.Name, StringComparison.OrdinalIgnoreCase) >= 0);
                if (exists) continue;
                //清理危险字符
                string val = _sqlClear(letter.GetParameter(param.Name).Value);
                letter.SetParameter(param.Name, val);
            }
            return attr;           
        }
        /// <summary>
        /// 校验
        /// </summary>
        /// <param name="method"></param>
        /// <param name="letter"></param>
        /// <returns>通过校验为true</returns>
        public static bool Verify(MethodInfo method, Letter letter)
        {
            SQLValidatorAttribute attr = WeishaAttr.GetAttr<SQLValidatorAttribute>(method);
            string[] nots = new string[] { };
            if (attr != null) nots = attr.Not.Split(',');
            foreach (ParameterInfo param in method.GetParameters())
            {
                if (param.ParameterType != typeof(string)) continue;    //不是字符串类型的跳过
                                                                        //明确不检测的跳过
                bool exists = nots.Any(s => s.IndexOf(param.Name, StringComparison.OrdinalIgnoreCase) >= 0);
                if (exists) continue;
                //检测是否存在危险字符
                string val = letter.GetParameter(param.Name).Value;
                string danger = _getSqlInjectionWord(val);
                if (!string.IsNullOrWhiteSpace(danger)) throw new Exception($"参数 {param.Name} 的内容“{val}”存在危险字符:{danger}");               
            }               
            return true;
        }
        /// <summary>
        /// Sql敏感字符串
        /// </summary>
        private static  readonly string[] sqlInjectionPatterns = {
            @"\bSELECT\b.*\bFROM\b",
            @"\bINSERT\b.*\bINTO\b",
            @"\bUPDATE\b.*\bSET\b",
            @"\bDELETE\b.*\bFROM\b",
            @"\bDROP\b.*\bTABLE\b",
            @"\bUNION\b.*\bSELECT\b",
            @"\bEXEC\b.*\(",
            @"\bEXECUTE\b.*\(",
            @"\bDECLARE\b.*@",
            @"\bCAST\b.*\(",
            @"--",
            @"';.*--",
            @"\bOR\b.*=",
            @"\bAND\b.*=",
            @"'.*\bOR\b.*'.*=",
            @"\bWAITFOR\b.*\bDELAY\b",
            @"\bBENCHMARK\b.*\("
        };
    
        /// <summary>
        /// 验证输入是否包含SQL注入特征
        /// </summary>
        private static bool _isSqlInjectionSafe(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            foreach (string pattern in sqlInjectionPatterns)            
                if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase)) return true;            
            return false;
        }
        /// <summary>
        /// 检索字符串出中带有SQL注入特征的字符
        /// </summary>
        /// <param name="input">被校验的字符串</param>
        /// <returns>SQL注的字符</returns>
        private static string _getSqlInjectionWord(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            // 使用HashSet去重，避免重复字符
            HashSet<string> foundPatterns = new HashSet<string>();
            //sqlInjectionPatterns，是Sql敏感字符串，静态变量
            foreach (string pattern in sqlInjectionPatterns)
            {
                // 使用正则表达式进行匹配（需要转义特殊字符）
                string escapedPattern = Regex.Escape(pattern);
                if (Regex.IsMatch(input, escapedPattern, RegexOptions.IgnoreCase))                
                    foundPatterns.Add(pattern);                
            }
            return foundPatterns.Count > 0 ? string.Join("; ", foundPatterns) : string.Empty;
        }
        /// <summary>
        /// 过滤SQL注入
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string _sqlClear(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            foreach (string pattern in sqlInjectionPatterns)
                input = Regex.Replace(input, pattern, "", RegexOptions.IgnoreCase);
            return input;
        }
    }
}
