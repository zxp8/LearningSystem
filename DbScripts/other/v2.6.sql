/*将原有超管super，移到默认机构的管理岗*/
UPDATE "EmpAccount" set "Posi_Id"= (SELECT "Posi_Id" FROM "Position" WHERE "Org_ID"=(SELECT "Org_ID" FROM "Organization" WHERE "Org_IsRoot"=true) and "Posi_IsAdmin"=TRUE),
"Org_ID"=(SELECT "Org_ID" FROM "Organization" WHERE "Org_IsDefault"=true)
WHERE "Org_ID"=(SELECT "Org_ID" FROM "Organization" WHERE "Org_IsRoot"=true) OR "Acc_AccName"='super';

/*将默认机构设置为根机构*/
UPDATE "Organization" set "Org_IsRoot"=TRUE WHERE "Org_ID"=(SELECT "Org_ID" FROM "Organization" WHERE "Org_IsDefault"=true);
UPDATE "Organization" set "Org_IsRoot"=false WHERE "Org_ID"!=(SELECT "Org_ID" FROM "Organization" WHERE "Org_IsDefault"=true);

--SELECT * from "EmpAccount" WHERE "Org_ID"=(SELECT "Org_ID" FROM "Organization" WHERE "Org_IsDefault"=true);
--SELECT * from "EmpAccount" WHERE "Acc_AccName"='super';
--SELECT * FROM "Position" WHERE "Org_ID"=(SELECT "Org_ID" FROM "Organization" WHERE "Org_IsRoot"=true) and "Posi_IsAdmin"=TRUE;

/*试卷增加是否需要批阅的字段*/

/*修订TestPaperItem表,删除一些冗余，添加试卷ID */
ALTER TABLE IF EXISTS "TestPaperItem" DROP COLUMN IF EXISTS "Org_Name" CASCADE;
ALTER TABLE IF EXISTS "TestPaperItem" DROP COLUMN IF EXISTS "Tp_UID" CASCADE;
ALTER TABLE IF EXISTS "TestPaperItem" ADD COLUMN "Tp_Id" int8 NOT NULL  DEFAULT 0;
ALTER TABLE IF EXISTS "TestPaperItem" ADD COLUMN "TPI_TypeName" VARCHAR(1000);

/*修订，增加人工批阅的字段*/
ALTER TABLE IF EXISTS  "Examination" ADD COLUMN "Exam_IsManual" BOOLEAN  NOT NULL DEFAULT FALSE;
ALTER TABLE IF EXISTS  "TestPaper" ADD COLUMN "Tp_IsManual" BOOLEAN  NOT NULL DEFAULT FALSE;
CREATE INDEX IF NOT EXISTS "Examination_Exam_IsManual" ON "Examination" ("Exam_IsManual" ASC);
CREATE INDEX IF NOT EXISTS "TestPaper_Tp_IsManual" ON "TestPaper" ("Tp_IsManual" ASC);
/*修订，考试表中增加试题量的值*/
ALTER TABLE IF EXISTS  "Examination" ADD COLUMN "Exam_QuesCount" INT NOT null DEFAULT 0;

/*修订一些其它字段，也许不用修订*/
UPDATE "SingleSignOn" set "SSO_IsAddSort"=FALSE WHERE "SSO_IsAddSort" ISNULL;
ALTER TABLE "SingleSignOn" ALTER COLUMN "SSO_IsAddSort" SET NOT NULL;
UPDATE "TestResults" set "Tr_Score"=0 WHERE "Tr_Score" ISNULL;
ALTER TABLE "TestResults" ALTER COLUMN "Tr_Score" SET NOT NULL;


/*有简答题的试卷，以及引用该试卷的考试，设置人工批阅*/
--SELECT "Tp_Id" FROM "TestPaper" WHERE (xpath('/Config/All/Items/TestPaperItem/TPI_Count/text()', "Tp_FromConfig"::xml ))[4]::text!='0';
--SELECT "Tp_Id" FROM "TestPaper" WHERE (xpath('/Config/Outline/Percent/TestPaperItem/TPI_Count/text()', "Tp_FromConfig"::xml ))[4]::text!='0';
UPDATE "TestPaper" set "Tp_IsManual"=FALSE;
UPDATE "Examination" set "Exam_IsManual"=FALSE;
UPDATE "TestPaper" set "Tp_IsManual"=true  WHERE (xpath('/Config/All/Items/TestPaperItem/TPI_Count/text()', "Tp_FromConfig"::xml ))[4]::text!='0';
UPDATE "TestPaper" set "Tp_IsManual"=true  WHERE (xpath('/Config/Outline/Percent/TestPaperItem/TPI_Count/text()', "Tp_FromConfig"::xml ))[4]::text!='0';

UPDATE "Examination" set "Exam_IsManual"=true WHERE "Tp_Id" in (SELECT "Tp_Id" FROM "TestPaper" WHERE "Tp_IsManual"=true);


