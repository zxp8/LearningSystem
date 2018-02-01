﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Xml;
using System.Collections;
using WeiSha.Common;

using Song.ServiceInterfaces;
using Song.Entities;

namespace Song.Extend.Login
{
    public class Admin
    {
        private static readonly Admin _singleton = new Admin();
        /// <summary>
        /// 获取参数
        /// </summary>
        public static Admin Singleton
        {
            get { return _singleton; }
        }
        #region 属性

        /// <summary>
        /// 后台管理登录的状态管理方式，值为Cookies或Session
        /// </summary>
        public static LoginPatternEnum LoginPattern
        {
            get
            {
                string tm = WeiSha.Common.Login.Get["Admin"].Pattern.String;
                if (tm.Equals("Session", StringComparison.CurrentCultureIgnoreCase)) return LoginPatternEnum.Session;
                if (tm.Equals("Cookies", StringComparison.CurrentCultureIgnoreCase)) return LoginPatternEnum.Cookies;
                return LoginPatternEnum.Cookies;
            }
        }
        private List<Song.Entities.EmpAccount> _onlineUser = new List<EmpAccount>();
        /// <summary>
        /// 在线用户
        /// </summary>
        public List<Song.Entities.EmpAccount> OnlineUser
        {
            get { return _onlineUser; }
        }
        /// <summary>
        /// 返回当前登录用户的实体
        /// </summary>
        /// <returns></returns>
        public Song.Entities.EmpAccount CurrentUser
        {
            get
            {
                Song.Entities.EmpAccount curr = null;
                //首先从在线列表中取当前登录的用户
                if (this._onlineUser.Count > 0)
                {
                    foreach (Song.Entities.EmpAccount em in this.OnlineUser)
                    {
                        if (em == null) continue;
                        if (em.Acc_Id == this.CurrentUserId)
                        {
                            curr = em;
                            break;
                        }
                    }
                }
                //内存中没有的话就从数据库取
                if (curr == null)
                {
                    if (this.CurrentUserId > 0)
                        curr = Business.Do<IEmployee>().GetSingle(this.CurrentUserId);
                    if (curr != null) this._onlineUser.Add(curr);
                    if (curr == null && this.CurrentUserId > 0) this.Logout();
                }
                //如果还是没有，那就是真没有了
                if (curr == null && this.CurrentUserId <= 0)
                {
                    System.Web.HttpContext _context = System.Web.HttpContext.Current;
                    string noLoginPath = WeiSha.Common.Login.Get["Admin"].NoLoginPath.VirtualPath;
                    if (noLoginPath.IndexOf("{") > -1)
                    {
                        Song.Entities.Organization org = Business.Do<IOrganization>().OrganCurrent();
                        noLoginPath = noLoginPath.Replace("{template}", org.Org_Template);
                    }
                    //_context.Response.Redirect(noLoginPath);
                }
                return curr;
            }
        }
        /// <summary>
        /// 当前登录用户的id
        /// </summary>
        public int CurrentUserId
        {
            get
            {
                int accid = 0;
                //登录标识名
                string key = WeiSha.Common.Login.Get["Admin"].KeyName.String;
                string domain = WeiSha.Common.Request.Domain.TwoDomain;
                key = domain + "_" + key;
                if (Admin.LoginPattern == LoginPatternEnum.Cookies)
                    accid = WeiSha.Common.Request.Cookies[key].Int32 ?? 0;
                if (Admin.LoginPattern == LoginPatternEnum.Session)
                    accid = WeiSha.Common.Request.Session[key].Int32 ?? 0;
                return accid;
            }
        }
        /// <summary>
        /// 用户是否登录
        /// </summary>
        public bool IsLogin
        {
            get { return this.CurrentUserId != 0; }
        }
        /// <summary>
        /// 当前登录用户是否为超级管理员
        /// </summary>
        public  bool IsSuperAdmin
        {
            get { return Business.Do<IEmployee>().IsSuperAdmin(this.CurrentUserId); }
        }
        /// <summary>
        /// 是否是根机构的员工
        /// </summary>
        public bool isForRoot
        {
            get
            {
                return Business.Do<IEmployee>().IsForRoot(this.CurrentUserId);
            }
        }
        /// <summary>
        /// 当前登录的账号是不是机构管理员
        /// </summary>
        public bool IsAdmin
        {
            get { return Business.Do<IEmployee>().IsAdmin(this.CurrentUserId); }
        }
        #endregion

