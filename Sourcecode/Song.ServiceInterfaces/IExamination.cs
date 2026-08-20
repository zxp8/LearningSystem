using System;
using System.Collections.Generic;
using System.Text;
using Song.Entities;
using System.Data;
using WeiSha.Data;

namespace Song.ServiceInterfaces
{
    /// <summary>
    /// 考试管理
    /// </summary>
    public interface IExamination : WeiSha.Core.IBusinessInterface
    {

        #region 考试管理
        /// <summary>
        /// 考试项目的整体添加，包括考试主题和场次，参考人员范围
        /// </summary>
        /// <param name="theme">考试主题</param>
        /// <param name="items">考试的场次</param>
        /// <param name="groups">参考人员的范围</param>
        /// <param name="accounts">限定参考的学员关联对象（Exam_Accounts）</param>
        void ExamAdd(Examination theme, Examination[] items, ExamGroup[] groups, Exam_Accounts[] accounts);
        /// <summary>
        /// 考试项目的整体修改，包括考试主题和场次，参考人员范围
        /// </summary>
        /// <param name="theme">考试主题</param>
        /// <param name="items">考试的场次</param>
        /// <param name="groups">参考人员的范围</param>
        /// <param name="accounts">限定参考的学员关联对象（Exam_Accounts）</param>
        void ExamSave(Examination theme, Examination[] items, ExamGroup[] groups, Exam_Accounts[] accounts);
        /// <summary>
        /// 修改考试主题，按条件修改
        /// </summary>
        /// <param name="examid">考试主题的ID</param>
        /// <param name="fiels">要修改的字段</param>
        /// <param name="objs">fiels对应的值</param>
        int ExamUpdate(long examid, Field[] fiels, object[] objs);
        /// <summary>
        /// 删除考试主题，按主键ID；
        /// </summary>
        /// <param name="examid">实体的主键</param>
        int ExamThemeDelete(long examid);
        /// <summary>
        /// 删除考试场次，按主键ID；
        /// </summary>
        /// <param name="examid">实体的主键</param>
        int ExamDelete(long examid);
        /// <summary>
        /// 删除考试场次，按主键ID；
        /// </summary>
        /// <param name="examid">实体的主键</param>
        /// <param name="tran"></param>
        int ExamDelete(long examid, DbTrans tran);
        /// <summary>
        /// 获取单一实体对象，按主键ID；此处获取的是考试主题或场次
        /// </summary>
        /// <param name="examid">实体的主键</param>
        /// <returns></returns>
        Examination ExamSingle(long examid);
        /// <summary>
        /// 获取单一实体对象，通过全局唯一值，此处获取的是考试主题
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        Examination ExamTheme(string uid);
        /// <summary>
        /// 获取单一实体对象，取最近一次考试；此处获取的是考试场次
        /// </summary>
        /// <returns></returns>
        Examination ExamLast();
        /// <summary>
        /// 获取当前考试主题的考试项目
        /// </summary>
        /// <param name="uid">考试主题的uid</param>
        /// <returns></returns>
        List<Examination> ExamItems(string uid);
        /// <summary>
        /// 获取当前考试主题的考试项目
        /// </summary>
        /// <param name="id">考试主题的id</param>
        /// <returns></returns>
        List<Examination> ExamItems(long id);
        /// <summary>
        /// 当前考试主题关联的学员组
        /// </summary>
        /// <param name="uid">考试主题的uid</param>
        /// <returns></returns>
        List<StudentSort> ScopeForStudentSort(string uid);
        /// <summary>
        /// 当前考试主题关联的学员账号
        /// </summary>
        /// <param name="uid">考试主题的uid</param>
        /// <returns></returns>
        List<Accounts> ScopeForAccounts(string uid);
        /// <summary>
        /// 当前考试主题关联的学员账号
        /// </summary>
        /// <param name="uid">考试主题的uid</param>
        /// <param name="index"></param>
        /// <param name="size"></param>
        /// <param name="countSum"></param>
        /// <returns></returns>
        List<Accounts> ScopeForAccounts(string uid, int index, int size, out int countSum);
        /// <summary>
        /// 当前考试主题关联的学员账号总数
        /// </summary>
        /// <param name="uid">考试主题的uid</param>
        /// <returns></returns>
        int ScopeForAccountTotal(string uid);
        /// <summary>
        /// 获取考试，不分页
        /// </summary>
        /// <param name="orgid"></param>
        /// <param name="isUse"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        List<Examination> ExamCount(int orgid, bool? isUse, int count);
        /// <summary>
        /// 考试数
        /// </summary>
        /// <param name="orgid">机构id</param>
        /// <param name="isUse"></param>
        /// <param name="istheme">为true取考试主题数；false时，取场次数</param>
        /// <returns></returns>
        int ExamOfCount(int orgid, bool? isUse, bool istheme);
        /// <summary>
        /// 获取当前学生要参加的考试
        /// </summary>
        /// <param name="stid"></param>
        /// <param name="start">时间范围查询的开始时间</param>
        /// <param name="end">时间范围查询的结束时间</param>
        /// <param name="search">考试主题检索</param>
        /// <returns></returns>
        List<Examination> GetSelfExam(int stid, DateTime? start, DateTime? end, string search);
        /// <summary>
        /// 分页获取当前学生要参加的考试
        /// </summary>
        /// <param name="stid"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="search"></param>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <param name="countSum"></param>
        /// <returns></returns>
        List<Examination> GetSelfExam(int stid, DateTime? start, DateTime? end, string search, int size, int index, out int countSum);
        /// <summary>
        /// 判断某个考试是否允许某个学生参加
        /// </summary>
        /// <param name="examid">考试id</param>
        /// <param name="stid">学生id</param>
        /// <returns></returns>
        bool ExamIsForStudent(long examid, int stid);
        /// <summary>
        /// 获取指定时间内容的考试,这里是考试主题
        /// </summary>
        /// <param name="orgid"></param>
        /// <param name="start">时间区间检索的开始时间</param>
        /// <param name="end">时间区间检索的末尾时间</param>
        /// <param name="isUse"></param>
        /// <param name="searName"></param>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <param name="countSum"></param>
        /// <returns></returns>
        List<Examination> ThemePager(int orgid, DateTime? start, DateTime? end, bool? isUse, string searName, int size, int index, out int countSum);
        /// <summary>
        /// 获取指定时间内容的考试,这里是考试场次
        /// </summary>
        /// <param name="orgid"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="isUse"></param>
        /// <param name="ismanual">是否需要人工批阅</param>
        /// <param name="searName"></param>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <param name="countSum"></param>
        /// <returns></returns>
        List<Examination> ExamPager(int orgid, DateTime? start, DateTime? end, bool? isUse, bool? ismanual, string searName, int size, int index, out int countSum);
        /// <summary>
        /// 获取当前学生参加的的考试
        /// </summary>
        /// <param name="stid"></param>
        /// <param name="sbjid">学科id</param>
        /// <param name="orgid"></param>
        /// <param name="sear"></param>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <param name="countSum"></param>
        /// <returns></returns>
        List<ExamResults> GetAttendPager(int stid, long  sbjid, int orgid, string sear, int size, int index, out int countSum);
        #endregion

