using ServiceStack.Common;
using Song.Entities;
using Song.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using WeiSha.Core;

namespace Song.ViewData.Helper
{
    /// <summary>
    /// 页面访问校验
    /// </summary>
    public class PageCheck
    {

        private static string configPath = "/Utilities/PageCheck";
        private static string configFile = "Config.weisha";
        //       
        private static readonly PageCheck _instance = new PageCheck();
        /// <summary>
        /// 单例对象
        /// </summary>
        public static PageCheck Instance => _instance;

        #region 不受权限控制的内容，页面或模板项
        private static readonly object _uncontrolled_items_lock = new object();
        private Dictionary<string, HashSet<string>> _uncontrolledItems = null;
        /// <summary>
        /// 不受限制的页面项，key值为模板库名称，value为页面列表
        /// </summary>
        /// <remarks></remarks>
        public Dictionary<string, HashSet<string>> UncontrolledItems
        {
            get
            {
                if (_uncontrolledItems != null) return _uncontrolledItems;
                lock (_uncontrolled_items_lock)
                {
                    //先取从缓存中读取
                    System.Web.Caching.Cache cache = System.Web.HttpRuntime.Cache;
                    object cachevalue = cache.Get(configFile + "_UncontrolledItems");
                    if (cachevalue != null)
                    {
                        _uncontrolledItems = (Dictionary<string, HashSet<string>>)cachevalue;
                        return _uncontrolledItems;
                    }
                    //如果缓存中没有，则从配置文件读取，并写入缓存
                    var dic = new Dictionary<string, HashSet<string>>();
                    if (!File.Exists(Path.Combine(configPath, configFile))) return dic;

                    var xmldoc = new XmlDocument();
                    xmldoc.Load(configPath + configFile);
                    XmlNodeList nodes = xmldoc.LastChild.FirstChild.ChildNodes;
                    for (int i = 0; i < nodes.Count; i++)
                    {
                        XmlNode node = nodes[i];
                        string key = node.Name;
                        HashSet<string> list = new HashSet<string>();
                        //不需要权限管理的页面
                        XmlNodeList childs = node.ChildNodes;
                        for (int j = 0; j < childs.Count; j++)
                        {
                            string page = childs[j].Attributes["value"].Value;
                            if (!string.IsNullOrWhiteSpace(page))
                            {
                                if (page.EndsWith("/") || page.EndsWith("\\")) page = page.Substring(0, page.Length - 1);
                                list.Add(page.ToLower());
                            }
                        }
                        dic.Add(key, list);
                    }
                    //加入缓存                   
                    cache.Insert(configFile + "_UncontrolledItems", dic, new System.Web.Caching.CacheDependency(configPath + configFile));
                    _uncontrolledItems = dic;
                    return _uncontrolledItems;
                }                
            }
        }
        /// <summary>
        /// 判断页而是否是不受控制的页面
        /// </summary>
        /// <param name="device">模板库名称</param>
        /// <param name="page">页面</param>
        /// <returns></returns>
        public bool UncontrolledItemCheck(string device,string page)
        {
            Dictionary<string, HashSet<string>> items = this.UncontrolledItems;
            if (items == null) return true;
            HashSet<string> hashset = this.UncontrolledItems.FirstOrDefault(x => x.Key.Equals(device, StringComparison.OrdinalIgnoreCase)).Value;

            //HashSet<string> hashset = value;
            if (hashset == null || hashset.Contains(page)) return true;
            return false;
        }

        // 不受限制的页面项
        private static readonly object _uncontrolled_pages_lock = new object();
        private List<string> _uncontrolledPages = null;
        /// <summary>
        /// 不受限制的页面项，支持正则表达式
        /// </summary>
        public List<string> UncontrolledPages
        {
            get
            {
                if (_uncontrolledPages != null) return _uncontrolledPages;
                lock (_uncontrolled_pages_lock)
                {
                    //先取从缓存中读取
                    System.Web.Caching.Cache cache = System.Web.HttpRuntime.Cache;
                    object cachevalue = cache.Get(configFile + "_UncontrolledPages");
                    if (cachevalue != null)
                    {
                        _uncontrolledPages = (List<string>)cachevalue;
                        return _uncontrolledPages;
                    }
                    //如果缓存中没有，则读取配置文件
                    var list = new List<string>();
                    if (!File.Exists(configPath + configFile)) return list;
                    var xmldoc = new XmlDocument();
                    xmldoc.Load(configPath + configFile);
                    var nodes = xmldoc.SelectNodes("config/allow/*[@value]");
                    if (nodes == null) return list;
                    list.AddRange(                        from XmlNode node in nodes
                        where node.NodeType == XmlNodeType.Element
                        select node.Attributes["value"].Value
                    );
                    cache.Insert(configFile + "_UncontrolledPages", list, new System.Web.Caching.CacheDependency(configPath + configFile));
                    _uncontrolledPages = list;
                    return _uncontrolledPages;
                }              
            }
        }
        /// <summary>
        /// 检测页面是否在不受控制的页面项中
        /// </summary>
        /// <param name="page">页面</param>
        /// <returns></returns>
        public bool UncontrolledPageCheck(string page)
        {
            //模板库之外的不限制页面，正则表达式匹配
            foreach (string s in this.UncontrolledPages)
                if (Regex.IsMatch(page, s, RegexOptions.IgnoreCase)) return true;
            return false;
        }
        #endregion
        private PageCheck()
        {
            configPath = WeiSha.Core.Server.MapPath(configPath);
            Business.Do<IManageMenu>().OnChanged += (object sender, EventArgs e) => InitializedMenu();
            Business.Do<IPurview>().OnChanged += (object sender, EventArgs e) => InitializedMenu();           
        }   