/*考试场次的记录中没有Org_ID,只有考试主题有，这里是同步数据*/
DO $$
DECLARE exam_record RECORD;
BEGIN
    -- 使用 FOR 循环直接遍历查询结果
    FOR exam_record IN SELECT "Exam_UID", "Org_ID" FROM "Examination" WHERE "Exam_IsTheme"=TRUE
    LOOP
        -- 在此处对每一行数据进行处理
        RAISE NOTICE 'Exam_UID: %, Org_ID: %', exam_record."Exam_UID", exam_record."Org_ID";
        UPDATE "Examination" set "Org_ID"=exam_record."Org_ID" WHERE "Exam_IsTheme"=FALSE and "Exam_UID"=exam_record."Exam_UID";
    END LOOP;
END $$;

/*修订一些字段，之前命名不规范*/

/*修改排序字段*/
ALTER TABLE "CoursePrice" RENAME COLUMN "CP_Tax" TO "CP_Order";
ALTER TABLE "EmpTitle" RENAME COLUMN "Title_Tax" TO "Title_Order";
ALTER TABLE "Links" RENAME COLUMN "Lk_Tax" TO "Lk_Order";
ALTER TABLE "LinksSort" RENAME COLUMN "Ls_Tax" TO "Ls_Order";
ALTER TABLE "QuesTypes" RENAME COLUMN "Qt_Tax" TO "Qt_Order";

ALTER TABLE "Subject" RENAME COLUMN "Sbj_Tax" TO "Sbj_Order";
ALTER TABLE "Teacher" RENAME COLUMN "Th_Tax" TO "Th_Order";
ALTER TABLE "TeacherSort" RENAME COLUMN "Ths_Tax" TO "Ths_Order";
ALTER TABLE "StudentSort" RENAME COLUMN "Sts_Tax" TO "Sts_Order";
ALTER TABLE "Special" RENAME COLUMN "Sp_Tax" TO "Sp_Order";

ALTER TABLE "Columns" RENAME COLUMN "Col_Tax" TO "Col_Order";
ALTER TABLE "Course" RENAME COLUMN "Cou_Tax" TO "Cou_Order";
ALTER TABLE "Depart" RENAME COLUMN "Dep_Tax" TO "Dep_Order";
ALTER TABLE "EmpGroup" RENAME COLUMN "EGrp_Tax" TO "EGrp_Order";
ALTER TABLE "ManageMenu" RENAME COLUMN "MM_Tax" TO "MM_Order";

ALTER TABLE "Examination" RENAME COLUMN "Exam_Tax" TO "Exam_Order";
ALTER TABLE "GuideColumns" RENAME COLUMN "Gc_Tax" TO "Gc_Order";
ALTER TABLE "KnowledgeSort" RENAME COLUMN "Kns_Tax" TO "Kns_Order";
ALTER TABLE "Navigation" RENAME COLUMN "Nav_Tax" TO "Nav_Order";
ALTER TABLE "OrganLevel" RENAME COLUMN "Olv_Tax" TO "Olv_Order";

ALTER TABLE "Outline" RENAME COLUMN "Ol_Tax" TO "Ol_Order";
ALTER TABLE "PayInterface" RENAME COLUMN "Pai_Tax" TO "Pai_Order";
ALTER TABLE "Position" RENAME COLUMN "Posi_Tax" TO "Posi_Order";
ALTER TABLE "Questions" RENAME COLUMN "Qus_Tax" TO "Qus_Order";
ALTER TABLE "ShowPicture" RENAME COLUMN "Shp_Tax" TO "Shp_Order";
ALTER TABLE "ThirdpartyLogin" RENAME COLUMN "Tl_Tax" TO "Tl_Order";

/*修改性别字段*/
ALTER TABLE "Accounts" RENAME COLUMN "Ac_Sex" TO "Ac_Gender";
ALTER TABLE "EmpAccount" RENAME COLUMN "Acc_Sex" TO "Acc_Gender";
ALTER TABLE "ExamResults" RENAME COLUMN "Ac_Sex" TO "Ac_Gender";
ALTER TABLE "Teacher" RENAME COLUMN "Th_Sex" TO "Th_Gender";
ALTER TABLE "TestResults" RENAME COLUMN "St_Sex" TO "Ac_Gender";