        #region 考试成绩提交等
        /// <summary>
        /// 添加考试答题信息
        /// </summary>
        /// <param name="result"></param>
        ExamResults ResultAdd(ExamResults result);
        /// <summary>
        /// 保存考试答题信息
        /// </summary>
        /// <param name="result"></param>
        void ResultSave(ExamResults result);
        /// <summary>
        /// 成绩提交
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        ExamResults ResultSubmit(ExamResults result);
        /// <summary>
        /// 计算成绩并保存
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        ExamResults ResultClacScore(ExamResults result);       
        /// <summary>
        /// 批量计算考试成绩
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <returns></returns>
        bool ResultBatchClac(long examid);
        /// <summary>
        /// 删除考试成绩
        /// </summary>
        /// <param name="exrid">成绩记录的id</param>
        int ResultDelete(int exrid);
        /// <summary>
        /// 删除某个学生的某个考试的成绩
        /// </summary>
        /// <param name="stid">学员账号id</param>
        /// <param name="examid">考试id</param>
        int ResultDelete(int stid, long examid);
        /// <summary>
        /// 删除考试下的所有成绩
        /// </summary>
        /// <param name="examid">考试id</param>
        int ResultClear(long examid);
        /// <summary>
        /// 删除考试下的所有成绩
        /// </summary>
        /// <param name="examid">考试id</param>
        /// <param name="tran"></param>
        int ResultClear(long examid, DbTrans tran);
        /// <summary>
        /// 获取最新的答题信息（正式答题信息）
        /// </summary>
        /// <param name="examid">考试id</param>
        /// <param name="tpid">试卷id</param>
        /// <param name="acid">考生id</param>
        /// <returns></returns>
        ExamResults ResultSingle(long examid, long tpid, int acid);
        /// <summary>
        /// 从缓存中获取考试答题信息
        /// </summary>
        /// <param name="examid"></param>
        /// <param name="tpid"></param>
        /// <param name="acid"></param>
        /// <returns></returns>
        ExamResults ResultForCache(long examid, long tpid, int acid);
        /// <summary>
        /// 更新答题信息缓存
        /// </summary>
        /// <param name="exr"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        string ResultCacheUpdate(ExamResults exr, int expires);
        /// <summary>
        /// 答题缓存的数量
        /// </summary>
        /// <param name="examid">考试id</param>
        /// <returns></returns>
        int ResultCacheCount(long examid);
        /// <summary>
        /// 学员在某个考试场次的得分
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <param name="acid">学员id</param>
        /// <returns></returns>
        double? ResultScore(int acid, long examid);      
        /// <summary>
        /// 获取当前考试的所有考生答题信息
        /// </summary>
        /// <param name="examid"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        ExamResults[] ResultCount(long examid, int count);
        /// <summary>
        /// 当前考试信息中，下一个
        /// </summary>
        /// <param name="examid"></param>
        /// <param name="stid"></param>
        /// <param name="isCorrect">是否是人工判卷过的，false下一个未判卷的信息</param>
        /// <returns></returns>
        ExamResults ResultSingleNext(long examid, int stid, bool? isCorrect);
        /// <summary>
        /// 通过答案id获取答题信息（正式答题信息）
        /// </summary>
        /// <param name="exrid"></param>
        /// <returns></returns>
        ExamResults ResultSingle(int exrid);
        /// <summary>
        /// 通过学员ID与考试ID，获取成绩（最好成绩）
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="examid"></param>
        /// <returns></returns>
        ExamResults ResultSingle(int accid, long examid);
        /// <summary>
        /// 根据答题信息，获取试题（针对答题过程中死机，又上线时）
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        List<Questions> QuesForResults(string results);
        #endregion