        #region 登录与注销的方法
        /// <summary>
        /// 将已经登录入的用户，写入seesion或cookies
        /// </summary>
        /// <param name="acc"></param>
        public void Write(Song.Entities.EmpAccount acc)
        {
            string domain = WeiSha.Common.Request.Domain.TwoDomain;
            Write(acc, domain);
        }
        /// <summary>
        /// 将已经登录入的用户，写入seesion或cookies
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="twoDomain">机构的二级域名，如果没有，则用机构id</param>
        public void Write(Song.Entities.EmpAccount acc, string twoDomain)
        {            
            System.Web.HttpContext _context = System.Web.HttpContext.Current;
            //登录标识名
            string key = WeiSha.Common.Login.Get["Admin"].KeyName.String;  
            key = twoDomain + "_" + key;
            if (Admin.LoginPattern == LoginPatternEnum.Cookies)
            {

                System.Web.HttpCookie cookie = new System.Web.HttpCookie(key);
                cookie.Value = acc.Acc_Id.ToString();
                string exp = WeiSha.Common.Login.Get["Admin"].Expires.String;
                //如果登录有效时间设置为auto，则默认为10分钟
                if (exp.Equals("auto", StringComparison.CurrentCultureIgnoreCase))
                {
                    cookie.Expires = DateTime.Now.AddDays(10);                    
                }
                else
                {
                    cookie.Expires = DateTime.Now.AddMinutes(WeiSha.Common.Login.Get["Admin"].Expires.Int32 ?? 10);
                }
                _context.Response.Cookies.Add(cookie);
            }
            if (Admin.LoginPattern == LoginPatternEnum.Session)
            {
                _context.Session[key] = acc.Acc_Id;
            }
            this._register(acc);
        }
        /// <summary>
        /// 写入当前用户
        /// </summary>
        public void Write()
        {
            //先读取当前用户，再写入
            EmpAccount ea = this.Read();
            this.Write(ea);
        }
        /// <summary>
        /// 获取当前登录用户的对象
        /// </summary>
        /// <returns></returns>
        public Song.Entities.EmpAccount Read()
        {
            int accid = this.CurrentUserId;
            if (accid < 1) return null;

            Song.Entities.EmpAccount acc = this.CurrentUser;
            if (acc == null) acc = Business.Do<IEmployee>().GetSingle(accid);
            return acc;
        }
        /// <summary>
        /// 注册已经登录的在线用户，如果已经注册，则更新注册时间
        /// </summary>
        public  void Register()
        {
            EmpAccount ea = this.Read();
            if (ea != null) _register(ea);
        }
        /// <summary>
        /// 注册某个用户到在线列表中
        /// </summary>
        /// <param name="acc"></param>
        private void _register(Song.Entities.EmpAccount acc)
        {
            if (acc == null) return;
            //登录时间，该时间不入数据库，仅为临时使用
            acc.Acc_LastTime = DateTime.Now;
            //登录用户是否已经存在;
            bool isHav = false;
            for (int i = 0; i < this._onlineUser.Count; i++)
            {
                EmpAccount e = this._onlineUser[i];
                if (e == null) continue;
                if (e.Acc_Id == acc.Acc_Id)
                {
                    this._onlineUser[i] = acc;
                    isHav = true;
                    break;
                }
            }
            //如果未登录，则注册进去
            if (!isHav) this._onlineUser.Add(acc);
        }
        /// <summary>
        /// 注销当前用户
        /// </summary>
        public void Logout()
        {
            int accid = this.CurrentUserId;
            if (accid < 1) return;
            System.Web.HttpContext _context = System.Web.HttpContext.Current;
            //登录标识名
            string key = WeiSha.Common.Login.Get["Admin"].KeyName.String;
            string domain = WeiSha.Common.Request.Domain.TwoDomain;
            key = domain + "_" + key;
            if (Admin.LoginPattern == LoginPatternEnum.Cookies)
                _context.Response.Cookies[key].Expires = DateTime.Now.AddYears(-1);
            if (Admin.LoginPattern == LoginPatternEnum.Session)
                _context.Session.Abandon();
        }        
        /// <summary>
        /// 清理超时用户
        /// </summary>
        public void CleanOut()
        {
            //设置超时时间，单位分钟
            int outTimeNumer = 3;
            List<Song.Entities.EmpAccount> _tm = new List<EmpAccount>();
            foreach (EmpAccount em in this.OnlineUser)
            {
                if (em == null) continue;
                if (DateTime.Now.AddMinutes(-outTimeNumer) > em.Acc_LastTime)
                {
                    _tm.Add(em);
                }
            }
            this._onlineUser = _tm;
        }
        /// <summary>
        /// 验证是否登录，没有登录，则跳转
        /// </summary>
        public void VerifyLogin()
        {
            System.Web.HttpContext _context = System.Web.HttpContext.Current;
            //获取当前文件的文件名
            string path = _context.Request.ServerVariables["PATH_INFO"];
            path = path.Substring(path.LastIndexOf("/") + 1);
            //如果不是首页
            if (path != "index.aspx")
            {
                if (!this.IsLogin)
                {
                    //如果未登录
                    string url = WeiSha.Common.Login.Get["Admin"].NoLoginPath.String ?? "/";
                    _context.Response.Redirect(url);
                }
            }
        }
        #endregion


