DELETE FROM "Student_Collect";
DELETE FROM "Student_Course";
DELETE FROM "Student_Notes";
DELETE FROM "Student_Ques";
DELETE FROM "StudentSort" WHERE "Sts_IsDefault"=false;
DELETE FROM "StudentSort_Course";

DELETE FROM "Accessory";
DELETE FROM "Course";
DELETE FROM "Subject";
DELETE FROM "Outline";
DELETE FROM "Questions";
DELETE FROM "QuesCollect";

DELETE FROM "Knowledge";
DELETE FROM "KnowledgeSort";
DELETE FROM "Guide";
DELETE FROM "GuideColumns";

DELETE FROM "LinksSort";
DELETE FROM "Links";
DELETE FROM "InternalLink";

DELETE FROM "LogForStudentExercise";
DELETE FROM "LogForStudentOnline";
DELETE FROM "LogForStudentQuestions";
DELETE FROM "LogForStudentStudy";


DELETE FROM "LearningCard";
DELETE FROM "LearningCardSet";
DELETE FROM "CouponAccount";
DELETE FROM "PointAccount";
DELETE FROM "RechargeCode";
DELETE FROM "RechargeSet";

DELETE FROM "Examination";
DELETE FROM "Exam_Accounts";
DELETE FROM "ExamTestPaper";
DELETE FROM "ExamResults";
DELETE FROM "ExamGroup";
DELETE FROM "QuesPart";
DELETE FROM "QuesTags";
DELETE FROM "QuesTypes";

DELETE FROM "Questions_QKnl";
DELETE FROM "Questions_QPart";
DELETE FROM "Questions_QTags";
DELETE FROM "QuesKnowledge";

DELETE FROM "PayInterface";
DELETE FROM "TestPaper";
DELETE FROM "TestPaperItem";
DELETE FROM "TestPaperQues";
DELETE FROM "TestResults";


DELETE FROM "LlmRecords";
DELETE FROM "ThirdpartyLogin";
DELETE FROM "ThirdpartyAccounts";

DELETE FROM "Columns";
DELETE FROM "Article";
DELETE FROM "Notice";
DELETE FROM "NewsNote";
DELETE FROM "ShowPicture";
DELETE FROM "SingleSignOn";
