using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Web;
using System.Security.Cryptography;
using System.Data.HashFunction;
using System.Data.HashFunction.FNV;
using System.Data.HashFunction;

namespace Song.ViewData.Helper
{
    /// <summary>
    /// 将某个值转换为任意数据类型
    /// </summary>
    public class ConvertToAnyValue
    {
        private object _paravlue;
        /// <summary>
        /// 参数的原始值
        /// </summary>
        public string ParaValue
        {
            get
            {
                return _paravlue.ToString();
            }
            set
            {
                _paravlue = value;
            }
        }
        private string _parakey = "";
        /// <summary>
        /// 参数的键名
        /// </summary>
        public string ParaKey
        {
            get
            {
                return _parakey;
            }
            set
            {
                _parakey = value;
            }
        }
        private string _unit = "";
        /// <summary>
        /// 参数的单位
        /// </summary>
        public string Unit
        {
            get { return _unit; }
            set { _unit = value; }
        }
        public ConvertToAnyValue()
        {
        }
        public ConvertToAnyValue(object para)
        {
            this._paravlue = para;
        }
        public ConvertToAnyValue(string para)
        {
            this._paravlue = string.IsNullOrWhiteSpace(para) ? "" : para;
        }
        public ConvertToAnyValue(string para, string unit)
        {
            this._paravlue = para;
        }
        public static ConvertToAnyValue Create(string para)
        {
            return new ConvertToAnyValue(para);
        }
        /// <summary>
        /// 参数的Int16类型值，如果参数不存在或异常，则返回null;
        /// </summary>
        public int? Int16
        {
            get
            {
                if (_paravlue == null) return null;
                try
                {
                    return System.Convert.ToInt16(_paravlue);
                }
                catch
                {
                    Regex r = new Regex("\\d+");
                    Match ms = r.Match(this.String);
                    if (ms.Success)
                    {
                        string tm = ms.Groups[0].Value;
                        return System.Convert.ToInt16(tm);
                    }
                    return null;
                }
            }
        }
        /// <summary>
        /// 参数的Int32类型值，如果参数不存在或异常，则返回null;
        /// </summary>
        public int? Int32
        {
            get
            {
                if (_paravlue == null) return null;
                try
                {
                    return System.Convert.ToInt32(_paravlue);
                }
                catch
                {
                    Regex r = new Regex("\\d+");
                    Match ms = r.Match(this.String);
                    if (ms.Success)
                    {
                        string tm= ms.Groups[0].Value;
                        return System.Convert.ToInt32(tm);
                    }
                    return null;
                }
            }
        }
        /// <summary>
        /// 参数的Int64类型值，如果参数不存在或异常，则返回null;
        /// </summary>
        public long? Int64
        {
            get
            {
                if (_paravlue == null) return null;
                try
                {
                    return System.Convert.ToInt64(_paravlue);
                }
                catch
                {
                    Regex r = new Regex("\\d+");
                    Match ms = r.Match(this.String);
                    if (ms.Success)
                    {
                        string tm = ms.Groups[0].Value;
                        return System.Convert.ToInt64(tm);
                    }
                    return null;
                }
            }
        }
        /// <summary>
        /// 参数的Double类型值，如果参数不存在或异常，则返回null;
        /// </summary>
        public double? Double
        {
            get
            {
                if (_paravlue == null) return null;               
                try
                {
                    return System.Convert.ToDouble(_paravlue);
                }
                catch
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 参数的String类型值，如果参数不存在或异常，则返回空字符串，非Null;
        /// </summary>
        public string String => _paravlue == null ? string.Empty : _paravlue.ToString().Trim();
        /// <summary>
        /// 参数文本类型值，自动去除html标签
        /// </summary>
        public string Text
        {
            get
            {
                string text = this.String;
                if (string.IsNullOrWhiteSpace(text)) return text;
                string strText = System.Text.RegularExpressions.Regex.Replace(text, "<[^>]+>", "");
                strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");
                return text;
            }
        }
        /// <summary>
        /// 参数的值,如果没有内容，返回Null
        /// </summary>
        public string Value
        {
            get
            {
                string text = this.String;
                if (string.IsNullOrWhiteSpace(text)) return null;
                return text;
            }
        }
        /// <summary>
        /// 参数的Boolean类型值，如果参数不存在或异常，则返回true;
        /// </summary>
        public bool? Boolean
        {
            get
            {
                if (_paravlue == null) return null;
                string text = _paravlue.ToString();
                if (string.IsNullOrWhiteSpace(text)) return null;
                if ("false".Equals(text, StringComparison.CurrentCultureIgnoreCase)) return false;
                if ("true".Equals(text, StringComparison.CurrentCultureIgnoreCase)) return true;
                ////
                //bool.TryParse(text, out bool result);
                //return result;
                try
                {
                    return System.Convert.ToBoolean(_paravlue);
                }
                catch
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 参数的DateTime类型值，如果参数不存在或异常，则返回null;
        /// </summary>
        public DateTime? DateTime
        {
            get
            {
                if (_paravlue == null) return null;
                try
                {
                    if (_paravlue is long)
                    {
                        long jsTimeStamp = 0;
                        long.TryParse(_paravlue.ToString(), out jsTimeStamp);
                        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                        DateTime dt = startTime.AddMilliseconds(jsTimeStamp);
                        return dt;
                    }
                    else
                    {
                        return System.Convert.ToDateTime(_paravlue);
                    }
                }
                catch
                {
                    string t = _paravlue.ToString();
                    //将非数字字符全部换成-
                    foreach (char c in t)
                        t += c >= 48 && c <= 57 ? c : '-';
                    string str = t;
                    //如果有多个-连惯，则换成一个
                    Regex re = new Regex(@"\-{1,}", RegexOptions.IgnorePatternWhitespace);
                    str = re.Replace(str, "-");
                    //如果前后有-，则去除
                    re = new Regex(@"^\-{1,}(\w)", RegexOptions.IgnorePatternWhitespace);
                    str = re.Replace(str, "$1");
                    re = new Regex(@"(\w)\-{1,}$", RegexOptions.IgnorePatternWhitespace);
                    str = re.Replace(str, "$1");
                    //如果年份为两位数，则补足四位，多于四位则取前四位
                    string year;
                    if (str.IndexOf('-') > -1)
                    {
                        year = str.Substring(0, str.IndexOf('-'));
                        year = year.Length == 1 ? "0" + year : year;
                        year = year.Length == 2 ? "19" + year : year;
                        year = year.Length > 4 ? year.Substring(0, 4) :
                        year; str = year + "-" + str.Substring(str.IndexOf('-') + 1);
                    }
                    else
                    {
                        str += "-1-1";
                    }
                    try
                    {
                        return System.Convert.ToDateTime(str);
                    }
                    catch
                    {
                        return null;
                    }
                }
                
            }
        }
        /// <summary>
        /// 一个速度很快的哈希算法
        /// </summary>
        public string FNV1a
        {
            get
            {
                if (_paravlue == null) return string.Empty;
                IFNV1a fnv1a = FNV1aFactory.Instance.Create();        
                IHashValue hashValue = fnv1a.ComputeHash(Encoding.UTF8.GetBytes(_paravlue.ToString()));
                return hashValue.AsHexString();
            }
        }
        /// <summary>
        /// 参数的MD5加密值(小写)，如果参数不存在或异常，则返回null;
        /// </summary>
        public string MD5
        {
            get
            {
                if (_paravlue == null) return "";
                if (string.IsNullOrWhiteSpace(_paravlue.ToString())) return "";
                //MD5加密
                System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(_paravlue.ToString()));
                string pwd = string.Empty;
                for (int i = 0; i < s.Length; i++)
                    pwd = pwd + s[i].ToString("x2");
                return pwd;
            }
        }
        /// <summary>
        /// SHA1加密值，FIPS 180-4标准，产生160位的哈希值
        /// </summary>
        public string SHA1
        {
            get
            {
                if (_paravlue == null) return null;
                string result = string.Empty;
                using (System.Security.Cryptography.SHA1 sha1 = new SHA1CryptoServiceProvider())
                {
                    byte[] bytes_in = System.Text.Encoding.UTF8.GetBytes(_paravlue.ToString());
                    byte[] bytes_out = sha1.ComputeHash(bytes_in);                    
                    result = BitConverter.ToString(bytes_out);//将运算结果转为string类型                    
                    sha1.Dispose();
                }
                return string.IsNullOrWhiteSpace(result) ? null : result.Replace("-", "");
            }
        }
        /// <summary>
        /// SHA2加密值，SHA-2（Secure Hash Algorithm 2）：包括SHA-224、SHA-256、SHA-384和SHA-512等，FIPS 180-4标准。
        /// </summary>
        public string SHA256
        {
            get
            {
                if (_paravlue == null) return null;
                string str = _paravlue.ToString();
                if(string.IsNullOrWhiteSpace(str)) return null;
                string shastring = string.Empty;
                // 创建 SHA-256 哈希算法实例
                using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
                {
                    // 将数据转换为字节数组
                    byte[] dataBytes = Encoding.UTF8.GetBytes(str);
                    // 执行哈希计算
                    byte[] hashBytes = sha256.ComputeHash(dataBytes);
                    // 将哈希结果转换为字符串或其他格式
                    shastring = BitConverter.ToString(hashBytes).Replace("-", "");                   
                }
                return shastring;
            }
        }
        /// <summary>
        /// 参数的字符串进行 URL 解码并返回已解码的字符串。如果参数不存在或异常，则返回null;
        /// </summary>
        public string UrlDecode
        {
            get
            {
                if (_paravlue == null) return null;
                return System.Web.HttpUtility.UrlDecode(_paravlue.ToString().Trim());
            }
        }
        public string UrlEncode
        {
            get
            {
                if (_paravlue == null) return null;
                return System.Web.HttpUtility.UrlEncode(_paravlue.ToString().Trim());
            }
        }
        /// <summary>
        /// 对经过HTML 编码的参数进行解码，并返回已解码的字符串。如果参数不存在或异常，则返回null;
        /// </summary>
        public string HtmlDecode
        {
            get
            {
                if (_paravlue == null) return null;
                System.Web.HttpContext _context = System.Web.HttpContext.Current;
                return _context.Server.HtmlDecode(_paravlue.ToString());
            }
        }
        /// <summary>
        /// 转换为物理路径
        /// </summary>
        public string MapPath
        {
            get
            {
                System.Web.HttpContext _context = System.Web.HttpContext.Current;
                return _context.Server.MapPath(_paravlue.ToString());
            }
        }
        /// <summary>
        /// 转换虚拟路径
        /// </summary>
        public string VirtualPath
        {
            get
            {
                string path = _paravlue.ToString();
                path = path.Replace("\\", "/");
                path = path.Replace("~/", System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
                //在路径末尾加上\\
                if (path.IndexOf("/") > -1)
                {
                    if (path.Substring(path.LastIndexOf("/")).IndexOf(".") < 0)
                        if (path.Substring(path.Length - 1) != "/") path += "/";
                }
                path = Regex.Replace(path, @"\/+", "/");
                return path;
                
            }
        }
        /// <summary>
        /// 将字符串分拆成数组
        /// </summary>
        /// <param name="split">用于分拆的字符</param>
        /// <returns></returns>
        public string[] Split(char split)
        {
            return _paravlue.ToString().Split(split);
        }
        /// <summary>
        /// 将C#时间转换成Javascript的时间
        /// </summary>
        [Obsolete] 
        public string JavascriptTime
        {
            get
            {
                if (_paravlue == null) return "";
                try
                {
                    System.DateTime time = this.DateTime ?? TimeZone.CurrentTimeZone.ToLocalTime(System.DateTime.Now);
                    string fmtDate = "ddd MMM d HH:mm:ss 'UTC'zz'00' yyyy";
                    CultureInfo ciDate = CultureInfo.CreateSpecificCulture("en-US");
                    //将C#时间转换成JS时间字符串    
                    string JSstring = time.ToString(fmtDate, ciDate);
                    return JSstring;
                }
                catch
                {
                    return "";
                }

            }
        }
        /// <summary>
        /// JavaScript时间戳：是指格林威治时间1970年01月01日00时00分00秒(北京时间1970年01月01日08时00分00秒)起至现在的总毫秒数。
        /// </summary>
        public long TimeStamp
        {
            get
            {
                System.DateTime time = this.DateTime ?? System.DateTime.Now;
                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
                long timeStamp = (long)(time - startTime).TotalMilliseconds; // 相差毫秒数
                return timeStamp;
            }
        }
        /// <summary>
        /// 转为指定的数据库类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object ChangeType(System.Type type)
        {
            object obj;
            switch (type.FullName)
            {
                case "System.DateTime":
                    obj = this.DateTime;
                    break;
                default:
                    obj = System.Convert.ChangeType(_paravlue, type);
                    break;
            }
            return obj;
        }

        /// <summary>
        /// 解密字符串,默认密钥为当前域名
        /// </summary>
        /// <returns></returns>
        public ConvertToAnyValue Decrypt()
        {
            string ret = string.Empty;
            ret = WeiSha.Core.DataConvert.DecryptForBase64(this.UrlDecode);
            return new ConvertToAnyValue(ret);
        }
    }
}