        /// <summary>
        /// 菜单项，第一层key值是角色名称，第二层key值是机构id，HashSet为菜单项的link
        /// </summary>
        private static Dictionary<string ,Dictionary<int, HashSet<string>>> _menu_hashset = null;               
        private static object lockObj = new object();
        /// <summary>
        /// 初始化权限菜单项
        /// </summary>
        public static void InitializedMenu()
        {
            lock (lockObj)
            {
                var menupages = new Dictionary<string, Dictionary<int, HashSet<string>>>();
                try
                {
                    //管理员按岗位划分权限
                    List<Position> positions = Business.Do<IPosition>().GetAll(0);
                    Dictionary<int, HashSet<string>> dic = new Dictionary<int, HashSet<string>>();
                    foreach (Position posi in positions)
                    {
                        List<ManageMenu> mms = Business.Do<IPurview>().PosiPurviewMenu(posi);
                        if (mms == null || mms.Count == 0) continue;
                        HashSet<string> hset = new HashSet<string>();
                        for (int j = 0; j < mms.Count; j++)
                        {
                            if (string.IsNullOrWhiteSpace(mms[j].MM_Link) || mms[j].MM_Link.StartsWith("http"))
                                continue;
                            hset.Add(mms[j].MM_Link?.ToLower());
                        }
                        dic.Add(posi.Posi_Id, hset);
                    }
                    menupages.Add("organAdmin", dic);
                }
                catch (Exception ex)
                {
                    WeiSha.Core.Log.Error("管理权限", ex);
                    throw ex;
                }

                //学员与教师的权限菜单              
                string[] keys = { "student", "teacher" };
                List<Organization> orgs = Business.Do<IOrganization>().OrganAll(null, -1, string.Empty);
                //取各角色、各机构的菜单项
                for (int i = 0; i < keys.Length - 1; i++)
                {
                    Dictionary<int, HashSet<string>> dic2 = new Dictionary<int, HashSet<string>>();
                    foreach (Organization org in orgs)
                    {
                        List<ManageMenu> mms = Business.Do<IPurview>().OrganPurviewMenu(org, keys[i]);
                        if (mms == null || mms.Count == 0) continue;
                        HashSet<string> hset2 = new HashSet<string>();
                        for (int j = 0; j < mms.Count; j++)
                        {
                            if (string.IsNullOrWhiteSpace(mms[j].MM_Link) || mms[j].MM_Link.StartsWith("http"))
                                continue;
                            hset2.Add(mms[j].MM_Link?.ToLower());
                        }
                        dic2.Add(org.Org_ID, hset2);
                    }
                    menupages.Add(keys[i], dic2);
                }

                Dictionary<int, HashSet<string>> dic3 = new Dictionary<int, HashSet<string>>();
                HashSet<string> hset3 = new HashSet<string>();
                //系统菜单
                foreach (ManageMenu m in Business.Do<IManageMenu>().GetAll(true, true, "sys"))
                {
                    if (string.IsNullOrWhiteSpace(m.MM_Link) || m.MM_Link.StartsWith("http")) continue;
                    hset3.Add(m.MM_Link?.ToLower());
                }
                dic3.Add(0, hset3);
                menupages.Add("manage", dic3);
                //
                _menu_hashset = menupages;
            }
        }
        /// <summary>
        /// 某一类角色的菜单项，包括各个机构的
        /// </summary>
        /// <param name="role">角色名称</param>
        /// <returns></returns>
        public Dictionary<int, HashSet<string>> Menus(string role)
        {
            if (_menu_hashset == null)
            {
                lock (this) InitializedMenu();
                    
            }
            if (role.Equals("orgadmin")) return _menu_hashset.ContainsKey("organAdmin") ? _menu_hashset["organAdmin"] : null;
            foreach (string str in _menu_hashset.Keys)          
                if(String.Equals(str, role, StringComparison.OrdinalIgnoreCase))                      
                    return _menu_hashset[str];           
            return null;          
        }
        /// <summary>
        /// 某一角色在所在机构的菜单项
        /// </summary>
        /// <param name="role">角色名称,例如organAdmin，sutdent,teacher</param>
        /// <param name="keyval">岗位id，或机构id</param>
        /// <returns></returns>
        public HashSet<string> Menus(string role,int keyval)
        {
            Dictionary<int, HashSet<string>> dic = this.Menus(role);
            if (dic == null) return null;
            return dic.ContainsKey(keyval) ? dic[keyval] : null;
        }
        #region 校验
        /// <summary>
        /// 检测API请求的所在页面是否拥有权限
        /// </summary>
        /// <param name="letter"></param>
        /// <returns></returns>
        public bool CheckPageAccess(Letter letter)
        {
            string pagepath = letter.WEB_PAGE;
            if (string.IsNullOrWhiteSpace(pagepath)) return false;
            if (pagepath == "/") return true;
            string[] pages = pagepath.Split(',');
            if (pages.Length < 1) return false;
            for (int i = 0; i < pages.Length; i++)
                if (pages[i].EndsWith("/")) pages[i] = pages[i].Substring(0, pages[i].Length - 1);
            string root = pages[pages.Length - 1];  //根页面，一般是管理后台的地址，如果不处理管理框架内，则是自身
            string self = pages[0];     //自身页面
            string page = pages.Length >= 2 ? pages[pages.Length - 2] : root;    //需要判断权限的页面
            //页面所在控制器路由，在当前系统框架中，往往用于描述为“设备(device)”
            string controller = root.Split('/').FirstOrDefault(s => !string.IsNullOrWhiteSpace(s)); //模板库的类型

            //判断是否拥有权限
            bool ispass = this.CheckPageAccess(page, controller.ToLower(), letter);           
           
            if (!ispass)
            {
                string msg = string.Format("当前页面 {0} 没有操作权限，请确认管理权限，或是否登录或登录失效！", self);
                throw VExcept.Verify(msg, 100);
            }
            return true;
        }
        /// <summary>
        /// 机构下某一类marker标识的菜单项,如果在机构等级中设置了权限，则返回该权限的菜单项；
        /// </summary>
        /// <param name="orgid"></param>
        /// <param name="marker">例如教师管理teacher,学生管理student,机构管理organAdmin</param>
        /// <returns></returns>
        public List<Song.Entities.ManageMenu> OrganMenus(int orgid,string marker)
        {
            List<Song.Entities.ManageMenu> mms = null;
            if (!string.IsNullOrWhiteSpace(marker))
            {
                Song.Entities.Organization org = Business.Do<IOrganization>().OrganSingle(orgid);
                mms = Business.Do<IPurview>().OrganPurviewMenu(org, marker);
            }              
            return mms;
        }
        /// <summary>
        /// 检测页面是否拥有权限
        /// </summary>
        /// <param name="page">用于权限判断的页面</param>
        /// <param name="device">页面所在控制器路由，在当前系统框架中，往往用于描述为“设备(device)”</param>
        /// <param name="letter"></param>
        /// <returns></returns>
        public bool CheckPageAccess(string page, string device, Letter letter)
        {
            //如果不是模板库限制的页面
            if (this.UncontrolledItemCheck(device, page)) return true;
            //模板库之外的不限制页面，正则表达式匹配
            if (this.UncontrolledPageCheck(page)) return true;

            //
            HashSet<string> menus = null; 
            int keyval = 0; //岗位id，或机构id，如果是管理员时，取岗位id
            //根据登录状态判断权限
            switch (device)
            {
                case "orgadmin":
                    //当前机构管理员
                    EmpAccount emp = LoginAdmin.Status.User(letter);
                    if (emp != null) keyval = emp.Posi_Id;
                    break;
                case "student":
                    Accounts acc = LoginAccount.Status.User(letter);
                    if (acc != null) keyval = acc.Org_ID;
                    break;
                case "teacher":
                    Teacher th = LoginAccount.Status.Teacher(letter);
                    if (th != null) keyval = th.Org_ID;
                    break;
                case "manage":
                    keyval = 0;
                    break;
                default:
                    return true;
            }
            menus = Menus(device, keyval);
            if (menus == null) return false;
            if( menus.Contains(page))return true;
            
            return false;
        }
        #endregion
    }
}
