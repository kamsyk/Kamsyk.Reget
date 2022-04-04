/****** Script for SelectTopNRows command from SSMS  ******/
--SELECT TOP 1000 [id]
--      ,[text_content]
--      ,[text_type]
--  FROM [InternalRequest].[dbo].[App_Text_Store]

--  SELECT * FROM dbo.[App_Text_Store] WHERE FREETEXT(text_content,'prah%')

  SELECT * FROM [App_Text_Store] atsd
  left outer join Request_Text rtd
  on rtd.reget_text_id = atsd.id
  WHERE CONTAINS(text_content, '"tiskovo*"')
