using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Linq;

namespace Song.ServiceImpls.Handler
{
    public class DataTableTree
    {
        #region 属性
        /// <summary>
        /// 用于生成树的数据
        /// </summary>
        public DataTable DataSource { get; set; }

        /// <summary>
        /// 数据列的主键id，字段名称
        /// </summary>
        public string IDKeyName { get; set; }
        /// <summary>
        /// 数据列的父id，字段名称
        /// </summary>
        public string ParentIDKeyName { get; set; }
        /// <summary>
        /// 数据列的排序号，字段名称
        /// </summary>
        public string TaxKeyName { get; set; }
        /// <summary>
        /// 树形根节点的id值
        /// </summary>
        public long Root { get; set; }
        #endregion

        #region 生成树形数据
        /// <summary>
        /// 将数据源转换成树形数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public DataTable BuilderTree<T>(List<T> list) where T : WeiSha.Data.Entity
        {
            return BuilderTree(this.DataTableFor<T>(list));
        }
        /// <summary>
        /// 将数据源将换成树形数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DataTable BuilderTree(DataTable dt)
        {
            if (dt == null) return null;
            //DataView dv = dt.DefaultView;
            //dv.Sort = IdKeyName+" Asc";
            //dt = dv.ToTable();
            //如果存在下述字段，说明已经处理过为树形
            if (dt.Columns["Tree"] == null) dt.Columns.Add(new DataColumn("Tree", typeof(String)));
            //是否为第一个
            if (dt.Columns["isTop"] == null) dt.Columns.Add(new DataColumn("isTop", typeof(String)));
            //是否为最后一个
            if (dt.Columns["isDown"] == null) dt.Columns.Add(new DataColumn("isDown", typeof(String)));
            //克隆一个同样的表，用于构建树形数据
            DataTable tree = dt.Clone();
            sortFunc(dt, tree, this.Root);
            //如果有些数据没有上级行，导致没有出现在树形中
            if (tree.Rows.Count < dt.Rows.Count)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    bool isExist = false;
                    for (int j = 0; j < tree.Rows.Count; j++)
                    {
                        if (tree.Rows[j][IDKeyName].ToString() == dt.Rows[i][IDKeyName].ToString())
                        {
                            isExist = true;
                            break;
                        }
                    }
                    if (!isExist) tree.ImportRow(dt.Rows[i]);                    
                }
            }
            return tree;
        }

        /// <summary>
        /// 处理数据源，生成树形数据
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="sortDt"></param>
        /// <param name="parentId"></param>
        private void sortFunc(DataTable dt, DataTable sortDt, long parentId)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string tm = dt.Rows[i][ParentIDKeyName] == null ? "" : dt.Rows[i][ParentIDKeyName].ToString();
                long pat;
                long.TryParse(dt.Rows[i][ParentIDKeyName].ToString(), out pat);
                //当前深度
                if (pat == parentId)
                {
                    //节点前的链接线
                    string before = line(dt, dt.Rows[i]);
                    //当前节点不是根节点时
                    //是不是最后一个
                    if (isBottom(dt, dt.Rows[i], i))
                    {
                        before += "┗";
                        dt.Rows[i]["isDown"] = true;
                    }
                    else
                    {
                        before += "┣";
                        dt.Rows[i]["isDown"] = false;
                    }
                    //是不是最后一个
                    if (isTop(dt, dt.Rows[i], i))
                    {
                        //before += "┕";
                        dt.Rows[i]["isTop"] = true;
                    }
                    else
                    {
                        //before += "┝";
                        dt.Rows[i]["isTop"] = false;
                    }
                    //}
                    dt.Rows[i]["Tree"] = before;
                    //lcs[i].Cname = " " + name + lcs[i].Cname;
                    sortDt.ImportRow(dt.Rows[i]);
                    //sort.Rows.Add(dt.Rows[i]);
                    if (isChildren(dt, dt.Rows[i]))
                    {
                        long parentid;
                        long.TryParse(dt.Rows[i][IDKeyName].ToString(),out parentid);
                        sortFunc(dt, sortDt, parentid);
                    }
                }
            }
            //return sort;
        }
        /// <summary>
        /// 当前对象是否有子级
        /// </summary>
        /// <param name="dt">当前层（深度）集合</param>
        /// <param name="dr">当前对象</param>
        /// <returns>是否有子级，有返回true，否则返回false</returns>
        private bool isChildren(DataTable dt, DataRow dr)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][ParentIDKeyName].ToString() == dr[IDKeyName].ToString())
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 是否为当前层深的最后一个；正序排
        /// </summary>
        /// <param name="dt">当前层（深度）集合</param>
        /// <param name="dr">当前对象</param>
        /// <param name="index"></param>
        /// <returns>是最后一个，返回true，否则返回false</returns>
        private bool isBottom(DataTable dt, DataRow dr, int index)
        {
            //当前对象序号，如果有“排序号字段”，则按字段排序，否则按索引
            int currentTax = TaxKeyName != "" ? Convert.ToInt32(dr[TaxKeyName] == null || dr[TaxKeyName].ToString() == "" ? 0 : dr[TaxKeyName]) : index;
            int tempTax = currentTax;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //同级的兄弟分类
                if (dt.Rows[i][ParentIDKeyName].ToString() == dr[ParentIDKeyName].ToString())
                {
                    int t = TaxKeyName != "" ? Convert.ToInt32(dt.Rows[i][TaxKeyName]==null || dt.Rows[i][TaxKeyName].ToString() == "" ? 0 : dt.Rows[i][TaxKeyName]) : i;
                    if (t > currentTax)
                    {
                        tempTax = t;
                    }
                }
            }
            if (currentTax == tempTax)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 是否为当前层深的第一个；正序排
        /// </summary>
        /// <param name="dt">当前层（深度）集合</param>
        /// <param name="dr">当前对象</param>
        /// <returns>是第一个，返回true，否则返回false</returns>
        private bool isTop(DataTable dt, DataRow dr, int index)
        {
            //当前对象序号，如果有“排序号字段”，则按字段排序，否则按索引
            int currentTax = TaxKeyName != "" ? Convert.ToInt32(dr[TaxKeyName] == null || dr[TaxKeyName].ToString() == "" ? 0 : dr[TaxKeyName]) : index;
            int tempTax = currentTax;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //同级的兄弟分类
                if (dt.Rows[i][ParentIDKeyName].ToString() == dr[ParentIDKeyName].ToString())
                {
                    int t = TaxKeyName != "" ? Convert.ToInt32(dt.Rows[i][TaxKeyName] == null || dt.Rows[i][TaxKeyName].ToString() == "" ? 0 : dt.Rows[i][TaxKeyName]) : i;
                    if (t < currentTax)
                    {
                        tempTax = t;
                    }
                }
            }
            if (currentTax == tempTax)
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 节点前的空格或竖线
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        private string line(DataTable dt, DataRow dr)
        {
            string line = "";
            while (Convert.ToInt64(dr[ParentIDKeyName]==null || dr[ParentIDKeyName].ToString() == "" ? 0 : dr[ParentIDKeyName]) > Root)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //取当前对象的父级
                    if (dt.Rows[i][IDKeyName].ToString() == dr[ParentIDKeyName].ToString())
                    {
                        dr = dt.Rows[i];
                        if (isBottom(dt, dr, i))
                        {
                            line = "　" + line;
                        }
                        else
                        {
                            line = "┃" + line;
                        }
                        break;
                    }
                }
            }
            return line;
        }
        #endregion

        #region 数据处理
        /// <summary>
        /// 校验数据源，主要是担心存在循环引用，导致递归栈溢出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<T> CheckTree<T>(List<T> list) where T : WeiSha.Data.Entity
        {
            return null;
        }
        //private List<T> _treeid(T id, List<T> ols) where T : WeiSha.Data.Entity
        //{
        //    List<T> list = new List<T>();
        //    if (id > 0) list.Add(id);
        //    List<T> childs = ols.Where(s => s.Ol_PID == id).Select(s => s.Sbj_ID).ToList();
        //    ols.RemoveAll(s => s.Ol_PID == id);
        //    for (int i = 0; i < childs.Count; i++)
        //    {
        //        list.Add(childs[i]);
        //        List<long> tm = _treeid(childs[i], ols);
        //        list.AddRange(tm.Except(list));
        //    }
        //    return list;
        //}
        /// <summary>
        /// 将对象数组转换为DataTable
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public DataTable DataTableFor<T>(List<T> list) where T : WeiSha.Data.Entity
        {
            if (list.Count < 1) return null;
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Tree", typeof(String)));
            dt.Columns.Add(new DataColumn("isTop", typeof(bool)));
            dt.Columns.Add(new DataColumn("isDown", typeof(bool)));
            //取数组中的一个实例
            foreach (T obj in list)
            {
                Type type = obj.GetType();
                //获取对象的属性列表
                PropertyInfo[] properties = type.GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo pi = properties[i];
                    //属性名（包括泛型名称）
                    var nullableType = Nullable.GetUnderlyingType(pi.PropertyType);
                    string typename = nullableType != null ? nullableType.Name : pi.PropertyType.Name;
                    //if (typename.IndexOf("`") > -1) typename = typename.Substring(0, typename.IndexOf("`"));
                    if (pi.PropertyType.ToString() == "System.Nullable" || typename.IndexOf("`") > -1)
                    {
                        dt.Columns.Add(new DataColumn(pi.Name, typeof(string)));
                    }
                    else
                    {
                        dt.Columns.Add(new DataColumn(pi.Name, pi.PropertyType));
                    }
                }
                break;
            }
            //填充数据
            foreach (T obj in list)
            {
                DataRow dr = dt.NewRow();
                Type info = obj.GetType();
                //获取对象的属性列表
                PropertyInfo[] properties = info.GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo pi = properties[i];
                    dr[pi.Name] = info.GetProperty(pi.Name).GetValue(obj, null);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        #endregion
    }
}
