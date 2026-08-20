using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Song.Entities;
using WeiSha.Data;
using System.Xml;

namespace Song.ServiceInterfaces
{
    /// <summary>
    /// 考试专用试卷
    /// </summary>
    public interface IExamTestPaper : WeiSha.Core.IBusinessInterface
    {
        #region 试卷管理
        /// <summary>
        /// 添加试卷
        /// </summary>
        /// <param name="entity">试卷对象</param>
        long PaperAdd(ExamTestPaper entity);
        /// <summary>
        /// 修改试卷
        /// </summary>
        /// <param name="entity">业务实体</param>
        void PaperSave(ExamTestPaper entity);
        /// <summary>
        /// 修改试卷的某些项
        /// </summary>
        /// <param name="etpid">试卷的id</param>
        /// <param name="fiels"></param>
        /// <param name="objs"></param>
        /// <returns></returns>
        bool PaperUpdate(long etpid, Field[] fiels, object[] objs);
        /// <summary>
        /// 删除试卷，按主键ID；
        /// </summary>
        /// <param name="etpid">实体的主键</param>
        int PaperDelete(long etpid);
        /// <summary>
        /// 回收，标记删除状态为false
        /// </summary>
        int PaperRecycle(long etpid);
        /// <summary>
        /// 真正删除，按主键ID；
        /// </summary>
        /// <param name="etpid">实体的主键</param>
        int PaperRemove(long etpid);
        /// <summary>
        /// 获取单一试卷实体对象，按主键ID；
        /// </summary>
        /// <param name="etpid">实体的主键</param>
        /// <returns></returns>
        ExamTestPaper PaperSingle(long etpid);
        /// <summary>
        /// 获取单一试卷实体对象，按试卷名称；
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ExamTestPaper PaperSingle(string name);
        /// <summary>
        /// 判断是否有简答题
        /// </summary>
        bool PaperIsManual(long etpid);
        /// <summary>
        /// 判断是否有简答题
        /// </summary>
        bool PaperIsManual(ExamTestPaper entity);
        /// <summary>
        /// 获取指定数据的试卷
        /// </summary>
        /// <param name="orgid">机构id</param>
        /// <param name="accid">管理员id</param>
        /// <param name="isdeleted">是否删除</param>
        /// <param name="diff"></param>
        /// <param name="isUse"></param>
        /// <param name="count">指定数量</param>
        /// <returns></returns>
        List<ExamTestPaper> PaperCount(int orgid, int accid, bool? isdeleted, int diff, bool? isUse, int count);
        /// <summary>
        /// 获取指定数据的试卷
        /// </summary>
        /// <param name="search"></param>
        /// <param name="accid">管理员id</param>
        /// <param name="orgid">机构id</param>
        /// <param name="isdeleted">是否删除</param>
        /// <param name="diff"></param>
        /// <param name="isUse"></param>
        /// <param name="count">指定数量</param>
        /// <returns></returns>
        List<ExamTestPaper> PaperCount(int orgid, string search, int accid, bool? isdeleted, int diff, bool? isUse, int count);
        /// <summary>
        /// 计算有多少个试卷
        /// </summary>
        /// <param name="orgid"></param>
        /// <param name="diff"></param>
        /// <param name="isdeleted"></param>
        /// <param name="isUse"></param>
        /// <returns></returns>
        int PaperOfCount(int orgid, int diff, bool? isdeleted, bool? isUse);
        /// <summary>
        /// 分页获取试卷
        /// </summary>
        /// <param name="orgid">机构id</param>
        /// <param name="accid">管理员id</param>
        /// <param name="diff">难度等级</param>
        /// <param name="isUse">是否使用</param>
        /// <param name="sear">标题检索</param>
        /// <param name="isdeleted">是否删除</param>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <param name="countSum"></param>
        /// <returns></returns>
        List<ExamTestPaper> PaperPager(int orgid,int accid, string sear, bool? isdeleted, int diff, bool? isUse,  int size, int index, out int countSum);

        #endregion

        #region 试卷的试题项
        /// <summary>
        /// 试题数量
        /// </summary>
        /// <param name="identify"></param>
        /// <returns></returns>
        int QuesCount(long identify);
        #endregion

        #region 出卷
        /// <summary>
        /// 获取试卷的试题项
        /// </summary>
        List<TestPaperItem> PaperItems(long tpid);
        /// <summary>
        /// 获取试卷的试题项
        /// </summary>
        List<TestPaperItem> PaperItems(ExamTestPaper tp);
        /// <summary>
        /// 出卷，输出试卷内容
        /// </summary>
        /// <param name="tpid">试卷id</param>
        /// <param name="isanswer">试题是否带答案，模拟考试一般带答案，方便前端计算成绩</param>
        /// <returns></returns>
        Dictionary<TestPaperItem, List<Questions>> Putout(long tpid, bool isanswer);
        /// <summary>
        /// 出卷，输出试卷内容
        /// </summary>
        /// <param name="tp">试卷对象</param>
        /// <param name="isanswer">试题是否带答案，模拟考试一般带答案，方便前端计算成绩</param>
        /// <returns></returns>
        Dictionary<TestPaperItem, List<Questions>> Putout(ExamTestPaper tp, bool isanswer);
        /// <summary>
        /// 出卷，按历史答题内容生成试卷
        /// </summary>
        /// <param name="results">学员答题的xml记录</param>
        /// <param name="isanswer">试题是否带答案，模拟考试一般带答案，方便前端计算成绩</param>
        /// <returns></returns>
        Dictionary<TestPaperItem, List<Questions>> Putout(string results, bool isanswer);
        /// <summary>
        /// 出卷，按历史答题内容生成试卷
        /// </summary>
        Dictionary<TestPaperItem, List<Questions>> Putout(XmlDocument resxml, bool isanswer);
        #endregion

    }
}
