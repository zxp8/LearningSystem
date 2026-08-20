using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Newtonsoft.Json.Linq;
//using System.Web.Mvc;
using Song.Entities;
using Song.ServiceInterfaces;
using Song.ViewData.Attri;
using WeiSha.Core;
using System.Data;


namespace Song.ViewData.Methods
{
    /// <summary>
    /// 专项考试的管理，包括出卷、考试、成绩查看等
    /// </summary>
    [HttpPut, HttpGet]
    public class Exam : ViewMethod, IViewAPI
    {
        //资源的虚拟路径和物理路径
        public static string PathKey = "Exam";
        public static string VirPath = WeiSha.Core.Upload.Get[PathKey].Virtual;
        public static string PhyPath = WeiSha.Core.Upload.Get[PathKey].Physics;

        #region 增删改查

        /// <summary>
        /// 考试主题
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>    
        public Song.Entities.Examination ThemeForUID(string uid)
        {
            return Business.Do<IExamination>().ExamTheme(uid);
        }
        /// <summary>
        /// 根据ID查询考试
        /// </summary>
        /// <param name="id">考试id，可以是考试题，也可以是场次</param>
        /// <returns></returns>    
        public Song.Entities.Examination ForID(long id)
        {
            return Business.Do<IExamination>().ExamSingle(id);
        }
        /// <summary>
        /// 新增考试
        /// </summary>
        /// <param name="theme">考试主题的对象</param>
        /// <param name="items">考试场次的对象数组</param>
        /// <param name="groups">限定参考的学员组关联对象（ExamGroup），即可以参加考试的学员组</param>
        /// <param name="accounts">限定参考的学员关联对象（Exam_Accounts）</param>
        [HttpPost]
        [Admin, Teacher]
        public bool Add(Examination theme, Examination[] items, ExamGroup[] groups, Exam_Accounts[] accounts)
        {
            //当前机构
            Song.Entities.Organization org = Business.Do<IOrganization>().OrganCurrent();
            if (org != null)
            {
                theme.Org_ID = org.Org_ID;
                theme.Org_Name = org.Org_Name;
            }
            //当前编辑的教师
            Song.Entities.Teacher teacher = this.Teacher;
            if (teacher != null)
            {
                theme.Th_ID = teacher.Th_ID;
                theme.Th_Name = teacher.Th_Name;
            }
            //当前编辑的管理员
            Song.Entities.EmpAccount admin = this.Admin;
            if (admin != null) theme.Acc_Id = admin.Acc_Id;
            //
            Business.Do<IExamination>().ExamAdd(theme, items, groups, accounts);
            return true;
        }
        /// <summary>
        /// 修改考试
        /// </summary>
        /// <param name="theme">考试主题的对象</param>
        /// <param name="items">考试场次的对象数组</param>
        /// <param name="groups">关联的学员组，即可以参加考试的学员组</param>
        /// <param name="accounts">限定参考的学员关联对象（Exam_Accounts）</param>
        [HttpPost]
        [Admin, Teacher]
        public bool Modify(Examination theme, Examination[] items, ExamGroup[] groups, Exam_Accounts[] accounts)
        {
            Song.Entities.Examination old = Business.Do<IExamination>().ExamSingle(theme.Exam_ID);
            if (old == null) throw new Exception("Not found entity for Examination！");
            old.Copy<Song.Entities.Examination>(theme);
            //考试场次
            if (items != null)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    Song.Entities.Examination ex_old = Business.Do<IExamination>().ExamSingle(items[i].Exam_ID);
                    if (ex_old != null)
                    {
                        ex_old.Copy<Song.Entities.Examination>(items[i]);
                        items[i] = ex_old;
                    }
                }
            }

            Business.Do<IExamination>().ExamSave(old, items, groups, accounts);