        #region 权限判断
        /// <summary>
        /// 验证是否拥操作当前页面的权限
        /// </summary>
        public void VerifyPurview()
        {
            //如果是超级管理员，则不受权限控制          
            if (this.IsAdmin) return;
            //如果当前页面所处的模块不受权限控制
            if (!isModuleOfControl()) return;
            //没有权限控制的页面
            if (!isPageOfControl()) return;
            //
            //进行权限判断
            Song.Entities.ManageMenu[] mm = Business.Do<IPurview>().GetAll4Emplyee(CurrentUserId);
            //页面权限标识
            string pagekey = WeiSha.Common.Request.Page.PurviewLabel;
            bool isHave = false;
            foreach (Song.Entities.ManageMenu m in mm)
            {
                if (isHave) break;
                if (m.MM_Link == string.Empty || m.MM_Link == null || m.MM_Link.Trim() == "") continue;
                //权限中记录的文件名标
                string pkey = m.MM_Link.ToLower();
                if (pkey.IndexOf("/") > -1) pkey = pkey.Substring(pkey.LastIndexOf("/") + 1);
                if (pkey.IndexOf("\\") > -1) pkey = pkey.Substring(pkey.LastIndexOf("\\") + 1);
                if (pkey.IndexOf("_") > -1)
                {
                    pkey = pkey.Substring(0, pkey.IndexOf("_"));
                }
                else
                {
                    pkey = pkey.IndexOf(".") > -1 ? pkey.Substring(0, pkey.IndexOf(".")) : pkey;
                }
                if (pkey == pagekey)
                {
                    isHave = true;
                    break;
                }
                if (m.MM_Marker != null && m.MM_Marker != "")
                {
                    foreach (string t in m.MM_Marker.Split(','))
                    {
                        if (pagekey == t.ToLower())
                        {
                            isHave = true;
                            break;
                        }
                    }
                }
            }
            if (!isHave)
            {
                //如果没有权限
                System.Web.HttpContext _context = System.Web.HttpContext.Current;
                _context.Response.Redirect("~/Manage/ErrorPage/NoPurview.aspx");
            }
        }
        /// <summary>
        /// 当前页面是否受控制
        /// </summary>
        /// <returns>如果不受控制，返回fasle</returns>
        private bool isPageOfControl()
        {
            bool isControl = true;
            //没有权限控制的页面
            string noPurview = WeiSha.Common.App.Get["noPurviewPage"].String;
            string manageFileName = WeiSha.Common.Request.Page.ManageFileName;
            foreach (string t in noPurview.Split(','))
            {
                if (t.Trim().ToLower() == manageFileName)
                {
                    isControl = false;
                    break;
                }
            }
            return isControl;
        }
        /// <summary>
        /// 当前页面所处的模块是否受控制
        /// </summary>
        /// <returns>如果不受控制，返回fasle</returns>
        private bool isModuleOfControl()
        {
            bool isControl = true;
            //没有权限控制的模块
            string noPurview = WeiSha.Common.App.Get["noPurviewModule"].String;
            string moduleName = WeiSha.Common.Request.Page.Module;
            foreach (string t in noPurview.Split(','))
            {
                if (t.Trim().ToLower() == moduleName)
                {
                    isControl = false;
                    break;
                }
            }
            return isControl;
        }
        #endregion
    }
}
