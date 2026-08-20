/*
 Navicat Premium Dump SQL

 Source Server         : postgreSQL
 Source Server Type    : PostgreSQL
 Source Server Version : 160003 (160003)
 Source Host           : localhost:5432
 Source Catalog        : examweisha
 Source Schema         : public

 Target Server Type    : PostgreSQL
 Target Server Version : 160003 (160003)
 File Encoding         : 65001

 Date: 16/04/2026 23:38:01
*/


-- ----------------------------
-- Table structure for ExamResults
-- ----------------------------
DROP TABLE IF EXISTS "public"."ExamResults";
CREATE TABLE "public"."ExamResults" (
  "Exr_ID" int4 NOT NULL DEFAULT nextval('"ExamResults_Exr_ID_seq"'::regclass),
  "Ac_Gender" int4 NOT NULL DEFAULT 0,
  "Ac_ID" int4 NOT NULL DEFAULT 0,
  "Ac_IDCardNumber" varchar(50) COLLATE "pg_catalog"."default",
  "Ac_Name" varchar(255) COLLATE "pg_catalog"."default",
  "Dep_Id" int4 NOT NULL DEFAULT 0,
  "Etp_Id" int8 NOT NULL DEFAULT 0,
  "Exam_ID" int4 NOT NULL DEFAULT 0,
  "Exam_Name" varchar(255) COLLATE "pg_catalog"."default",
  "Exam_Title" varchar(255) COLLATE "pg_catalog"."default",
  "Exam_UID" varchar(255) COLLATE "pg_catalog"."default",
  "Exr_CalcTime" timestamptz(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "Exr_Colligate" float4 NOT NULL DEFAULT 0,
  "Exr_CrtTime" timestamptz(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "Exr_Draw" float4 NOT NULL DEFAULT 0,
  "Exr_IP" varchar(255) COLLATE "pg_catalog"."default",
  "Exr_IsCalc" bool NOT NULL DEFAULT false,
  "Exr_IsManual" bool NOT NULL DEFAULT false,
  "Exr_IsSubmit" bool NOT NULL DEFAULT false,
  "Exr_LastTime" timestamptz(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "Exr_Mac" varchar(255) COLLATE "pg_catalog"."default",
  "Exr_OverTime" timestamptz(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "Exr_Results" text COLLATE "pg_catalog"."default",
  "Exr_Score" float4 NOT NULL DEFAULT 0,
  "Exr_ScoreFinal" float4 NOT NULL DEFAULT 0,
  "Exr_SubmitTime" timestamptz(6) NOT NULL DEFAULT CURRENT_TIMESTAMP,
  "Org_ID" int4 NOT NULL DEFAULT 0,
  "Org_Name" varchar(255) COLLATE "pg_catalog"."default",
  "Sbj_ID" int8 NOT NULL DEFAULT 0,
  "Sbj_Name" varchar(255) COLLATE "pg_catalog"."default",
  "Sts_ID" int8 NOT NULL DEFAULT 0,
  "Tp_Id" int8 NOT NULL DEFAULT 0
)
;

-- ----------------------------
-- Indexes structure for table ExamResults
-- ----------------------------
CREATE INDEX "ExamResults_IX_Ac_ID" ON "public"."ExamResults" USING btree (
  "Ac_ID" "pg_catalog"."int4_ops" ASC NULLS LAST
);
CREATE INDEX "ExamResults_IX_Ac_IDCardNumber" ON "public"."ExamResults" USING btree (
  "Ac_IDCardNumber" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);
CREATE INDEX "ExamResults_IX_Ac_Name" ON "public"."ExamResults" USING btree (
  "Ac_Name" COLLATE "pg_catalog"."default" "pg_catalog"."text_ops" ASC NULLS LAST
);
CREATE INDEX "ExamResults_IX_Etp_Id" ON "public"."ExamResults" USING btree (
  "Etp_Id" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "ExamResults_IX_Exam_ID" ON "public"."ExamResults" USING btree (
  "Exam_ID" "pg_catalog"."int4_ops" ASC NULLS LAST
);
CREATE INDEX "ExamResults_IX_Exr_CrtTime" ON "public"."ExamResults" USING btree (
  "Exr_CrtTime" "pg_catalog"."timestamptz_ops" ASC NULLS LAST
);
CREATE INDEX "ExamResults_IX_Exr_ScoreFinal" ON "public"."ExamResults" USING btree (
  "Exr_ScoreFinal" "pg_catalog"."float4_ops" ASC NULLS LAST
);
CREATE INDEX "ExamResults_IX_IsSubmit" ON "public"."ExamResults" USING btree (
  "Exr_IsSubmit" "pg_catalog"."bool_ops" ASC NULLS LAST
);
CREATE INDEX "ExamResults_IX_Org_ID" ON "public"."ExamResults" USING btree (
  "Org_ID" "pg_catalog"."int4_ops" ASC NULLS LAST
);
CREATE INDEX "ExamResults_IX_OverTime" ON "public"."ExamResults" USING btree (
  "Exr_OverTime" "pg_catalog"."timestamptz_ops" ASC NULLS LAST
);
CREATE INDEX "ExamResults_IX_Sts_ID" ON "public"."ExamResults" USING btree (
  "Sts_ID" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "ExamResults_IX_Tp_Id" ON "public"."ExamResults" USING btree (
  "Tp_Id" "pg_catalog"."int8_ops" ASC NULLS LAST
);
CREATE INDEX "ExamResults_aaaaaExamResults_PK" ON "public"."ExamResults" USING btree (
  "Exr_ID" "pg_catalog"."int4_ops" ASC NULLS LAST
);
CREATE INDEX "aaaaaExamResults_PK" ON "public"."ExamResults" USING btree (
  "Exr_ID" "pg_catalog"."int4_ops" ASC NULLS LAST
);

-- ----------------------------
-- Primary Key structure for table ExamResults
-- ----------------------------
ALTER TABLE "public"."ExamResults" ADD CONSTRAINT "key_examresults" PRIMARY KEY ("Exr_ID");
