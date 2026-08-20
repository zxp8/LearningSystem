using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Net;
using System.IO;
using WeiSha.Core;
using Newtonsoft.Json.Linq;

namespace Song.SMS.Object
{
    /// <summary>
    /// 河南腾信的短信开发接口
    /// </summary>
    public class HenanTengxin2 : ISMS
    {
        //private string apipath = "http://api.sms1086.com/Api/";

        private static readonly sms1086.WsAPIs ObjWsAPIs = new sms1086.WsAPIs();
        private SmsItem _current;
        public SmsItem Current
        {
            get { return _current; }
            set { _current = value; }
        }
        /// <summary>
        /// 用户的账号
        /// </summary>
        public string User
        {
            get { return _current.User; }
            set { _current.User = value; }
        }
        /// <summary>
        /// 用户的密码
        /// </summary>
        public string Password
        {
            get { return _current.Password; }
            set { _current.Password = value; }
        }
        #region ISMS 成员

        public SmsState Send(string mobiles, string context)
        {
            return Send(mobiles, context, DateTime.Now);
        }

        public SmsState Send(string mobiles, string content, DateTime time)
        {
            //网址
            string url = Current.Domain + "api/sendMessageOne";
            JObject jo = new JObject();
            long timestamp = time.TimeStamp();
            jo.Add("userName", Current.User);
            jo.Add("timestamp", timestamp);
            JArray array = new JArray();
            foreach (string mobile in mobiles.Split(','))
            {
                if (string.IsNullOrWhiteSpace(mobile)) continue;
                JObject o = new JObject();
                o.Add("phone", mobile);             
                o.Add("content", content);
                array.Add(o);
            }
            jo.Add("messageList", array);
            //MD5(userName + timestamp + MD5(password))          
            string sign = Current.User + timestamp + DataConvert.MD5(Current.Password);
            jo.Add("sign", DataConvert.MD5(sign));

            //
            string json = jo.ToString();          
            string result = HttpPost(url, json);
            JObject rjson = JObject.Parse(result);
            int code = rjson["code"].ToString().Convert<int>();
            if (code != 0) throw new Exception(rjson["message"].ToString());

            //发送状态
            SmsState state = new SmsState();
            state.Code = code;
            state.Success = state.Code == 0;
            state.Result = rjson["message"].ToString();
           
            return state;            
        }
        /// <summary>
        /// 查询剩余的短信条数
        /// </summary>
        /// <returns></returns>
        public int Query()
        {
            //短信帐号与密码
            string account = Current.User;
            string pw = Current.Password;
            return Query(account, pw);
        }
        /// <summary>
        /// 查询剩余的短信条数
        /// </summary>
        /// <param name="user">账号</param>
        /// <param name="pw">密码</param>
        /// <returns></returns>
        public int Query(string user, string pw)
        {
            //网址
            string url = Current.Domain + "api/getBalance";
            JObject jo = new JObject();
            long timestamp = DateTime.Now.TimeStamp();
            jo.Add("userName", user);
            jo.Add("timestamp", timestamp);
            //MD5(userName + timestamp + MD5(password))          
            string sign = user + timestamp + DataConvert.MD5(pw);
            jo.Add("sign", DataConvert.MD5(sign));

            //
            string json = jo.ToString();
            string result = HttpPost(url, json);
            JObject rjson = JObject.Parse(result);
            int code = rjson["code"].ToString().Convert<int>();
            if (code == 0) return int.Parse(rjson["balance"].ToString());
            else throw new Exception(rjson["message"].ToString());
        }
        
        public string ReceiveSMS(DateTime from, string readflag)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Post方式获取网页的返回结果
        /// </summary>
        /// <param name="url">网址</param>
        /// <param name="json">json格式参数</param>
        /// <returns></returns>
        private static string HttpPost(string url, string json)
        {
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.Accept = "application/json";
            req.ContentType = "application/json;charset=utf-8";
            if (!string.IsNullOrWhiteSpace(json))
            {
                byte[] utf8Bytes = Encoding.UTF8.GetBytes(json);
                req.ContentLength = utf8Bytes.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(utf8Bytes, 0, utf8Bytes.Length);
                }
            }
            string result = string.Empty;
            using (HttpWebResponse hwr = req.GetResponse() as HttpWebResponse)
            {
                System.IO.StreamReader reader = new System.IO.StreamReader(hwr.GetResponseStream(), Encoding.UTF8);
                result = reader.ReadToEnd();
            }         
            return result;
        }
        #endregion
    }
}
