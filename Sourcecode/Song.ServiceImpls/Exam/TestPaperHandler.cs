using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Song.Entities;
using System.Reflection;
using Newtonsoft.Json.Linq;
using WeiSha.Core;
using WeiSha.Data;

namespace Song.ServiceImpls.Exam
{
    /// <summary>
    /// 试卷处理类，用于生成试卷
    /// </summary>
    public class TestPaperHandler
    {
        //课程试卷的操作对象
        private static readonly TestPaperCom tpcom = new Song.ServiceImpls.TestPaperCom();       
        //考试试卷的操作对象
        private static readonly ExamTestPaperCom etpcom = new Song.ServiceImpls.ExamTestPaperCom();

        private static readonly SubjectCom sbjcom = new Song.ServiceImpls.SubjectCom();
        private static readonly QuestionsCom quescom = new Song.ServiceImpls.QuestionsCom();
        /// <summary>
        /// 试卷ID
        /// </summary>
        public long TestPaperID { get; set; }        
        /// <summary>
        /// 考试场次的对象
        /// </summary>
        public Examination Exam { get; set; }
        /// <summary>
        /// 生成的试卷的内容，包括试题和得分等
        /// </summary>
        public Dictionary<TestPaperItem, List<Questions>> PagerContents { get; set; }

        /// <summary>
        /// 构建方法
        /// </summary>
        /// <param name="exam"></param>
        /// <param name="tpid"></param>
        public TestPaperHandler(Examination exam)
        {
            this.Exam = exam;
            this.TestPaperID = exam.Exam_Purpose == 0 ? exam.Tp_Id : exam.Etp_Id;                  
        }
        public TestPaperHandler()
        {

        }
        #region 随机生成试卷
        /// <summary>
        /// 生成试卷
        /// </summary>
        /// <param name="examid">考试场次的id</param>
        /// <param name="tpid">试卷id</param>
        /// <returns></returns>
        public static TestPaperHandler Putout(int examid)
        {
            Examination exam = Gateway.Default.From<Examination>().Where(Examination._.Exam_ID == examid).ToFirst<Examination>();
            return Putout(exam);
        }
        /// <summary>
        /// 生成试卷
        /// </summary>
        /// <param name="exam"></param>
        /// <param name="tpid">试卷id</param>
        /// <param name="isanswer">试题是否带答案，模拟考试一般带答案，方便前端计算成绩</param>
        /// <returns></returns>
        public static TestPaperHandler Putout(Examination exam)
        {
            return Putout(exam, false);
        }
        public static TestPaperHandler Putout(Examination exam, bool isanswer)
        {
            TestPaperHandler handler = new TestPaperHandler(exam);
            //生成试卷,分课程试卷和考试试卷
            if (exam.Exam_Purpose==0)
                handler.PagerContents = tpcom.Putout(exam.Tp_Id, isanswer); 
            else
                handler.PagerContents = etpcom.Putout(exam.Etp_Id, isanswer);
            return handler;
        }
        #endregion

        #region 通过答题信息，反向生成试卷
        /// <summary>
        /// 通过答题信息，反向生成试卷
        /// </summary>
        /// <param name="exr">考试答题信息的记录对象</param>
        /// <returns></returns>
        public static TestPaperHandler Putout(ExamResults exr) => Putout(exr.Exr_Results);
        /// <summary>
        /// 通过答题信息，反向生成试卷
        /// </summary>
        /// <param name="tr">测试的答题信息</param>
        /// <returns></returns>
        public static TestPaperHandler Putout(TestResults tr) => Putout(tr.Tr_Results);
        /// <summary>
        /// 通过答题信息，反向生成试卷
        /// </summary>
        /// <param name="results">答题信息的xml文本</param>
        /// <returns></returns>
        public static TestPaperHandler Putout(string results)
        {
            XmlDocument docxml = new XmlDocument();
            docxml.XmlResolver = null;
            docxml.LoadXml(results, false);
            return Putout(docxml);
        }
        /// <summary>
        ///  通过答题信息，反向生成试卷
        /// </summary>
        /// <param name="resxml">答题信息的xml对象</param>
        /// <returns></returns>
        public static TestPaperHandler Putout(XmlDocument resxml)
        {
            TestPaperHandler tp = new TestPaperHandler();
            //生成试卷内容，即试题
            tp.PagerContents = tpcom.Putout(resxml, false);
            return tp;
        }
        #endregion