            return true;
        }
        /// <summary>
        /// 删除考试
        /// </summary>
        /// <param name="id">考试主题的id，可以是多个，用逗号分隔</param>
        /// <returns></returns>
        [Admin,Teacher]
        [HttpDelete]
        public int ThemeDelete(string id)
        {
            int i = 0;
            if (string.IsNullOrWhiteSpace(id)) return i;
            List<long> list = id.ToList<long>();
            foreach (long s in list)
                i += Business.Do<IExamination>().ExamThemeDelete(s);
            return i;
        }
        /// <summary>
        /// 修改考试的状态
        /// </summary>
        /// <param name="id">考试的id，可以是多个，用逗号分隔</param>
        /// <param name="use">是否启用</param>    
        /// <returns></returns>
        [HttpPost]
        [Admin,Teacher]
        public int ModifyState(string id, bool use)
        {
            int i = 0;
            if (string.IsNullOrWhiteSpace(id)) return i;
            List<long> list = id.ToList<long>();
            foreach (long s in list)
                i += Business.Do<IExamination>().ExamUpdate(s,
                    new WeiSha.Data.Field[] { Song.Entities.Examination._.Exam_IsUse },
                    new object[] { use });
            return i;
        }
       
        /// <summary>
        /// 是否需要人工批阅
        /// </summary>
        /// <param name="examid"></param>
        /// <returns>id:考试id,manual:是否需要人工批阅，true为需要</returns>
        public JObject Manual4Exam(long examid)
        {
            bool manual = false;
            //考生数，如果没有人考试，则不需要批阅
            int students = Business.Do<IExamination>().Numbertimes4Exam(examid);
            if (students > 0)
            {
                Song.Entities.Examination exas = Business.Do<IExamination>().ExamSingle(examid);
                Song.Entities.TestPaper pager = Business.Do<ITestPaper>().PaperSingle(exas.Tp_Id);
                if (pager != null)
                {
                    List<Song.Entities.TestPaperItem> items = Business.Do<ITestPaper>().PaperItems(pager);
                    foreach (Song.Entities.TestPaperItem ti in items)
                    {
                        if (ti.TPI_Type == 4)
                        {
                            manual = true;
                            break;
                        }
                    }
                }
            }
            JObject jo = new JObject();
            jo.Add("id", examid);
            jo.Add("manual", manual);
            return jo;
        }
        /// <summary>
        /// 获取考试主题
        /// </summary>
        /// <param name="orgid">机构id</param>
        /// <param name="start">时间范围查询的开始时间</param>
        /// <param name="end"></param>
        /// <param name="use"></param>
        /// <param name="search">按考试主题检索</param>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ListResult ThemePager(int orgid, DateTime? start, DateTime? end, bool? use, string search, int size, int index)
        {
            int count;
            List<Examination> datas = Business.Do<IExamination>().ThemePager(orgid, start, end, use, search, size, index, out count);
            ListResult result = new ListResult(datas);
            result.Index = index;
            result.Size = size;
            result.Total = count;
            return result;
        }
        /// <summary>
        /// 获取考试主题
        /// </summary>
        /// <param name="orgid"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="use"></param>
        /// <param name="search"></param>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [HttpPost]
        [Admin, Teacher]
        public ListResult ThemeAdminPager(int orgid, DateTime? start, DateTime? end, bool? use, string search, int size, int index)
        {
            int count;
            List<Examination> datas = Business.Do<IExamination>().ThemePager(orgid, start, end, use, search, size, index, out count);
            ListResult result = new ListResult(datas);
            result.Index = index;
            result.Size = size;
            result.Total = count;
            return result;
        }
        /// <summary>
        /// 获取考试场次
        /// </summary>
        /// <param name="orgid"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="use">是否启用</param>
        /// <param name="ismanual">是否需要人工批阅</param>
        /// <param name="search">考试名称的检索</param>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [HttpPost]
        [Admin, Teacher]
        public ListResult ExamAdminPager(int orgid, DateTime? start, DateTime? end, bool? use, bool? ismanual, string search, int size, int index)
        {
            int count;
            List<Examination> datas = Business.Do<IExamination>().ExamPager(orgid, start, end, use, ismanual, search, size, index, out count);
            ListResult result = new ListResult(datas);
            result.Index = index;
            result.Size = size;
            result.Total = count;
            return result;
        }
        /// <summary>
        /// 某个考试主题下的所有考试场次
        /// </summary>
        /// <param name="uid">考试主题的uid</param>
        /// <returns></returns>
        public List<Examination> Exams(string uid)
        {
            List<Examination> exams = Business.Do<IExamination>().ExamItems(uid);
            for (int i = 0; i < exams.Count; i++)
            {
                DateTime examDate = exams[i].Exam_Date < DateTime.Now.AddYears(-100) ? DateTime.Now : (DateTime)exams[i].Exam_Date;
                exams[i].Exam_Date = examDate < DateTime.Now.AddYears(-100) ? DateTime.Now : examDate;
            }
            return exams;
        }
        #endregion

        #region 参考人员
        /// <summary>
        /// 获取参考人员信息
        /// </summary>
        /// <param name="type">类型，1为全体学员，2为分组</param>
        /// <param name="uid">考试主题的uid</param>
        /// <returns>返回的是字符串</returns>
        public string ScopeInfo(string type, string uid)
        {
            if (type == "1") return "全体学员";
            if (type == "2")
            {
                List<StudentSort> sts = Business.Do<IExamination>().ScopeForStudentSort(uid);
                int maxCount = 6;
                string strDep = "";
                for (int i = 0; i < sts.Count && i < maxCount; i++)
                {
                    strDep += sts[i].Sts_Name;
                    if (i < sts.Count - 1) strDep += ",";
                }
                if (sts.Count > maxCount) strDep += "...";
                if (string.IsNullOrWhiteSpace(strDep))
                    strDep = "(没有学员组)";
                return strDep;
            }
            if (type == "3")
            {
                int count = Business.Do<IExamination>().ScopeForAccountTotal(uid);
                return count + "名"; 
            }
            return "";
        }
        /// <summary>
        /// 获取允许参加考试的人员数量
        /// </summary>
        /// <param name="uid">考试主题的uid/param>
        /// <returns>如果考试不存在，则返回-1</returns>
        public int ScopeTotal(string uid)
        {
            Entities.Examination theme= Business.Do<IExamination>().ExamTheme(uid);
            if (theme == null) return -1;
            //全部学员
            if (theme.Exam_GroupType == 1) return Business.Do<IAccounts>().Total(theme.Org_ID, true, true);
            //指定学员组
            if (theme.Exam_GroupType == 2)
            {
                List<StudentSort> sts = Business.Do<IExamination>().ScopeForStudentSort(uid);
                long[] list = sts.Select(s => s.Sts_ID).ToArray();
                if (list.Length == 0) return 0;
                return Business.Do<IStudent>().TotalOfSort(list);
            }
            //指定的学员
            if (theme.Exam_GroupType == 3)
            {
                return Business.Do<IExamination>().ScopeForAccountTotal(uid);
            }
            return 0;
        }
        /// <summary>
        /// 当考试的参考范围限定为指定学员时，这时是限定的学员数
        /// </summary>
        /// <param name="uid">考试主题的uid</param>
        /// <returns></returns>
        public int ScopeForAccountTotal(string uid)
        {
            return Business.Do<IExamination>().ScopeForAccountTotal(uid);
        }
        /// <summary>
        /// 允许参考的学员组的信息
        /// </summary>
        /// <param name="uid">考试主题的uid</param>
        /// <returns>学员组</returns>
        public List<StudentSort> ScopeGroups(string uid)
        {
            return Business.Do<IExamination>().ScopeForStudentSort(uid);
        }
        /// <summary>
        /// 允许参考的学员信息
        /// </summary>
        /// <param name="uid">考试主题的uid</param>
        /// <returns></returns>
        public List<Accounts> ScopeAccounts(string uid)
        {
            return Business.Do<IExamination>().ScopeForAccounts(uid);
        }
        /// <summary>
        /// 分页获取允许参考的学员信息
        /// </summary>
        /// <param name="uid">考试主题的uid</param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public ListResult ScopeAccountsPager(string uid, int index, int size)
        {
            int sum;
            List<Accounts> list = Business.Do<IExamination>().ScopeForAccounts(uid, index, size, out sum);
            Song.ViewData.ListResult result = new ListResult(list);
            result.Index = index;
            result.Size = size;
            result.Total = sum;
            return result;
        }
        #endregion

        #region 出卷与交卷
        /// <summary>
        /// 当前考试的状态
        /// </summary>
        /// <param name="examid">考试场次的id</param>
        /// <returns></returns>
        public JObject State(long examid)
        {
            JObject jo = new JObject();
            //学员是否登录
            Song.Entities.Accounts acc = LoginAccount.Status.User();
            jo.Add("islogin", acc != null);
            if (acc == null) return jo;

            //是否可以考试
            Song.Entities.Examination exam = Business.Do<IExamination>().ExamSingle(examid);
            jo.Add("exist", !(exam == null || !exam.Exam_IsUse || exam.Exam_IsTheme));
            if (exam == null) return jo;
            long tpid = exam.Exam_Purpose != 0 ? exam.Etp_Id : exam.Tp_Id;
            jo.Add("uid", exam.Exam_UID);               //考试主题的uid
            jo.Add("exam", exam.ToJObject());
            //考试主题
            Examination theme = Business.Do<IExamination>().ExamTheme(exam.Exam_UID);
            jo.Add("theme", theme.ToJObject());
            jo.Add("subject", exam.Sbj_ID.ToString());  //专业id
            jo.Add("paperid", tpid.ToString());     //试卷id
            //默认是0，表示关联的试卷来自课程，如果是1，则表示关联的试卷来自考试专用试卷
            jo.Add("purpose", exam.Exam_Purpose);       //

            jo.Add("timespan", exam.Exam_Span);         //考试限时 

            //1为固定时间开始，2为限定时间区间考试
            if (exam != null) jo.Add("type", exam.Exam_DateType);
            //当前考生是否可以参加该场考试
            bool isAllow = Business.Do<IExamination>().ExamIsForStudent(examid, acc.Ac_ID);
            jo.Add("allow", isAllow);

            //关于考试状态的一些时间
            bool isStart, isOver, isSubmit;
            DateTime startTime, overTime;
            //答题记录
            Song.Entities.ExamResults exr = Business.Do<IExamination>().ResultForCache(examid, tpid, acc.Ac_ID);
            if (exr != null) jo.Add("exrid", exr.Exr_ID);
            //判断是否已经开始、是否已经结束
            if (exam.Exam_DateType == 1)
            {
                //固定时间开始
                isStart = exam.Exam_Date <= DateTime.Now;    //是否开始
                isOver = DateTime.Now > exam.Exam_Date.AddMinutes(exam.Exam_Span);   //是否结束
                startTime = exam.Exam_Date;           //开始时间
                overTime = exam.Exam_Date.AddMinutes(exam.Exam_Span);     //结束时间
                isSubmit = exr != null ? DateTime.Now > exr.Exr_OverTime || exr.Exr_IsSubmit : false;    //是否交卷
            }
            else
            {
                //按时间区间
                isStart = DateTime.Now > exam.Exam_Date && DateTime.Now < exam.Exam_DateOver;    //是否开始
                isOver = DateTime.Now > exam.Exam_DateOver;   //是否结束
                startTime = exam.Exam_Date <= DateTime.Now ? DateTime.Now : exam.Exam_Date;        //开始时间
                overTime = exam.Exam_DateOver.AddMinutes(exam.Exam_Span);     //结束时间
                isSubmit = exr != null ? exr.Exr_IsSubmit : false;    //是否交卷               
                if (exr != null && !string.IsNullOrWhiteSpace(exr.Exr_Results))
                {
                    XmlDocument resXml = new XmlDocument();
                    resXml.LoadXml(exr.Exr_Results, false);
                    XmlNode xn = resXml.LastChild;
                    //考试的开始与结束时间，防止学员刷新考试界面，导致时间重置
                    long lbegin, lover;
                    long.TryParse(xn.Attributes["begin"] != null ? xn.Attributes["begin"].Value : "0", out lbegin);
                    long.TryParse(xn.Attributes["overtime"] != null ? xn.Attributes["overtime"].Value : "0", out lover);
                    lbegin = lbegin * 10000;
                    lover = lover * 10000;
                    DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                    DateTime beginTime = dtStart.Add(new TimeSpan(lbegin));
                    overTime = dtStart.Add(new TimeSpan(lover));    //得到转换后的结束时间
                    startTime = exam.Exam_Date <= beginTime ? beginTime : exam.Exam_Date;        //开始时间
                    isOver = DateTime.Now > overTime;
                    isSubmit = DateTime.Now > overTime || DateTime.Now > exr.Exr_OverTime || exr.Exr_IsSubmit; //是否交卷
                }
            }
            jo.Add("isstart", isStart);
            jo.Add("isover", isOver);
            jo.Add("startTime", WeiSha.Core.Server.getTime(startTime));
            jo.Add("overTime", WeiSha.Core.Server.getTime(overTime));
            jo.Add("issubmit", isSubmit);
            //正在考试
            bool doing = startTime <= DateTime.Now && overTime > DateTime.Now && !isSubmit;
            jo.Add("doing", doing && !isOver && isAllow);
            //答题详情
            jo.Add("result", _resultJson(exr));
            return jo;
        }
        /// <summary>
        /// 根据考试答题记录的ID，获取详细的答题信息
        /// </summary>
        /// <param name="exrid">考试成绩记录的ID</param>
        /// <returns></returns>
        public JObject ResultJson(int exrid)
        {
            Song.Entities.ExamResults exr = Business.Do<IExamination>().ResultSingle(exrid);
            return _resultJson(exr);
        }
        private JObject _resultJson(Song.Entities.ExamResults exr)
        {
            JObject jo = new JObject();
            if (exr == null || string.IsNullOrWhiteSpace(exr.Exr_Results)) return jo;
            XmlDocument resXml = new XmlDocument();
            resXml.XmlResolver = null;
            resXml.LoadXml(exr.Exr_Results, false);
            //           
            XmlNode root = resXml.LastChild;
            foreach (XmlAttribute attr in root.Attributes)
                jo.Add(attr.Name, attr.Value);
            JArray qgroup = new JArray();
            XmlNodeList quesGroupNodes = resXml.GetElementsByTagName("ques");
            for (int i = 0; i < quesGroupNodes.Count; i++)
            {
                XmlNode node = quesGroupNodes[i];
                JObject ques = new JObject();
                int type = node.GetAttr<int>("type");
                ques.Add("type", type);
                ques.Add("byname", node.GetAttr("byname"));
                ques.Add("count", node.GetAttr<float>("count"));
                ques.Add("number", node.GetAttr<float>("number"));
                JArray qarray = new JArray();
                for (int n = 0; n < node.ChildNodes.Count; n++)
                {
                    JObject jq = new JObject();
                    XmlNode q = node.ChildNodes[n];
                    jq.Add("id", q.GetAttr("id"));
                    string ans = string.Empty, file = string.Empty;
                    //如果是单选、多选、判断
                    if (type == 1 || type == 2 || type == 3) ans = q.GetAttr("ans");
                    if (type == 4 || type == 5) ans = q.InnerText;
                    if (type == 5) file = q.GetAttr("file");
                    ans = Html.ClearHTML(ans);
                    jq.Add("ans", ans);
                    if (q.Attributes["file"] != null) jq.Add("file", file);
                    jq.Add("num", q.GetAttr("num"));
                    qarray.Add(jq);
                }
                ques.Add("q", qarray);
                qgroup.Add(ques);
            }
            jo.Add("ques", qgroup);
            return jo;
        }
        /// <summary>
        /// 出卷
        /// </summary>
        /// <param name="examid">考试id</param>
        /// <param name="tpid">试卷id</param>
        /// <param name="stid">学员id</param>
        /// <returns></returns>
        [HttpGet]
        public JArray MakeoutPaper(long examid, long tpid,int stid)
        {
            Examination exam = Business.Do<IExamination>().ExamSingle(examid);
            if (exam == null || (exam.Exam_IsUse == false || exam.Exam_IsDeleted)) throw new Exception("当前考试不存在");
            //获取答题信息
            Song.Entities.ExamResults exr = Business.Do<IExamination>().ResultForCache(examid, tpid, stid);           
            if (exr != null && exr.Exr_IsSubmit) throw new Exception("已经交过卷");

            //试卷的试题，如果已经答题，则从答题信息中生成；如果没有答题，则随机生成
            Dictionary<TestPaperItem, List<Questions>> dics = null;
            if (exam.Exam_Purpose == 0)
            {
                //课程试卷
                if (exr != null) dics = Business.Do<ITestPaper>().Putout(exr.Exr_Results, false);
                else dics = Business.Do<ITestPaper>().Putout(tpid, false);
            }
            else
            {
                //考试试卷
                if (exr != null) dics = Business.Do<IExamTestPaper>().Putout(exr.Exr_Results, false);
                else dics = Business.Do<IExamTestPaper>().Putout(tpid, false);
            }
            //
            JArray jarr = new JArray();
            foreach (var di in dics)
            {
                //按题型输出
                Song.Entities.TestPaperItem pi = (Song.Entities.TestPaperItem)di.Key;   //试题类型                
                List<Questions> questions = (List<Questions>)di.Value;   //当前类型的试题   
                if (questions.Count < 1) continue;
                JObject jo = new JObject();
                jo.Add("type", (int)pi.TPI_Type);       //试题类型
                jo.Add("byname", pi.TPI_TypeName);
                jo.Add("count", questions.Count);       //试题数目
                jo.Add("number", (float)pi.TPI_Number); //占用多少分
                JArray ques = new JArray();
                foreach (Song.Entities.Questions q in questions)
                {
                    ques.Add(q.ToJObject("", "Qus_CrtTime,Qus_LastTime"));
                }
                jo.Add("ques", ques);
                jarr.Add(jo);
            }
            return jarr; 
        }
        /// <summary>
        /// 提交考试答题信息
        /// </summary>
        /// <param name="xml">答题信息,xml格式</param>
        /// <param name="async">是否异步计算成绩，true后台异步计算，false为立即计算（并返回成绩值）</param>
        /// <returns>examid:考试id; exrid:成绩记录id; score:成绩得分（如果实时计算的话）; resubmit:是否重复提交</returns>
        [HttpPost, HttpPut]
        [HtmlClear(Not = "xml")]
        public JObject SubmitResult(string xml, bool async)
        {
            JObject jo = new JObject();
            XmlDocument resXml = new XmlDocument();
            resXml.LoadXml(xml, false);
            //遍历试题答题内容
            XmlNodeList quesnodes = resXml.GetElementsByTagName("ques");
            if (quesnodes.Count < 1) return jo;
            foreach (XmlNode ques in quesnodes)
            {
                int type = ques.GetAttr<int>("type");
                //填空和简答,清理冗余html标签
                if (type == 4 || type == 5)
                {
                    foreach (XmlNode q in ques.ChildNodes)
                        q.InnerText = Html.ClearHTML(q.InnerText);

                }
            }
            XmlNode xn = getAttrBase64(resXml.SelectSingleNode("results"));
            //试卷id，考试id      
            long examid = xn.GetAttr<long>("examid");

            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            ////考试开始时间,结束时间
            //DateTime beginTime = xn.Attributes["begin"]?.Value?.ConvertNullable<DateTime>() ?? dtStart;
            //DateTime overTime = xn.Attributes["overtime"]?.Value?.ConvertNullable<DateTime>() ?? dtStart;           

            //提交方式，1为自动提交，2为交卷
            int patter = xn.GetAttr<int>("patter", 1);
            //
            Song.Entities.Examination exam = Business.Do<IExamination>().ExamSingle(examid);
            //如果考试不存在
            if (exam == null) throw new Exception("当前考试不存在！");
            //如果考试已经结束
            int span = (int)exam.Exam_Span;

            Song.Entities.ExamResults exr = new ExamResults();
            exr.Exr_IsSubmit = patter == 2;
            exr.Exam_ID = xn.GetAttr<long>("examid");    //考试id
            exr.Exam_Name = exam.Exam_Name;
            exr.Tp_Id = xn.GetAttr<long>("tpid");       //试卷id
            exr.Ac_ID = xn.GetAttr<int>("stid");        //学员id
            exr.Ac_Name = xn.GetAttr("stname");         //学员名称
            exr.Sts_ID = xn.GetAttr<long>("stsid");     //学员分组id
            exr.Ac_Gender = xn.GetAttr<int>("stsex");   //性别
            exr.Ac_IDCardNumber = xn.GetAttr("stcardid");   //学员身份证号
            exr.Sbj_ID = xn.GetAttr<long>("sbjid", 0);     //专业id
            exr.Sbj_Name = xn.GetAttr("sbjname");
            exr.Exr_IP = WeiSha.Core.Browser.IP;
            exr.Exr_Mac = WeiSha.Core.Request.UniqueID();   //原本是网卡的mac地址,此处不再记录
            exr.Exr_Results = resXml.OuterXml;
            //UID与考试主题
            exr.Exam_UID = xn.GetAttr("uid");
            exr.Exam_Title = xn.GetAttr("theme");
            //exr.Exr_IsSubmit = patter == 2;
            if (exr.Exr_IsSubmit) exr.Exr_SubmitTime = DateTime.Now;
            exr.Exr_OverTime = xn.GetAttr<DateTime>("overtime");
            exr.Exr_CrtTime = xn.GetAttr<DateTime>("starttime");
            exr.Exr_LastTime = DateTime.Now;
            //
            exr.Org_ID = exam.Org_ID;
            exr.Org_Name = exam.Org_Name;

            //缓存当前答题信息

            Business.Do<IExamination>().ResultCacheUpdate(exr, -1);
            //if (patter == 1) return jo;
            exr = Business.Do<IExamination>().ResultSubmit(exr);
            //是否重复提交
            jo.Add("resubmit", exr.Exr_IsCalc);
            //如果是手动提交，且没有计算成绩的，此处计算成绩
            double score = -1;
            if (exr.Exr_IsSubmit && !exr.Exr_IsCalc)
            {
                //异步计算成绩
                if (async)
                {
                    //后台异步计算
                    Exam_Calc handler = new Exam_Calc(exr);
                    System.Threading.Tasks.Task task = new System.Threading.Tasks.Task(handler.Calc);
                    task.Start();
                }
                else
                {
                    //实时计算成绩
                    Business.Do<IExamination>().ResultClacScore(exr);
                }
            }
            if (exr.Exr_IsCalc) score = exr.Exr_ScoreFinal;
            jo.Add("examid", exr.Exam_ID);
            jo.Add("exrid", exr.Exr_ID);
            jo.Add("score", score);
            jo.Add("async", async);

            return jo;
        }
        /// <summary>
        /// 将属性进行Base64解码
        /// </summary>
        /// <param name="xn"></param>
        /// <returns></returns>
        private XmlNode getAttrBase64(XmlNode xn)
        {
            foreach (XmlAttribute attr in xn.Attributes)
            {
                string val = WeiSha.Core.DataConvert.DecryptForBase64(attr.Value);
                val = val.Replace("<", "＜");
                val = val.Replace(">", "＞");
                val = val.Replace("(", "（");
                val = val.Replace(")", "）");
                val = val.Replace("&", "＆");
                val = val.Replace("=", "〓");
                val = val.Replace("\"", "＂");
                val = val.Replace("'", "｀");
                val = val.Replace("\\", "＼");
                attr.Value = val;
            }
            return xn;
        }
        #endregion

        #region 文件上传管理
        /// <summary>
        /// 考试中上传附件，用于简答题的答题信息中的附件文件
        /// </summary>
        /// <param name="stid">学员id</param>
        /// <param name="examid">考试id</param>
        /// <param name="qid">试题id</param>
        /// <returns>qid:试题id;state:是否成功;name:文件名; url:文件完整路径</returns>
        [HttpPost]
        [Student]
        [Upload]
        public JObject FileUp(int stid, long examid, long qid)
        {
            if (stid <= 0 || examid <= 0 || qid <= 0) throw new Exception("参数错误！");
            Song.Entities.Accounts st = this.User;
            if (st==null || st.Ac_ID != stid) throw new Exception("学员未登录！");

            JObject jo = new JObject();
            jo.Add("qid", qid.ToString());

            string filepath = PhyPath + examid + "\\" + stid + "\\";
            //只保存第一个文件
            foreach (string key in this.Files)
            {
                HttpPostedFileBase file = this.Files[key];
                if (!System.IO.Directory.Exists(filepath)) System.IO.Directory.CreateDirectory(filepath);
                //删除原来的文件
                foreach (FileInfo f in new DirectoryInfo(filepath).GetFiles())
                {
                    if (f.Name.IndexOf("_") > 0)
                    {
                        string idtm = f.Name.Substring(0, f.Name.IndexOf("_"));
                        if (idtm == qid.ToString()) f.Delete();
                    }
                }
                string filename = file.FileName;
                //处理文件名长度，由于文件名前面还要加上试题id，防止文件名过长
                int maxLength = 220;
                if (filename.Length > maxLength)
                {
                    if (filename.IndexOf(".") > -1)
                    {
                        string ext = filename.Substring(filename.LastIndexOf("."));
                        string name = filename.Substring(0, filename.LastIndexOf("."));
                        if(name.Length> maxLength - ext.Length)
                        {
                            name = name.Substring(0, maxLength - ext.Length);
                            filename = name + ext;
                        }
                    }
                    else
                    {
                        filename = filename.Substring(0, maxLength);
                    }
                }
                jo.Add("name", filename);
                file.SaveAs(filepath + qid + "_" + filename);
                jo.Add("url", string.Format("{0}{1}/{2}/{3}", VirPath, examid, stid, qid + "_" + filename));
                jo.Add("state", true);
                break;
            }
            return jo;
        }
        /// <summary>
        /// 加载考试中的简答题的答题信息的附件，按试题加载
        /// </summary>
        /// <param name="stid">学员id</param>
        /// <param name="examid">考试id</param>
        /// <param name="qid">试题id</param>
        /// <returns>qid:试题id;state:是否成功;name:文件名; url:文件完整路径</returns>
        public JObject FileLoad(int stid, long examid, long qid)
        {
            JObject jo = new JObject();
            jo.Add("qid", qid.ToString());
            //文件所在路径
            string filepath = PhyPath + examid + "\\" + stid + "\\";
            if (!System.IO.Directory.Exists(filepath))
            {
                jo.Add("state", false);
                return jo;
            }
            string file = string.Empty;
            foreach (FileInfo f in new DirectoryInfo(filepath).GetFiles())
            {
                if (f.Name.IndexOf("_") > 0)
                {
                    string idtm = f.Name.Substring(0, f.Name.IndexOf("_"));
                    if (idtm == qid.ToString())
                    {
                        file = f.Name.Substring(f.Name.IndexOf("_") + 1);
                        jo.Add("name", file);
                        jo.Add("url", string.Format("{0}{1}/{2}/{3}", VirPath, examid, stid, f.Name));
                        jo.Add("state", true);
                    }
                }
            }
            if (string.IsNullOrEmpty(file))           
                jo.Add("state", false);          
            return jo;
        }
        /// <summary>
        /// 删除考试简称题的答题信息的附件
        /// </summary>
        /// <param name="stid">学员id</param>
        /// <param name="examid">考试id</param>
        /// <param name="qid">试题id</param>
        /// <returns>state:是否成功;qid:试题id</returns>
        [Student]
        [HttpDelete]
        public JObject FileDelete(int stid, long examid, long qid)
        {
            JObject jo = new JObject();
            jo.Add("qid", qid.ToString());
            //文件所在路径
            string filepath = PhyPath + examid + "\\" + stid + "\\";
            if (!System.IO.Directory.Exists(filepath))
            {
                jo.Add("state", false);
                return jo;
            }
            //文件名
            string file = "";
            foreach (FileInfo f in new DirectoryInfo(filepath).GetFiles())
            {
                if (f.Name.IndexOf("_") > 0)
                {
                    string idtm = f.Name.Substring(0, f.Name.IndexOf("_"));
                    if (idtm == qid.ToString()) file = f.FullName;
                }
            }
            if (!string.IsNullOrWhiteSpace(file) && System.IO.File.Exists(file))                   
                    System.IO.File.Delete(file);
            jo.Add("state", true);
            return jo;
        }
        #endregion
        
        #region 考试人数统计
        /// <summary>
        /// 某场考试实际参考的人数
        /// </summary>
        /// <param name="examid">考试id</param>
        /// <returns>id:考试id,number:参考人数</returns>
        [HttpGet]
        public JObject AttendCount(long examid)
        {            
            int num = Business.Do<IExamination>().Numbertimes4Exam(examid);
            JObject jo = new JObject();
            jo.Add("id", examid);
            jo.Add("number", num);
            return jo;
        }
        /// <summary>
        /// 某场考试的缺考人数
        /// </summary>
        /// <param name="examid">考试id</param>
        /// <returns>id:考试id,number:参考人数</returns>
        [HttpGet]
        public JObject AbsenceCount(long examid)
        {
            int num = Business.Do<IExamination>().NumberAbsence4Exam(examid);
            JObject jo = new JObject();
            jo.Add("id", examid);
            jo.Add("number", num);
            return jo;
        }
        /// <summary>
        /// 考试主题下的参加考试人数
        /// </summary>
        /// <param name="examid">考试id</param>
        /// <returns>id:考试id,number:参考人数</returns>
        [HttpGet]
        public JObject StudentTotalTheme(long examid)
        {
            int total = Business.Do<IExamination>().NumberOfStudent(examid);
            JObject jo = new JObject();
            jo.Add("id", examid);
            jo.Add("number", total);
            return jo;
        }
        /// <summary>
        /// 考试主题下的参加考试人数
        /// </summary>
        /// <param name="examid">考试id</param>
        /// <returns>id:考试id,number:参考人数</returns>
        [HttpGet]
        public JObject AttendTheme(long examid)
        {
            int total = Business.Do<IExamination>().Number4Theme(examid);
            JObject jo = new JObject();
            jo.Add("id", examid);
            jo.Add("number", total);
            return jo;
        }
        /// <summary>
        /// 考试中的人员数，通过答题缓存的数量计算
        /// </summary>
        /// <param name="examid">考试id</param>
        /// <returns></returns>
        public JObject ExaminingCount(long examid)
        {
            int count = Business.Do<IExamination>().ResultCacheCount(examid);
            JObject jo = new JObject();
            jo.Add("id", examid);
            jo.Add("count", count);
            return jo;
        }
        /// <summary>
        /// 参加考试的所有学员，
        /// </summary>
        /// <param name="examid">考试主题的id</param>
        /// <param name="name">学员姓名</param>
        /// <param name="idcard">身份证号</param>
        /// <param name="stsid">学员组</param>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ListResult AttendThemeAccounts(long examid, string name, string idcard, long stsid, int size,int index)
        {
            int total;
            List<Accounts> datas = Business.Do<IExamination>().AttendThemeAccounts(examid, name, idcard, stsid,size, index, out total);
            ListResult result = new ListResult(datas);
            result.Index = index;
            result.Size = size;
            result.Total = total;
            return result;
        }
        /// <summary>
        /// 缺考的学员列表
        /// </summary>
        /// <param name="examid">考试场次的id</param>
        /// <param name="name">学员姓名</param>
        /// <param name="idcard">身份证号</param>
        /// <param name="phone">手机号</param>
        /// <param name="stsid">学员组</param>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [Admin,Teacher]
        public ListResult AbsenceExamAccounts(long examid, string name, string idcard, string phone, long stsid, int size, int index)
        {
            int total;
            List<Accounts> datas = Business.Do<IExamination>().AbsenceExamAccounts(examid, name, idcard, phone, stsid, size, index, out total);
            for (int i = 0; i < datas.Count; i++)
            {
                datas[i] = Account._tran(datas[i]);
            }
            ListResult result = new ListResult(datas);
            result.Index = index;
            result.Size = size;
            result.Total = total;
            return result;
        }
        #endregion      
    
        #region 得分
        /// <summary>
        /// 某场考试的平均分数
        /// </summary>
        /// <param name="examid">考试id</param>
        /// <returns>id:考试id,average:平均数</returns>
        [HttpGet]
        public JObject Average4Exam(long examid)
        {
            double avg = Business.Do<IExamination>().Avg4Exam(examid);
            avg = Math.Round(Math.Round(avg * 10000) / 10000, 2, MidpointRounding.AwayFromZero);
            JObject jo = new JObject();
            jo.Add("id", examid);
            jo.Add("average", avg);
            return jo;
        }
        /// <summary>
        /// 场次的成绩统计，平均值、最高分、最低分、通过率
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <returns>
        /// 示例："id":场次id,"average":平均值,"highest":最高分,"lowest":最低分,"passrate":及格率
        ///</returns>
        public JObject Score4Exam(long examid)
        {
            double avg = Business.Do<IExamination>().Avg4Exam(examid);
            double hig = Business.Do<IExamination>().Highest4Exam(examid);
            double low = Business.Do<IExamination>().Lowest4Exam(examid);
            double pass = Business.Do<IExamination>().PassRate4Exam(examid);           
            JObject jo = new JObject();
            jo.Add("id", examid);
            jo.Add("average", avg);
            jo.Add("highest", hig);
            jo.Add("lowest", low);
            jo.Add("passrate", pass);  
            return jo;
        }
        #endregion

        #region 考试成绩
        /// <summary>
        /// 当前考试下所有学员的的学员组,并统计参考人员数量
        /// </summary>
        /// <param name="examid">考试主题的id</param>
        /// <returns>Sts_Count列为学员组下的参考人员数量</returns>
        public List<StudentSort> Sort4Theme(long examid)
        {
            return Business.Do<IExamination>().StudentSort4Theme(examid);
        }
        /// <summary>
        /// 当前场次下的所有学员的学员组,并统计参考人员数量
        /// </summary>
        /// <param name="examid"></param>
        /// <returns>Sts_Count列为学员组下的参考人员数量</returns>
        public List<StudentSort> ResultSort4Exam(long examid)
        {
            return Business.Do<IExamination>().ResultSort4Exam(examid);
        }
        /// <summary>
        /// 未参加考试的学员的学员组
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <returns></returns>
        public List<StudentSort> AbsenceSort4Exam(long examid)
        {
            return Business.Do<IExamination>().AbsenceSort4Exam(examid);
        }
        /// <summary>
        /// 当前考试主题下的所有成绩，包括各个场次
        /// </summary>
        /// <param name="examid">考试主题的id</param>
        /// <param name="sort">分组id</param>
        /// <returns></returns>
        public DataTable Results(long examid,int sort)
        {
            DataTable dt = null;    //数据源  
            Song.Entities.Examination theme = Business.Do<IExamination>().ExamSingle(examid);
            if (theme == null) return dt;
            string stsid = "";
            switch (sort)
            {
                //所有学员
                case 0:
                    //当前考试限定的学生分组
                    List<StudentSort> sts = Business.Do<IExamination>().ScopeForStudentSort(theme.Exam_UID);
                    //如果没有设定分组，则取当前参加考试的学员的分组
                    if (sts == null || sts.Count < 1) sts = Business.Do<IExamination>().StudentSort4Theme(examid);
                    foreach (Song.Entities.StudentSort ss in sts)
                        stsid += ss.Sts_ID + ",";
                    stsid += "-1";
                    dt = Business.Do<IExamination>().Result4Theme(examid, stsid);
                    break;
                //未分组学员
                case -1:
                    dt = Business.Do<IExamination>().Result4Theme(examid, -1);
                    break;
                //当前分组学员
                default:
                    dt = Business.Do<IExamination>().Result4Theme(examid, sort);
                    break;
            }
            return dt;
        }
        /// <summary>
        /// 通过考试成绩的id，获取成绩信息
        /// </summary>
        /// <param name="id">考试成绩记录的id</param>
        /// <returns></returns>
        public ExamResults ResultForID(int id)
        {
            return Business.Do<IExamination>().ResultSingle(id);
        }
        /// <summary>
        /// 保存考试成绩，用于教师批阅后的保存
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        [Teacher,Admin]
        [HttpPost]
        [HtmlClear(Not = "result")]
        public bool ResultModify(ExamResults result)
        {
            ExamResults exr=Business.Do<IExamination>().ResultSingle(result.Exr_ID);
            if (exr == null) return false;
            exr.Copy<Song.Entities.ExamResults>(result);
            Business.Do<IExamination>().ResultSave(exr);
            return true;
        }
        /// <summary>
        /// 获取成绩记录
        /// </summary>
        /// <param name="examid">考试id</param>
        /// <param name="tpid">试卷id</param>
        /// <param name="stid">学员id</param>
        /// <returns></returns>
        public ExamResults Result(long examid,long  tpid,int stid)
        {
            Song.Entities.ExamResults exr = Business.Do<IExamination>().ResultForCache(examid, tpid, stid);
            //if(exr==null) exr= Business.Do<IExamination>().ResultSingle(examid, tpid, stid);
            return exr;
        }
        /// <summary>
        /// 学员在某个考试场次的得分
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <param name="acid">学员id</param>
        /// <returns>"examid":场次id,"acid":学员id,"score":得分,如果没有考试，返回-1,"testpaper":试题id,"exrid":成绩记录的id,</returns>
        public JObject ResultScore(int acid, long examid)
        {
            JObject jo = new JObject();
            jo.Add("examid",examid.ToString());
            jo.Add("acid", acid.ToString());

            ExamResults result = Business.Do<IExamination>().ResultSingle(acid, examid);
            if (result == null)
            {
                jo.Add("score", -1);
                return jo;
            }
            //考试成绩
            double score = result.Exr_ScoreFinal;
            score = Math.Round(Math.Round(score * 10000) / 10000, 2, MidpointRounding.AwayFromZero);
            jo.Add("score", score);
            jo.Add("testpaper", result.Tp_Id.ToString());
            jo.Add("exrid", result.Exr_ID.ToString());
            return jo;
        }
        /// <summary>
        /// 考试成绩回顾
        /// </summary>
        /// <param name="id">考试成绩记录的id</param>
        /// <returns></returns>
        public ExamResults ResultReview(int id)
        {
            //成绩记录
            ExamResults result = Business.Do<IExamination>().ResultSingle(id);
            if (result == null) throw new Exception("未找到成绩记录");
            //加载答题记录
            XmlDocument resXml = new XmlDocument();
            //考试信息
            Song.Entities.Examination exam = Business.Do<IExamination>().ExamSingle(result.Exam_ID);
            if (exam == null) return result;
            List<TestPaperItem> paperitems = exam.Exam_Purpose == 0 ? Business.Do<ITestPaper>().PaperItems(exam.Tp_Id) : Business.Do<IExamTestPaper>().PaperItems(exam.Etp_Id);
            resXml.LoadXml(result.Exr_Results, false);
            XmlNode results = resXml.SelectSingleNode("results");
            foreach (TestPaperItem item in paperitems)
            {
                XmlNode xn = results.SelectSingleNode($"ques[@type='{item.TPI_Type}']");
                if (xn == null) continue;
                //填空和简答,清理冗余html标签
                if (item.TPI_Type == 4 || item.TPI_Type == 5)
                {
                    foreach (XmlNode q in xn.ChildNodes)
                        q.InnerText = Html.ClearHTML(q.InnerText);

                }
                xn.SetAttr("byname", item.TPI_TypeName);
            }
            result.Exr_Results = resXml.InnerXml;
            //判断开始时间与结束时间，是否考试结束等
            bool isOver;
            //判断是否已经开始、是否已经结束
            if (exam.Exam_DateType == 1)
            {
                //固定时间开始               
                isOver = DateTime.Now > exam.Exam_Date.AddMinutes(exam.Exam_Span);   //是否结束
            }
            else
            {
                return result;
                isOver = DateTime.Now > exam.Exam_DateOver;   //是否结束                         
                if (result != null && !string.IsNullOrWhiteSpace(result.Exr_Results))
                {
                    XmlNode xn = resXml.LastChild;
                    //考试的开始与结束时间，防止学员刷新考试界面，导致时间重置
                    long lover;
                    long.TryParse(xn.Attributes["overtime"] != null ? xn.Attributes["overtime"].Value : "0", out lover);
                    lover = lover * 10000;
                    DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
                    DateTime overTime = dtStart.Add(new TimeSpan(lover));    //得到转换后的结束时间
                    isOver = DateTime.Now > overTime;                    
                }
            }
            if(!isOver) throw new Exception("考试时间尚未结束，为防止泄题，请稍后查看回顾");
           
            return result;
        }

        /// <summary>
        /// 获取考试成绩,某场考试的成绩
        /// </summary>
        /// <param name="examid">考试id</param>
        /// <param name="name">学员姓名</param>
        /// <param name="idcard">学员身份证号</param>
        /// <param name="stsid">学员组的id</param>
        /// <param name="min">按分数区间获取记录，此处是最低分</param>
        /// <param name="max">最高分</param>
        /// <param name="manual">是否批阅</param>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        [Admin,Teacher]
        public ListResult Result4Exam(long examid, string name, string idcard, long stsid, float min, float max, bool? manual, int size, int index)
        {
            int count;
            List<ExamResults> datas = Business.Do<IExamination>().ResultsPager(examid, name, idcard, stsid, min, max, manual, size, index, out count);
            ListResult result = new ListResult(datas);
            result.Index = index;
            result.Size = size;
            result.Total = count;
            return result;
        }
        /// <summary>
        /// 某个学员的考试成绩
        /// </summary>
        /// <param name="acid">学员id</param>
        /// <param name="orgid">机构id</param>
        /// <param name="sbjid">专业id</param>
        /// <param name="search">考试场次的标题</param>      
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public ListResult Result4Student(int acid, int orgid, long sbjid, string search, int size, int index)
        {
            int count;
            List<ExamResults> datas = null;
            datas = Business.Do<IExamination>().GetAttendPager(acid, sbjid, orgid, search, size, index, out count);
            ListResult result = new ListResult(datas);
            result.Index = index;
            result.Size = size;
            result.Total = count;
            return result;
        }
        /// <summary>
        /// 删除考试成绩，按学员id与考试id
        /// </summary>
        /// <param name="acid">学员id,可以为多个</param>
        /// <param name="examid">考试id</param>
        /// <returns></returns>
        [HttpDelete]
        public int ResultDelete4Acc(string acid, long examid)
        {
            int i = 0;
            if (string.IsNullOrWhiteSpace(acid)) return i;
            List<int> list = acid.ToList<int>();
            foreach (int s in list)
                i += Business.Do<IExamination>().ResultDelete(s, examid);
            return i;
        }
        /// <summary>
        /// 删除考试成绩，按成绩记录的id
        /// </summary>
        /// <param name="exrid">成绩记录的id,可以为多个</param>
        /// <returns></returns>
        [HttpDelete]
        public int ResultDelete(string exrid)
        {
            int i = 0;
            if (string.IsNullOrWhiteSpace(exrid)) return i;
            List<int> list = exrid.ToList<int>();
            foreach (int s in list)
                i += Business.Do<IExamination>().ResultDelete(s);
            return i;
        }
        /// <summary>
        /// 删除考试下的所有成绩
        /// </summary>
        /// <param name="examid">考试id</param>
        /// <returns></returns>
        [HttpDelete]
        public bool ResultClear(long examid)
        {
            Business.Do<IExamination>().ResultClear(examid);
            return true;
        }
        /// <summary>
        /// 计算考试成绩，通过考试成绩的记录计算，一般用于重新计算
        /// </summary>
        /// <param name="exrid">考试记录的id</param>
        /// <returns></returns>
        [Admin,Teacher]
        public float ResultClacScore(int exrid)
        {
            ExamResults exr = Business.Do<IExamination>().ResultSingle(exrid);
            if (exr == null) return 0;
            exr = Business.Do<IExamination>().ResultClacScore(exr);
            if (exr == null) return 0;
            return exr.Exr_ScoreFinal;
        }
        /// <summary>
        /// 设置考试成绩
        /// </summary>
        /// <param name="exrid">考试成绩记录的id</param>
        /// <param name="score">得分</param>
        /// <param name="time">开始时间</param>
        /// <param name="dura">用时</param>
        [Admin]
        public ExamResults ResultSetScore(int exrid, float score, DateTime? time, int dura)
        {
            ExamResults exr = Business.Do<IExamination>().ResultSingle(exrid);
            if (exr == null) throw new Exception("考试成绩不存在");
            exr = Business.Do<IExamination>().ResultSetScore(exr, score, time, dura);
            return exr;
        }
        /// <summary>
        /// 创建考试成绩
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <param name="acid">学员账号id</param>
        /// <param name="score">得分</param>
        /// <param name="time">开始时间</param>
        /// <param name="dura">用时</param>
        [Admin]
        public ExamResults ResultCreateScore(long examid,int acid, float score, DateTime? time, int dura)
        {
            return Business.Do<IExamination>().ResultSetScore(examid, acid, score, time, dura);
        }
        /// <summary>
        /// 创建缺考学员的成绩
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <param name="minScore">最少得分</param>
        /// <param name="maxScore">最高得分</param>
        /// <param name="minTime">最早考试时间</param>
        /// <param name="maxTime">最晚考试时间</param>
        /// <param name="minSpan">最短考试用时</param>
        /// <param name="maxSpan">最长考试用时</param>
        /// <returns></returns>
        [Admin]
        public JObject ResultAbsenceBatchScore(long examid, int minScore, int maxScore, DateTime minTime, DateTime maxTime, int minSpan, int maxSpan)
        {
            var task= Business.Do<IExamination>().ResultAbsenceBatchScore(examid, minScore, maxScore, minTime, maxTime, minSpan, maxSpan);
            JObject jo = new JObject();
            jo.Add("total",task.Item1);
            jo.Add("count", task.Item2);
            return jo;
        }
        /// <summary>
        /// 创建缺考学员的成绩的进度
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        [Admin]
        public JObject ResultAbsenceBatchScoreLoading(long examid)
        {
            var task = Business.Do<IExamination>().ResultAbsenceBatchScore(examid);
            JObject jo = new JObject();
            jo.Add("total", task.Item1);
            jo.Add("count", task.Item2);
            return jo;
        }
        /// <summary>
        /// 批量计算考试成绩
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <returns></returns>
        public bool ResultBatchClac(long examid)
        {
            return Business.Do<IExamination>().ResultBatchClac(examid);
        }
        #endregion

        #region 我的考试
        /// <summary>
        /// 学员今天以及之后的考试，过期的不再显示
        /// </summary>
        /// <param name="acid">学员ID</param>
        /// <param name="search">考试检索</param>
        /// <param name="size">每次几条</param>
        /// <param name="index">第几页</param>
        /// <returns>考试场次，而非考试主题</returns>
        public ListResult SelfExam4Todaylate(int acid, string search, int size, int index)
        {
            int count;
            DateTime start = DateTime.Now.Date;
            List<Song.Entities.Examination> todaylate = Business.Do<IExamination>().GetSelfExam(acid, start, null, search, size, index, out count);
            ListResult result = new ListResult(todaylate);
            result.Index = index;
            result.Size = size;
            result.Total = count;
            return result;
        }        
        #endregion

        #region 导出考试成绩
        private static string outputPath = "ExamresultToExcel";
        /// <summary>
        /// 某场考试的考试成绩按学员组导出
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <param name="sorts">学员组的id，多个id用逗号分隔</param>
        /// <returns></returns>
        [HttpPost]
        public JObject ResultsExport4Eaxm(long examid,string sorts)
        {           
            List<long> sortsid = sorts.ToList<long>();
            //导出文件的位置
            string rootpath = WeiSha.Core.Upload.Get["Temp"].Physics + outputPath + "\\" + examid + "\\";
            if (!System.IO.Directory.Exists(rootpath))
                System.IO.Directory.CreateDirectory(rootpath);

            DateTime date = DateTime.Now;
            string filename = string.Format("Results.{0}.({1}).xls", examid, date.ToString("yyyy-MM-dd HH-mm-ss"));
            string filePath = rootpath + filename;
            filePath = Business.Do<IExamination>().ExportResults4Exam(filePath, examid, sortsid.ToArray<long>());
            JObject jo = new JObject();
            jo.Add("file", filename);
            jo.Add("url", string.Format("{0}/{1}/{2}", WeiSha.Core.Upload.Get["Temp"].Virtual + outputPath, examid, filename));
            jo.Add("date", date);
            return jo;
        }
        /// <summary>
        /// 考试主题下的所有成绩，按学员组分成工作表
        /// </summary>
        /// <param name="examid">考试主题的id</param>
        /// <param name="sorts">学员组的id</param>
        /// <returns></returns>
        [HttpPost]
        public JObject ResultsExport4Theme(long examid, string sorts)
        {
            List<long> sortsid = sorts.ToList<long>();
            //导出文件的位置
            string rootpath = WeiSha.Core.Upload.Get["Temp"].Physics + outputPath + "\\" + examid + "\\";
            if (!System.IO.Directory.Exists(rootpath))
                System.IO.Directory.CreateDirectory(rootpath);

            DateTime date = DateTime.Now;
            string filename = string.Format("Results.{0}.({1}).xls", examid, date.ToString("yyyy-MM-dd HH-mm-ss"));
            string filePath = rootpath + filename;
            filePath = Business.Do<IExamination>().ExportResults4Theme(filePath, examid, sortsid.ToArray<long>());
            JObject jo = new JObject();
            jo.Add("file", filename);
            jo.Add("url", string.Format("{0}/{1}/{2}", WeiSha.Core.Upload.Get["Temp"].Virtual + outputPath, examid, filename));
            jo.Add("date", date);
            return jo;
        }
        /// <summary>
        /// 导出某场考试的缺考人员
        /// </summary>
        /// <param name="examid">考试主题的id</param>
        /// <returns></returns>
        [HttpPost]
        public JObject ExportAbsences4Exam(long examid)
        {
            //导出文件的位置
            string rootpath = WeiSha.Core.Upload.Get["Temp"].Physics + outputPath + "\\" + examid + "\\";
            if (!System.IO.Directory.Exists(rootpath))
                System.IO.Directory.CreateDirectory(rootpath);

            DateTime date = DateTime.Now;
            string filename = string.Format("Absences.{0}.({1}).xls", examid, date.ToString("yyyy-MM-dd HH-mm-ss"));
            string filePath = rootpath + filename;
            filePath = Business.Do<IExamination>().ExportAbsences4Exam(filePath, examid);
            JObject jo = new JObject();
            jo.Add("file", filename);
            jo.Add("url", string.Format("{0}/{1}/{2}", WeiSha.Core.Upload.Get["Temp"].Virtual + outputPath, examid, filename));
            jo.Add("date", date);
            return jo;
        }
        /// <summary>
        /// 删除Excel文件
        /// </summary>
        /// <param name="examid"></param>
        /// <param name="filename">文件名，带后缀名，不带路径</param>
        /// <returns></returns>
        [HttpDelete]
        public bool ExcelDelete(long examid, string filename)
        {
            return Song.ViewData.Helper.Excel.DeleteFile(filename, outputPath + "\\" + examid, "Temp");
        }
        /// <summary>
        /// 获取已经生成的成绩导出的Excel文件
        /// </summary>
        /// <param name="examid">考试的id，不分考试主题与场次</param>
        /// <returns>file:文件名,url:下载地址,date:创建时间</returns>
        public JArray ExcelResultsFiles(long examid) => _excelFiles(examid, "Results");
        /// <summary>
        ///  获取已经生成的缺考学员的Excel文件
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        public JArray ExcelAbsencesFiles(long examid) => _excelFiles(examid, "Absences");
        /// <summary>
        /// 获取Excel文件
        /// </summary>
        /// <param name="examid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private JArray _excelFiles(long examid,string type)
        {
            string rootpath = WeiSha.Core.Upload.Get["Temp"].Physics + outputPath + "\\" + examid + "\\";
            if (!System.IO.Directory.Exists(rootpath))
                System.IO.Directory.CreateDirectory(rootpath);
            JArray jarr = new JArray();
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(rootpath);
            FileInfo[] files = dir.GetFiles("*.xls").OrderByDescending(f => f.CreationTime).ToArray();
            foreach (System.IO.FileInfo f in files)
            {
                if (!f.Name.StartsWith(type)) continue;
                JObject jo = new JObject();
                jo.Add("file", f.Name);
                jo.Add("url", string.Format("{0}/{1}/{2}", WeiSha.Core.Upload.Get["Temp"].Virtual + outputPath, examid, f.Name));
                jo.Add("date", f.CreationTime);
                jo.Add("type", Path.GetExtension(f.Name).TrimStart('.'));
                jo.Add("size", f.Length);
                jarr.Add(jo);
            }
            return jarr;
        }
        #endregion
    }

    /// <summary>
    /// 考试成绩计算，用于异步方法
    /// </summary>
    public class Exam_Calc
    {
        public Song.Entities.ExamResults ExamResult { get; set; }
       
        public Exam_Calc(Song.Entities.ExamResults result)
        {
            this.ExamResult = result;
        }
        public void Calc()
        {
            Business.Do<IExamination>().ResultClacScore(this.ExamResult);
        }
    }
}
