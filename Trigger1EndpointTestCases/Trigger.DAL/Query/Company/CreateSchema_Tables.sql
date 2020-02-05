CREATE TABLE [dbo].[Department](
	[departmentid] [smallint] IDENTITY(1,1) NOT NULL,
	[department] [varchar](100) NOT NULL,
	[companyId] [int] NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL,
 CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED 
(
	[departmentid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

CREATE TABLE [dbo].[EmployeeDetails](
	[empid] [int] IDENTITY(1,1) NOT NULL,
	[companyid] [int] NOT NULL,
	[firstname] [nvarchar](100) NOT NULL,
	[middlename] [nvarchar](100) NULL,
	[lastname] [nvarchar](100) NOT NULL,
	[suffix] [varchar](8) NULL,
	[email] [varchar](100) NULL,
	[jobtitle] [varchar](100) NOT NULL,
	[joiningdate] [date] NOT NULL,
	[workcity] [varchar](100) NOT NULL,
	[workstate] [varchar](100) NOT NULL,
	[workzipcode] [varchar](25) NULL,
	[departmentid] [smallint] NOT NULL,
	[managerid] [int] NULL,
	[managername] [nvarchar](200) NULL,
	[managerlname] [nvarchar](200) NULL,
	[empstatus] [bit] NOT NULL,
	[roleid] [tinyint] NOT NULL,
	[dateofbirth] [date] NULL,
	[raceorethanicityid] [smallint] NULL,
	[gender] [nvarchar](50) NULL,
	[jobcategoryid] [smallint] NULL,
	[jobcodeid] [smallint] NULL,
	[jobgroupid] [smallint] NULL,
	[lastpromodate] [date] NULL,
	[currentsalary] [decimal](12, 2) NULL,
	[lastincdate] [date] NULL,
	[emplocation] [nvarchar](100) NULL,
	[countryid] [int] NULL,
	[regionid] [int] NULL,
	[empimgpath] [varchar](1000) NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL,
	[EmployeeId] [varchar](100) NULL,
	[jobcategory] [nvarchar](100) NULL,
	[jobcode] [nvarchar](100) NULL,
	[JobGroup] [nvarchar](100) NULL,
	[IsMailSent] [bit] NULL,
	[phonenumber] [varchar](25) NULL,
	[phoneconfirmed] [bit] NULL,
	[optforsms] [bit] NULL,
 CONSTRAINT [PK_employeedetails_1] PRIMARY KEY CLUSTERED 
(
	[empid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);
ALTER TABLE [dbo].[EmployeeDetails] ADD  CONSTRAINT [DF_EmployeeDetails_IsMailSent]  DEFAULT ((0)) FOR [IsMailSent];

ALTER TABLE [dbo].[EmployeeDetails] ADD  CONSTRAINT [DF_EmployeeDetails_optforsms]  DEFAULT ((0)) FOR [optforsms];

ALTER TABLE [dbo].[EmployeeDetails] ADD  CONSTRAINT [DF_EmployeeDetails_phoneconfirmed]  DEFAULT ((0)) FOR [phoneconfirmed];

ALTER TABLE [dbo].[EmployeeDetails]  WITH CHECK ADD  CONSTRAINT [FK__EmployeeDetails__deparment] FOREIGN KEY([departmentid])
REFERENCES [dbo].[Department] ([departmentid]);

ALTER TABLE [dbo].[EmployeeDetails] CHECK CONSTRAINT [FK__EmployeeDetails__deparment];

CREATE TABLE [dbo].[UserLogin](
	[userid] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](150) NOT NULL,
	[password] [varchar](max) NULL,
	[empid] [int] NOT NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL,
 CONSTRAINT [PK_userlogin] PRIMARY KEY CLUSTERED 
(
	[userid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

ALTER TABLE [dbo].[UserLogin]  WITH CHECK ADD  CONSTRAINT [fk_userlogin_1] FOREIGN KEY([empid])
REFERENCES [dbo].[EmployeeDetails] ([empid]);

ALTER TABLE [dbo].[UserLogin] CHECK CONSTRAINT [fk_userlogin_1];

CREATE TABLE [dbo].[NotificationsLogs](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[EmpId] [int] NULL,
	[Message] [varchar](max) NULL,
	[ManagerId] [int] NOT NULL,
	[Action] [varchar](50) NULL,
	[IsSent] [bit] NULL,
	[MarkAs] [bit] NULL,
	[CreatedBy] [int] NULL,
	[CreatedDtStamp] [datetime] NULL,
	[Type] [varchar](50) NULL,
	[reportingtoid] [int] NULL,
 CONSTRAINT [PK_NotificationsLogs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

ALTER TABLE [dbo].[NotificationsLogs]  WITH CHECK ADD  CONSTRAINT [fk_NotificationsLogs_EmpID] FOREIGN KEY([EmpId])
REFERENCES [dbo].[EmployeeDetails] ([empid]);

ALTER TABLE [dbo].[NotificationsLogs] CHECK CONSTRAINT [fk_NotificationsLogs_EmpID];


CREATE TABLE [dbo].[questioncategories](
	[categoryid] [smallint] IDENTITY(1,1) NOT NULL,
	[category] [varchar](50) NOT NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL,
 CONSTRAINT [PK_questioncategories] PRIMARY KEY CLUSTERED 
(
	[categoryid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

CREATE TABLE [dbo].[questionsdirectory](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[categoryid] [smallint] NOT NULL,
	[questions] [varchar](500) NOT NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL,
 CONSTRAINT [PK_questionsdirectory] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

ALTER TABLE [dbo].[questionsdirectory]  WITH CHECK ADD  CONSTRAINT [fk_questionsdirectory_1] FOREIGN KEY([categoryid])
REFERENCES [dbo].[questioncategories] ([categoryid]);

ALTER TABLE [dbo].[questionsdirectory] CHECK CONSTRAINT [fk_questionsdirectory_1];

CREATE TABLE [dbo].[answersconfig](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[questionid] [int] NOT NULL,
	[answer] [nvarchar](100) NOT NULL,
	[weightage] [numeric](5, 0) NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL,
 CONSTRAINT [PK_answersconfig] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON));

ALTER TABLE [dbo].[answersconfig]  WITH CHECK ADD  CONSTRAINT [fk_answersconfig_1] FOREIGN KEY([questionid])
REFERENCES [dbo].[questionsdirectory] ([id]);

ALTER TABLE [dbo].[answersconfig] CHECK CONSTRAINT [fk_answersconfig_1];

CREATE TABLE [dbo].[ScoreRemarks](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[ScoreRank] [varchar](10) NOT NULL,
	[ScoreRemarks] [nvarchar](max) NOT NULL,
	[Bactive] [bit] NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDtstamp] [date] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDtstamp] [date] NULL,
	[GradeOrder] [decimal](5, 2) NULL,
	[FromScore] [int] NULL,
	[ToScore] [int] NULL,
	[ManagerAction] [varchar](100) NULL,
	[ScoreSummary] [varchar](100) NULL,
	[GeneralScoreRank] [varchar](100) NULL,
 CONSTRAINT [PK_ScoreRemarks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);


CREATE TABLE [dbo].[employee_assessment](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[empid] [int] NOT NULL,
	[assessmentdate] [datetime] NOT NULL,
	[assessmentperiod] [varchar](50) NULL,
	[assessmentby] [int] NOT NULL,
	[remarks] [nvarchar](max) NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL,
	[GeneralStatus] [int] NULL,
	[score] [int] NULL,
	[scoreid] [smallint] NULL,
	[GeneralDocPath] [varchar](1000) NULL,
	[CloudFilePath]  [VARCHAR](500) NULL,
	[ScoreFeedback] [BIT]  NULL,
	[FeedbackRemark] [VARCHAR](4000) NULL,
	[ExpectedScoreId] [INT]  NULL
 CONSTRAINT [PK_employeeassessment] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

ALTER TABLE [dbo].[employee_assessment]  WITH CHECK ADD  CONSTRAINT [FK__employee_assessment__EmpID] FOREIGN KEY([empid])
REFERENCES [dbo].[EmployeeDetails] ([empid]);

ALTER TABLE [dbo].[employee_assessment] CHECK CONSTRAINT [FK__employee_assessment__EmpID];

ALTER TABLE [dbo].[employee_assessment]  WITH CHECK ADD  CONSTRAINT [fk_employee_assessment_ScoreID] FOREIGN KEY([scoreid])
REFERENCES [dbo].[ScoreRemarks] ([Id]);

ALTER TABLE [dbo].[employee_assessment] CHECK CONSTRAINT [fk_employee_assessment_ScoreID];

CREATE TABLE [dbo].[assessmentdetails](
	[assessmentid] [int] NOT NULL,
	[questionid] [int] NOT NULL,
	[answerid] [int] NOT NULL,
	[remarks] [nvarchar](max) NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL,
	[KPIStatus] [int] NULL,
	[DocumentPath] [varchar](1000) NULL,
	[CloudFilePath]  [VARCHAR](500) NULL
)


ALTER TABLE [dbo].[assessmentdetails]  WITH CHECK ADD  CONSTRAINT [fk_assessmentdetails_1] FOREIGN KEY([assessmentid])
REFERENCES [dbo].[employee_assessment] ([id]);

ALTER TABLE [dbo].[assessmentdetails] CHECK CONSTRAINT [fk_assessmentdetails_1];

ALTER TABLE [dbo].[assessmentdetails]  WITH CHECK ADD  CONSTRAINT [fk_assessmentdetails_2] FOREIGN KEY([questionid])
REFERENCES [dbo].[questionsdirectory] ([id]);

ALTER TABLE [dbo].[assessmentdetails] CHECK CONSTRAINT [fk_assessmentdetails_2];

ALTER TABLE [dbo].[assessmentdetails]  WITH CHECK ADD  CONSTRAINT [fk_assessmentdetails_3] FOREIGN KEY([answerid])
REFERENCES [dbo].[answersconfig] ([id]);

ALTER TABLE [dbo].[assessmentdetails] CHECK CONSTRAINT [fk_assessmentdetails_3];


CREATE TABLE [dbo].[Partial_EmployeeAssessment](
 [Id] [int] IDENTITY(1,1) NOT NULL,
 [Empid] [int] NOT NULL,
 [AssessmentDate] [datetime] NOT NULL,
 [AssessmentBy] [int] NOT NULL,
 [KPIRemarks] [nvarchar](max) NULL,
 [KPIStatus] [int] NULL,
 [GeneralRemarks] [nvarchar](max) NULL,
 [GeneralStatus] [int] NULL,
 [BActive] [bit] NOT NULL,
 [CreatedBy] [int] NOT NULL,
 [CreatedDtstamp] [datetime] NOT NULL,
 [UpdatedBy] [int] NULL,
 [UpdatedDtstamp] [datetime] NULL,
 CONSTRAINT [PK_Partial_EmployeeAssessment] PRIMARY KEY CLUSTERED 
(
 [Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];


CREATE TABLE [dbo].[spt_values](
	[name] [nvarchar](35) NULL,
	[number] [int] NULL,
	[type] [nchar](3) NULL,
	[low] [int] NULL,
	[high] [int] NULL,
	[status] [int] NULL
);

CREATE TABLE [dbo].[ContactUs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[FirstName] [nvarchar](100) NULL,
	[LastName] [nvarchar](100) NULL,
	[Email] [nvarchar](100) NULL,
	[PhoneNo] [bigint] NULL,
	[Subject] [nvarchar](max) NULL,
	[Comments] [nvarchar](max) NULL,
	[CreatedDtstamp] [datetime] NULL,
 CONSTRAINT [PK_ContactUs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
);

CREATE TABLE [dbo].[SubscriberEmailID](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](50) NULL,
 CONSTRAINT [PK_SubscriberEmailID] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
);

CREATE TABLE [dbo].[CompanyAndRoleWiseWidget](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[WidgetTypeId] [int] NOT NULL,
	[WidgetId] [int] NOT NULL,
	[RoleId] [tinyint] NOT NULL,
	[BActive] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDtstamp] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDtstamp] [datetime] NULL,
 CONSTRAINT [PK_CompanyAndRoleWiseWidget] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

CREATE TABLE [dbo].[UserWiseWidgetDetails](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[WidgetId] [int] NOT NULL,
	[SequenceNumber] [int] NULL,
	[TileSequence] [int] NULL,
	[Position] [decimal](10, 4) NULL,
	[IsActive] [bit] NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDtstamp] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDtstamp] [datetime] NULL,
 CONSTRAINT [PK_UserWiseWidgetDetails] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

ALTER TABLE [dbo].[UserWiseWidgetDetails]  WITH CHECK ADD  CONSTRAINT [fk_UserWiseWidgetDetails_UserID] FOREIGN KEY([UserId])
REFERENCES [dbo].[UserLogin] ([userid]);

ALTER TABLE [dbo].[UserWiseWidgetDetails] CHECK CONSTRAINT [fk_UserWiseWidgetDetails_UserID];

CREATE TABLE [dbo].[tmpEmployeedetails](
	[empId] [int] NOT NULL,
	[companyId] [varchar](100) NOT NULL,
	[firstName] [nvarchar](100) NOT NULL,
	[middleName] [nvarchar](100) NULL,
	[lastName] [nvarchar](100) NOT NULL,
	[suffix] [varchar](8) NULL,
	[email] [varchar](100) NULL,
	[jobTitle] [varchar](100) NOT NULL,
	[joiningDate] [varchar](15) NOT NULL,
	[workCity] [varchar](100) NOT NULL,
	[workState] [varchar](100) NOT NULL,
	[workZipcode] [varchar](25) NOT NULL,
	[departmentId] [varchar](100) NOT NULL,
	[department] [varchar](100) NULL,
	[managerId] [varchar](100) NULL,
	[managerName] [nvarchar](200) NULL,
	[managerLName] [nvarchar](200) NULL,
	[empStatus] [bit] NOT NULL,
	[roleId] [varchar](50) NOT NULL,
	[dateOfBirth] [varchar](15) NULL,
	[raceorethanicityId] [nvarchar](60) NULL,
	[gender] [nvarchar](50) NULL,
	[jobCategory] [nvarchar](100) NULL,
	[jobCode] [nvarchar](100) NULL,
	[jobGroup] [nvarchar](100) NULL,
	[lastPromodate] [varchar](15) NULL,
	[currentSalary] [decimal](12, 2) NULL,
	[lastIncDate] [varchar](15) NULL,
	[empLocation] [nvarchar](100) NULL,
	[countryId] [nvarchar](100) NULL,
	[regionId] [varchar](100) NULL,
	[empImgPath] [varchar](1000) NULL,
	[bactive] [bit] NOT NULL,
	[createdBy] [int] NOT NULL,
	[createddtstamp] [varchar](25) NOT NULL,
	[updatedBy] [int] NULL,
	[updateddtstamp] [varchar](25) NULL,
	[employeeId] [varchar](100) NULL,
	[CSVManagerId] [varchar](100) NULL,
	[phonenumber] [varchar](25) NULL
);

CREATE TABLE [dbo].[DeviceRegistration](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserID] [int] NOT NULL,
	[DeviceType] [varchar](50) NULL,
	[DeviceID] [varchar](max) NULL,
	[CreatedBy] [int] NULL,
	[CreatedDtStamp] [datetime] NULL,
 CONSTRAINT [PK_DeviceRegistration] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

ALTER TABLE [dbo].[DeviceRegistration]  WITH CHECK ADD  CONSTRAINT [FK_DeviceRegistration1_UserLogin1] FOREIGN KEY([UserID])
REFERENCES [dbo].[UserLogin] ([userid]);

ALTER TABLE [dbo].[DeviceRegistration] CHECK CONSTRAINT [FK_DeviceRegistration1_UserLogin1];

CREATE TABLE [dbo].[UserSmsVerificationCode](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmpID] [int] NOT NULL,
	[VerificationCode] [int] NOT NULL,
	[VerificationCodeTimeout] [int] NOT NULL,
	[Bactive] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreateddtStamp] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdateddtStamp] [datetime] NULL,
 CONSTRAINT [PK_SMSVerificationId] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);


ALTER TABLE [dbo].[UserSmsVerificationCode]  WITH CHECK ADD  CONSTRAINT [FK_UserSmsVerification_EmpDetails_EmpId] FOREIGN KEY([EmpID])
REFERENCES [dbo].[EmployeeDetails] ([empid])
ON DELETE CASCADE

ALTER TABLE [dbo].[UserSmsVerificationCode] CHECK CONSTRAINT [FK_UserSmsVerification_EmpDetails_EmpId];

CREATE TABLE [dbo].[InactivityLog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[empid] [int] NOT NULL,
	[firstname] [nvarchar](100) NOT NULL,
	[lastname] [nvarchar](100) NOT NULL,
	[email] [nvarchar](100) NOT NULL,
	[smstext] [nvarchar](500) NOT NULL,
	[emailtext] [nvarchar](1000) NOT NULL,
	[phonenumber] [varchar](25) NOT NULL,
	[inactivitydays] [int] NOT NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL,
 CONSTRAINT [PK_LogId] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

ALTER TABLE [dbo].[InactivityLog]  WITH CHECK ADD  CONSTRAINT [FK_InActivityLog_Id] FOREIGN KEY([empid])
REFERENCES [dbo].[EmployeeDetails] ([empid]);

ALTER TABLE [dbo].[InactivityLog] CHECK CONSTRAINT [FK_InActivityLog_Id];


CREATE TABLE [dbo].[DimensionElements](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[DimensionId] [smallint] NOT NULL,
	[DimensionValueId] [smallint] NOT NULL,
	[DimensionValues] [varchar](50) NOT NULL,
	[BActive] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDtstamp] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDtstamp] [datetime] NULL,
 CONSTRAINT [PK_DimensionElements] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);


CREATE TABLE [dbo].[DimensionWiseActionPermission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DimensionId] [smallint] NOT NULL,
	[DimensionValueId] [smallint] NOT NULL,
	[ActionId] [smallint] NOT NULL,
	[CanView] [bit] NOT NULL,
	[CanAdd] [bit] NOT NULL,
	[CanEdit] [bit] NOT NULL,
	[CanDelete] [bit] NOT NULL,
	[BActive] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDtstamp] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDtstamp] [datetime] NULL,
 CONSTRAINT [PK_DimensionWiseActionPermission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

ALTER TABLE [dbo].[DimensionWiseActionPermission]  WITH CHECK ADD  CONSTRAINT [FK_DimensionWiseActionPermission_DimensionValueId] FOREIGN KEY([Id])
REFERENCES [dbo].[DimensionWiseActionPermission] ([Id]);

ALTER TABLE [dbo].[DimensionWiseActionPermission] CHECK CONSTRAINT [FK_DimensionWiseActionPermission_DimensionValueId];


CREATE TABLE [dbo].[Classifications](
	[Id] [smallint] IDENTITY(1,1) NOT NULL,
	[Classification] [varchar](500) NOT NULL,
	[Bactive] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDtstamp] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDtstamp] [datetime] NULL,
CONSTRAINT [PK_Classifications] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

CREATE TABLE [dbo].[EmployeeSparkDetails](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmpId] [int] NOT NULL,
	[SparkDate] [datetime] NOT NULL,
	[SparkBy] [int] NOT NULL,
	[CategoryId][SMALLINT] NULL,
	[ClassificationId] [smallint] NULL,
	[Remarks] [nvarchar](4000) NULL,
	[DocumentPath] [varchar](1000) NULL,
	[ViaSms] [bit] NOT NULL,
	[SenderPhoneNumber][VARCHAR](25) NULL,
	[BActive] [bit] NOT NULL,
	[ApprovalStatus] [int] Not Null,
	[ApprovalBy] [int] Null,
	[ApprovalDate] [datetime] NULL,
	[RejectionRemark] [nvarchar](4000)  NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDtstamp] [datetime] NOT NULL,
	[Updatedby] [int] NULL,
	[UpdatedDtstamp] [datetime] NULL,
	[CloudFilePath] varchar(500) NULL
 CONSTRAINT [PK_EmployeeSparkDetails_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

ALTER TABLE [dbo].[EmployeeSparkDetails]  WITH CHECK ADD  CONSTRAINT [FK_EmployeeSparkDetails_EmpId] FOREIGN KEY([empid])
REFERENCES [dbo].[EmployeeDetails] ([empid]);

ALTER TABLE [dbo].[EmployeeSparkDetails] CHECK CONSTRAINT [FK_EmployeeSparkDetails_EmpId];

CREATE TABLE [dbo].[TeamConfiguration](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](500) NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[TriggerActivityDays] [int] NOT NULL,
	[Status] [bit] NOT NULL,
	[BActive] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDtstamp] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDtstamp] [datetime] NULL,
 CONSTRAINT [PK_TeamConfiguration] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

CREATE TABLE [dbo].[TeamManagers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TeamId] [int] NOT NULL,
	[ManagerId] [int] NOT NULL,
	[BActive] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDtstamp] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDtstamp] [datetime] NULL,
 CONSTRAINT [PK_TeamManagers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

ALTER TABLE [dbo].[TeamManagers]  WITH CHECK ADD  CONSTRAINT [fk_TeamManagers_ManagerId] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[EmployeeDetails] ([empid]);

ALTER TABLE [dbo].[TeamManagers] CHECK CONSTRAINT [fk_TeamManagers_ManagerId];

CREATE TABLE [dbo].[TeamEmployees](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TeamId] [int] NOT NULL,
	[EmpId] [int] NOT NULL,
	[BActive] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDtstamp] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDtstamp] [datetime] NULL,
 CONSTRAINT [PK_TeamEmployees] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);

ALTER TABLE [dbo].[TeamEmployees]  WITH CHECK ADD  CONSTRAINT [fk_TeamEmployees_EmpId] FOREIGN KEY([EmpId])
REFERENCES [dbo].[EmployeeDetails] ([empid]);

ALTER TABLE [dbo].[TeamEmployees] CHECK CONSTRAINT [fk_TeamEmployees_EmpId];


CREATE TABLE [dbo].[TeamInactivityLog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[teamid] [int] NOT NULL,
	[emailto] [nvarchar](1000) NOT NULL,
	[emailtext] [nvarchar](1000) NOT NULL,
	[triggeractivitydays] [int] NOT NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL,
 CONSTRAINT [PK_TeamLogId] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
);