        #region 自动设置成绩
        /// <summary>
        /// 自助设置考试成绩得分
        /// </summary>
        /// <param name="exrid">考试的答题记录id</param>
        /// <param name="score">期望的得分</param>
        /// <returns></returns>
        ExamResults ResultSetScore(int exrid, float score);
        /// <summary>
        /// 自助设置考试成绩得分
        /// </summary>
        /// <param name="result">考试的答题记录</param>
        /// <param name="score">期望的得分</param>
        ExamResults ResultSetScore(ExamResults result, float score);
        /// <summary>
        /// 自助设置考试成绩得分
        /// </summary>
        /// <param name="result">考试的答题记录</param>
        /// <param name="score">期望的得分</param>
        /// <param name="time">考试开始时间</param>
        /// <param name="duration">考试用时，单位分钟</param>
        /// <returns></returns>
        ExamResults ResultSetScore(ExamResults result, float score, DateTime? time, int duration);
        /// <summary>
        /// 自助设置考试成绩得分，如果没有成绩记录，则创建一个
        /// </summary>
        /// <param name="accid">学员账号id</param>
        /// <param name="examid">考试场次id</param>
        /// <param name="score">期望的得分</param>
        /// <param name="time">考试开始时间</param>
        /// <param name="duration">考试用时，单位分钟</param>
        /// <returns></returns>
        ExamResults ResultSetScore(long examid, int accid, float score, DateTime? time, int duration);
        /// <summary>
        /// 自助设置考试成绩得分，如果没有成绩记录，则创建一个
        /// </summary>
        /// <param name="exam">考试场次的对象</param>
        /// <param name="acc">学员账号的对象</param>
        /// <param name="score">期望的得分</param>
        /// <param name="time">考试开始时间</param>
        /// <param name="duration">考试用时，单位分钟</param>
        /// <returns></returns>
        ExamResults ResultSetScore(Examination exam, Accounts acc, float score, DateTime? time, int duration);

