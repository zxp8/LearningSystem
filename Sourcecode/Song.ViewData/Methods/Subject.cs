using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Web.Mvc;
using Song.Entities;
using Song.ServiceInterfaces;
using Song.ViewData.Attri;
using WeiSha.Core;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Web;
using System.IO;

namespace Song.ViewData.Methods
{
    /// <summary>
    /// 专业管理，或叫学科管理
    /// </summary>
    [HttpPut, HttpGet]
    public class Subject : ViewMethod, IViewAPI
    {
        //资源的虚拟路径和物理路径
        private static string _virPath = WeiSha.Core.Upload.Get["Subject"].Virtual;
        private static string _phyPath = WeiSha.Core.Upload.Get["Subject"].Physics;
        /// <summary>
        /// 通过专业ID，获取专业信息
        /// </summary>
        /// <param name="id">专业id</param>
        /// <returns></returns>
        [Cache(AdminDisable = true)]
        public Song.Entities.Subject ForID(long id)
        {
            Song.Entities.Subject sbj = Business.Do<ISubject>().SubjectSingle(id);
            return _tran(sbj);
        }

        /// <summary>
        /// 添加课程专业
        /// </summary>
        /// <param name="entity">专业的实体</param>
        /// <returns></returns>
        [Admin]
        [HttpPost]
        [Upload(Config = "SubjectLogo")]
        [HtmlClear(Not = "entity")]
        public Song.Entities.Subject Add(Song.Entities.Subject entity)
        {          
            if (this.Files.Count > 0)
                entity = this._upload_photo(this.Files, entity);
            try
            {  
                Business.Do<ISubject>().SubjectAdd(entity);
                return entity;
            }
            catch (Exception ex)
            {
                if (this.Files.Count > 0)
                    WeiSha.Core.Upload.Get["Subject"].DeleteFile(entity.Sbj_Logo);
                throw ex;
            }
        }
        /// <summary>
        /// 修改课程专业
        /// </summary>
        /// <param name="entity">专业的实体</param>
        /// <returns></returns>
        [Admin]
        [HttpPost]
        [Upload(Config = "SubjectLogo")]
        [HtmlClear(Not = "entity")]
        public Song.Entities.Subject Modify(Song.Entities.Subject entity)
        {
            Song.Entities.Subject old = Business.Do<ISubject>().SubjectSingle(entity.Sbj_ID);
            if (old == null) throw new Exception("Not found entity for Subject！");
            //上传的图片
            string oldlogo = old.Sbj_Logo;      //之前的图片

            if (this.Files.Count > 0)
                entity = this._upload_photo(this.Files, entity);
            try
            {
                string nomidfy = "Sbj_CrtTime";
                if (this.Files.Count < 1) nomidfy += "Sbj_Logo,Sbj_LogoSmall";
                old.Copy<Song.Entities.Subject>(entity, nomidfy);
                Business.Do<ISubject>().SubjectSave(old);
                if ((this.Files.Count > 0 && !string.IsNullOrWhiteSpace(oldlogo))
                    || (this.Files.Count < 1) && string.IsNullOrWhiteSpace(entity.Sbj_Logo) && !string.IsNullOrWhiteSpace(oldlogo))
                {
                    WeiSha.Core.Upload.Get["Subject"].DeleteFile(oldlogo);
                }
                return _tran(old);
            }
            catch (Exception ex)
            {
                if (this.Files.Count > 0)
                    WeiSha.Core.Upload.Get["Subject"].DeleteFile(entity.Sbj_Logo);                
                throw ex;
            }
        }
        private Song.Entities.Subject _upload_photo(HttpFileCollectionBase files, Song.Entities.Subject entity)
        {
            string filename = string.Empty, smallfile = string.Empty;
            //只保存第一张图片
            if (this.Files.Count > 0)
            {
                foreach (string key in this.Files)
                {
                    HttpPostedFileBase file = this.Files[key];
                    filename = WeiSha.Core.Request.UniqueID() + Path.GetExtension(file.FileName);
                    file.SaveAs(_phyPath + filename);
                    try
                    {
                        //生成缩略图
                        smallfile = WeiSha.Core.Images.Name.ToSmall(filename);
                        WeiSha.Core.Images.FileTo.Thumbnail(_phyPath + filename, _phyPath + smallfile, 320, 180, 0);
                    }
                    catch (Exception ex)
                    {
                        //WeiSha.Core.Upload.Get["Subject"].DeleteFile(filename);
                        //throw ex;
                    }

                    break;
                }
            }
            entity.Sbj_Logo = filename;
            entity.Sbj_LogoSmall = smallfile;
            return entity;
        }
        /// <summary>
        /// 更改专业的排序
        /// </summary>
        /// <param name="list">专业的数组</param>
        /// <returns></returns>
        [HttpPost]
        [Admin]
        public bool ModifyTaxis(Song.Entities.Subject[] list)
        {
            Business.Do<ISubject>().UpdateTaxis(list);
            return true;
        }
        /// <summary>
        /// 删除课程专业，只是逻辑删除
        /// </summary>
        /// <param name="id">账户id，可以是多个，用逗号分隔</param>
        /// <returns></returns>
        [Admin]
        [HttpDelete, HttpGet(Ignore = true)]
        public int Delete(string id)
        {
            int i = 0;
            if (string.IsNullOrWhiteSpace(id)) return i;
            List<long> list = id.ToList<long>();
            foreach (long s in list)
                i += Business.Do<ISubject>().SubjectDelete(s);
            return i;
        }
        /// <summary>
        /// 还原逻辑删除的专业
        /// </summary>
        [Admin]
        [HttpPost, HttpGet(Ignore = true)]
        public int Recycle(string id)
        {
            int i = 0;
            if (string.IsNullOrWhiteSpace(id)) return i;
            List<long> list = id.ToList<long>();
            foreach (long s in list)
                i += Business.Do<ISubject>().SubjectRecycle(s);
            return i;
        }

