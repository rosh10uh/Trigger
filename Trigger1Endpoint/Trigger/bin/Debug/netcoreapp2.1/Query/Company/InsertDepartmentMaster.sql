SET IDENTITY_INSERT [dbo].[Department] ON 

INSERT INTO [dbo].[Department]
           ([departmentId]
     ,[department]
           ,[companyId]
           ,[bactive]
           ,[createdby]
           ,[createddtstamp])
     VALUES
           (5,
     'Human Resources'
           ,@companyid
           ,1
           ,1
           ,GETDATE());

SET IDENTITY_INSERT [dbo].[Department] OFF