        /// <summary>
        /// 批量生成缺考人员的成绩
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <param name="minScore"></param>
        /// <param name="maxScore"></param>
        /// <param name="minTime"></param>
        /// <param name="maxTime"></param>
        /// <param name="minSpan"></param>
        /// <param name="maxSpan"></param>
        /// <returns></returns>
        (int, int) ResultAbsenceBatchScore(long examid, int minScore, int maxScore, DateTime minTime, DateTime maxTime, int minSpan, int maxSpan);
        /// <summary>
        /// 批量生成缺考人员的成绩的进度
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        (int, int) ResultAbsenceBatchScore(long examid);
        #endregion

        #region 成绩统计
        /// <summary>
        /// 考试主题下的所有参考人员成绩
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        DataTable Result4Theme(long examid);
        /// <summary>
        /// 考试主题下的所有参考人员的班组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<StudentSort> StudentSort4Theme(long examid);
        /// <summary>
        /// 考试场次下，参加考试的学员的学员组,仅统计参加考试的学员
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <returns></returns>
        List<StudentSort> ResultSort4Exam(long examid);
        /// <summary>
        /// 未参加考试的学员的学员组
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <returns></returns>
        List<StudentSort> AbsenceSort4Exam(long examid);
        /// <summary>
        /// 考试主题下的所有参考人员成绩
        /// </summary>
        /// <param name="examid">当前考试主题的ID</param>
        /// <param name="stsid">学生分组的id，为0时取所有，为-1时取不在组的学员，大于0则取当前组学员</param>
        /// <returns></returns>
        DataTable Result4Theme(long examid, long stsid);
        /// <summary>
        /// 考试主题下的所有参考人员成绩
        /// </summary>
        /// <param name="examid">当前考试主题的ID</param>
        /// <param name="stsid">学生分组的id，为0时取所有，为-1时取不在组的学员，大于0则取当前组学员</param>
        /// <returns></returns>
        DataTable Result4Theme(long examid, string stsid);
        /// <summary>
        /// 考试主题下的所有参考人员成绩
        /// </summary>
        /// <param name="examid"></param>
        /// <param name="stsid">学生分组的id，为0时取所有，为-1时取不在组的学员，大于0则取当前组学员</param>
        /// <param name="isAll">是否取所有人员（含缺考人员）,false为仅参考人员</param>
        /// <returns></returns>
        DataTable Result4Theme(long examid, long stsid, bool isAll);
        /// <summary>
        /// 当前考试主题下的各学员分组成绩排行
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        DataTable Result4StudentSort(long examid);        
        /// <summary>
        /// 计算某个考试主题的及格率
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        double PassRate4Theme(string uid);
        /// <summary>
        /// 计算某场考试的及极率
        /// </summary>
        /// <param name="exam"></param>
        /// <returns></returns>
        double PassRate4Exam(Examination exam);
        /// <summary>
        /// 计算某场考试的及极率
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        double PassRate4Exam(long examid);
        /// <summary>
        /// 计算某个考试主题的平均分
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        double Avg4Theme(string uid);
        /// <summary>
        /// 计算某场考试的平均分
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <returns></returns>
        double Avg4Exam(long examid);
        /// <summary>
        /// 某场考试的最高分
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <returns></returns>
        double Highest4Exam(long examid);
        /// <summary>
        /// 某场考试的最低分
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <returns></returns>
        double Lowest4Exam(long examid);
        /// <summary>
        /// 参加考试主题的学员列表
        /// </summary>
        /// <param name="examid">考试主题的id</param>
        /// <param name="name">学员姓名</param>
        /// <param name="idcard">身份证号</param>
        /// <param name="stsid">学员组ID</param>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <param name="countSum"></param>
        /// <returns></returns>
        List<Accounts> AttendThemeAccounts(long examid, string name, string idcard, long stsid, int size, int index, out int countSum);
        /// <summary>
        /// 某个考试场次所缺考的学员列表
        /// </summary>
        /// <param name="examid">考试场次的id</param>
        /// <param name="name">学员姓名</param>
        /// <param name="idcard">身份证号</param>
        /// <param name="phone"></param>
        /// <param name="stsid">学员组id</param>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <param name="countSum"></param>
        /// <returns></returns>
        List<Accounts> AbsenceExamAccounts(long examid, string name, string idcard, string phone, long stsid, int size, int index, out int countSum);
        /// <summary>
        /// 当前考试场次下的学员成绩
        /// </summary>
        /// <param name="examid"></param>
        /// <param name="name">学员姓名</param>
        /// <param name="idcard">身份证号</param>
        /// <param name="stsid">学员组ID</param>
        /// <param name="min">按分数区间获取记录，此处是最低分</param>
        /// <param name="max">最高分</param>
        /// <param name="manual">是否批阅</param>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <param name="countSum"></param>
        /// <returns></returns>
        List<ExamResults> ResultsPager(long examid, string name, string idcard, long stsid, float min, float max,bool? manual, int size, int index, out int countSum);
      