        /// <summary>
        /// 删除专业（物理删除）
        /// </summary>
        /// <param name="id">试题id，可以是多个，用逗号分隔</param>
        /// <returns></returns>
        [Admin]
        [HttpDelete, HttpGet(Ignore = true)]
        public int Remove(string id)
        {
            int i = 0;
            if (string.IsNullOrWhiteSpace(id)) return i;
            List<long> list = id.ToList<long>();
            foreach (long s in list)
            {
                try
                {
                    i += Business.Do<ISubject>().SubjectRemove(s);
                }
                catch
                {

                }
            }
            return i;
        }
        /// <summary>
        /// 当前试题分类的上级父级
        /// </summary>
        /// <param name="sbjid"></param>
        /// <param name="isself">是否包括自身</param>
        /// <returns></returns>
        public List<Entities.Subject> Parents(long sbjid, bool isself) => Business.Do<ISubject>().Parents(sbjid, isself);
        /// <summary>
        /// 某个机构下的所有专业
        /// </summary>
        /// <param name="orgid">机构id</param>
        /// <param name="search"></param>
        /// <param name="isuse"></param>
        /// <param name="delete"></param>
        /// <returns>专业列表</returns>
        public List<Song.Entities.Subject> List(int orgid, string search, bool? isuse, bool? delete)
        {
            List<Song.Entities.Subject> sbjs = Business.Do<ISubject>().SubjectCount(orgid, search, isuse, delete, -1, -1);
            for (int i = 0; i < sbjs.Count; i++)
            {
                sbjs[i] = _tran(sbjs[i]);
            }
            return sbjs;
        }
        /// <summary>
        /// 获取某个机构下的所有专业
        /// </summary>
        /// <param name="orgid"></param>
        /// <param name="search"></param>
        /// <param name="isuse"></param>
        /// <param name="delete">是否删除</param>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [Admin]
        public ListResult Pager(int orgid, string search, bool? isuse, bool? delete, int size, int index)
        {
            size = size <= 0 ? int.MaxValue : size;
            int count;
            List<Song.Entities.Subject> eas = Business.Do<ISubject>().SubjectPager(orgid, -1, isuse, delete, search,  size, index, out count);
            for (int i = 0; i < eas.Count; i++)
            {
                eas[i] = _tran(eas[i]);               
            }
            ListResult result = new ListResult(eas);
            result.Index = index;
            result.Size = size;
            result.Total = count;
            return result;
        }
        /// <summary>
        /// 某个机构下的专业，用于前端展示，被禁用的专业不显示
        /// </summary>
        /// <param name="orgid">机构id</param>
        /// <returns>专业列表</returns>
        [Cache]
        public JArray TreeFront(int orgid)
        {
            List<Song.Entities.Subject> sbjs = Business.Do<ISubject>().SubjectCount(orgid, string.Empty, true, false, -1, -1);
            for (int i = 0; i < sbjs.Count; i++)
            {
                sbjs[i] = _tran(sbjs[i]);
            }           
            return sbjs.Count > 0 ? _SubjectNode(null, sbjs) : null;
        }
        /// <summary>
        /// 机构下的专业，树形数据
        /// </summary>
        /// <param name="orgid">机构id</param>
        /// <param name="search">按名称检索</param>
        /// <param name="isuse">是否启用</param>
        /// <param name="delete">是否删除</param>
        /// <returns></returns>
        public JArray Tree(int orgid, string search, bool? isuse,bool? delete)
        {
            List<Song.Entities.Subject> sbjs = Business.Do<ISubject>().SubjectCount(orgid, search, isuse, delete, -1, -1);
            for (int i = 0; i < sbjs.Count; i++) sbjs[i] = _tran(sbjs[i]);          
            return sbjs.Count > 0 ? _SubjectNode(null, sbjs) : null;
        }
        /// <summary>
        /// 生成菜单子节点
        /// </summary>
        /// <param name="item">当前菜单项</param>
        /// <param name="items">所有菜单项</param>
        /// <returns></returns>
        private JArray _SubjectNode(Song.Entities.Subject item, List<Song.Entities.Subject> items)
        {            
            List<Song.Entities.Subject> childs = new List<Song.Entities.Subject>();
            for (int i = 0; i < items.Count; i++)
            {
                Entities.Subject m = items[i];
                if (item == null && m.Sbj_PID != 0) continue;
                if (item != null && m.Sbj_PID != item.Sbj_ID) continue;
                childs.Add(m);
                items.RemoveAt(i);
                i--;
            }
            JArray jarr = new JArray();
            for (int i = 0; i < childs.Count; i++)
            {
                JObject jo = childs[i].ToJObject();              
                //计算下级
                JArray charray = _SubjectNode(childs[i], items);
                if (charray.Count > 0) jo.Add("children", charray);
                jarr.Add(jo);
            }
            return jarr;
        }
        /// <summary>
        /// 供前端显示的专业最顶级分类
        /// </summary>
        /// <param name="orgid"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [Cache]
        public List<Song.Entities.Subject> ShowRoot(int orgid, int count)
        {
            List<Song.Entities.Subject> sbjs = Business.Do<ISubject>().SubjectCount(orgid, string.Empty, true, false, 0, count);
            string path = WeiSha.Core.Upload.Get["Subject"].Virtual;
            for(int i = 0; i < sbjs.Count; i++)
            {
                Song.Entities.Subject c = sbjs[i];
                c = _tran(c);              
                //如果别名为空，则别名等于专业名称
                if (string.IsNullOrWhiteSpace(c.Sbj_ByName) || c.Sbj_ByName.Trim() == "")
                    c.Sbj_ByName = c.Sbj_Name;
                c.Sbj_Intro = HTML.ClearTag(c.Sbj_Intro);
            }
            return sbjs;
        }
        /// <summary>
        /// 分页获取专业（前端页面）
        /// </summary>
        /// <param name="orgid">机构id</param>
        /// <param name="pid">上级id，默认是0</param>
        /// <param name="index">页码，即第几页</param>
        /// <param name="size">每页多少条记录</param>
        /// <returns></returns>
        public ListResult PagerFront(int orgid,long pid,int index, int size)
        {
            int sum;
            List<Song.Entities.Subject> list = Business.Do<ISubject>().SubjectPager(orgid, pid, true, false, string.Empty, size, index, out sum);
            for (int i = 0; i < list.Count; i++)
                list[i] = _tran(list[i]);
            Song.ViewData.ListResult result = new ListResult(list);
            result.Index = index;
            result.Size = size;
            result.Total = sum;
            return result;
        }
        /// <summary>
        /// 专业下的课程数，包括专业下的子级专业
        /// </summary>
        /// <param name="sbjid">专业id</param>
        /// <param name="use">是否包括启用的课程,null取所有，true取启用的，false取未启用的</param>
        /// <param name="del">是否删除的课程</param>
        /// <returns></returns>
        [Cache(AdminDisable = true)]
        public int CountOfCourse(long sbjid, bool? use, bool? del)
        {
            return Business.Do<ICourse>().CourseOfCount(-1, sbjid, -1, use, del, null);
        }
        /// <summary>
        /// 更新专业的统计数据
        /// </summary>
        /// <param name="orgid">机构id，如果大于0，则刷新当前机构下的所有专业数据</param>
        /// <param name="sbjid">专业id</param>
        /// <returns></returns>
        [HttpPost]
        public bool UpdateStatisticalData(int orgid,long sbjid)
        {
            return Business.Do<ISubject>().UpdateStatisticalData(orgid, sbjid);
        }
        /// <summary>
        /// 当前专业所包含的所有下级专业数量（不包括自身）
        /// </summary>
        /// <param name="orgid">机构id</param>
        /// <param name="sbjid">当前专业id，为0时为顶级</param>
        /// <param name="use">是否包括启用的课程,null取所有，true取启用的，false取未启用的</param>
        /// <returns></returns>
        public int CountOfChildren(int orgid,long sbjid, bool? use)
        {
            return Business.Do<ISubject>().SubjectOfCount(orgid, sbjid, use, true);
        }
        #region 私有方法，处理对象的关联信息
        /// <summary>
        /// 处理专业信息，图片转为全路径，并生成clone对象
        /// </summary>
        /// <param name="obj">专业对象</param>
        /// <returns></returns>
        private Song.Entities.Subject _tran(Song.Entities.Subject obj)
        {
            if (obj == null) return obj;
            obj.Sbj_Logo = System.IO.File.Exists(_phyPath + obj.Sbj_Logo) ? _virPath + obj.Sbj_Logo : "";
            obj.Sbj_LogoSmall = System.IO.File.Exists(_phyPath + obj.Sbj_LogoSmall) ? _virPath + obj.Sbj_LogoSmall : "";
            if (string.IsNullOrWhiteSpace(obj.Sbj_LogoSmall) && !string.IsNullOrWhiteSpace(obj.Sbj_Logo))
                obj.Sbj_LogoSmall = obj.Sbj_Logo;
            return obj;
        }
        #endregion
    }
}