/*在线考试中的试题相关*/
-- 创建试题分类表
DROP TABLE IF EXISTS "QuesPart" CASCADE;
CREATE TABLE IF NOT EXISTS "QuesPart" (
    "Qp_ID" BIGINT PRIMARY KEY,           -- 主键，雪花ID
    "Qp_PID" BIGINT NOT NULL,             -- 父级ID，雪花ID
    "Qp_Name" VARCHAR(1000) NOT NULL,     -- 分类名称
    "Org_ID" INT NOT NULL,                -- 组织机构ID
    "QP_Count" INT NOT null DEFAULT 0,             -- 试题数量
    "Qp_Order" INT NOT null DEFAULT 0,             -- 排序号
    "Qp_Intro" TEXT,                      -- 说明或介绍
    "Qp_IsUse" BOOLEAN NOT null  DEFAULT TRUE,      -- 是否启用
    "Qp_IsDeleted" BOOLEAN NOT null  DEFAULT FALSE,      -- 是否删除
    "Qp_CrtTime" TIMESTAMP NOT null  DEFAULT CURRENT_TIMESTAMP, -- 创建时间
    "Qp_UpdateTime" TIMESTAMP NOT null  DEFAULT CURRENT_TIMESTAMP -- 修改时间 
);

-- 创建索引
CREATE INDEX IF NOT EXISTS "QuesPart_IX_Qp_PID" ON "QuesPart"("Qp_PID");
CREATE INDEX IF NOT EXISTS "QuesPart_IX_Qp_Name" ON "QuesPart"("Qp_Name");
CREATE INDEX IF NOT EXISTS "QuesPart_IX_Qp_Order" ON "QuesPart"("Qp_Order");
CREATE INDEX IF NOT EXISTS "QuesPart_IX_Qp_IsUse" ON "QuesPart"("Qp_IsUse");
CREATE INDEX IF NOT EXISTS "QuesPart_IX_Org_ID" ON "QuesPart"("Org_ID");
CREATE INDEX  IF NOT EXISTS "QuesPart_IX_Qp_IsDeleted" ON "QuesPart"("Qp_IsDeleted");

-- 创建试题与分类关联表
DROP TABLE IF EXISTS "Questions_QPart" CASCADE;
CREATE TABLE IF NOT EXISTS "Questions_QPart" (
    "Qqp_ID" BIGINT PRIMARY KEY,          -- 主键，雪花ID
    "Qus_ID" BIGINT NOT NULL,            -- 试题ID
    "Qp_ID" BIGINT NOT NULL               -- 分类ID
);

-- 创建关联表索引
CREATE INDEX IF NOT EXISTS "Questions_QPart_IX_Qus_ID" ON "Questions_QPart"("Qus_ID");
CREATE INDEX IF NOT EXISTS "Questions_QPart_IX_Qp_ID" ON "Questions_QPart"("Qp_ID");
CREATE INDEX IF NOT EXISTS "Questions_QPart_IX_QuesQp" ON "Questions_QPart"("Qus_ID", "Qp_ID");


/*创建试题知识点*/
DROP TABLE IF EXISTS "QuesKnowledge" CASCADE;
CREATE TABLE IF NOT EXISTS  "QuesKnowledge" (
    "Qk_ID" BIGINT PRIMARY KEY DEFAULT 0,     -- 主键，雪花ID
    "Qk_PID" BIGINT NOT NULL DEFAULT 0,   -- 父级ID，雪花ID
    "Qk_Name" VARCHAR(1000) NOT NULL,
    "Org_ID" INT NOT NULL,             -- 组织机构ID
    "Qk_Order" INT NOT NULL DEFAULT 0,     -- 排序号
    "Qk_Weight" INT NOT NULL DEFAULT 0,     -- 权重值
    "Qk_IsUse" BOOLEAN NOT NULL DEFAULT TRUE,
    "Qk_Count" INT NOT NULL DEFAULT 0,       -- 试题数量
    "Qk_CrtTime" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,    -- 创建时间
    "Qk_UpdateTime" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP, -- 修改时间
    "Qk_Intro" TEXT,                        --简述
    "Qk_Details" TEXT
);
/*是否删除的字段*/
ALTER TABLE "QuesKnowledge" ADD COLUMN "Qk_IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE;
-- 创建索引
CREATE INDEX  IF NOT EXISTS "QuesKnowledge_IX_PID" ON "QuesKnowledge"("Qk_PID");
CREATE INDEX  IF NOT EXISTS "QuesKnowledge_IX_Name" ON "QuesKnowledge"("Qk_Name");
CREATE INDEX  IF NOT EXISTS "QuesKnowledge_IX_Order" ON "QuesKnowledge"("Qk_Order");
CREATE INDEX  IF NOT EXISTS "QuesKnowledge_IX_IsUse" ON "QuesKnowledge"("Qk_IsUse");
CREATE INDEX  IF NOT EXISTS "QuesKnowledge_IX_OrgID" ON "QuesKnowledge"("Org_ID");
CREATE INDEX  IF NOT EXISTS "QuesKnowledge_IX_PID_Order" ON "QuesKnowledge"("Qk_PID", "Qk_Order");
CREATE INDEX  IF NOT EXISTS "QuesKnowledge_IX_Org_ID" ON "QuesKnowledge"("Org_ID");
CREATE INDEX  IF NOT EXISTS "QuesKnowledge_IX_Qk_Count" ON "QuesKnowledge"("Qk_Count");
CREATE INDEX  IF NOT EXISTS "QuesKnowledge_IX_Qk_IsDeleted" ON "QuesKnowledge"("Qk_IsDeleted");