        /// <summary>
        /// 当前考试场次下的所有人员成绩
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <param name="count">取多少条</param>
        /// <returns></returns>
        List<ExamResults> Results(long examid, int count);

        #endregion

        #region 成绩导出
        /// <summary>
        /// 某场考试的考试成绩，按学员组导出，如果没有设置组，则导出全部
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="examid">考试场次id</param>
        /// <param name="sorts">学员组</param>
        /// <returns></returns>
        string ExportResults4Exam(string filePath, long examid, long[] sorts);
        /// <summary>
        /// 考试主题下的所有成绩
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="examid">考试主题的id</param>
        /// <param name="sorts">学员组</param>
        /// <returns></returns>
        string ExportResults4Theme(string filePath, long examid, long[] sorts);
        /// <summary>
        /// 导出某场考试的缺考人员
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="examid">考试场次id</param>
        /// <returns></returns>
        string ExportAbsences4Exam(string filePath, long examid);
        /// <summary>
        /// 学员在某个课程下的考试成绩
        /// </summary>
        /// <param name="couid"></param>
        /// <param name="acid"></param>
        /// <returns>返回成绩记录</returns>
        ExamResults StudentForCourseExam(long couid, int acid);
        #endregion

        #region 统计
        /// <summary>
        /// 考试主题下允许参考的学员数量
        /// </summary>
        /// <param name="examid">考试主题的ID</param>
        /// <returns></returns>
        int NumberOfStudent(long examid);
        /// <summary>
        /// 考试主题下允许参考的学员数量
        /// </summary>
        /// <param name="exam">考试主题的实体</param>
        /// <returns></returns>
        int NumberOfStudent(Examination exam);
        /// <summary>
        /// 当前考试的参考人次，如果学员多次考试，则人次大于人数
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <returns></returns>
        int Numbertimes4Exam(long examid);
        /// <summary>
        /// 当前考试的参考人数
        /// </summary>
        /// <param name="examid">考试场次id</param>
        /// <returns></returns>
        int Number4Exam(long examid);
        /// <summary>
        /// 当前考试的缺考的人数
        /// </summary>
        /// <param name="examid"></param>
        /// <returns></returns>
        int NumberAbsence4Exam(long examid);
        /// <summary>
        /// 当前考试主题的参考人次，如果学员多次考试，则人次大于人数
        /// </summary>
        /// <param name="examid">考试主题的id</param>
        /// <returns></returns>
        int Numbertimes4Theme(long examid);
        /// <summary>
        /// 当前考试的参考人数
        /// </summary>
        /// <param name="examid">考试主题的id</param>
        /// <returns></returns>
        int Number4Theme(long examid);
        /// <summary>
        /// 当前考试的缺考的人数
        /// </summary>
        /// <param name="examid">考试主题的id</param>
        /// <returns></returns>
        int NumberAbsence4Theme(long examid);
        #endregion
    }
}