        #region 输出试卷
        /// <summary>
        /// 输出试卷为Json
        /// </summary>
        /// <returns></returns>
        public JArray ToJson()
        {
            //生成试卷
            Dictionary<TestPaperItem, List<Questions>> dics = this.PagerContents;
            JArray jarr = new JArray();
            foreach (var di in dics)
            {
                //按题型输出
                Song.Entities.TestPaperItem pi = (Song.Entities.TestPaperItem)di.Key;   //试题类型                
                List<Questions> questions = (List<Questions>)di.Value;   //当前类型的试题
                if (questions.Count < 1) continue;
                JObject jo = new JObject();
                jo.Add("type", pi.TPI_Type);     //试题类型
                jo.Add("byname", pi.TPI_TypeName);  //题型的名称
                jo.Add("count", questions.Count);   //试题数目
                jo.Add("number", pi.TPI_Number);     //占用多少分                
                JArray ques = new JArray();
                foreach (Song.Entities.Questions q in questions)
                {
                    string json = q.ToJson("", "Qus_CrtTime,Qus_LastTime");
                    ques.Add(JObject.Parse(json));
                }
                jo.Add("ques", ques);
                jarr.Add(jo);
            }
            return jarr;
        }
        /// <summary>
        /// 输出试卷为xml，答题状态
        /// </summary>
        /// <returns></returns>
        public XmlDocument ToXml()
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(xmlDeclaration);
            //创建根节点
            XmlElement root = doc.CreateElement("results");
            root.SetAttribute("tpid", this.TestPaperID.ToString());   //试卷ID
            //如果是程序试卷，则添加专业信息
            if (this.Exam.Exam_Purpose == 0)
            {
                long sbjid = 0;
                string sbjname = "";
                TestPaper tp = tpcom.PaperSingle(Exam.Tp_Id);
                if (tp != null)
                {
                    //专业信息
                    Subject subject = sbjcom.SubjectSingle(tp.Sbj_ID);
                    if (subject != null)
                    {
                        sbjid = subject.Sbj_ID;
                        sbjname = subject.Sbj_Name;
                    }
                }
                root.SetAttribute("sbjid", sbjid.ToString());   //专业id
                root.SetAttribute("sbjname", sbjname);   //专业名称
            }

            doc.AppendChild(root);
            Dictionary<TestPaperItem, List<Questions>> dics = this.PagerContents;
            foreach (var di in dics)
            {
                //按题型输出
                Song.Entities.TestPaperItem pi = (Song.Entities.TestPaperItem)di.Key;   //试题类型
                List<Questions> questions = (List<Questions>)di.Value;   //当前类型的试题
                XmlElement elques = doc.CreateElement("ques");
                elques.SetAttribute("type", pi.TPI_Type.ToString());
                elques.SetAttribute("count", questions.Count.ToString());
                elques.SetAttribute("number", pi.TPI_Number.ToString());
                elques.SetAttribute("byname", pi.TPI_TypeName);
                //
                foreach (Song.Entities.Questions q in questions)
                {
                    XmlElement elq = doc.CreateElement("q");
                    elq.SetAttribute("id", q.Qus_ID.ToString());
                    elq.SetAttribute("num", q.Qus_Number.ToString());
                    if (q.Qus_Type == 4 || q.Qus_Type == 5)
                    {
                        elq.InnerText = string.Empty;
                    }
                    else elq.SetAttribute("ans", "");
                    elq.SetAttribute("file", "");
                    elq.SetAttribute("sucess", "false");
                    elq.SetAttribute("score", "0");
                    elques.AppendChild(elq);
                }
                root.AppendChild(elques);
            }
            return doc;
        }
        /// <summary>
        /// 根据试卷生成答题的xml解析对象
        /// </summary>
        /// <returns></returns>
        public Results ToResults()
        {
            XmlDocument doc = this.ToXml();
            Results results = new Results(doc.OuterXml);
            return results;
        }
        /// <summary>
        /// 根据试卷生成答题的xml解析对象
        /// </summary>
        /// <returns></returns>
        public Results ToResults(Examination exam, Accounts acc)
        {
            XmlDocument doc = this.ToXml();
            Results results = new Results(doc.OuterXml);
            results.Examid = exam.Exam_ID;
            results.ExamUid = exam.Exam_UID;
            results.ExamTheme = exam.Exam_Title;    //考试主题
            //学员信息
            results.AccountID = acc.Ac_ID;
            results.AccountName = acc.Ac_Name;
            results.IDCardNumber = acc.Ac_IDCardNumber;
            results.Gender = acc.Ac_Gender;
            results.SortID = acc.Sts_ID;
            
            return results;
        }
        /// <summary>
        /// 根据试卷生成答题的xml解析对象
        /// </summary>
        /// <returns></returns>
        public Results ToResults(Accounts acc)
        {
            return ToResults(this.Exam, acc);
        }
        #endregion
    }
}