/*试题与知识点的关联表*/
DROP TABLE IF EXISTS "Questions_QKnl" CASCADE;
CREATE TABLE IF NOT EXISTS  "Questions_QKnl" (
    "Qqk_ID" BIGINT PRIMARY KEY DEFAULT 0,
    "Qus_ID" BIGINT NOT NULL,
    "Qk_ID" BIGINT NOT NULL
);
-- 创建所有字段的索引
CREATE INDEX  IF NOT EXISTS "Questions_QKnl_IX_ID" ON "Questions_QKnl"("Qqk_ID");
CREATE INDEX  IF NOT EXISTS "Questions_QKnl_IX_QuesID" ON "Questions_QKnl"("Qus_ID");
CREATE INDEX  IF NOT EXISTS "Questions_QKnl_Qk_ID" ON "Questions_QKnl"("Qk_ID");
-- 复合索引
CREATE INDEX  IF NOT EXISTS "Questions_QKnl_IX_QuesID_QkID" ON "Questions_QKnl"("Qus_ID", "Qk_ID");
CREATE INDEX  IF NOT EXISTS "Questions_QKnl_IX_QkID_QuesID" ON "Questions_QKnl"("Qk_ID", "Qus_ID");


--创建试题标签
DROP TABLE IF EXISTS "QuesTags" CASCADE;
CREATE TABLE  IF NOT EXISTS "QuesTags" (
    "Qtag_ID" BIGINT PRIMARY KEY DEFAULT 0,
    "Qtag_PID" BIGINT NOT NULL DEFAULT 0,
    "Org_ID" INT NOT NULL,
    "Qtag_Name" VARCHAR(255) NOT NULL,
    "Cou_ID" BIGINT NOT NULL,
    "Qtag_Count" INT NOT NULL DEFAULT 0,       -- 试题数量
    "Qtag_Order" INT NOT NULL DEFAULT 0,
    "Qtag_Weight" INT NOT NULL DEFAULT 0,             --权重
    "Qtag_CrtTime" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,    -- 创建时间
    "Qtag_UpdateTime" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP -- 修改时间
);
-- 创建索引
CREATE INDEX  IF NOT EXISTS "QuesTags_IX_PID" ON "QuesTags"("Qtag_PID");
CREATE INDEX  IF NOT EXISTS "QuesTags_IX_OrgID" ON "QuesTags"("Org_ID");
CREATE INDEX  IF NOT EXISTS "QuesTags_IX_Name" ON "QuesTags"("Qtag_Name");
CREATE INDEX  IF NOT EXISTS "QuesTags_IX_CouID" ON "QuesTags"("Cou_ID");
CREATE INDEX  IF NOT EXISTS "QuesTags_IX_Count" ON "QuesTags"("Qtag_Count");
CREATE INDEX  IF NOT EXISTS "QuesTags_IX_Order" ON "QuesTags"("Qtag_Order");
CREATE INDEX  IF NOT EXISTS "QuesTags_IX_Weight" ON "QuesTags"("Qtag_Weight");
/*是否删除的字段*/
ALTER TABLE "QuesTags" ADD COLUMN "Qtag_IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE;
CREATE INDEX  IF NOT EXISTS "QuesTags_IX_IsDeleted" ON "QuesTags"("Qtag_IsDeleted");


--创建试题与标签的关联表
CREATE TABLE "Questions_QTags" (
    "Qqt_ID" BIGINT PRIMARY KEY DEFAULT 0,
    "Qus_ID" BIGINT NOT NULL,
    "Qtag_ID" BIGINT NOT NULL
);
-- 创建所有字段的索引
CREATE INDEX  IF NOT EXISTS "Questions_QTags_IX_QuesID" ON "Questions_QTags"("Qus_ID");
CREATE INDEX  IF NOT EXISTS "Questions_QTags_IX_TagID" ON "Questions_QTags"("Qtag_ID");
-- 复合索引
CREATE INDEX  IF NOT EXISTS "Questions_QTags_IX_QuesID_TagID" ON "Questions_QTags"("Qus_ID", "Qtag_ID");
CREATE INDEX  IF NOT EXISTS "Questions_QTags_IX_TagID_QuesID" ON "Questions_QTags"("Qtag_ID", "Qus_ID");


/*为试题添用途的字段，默认为0，即课程使用；考试用为1*/
ALTER TABLE "Questions" ADD COLUMN "Qus_Purpose" int NOT NULL  DEFAULT 0;
CREATE INDEX  IF NOT EXISTS "Questions_IX_Purpose" ON "Questions"("Qus_Purpose");
/*为试题添加是否删除的字段*/
ALTER TABLE "Questions" ADD COLUMN "Qus_IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE;
CREATE INDEX  IF NOT EXISTS "Questions_IX_IsDeleted" ON "Questions"("Qus_IsDeleted");

