/****** 
2020-11-05
******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

go
--��������ҳ��
alter table Notice add  No_Page nvarchar(200) NULL
go
--ʱ�Σ�ÿ��ļ��㵽���㣬��ͬ����ʼʱ��
alter table Notice add  No_Interval  nvarchar(2000) NULL
go
--���ڷ�Χ��0Ϊ���У�1Ϊ��¼ѧԱ��2Ϊδ��¼ѧԱ
alter table Notice add  No_Range   int NULL
go
UPDATE Notice SET No_Range  = 0
GO
alter table Notice ALTER COLUMN No_Range   int NOT NULL
go
--ÿ�쵯������
alter table Notice add  No_OpenCount  int NULL
go
UPDATE Notice SET No_OpenCount  = 0
GO
alter table Notice ALTER COLUMN No_OpenCount  int NOT NULL
go
--���
alter table Notice add  No_Width  int NULL
go
UPDATE Notice SET No_Width  = 0
GO
alter table Notice ALTER COLUMN No_Width  int NOT NULL
go
alter table Notice add  No_Height  int NULL
go
UPDATE Notice SET No_Height  = 0
GO
alter table Notice ALTER COLUMN No_Height  int NOT NULL
go
--��������ͼƬ
alter table Notice add  No_BgImage  [nvarchar](max) NULL
go
--���ӵ�ַ�����Ϊ�գ�����ת��֪ͨҳ
alter table Notice add  No_Linkurl  nvarchar(2000) NULL
go
--��ѧԱ����Ĺ�����Ϣ
alter table Notice add  No_StudentSort  nvarchar(2000) NULL
go

--������ʱ�䣨���룩
alter table Notice add  No_Timespan  int NULL
go
UPDATE Notice SET No_Timespan  = 0
GO
alter table Notice ALTER COLUMN No_Timespan   int NOT NULL
go
--֪ͨ���ͣ�1Ϊ��֪ͨͨ��2Ϊ����֪ͨ��3Ϊ����
alter table Notice add  No_Type  int NULL
go
UPDATE Notice SET No_Type  = 1
GO
alter table Notice ALTER COLUMN No_Type   int NOT NULL
go
ALTER TABLE Notice drop CONSTRAINT DF__Notice__No_IsOpe__6CD828CA
go
alter table Notice drop column No_IsOpen