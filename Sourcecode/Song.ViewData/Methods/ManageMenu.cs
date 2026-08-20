#define RELEASE
#define DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Song.Entities;
using Song.ServiceInterfaces;
using Song.ViewData.Attri;
using WeiSha.Core;

namespace Song.ViewData.Methods
{
    /// <summary>
    /// 系统管理菜单的管理，包括后台管理、学员、教师等的菜单都在这里管理
    /// </summary>
    [HttpPut, HttpGet]
    public class ManageMenu : ViewMethod, IViewAPI
    {
        #region 根菜单
        /// <summary>
        /// 根菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet, SuperAdmin]
        public Song.Entities.ManageMenu[] Root()
        {
            return Business.Do<IManageMenu>().GetRoot("func");
        }
        /// <summary>
        /// 所有启用的根菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Song.Entities.ManageMenu[] EnableRoot()
        {
            return Business.Do<IManageMenu>().GetRoot("func", true);
        }
        /// <summary>
        /// 更改根菜单的排序
        /// </summary>
        /// <param name="items">根菜单的数组</param>
        /// <returns></returns>
        [HttpPost]
        [SuperAdmin]
        public bool ModifyTaxis(Song.Entities.ManageMenu[] items)
        {
            try
            {
                Business.Do<IManageMenu>().UpdateTaxis(items);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 功能菜单的管理
        /// <summary>
        /// 获取功能菜单的对象信息
        /// </summary>
        /// <param name="id">菜单id</param>
        /// <returns></returns>
        [HttpGet]
        public Song.Entities.ManageMenu ForID(int id)
        {
            return Business.Do<IManageMenu>().GetSingle(id);
        }
        /// <summary>
        /// 通过uid获取菜单对象
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public Song.Entities.ManageMenu ForUID(string uid)
        {
            return Business.Do<IManageMenu>().GetSingle(uid);
        }
        /// <summary>
        /// 修改菜单对象
        /// </summary>
        /// <param name="mm">菜单对象</param>
        /// <returns></returns>
        [HttpPost, SuperAdmin]
        public bool Modify(Song.Entities.ManageMenu mm)
        {
            Song.Entities.ManageMenu old = Business.Do<IManageMenu>().GetSingle(mm.MM_Id);
            if (old == null) throw new Exception("对象不存在！");
            old.Copy<Song.Entities.ManageMenu>(mm);
            Business.Do<IManageMenu>().Save(old);
            return true;
        }
        /// <summary>
        /// 修改根菜单
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        [HttpPost, SuperAdmin]
        public bool ModifyFuncRoot(Song.Entities.ManageMenu mm)
        {
            Song.Entities.ManageMenu old = Business.Do<IManageMenu>().GetSingle(mm.MM_Id);
            if (old == null) throw new Exception("对象不存在！");
            old.Copy<Song.Entities.ManageMenu>(mm, "MM_Marker");
            Business.Do<IManageMenu>().RootSave(old);
            return true;
        }
        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id">可以是多个，用逗号分隔</param>
        /// <returns>返回删除的个数</returns>
        [HttpDelete, SuperAdmin]
        public int Delete(string id)
        {
            int i = 0;
            if (string.IsNullOrWhiteSpace(id)) return i;
            List<int> list = id.ToList<int>();
            foreach (int s in list)
                i += Business.Do<IManageMenu>().Delete(s);
            return i;
        }
        /// <summary>
        /// 添加功能菜单的根菜单
        /// </summary>
        /// <param name="mm">菜单对象</param>
        /// <returns></returns>
        [HttpPost, SuperAdmin]
        public bool AddFuncRoot(Song.Entities.ManageMenu mm)
        {
            try
            {
                mm.MM_PatId = "0";
                mm.MM_Func = "func";
                Business.Do<IManageMenu>().RootAdd(mm);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 将功能菜单，从一个根菜单移到另一个根菜单
        /// </summary>
        /// <param name="cuid">当前菜单的uid</param>
        /// <param name="puid">要移动到的菜单的uid</param>
        /// <returns></returns>
        [SuperAdmin]
        [HttpPost]
        public bool FuncMoveRoot(string cuid, string puid)
        {
            Song.Entities.ManageMenu mm = Business.Do<IManageMenu>().GetSingle(cuid);
            if (mm == null) return false;
            mm.MM_PatId = puid;
            try
            {
                Business.Do<IManageMenu>().Save(mm);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 更改菜单项的完成度，用于开发过程中方便查看工作进度，实际应用中用不到
        /// </summary>
        /// <param name="uid">菜单项的uid</param>
        /// <param name="complete">完成度</param>
        /// <returns></returns>
        [SuperAdmin]
        [HttpPost]
        public bool UpdateComplete(string uid, int complete)
        {
            Song.Entities.ManageMenu mm = Business.Do<IManageMenu>().GetSingle(uid);
            if (mm == null) return false;
            mm.MM_Complete = complete;
            try
            {
                Business.Do<IManageMenu>().Save(mm);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// 获取功能菜单的树
        /// </summary>
        /// <param name="uid">功能菜单根节点的uid</param>
        /// <returns></returns>
        [HttpGet]
        [SuperAdmin]
        public JArray FuncMenu(string uid)
        {
            List<Song.Entities.ManageMenu> mm = Business.Do<IManageMenu>().GetFunctionMenu(uid, null, null);
            if (mm.Count > 0)
            {
                Song.Entities.ManageMenu root = Business.Do<IManageMenu>().GetSingle(uid);
                JArray ja = _MenuNode(root, mm, false);
                return ja;
            }
            return null;
        }


        #region 系统菜单
        /// <summary>
        /// 系统菜单，即超级管理左上角菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [SuperAdmin]
        public JArray SystemMenu()
        {
            List<Song.Entities.ManageMenu> mm = Business.Do<IManageMenu>().GetAll(null, null, "sys");
            return mm.Count > 0 ? _MenuNode(null, mm, false) : null;
        }
        /// <summary>
        /// 显示系统菜单项
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Cache]
        public JArray SystemMenuShow()
        {
            List<Song.Entities.ManageMenu> mm = Business.Do<IManageMenu>().GetAll(true, true, "sys");
            return mm.Count > 0 ? _MenuNode(null, mm, true) : null;
        }
        /// <summary>
        /// 生成菜单子节点
        /// </summary>
        /// <param name="item">当前菜单项</param>
        /// <param name="items">所有菜单项</param>
        /// <param name="simplify">简化的，如果为true则去除一些字段，false取全部字段</param>
        /// <returns></returns>
        private JArray _MenuNode(Song.Entities.ManageMenu item, List<Song.Entities.ManageMenu> items, bool simplify)
        {
            JArray jarr = new JArray();
            bool islocal = WeiSha.Core.Server.IsLocalIP;
            foreach (Song.Entities.ManageMenu m in items)
            {
                if (item == null)
                {
                    if (m.MM_PatId != "0") continue;
                }
                else
                {
                    if (!m.MM_PatId.Equals(item.MM_UID, StringComparison.OrdinalIgnoreCase)) continue;
                }
                //如果不是本机id，则显示项单项完成（因为完成度只是为了在开发时记录一下完成状态）
                if (!islocal) m.MM_Complete = 100;

                JObject jo = m.ToJObject(string.Empty, simplify ? @"MM_UID,MM_Marker,MM_IsFixed,MM_Func,MM_Order,MM_IsChilds,MM_IsShow,
                                                    MM_PatId,MM_Root,
                                                    MM_Font,MM_IsBold,MM_IsItalic,MM_Color,
                                                    MM_IcoCode,MM_IcoSize,MM_IcoColor,MM_IcoX,MM_IcoY" : string.Empty);
                jo.Add("id", "node_" + m.MM_UID.ToString());
                jo.Add("label", m.MM_Name);
                jo.Add("ico", string.IsNullOrWhiteSpace(m.MM_IcoCode) ? "" : m.MM_IcoCode);
                //字体样式
                JObject jfont = new JObject();
                jfont.Add("bold", m.MM_IsBold);
                jfont.Add("italic", m.MM_IsItalic);
                jfont.Add("font", m.MM_Font);
                jfont.Add("color", m.MM_Color);
                jo.Add("font", jfont);
                //图标样式
                JObject jicon = new JObject();
                jicon.Add("size", m.MM_IcoSize);
                jicon.Add("color", m.MM_IcoColor);
                jicon.Add("x", m.MM_IcoX);
                jicon.Add("y", m.MM_IcoY);
                jo.Add("icon", jicon);
                //计算下级
                jo.Add("children", _MenuNode(m, items, simplify));
                jarr.Add(jo);
            }
            return jarr;
        }
        /// <summary>
        /// 更新系统菜单
        /// </summary>
        /// <param name="tree">来自客户端提交的json</param>
        /// <returns></returns>
        [HttpPost]
        [SuperAdmin]
        public bool SystemMenuUpdate(JArray tree)
        {
            List<Song.Entities.ManageMenu> mlist = new List<Entities.ManageMenu>();
            _MenuUpdate(tree, "0", mlist);
            Business.Do<IManageMenu>().UpdateSystemTree(mlist.ToArray());
            return true;
        }
        #endregion

        /// <summary>
        /// 更新功能菜单
        /// </summary>
        /// <param name="uid">功能菜单根菜单的uid</param>
        /// <param name="tree">来自客户端提交的json</param>
        /// <returns></returns>
        [HttpPost]
        [SuperAdmin]
        public bool FuncMenuUpdate(string uid, JArray tree)
        {
            List<Song.Entities.ManageMenu> mlist = new List<Entities.ManageMenu>();
            _MenuUpdate(tree, uid, mlist);
            Business.Do<IManageMenu>().UpdateFunctionTree(uid, mlist.ToArray());
            return true;
        }
        private void _MenuUpdate(JArray tree, string pid, List<Song.Entities.ManageMenu> mlist)
        {
            //JArray jarr = JArray.Parse(tree);
            for (int i = 0; i < tree.Count; i++)
            {
                JArray childJson = new JArray();
                Song.Entities.ManageMenu m = _MenuParse((JObject)tree[i], ref childJson);
                if (string.IsNullOrWhiteSpace(m.MM_UID))
                    m.MM_UID = WeiSha.Core.Request.UniqueID();
                m.MM_Order = i;
                m.MM_PatId = pid;
                mlist.Add(m);
                if (m.MM_IsChilds)
                {
                    _MenuUpdate(childJson, m.MM_UID, mlist);
                }
            }
        }
        private Song.Entities.ManageMenu _MenuParse(JObject jo, ref JArray childJson)
        {  
            Song.Entities.ManageMenu mm = new Entities.ManageMenu();
            Type target = mm.GetType();
            IEnumerable<JProperty> properties = jo.Properties();
            foreach (JProperty item in properties)
            {
                string key = item.Name;
                string val = item.Value.ToString();

                PropertyInfo targetPP = target.GetProperty(key);
                if (targetPP != null)
                {
                    object tm = string.IsNullOrEmpty(val) ? null : WeiSha.Core.DataConvert.ChangeType(val.Trim(), targetPP.PropertyType);
                    targetPP.SetValue(mm, tm, null);
                }
                if (key.Equals("children", StringComparison.InvariantCultureIgnoreCase))
                {
                    childJson = (JArray)item.Value;
                    if (childJson.ToString() != "[]") mm.MM_IsChilds = true;
                }
            }
            return mm;
        }
        #region 菜单操作权限相关
        /// <summary>
        /// 当前管理员的菜单项
        /// </summary>
        /// <returns></returns>
        [Admin]
        [HttpGet]
        public JArray Menus()
        {
            Song.Entities.EmpAccount acc = LoginAdmin.Status.User(this.Letter);
            if (acc == null) throw new ExceptionForNoLogin();
            acc = Business.Do<IEmployee>().GetSingle(acc.Acc_Id);
            if(acc==null) throw new ExceptionForNoLogin("当前管理员不存在");
            //根菜单的标识，由于菜单分为管理菜单、学员菜单、教师菜单，所以这里要区分
            string menu_marker = "organAdmin";
            //超级管理员，取所有功能菜单
            if (LoginAdmin.Status.IsSuperAdmin(acc))
            {
                List<Song.Entities.ManageMenu> list = new List<Entities.ManageMenu>();
                Entities.ManageMenu root = Business.Do<IManageMenu>().GetRootMarker(menu_marker);
                list.Add(root);
                List<Song.Entities.ManageMenu> mm = Business.Do<IManageMenu>().GetFunctionMenu(root.MM_UID, true, null);
                if (mm.Count > 0) list.AddRange(mm);
                return _MenuNode(null, list, true);
            }
            ////机构管理员，取分配给当前机构的功能菜单
            //else if (LoginAdmin.Status.IsAdmin(acc))
            //{
            //    return this.OrganMenus(menu_marker);
            //}
            //普通管理员，取分配给所在岗位的功能菜单
            else
            {
                List<Song.Entities.ManageMenu> mm = Business.Do<IPurview>().PosiPurviewMenu(acc.Posi_Id);
                if (mm == null) throw new Exception("当前管理员不属于任何岗位");
                return _MenuNode(null, mm, true);
            }
            //return null;
        }
        /// <summary>
        /// 机构下某一类marker标识的菜单项,如果在机构等级中设置了权限，则返回该权限的菜单项；
        /// 如果未设置，则取所有菜单项
        /// </summary>
        /// <param name="marker">例如教师管理teacher,学生管理student,机构管理organAdmin，如果为空，则取所有</param>
        /// <returns></returns>
        //[Cache(AdminDisable = true, Expires = 1440)]
        public JArray OrganMenus(string marker)
        {
            List<Song.Entities.ManageMenu> mms = null;
            if (!string.IsNullOrWhiteSpace(marker))
            {
                Song.Entities.Organization org = Business.Do<IOrganization>().OrganCurrent();
                mms = Business.Do<IPurview>().OrganPurviewMenu(org, marker);
            }
            else mms = Business.Do<IManageMenu>().GetFunctionMenu("0", true, false);
            //
            return mms != null && mms.Count > 0 ? _MenuNode(null, mms, false) : null;
        }

        /// <summary>
        /// 机构等级的权限菜单项的Uid
        /// </summary>
        /// <param name="lvid">机构等级的id</param>
        /// <returns>菜单项的UID列表</returns>
        public JArray OrganLevelPurview(int lvid)
        {
            List<Purview> pur = Business.Do<IPurview>().OrganLevelPurview(lvid);
            JArray ja = new JArray();
            for (int i = 0; i < pur.Count; i++)
            {
                ja.Add(pur[i].MM_UID);
            }
            return ja;
        }
        /// <summary>
        ///修改机构等级的权限选中信息
        /// </summary>
        /// <param name="lvid">机构等级id</param>
        /// <param name="mms">菜单项的uid</param>
        /// <returns></returns>
        [HttpPost]
        [SuperAdmin]
        public bool UpdateOrganLevelPurview(int lvid, string[] mms)
        {
            Business.Do<IPurview>().BatchAdd(lvid, mms, "orglevel");
            return true;
        }
        /// <summary>
        /// 岗位的权限
        /// </summary>
        /// <param name="posid">岗位的id</param>
        /// <returns>菜单项的UID列表</returns>
        public JArray PositionPurview(int posid)
        {
            List<Purview> pur = Business.Do<IPurview>().PositionPurview(posid);
            JArray ja = new JArray();
            for (int i = 0; i < pur.Count; i++)
            {
                ja.Add(pur[i].MM_UID);
            }
            return ja;
        }
        /// <summary>
        /// 修改岗位的权限选中信息
        /// </summary>
        /// <param name="posid">岗位的id</param>
        /// <param name="mms">菜单项的uid</param>
        /// <returns></returns>
        [HttpPost]
        [SuperAdmin]
        public bool UpdatePositionPurview(int posid, string[] mms)
        {
            Business.Do<IPurview>().BatchAdd(posid, mms, "posi");
            return true;
        }
        #endregion
    }
}