--创建操作日志的记录表
DROP TABLE IF EXISTS "DataOperateLog" CASCADE;
CREATE TABLE "DataOperateLog" (
    -- 主键
    "Dlog_ID" BIGINT PRIMARY KEY DEFAULT 0,    
    -- 操作对象的记录
    "Dlog_Entity" VARCHAR(100) NOT NULL,    --记录名，即表结构名称
    "Dlog_KeyID" BIGINT NOT NULL,   --被操作表的主键值
    "Dlog_Type" INTEGER NOT NULL, -- 1新增, 2修改, 3删除    
    -- 时间信息
    "Dlog_CrtTime" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,  --创建时间
    "Dlog_Timespan" INTEGER, -- 用时（毫秒）    
    -- 客户端信息
    "Dlog_IP" VARCHAR(50),
    "Dlog_Browser" VARCHAR(100),
    "Dlog_OS" VARCHAR(100),
    "Dlog_BrwUa" VARCHAR(1000),    
    -- 外部ID
    "Acc_ID" BIGINT, -- 管理员ID
    "Th_ID" BIGINT, -- 教师ID
    "Ac_ID" BIGINT, -- 基础账号ID
    "Org_ID" BIGINT, -- 组织机构ID    
    -- JSON数据
    "Dlog_OldData" TEXT,  -- 历史记录
    "Dlog_NewData" TEXT,    --修改后的记录
    "Dlog_Fields" TEXT, -- 修改的字段    
    -- 操作上下文
    "Dlog_Module" VARCHAR(200), -- 业务模块
    "MM_Link" VARCHAR(1000), -- 菜单路径
    "MM_UID" VARCHAR(100), -- 菜单UID
    "Dlog_Mark" VARCHAR(500), -- 操作描述
    "Dlog_API" VARCHAR(500) -- RESTful API名称
);

-- 创建索引
CREATE INDEX  IF NOT EXISTS "DataOperateLog_IX_ID" ON "DataOperateLog"("Dlog_ID");
CREATE INDEX  IF NOT EXISTS "DataOperateLog_IX_Entity" ON "DataOperateLog"("Dlog_Entity");
CREATE INDEX  IF NOT EXISTS "DataOperateLog_IX_KeyID" ON "DataOperateLog"("Dlog_KeyID");
CREATE INDEX  IF NOT EXISTS "DataOperateLog_IX_Type" ON "DataOperateLog"("Dlog_Type");
CREATE INDEX  IF NOT EXISTS "DataOperateLog_IX_CrtTime" ON "DataOperateLog"("Dlog_CrtTime");
CREATE INDEX  IF NOT EXISTS "DataOperateLog_IX_IP" ON "DataOperateLog"("Dlog_IP");
CREATE INDEX  IF NOT EXISTS "DataOperateLog_IX_AccID" ON "DataOperateLog"("Acc_ID");
CREATE INDEX  IF NOT EXISTS "DataOperateLog_IX_ThID" ON "DataOperateLog"("Th_ID");
CREATE INDEX  IF NOT EXISTS "DataOperateLog_IX_AcID" ON "DataOperateLog"("Ac_ID");
CREATE INDEX  IF NOT EXISTS "DataOperateLog_IX_OrgID" ON "DataOperateLog"("Org_ID");
CREATE INDEX  IF NOT EXISTS "DataOperateLog_IX_Module" ON "DataOperateLog"("Dlog_Module");
CREATE INDEX  IF NOT EXISTS "DataOperateLog_IX_MMUID" ON "DataOperateLog"("MM_UID");
CREATE INDEX  IF NOT EXISTS "DataOperateLog_IX_API" ON "DataOperateLog"("Dlog_API");


--创建操作日志的档案表
DROP TABLE IF EXISTS "DataOperateLogArchive" CASCADE;
CREATE TABLE "DataOperateLogArchive" (
    -- 主键
    "Dlog_ID" BIGINT PRIMARY KEY DEFAULT 0,    
    -- 操作对象的记录
    "Dlog_Entity" VARCHAR(100) NOT NULL,    --记录名，即表结构名称
    "Dlog_KeyID" BIGINT NOT NULL,   --被操作表的主键值
    "Dlog_Type" INTEGER NOT NULL, -- 1新增, 2修改, 3删除    
    -- 时间信息
    "Dlog_CrtTime" TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,  --创建时间
    "Dlog_Timespan" INTEGER, -- 用时（毫秒）    
    -- 客户端信息
    "Dlog_IP" VARCHAR(50),
    "Dlog_Browser" VARCHAR(100),
    "Dlog_OS" VARCHAR(100),
    "Dlog_BrwUa" VARCHAR(1000),    
    -- 外部ID
    "Acc_ID" BIGINT, -- 管理员ID
    "Th_ID" BIGINT, -- 教师ID
    "Ac_ID" BIGINT, -- 基础账号ID
    "Org_ID" BIGINT, -- 组织机构ID    
    -- JSON数据
    "Dlog_OldData" TEXT,  -- 历史记录
    "Dlog_NewData" TEXT,    --修改后的记录
    "Dlog_Fields" TEXT, -- 修改的字段    
    -- 操作上下文
    "Dlog_Module" VARCHAR(200), -- 业务模块
    "MM_Link" VARCHAR(1000), -- 菜单路径
    "MM_UID" VARCHAR(100), -- 菜单UID
    "Dlog_Mark" VARCHAR(500), -- 操作描述
    "Dlog_API" VARCHAR(500) -- RESTful API名称
);

-- 创建索引
CREATE INDEX  IF NOT EXISTS "DataOperateLogArchive_IX_ID" ON "DataOperateLogArchive"("Dlog_ID");
CREATE INDEX  IF NOT EXISTS "DataOperateLogArchive_IX_Entity" ON "DataOperateLogArchive"("Dlog_Entity");
CREATE INDEX  IF NOT EXISTS "DataOperateLogArchive_IX_KeyID" ON "DataOperateLogArchive"("Dlog_KeyID");
CREATE INDEX  IF NOT EXISTS "DataOperateLogArchive_IX_Type" ON "DataOperateLogArchive"("Dlog_Type");
CREATE INDEX  IF NOT EXISTS "DataOperateLogArchive_IX_CrtTime" ON "DataOperateLogArchive"("Dlog_CrtTime");
CREATE INDEX  IF NOT EXISTS "DataOperateLogArchive_IX_IP" ON "DataOperateLogArchive"("Dlog_IP");
CREATE INDEX  IF NOT EXISTS "DataOperateLogArchive_IX_AccID" ON "DataOperateLogArchive"("Acc_ID");
CREATE INDEX  IF NOT EXISTS "DataOperateLogArchive_IX_ThID" ON "DataOperateLogArchive"("Th_ID");
CREATE INDEX  IF NOT EXISTS "DataOperateLogArchive_IX_AcID" ON "DataOperateLogArchive"("Ac_ID");
CREATE INDEX  IF NOT EXISTS "DataOperateLogArchive_IX_OrgID" ON "DataOperateLogArchive"("Org_ID");
CREATE INDEX  IF NOT EXISTS "DataOperateLogArchive_IX_Module" ON "DataOperateLogArchive"("Dlog_Module");
CREATE INDEX  IF NOT EXISTS "DataOperateLogArchive_IX_MMUID" ON "DataOperateLogArchive"("MM_UID");
CREATE INDEX  IF NOT EXISTS "DataOperateLogArchive_IX_API" ON "DataOperateLogArchive"("Dlog_API");

--创建题库收藏的表，这里是收藏管理员的试题
CREATE TABLE "QuesCollect" (
    "Qcl_ID" BIGINT PRIMARY KEY DEFAULT 0,
    "Acc_ID" BIGINT NOT NULL DEFAULT 0,
    "Qus_ID" BIGINT NOT NULL DEFAULT 0,
    "Qcl_CrtTime" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
-- 创建索引
CREATE INDEX  IF NOT EXISTS "QuesCollect_IX_AccID" ON "QuesCollect"("Acc_ID");
CREATE INDEX  IF NOT EXISTS "QuesCollect_IX_QuesID" ON "QuesCollect"("Qus_ID");
CREATE INDEX  IF NOT EXISTS "QuesCollect_IX_CrtTime" ON "QuesCollect"("Qcl_CrtTime");


-- 创建考试专用的试卷表 ExamTestPaper --
DROP TABLE IF EXISTS "ExamTestPaper" CASCADE;
CREATE TABLE IF NOT EXISTS "ExamTestPaper"
(
	"Etp_Id" bigint NOT NULL DEFAULT 0,
	"Org_ID" integer NOT NULL DEFAULT 0,	
  "Acc_Id" integer NOT NULL DEFAULT 0,
	"Acc_AccName" character varying(255) COLLATE pg_catalog."default" NOT NULL DEFAULT '',
	"Etp_Author" character varying(50) COLLATE pg_catalog."default",
	"Etp_Count" integer NOT NULL DEFAULT 0,
	"Etp_CrtTime" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
	"Etp_Diff" integer NOT NULL DEFAULT 0,
	"Etp_Diff2" integer NOT NULL DEFAULT 0,
	"Etp_FromConfig" text,
	"Etp_FromType" integer NOT NULL DEFAULT 0,
	"Etp_Intro" text,
	"Etp_IsBuild" boolean NOT NULL DEFAULT false,
	"Etp_IsManual" boolean NOT NULL DEFAULT false,
	"Etp_IsRec" boolean NOT NULL DEFAULT false,
  "Etp_IsDeleted" boolean NOT NULL DEFAULT false,
	"Etp_IsUse" boolean NOT NULL DEFAULT false,
	"Etp_Lasttime" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP,
	"Etp_Logo" character varying(255) COLLATE pg_catalog."default",
	"Etp_Name" character varying(255) COLLATE pg_catalog."default",
	"Etp_PassScore" integer NOT NULL DEFAULT 0,
	"Etp_Remind" text,
	"Etp_Span" integer NOT NULL DEFAULT 0,
	"Etp_SubName" character varying(255) COLLATE pg_catalog."default",
	"Etp_Total" integer NOT NULL DEFAULT 0,
	"Etp_Type" integer NOT NULL DEFAULT 0,
	 CONSTRAINT key_examtestpaper PRIMARY KEY ("Etp_Id")
);

CREATE INDEX IF NOT EXISTS "ExamTestPaper_IX_Org_ID" ON "ExamTestPaper" ("Org_ID" ASC);
CREATE INDEX IF NOT EXISTS "ExamTestPaper_IX_Acc_Id" ON "ExamTestPaper" ("Acc_Id" ASC);
CREATE INDEX IF NOT EXISTS "ExamTestPaper_IX_Etp_CrtTime" ON "ExamTestPaper" ("Etp_CrtTime" ASC);
CREATE INDEX IF NOT EXISTS "ExamTestPaper_IX_Etp_Diff" ON "ExamTestPaper" ("Etp_Diff" ASC);
CREATE INDEX IF NOT EXISTS "ExamTestPaper_Etp_IsManual" ON "ExamTestPaper" ("Etp_IsManual" ASC);
CREATE INDEX IF NOT EXISTS "ExamTestPaperr_IX_Etp_IsUse" ON "ExamTestPaper" ("Etp_IsUse" ASC);
CREATE INDEX IF NOT EXISTS "ExamTestPaper_IX_Etp_IsDeleted" ON "ExamTestPaper" ("Etp_IsDeleted" ASC);
CREATE INDEX IF NOT EXISTS "ExamTestPaper_IX_Etp_Name" ON "ExamTestPaper" ("Etp_Name" ASC);


/*添加考试是否删除、关联试卷ID、试卷来源的字段*/
ALTER TABLE "Examination" ADD COLUMN "Exam_IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE;
CREATE INDEX  IF NOT EXISTS "Examination_IX_IsDeleted" ON "Examination"("Exam_IsDeleted");

ALTER TABLE "Examination" ADD COLUMN "Etp_Id" bigint NOT NULL DEFAULT 0;
CREATE INDEX  IF NOT EXISTS "Examination_IX_Etp_Id" ON "Examination"("Etp_Id");

ALTER TABLE "Examination" ADD COLUMN "Acc_Id" integer NOT NULL DEFAULT 0;
CREATE INDEX  IF NOT EXISTS "Examination_IX_Acc_Id" ON "Examination"("Acc_Id");

ALTER TABLE "Examination" ADD COLUMN "Exam_Purpose" integer NOT NULL DEFAULT 0;
CREATE INDEX  IF NOT EXISTS "Examination_IX_Purpose" ON "Examination"("Exam_Purpose");
CREATE INDEX  IF NOT EXISTS "Examination_IX_Order" ON "Examination"("Exam_Order");
/*考试成绩中的试卷id*/
ALTER TABLE "ExamResults" ADD COLUMN "Etp_Id" bigint NOT NULL DEFAULT 0;
CREATE INDEX  IF NOT EXISTS "ExamResults_IX_Etp_Id" ON "ExamResults"("Etp_Id");


/*课程，专业，增加是否删除的字段*/
ALTER TABLE "Course" ADD COLUMN IF NOT EXISTS "Cou_IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE;
CREATE INDEX  IF NOT EXISTS "Course_IX_IsDeleted" ON "Course"("Cou_IsDeleted");
ALTER TABLE "Subject" ADD COLUMN IF NOT EXISTS "Sbj_IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE;
CREATE INDEX  IF NOT EXISTS "Subject_IX_IsDeleted" ON "Subject"("Sbj_IsDeleted");

/*增加考试与学员组关联的索引*/
DROP INDEX IF EXISTS "ExamGroup_aaaaaExamGroup_PK";
CREATE INDEX  IF NOT EXISTS "ExamGroup_IX_Sts_ID" ON "ExamGroup"("Sts_ID");
CREATE INDEX  IF NOT EXISTS "ExamGroup_IX_Exam_UID" ON "ExamGroup"("Exam_UID");
ALTER TABLE "ExamGroup" DROP COLUMN IF EXISTS "Org_Name" CASCADE;

-- ----------------------------
-- 创建学员与考试的关联关系
-- ----------------------------
DROP TABLE IF EXISTS "Exam_Accounts" CASCADE;
CREATE TABLE "public"."Exam_Accounts" (
  "Ea_ID" bigint NOT NULL DEFAULT 0,
  "Exam_UID" varchar(255) COLLATE "pg_catalog"."default",
  "Ac_ID" int4 NOT NULL,
   CONSTRAINT key_Exam_Accountsr PRIMARY KEY ("Ea_ID")
);

CREATE INDEX "Exam_Accounts_IX_Exam_UID" ON "public"."Exam_Accounts" USING btree (
  "Exam_UID" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);
CREATE INDEX "Exam_Accounts_IX_Ac_ID" ON "public"."Exam_Accounts" USING btree ("Ac_ID");
CREATE INDEX "aaaaaExam_Accounts_PK" ON "public"."Exam_Accounts" USING btree ("Ea_ID");
/*增加学员账号的是否删除字段*/
ALTER TABLE "Accounts" ADD COLUMN "Ac_IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE;
CREATE INDEX  IF NOT EXISTS "Accounts_IX_IsDeleted" ON "Accounts"("Ac_IsDeleted");

/*将考试的ID转为雪花ID*/
ALTER TABLE "ExamResults" ALTER COLUMN "Exam_ID" TYPE BIGINT;
ALTER TABLE "Examination"  ADD COLUMN "new_id" BIGINT;
UPDATE "Examination"  set "new_id"="Exam_ID";
ALTER TABLE "Examination" ALTER COLUMN "Exam_ID" DROP DEFAULT;
DROP SEQUENCE IF EXISTS "Examination_Exam_ID_seq";
ALTER TABLE "Examination" DROP CONSTRAINT IF EXISTS "key_examination";
ALTER TABLE "Examination" ALTER COLUMN "Exam_ID" DROP IDENTITY IF EXISTS;
-- 删除旧的 Exam_ID 列
ALTER TABLE "Examination" DROP COLUMN IF EXISTS "Exam_ID";
-- 将 new_id 重命名为 Exam_ID
ALTER TABLE "Examination" RENAME COLUMN "new_id" TO "Exam_ID";
-- 生新设置Exam_ID为主键
ALTER TABLE "Examination" ADD PRIMARY KEY ("Exam_ID");


/*试卷增加删除字段*/
ALTER TABLE "TestPaper"  ADD COLUMN IF NOT EXISTS "Tp_IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE;
CREATE INDEX  IF NOT EXISTS "TestPaper_IX_IsDeleted" ON "TestPaper"("Tp_IsDeleted");
ALTER TABLE "TestPaper" ADD COLUMN IF NOT EXISTS "Tp_DeleteTime" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP;
CREATE INDEX  IF NOT EXISTS "TestPaper_IX_DeleteTime" ON "TestPaper"("Tp_DeleteTime");

/*课程，专业，增加是否删除的时间*/
ALTER TABLE "Course" ADD COLUMN IF NOT EXISTS "Cou_DeleteTime" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP;
CREATE INDEX  IF NOT EXISTS "Course_IX_DeleteTime" ON "Course"("Cou_DeleteTime");
ALTER TABLE "Subject" ADD COLUMN IF NOT EXISTS "Sbj_DeleteTime" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP;
CREATE INDEX  IF NOT EXISTS "Subject_IX_DeleteTime" ON "Subject"("Sbj_DeleteTime");

/*试题，增加是否删除的时间*/
ALTER TABLE "Questions" ADD COLUMN IF NOT EXISTS "Qus_DeleteTime" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP;
CREATE INDEX  IF NOT EXISTS "Questions_IX_DeleteTime" ON "Questions"("Qus_DeleteTime");


/*试题分类、知识点、考试试卷、关键字，增加是否删除的时间*/
ALTER TABLE "QuesPart" ADD COLUMN IF NOT EXISTS "Qp_DeleteTime" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP;
CREATE INDEX  IF NOT EXISTS "QuesPart_IX_DeleteTime" ON "QuesPart"("Qp_DeleteTime");
ALTER TABLE "QuesKnowledge" ADD COLUMN IF NOT EXISTS "Qk_DeleteTime" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP;
CREATE INDEX  IF NOT EXISTS "QuesKnowledge_IX_DeleteTime" ON "QuesKnowledge"("Qk_DeleteTime");
ALTER TABLE "ExamTestPaper" ADD COLUMN IF NOT EXISTS "Etp_DeleteTime" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP;
CREATE INDEX  IF NOT EXISTS "ExamTestPaper_IX_DeleteTime" ON "ExamTestPaper"("Etp_DeleteTime");
ALTER TABLE "QuesTags" ADD COLUMN IF NOT EXISTS "Qtag_DeleteTime" TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP;
CREATE INDEX  IF NOT EXISTS "QuesTags_IX_DeleteTime" ON "QuesTags"("Qtag_DeleteTime");

/*删除冗余字段*/
ALTER TABLE "GuideColumns" DROP COLUMN IF EXISTS "Org_Name";