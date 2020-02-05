CREATE OR ALTER PROCEDURE [dbo].[usp_GetAllCategories]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT categoryid, category FROM questioncategories WHERE bactive = 1
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetAllQuestAnswers]
AS
BEGIN
	DECLARE @questionid INT 
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
SELECT CASE WHEN rowno = 1 THEN category ELSE '' END AS category, CASE WHEN rowno = 1 THEN questions ELSE '' END AS questions,
answer, weightage, questionid, categoryid, answerid
FROM
(
	SELECT ROW_NUMBER() OVER (PARTITION BY q.id ORDER BY  q.id) AS rowno,  
		c.category, q.questions, a.answer, a.weightage, q.id as questionid, c.categoryid, a.id as answerid  FROM questionsdirectory q 
		LEFT JOIN questioncategories c ON c.categoryid = q.categoryid AND c.bactive = 1
		LEFT JOIN answersconfig a ON a.questionid  = q.id AND a.bactive = 1
	WHERE q.bactive = 1 and answer IS NOT NULL 
) v 
ORDER BY categoryid
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetAllQuestions]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT c.category, q.questions, q.id as questionid, c.categoryid  FROM questionsdirectory q 
	LEFT JOIN questioncategories c ON c.categoryid = q.categoryid AND c.bactive = 1
	WHERE q.bactive = 1
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetAnswersByQuestionId]
(
	@questionid INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT answer FROM answersconfig WHERE questionid = @questionid AND bactive = 1
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetQuestionnariesByCategory]
(
	@categoryid SMALLINT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT c.category, q.questions FROM  questionsdirectory q 
	LEFT JOIN questioncategories c ON c.categoryid =q.categoryid  AND c.bactive = 1
	WHERE q.bactive = 1 AND q.categoryid = @categoryid 

END

----------
CREATE  OR ALTER PROCEDURE [dbo].[usp_AddEmpAssessment]
(
	    @empid INT,
        @assessmentdate DATETIME,
        @assessmentby INT,
		@assessmentPeriod VARCHAR(50),
		@remarks NVARCHAR(MAX),
		@GeneralStatus INT,
        @CreatedBy INT,
		@id INT OUT 
		
)	
AS
BEGIN
IF NOT EXISTS(SELECT * FROM employee_assessment WHERE empid = @empid AND  Convert(date,assessmentdate,110) = Convert(date,GETDATE(),110))-- assessmentdate = @assessmentdate)-- assessmentPeriod = @assessmentPeriod) --assessmentby =@assessmentby AND 
	BEGIN
	-- Insert statements for procedure here
			INSERT INTO employee_assessment
			(
										 empid,
										 assessmentdate,
										 assessmentby,
										 assessmentPeriod,
										 remarks,
										 GeneralStatus,
										 bactive,
										 CreatedBy,
										 Createddtstamp
        
			 )
			 VALUES 
			(
										@empid,
										@assessmentdate,
										@assessmentby,
										'Quarterly',
										@remarks,
										@GeneralStatus,
										1,
										@CreatedBy,
										GETDATE()
        
			);

			 SET @id =SCOPE_IDENTITY() 
			SELECT @id;
	
	END
END
----------
CREATE  OR ALTER PROCEDURE [dbo].[usp_AddEmpAssessmentDetails]  
 -- Add the parameters for the stored procedure here 
 (
	 @assessmentid INT,
	 @questionid INT,
	 @answerid int,
	 @remarks NVARCHAR(MAX),
	 @KpiStatus INT,
	 @CreatedBy INT,
	 @result INT OUT
 )
AS 
BEGIN 

IF NOT EXISTS(SELECT * FROM assessmentdetails WHERE assessmentid = @assessmentid AND questionid = @questionid)
	BEGIN
		 INSERT INTO [dbo].[assessmentdetails]
		(
		assessmentid,
		questionid,
		answerid,
		remarks,
		KpiStatus,
		bactive,
		CREATEdby,
		CREATEddtstamp
		)
		VALUES
		(
		@assessmentid,
		@questionid,
		@answerid,
		@remarks,
		@KpiStatus,
		1,
		@CREATEdby,
		GETDATE()
		) 
	SET @result = 1
	SELECT @result
	END
	ELSE
		BEGIN
		SET @result =0 
		SELECT @result
		END
    
END
----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GetAssessmentScoreById] 
(
	@AssessmentId INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--Commented by Vivek Bhavsar on 25-12-2018

    -- Insert statements for procedure here
	SELECT a.id as assessmentId,
		   e.CompanyId, 
		   e.Empid, 
		   dbo.[GetEmpNameById](null,e.firstname,e.middlename,e.lastname) AS EmpName,
		   a.score AS empScore,
		   d.scorerank AS empScoreRank, 
		   d.GeneralScoreRank, 
		   d.ScoreRemarks,
		   d.ManagerAction,
		   d.ScoreSummary, 
		   a.assessmentdate AS RatingDate, 
		   a.Assessmentby AS Assessmentbyid,
		   dbo.[GetEmpNameById](u.empid,null,null,null) AS Assessmentby
	FROM employee_assessment  a
	LEFT  JOIN employeedetails e ON e.empid= a.empid
	INNER JOIN userlogin u ON a.Assessmentby = u.userid
	LEFT JOIN scoreremarks d ON a.scoreid= d.id
	WHERE a.id=@AssessmentId
END

----------
CREATE OR ALTER      PROCEDURE [dbo].[usp_GetEmpDashboard] 
(
@EmpId INT
)
AS
BEGIN
 
SELECT CASE WHEN rowno = 1 THEN empid ELSE 0 END AS empid,
CASE WHEN rowno = 1 THEN lastscore ELSE 0 END AS lastscore,
CASE WHEN rowno = 1 THEN lastAssessedDate ELSE '' END AS lastAssessedDate,
CASE WHEN rowno = 1 THEN lastrank ELSE '' END AS lastrank,
CASE WHEN rowno = 1 THEN noofcount ELSE 0 END AS noofcount,
CASE WHEN rowno = 1 THEN noofcountlyr ELSE 0 END AS lyrNoOfCount,
CASE WHEN rowno = 1 THEN CurrAvgScore ELSE 0 END AS CurrAvgScore, CASE WHEN rowno = 1 THEN CurrAvgScoreRank ELSE '' END AS CurrAvgScoreRank,
CASE WHEN rowno = 1 THEN lyrScore ELSE 0 END AS lyrAvgScore,
CASE WHEN rowno = 1 THEN lyrAvgScoreRank ELSE '' END AS lyrAvgScoreRank,
CASE WHEN rowno = 1 THEN lastscoreremarks ELSE '' END AS lastscoreremarks,
CASE WHEN rowno = 1 THEN lastManagerAction ELSE '' END AS lastManagerAction,
CASE WHEN rowno = 1 THEN lastScoreSummary ELSE '' END AS lastScoreSummary,
CASE WHEN rowno = 1 THEN lastGeneralScoreRank ELSE '' END AS lastGeneralScoreRank,
CASE WHEN rowno = 1 THEN empname ELSE '' END AS empName,
 --CASE WHEN rowno = 1 THEN ISNULL(remarks,'') ELSE '' END AS GeneralRemark,
--Category, ISNULL(remark,'') AS remark,ISNULL(status,0) AS status,
ISNULL(remarks,'') AS GeneralRemark, GeneralDocPath,GeneralCloudFilePath,
ISNULL(GeneralStatus,0) AS GeneralStatus, ISNULL(category,'') AS Category, ISNULL(kpiRemarks,'') AS remark,CloudFilePath, ISNULL(DocumentPath,'')AS DocumentPath,ISNULL(kpiStatus,0) AS status,
AssessmentId, RemarkId, assessmentdate, assessmentbyId, FirstName,LastName, empimgpath AS AssessmentByImgPath, monyrid, monyr, dayscore, dbo.GETSCORERANK(dayscore) AS dayScoreRank,
CASE WHEN Format(assessmentdate, 'yyyyMM') > FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM') THEN MonthAvgScore ELSE 0 END AS MonthAvgScore,
CASE WHEN Format(assessmentdate, 'yyyyMM') > FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM') THEN dbo.GETSCORERANK(MonthAvgScore) ELSE '' END AS MonthAvgScoreRank,
weekno, CAST(wkno AS INT) AS wkno,
CASE WHEN CONVERT(DATE, assessmentdate,103) >=CONVERT(DATE, DATEADD(MONTH , -12 , GETDATE() ),103) AND CONVERT(date, assessmentdate,103) <=CONVERT(date, GETDATE(),103) THEN WeekAvgScore ELSE 0 END AS WeekAvgScore,
CASE WHEN CONVERT(DATE, assessmentdate,103) >=CONVERT(DATE, DATEADD(MONTH , -12 , GETDATE() ),103) AND CONVERT(date, assessmentdate,103) <=CONVERT(date, GETDATE(),103) THEN dbo.GETSCORERANK(WeekAvgScore) ELSE '' END AS WeekAvgScoreRank,
year, yearId,  YearAvgScore, dbo.GETSCORERANK(YearAvgScore) AS YeatAvgScoreRank,
ISNULL(GeneralUpddtstamp,assessmentdate) AS GeneralUpddtstamp , ISNULL(KpiUpddtstamp, assessmentdate) AS KpiUpddtstamp
FROM
(
SELECT DISTINCT ROW_NUMBER() OVER (PARTITION BY at.empid ORDER BY at.id) AS rowno, v.score AS lastscore, v.scorerank AS lastrank, ISNULL(CONVERT(VARCHAR,v.RatingDate, 101),'') AS lastAssessedDate,
v.empname, v.noofcount,v.noofcountlyr, v.empid, v.curravgscorerank, v.curravgscore, ISNULL(v.lyrscore, 0) AS lyrscore,
CASE WHEN v.lyrScore IS NULL THEN '' ELSE dbo.GETSCORERANK(lyrScore) END AS lyrAvgScoreRank,  
v.scoreremarks AS lastscoreremarks, v.ManagerAction AS lastManagerAction, v.ScoreSummary AS lastScoreSummary, v.GeneralScoreRank AS lastGeneralScoreRank,
at.remarks, at.GeneralStatus,
at.assessmentdate, FORMAT(at.assessmentdate,'yyyyMM') AS monyrid, FORMAT(at.assessmentdate, 'MMM-yyyy') AS monyr, e.score AS dayscore,
CAST(ROUND(AVG(e.score) OVER (PARTITION BY FORMAT(at.assessmentdate,'yyyyMM')),0) AS DECIMAL(10,0)) AS MonthAvgScore,
'Week' + CAST(DATEPART(wk,at.assessmentdate) AS VARCHAR) + '-' + CAST(YEAR(assessmentdate) AS VARCHAR) AS weekno,
CAST(DATEPART(wk,at.assessmentdate) AS int) AS wkno,
CAST(ROUND(AVG(e.score) OVER (PARTITION BY YEAR(at.assessmentdate), DATEPART(wk,at.assessmentdate)),0) AS DECIMAL(10,0)) AS WeekAvgScore,
'Year' + ': ' + CAST(DATEPART(yyyy,at.assessmentdate) AS VARCHAR) AS year,
DATEPART(yyyy,at.assessmentdate) AS yearid,
CAST(ROUND(AVG(e.score) OVER (PARTITION BY DATEPART(yyyy,at.assessmentdate)),0) AS DECIMAL(10,0)) AS YearAvgScore,
at.assessmentby AS assessmentbyId,ed.FirstName,ed.LastName, ed.empimgpath,  kpi.category,  kpi.kpiRemarks, kpi.kpiStatus,
ISNULL(at.generaldocpath,'') AS GeneralDocPath,kpi.DocumentPath,at.id AS AssessmentId,ISNULL(kpi.questionid,0) AS RemarkId,
at.updateddtstamp AS GeneralUpddtstamp, kpi.KpiUpddtstamp,ISNULL(at.CloudFilePath,'') AS GeneralCloudFilePath,ISNULL(kpi.CloudFilePath,'') AS CloudFilePath
FROM employee_assessment at
INNER JOIN
(
SELECT     l.score,[dbo].[GetScoreRankById](l.scoreID) ScoreRank,scoreID, l.empName, noofcount,noofcountlyr, l.empid, dbo.GETSCORERANK(vg.avgscore) AS curravgscorerank, vg.avgscore AS curravgscore,
CAST(ROUND(lyrScore,0) AS DECIMAL(10,0)) AS lyrScore, l.RatingDate , r.scoreremarks, r.ManagerAction, r.ScoreSummary, r.GeneralScoreRank
FROM
(
SELECT CAST(ROUND(AVG(a.Score),0) AS  DECIMAL(2,0)) AS  SCORE,[dbo].[GetScoreRankID](CAST(ROUND(AVG(a.Score),0) AS INT)) As scoreid,Empid,CONVERT(DATE,a.RatingDate,103) AS RatingDate,a.empName
FROM vw_EmpLastAssessmentDet a WHERE EMPID=@empid AND CONVERT(DATE,a.RatingDate,103)=(select CONVERT(DATE,MAX(e.assessmentdate),103) FROM employee_assessment e
WHERE empid=@empid GROUP BY EMPID)
GROUP BY EMPID,CONVERT(DATE,a.RatingDate,103),EmpName
)l  
LEFT JOIN ScoreRemarks r ON R.Id=l.scoreid
LEFT JOIN
(
 SELECT empid,SUM(noofcount) noofcount, SUM(noofcountlyr)noofcountlyr
 FROM
 (
SELECT  COUNT(1)AS noofcount,0 AS noofcountlyr, empid FROM employee_assessment
WHERE empid=@empid and DATEPART(yyyy,assessmentdate) = DATEPART(yyyy,GETDATE())
GROUP BY empid
UNION
SELECT  0 AS noofcount,COUNT(1) AS noofcountlyr, empid FROM employee_assessment
WHERE empid=@empid and DATEPART(yyyy,assessmentdate) = DATEPART(yyyy,DATEADD(yyyy,-1,GETDATE()))
GROUP BY empid
 )a
 GROUP BY empid
) v1 ON v1.empid = l.empid
LEFT JOIN (
SELECT AVG(Score) AS lyrScore, empid FROM vw_EmpLastAssessmentDet
WHERE DATEPART(yyyy,Ratingdate) = DATEPART(yyyy,DATEADD(yyyy,-1,GETDATE()))
AND empid = @empid
GROUP BY empid
) k ON k.empid = l.empid
LEFT JOIN vw_EmpAverageAssessmentDetForDashBoard vg ON vg.empid = l.empid and DATEPART(yyyy,GETDATE()) = vg.yearid
) v ON at.empid = v.empid
LEFT JOIN vw_EmpAssessmentScore e ON e.ratingdate = at.assessmentdate and e.empid = at.empid
LEFT JOIN userlogin u ON u.userId = at.assessmentby
LEFT JOIN employeedetails ed ON ed.empid = u.empid
LEFT JOIN
(
SELECT ad.assessmentid, CASE WHEN ad.questionid = 13 THEN '#1 Point of Frustration' ELSE qc.category END AS category, ad.questionid, ad.KPIStatus AS kpiStatus,  ISNULL(ad.remarks,'') AS kpiRemarks, DocumentPath,
ad.updateddtstamp AS KpiUpddtstamp,ad.CloudFilePath
FROM assessmentdetails ad
INNER JOIN questionsdirectory qd ON qd.bactive = 1 AND qd.id = ad.questionId
INNER JOIN questioncategories qc ON qc.bactive = 1 AND qc.categoryId = qd.categoryId
WHERE (ISNULL(remarks,'') <> '' OR ISNULL(documentpath,'') <>'') OR ISNULL(CloudFilePath,'') <>''
) kpi ON kpi.assessmentid = at.id
) v
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GetEmpLastAssessmentDetails] 
(
	@EmpId INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT k.empId, k.assessmentBy, k.assessmentDate, k.category, -- CASE WHEN rowno = 1 THEN k.category ELSE '' END AS category,
k.questions, --CASE WHEN rowno = 1 THEN k.questions ELSE '' END AS questions,
k.answer, k.weightage, k.questionId, k.categoryId, k.answerId, k.GeneralRemarks, k.GeneralStatus, k.KPIRemarks, k.KPIStatus, k.IsSelected 
FROM
(
	SELECT ROW_NUMBER() OVER (PARTITION BY q.id ORDER BY  q.id) AS rowno,  
		c.category, q.questions, a.answer, CAST(a.weightage AS INT) AS weightage, q.id as questionid, c.categoryid, a.id as answerid,
		ae.empid, ae.assessmentdate, ae.assessmentby, ae.remarks AS GeneralRemarks, ISNULL(ae.GeneralStatus,0) AS GeneralStatus,
		ad.assessmentid, CASE WHEN a.questionid =  ad.questionid AND a.id = ad.answerid THEN ad.remarks ELSE '' END AS KPIRemarks, ISNULL(ad.KPIStatus,0) AS KPIStatus,
		CASE WHEN a.questionid =  ad.questionid AND a.id = ad.answerid THEN 1 ELSE 0 END AS IsSelected
	FROM employee_Assessment ae
	LEFT JOIN assessmentdetails ad ON ad.assessmentid = ae.id
	LEFT JOIN questionsdirectory q ON q.bactive = 1 AND q.id = ad.questionId 
	LEFT JOIN questioncategories c ON c.categoryid = q.categoryid AND c.bactive = 1
	LEFT JOIN answersconfig a ON a.questionid  =  ad.questionid AND a.bactive = 1
	WHERE q.bactive = 1 and answer IS NOT NULL AND empid = @EmpId
	GROUP BY c.category, q.questions, a.answer, a.weightage, q.id, c.categoryid, a.id,
	ae.empid, ae.assessmentdate, ae.assessmentby, ae.remarks,ISNULL(ae.GeneralStatus,0),
	ad.assessmentid,  CASE WHEN a.questionid =  ad.questionid AND a.id = ad.answerid THEN ad.remarks ELSE '' END, ISNULL(ad.KPIStatus,0),
	CASE WHEN a.questionid =  ad.questionid AND a.id = ad.answerid THEN 1 ELSE 0 END
	HAVING CONVERT(DATE, ae.assessmentdate ,101) = (SELECT MAX(CONVERT(DATE, assessmentdate ,101)) FROM employee_Assessment WHERE empid = @EmpId) 
) k
ORDER BY k.categoryid
END
----------

CREATE OR ALTER PROCEDURE [dbo].[usp_AddCompanyAndRoleWiseWidget]
(
 @CompanyId INT,
 @RoleId INT,
 @WidgetTypeId INT,
 @WidgetId INT,
 @updatedBy INT,
 @result INT OUT
)
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;
--IF NOT EXISTS(SELECT * FROM department WHERE departmentid = @DepartmentId AND bactive = 1)
--IF EXISTS(SELECT * FROM UserWiseWidgetDetails WHERE WidgetId = @WidgetId)
--BEGIN
	DELETE u FROM UserWiseWidgetDetails u
	INNER JOIN userlogin ug ON ug.userid = u.UserId 
	INNER JOIN EmployeeDetails e ON ug.empid = e.empid 
	INNER JOIN WidgetMaster w ON w.id = u.widgetid
	WHERE e.companyid = @CompanyId AND e.roleid = @roleid
	AND w.WidgetTypeId = @WidgetTypeId 
--END
	 INSERT INTO CompanyAndRoleWiseWidget
	 (
	  CompanyID,
	  WidgetTypeId,
	  RoleId,
	  WidgetID,
	  bactive,
	  CREATEdby,
	  CREATEddtstamp
	 )
	 VALUES
	 (
	  @CompanyId,
	  @WidgetTypeId,
	  @RoleId,
	  @WidgetId,
	  1,
	  @updatedBy,
	  GETDATE()
	 )
	 SET @result = 1;
		SELECT @result;
	END
----------


CREATE OR ALTER PROCEDURE [dbo].[usp_DeleteCompanyAndRoleWiseWidget]
(
  @CompanyId INT,
  @WidgetTypeID INT,
  @RoleId INT,
  @result INT OUT
  
  
)
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 

    -- Insert statements for procedure here
 
 IF EXISTS (SELECT * FROM CompanyAndRoleWiseWidget WHERE companyId = @companyid AND RoleId = @RoleId)
 BEGIN
	  DELETE FROM CompanyAndRoleWiseWidget  
	  WHERE CompanyId =@companyId AND RoleId = @RoleId AND WidgetTypeId = @WidgetTypeID
	  SET @result = 1; 
	  SELECT @result;
 END
 ELSE
  BEGIN
	  SET @result = 0;
	  SELECT @result;
 END
END
----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GETCompanyAndRoleWiseWidget]
(
 @CompanyId INT,
 @RoleId TINYINT,
 @DashBoardTypeId INT
)
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;


IF (@RoleId > 0 AND @DashBoardTypeId > 0) 
BEGIN
	SELECT CASE WHEN rowno = 1 THEN WidgetType ELSE '' END AS DashBoardType, WidgetName,WidgetTypeId AS DashBoardTypeId,
	WidgetId, WidgetActualName, RoleId, Selected 
	FROM
	(
		 SELECT ROW_NUMBER() OVER (PARTITION BY wt.id ORDER BY  wt.id) AS rowno,  wt.WidgetType,  wm.Id AS WidgetId, wm.WidgetName, 
			wm.WidgetActualName, wm.WidgetTypeId, ISNULL(crw.RoleId, 0) AS RoleId, 
		 CASE WHEN crw.WidgetId = wm.id THEN 1 ELSE 0 END AS Selected
		 FROM [dbo].WidgetMaster wm
		 LEFT JOIN WidgetTypeMaster wt ON wm.WidgetTypeId = wt.id 
		 LEFT OUTER JOIN CompanyAndRoleWiseWidget crw ON wm.id = crw.WidgetId AND crw.companyid = @CompanyId AND crw.roleid = @RoleId and wm.WidgetTypeId = @DashBoardTypeId
		 AND crw.bactive = 1 
	 ) v
END
ELSE IF (@RoleId = 0 AND @DashBoardTypeId = 0)
BEGIN
	SELECT CASE WHEN rowno = 1 THEN WidgetType ELSE '' END AS DashBoardType, WidgetName,WidgetTypeId AS DashBoardTypeId,
	WidgetId, WidgetActualName,  0 AS RoleId, 0 AS Selected 
	FROM
	(
		 SELECT ROW_NUMBER() OVER (PARTITION BY wt.id ORDER BY  wt.id) AS rowno,  
			wt.WidgetType, wm.WidgetName,wt.id AS WidgetTypeId, wm.id As WidgetId, wm.WidgetActualName AS WidgetActualName 
		 FROM WidgetTypeMaster wt 
		 LEFT JOIN WidgetMaster wm ON wm.WidgetTypeId = wt.id 
	) v 
END
END
----------


CREATE OR ALTER PROCEDURE [dbo].[usp_AddUserWiseWidgetPosition]
(
	@UserId INT,
	@WidgetId INT,
	@SequenceNumber INT,
	@TileSequence INT,
	@Position DECIMAL(10,4),
	@IsActive BIT,
	@CREATEdBy INT,
	@Result int out
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
IF EXISTS(SELECT * FROM [dbo].[UserWiseWidgetDetails] WHERE userId = @UserId AND widgetId = @WidgetId)
BEGIN
	DELETE FROM [dbo].[UserWiseWidgetDetails] WHERE userId = @UserId AND widgetId = @WidgetId
END

	INSERT INTO [dbo].[UserWiseWidgetDetails]
	(
		userid,
		widgetid,
		sequencenumber,
		tileSequence,
		position,
		isactive,
		CREATEdby,
		CREATEddtstamp
	)
	VALUES
	(
		@UserId,
		@WidgetId,
		@SequenceNumber,
		@TileSequence,
		@Position,
		@IsActive,
		@CREATEdBy,
		GETDATE()
	)
	SET @result = 1
	SELECT @result;

END
----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GetUserWiseWidgetDet] 
(
	@UserId INT,
	@WidgetType INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here

IF NOT EXISTS(SELECT * FROM UserWiseWidgetDetails  uw INNER JOIN widgetMaster wm on wm.id = uw.widgetid WHERE UserId = @UserId AND wm.widgettypeid = @WidgetType) 
BEGIN
	SELECT @UserId AS UserId, wm.Id AS WidgetId, wm.WidgetName AS WidgetName, wm.WidgetactualName, CAST(1 AS BIT) AS IsActive,
	ROW_NUMBER() OVER (ORDER BY id) AS SequenceNumber, ROW_NUMBER() OVER (ORDER BY id) AS TileSequence,
	wm.Position FROM WidgetMaster wm WHERE wm.widgettypeid = @WidgetType
END
ELSE 
	BEGIN
		SELECT ISNULL(uw.UserId,0) AS UserId, wm.Id AS WidgetId, wm.WidgetName AS WidgetName, wm.WidgetactualName, ISNULL(CAST(uw.IsActive AS BIT),0) AS IsActive,
			ISNULL(uw.SequenceNumber, 0) AS SequenceNumber, ISNULL(uw.TileSequence, 0) AS TileSequence, ISNULL(uw.Position, wm.Position) AS Position
		FROM [dbo].[WidgetMaster] wm
		LEFT JOIN [dbo].[UserWiseWidgetDetails] uw ON wm.id = uw.WidgetId AND uw.UserID = @UserId
		WHERE wm.widgettypeid = @WidgetType
		ORDER BY wm.Id
	END	
END
----------
CREATE OR ALTER   PROCEDURE [dbo].[usp_GetAllEmployees] 
(
	@Companyid INT,
	@Managerid INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

IF(@managerid > 0 and @companyid > 0) 
BEGIN  
	
	SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, 
		   e.email, e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, e.empstatus, e.roleid, 
		   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid), 101),'') AS lastAssessedDate, 
		   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = DATEPART(yyyy, GETDATE())) AS VARCHAR),'') AS RatingCompleted,
		   ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
	FROM employeedetails e 
	LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
	LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
	WHERE e.companyid = @Companyid AND e.bactive = 1 AND managerid = @Managerid 
	AND e.empstatus = 1
	ORDER BY e.firstname
END
ELSE
BEGIN
	SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, 
		   e.email, e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, e.empstatus, e.roleid, 
		   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid), 101),'') AS lastAssessedDate, 
		   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = DATEPART(yyyy, GETDATE())) AS VARCHAR),'') AS RatingCompleted,
		   '' as managerLastAssessedDate
	FROM employeedetails e 
	LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
	WHERE e.companyid = @Companyid AND e.bactive = 1 
	AND e.empstatus = 1
	ORDER BY e.firstname
END
END

----------
CREATE  OR ALTER  PROCEDURE [dbo].[GetAllEmpDetByManager]
(
	@companyid INT,
	@managerid INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

IF(@managerid > 0 and @companyid > 0) 
BEGIN  
	SELECT e.empid, e.employeeId, e.companyid, '' AS companyName, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
			   e.workcity, e.workstate, e.workzipcode, e.departmentid, d.department, ISNULL(e.suffix, '') AS suffix, 
			   ISNULL(e.managerid,0) AS managerid , ISNULL(e.managername, '') AS managerfname, ISNULL(e.managerlname, '') AS managerlname, e.empstatus, e.roleid,
			    [dbo].[GetDimensionValueByID](1,e.roleid) AS role, CONVERT(DATE, e.dateofbirth,101) AS dateofbirth, e.raceorethanicityid, e.gender, ISNULL(e.jobcategoryid,0) AS jobcategoryid, ISNULL(e.jobcategory,'') AS jobcategory, 
			   ISNULL(e.jobgroupId,0) AS jobgroupId, ISNULL(e.jobgroup, '') AS jobgroup, ISNULL(e.jobcodeId,0) AS jobcodeId, ISNULL(e.jobcode,0) AS jobcode, CONVERT(DATE, e.lastpromodate ,101) AS lastpromodate, CONVERT(DATE, e.lastincdate,101) AS lastincdate,
			   ISNULL(e.raceorethanicityid, 0) AS raceorethanicityId, ISNULL(rc.raceorethnicity, '') AS raceorethanicity, e.currentsalary,e.bactive,
			   ISNULL(e.countryId, 0) AS countryId, ISNULL(c.country, '') AS country, e.emplocation, e.regionid, rg.region, ISNULL(e.empimgpath,'') AS empimgpath,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
		       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From vw_EmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = DATEPART(yyyy, GETDATE())) AS VARCHAR),'') AS RatingCompleted,
			    0 AS ord
	FROM employeedetails e
	LEFT JOIN
	(
		SELECT * from  employeedetails where  bactive = 1 AND empstatus = 1
	) n ON n.companyid = e.companyid and n.empid  = e.managerid
	LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
	LEFT JOIN regions rg ON rg.regionid = e.regionid AND rg.bactive = 1
	LEFT JOIN country c ON c.id = e.countryid AND c.bactive = 1
	LEFT JOIN raceorethnicity rc ON rc.id = e.raceorethanicityid AND rc.bactive = 1
	LEFT JOIN vw_EmpAverageAssessmentDet vg ON vg.empId = e.empid AND vg.year = DATEPART(yyyy,GETDATE())
	WHERE  e.companyid = @companyid AND e.bactive = 1 AND (e.managerId = @managerId OR n.managerid = @managerId)
	AND e.empStatus = 1
	ORDER BY e.firstname
END

END
----------
CREATE  OR ALTER PROCEDURE [dbo].[usp_GetCompanyWiseEmployeeList]
(
	@companyid INT
)
AS
BEGIN
	SELECT e.empid, e.employeeId, e.companyid, '' AS companyName, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, 
	       ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
	       e.workcity, e.workstate, e.workzipcode,ISNULL(e.suffix, '') AS suffix,  e.departmentid, d.department,  ISNULL(e.managerid,0) AS managerid,
 	       e.empstatus, e.roleid,[dbo].[GetDimensionValueByID](1,e.roleid) AS role, CONVERT(DATE, e.dateofbirth,101) AS dateofbirth, e.raceorethanicityid, e.gender, 
	       ISNULL(e.jobcategoryid,0) AS jobcategoryid, ISNULL(e.jobcategory,'') AS jobcategory, 
	       ISNULL(e.jobgroupId,0) AS jobgroupId, ISNULL(e.jobgroup, '') AS jobgroup, ISNULL(e.jobcodeId,0) AS jobcodeId, ISNULL(e.jobcode,'') AS jobcode, 
               CONVERT(DATE, e.lastpromodate ,101) AS lastpromodate,
	       CONVERT(DATE, e.lastincdate,101) AS lastincdate,
	       ISNULL(e.raceorethanicityid, 0) AS raceorethanicityId, e.currentsalary,  ISNULL(e.countryId, 0) AS countryId, 
	       e.emplocation, ISNULL(e.regionid, 0) AS regionid
	FROM employeedetails e 
	LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
	WHERE e.companyid = @companyid AND e.bactive = 1 AND e.roleId <> 5 AND e.empstatus = 1
	ORDER BY e.firstname
END

----------
CREATE OR ALTER PROCEDURE [dbo].[usp_AddDepartment] 
(
	@companyId INT,
	@department VARCHAR(100),
	@CREATEdby int,
	@result INT OUT
)	
AS
BEGIN
	
	--DECLARE @lastinserted INT;
	
	    -- Insert statements for procedure here
IF EXISTS(SELECT * FROM [dbo].[department] WHERE department=@department AND bactive = 0 AND companyId = @CompanyId)
BEGIN
	UPDATE department SET bActive = 1 WHERE department=@department AND companyId = @CompanyId
	SET @result=1
END
ELSE
IF NOT EXISTS(SELECT * FROM [dbo].[department] WHERE department=@department AND bactive = 1 AND companyId = @CompanyId)
BEGIN
	INSERT INTO department
	(
								  companyId,	
								  department, 
								  bactive,
								  CREATEdby, 
								  CREATEddtstamp  
	 )
	 VALUES 
	(
								 @companyId,
								 @department, 
								 1,
								 @CREATEdby,
								 GETDATE() 
	);
	SET @result = 1;
END
ELSE
	BEGIN
		SET @result=0
	END
	
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateDepartment] 
(
	@CompanyId INT,
	@departmentid INT,
	@department VARCHAR(100),
	@updatedby INT,
	@result INT OUTPUT
) 
AS
BEGIN
IF EXISTS( SELECT * FROM [dbo].[department] WHERE departmentId=@departmentId AND department = @department AND CompanyId = @CompanyId AND bactive = 1)
BEGIN 
	 SET @result = 1;
	  SELECT @result;
END
ELSE
--IF EXISTS(SELECT * FROM [dbo].[department] WHERE departmentId=@departmentId AND CompanyId = @CompanyId AND bactive = 1)
--BEGIN
	IF EXISTS(SELECT * FROM [dbo].[department] WHERE department=@department AND departmentid <> @departmentId AND bactive = 0)
	BEGIN
		UPDATE department 
			SET bactive=1,
				updatedby=@updatedby, 
				updateddtstamp=GETDATE()    
		WHERE department = @department
		SET @result = 0;
		SELECT @result;
	END
	ELSE IF EXISTS(SELECT * FROM [dbo].[department] WHERE department=@department AND departmentid <> @departmentId AND bactive = 1)
	BEGIN
		SET @result = 0;
		SELECT @result;
	END
	ELSE
	BEGIN
	  UPDATE department SET department=@department,
	         bactive=1,
	         updatedby=@updatedby, 
	         updateddtstamp=GETDATE()    
	  WHERE departmentid = @departmentid    
	  AND CompanyId = @CompanyId      
      SET @result = 1;
	  SELECT @result;
END
--END
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_DeleteDepartment]
(
  @companyId INT,
  @Departmentid INT, 
  @Updatedby  INT,
  @Result INT OUTPUT
)
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 

    -- Insert statements for procedure here

 IF EXISTS (SELECT * FROM department WHERE departmentid = @departmentid AND bactive = 1 AND companyId = @companyId)
 BEGIN
	IF NOT EXISTS(SELECT * FROM employeedetails WHERE departmentid = @departmentid AND companyId = @companyId)
	BEGIN
    
		  UPDATE department SET bactive  = 0,
			updatedby = @updatedby,
			updateddtstamp = GETDATE()
		  WHERE departmentid = @departmentid  
		  AND companyId = @companyId
		  SET @result = 1;  ----Sucessfully  Deleted
		  SELECT @result;
 
	END
ELSE
	BEGIN
	  SET @result = 0;
	  SELECT @result;  ---------Can't deleted 
END
 END
 END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetAlldepartment]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT departmentid, department FROM department WHERE bactive = 1
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GETCompanyWiseDepartment] 
(
	@CompanyId INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT ISNULL(d.companyId,0 ) AS Companyid, d.departmentid AS mstDepartmentId, d.department, ISNULL(d.departmentId,0) AS departmentid,
	CAST(d.bactive AS INT) AS isSelected
	FROM [dbo].department d
	WHERE d.bactive = 1
END
----------
CREATE  OR ALTER  PROCEDURE [dbo].[usp_GetAllMastersForCSV] 
(
	@CompanyId INT
)
AS
BEGIN
SET NOCOUNT ON;

SELECT countryid, country
FROM
(
	SELECT 0 AS countryId, 'Select Country' AS Country, 0 AS ord 
	UNION
	SELECT id AS countryid, country, 1 AS ord  FROM country WHERE bactive = 1
)c ORDER BY ord, country
--Region List
SELECT region, regionid FROM
(
	SELECT 'Select Region' AS region, 0 AS regionid, 0 AS ord 
	UNION
	SELECT DISTINCT region,regionId, 1 AS ord FROM regions WHERE bactive = 1
) rg
ORDER BY ord, region
---raceorethnicity List
SELECT id, raceorethnicity
FROM
(
	SELECT 0 AS id, 'Select ethnicity' as raceorethnicity, 0 AS ord 
	UNION
	SELECT id AS id, raceorethnicity, 1 AS ord  FROM [dbo].[raceorethnicity] WHERE bactive = 1
) rc
ORDER BY ord, raceorethnicity
---DepartMent List
SELECT departmentId, department
FROM
(
	SELECT 0 AS departmentId, 'Select Department' AS Department, 0 AS ord
	UNION
	SELECT departmentid, department, 1 AS ord 
	from department
	WHERE bactive = 1 AND companyId = @CompanyId
) d
ORDER BY ord, department
---Role List
SELECT roleid, Role
FROM
(
	SELECT 0 AS roleid, 'Select Role' AS Role, 0 AS ord 
	UNION
	SELECT dimensionvalueid AS roleid, dimensionvalues AS role,1 AS ord  FROM [dbo].[dimensionelements] WHERE dimensionid=1 AND dimensionvalueid <> 1 AND bactive = 1
) r
ORDER BY ord, Role
---Manager List
SELECT ManagerEmpId, ManagerId + ': ' + ManagerName AS ManagerId, ManagerName
FROM
(
	SELECT 0 AS ManagerEmpID, 'Select Manager'  AS ManagerId, '' AS ManagerName, 0 AS ord 
	UNION
	SELECT empid AS ManagerEmpId, employeeid AS ManagerId, CONCAT(FirstName, CASE WHEN MiddleName <> '' THEN ' ' + MiddleName + ' ' ELSE ' ' END, LastName) AS ManagerName, 1 AS ord 
	FROM employeedetails
	WHERE companyid = @CompanyId  and bactive = 1 and empstatus = 1 AND roleid NOT IN (1,5) 
) m
ORDER BY ord, ManagerId
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GetCSVNewEmployees]
(
	@companyid INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT  empid, companyid, firstName, middleName,lastName, suffix, email, ISNULL(phonenumber, '') AS phonenumber, jobTitle, joiningDate,
			workCity, workState, workZipcode, departmentId,managerId,managername,managerlname,
			empStatus, roleId,dateOfBirth,raceorethanicityId,jobGroupid, gender,jobCategoryId,
			empLocation,jobCodeid,lastPromodate,currentSalary,lastIncDate,countryid,regionId,
			empImgPath,bactive,CREATEdBy,CREATEdDtstamp,updatedBy,updateddtstamp,employeeid,
			ISNULL(ismailSent, 0) AS ismailSent
	FROM employeedetails where companyid = @companyid
END

----------
CREATE OR ALTER PROCEDURE [dbo].[usp_DeleteTempEmployee] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	TRUNCATE TABLE tmpEmployeedetails
END

----------

CREATE OR ALTER   PROCEDURE [dbo].[usp_GetAllEmployeesWithPagination] 
(
	@companyid INT,
	@managerid INT,
	@PageNumber INT,
	@PageSize   INT,
	@SearchString NVARCHAR(256),
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
SET NOCOUNT ON;
DECLARE @curryear INT = DATEPART(yyyy,GETDATE());

DECLARE @TotalRowCount INT

SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

SELECT * INTO #tempEmpAverageAssessmentDet
FROM
(
	SELECT ISNULL(AvgScoreRank, '') AS AvgScoreRank, ISNULL(AvgScore,0) AS AvgScore, empId, year FROM vw_EmpAverageAssessmentDet WHERE year = @curryear
)av

SELECT * INTO #tempEmpLastAssessmentDet
FROM
(
	SELECT empid, ScoreRank, RatingDate FROM vw_EmpLastAssessmentDet 
)ve

IF(@managerid > 0 and @companyid > 0) 
BEGIN  
	
	SELECT empid, employeeId, companyid, firstname, middlename,  lastname, email,  jobtitle, joiningdate,
		   departmentid, managerid, managerFName, managerLName, empstatus, roleid, IsMailSent, currentsalary,empImgPath
	INTO #tempEmployeedetailsMC 
	FROM
	(
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
			   e.departmentid, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, ISNULL(e.IsMailSent,0) AS IsMailSent,
			   e.currentsalary,e.empImgPath
		FROM EmployeeDetails e
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
		WHERE e.companyid = @companyid AND e.bactive = 1 AND e.managerid = @managerid  
		AND e.empstatus = 1 AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
	) v

	SELECT * INTO #tempEmployeeResultMCE
	FROM(		
		SELECT e.empid, e.employeeId, e.companyid, '' AS companyName, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email,
			   e.managerFName, e.managerLName,
			   ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
			   e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid,e.empstatus, e.roleid,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
			   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid) AS VARCHAR),'') AS RatingCompleted,
				0 AS ord, e.IsMailSent, e.currentsalary,e.empImgPath,
				ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM #tempEmployeedetailsMC e 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid
		LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
	)MCE
		SELECT @TotalRowCount = COUNT(empid) FROM #tempEmployeeResultMCE
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount,empid, employeeId, companyid, companyName, firstname, middlename, lastname, 
		       managerFName, managerLName,
		       jobtitle, joiningdate, departmentid, department, managerid, empstatus, roleid,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   lastScoreRank, RatingCompleted, IsMailSent, currentsalary,empImgPath,managerLastAssessedDate
		FROM #tempEmployeeResultMCE
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY firstname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY 
END
ELSE if (@managerid < 0 and @companyid > 0) 
BEGIN
		
		SELECT empid, employeeId, companyid, firstname, middlename,  lastname, email, jobtitle, joiningdate,
		   departmentid, managerid, managerFName, managerLName, empstatus, roleid, IsMailSent, currentsalary,empImgPath 
		INTO #tempEmployeedetails 
		FROM
		(
			SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
			   e.departmentid, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, ISNULL(e.IsMailSent,0) AS IsMailSent, e.currentsalary,e.empImgPath
		    FROM Employeedetails e 
			LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
			WHERE e.companyid = @companyid AND e.bactive = 1 
			AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
		) v

	SELECT * INTO #tempEmployeeResult
	FROM
	(
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
			   e.managerFName, e.managerLName,
			   e.departmentid, d.department, e.managerid, e.empstatus, e.roleid,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
			   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid) AS VARCHAR),'') AS RatingCompleted,
			   CASE WHEN e.managerid = ABS(@managerid) THEN 0 ELSE 1 END AS ord, IsMailSent, e.currentsalary,empImgPath,
			   ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM #tempEmployeedetails e 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid 
		LEFT JOIN GetLastAssessmentDateByManager(ABS(@Managerid)) assmnt on assmnt.empid=e.empid
		GROUP BY e.empid, e.employeeId,e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle,  e.joiningdate,
				 e.departmentid, d.department, e.managerid, e.empstatus, e.roleid, e.managerFName, e.managerLName,
				ISNULL(vg.AvgScoreRank, ''),vg.AvgScore, IsMailSent, e.currentsalary,empImgPath,assmnt.assessmentdate
	)er
		SELECT @TotalRowCount = COUNT(empid) FROM  #tempEmployeeResult
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount, empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
			  managerFName, managerLName,
			  departmentid, department, managerid, empstatus, roleid,
			  lastAssessedDate, AvgScoreRank, AvgScore,
			  lastScoreRank, RatingCompleted,IsMailSent, currentsalary,empImgPath,managerLastAssessedDate
		FROM  #tempEmployeeResult
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY CASE WHEN managerid = ABS(@managerid) THEN 0 ELSE 1 END, empid DESC, firstname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY 
END
ELSE if (@managerid = 0 and @companyid > 0) 
BEGIN

	SELECT empid, employeeId, companyid, firstname, middlename,  lastname,  email, jobtitle, joiningdate,
		   departmentid, managerid, managerFName, managerLName, empstatus, roleid, IsMailSent, currentsalary,empImgPath 
	INTO #tempEmployeedetailsC 
	FROM
	(
			SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
			       e.departmentid, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, ISNULL(e.IsMailSent,0) AS IsMailSent, e.currentsalary,e.empImgPath
		    FROM employeedetails e 
			LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
			WHERE e.companyid = @companyid AND e.bactive = 1 AND e.roleid <> 4 AND e.empstatus = 1
			AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
	) v

	SELECT * INTO #tempEmployeeResultC
	FROM
	(
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.managerFName, e.managerLName, 
		       e.jobtitle, e.joiningdate, e.departmentid, d.department, e.managerid, e.empstatus, e.roleid,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
			   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid) AS VARCHAR),'') AS RatingCompleted,
			   0 AS ord, IsMailSent, currentsalary,e.empImgPath
		FROM #tempEmployeedetailsC e
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid 
	)C
		SELECT @TotalRowCount = COUNT(empid) FROM  #tempEmployeeResultC
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount, empid, employeeId, companyid, firstname, middlename, lastname, email, managerFName, managerLName, 
		       jobtitle, joiningdate, departmentid, department, managerid, empstatus, roleid, lastAssessedDate, AvgScoreRank, AvgScore,
			   lastScoreRank, RatingCompleted, IsMailSent, currentsalary,empImgPath
		FROM  #tempEmployeeResultC
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY empid DESC, firstname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY 
END

ELSE 
BEGIN
SELECT * INTO #tempTriggerEmployeedetails 
FROM
(
	SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
		   e.departmentid, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName,  e.empstatus, e.roleid, ISNULL(e.IsMailSent,0) AS IsMailSent, e.currentsalary ,e.empImgPath 
	FROM employeedetails e 
	LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
	LEFT JOIN companydetails cd ON cd.compid = e.companyId AND cd.bactive = 1
	WHERE e.bactive = 1 AND cd.bactive = 1 and e.roleid in (2)
	AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
) v

SELECT @TotalRowCount = COUNT(e.empid) FROM #tempTriggerEmployeedetails e 
LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
		OR e.lastname LIKE '%'+ @SearchString + '%'
		OR e.firstname LIKE '%'+ @SearchString + '%'
		OR e.firstname + ' ' + e.lastname LIKE '%'+ @SearchString + '%')

	SELECT @TotalRowCount as totalrowcount, e.empid, e.employeeId, e.companyid, cd.companyName, e.firstname, e.middlename, e.lastname, 
		   e.email, e.managerFName, e.managerLName, e.jobtitle, e.joiningdate, e.departmentid, d.department, e.managerid, e.empstatus, e.roleid,
		   '' AS lastAssessedDate, 0 as AvgScore, '' AS AvgScoreRank, '' AS lastScoreRank, '' AS RatingCompleted,
		   0 AS ord, IsMailSent, e.currentsalary,empImgPath
	FROM #tempTriggerEmployeedetails e
	LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
	LEFT JOIN companydetails cd ON cd.compid = e.companyId AND cd.bactive = 1
	WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR e.lastname LIKE '%'+ @SearchString + '%'
			OR e.firstname LIKE '%'+ @SearchString + '%'
			OR e.firstname + ' ' + e.lastname LIKE '%'+ @SearchString + '%')
		ORDER BY e.empid DESC, cd.companyName, e.firstname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY 
END
END


----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetCompanyWiseEmployeeListWithPagination] 
(
	@companyid INT,
	@PageNumber INT,
	@PageSize   INT,
	@SearchString NVARCHAR(256),
	@departmentlist VARCHAR(256)
)
AS
BEGIN

DECLARE @TotalRowCount INT
SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d
	SELECT * INTO #tempEmpAverageAssessmentDet
FROM
(
	SELECT * FROM vw_EmpAverageAssessmentDet WHERE year = DATEPART(yyyy,GETDATE()) 
)av

SELECT * INTO #tempEmpAssessmentScore
FROM
(
	SELECT * FROM vw_EmpLastAssessmentDet 
)ve

SELECT * INTO #tempEmployeedetailsMC
FROM
(
	SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate, 
			   e.workcity, e.workstate, e.workzipcode,ISNULL(e.suffix, '') AS suffix,  e.departmentid, ISNULL(e.managerid,0) AS managerid , e.empstatus, e.roleid,
			   '' AS role, CONVERT(DATE, e.dateofbirth,101) AS dateofbirth, e.gender, ISNULL(e.jobcategoryid,0) AS jobcategoryid, ISNULL(e.jobcategory,'') AS jobcategory, 
			   ISNULL(e.jobgroupId,0) AS jobgroupId, ISNULL(e.jobgroup, '') AS jobgroup, ISNULL(e.jobcodeId,0) AS jobcodeId, ISNULL(e.jobcode,'') AS jobcode, CONVERT(DATE, e.lastpromodate ,101) AS lastpromodate, CONVERT(DATE, e.lastincdate,101) AS lastincdate,
			   ISNULL(e.raceorethanicityid, 0) AS raceorethanicityId, '' AS raceorethanicity, e.currentsalary,  ISNULL(e.countryId, 0) AS countryId, '' AS country, 
			   e.emplocation, ISNULL(e.regionid, 0) AS regionid ,'' AS region, ISNULL(e.empimgpath,'') AS empimgpath, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, ISNULL(e.isMailSent, 0) AS isMailSent
	FROM employeedetails e 
	LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
	WHERE e.companyid = @companyid AND e.bactive = 1 
	AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
) v

SELECT * INTO #tempEmployeeResultMC
	FROM(
		SELECT e.empid, e.employeeId, e.companyid, '' AS companyName, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate, managerFName, managerLName,
			   e.workcity, e.workstate, e.workzipcode,ISNULL(e.suffix, '') AS suffix,  e.departmentid, d.department,  ISNULL(e.managerid,0) AS managerid , e.empstatus, e.roleid,
			   '' AS role, CONVERT(DATE, e.dateofbirth,101) AS dateofbirth, e.gender, ISNULL(e.jobcategoryid,0) AS jobcategoryid, ISNULL(e.jobcategory,'') AS jobcategory, 
			   ISNULL(e.jobgroupId,0) AS jobgroupId, ISNULL(e.jobgroup, '') AS jobgroup, ISNULL(e.jobcodeId,0) AS jobcodeId, ISNULL(e.jobcode,'') AS jobcode, CONVERT(DATE, e.lastpromodate ,101) AS lastpromodate, CONVERT(DATE, e.lastincdate,101) AS lastincdate,
			   ISNULL(e.raceorethanicityid, 0) AS raceorethanicityId, '' AS raceorethanicity, e.currentsalary,  ISNULL(e.countryId, 0) AS countryId, '' AS country, 
			   e.emplocation, ISNULL(e.regionid, 0) AS regionid ,'' AS region, ISNULL(e.empimgpath,'') AS empimgpath,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessmentDate FROM employee_assessment WHERE empid = e.empid), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
			   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpAssessmentScore WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = DATEPART(yyyy, GETDATE())) AS VARCHAR),'') AS RatingCompleted,
			   ISNULL(e.IsMailSent,0) AS IsMailSent
		FROM #tempEmployeedetailsMC e 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid
		)mc
		
		SELECT @TotalRowCount = COUNT(empid) FROM  #tempEmployeeResultMC
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount,empid, employeeId, companyid, companyName, firstname, middlename, lastname, email, jobtitle, joiningdate,
			workcity, workstate, workzipcode, suffix, departmentid, department, managerid , managerFName, managerLName, empstatus, roleid,
			role,dateofbirth, gender, jobcategoryid,jobcategory, 
			jobgroupId, jobgroup, jobcodeId, jobcode,lastpromodate,lastincdate,
			raceorethanicityId, raceorethanicity, currentsalary, countryId, country,
			emplocation, regionid, region, empimgpath, lastAssessedDate, AvgScoreRank, AvgScore,
			lastScoreRank, RatingCompleted, IsMailSent
		FROM  #tempEmployeeResultMC 
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY firstname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY
END

----------

CREATE  OR ALTER  PROCEDURE [dbo].[usp_GetCSVDataWithCount_New] 
(
	@companyid INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
SELECT * INTO #tempEmp
FROM
(
	SELECT * FROM tmpemployeedetails WHERE companyID = @companyid
)t

SELECT * INTO #tempEmployeedet 
FROM
(
	SELECT * FROM employeedetails WHERE companyID = @companyid
)v

SELECT * INTO #temprolemaster
FROM
(
	--SELECT * FROM rolemaster WHERE bactive = 1
	SELECT dimensionvalueid AS roleid,dimensionvalues AS role,bactive FROM DimensionElements WHERE Dimensionid=1 AND bactive=1
)r

SELECT * INTO #tempraceorethnicity
FROM
(
	SELECT * FROM raceorethnicity WHERE bactive = 1
)rc

SELECT * INTO #tempcountry
FROM
(
	SELECT * FROM country WHERE bactive = 1
)ct

SELECT * INTO #tempregions
FROM
(
	SELECT * FROM regions WHERE bactive = 1
)ct

SELECT SUM(NewlyInserted) AS NewlyInserted, SUM(MismatchRecord) AS MismatchRecord, '' AS [Source], 0 AS empid, '' AS employeeid, '' AS companyId,
	'' AS firstname, '' AS middleName, '' AS lastName, '' AS suffix, '' AS jobTitle, '' AS joiningdate, '' AS email,  '' AS phonenumber,
	'' AS workCity, '' AS workState, '' AS WorkZipcode, 0 AS departmentId, '' AS department, '' AS managerId, '' AS managerName, '' AS managerLName, 
	0 AS empStatus, 0 AS roleId, '' AS dateOfBirth, 0 AS raceorethanicityId, '' AS gender, '' AS jobCategory, '' AS jobCode, 
	'' AS jobGroup, '' AS lastPromodate, 0 AS currentSalary, '' AS lastIncDate, '' AS empLocation, 0 AS countryId, 0 AS regionId, '' AS empImgPath,
	'' AS CSVManagerId, 0 As ord
FROM
(
	SELECT 0 AS NewlyInserted, count(*) AS MismatchRecord FROM 
	(
		SELECT c.empid, c.employeeid, c.companyId, c.firstname, c.middleName, c.lastName,
			c.suffix, c.jobTitle, c.joiningdate, CASE WHEN c.email = '' THEN c.employeeId ELSE c.email END AS email, ISNULL(c.phonenumber, '') AS phonenumber, c.workCity, c.workState, c.WorkZipcode, c.departmentId, c.managerId, c.managerName, c.managerLName, 
			CAST(c.empStatus AS BIT) AS empStatus, c.roleId, c.dateOfBirth, ISNULL(c.raceorethanicityId,0) AS raceorethanicityId, c.gender, ISNULL(c.jobCategory,'') AS jobCategory, ISNULL(c.jobcode, '' ) AS jobcode, 
			c.jobGroup, c.lastPromodate, c.currentSalary, c.lastIncDate, c.empLocation, ISNULL(c.countryId,0) AS countryId, ISNULL(c.regionId,0) AS regionId, c.empImgPath
		FROM #tempEmp c 
		FULL OUTER JOIN #tempEmployeedet e ON (c.email=e.email OR c.employeeId = e.EmployeeId) AND (e.email <> '' OR c.email <> '')
		WHERE c.empid IS NOT NULL AND e.empid IS NOT NULL  
		AND c.companyId = e.companyId
		UNION
		SELECT c.empid, c.employeeid, c.companyId, c.firstname, c.middleName, c.lastName,
			c.suffix, c.jobTitle, c.joiningdate, CASE WHEN c.email = '' THEN c.employeeId ELSE c.email END AS email, ISNULL(c.phonenumber, '') AS phonenumber, c.workCity, c.workState, c.WorkZipcode, c.departmentId, c.managerId, c.managerName, c.managerLName, 
			CAST(c.empStatus AS BIT) AS empStatus, c.roleId, c.dateOfBirth, ISNULL(c.raceorethanicityId,0) AS raceorethanicityId, c.gender, ISNULL(c.jobCategory,'') AS jobCategory, ISNULL(c.jobcode, '' ) AS jobcode, 
			c.jobGroup, c.lastPromodate, c.currentSalary, c.lastIncDate, c.empLocation, ISNULL(c.countryId,0) AS countryId, ISNULL(c.regionId,0) AS regionId, c.empImgPath
		FROM #tempEmp c 
		FULL OUTER JOIN #tempEmployeedet e ON c.employeeId = e.EmployeeId AND (e.email = '' OR c.email = '')
		WHERE c.empid IS NOT NULL AND e.empid IS NOT NULL  
		AND c.companyId = e.companyId
		--WHERE CASE WHEN c.email = '' THEN c.employeeId ELSE c.email END IN (SELECT CASE WHEN t.email = '' THEN t.employeeId ELSE t.email END AS employeeId FROM #tempEmployeedet t WHERE t.companyID = @companyid)
		
	) v
	UNION
	SELECT count(t.empid) AS NewlyInserted, 0 as mismatch FROM #tempEmp t
	WHERE (t.email = '' OR t.email not in (SELECT email FROM #tempEmployeedet WHERE t.companyId = companyId))
	 AND (t.employeeId = '' OR t.employeeId NOT IN (SELECT employeeId FROM #tempEmployeedet WHERE t.companyId = companyId))
) k
UNION
SELECT 0 AS NewlyInserted, 0 AS MismatchRecord,'' AS [Source], c.empid, c.employeeid, CAST(c.companyid AS VARCHAR) AS companyId, c.firstname, c.middleName, c.lastName,
    c.suffix, c.jobTitle, c.joiningdate, c.email, ISNULL(c.phonenumber, '') AS phonenumber,
	c.workCity, c.workState, c.WorkZipcode, d.departmentId, d.department, ISNULL(CAST(e.empid AS VARCHAR),'0') AS managerId, c.managerName, c.managerLName, 
	CAST(c.empStatus AS BIT) AS empStatus, r.roleId, c.dateOfBirth, ISNULL(rc.id,0) AS raceorethanicityId, c.gender, ISNULL(c.jobCategory,'') AS jobCategory, ISNULL(c.jobcode, '' ) AS jobcode, 
	c.jobGroup, c.lastPromodate, c.currentSalary, c.lastIncDate, c.empLocation, ISNULL(ct.id,0) AS countryId, ISNULL(rg.regionId,0) AS regionId, c.empImgPath, c.CSVManagerId, 0 AS ord
FROM #tempEmp c 
LEFT JOIN department d ON d.department = c.departmentId
LEFT JOIN #tempEmployeedet e ON e.bactive = 1 AND substring(c.managerId, 0, charindex(':',c.managerId)) = e.EmployeeId AND e.companyId = @companyid
LEFT JOIN #temprolemaster r ON r.bactive = 1 AND r.role  = c.roleId
LEFT JOIN #tempraceorethnicity rc ON rc.bactive = 1 AND rc.raceorethnicity = c.raceorethanicityId 
LEFT JOIN #tempcountry ct ON ct.bactive = 1 AND ct.country  = c.countryId
LEFT JOIN #tempregions rg ON rg.bactive = 1 AND rg.region = c.regionId AND ct.id = rg.countryid 
WHERE (c.email = '' OR  c.email NOT IN (SELECT email FROM #tempEmployeedet WHERE companyId=@companyid))
AND (c.employeeId = '' OR c.employeeId NOT IN (SELECT employeeId FROM #tempEmployeedet  WHERE companyId = @companyid))
UNION
SELECT 0 AS NewlyInserted, 0 AS MismatchRecord, [Source], empid,  employeeid,  companyId,  firstname,  middleName,  lastName,  suffix,  jobTitle,  joiningdate,  email, phonenumber,
	 workCity,  workState,  WorkZipcode,  departmentId, department,  managerId,  managerName,  managerLName, 
	 empStatus,  roleId,  dateOfBirth,  raceorethanicityId,  gender,  jobCategory,  jobCode, 
	 jobGroup,  lastPromodate,  currentSalary,  lastIncDate,  empLocation,  countryId,  regionId,  empImgPath,CSVManagerId, ord
FROM
( 

	SELECT 'CSV' AS [Source], e.empid, CASE WHEN ISNULL(t.employeeid,'') ='' THEN CAST(e.empid AS VARCHAR(15)) ELSE ISNULL(t.employeeid,'') END AS employeeid, t.companyId AS companyId, --CAST(cd.compid AS VARCHAR) AS companyId, 
		t.firstname, t.middleName, t.lastName, t.suffix, t.jobTitle, t.joiningdate, t.email, ISNULL(t.phonenumber, '') AS phonenumber,
		t.workCity, t.workState, t.WorkZipcode, d.departmentId, d.department, ISNULL(CAST(e1.empid AS VARCHAR),'0') managerId,  t.managerName, t.managerLName, 
		CAST(t.empStatus AS BIT) AS empstatus, r.roleId, t.dateOfBirth, rc.id AS raceorethanicityId, t.gender, ISNULL(t.jobCategory,'') AS jobCategory, ISNULL(t.jobCode, '') AS jobcode, 
		t.jobGroup, t.lastPromodate, t.currentSalary, t.lastIncDate, t.empLocation, ct.id AS countryId, rg.regionId, t.empImgPath,t.CSVManagerId, 1 AS ord
	FROM #tempEmp t 
	FULL OUTER JOIN #tempEmployeedet e ON (t.email=e.email OR t.employeeId = e.EmployeeId) AND (e.email <> '' OR t.email <> '')
	LEFT JOIN department d ON d.department = t.departmentId 
	LEFT JOIN #tempEmployeedet e1 ON e.bactive = 1 AND SUBSTRING(t.managerId, 0, CHARINDEX(':',t.managerId)) = e1.EmployeeId AND e1.companyID = @companyid
	LEFT JOIN #temprolemaster r ON r.bactive = 1 AND r.role  = t.roleId
	LEFT JOIN #tempraceorethnicity rc ON rc.bactive = 1 AND rc.raceorethnicity = t.raceorethanicityId 
	LEFT JOIN #tempcountry ct ON ct.bactive = 1 AND ct.country  = t.countryId
	LEFT JOIN #tempregions rg ON rg.bactive = 1 AND rg.region = t.regionId AND ct.id = rg.countryid 
	where t.empid IS NOT NULL AND e.empid IS NOT NULL  
	AND t.companyId = e.companyId
	UNION
	SELECT 'CSV' AS [Source], e.empid, CASE WHEN ISNULL(t.employeeid,'') ='' THEN CAST(e.empid AS VARCHAR(15)) ELSE ISNULL(t.employeeid,'') END AS employeeid, t.companyId AS companyId, --CAST(cd.compid AS VARCHAR) AS companyId, 
		t.firstname, t.middleName, t.lastName, t.suffix, t.jobTitle, t.joiningdate, t.email, ISNULL(t.phonenumber, '') AS phonenumber,
		t.workCity, t.workState, t.WorkZipcode, d.departmentId, d.department, ISNULL(CAST(e1.empid AS VARCHAR),'0') managerId,  t.managerName, t.managerLName, 
		CAST(t.empStatus AS BIT) AS empstatus, r.roleId, t.dateOfBirth, rc.id AS raceorethanicityId, t.gender, ISNULL(t.jobCategory,'') AS jobCategory, ISNULL(t.jobCode, '') AS jobcode, 
		t.jobGroup, t.lastPromodate, t.currentSalary, t.lastIncDate, t.empLocation, ct.id AS countryId, rg.regionId, t.empImgPath,t.CSVManagerId, 1 AS ord
	FROM #tempEmp t 
	FULL OUTER JOIN #tempEmployeedet e ON t.employeeId = e.EmployeeId AND (e.email = '' OR t.email = '')
	LEFT JOIN department d ON d.department = t.departmentId 
	LEFT JOIN #tempEmployeedet e1 ON e.bactive = 1 AND SUBSTRING(t.managerId, 0, CHARINDEX(':',t.managerId)) = e1.EmployeeId AND e1.companyID = @companyid
	LEFT JOIN #temprolemaster r ON r.bactive = 1 AND r.role  = t.roleId
	LEFT JOIN #tempraceorethnicity rc ON rc.bactive = 1 AND rc.raceorethnicity = t.raceorethanicityId 
	LEFT JOIN #tempcountry ct ON ct.bactive = 1 AND ct.country  = t.countryId
	LEFT JOIN #tempregions rg ON rg.bactive = 1 AND rg.region = t.regionId AND ct.id = rg.countryid 
	where t.empid IS NOT NULL AND e.empid IS NOT NULL  
	AND t.companyId = e.companyId
UNION
SELECT 'DB' AS [Source], t.empid, t.employeeid, CAST(t.companyId AS VARCHAR) AS companyId, t.firstname, t.middleName, t.lastName, t.suffix, t.jobTitle, FORMAT(t.joiningdate, 'MM-dd-yyyy') AS joiningdate, t.email, ISNULL(t.phonenumber, '') AS phonenumber,
	t.workCity, t.workState, t.WorkZipcode, t.departmentId, d.department, ISNULL(CAST(t.managerid AS VARCHAR),'0') AS managerId, t.managerName, t.managerLName, 
	CAST(t.empStatus AS BIT) AS empstatus, t.roleId, FORMAT(t.dateOfBirth, 'MM-dd-yyyy') AS dateOfBirth, t.raceorethanicityId, t.gender, t.jobCategory, t.jobCode, 
	t.jobGroup, FORMAT(t.lastPromodate, 'MM-dd-yyyy') AS lastPromodate, t.currentSalary, FORMAT(t.lastIncDate, 'MM-dd-yyyy') AS lastIncDate, 
	t.empLocation, t.countryId, t.regionId, t.empImgPath,'' AS CSVManagerId, 2 As ord
	FROM #tempEmployeedet t 
	FULL OUTER JOIN #tempEmp e ON (t.email=e.email OR t.employeeId = e.EmployeeId)  AND (e.email <> '' OR t.email <> '')
	LEFT JOIN department d ON d.departmentid = t.departmentId  
	where t.empid IS NOT NULL AND e.empid IS NOT NULL 
	 AND t.companyId = e.companyId
UNION                                                                                                                                                                                                                                                                                                                                                                                                             
SELECT 'DB' AS [Source], t.empid, t.employeeid, CAST(t.companyId AS VARCHAR) AS companyId, t.firstname, t.middleName, t.lastName, t.suffix, t.jobTitle,FORMAT(t.joiningdate, 'MM-dd-yyyy') AS joiningdate, t.email, ISNULL(t.phonenumber, '') AS phonenumber,
	t.workCity, t.workState, t.WorkZipcode, t.departmentId, d.department, ISNULL(CAST(t.managerid AS VARCHAR),'0') AS managerId, t.managerName, t.managerLName, 
	CAST(t.empStatus AS BIT) AS empstatus, t.roleId, FORMAT(t.dateOfBirth, 'MM-dd-yyyy') AS dateOfBirth, t.raceorethanicityId, t.gender, t.jobCategory, t.jobCode, 
	t.jobGroup, FORMAT(t.lastPromodate, 'MM-dd-yyyy') AS lastPromodate, t.currentSalary, FORMAT(t.lastIncDate, 'MM-dd-yyyy') AS lastIncDate, 
	t.empLocation, t.countryId, t.regionId, t.empImgPath,'' AS CSVManagerId, 2 As ord
	FROM #tempEmployeedet t 
	FULL OUTER JOIN #tempEmp e ON (t.employeeId = e.EmployeeId)  AND (e.email = '' OR t.email = '')
	LEFT JOIN department d ON d.departmentid = t.departmentId  
	where t.empid IS NOT NULL AND e.empid IS NOT NULL 
	 AND t.companyId = e.companyId
) v
ORDER BY empid,ord
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetAssessmentYear]
(
	@CompanyId INT,
	@ManagerId INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

DECLARE @UserRole INT;
SELECT  @UserRole = dbo.GetRoleByEmpid(@ManagerID)

IF(@UserRole IN(1,2,4))
BEGIN
    -- Insert statements for procedure here
  SELECT assessedYear 
  FROM
  (
	   SELECT DISTINCT FORMAT(assessmentdate, 'yyyy') AS assessedYear,FORMAT(assessmentdate, 'yyyy') AS ord
	   FROM employee_assessment
	   WHERE bactive = 1
	   UNION
	   SELECT FORMAT(GETDATE(), 'yyyy') AS assessedYear, FORMAT(GETDATE(), 'yyyy') AS ord
	   UNION
	   SELECT 'Last 12 Months' AS assessedYear, 0 AS ord
) v 
ORDER BY ord DESC
END
ELSE
BEGIN
WITH tempEmployee(empId, companyid, managerid, roleId)
AS
(
	SELECT empId, companyId, managerId, roleId
	FROM employeedetails AS FirstLevel
	WHERE managerId = @ManagerId AND companyid = @CompanyId 
	AND bactive = 1 AND empStatus = 1
	UNION ALL
	SELECT NextLevel.empId, NextLevel.companyId,
	NextLevel.managerid, NextLevel.roleId
	FROM employeedetails AS NextLevel
	INNER JOIN tempEmployee AS t ON NextLevel.managerId = t.empId
	WHERE NextLevel.companyId = @CompanyId AND bactive = 1  AND empStatus = 1
)

  SELECT assessedYear 
  FROM
  (
	  SELECT DISTINCT FORMAT(a.assessmentdate, 'yyyy') AS assessedYear, FORMAT(assessmentdate, 'yyyy') AS ord
	  FROM employee_assessment a
	  INNER JOIN userlogin u ON u.bActive = 1 AND u.userID = a.assessmentBy
	  INNER JOIN tempEmployee t ON t.managerId = u.empId
	  WHERE a.bactive = 1
	  UNION
	  SELECT FORMAT(GETDATE(), 'yyyy') AS assessedYear, FORMAT(GETDATE(), 'yyyy') AS ord
	  UNION
	  SELECT 'Last 12 Months' AS assessedYear, 0 AS ord
  ) v ORDER BY  ord DESC
END
END
----------

CREATE OR ALTER  PROCEDURE [dbo].[usp_GetYearlyDepartmentWiseManagerDashboard]
(
	@YearId INT,
	@managerId INT, 
	@companyId INT,
	@departmentlist VARCHAR(max)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets FROM
	-- interfering with SELECT  statements.
	SET NOCOUNT ON;
SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d;

DECLARE @UserRole INT;
SELECT  @UserRole = dbo.GetRoleByEmpid(@managerId)


SELECT * INTO #tempEmployeedetails FROM 
(
		SELECT empid, managerid, companyid, roleid, departmentid, Format(joiningDate, 'yyyy') AS joiningYear 
		FROM employeedetails 
		WHERE bactive = 1 AND empstatus = 1 AND companyid = @companyId
			  AND Format(joiningDate, 'yyyy') <= CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END
			  AND departmentId IN (SELECT department FROM #departmentlist)
)v

DECLARE @CurrYear INT = CAST(FORMAT(GETDATE(), 'yyyy') AS VARCHAR)
DECLARE @LastYear INT = FORMAT(GETDATE(), 'yyyy') - 1
DECLARE @LastDate DATE = CASE WHEN @YearID <> 0  AND @CurrYear <> @YearId THEN DATEFROMPARTS(@yearID,12,31) ELSE CONVERT(DATE, GETDATE(), 101) END
DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

CREATE TABLE #monthlist (yearmonth VARCHAR(20), monthname VARCHAR(20));
	
	
			IF(@YearId = 0)
			BEGIN
			INSERT INTO #monthlist
			SELECT yearmonth, monthname FROM
			(
				SELECT Format(getdate(),'yyyy')+FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MM')yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number between 1 and MONTH(GETDATE())
				UNION		
				SELECT  CAST(@LastYear AS VARCHAR) + FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@LastYear AS VARCHAR) AS DATE), 'MM')yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number > MOnth(DATEADD(month,-12,GETDATE()))
			) v
			END
			ELSE
			BEGIN
			INSERT INTO #monthlist
			SELECT  yearmonth, monthname  
			FROM
			(
				SELECT CAST(@yearID AS VARCHAR) + FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@yearID AS VARCHAR) AS DATE), 'MM')AS yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@yearID AS VARCHAR) AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number between 1 and CASE WHEN @CurrYear <> @YearId THEN MONTH(DATEFROMPARTS(@yearID,12,31)) ELSE MONTH(GETDATE()) END
			)v
			END

	CREATE TABLE #tmpViewEmpScore (
	companyid INT, empid INT ,empName VARCHAR(1000),Score DECIMAL(2,0),ScoreRank VARCHAR(50),
	RatingDate DATETIME, assessmentbyId INT, assessmentBy VARCHAR(1000), 
	assessmentId INT, managerId INT, departmentid INT, roleId int, empStatus BIT,inDirectManager VARCHAR(100),  yearId INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tmpViewEmpScore
	SELECT * FROM
	(
		SELECT companyid, empid, empName, Score, ScoreRank, RatingDate, assessmentbyId, assessmentBy, assessmentId, managerId,
		departmentid, roleId, empStatus, inDirectManager, yearId FROM vw_EmpAssessmentScore 
		WHERE  empStatus = 1 AND empid <> @ManagerId AND departmentId IN (SELECT department FROM #departmentlist) 
		AND FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear  AND companyid = @companyId
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tmpViewEmpScore
	SELECT * FROM
	(
		SELECT companyid, empid, empName, Score, ScoreRank, RatingDate, assessmentbyId, assessmentBy, assessmentId, managerId,
		departmentid, roleId, empStatus, inDirectManager, yearId FROM vw_EmpAssessmentScore 
		WHERE  empStatus = 1 AND empid <> @ManagerId AND departmentId IN (SELECT department FROM #departmentlist) 
		AND yearid = @YearId  AND companyid = @companyId
	)k
	END

SELECT  * INTO #tmpEmpScore
FROM
(
		SELECT avgscore,empid,managerId,monyrid,monyr,inDirectManager
		FROM
		(
			SELECT row_number() OVER (Partition BY empid, FORMAT(RatingDate,'yyyyMM') order by empid, FORMAT(RatingDate,'yyyyMM')) AS rownumber,
				  CAST(ROUND(AVG(score) OVER (Partition BY empid, FORMAT(RatingDate,'yyyyMM')),0) AS decimal(10,0)) as avgscore,
				  empid, managerId, FORMAT(RatingDate,'yyyyMM') as monyrid, FORMAT(RatingDate,'MMM') as monyr,inDirectManager
			FROM #tmpViewEmpScore
		) a WHERE rownumber=1
) p

SELECT  * INTO #tmpAvgEmpScore
FROM
(
	  SELECT avgscore,monyrid,monyr,managerId,inDirectManager
	  FROM
	  (
		  SELECT  row_number() OVER (Partition BY FORMAT(RatingDate,'yyyyMM'), managerid ORDER BY FORMAT(RatingDate,'yyyyMM')) As rownumber, 
				  CAST(ROUND(AVG(score) OVER (PARTITION BY FORMAT(RatingDate,'yyyyMM'), managerid),0) AS DECIMAL(10,0)) as avgscore,    
				  FORMAT(RatingDate,'yyyyMM') AS monyrid, FORMAT(RatingDate,'MMM') AS monyr, managerId,inDirectManager
		 FROM #tmpViewEmpScore 
														
	  ) a WHERE rownumber=1
) p



IF(@managerId > 0 AND @companyId > 0 AND @UserRole IN(2,4))
BEGIN
SELECT  CntDirectEmps, DirectRptAvgScore,DirectRptAvgScoreRank, CntOrgEmps,OrgRptAvgScore,
		OrgRptAvgScoreRank, grphDrctRpt,   DrctScoreRank, DrctmonYrId, DrctmonYr,
		TotDirctEmps,DrctPct,grphOrgRpt,OrgRank,OrgMonYrId,OrgMonYr,TotOrgEmps,OrgPct,DrctAvgScore,AvgDrctRank,
		DrctAvgMonYrId,DrctAvgMonYr,OrgAvgScore,AvgOrgRank,OrgAvgMonYrId,OrgAvgMonYr,TodayRptEmpCnt,TodayRptEmpRank,
		TodayRptEmpList,TodayOrgEmpCnt,TodayOrgEmpRank,TodayOrgEmpList
INTO #tempAdminExec
FROM
(
		SELECT  0 AS CntDirectEmps, 0 AS DirectRptAvgScore,'' AS DirectRptAvgScoreRank,  0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, grphDrctRpt,
						  DrctScoreRank, monyrid AS DrctmonYrId, monyr AS DrctmonYr,
						  TotDirctEmps,
						  (ROUND(CAST((100 / CAST(TotDirctEmps AS decimal)) AS DECIMAL(10,0)),0) * grphDrctRpt) AS DrctPct, 0 AS grphOrgRpt,
						  '' AS OrgRank, '' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
						  0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						  0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						  0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
		FROM
		(
					SELECT  DrctScoreRank, monyrid, monyr, SUM(grphDrctRpt) AS grphDrctRpt, TotDirctEmps
					FROM
					(
							SELECT dbo.Get_grade_value(dbo.GETSCoreRank(avgscore)) AS DrctScoreRank,
					    			monyrid, monyr,  COUNT(empid) AS grphDrctRpt, COUNT(empid) OVER (Partition BY monyrid) AS TotDirctEmps
							FROM (
									SELECT  empid,avgscore as avgscore,m.yearmonth as monyrid,m.monthname as monyr FROM #monthlist m 
									LEFT OUTER JOIN #tmpEmpScore s on s.monyrid =  (SELECT  MAX(monyrid) FROM #tmpEmpScore WHERE monyrid < yearmonth and empid = s.empid)
									WHERE  empid = s.empid and managerid = @managerId AND 
									yearmonth NOT IN(SELECT  monyrid FROM #tmpEmpScore WHERE empid = s.empid)
									UNION ALL
									SELECT  empid,avgscore,monyrid, monyr FROM  #tmpEmpScore where managerid = @managerId
		 						) v
							GROUP BY avgscore,monyrid, monyr, empid
						) v1 GROUP BY DrctScoreRank, monyrid, monyr,TotDirctEmps
		) k
	  UNION
	  SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore,'' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, 0 AS grphDrctRpt,
			   '' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
			   grphOrgRpt, OrgRank, monyrid AS  OrgMonYrId, monyr AS OrgMonYr, TotOrgEmps, 
			   (ROUND(CAST((100 / CAST(TotOrgEmps AS decimal)) AS DECIMAL(10,0)),0) * grphOrgRpt) AS OrgPct,
			   0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
			   0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
			   0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
	  FROM
	  (
		  SELECT  OrgRank, monyrid, monyr, SUM(grphOrgRpt) AS grphOrgRpt, TotOrgEmps
		  FROM (
				SELECT  dbo.Get_grade_value(dbo.GETSCoreRank(avgscore)) AS OrgRank,
						empid,monyrid, monyr,COUNT(empid) AS grphOrgRpt, COUNT(empid) OVER (Partition BY monyrid) AS TotOrgEmps FROM
				(
					 SELECT  empid,avgscore as avgscore,m.yearmonth as monyrid,m.monthname as monyr FROM #monthlist m 
					 LEFT OUTER JOIN #tmpEmpScore s on s.monyrid =  (SELECT  MAX(monyrid) FROM #tmpEmpScore WHERE monyrid < yearmonth and empid = s.empid)
					 WHERE yearmonth NOT IN(SELECT  monyrid FROM #tmpEmpScore r  WHERE r.empid = s.empid) and  empid = s.empid 
					 UNION ALL
					 SELECT  empid,avgscore,monyrid, monyr FROM  #tmpEmpScore
					)a 
					GROUP BY avgscore,monyrid, monyr, empid
				) B GROUP BY OrgRank, monyrid, monyr,TotOrgEmps) k
				UNION
					 SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore,'' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore,'' AS OrgRptAvgScoreRank, 0 AS grphDrctRpt,
								  '' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
								  0 AS grphOrgRpt, '' AS OrgRank, '' AS OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct, 
								  avgscore AS DrctAvgScore, AvgDrctRank, monyrid AS DrctAvgMonYrId, monyr AS DrctAvgMonYr, 
								  0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
								  0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
							FROM
							(
									SELECT  dbo.GETSCoreRank(avgscore) AS AvgDrctRank, 
											monyrid, monyr,avgscore
						FROM (
							 SELECT DISTINCT CAST(ROUND(AVG(score) OVER (Partition BY FORMAT(RatingDate,'yyyyMM')),0) AS decimal(10,0)) as avgscore,    
							 FORMAT(RatingDate,'yyyyMM') as monyrid, FORMAT(RatingDate,'MMM') as monyr FROM #tmpViewEmpScore 
							 WHERE managerId = @managerId
						) v
				) k
			UNION
			--GET SCORRANK WISE DATA OF ALL EMPLOYEES OF ORGANIZATION
			 SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore,'' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore,'' AS OrgRptAvgScoreRank, 0 AS grphDrctRpt,
					  '' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
					  0 AS grphOrgRpt, '' AS OrgRank, '' AS OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct, 
					  0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
					  avgscore AS OrgAvgScore, AvgOrgRank, monyrid AS OrgAvgMonYrId, monyr AS OrgAvgMonYr, 
					  0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank, '' AS TodayRptEmpList,0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
				FROM
				(
						SELECT  dbo.GETSCoreRank(avgscore) AS AvgOrgRank, 
											monyrid, monyr,avgscore
						FROM (
							 SELECT  DISTINCT CAST(ROUND(AVG(score) OVER (Partition BY FORMAT(RatingDate,'yyyyMM')),0) AS decimal(10,0)) as avgscore,    
								FORMAT(RatingDate,'yyyyMM') as monyrid, FORMAT(RatingDate,'MMM') as monyr 
							 FROM #tmpViewEmpScore 
						) v
				) k

			)l


			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore,'' AS OrgRptAvgScoreRank,
			ISNULL(grphDrctRpt,0) AS grphDrctRpt, ISNULL(DrctScoreRank,'') AS DrctScoreRank, yearmonth AS DrctmonYrId, monthname  AS DrctmonYr, 
			ISNULL(TotDirctEmps,0) AS TotDirctEmps, ISNULL(DrctPct,0) AS DrctPct, 0 AS grphOrgRpt,
							  '' AS OrgRank, '' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
							  0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							  0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
							  0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
			FROM #monthlist m LEFT OUTER JOIN #tempAdminExec s on s.DrctmonYrId = (SELECT  MAX(drctmonyrid) FROM #tempAdminExec WHERE drctmonyrid < yearmonth)
			WHERE yearmonth NOT IN(SELECT  DrctmonYrId FROM #tempAdminExec WHERE DrctmonYrId <> '')
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank,
				ISNULL(grphDrctRpt,0) AS grphDrctRpt, ISNULL(DrctScoreRank,'') AS DrctScoreRank, DrctmonYrId, DrctmonYr, ISNULL(TotDirctEmps,0) AS TotDirctEmps, ISNULL(DrctPct,0) AS DrctPct,
				ISNULL(grphOrgRpt,0) AS grphOrgRpt, OrgRank,OrgMonYrId,OrgMonYr, TotOrgEmps, OrgPct,
				ISNULL(DrctAvgScore,0) AS DrctAvgScore, ISNULL(AvgDrctRank,'') AS AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, 
				ISNULL(OrgAvgScore,0) AS OrgAvgScore, ISNULL(AvgOrgRank,'') AS AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
				TodayRptEmpCnt, TodayRptEmpRank,'' AS TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank,'' AS TodayOrgEmpList
			FROM #tempAdminExec 
			WHERE DrctmonYrId <> ''
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, 0 AS grphdrctrpt,'' AS drctscorerank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
						ISNULL(grphOrgRpt,0) AS grphOrgRpt, ISNULL(OrgRank,'') AS OrgRank, yearmonth AS OrgMonYrId, monthname  AS OrgMonYr, ISNULL(TotOrgEmps,0) AS TotOrgEmps, ISNULL(OrgPct,0) AS OrgPct,
						0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
						FROM #monthlist m LEFT OUTER JOIN #tempAdminExec s on s.OrgMonYrId = (SELECT  MAX(OrgMonYrId)  FROM #tempAdminExec WHERE OrgMonYrId < yearmonth)
						WHERE yearmonth NOT IN(SELECT  OrgMonYrId  FROM #tempAdminExec WHERE OrgMonYrId <> '') 
			UNION
			SELECT   0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank,
				grphDrctRpt, DrctScoreRank, DrctmonYrId, DrctmonYr, TotDirctEmps, DrctPct,
				ISNULL(grphOrgRpt,0) AS grphOrgRpt, ISNULL(OrgRank,'') AS OrgRank, OrgMonYrId,OrgMonYr, ISNULL(TotOrgEmps,0) AS TotOrgEmps,  ISNULL(OrgPct,0) AS OrgPct,
				ISNULL(DrctAvgScore,0) AS DrctAvgScore, ISNULL(AvgDrctRank,'') AS AvgDrctRank,  DrctAvgMonYrId, DrctAvgMonYr, 
				ISNULL(OrgAvgScore,0) AS OrgAvgScore, ISNULL(AvgOrgRank,'') AS AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
				TodayRptEmpCnt, TodayRptEmpRank,'' AS TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank,'' AS TodayOrgEmpList 
			FROM #tempAdminExec
			WHERE OrgMonYrId <> ''
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, 0 AS grphdrctrpt,'' AS drctscorerank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
						0 AS grphOrgRpt, '' AS OrgRank, '' AS OrgMonYrId, ''  AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
						ISNULL(DrctAvgScore,0) AS DrctAvgScore, ISNULL(AvgDrctRank,'') AS AvgDrctRank, yearmonth AS DrctAvgMonYrId, monthname AS DrctAvgMonYr, 
						0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
						FROM #monthlist m LEFT OUTER JOIN #tempAdminExec s on s.DrctAvgMonYrId = (SELECT  MAX(DrctAvgMonYrId)  FROM #tempAdminExec WHERE DrctAvgMonYrId < yearmonth)
						WHERE yearmonth NOT IN(SELECT  DrctAvgMonYrId  FROM #tempAdminExec  WHERE DrctAvgMonYrId <> '')
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank,
						 0 AS grphdrctrpt,'' AS drctscorerank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
							0 AS grphOrgRpt, '' AS OrgRank, '' AS OrgMonYrId, ''  AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
							ISNULL(DrctAvgScore,0) AS DrctAvgScore, ISNULL(AvgDrctRank,'') AS AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, 
							0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
							0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
			FROM #tempAdminExec WHERE DrctAvgMonYrId <> ''
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, 0 AS grphdrctrpt,'' AS drctscorerank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
						0 AS grphOrgRpt, '' AS OrgRank,  '' AS OrgMonYrId, ''  AS OrgMonYr, 0 as TotOrgEmps, 0 as OrgPct,
						0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						ISNULL(OrgAvgScore,0) AS OrgAvgScore, ISNULL(AvgOrgRank,'') AS AvgOrgRank, yearmonth AS OrgAvgMonYrId, monthname AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
						FROM #monthlist m LEFT OUTER JOIN #tempAdminExec s on s.OrgAvgMonYrId = (SELECT  MAX(OrgAvgMonYrId)  FROM #tempAdminExec WHERE OrgAvgMonYrId < yearmonth)
						WHERE yearmonth NOT IN(SELECT  OrgAvgMonYrId  FROM #tempAdminExec WHERE OrgAvgMonYrId <> '')
						--WHERE EXISTS(SELECT 'Y'  FROM #temp r WHERE r.OrgAvgMonYrId <> ''  AND m.yearmonth <> r.OrgAvgMonYrId )
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, 0 AS grphdrctrpt,'' AS drctscorerank, 
							'' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
							0 AS grphOrgRpt, '' AS OrgRank,  '' AS OrgMonYrId, ''  AS OrgMonYr, 0 as TotOrgEmps, 0 as OrgPct,
							0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							 ISNULL(OrgAvgScore,0) AS OrgAvgScore,  ISNULL(AvgOrgRank,'') AS AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
							0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank, '' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
			FROM #tempAdminExec WHERE OrgAvgMonYrId <> ''
			UNION
			SELECT  CntDirectEmps, ISNULL(DirectRptAvgScore,0) AS DirectRptAvgScore, DirectRptAvgScoreRank, 
			CntOrgEmps, ISNULL(OrgRptAvgScore,0) AS OrgRptAvgScore, OrgRptAvgScoreRank,
			grphDrctRpt, DrctScoreRank, DrctmonYrId, DrctmonYr, TotDirctEmps, DrctPct,
			grphOrgRpt, OrgRank,OrgMonYrId,OrgMonYr, TotOrgEmps, OrgPct,
			DrctAvgScore, AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, 
			OrgAvgScore, AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
			TodayRptEmpCnt, TodayRptEmpRank, TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank,TodayOrgEmpList
			FROM
			(
				SELECT  CntDirectEmps, DirectRptAvgScore, CASE WHEN DirectRptAvgScore IS NULL THEN '' ELSE DBO.GETScoreRank(DirectRptAvgScore) END AS DirectRptAvgScoreRank,
				CntOrgEmps, OrgRptAvgScore, CASE WHEN OrgRptAvgScore IS NULL THEN '' ELSE DBO.GETScoreRank(OrgRptAvgScore) END AS OrgRptAvgScoreRank,
				grphDrctRpt, DrctScoreRank, DrctmonYrId, DrctmonYr, TotDirctEmps, DrctPct,
				grphOrgRpt, OrgRank,OrgMonYrId,OrgMonYr, TotOrgEmps, OrgPct,
				DrctAvgScore, AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, 
				OrgAvgScore, AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
				TodayRptEmpCnt, TodayRptEmpRank, TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank,TodayOrgEmpList
				FROM
				(
						SELECT  SUM(CntDirectEmps) AS CntDirectEmps, SUM(DirectRptAvgScore) AS DirectRptAvgScore, --DirectRptAvgScoreRank,
						SUM(CntOrgEmps) AS CntOrgEmps, SUM(OrgRptAvgScore) AS OrgRptAvgScore, --OrgRptAvgScoreRank,
						SUM(grphDrctRpt) AS grphDrctRpt, DrctScoreRank, DrctmonYrId, DrctmonYr, SUM(TotDirctEmps) AS TotDirctEmps, 
						SUM(DrctPct) AS DrctPct, SUM(grphOrgRpt) AS grphOrgRpt, OrgRank, OrgMonYrId, OrgMonYr,SUM(TotOrgEmps) AS TotOrgEmps, SUM(OrgPct) AS OrgPct,
						DrctAvgScore, AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, 
						OrgAvgScore, AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
						TodayRptEmpCnt, TodayRptEmpRank, TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank,TodayOrgEmpList
						FROM
						(
						---GET NO OF EMPLOYEES DIRECLTLY REPORT TO MANAGER
						SELECT  Count(empid) AS CntDirectEmps,NULL AS DirectRptAvgScore, 
							0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,
							0 AS grphDrctRpt,
							'' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, '' AS  OrgMonYrId, '' AS OrgMonYr, 
							0 AS TotOrgEmps, 0 AS OrgPct,
							0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
							0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank, '' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
						FROM #tempEmployeedetails e 
						WHERE e.managerid = @managerId 
						UNION
						---GET AVERAGE SCORE OF TODAY EMPLOYEES DIRECLTLY REPORT TO MANAGER
						SELECT  0 AS CntDirectEmps, CASE WHEN DirectRptAvgScore1 IS NULL THEN DirectRptAvgScore2 ELSE DirectRptAvgScore1 END AS DirectRptAvgScore, 0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
							0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
							'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
							0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
							0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
						FROM
						(
							SELECT  0 AS CntDirectEmps, SUM(DirectRptAvgScore1) AS DirectRptAvgScore1, SUM(DirectRptAvgScore2) AS DirectRptAvgScore2,  0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
							0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
							'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
							0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
							0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
							FROM
							(
								SELECT  0 AS CntDirectEmps, CAST(ROUND(AVG(Score),0) AS DECIMAL(10,0)) AS DirectRptAvgScore1, NULL AS DirectRptAvgScore2,
													0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
													0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
													'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
													0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
													0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
													0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
												FROM #tmpViewEmpScore vs
												WHERE CONVERT(DATE, vs.RatingDate, 101) = @LastDate -- CONVERT(DATE, GETDATE(), 101)
													AND vs.managerId = @managerId --AND vs.companyid = @companyId AND vs.departmentId IN (SELECT department FROM #departmentlist)
												UNION
								SELECT  0 AS CntDirectEmps, NULL AS DirectRptAvgScore1, CAST(ROUND(AVG(Score),0) AS DECIMAL(10,0)) AS DirectRptAvgScore2,
													0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
													0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
													'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
													0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
													0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
													0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
					   								FROM
													(
													SELECT  k.Score, k.empid, k.empNAme FROM #tmpViewEmpScore k
															where k.managerId = @managerId
															and assessmentid = (SELECT  MAX(assessmentId) AS Id FROM #tmpViewEmpScore s where k.empid = s.empid) 
													
													) v
							) k
						) v
					UNION
					---GET AVERAGE SCORE OF TODAY OF ALL EMPLOYEES OF ORGANIZATION
					SELECT  0 AS CntDirectEmps, NULL AS DirectRptAvgScore, 0 AS CntOrgEmps, CASE WHEN OrgRptAvgScore1 IS NULL THEN OrgRptAvgScore2 ELSE OrgRptAvgScore1 END AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
						0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
						'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
						0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
					FROM
					(	
						SELECT  0 AS CntDirectEmps, NULL AS DirectRptAvgScore, 0 AS CntOrgEmps, SUM(OrgRptAvgScore1) AS OrgRptAvgScore1, SUM(OrgRptAvgScore2) AS OrgRptAvgScore2,--'' AS OrgRptAvgScoreRank, 
							0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
							'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
							0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
							0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
						FROM
						(					
											SELECT  0 AS CntDirectEmps, NULL AS DirectRptAvgScore,-- '' AS DirectRptAvgScoreRank,
												0 AS CntOrgEmps, CAST(ROUND(AVG(Score), 0) AS DECIMAL(10,0)) AS OrgRptAvgScore1, NULL AS OrgRptAvgScore2, 0 AS grphDrctRpt,
												'' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
												'' AS  OrgMonYrId,'' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
												0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
												0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
												0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
											FROM #tmpViewEmpScore vs
											WHERE CONVERT(DATE, vs.RatingDate, 101) = @LastDate 
											UNION
											SELECT  0 AS CntDirectEmps,  NULL AS DirectRptAvgScore,
												0 AS CntOrgEmps, NULL AS OrgRptAvgScore1, CAST(ROUND(AVG(Score),0) AS DECIMAL(10,0)) AS OrgRptAvgScore2,
												0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
												'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
												0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
												0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
												0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
					   							FROM
												(
												SELECT  k.Score, k.empid, k.empNAme FROM #tmpViewEmpScore k 
												where assessmentid = (SELECT  MAX(assessmentId) AS Id FROM #tmpViewEmpScore s where k.empid = s.empid)
												) v
						) o
					)p
					UNION					
					---GET NO OF ALL EMPLOYEES OF ORGANIZATION
					SELECT  0 AS CntDirectEmps,NULL AS DirectRptAvgScore, 
							  Count(empid) AS CntOrgEmps,  NULL AS OrgRptAvgScore, 
							   0 AS grphDrctRpt,
							'' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, '' AS  OrgMonYrId,'' AS OrgMonYr,
							  0 AS TotOrgEmps, 0 AS OrgPct,
							  0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							  0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
							  0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank, '' AS TodayOrgEmpList
					FROM #tempEmployeedetails e 
					WHERE e.roleid <> 4 AND empid <> @ManagerId
					UNION
					SELECT  0 AS CntDirectEmps,NULL AS DirectRptAvgScore, 0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,
							 0 AS grphDrctRpt,
							'' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, ''  AS OrgRank,
								'' AS OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
								0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
								0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
			    				TodayRptEmpCnt, TodayRptEmpRank,TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
						FROM
						(
							SELECT  TodayRptEmpRank,  SUM(TodayRptEmpCnt) AS TodayRptEmpCnt,'' AS TodayRptEmpList
							FROM
							(
								SELECT  TodayRptEmpRank, COUNT(empid) AS TodayRptEmpCnt, '' AS TodayRptEmpList
								FROM
								(
									SELECT  dbo.get_grade_value(dbo.GETSCoreRank(v.Score)) AS TodayRptEmpRank, 
													empid
									FROM 
									(
									
										 SELECT  k.Score, k.empid FROM #tmpViewEmpScore k where k.managerid = @managerId AND k.companyid = companyId
											and assessmentid = (SELECT  MAX(assessmentId) AS Id FROM #tmpViewEmpScore s where k.empid = s.empid
											--AND DATEPART(yyyy,Ratingdate) = @YearID
											)
									) v
								) g GROUP BY TodayRptEmpRank
							) v1 GROUP BY TodayRptEmpRank
						) k
					UNION
					---Employee current Score  of whole organization
						SELECT  0 AS CntDirectEmps,NULL AS DirectRptAvgScore,
						  0 AS CntOrgEmps,  NULL AS OrgRptAvgScore, 
						  0 AS grphDrctRpt,
							'' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, ''  AS OrgRank,
								'' AS OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
								0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
								0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
			    				0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank, TodayOrgEmpList
						FROM
						(
							SELECT  TodayOrgEmpRank,  SUM(TodayOrgEmpCnt) AS TodayOrgEmpCnt,'' AS TodayOrgEmpList
							FROM
							(
								SELECT  TodayOrgEmpRank, COUNT(empid) AS TodayOrgEmpCnt, '' AS TodayOrgEmpList
								FROM
								(
									SELECT  dbo.get_grade_value(dbo.GETSCoreRank(v.Score)) AS TodayOrgEmpRank, 
													empid  
									FROM 
									(
											 SELECT  k.Score, k.empid FROM #tmpViewEmpScore k where k.companyid = @companyId
												and assessmentid = (SELECT  MAX(assessmentId) AS Id FROM #tmpViewEmpScore s where k.empid = s.empid
												--AND DATEPART(yyyy,Ratingdate) = @YearID -- DatePart(yyyy, GETDATE())
												)
									) v
								) g GROUP BY TodayOrgEmpRank
							) v1 GROUP BY TodayOrgEmpRank
						) k 
					) v
				GROUP BY  DrctScoreRank,--DirectRptAvgScoreRank,
				DrctmonYrId,OrgRank,-- OrgRptAvgScoreRank,
				OrgMonYrId,DrctmonYr,OrgMonYr,TodayRptEmpCnt, TodayRptEmpRank, TodayOrgEmpCnt, TodayOrgEmpRank,
				DrctAvgScore, AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, OrgAvgScore, AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr,TodayRptEmpList,TodayOrgEmpList
		) t
	) f
END

ELSE IF (@managerId > 0 AND @companyId > 0 AND @UserRole NOT IN (2,4))
BEGIN
CREATE TABLE #tempEmployeeHierachy (
	companyid INT, empid INT ,empName VARCHAR(1000),Score DECIMAL(2,0),ScoreRank VARCHAR(50),
	RatingDate DATETIME, assessmentbyId INT,assessmentBy VARCHAR(1000), 
	assessmentId INT, managerId INT, departmentid INT, roleId int, empStatus BIT,inDirectManager INT,  yearId INT
	)
	
	INSERT INTO #tempEmployeeHierachy EXEC [usp_GetYearWiseHierachywiseDetails] @YearID, @managerId, @companyId, @departmentlist;

	WITH #tempEmployeeM (empid, companyid, managerid, roleid, departmentid, empstatus, JoiningYearId)--, inDirectManager)
	AS
	(
		SELECT empId, companyId, managerId, roleId, departmentId, empStatus, Format(joiningdate, 'yyyy') AS JoiningYearId --, CAST('' AS VARCHAR(2000))
		FROM EmployeeDetails AS FirstLevel
		WHERE bactive = 1 AND empStatus = 1 
		AND managerid= @managerId AND companyID = @companyID
		UNION ALL
		SELECT NextLevel.empID, NextLevel.companyId, NExtLevel.managerId,
			NextLevel.roleId, NextLevel.departmentId, NextLevel.empStatus, Format(NextLevel.joiningdate, 'yyyy') AS JoiningYearId--,
			--CAST(CASE WHEN t.inDirectManager = '' 
			--THEN (CAST(NextLevel.managerId AS VARCHAR(2000)))
			--ELSE (t.inDirectManager + ',' + CAST(NextLevel.managerID AS VARCHAR(2000)))
			--END AS VARCHAR(2000))
		FROM EmployeeDetails AS NextLevel
		INNER JOIN #tempEmployeeM AS t ON NextLEvel.managerId = t.empId
		WHERE NExtLEvel.bactive = 1 AND NextLEvel.empStatus = 1 AND NextLevel.companyID = @companyId
	)SELECT * INTO #tempEmployeeM
FROM #tempEmployeeM WHERE JoiningYearId <= CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END

	SELECT  * INTO #tmpM
	FROM(
			SELECT avgscore,empid,managerId,monyrid,monyr
			FROM
			(
				SELECT row_number() OVER (Partition BY empid, FORMAT(RatingDate,'yyyyMM') order by empid, FORMAT(RatingDate,'yyyyMM')) AS rownumber,
					CAST(ROUND(AVG(score) OVER (Partition BY empid, FORMAT(RatingDate,'yyyyMM')),0) AS decimal(10,0)) as avgscore,
					empid, managerId, FORMAT(RatingDate,'yyyyMM') as monyrid, FORMAT(RatingDate,'MMM') as monyr
				FROM #tempEmployeeHierachy
				--WHERE managerID = @managerId
			) a WHERE rownumber=1
		) p
	
    SELECT  CntDirectEmps, DirectRptAvgScore,DirectRptAvgScoreRank, CntOrgEmps,OrgRptAvgScore,
	OrgRptAvgScoreRank, grphDrctRpt,   DrctScoreRank, DrctmonYrId, DrctmonYr,
	TotDirctEmps,DrctPct,grphOrgRpt,OrgRank,OrgMonYrId,OrgMonYr,TotOrgEmps,OrgPct,DrctAvgScore,AvgDrctRank,
	DrctAvgMonYrId,DrctAvgMonYr,OrgAvgScore,AvgOrgRank,OrgAvgMonYrId,OrgAvgMonYr,TodayRptEmpCnt,TodayRptEmpRank,
	TodayRptEmpList,TodayOrgEmpCnt,TodayOrgEmpRank,TodayOrgEmpList
	INTO #tempM
	FROM
	(
	SELECT  0 AS CntDirectEmps, 0 AS DirectRptAvgScore,'' AS DirectRptAvgScoreRank,  0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, grphDrctRpt,
					  DrctScoreRank, monyrid AS DrctmonYrId, monyr AS DrctmonYr,
					  TotDirctEmps,
					  (ROUND(CAST((100 / CAST(TotDirctEmps AS decimal)) AS DECIMAL(10,0)),0) * grphDrctRpt) AS DrctPct, 0 AS grphOrgRpt,
					  '' AS OrgRank, '' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
					  0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
					  0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
					  0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
				FROM
				(
					SELECT  DrctScoreRank, monyrid, monyr, SUM(grphDrctRpt) AS grphDrctRpt, TotDirctEmps
					FROM
					(
						SELECT  dbo.Get_grade_value(dbo.GETSCoreRank(avgscore)) AS DrctScoreRank,
								monyrid, monyr,  COUNT(empid) AS grphDrctRpt, COUNT(empid) OVER (Partition BY monyrid) AS TotDirctEmps
						FROM
						(
							SELECT empid,avgscore as avgscore,m.yearmonth as monyrid,m.monthname as monyr FROM #monthlist m 
							LEFT OUTER JOIN #tmpM s on s.monyrid =  (SELECT  MAX(monyrid) FROM #tmpM WHERE monyrid < yearmonth and empid = s.empid)
							WHERE yearmonth NOT IN(SELECT  monyrid FROM #tmpM WHERE monyrid <> '' and empid = s.empid) and  empid = s.empid and managerid = @managerId 
							UNION ALL
							SELECT  empid,avgscore,monyrid, monyr FROM  #tmpM where managerid = @managerId
		 				) v
						GROUP BY avgscore,monyrid, monyr, empid
						) v1 GROUP BY DrctScoreRank, monyrid, monyr,TotDirctEmps
				) k
		UNION
	  SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore,'' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, 0 AS grphDrctRpt,
			   '' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
			   grphOrgRpt, OrgRank, monyrid AS  OrgMonYrId, monyr AS OrgMonYr, TotOrgEmps, 
			   (ROUND(CAST((100 / CAST(TotOrgEmps AS decimal)) AS DECIMAL(10,0)),0) * grphOrgRpt) AS OrgPct,
			   0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
			   0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
			   0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
			FROM
			(
			  SELECT  OrgRank, monyrid, monyr, SUM(grphOrgRpt) AS grphOrgRpt, TotOrgEmps 
			  FROM (
				SELECT  dbo.Get_grade_value(dbo.GETSCoreRank(avgscore)) AS OrgRank,
				empid,monyrid, monyr,COUNT(empid) AS grphOrgRpt, COUNT(empid) OVER (Partition BY monyrid) AS TotOrgEmps FROM
				 (
					 SELECT  empid,avgscore as avgscore,m.yearmonth as monyrid,m.monthname as monyr 
					 FROM #monthlist m left outer join #tmpM s on s.monyrid =  (SELECT  MAX(monyrid) FROM #tmpM WHERE monyrid < yearmonth and empid = s.empid)
					 WHERE yearmonth NOT IN(SELECT  monyrid FROM #tmpM WHERE monyrid <> '' and empid = s.empid) and  empid = s.empid 
					 union all
					 SELECT  empid,avgscore,monyrid, monyr FROM  #tmpM)a 
					 GROUP BY avgscore,monyrid, monyr, empid) B
					 group by OrgRank, monyrid, monyr,TotOrgEmps) k
				 UNION
				 SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore,'' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore,'' AS OrgRptAvgScoreRank, 0 AS grphDrctRpt,
							  '' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
							  0 AS grphOrgRpt, '' AS OrgRank, '' AS OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct, 
							  avgscore AS DrctAvgScore, AvgDrctRank, monyrid AS DrctAvgMonYrId, monyr AS DrctAvgMonYr, 
							  0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
							  0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
				FROM
				(
						SELECT dbo.GETSCoreRank(avgscore) AS AvgDrctRank, 
								monyrid, monyr,avgscore
						FROM (
							
							 SELECT  distinct CAST(ROUND(AVG(score) OVER (Partition BY FORMAT(RatingDate,'yyyyMM')),0) AS decimal(10,0)) as avgscore,    
							 FORMAT(RatingDate,'yyyyMM') as monyrid, FORMAT(RatingDate,'MMM') as monyr FROM #tempEmployeeHierachy 
							 WHERE managerId = @managerId
						) v
				) k
			UNION
			--GET SCORRANK WISE DATA OF ALL EMPLOYEES OF ORGANIZATION
			 SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore,'' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore,'' AS OrgRptAvgScoreRank, 0 AS grphDrctRpt,
					  '' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
					  0 AS grphOrgRpt, '' AS OrgRank, '' AS OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct, 
					  0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
					  avgscore AS OrgAvgScore, AvgOrgRank, monyrid AS OrgAvgMonYrId, monyr AS OrgAvgMonYr, 
					  0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank, '' AS TodayRptEmpList,0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
				FROM
				(
						SELECT 	dbo.GETSCoreRank(avgscore) AS AvgOrgRank, 
											monyrid, monyr,avgscore
						FROM (
						
							 SELECT  distinct CAST(ROUND(AVG(score) OVER (Partition BY FORMAT(RatingDate,'yyyyMM')),0) AS decimal(10,0)) as avgscore,    
								FORMAT(RatingDate,'yyyyMM') as monyrid, FORMAT(RatingDate,'MMM') as monyr 
							 FROM #tempEmployeeHierachy 
							 --WHERE (managerId = @managerId) -- OR inDirectManager = @managerId)
							 --OR (',' + RTRIM(inDirectManager) + ',') LIKE '%,' + CAST(@managerId AS VARCHAR(1000)) + ',%' )
						) v
				) k

)l
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore,'' AS OrgRptAvgScoreRank,
			ISNULL(grphDrctRpt,0) AS grphDrctRpt, ISNULL(DrctScoreRank,'') AS DrctScoreRank, yearmonth AS DrctmonYrId, monthname  AS DrctmonYr, 
			ISNULL(TotDirctEmps,0) AS TotDirctEmps, ISNULL(DrctPct,0) AS DrctPct, 0 AS grphOrgRpt,
							  '' AS OrgRank, '' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
							  0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							  0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
							  0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
							  FROM #monthlist m LEFT OUTER JOIN #tempM s on s.DrctmonYrId = (SELECT  MAX(drctmonyrid) FROM #tempM WHERE drctmonyrid < yearmonth)
			WHERE yearmonth NOT IN(SELECT  DrctmonYrId FROM #tempM WHERE DrctmonYrId <> '')
			--WHERE EXISTS(SELECT  'Y' FROM #tempM r WHERE r.DrctmonYrId <> '' AND r.DrctmonYrId <> m.yearmonth)
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank,
			ISNULL(grphDrctRpt,0) AS grphDrctRpt, ISNULL(DrctScoreRank,'') AS DrctScoreRank, DrctmonYrId, DrctmonYr, ISNULL(TotDirctEmps,0) AS TotDirctEmps, ISNULL(DrctPct,0) AS DrctPct,
			ISNULL(grphOrgRpt,0) AS grphOrgRpt, OrgRank,OrgMonYrId,OrgMonYr, TotOrgEmps, OrgPct,
			ISNULL(DrctAvgScore,0) AS DrctAvgScore, ISNULL(AvgDrctRank,'') AS AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, 
			ISNULL(OrgAvgScore,0) AS OrgAvgScore, ISNULL(AvgOrgRank,'') AS AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
			TodayRptEmpCnt, TodayRptEmpRank,'' AS TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank,'' AS TodayOrgEmpList 
			FROM #tempM
			WHERE DrctmonYrId <> ''
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, 0 AS grphdrctrpt,'' AS drctscorerank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
						ISNULL(grphOrgRpt,0) AS grphOrgRpt, ISNULL(OrgRank,'') AS OrgRank, yearmonth AS OrgMonYrId, monthname  AS OrgMonYr, ISNULL(TotOrgEmps,0) AS TotOrgEmps, ISNULL(OrgPct,0) AS OrgPct,
						0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
						FROM #monthlist m LEFT OUTER JOIN #tempM s on s.OrgMonYrId = (SELECT  MAX(OrgMonYrId)  FROM #tempM WHERE OrgMonYrId < yearmonth)
			WHERE yearmonth NOT IN(SELECT  OrgMonYrId  FROM #tempM WHERE OrgMonYrId <> '') 
			UNION
			SELECT   0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank,
			grphDrctRpt, DrctScoreRank, DrctmonYrId, DrctmonYr, TotDirctEmps, DrctPct,
			ISNULL(grphOrgRpt,0) AS grphOrgRpt, ISNULL(OrgRank,'') AS OrgRank, OrgMonYrId,OrgMonYr, ISNULL(TotOrgEmps,0) AS TotOrgEmps,  ISNULL(OrgPct,0) AS OrgPct,
			ISNULL(DrctAvgScore,0) AS DrctAvgScore, ISNULL(AvgDrctRank,'') AS AvgDrctRank,  DrctAvgMonYrId, DrctAvgMonYr, 
			ISNULL(OrgAvgScore,0) AS OrgAvgScore, ISNULL(AvgOrgRank,'') AS AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
			TodayRptEmpCnt, TodayRptEmpRank,'' AS TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank,'' AS TodayOrgEmpList 
			FROM #tempM WHERE OrgMonYrId <> ''
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, 0 AS grphdrctrpt,'' AS drctscorerank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
						0 AS grphOrgRpt, '' AS OrgRank, '' AS OrgMonYrId, ''  AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
						ISNULL(DrctAvgScore,0) AS DrctAvgScore, ISNULL(AvgDrctRank,'') AS AvgDrctRank, yearmonth AS DrctAvgMonYrId, monthname AS DrctAvgMonYr, 
						0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
						FROM #monthlist m LEFT OUTER JOIN #tempM s on s.DrctAvgMonYrId = (SELECT  MAX(DrctAvgMonYrId)  FROM #tempM WHERE DrctAvgMonYrId < yearmonth)
						WHERE yearmonth NOT IN(SELECT  DrctAvgMonYrId  FROM #tempM  WHERE DrctAvgMonYrId <> '')
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank,
					 0 AS grphdrctrpt,'' AS drctscorerank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
						0 AS grphOrgRpt, '' AS OrgRank, '' AS OrgMonYrId, ''  AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
						ISNULL(DrctAvgScore,0) AS DrctAvgScore, ISNULL(AvgDrctRank,'') AS AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, 
						0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
			FROM #tempM WHERE DrctAvgMonYrId <> ''
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, 0 AS grphdrctrpt,'' AS drctscorerank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
						0 AS grphOrgRpt, '' AS OrgRank,  '' AS OrgMonYrId, ''  AS OrgMonYr, 0 as TotOrgEmps, 0 as OrgPct,
						0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						ISNULL(OrgAvgScore,0) AS OrgAvgScore, ISNULL(AvgOrgRank,'') AS AvgOrgRank, yearmonth AS OrgAvgMonYrId, monthname AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
						FROM #monthlist m LEFT OUTER JOIN #tempM s on s.OrgAvgMonYrId = (SELECT  MAX(OrgAvgMonYrId)  FROM #tempM WHERE OrgAvgMonYrId < yearmonth)
						WHERE yearmonth NOT IN(SELECT  OrgAvgMonYrId  FROM #tempM WHERE OrgAvgMonYrId <> '')
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, 0 AS grphdrctrpt,'' AS drctscorerank, 
						'' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
						0 AS grphOrgRpt, '' AS OrgRank,  '' AS OrgMonYrId, ''  AS OrgMonYr, 0 as TotOrgEmps, 0 as OrgPct,
						0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						 ISNULL(OrgAvgScore,0) AS OrgAvgScore,  ISNULL(AvgOrgRank,'') AS AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank, '' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
			FROM #tempM WHERE OrgAvgMonYrId <> ''
			UNION
	SELECT  CntDirectEmps, ISNULL(DirectRptAvgScore,0) AS DirectRptAvgScore, DirectRptAvgScoreRank, 
	CntOrgEmps, ISNULL(OrgRptAvgScore,0) AS OrgRptAvgScore, OrgRptAvgScoreRank,
	grphDrctRpt, DrctScoreRank, DrctmonYrId, DrctmonYr, TotDirctEmps, DrctPct,
	grphOrgRpt, OrgRank,OrgMonYrId,OrgMonYr, TotOrgEmps, OrgPct,
	DrctAvgScore, AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, 
	OrgAvgScore, AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
	TodayRptEmpCnt, TodayRptEmpRank, TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank,TodayOrgEmpList
	FROM
	(
		SELECT  CntDirectEmps, DirectRptAvgScore, CASE WHEN DirectRptAvgScore IS NULL THEN '' ELSE DBO.GETScoreRank(DirectRptAvgScore) END AS DirectRptAvgScoreRank,
		CntOrgEmps, OrgRptAvgScore, CASE WHEN OrgRptAvgScore IS NULL THEN '' ELSE DBO.GETScoreRank(OrgRptAvgScore) END AS OrgRptAvgScoreRank,
		grphDrctRpt, DrctScoreRank, DrctmonYrId, DrctmonYr, TotDirctEmps, DrctPct,
		grphOrgRpt, OrgRank,OrgMonYrId,OrgMonYr, TotOrgEmps, OrgPct,
		DrctAvgScore, AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, 
		OrgAvgScore, AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
		TodayRptEmpCnt, TodayRptEmpRank, TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank,TodayOrgEmpList
		FROM
		(
				SELECT  SUM(CntDirectEmps) AS CntDirectEmps, SUM(DirectRptAvgScore) AS DirectRptAvgScore, --DirectRptAvgScoreRank,
				SUM(CntOrgEmps) AS CntOrgEmps, SUM(OrgRptAvgScore) AS OrgRptAvgScore, --OrgRptAvgScoreRank,
				SUM(grphDrctRpt) AS grphDrctRpt, DrctScoreRank, DrctmonYrId, DrctmonYr, SUM(TotDirctEmps) AS TotDirctEmps, 
				SUM(DrctPct) AS DrctPct, SUM(grphOrgRpt) AS grphOrgRpt, OrgRank, OrgMonYrId, OrgMonYr,SUM(TotOrgEmps) AS TotOrgEmps, SUM(OrgPct) AS OrgPct,
				DrctAvgScore, AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, 
				OrgAvgScore, AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
				TodayRptEmpCnt, TodayRptEmpRank, TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank,TodayOrgEmpList
				FROM
				(
					---GET NO OF EMPLOYEES DIRECLTLY REPORT TO MANAGER
					SELECT  Count(empId) AS CntDirectEmps,NULL AS DirectRptAvgScore, 
						--'' AS DirectRptAvgScoreRank, 
						0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,
						--'' AS OrgRptAvgScoreRank,
						0 AS grphDrctRpt,
						'' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, '' AS  OrgMonYrId, '' AS OrgMonYr, 
						0 AS TotOrgEmps, 0 AS OrgPct,
						0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank, '' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
					FROM #tempEmployeedetails e 
					WHERE e.managerid = @managerId --AND e.companyid = @companyId AND e.departmentId IN (SELECT department FROM #departmentlist)
					UNION
					---GET AVERAGE SCORE OF TODAY EMPLOYEES DIRECLTLY REPORT TO MANAGER
					SELECT  0 AS CntDirectEmps, CASE WHEN DirectRptAvgScore1 IS NULL THEN DirectRptAvgScore2 ELSE DirectRptAvgScore1 END AS DirectRptAvgScore, 0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
						0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
						'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
						0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
					FROM
					(
						SELECT  0 AS CntDirectEmps, SUM(DirectRptAvgScore1) AS DirectRptAvgScore1, SUM(DirectRptAvgScore2) AS DirectRptAvgScore2,  0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
						0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
						'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
						0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
						FROM
						(
							SELECT  0 AS CntDirectEmps, CAST(ROUND(AVG(Score),0) AS DECIMAL(10,0)) AS DirectRptAvgScore1, NULL AS DirectRptAvgScore2,
												0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
												0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
												'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
												0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
												0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
												0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
											FROM #tempEmployeeHierachy vs
											WHERE CONVERT(DATE, vs.RatingDate, 101) = @LastDate --CONVERT(DATE, GETDATE(), 101)
												AND vs.managerId = @managerId --AND vs.companyid = @companyId AND vs.departmentId IN (SELECT department FROM #departmentlist)
											UNION
							SELECT  0 AS CntDirectEmps, NULL AS DirectRptAvgScore1, CAST(ROUND(AVG(Score),0) AS DECIMAL(10,0)) AS DirectRptAvgScore2,
												0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
												0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
												'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
												0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
												0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
												0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
					   							FROM
												(
												SELECT  k.Score, k.empid, k.empNAme FROM #tempEmployeeHierachy k
												where k.managerId = @managerId 
												and assessmentid = (SELECT  MAX(assessmentId) AS Id FROM #tempEmployeeHierachy s where k.empid = s.empid
												--AND DATEPART(yyyy,Ratingdate) = @YearID
												) 
												) v
						) k
					) v
					UNION
					---GET AVERAGE SCORE OF TODAY OF ALL EMPLOYEES OF ORGANIZATION
					SELECT  0 AS CntDirectEmps, NULL AS DirectRptAvgScore, 0 AS CntOrgEmps, CASE WHEN OrgRptAvgScore1 IS NULL THEN OrgRptAvgScore2 ELSE OrgRptAvgScore1 END AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
						0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
						'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
						0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
					FROM
					(	
						SELECT  0 AS CntDirectEmps, NULL AS DirectRptAvgScore, 0 AS CntOrgEmps, SUM(OrgRptAvgScore1) AS OrgRptAvgScore1, SUM(OrgRptAvgScore2) AS OrgRptAvgScore2,--'' AS OrgRptAvgScoreRank, 
							0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
							'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
							0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
							0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
						FROM
						(					
											SELECT  0 AS CntDirectEmps, NULL AS DirectRptAvgScore,-- '' AS DirectRptAvgScoreRank,
												0 AS CntOrgEmps, CAST(ROUND(AVG(Score), 0) AS DECIMAL(10,0)) AS OrgRptAvgScore1, NULL AS OrgRptAvgScore2, 0 AS grphDrctRpt,
												'' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
												'' AS  OrgMonYrId,'' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
												0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
												0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
												0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
											FROM #tempEmployeeHierachy vs
											WHERE CONVERT(DATE, vs.RatingDate, 101) = @LastDate 
											--AND (managerId = @managerId)
											--OR (',' + RTRIM(inDirectManager) + ',') LIKE '%,' + CAST(@managerId AS VARCHAR(1000)) + ',%' )
											--GROUP BY vs.inDirectManager
											UNION
											SELECT  0 AS CntDirectEmps,  NULL AS DirectRptAvgScore,
												0 AS CntOrgEmps, NULL AS OrgRptAvgScore1, CAST(ROUND(AVG(Score),0) AS DECIMAL(10,0)) AS OrgRptAvgScore2,
												0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
												'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
												0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
												0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
												0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
					   							FROM
												(
												SELECT  k.Score, k.empid, k.empNAme FROM #tempEmployeeHierachy k 
												where --(managerId = @managerId) --and
												 --OR (',' + RTRIM(inDirectManager) + ',') LIKE '%,' + CAST(@managerId AS VARCHAR(1000)) + ',%' )
													 assessmentid = (SELECT  MAX(assessmentId) AS Id FROM #tempEmployeeHierachy s where k.empid = s.empid) 
												) v
						) o
					)p
					UNION					
					---GET NO OF ALL EMPLOYEES OF ORGANIZATION
					SELECT  0 AS CntDirectEmps,NULL AS DirectRptAvgScore, 
						  Count(*) AS CntOrgEmps,  NULL AS OrgRptAvgScore, 
						   0 AS grphDrctRpt,
						'' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, '' AS  OrgMonYrId,'' AS OrgMonYr,
						  0 AS TotOrgEmps, 0 AS OrgPct,
						  0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						  0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						  0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank, '' AS TodayOrgEmpList  
					FROM #tempEmployeeM e 
					WHERE e.departmentId IN(SELECT department FROM #departmentList)
					AND e.roleid NOT IN (4,2)
					--AND (e.managerId = @managerId) --OR (',' + RTRIM(e.inDirectManager) + ',') LIKE '%,' + CAST(@managerId AS VARCHAR(1000)) + ',%' )
					UNION
					---Employee current Score  of direct report
					SELECT  0 AS CntDirectEmps,NULL AS DirectRptAvgScore, 0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
						 0 AS grphDrctRpt,
						'' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, ''  AS OrgRank,
							'' AS OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
							0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
			    			TodayRptEmpCnt, TodayRptEmpRank,TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
					FROM
					(
						SELECT  TodayRptEmpRank,  SUM(TodayRptEmpCnt) AS TodayRptEmpCnt, '' AS TodayRptEmpList
						FROM
						(
							SELECT  TodayRptEmpRank, COUNT(empid) AS TodayRptEmpCnt
							FROM
							(
								SELECT  dbo.get_grade_value(dbo.GETSCoreRank(v.Score)) AS TodayRptEmpRank, 
													empid  --, COUNT(empid) OVER (Partition BY monyrid) AS TodayRptEmpCnt
								FROM 
								(
									 SELECT  k.Score, k.empid, k.empNAme FROM #tempEmployeeHierachy k where k.managerid = @managerId
										and assessmentid = (SELECT  MAX(assessmentId) AS Id FROM #tempEmployeeHierachy s where k.empid = s.empid)
								) v
							) g GROUP BY TodayRptEmpRank
						) v1 GROUP BY TodayRptEmpRank
					) k
					UNION
						---Employee current Score  of whole organization
					SELECT  0 AS CntDirectEmps,NULL AS DirectRptAvgScore, --'' AS DirectRptAvgScoreRank, 
					  0 AS CntOrgEmps,  NULL AS OrgRptAvgScore, --'' AS OrgRptAvgScoreRank,
					  0 AS grphDrctRpt,
						'' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, ''  AS OrgRank,
							'' AS OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
							0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
			    			0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank, TodayOrgEmpList
					FROM
					(
						SELECT  TodayOrgEmpRank,  SUM(TodayOrgEmpCnt) AS TodayOrgEmpCnt, '' AS TodayOrgEmpList
						FROM
						(
							SELECT  TodayOrgEmpRank, COUNT(empid) AS TodayOrgEmpCnt
							FROM
							(
								SELECT  dbo.get_grade_value(dbo.GETSCoreRank(v.Score)) AS TodayOrgEmpRank, 
														empid  --, COUNT(empid) OVER (Partition BY monyrid) AS TodayRptEmpCnt
									FROM 
									(
										 SELECT  k.Score, k.empid, k.empNAme FROM #tempEmployeeHierachy k where 
											 assessmentid = (SELECT  MAX(assessmentId) AS Id FROM #tmpViewEmpScore s where k.empid = s.empid
											)
									) v
									
							) g GROUP BY TodayOrgEmpRank
						) v1 GROUP BY TodayOrgEmpRank
					) k 
				) v
				GROUP BY  DrctScoreRank,--DirectRptAvgScoreRank,
				DrctmonYrId,OrgRank,-- OrgRptAvgScoreRank,
				OrgMonYrId,DrctmonYr,OrgMonYr,TodayRptEmpCnt, TodayRptEmpRank, TodayOrgEmpCnt, TodayOrgEmpRank,
				DrctAvgScore, AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, OrgAvgScore, AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr,TodayRptEmpList,TodayOrgEmpList
		) t
	) f
END
ELSE
BEGIN
	SELECT  TOP 1 @managerId = empid FROM #tempEmployeedetails WHERE roleid =2 --companyid =@companyid AND bactive = 1 AND empstatus = 1 AND 
		SELECT  CntDirectEmps, DirectRptAvgScore,DirectRptAvgScoreRank, CntOrgEmps,OrgRptAvgScore,
	OrgRptAvgScoreRank, grphDrctRpt,   DrctScoreRank, DrctmonYrId, DrctmonYr,
	TotDirctEmps,DrctPct,grphOrgRpt,OrgRank,OrgMonYrId,OrgMonYr,TotOrgEmps,OrgPct,DrctAvgScore,AvgDrctRank,
	DrctAvgMonYrId,DrctAvgMonYr,OrgAvgScore,AvgOrgRank,OrgAvgMonYrId,OrgAvgMonYr,TodayRptEmpCnt,TodayRptEmpRank,
	TodayRptEmpList,TodayOrgEmpCnt,TodayOrgEmpRank,TodayOrgEmpList
	INTO #temp1
	FROM
	(
		SELECT  0 AS CntDirectEmps, 0 AS DirectRptAvgScore,'' AS DirectRptAvgScoreRank,  0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, grphDrctRpt,
					  DrctScoreRank, monyrid AS DrctmonYrId, monyr AS DrctmonYr,
					  TotDirctEmps,
					  (ROUND(CAST((100 / CAST(TotDirctEmps AS decimal)) AS DECIMAL(10,0)),0) * grphDrctRpt) AS DrctPct, 0 AS grphOrgRpt,
					  '' AS OrgRank, '' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
					  0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
					  0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
					  0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
		FROM
		(
					SELECT  DrctScoreRank, monyrid, monyr, SUM(grphDrctRpt) AS grphDrctRpt, TotDirctEmps
					FROM
					(
						SELECT dbo.Get_grade_value(dbo.GETSCoreRank(avgscore)) AS DrctScoreRank,
					    		monyrid, monyr,  COUNT(empid) AS grphDrctRpt, COUNT(empid) OVER (Partition BY monyrid) AS TotDirctEmps
						FROM (
								SELECT  empid,avgscore as avgscore,m.yearmonth as monyrid,m.monthname as monyr FROM #monthlist m 
								left outer join #tmpEmpScore s on s.monyrid =  (SELECT  MAX(monyrid) FROM #tmpEmpScore WHERE monyrid < yearmonth and empid = s.empid)
								WHERE  empid = s.empid and managerid = @managerId AND 
								yearmonth NOT IN(SELECT  monyrid FROM #tmpEmpScore WHERE empid = s.empid)  
								-- exists(SELECT  'Y' FROM #tmpEmpScore r WHERE r.empid = s.empid AND m.yearmonth <> r.monyrid)
								UNION ALL
								SELECT  empid,avgscore,monyrid, monyr FROM  #tmpEmpScore where managerid = @managerId
		 					) v
						GROUP BY avgscore,monyrid, monyr, empid
					) v1 GROUP BY DrctScoreRank, monyrid, monyr,TotDirctEmps
		) k
	  UNION
	  SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore,'' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, 0 AS grphDrctRpt,
			   '' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
			   grphOrgRpt, OrgRank, monyrid AS  OrgMonYrId, monyr AS OrgMonYr, TotOrgEmps, 
			   (ROUND(CAST((100 / CAST(TotOrgEmps AS decimal)) AS DECIMAL(10,0)),0) * grphOrgRpt) AS OrgPct,
			   0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
			   0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
			   0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
	  FROM
	  (
		  SELECT  OrgRank, monyrid, monyr, SUM(grphOrgRpt) AS grphOrgRpt, TotOrgEmps
		  FROM (
				SELECT  dbo.Get_grade_value(dbo.GETSCoreRank(avgscore)) AS OrgRank,
						empid,monyrid, monyr,COUNT(empid) AS grphOrgRpt, COUNT(empid) OVER (Partition BY monyrid) AS TotOrgEmps FROM
				(
					 SELECT  empid,avgscore as avgscore,m.yearmonth as monyrid,m.monthname as monyr FROM #monthlist m 
					 left outer join #tmpEmpScore s on s.monyrid =  (SELECT  MAX(monyrid) FROM #tmpEmpScore WHERE monyrid < yearmonth and empid = s.empid)
					 WHERE yearmonth NOT IN(SELECT  monyrid FROM #tmpEmpScore r  WHERE r.empid = s.empid) and  empid = s.empid 
					 --WHERE   exists(SELECT  'Y' FROM #tmpEmpScore r  WHERE r.empid = s.empid and r.monyrid <> m.yearmonth) and  empid = s.empid 
					 UNION ALL
					 SELECT  empid,avgscore,monyrid, monyr FROM  #tmpEmpScore)a 
					 GROUP BY avgscore,monyrid, monyr, empid) B
					 GROUP BY OrgRank, monyrid, monyr,TotOrgEmps) k
					 UNION
					 SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore,'' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore,'' AS OrgRptAvgScoreRank, 0 AS grphDrctRpt,
								  '' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
								  0 AS grphOrgRpt, '' AS OrgRank, '' AS OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct, 
								  avgscore AS DrctAvgScore, AvgDrctRank, monyrid AS DrctAvgMonYrId, monyr AS DrctAvgMonYr, 
								  0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
								  0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
							FROM
							(
									SELECT  dbo.GETSCoreRank(avgscore) AS AvgDrctRank, 
											monyrid, monyr,avgscore
						FROM (
								--SELECT avgscore, monyrid, monyr FROM #tmpAvgEmpScore WHERE managerId = @managerId
							 SELECT  distinct CAST(ROUND(AVG(score) OVER (Partition BY FORMAT(RatingDate,'yyyyMM')),0) AS decimal(10,0)) as avgscore,    
							 FORMAT(RatingDate,'yyyyMM') as monyrid, FORMAT(RatingDate,'MMM') as monyr FROM #tmpViewEmpScore 
							 WHERE managerId = @managerId
						) v
				) k
			UNION
			--GET SCORRANK WISE DATA OF ALL EMPLOYEES OF ORGANIZATION
			 SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore,'' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore,'' AS OrgRptAvgScoreRank, 0 AS grphDrctRpt,
					  '' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
					  0 AS grphOrgRpt, '' AS OrgRank, '' AS OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct, 
					  0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
					  avgscore AS OrgAvgScore, AvgOrgRank, monyrid AS OrgAvgMonYrId, monyr AS OrgAvgMonYr, 
					  0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank, '' AS TodayRptEmpList,0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
				FROM
				(
						SELECT  dbo.GETSCoreRank(avgscore) AS AvgOrgRank, 
											monyrid, monyr,avgscore
						FROM (
							 SELECT  distinct CAST(ROUND(AVG(score) OVER (Partition BY FORMAT(RatingDate,'yyyyMM')),0) AS decimal(10,0)) as avgscore,    
								FORMAT(RatingDate,'yyyyMM') as monyrid, FORMAT(RatingDate,'MMM') as monyr 
							 FROM #tmpViewEmpScore 
							--				 SELECT avgscore, monyrid, monyr FROM #tmpAvgEmpScore 
						) v
				) k

			)l
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore,'' AS OrgRptAvgScoreRank,
			ISNULL(grphDrctRpt,0) AS grphDrctRpt, ISNULL(DrctScoreRank,'') AS DrctScoreRank, yearmonth AS DrctmonYrId, monthname  AS DrctmonYr, 
			ISNULL(TotDirctEmps,0) AS TotDirctEmps, ISNULL(DrctPct,0) AS DrctPct, 0 AS grphOrgRpt,
							  '' AS OrgRank, '' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
							  0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							  0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
							  0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
							  FROM #monthlist m LEFT OUTER JOIN #temp1 s on s.DrctmonYrId = (SELECT  MAX(drctmonyrid) FROM #temp1 WHERE drctmonyrid < yearmonth)
			WHERE yearmonth NOT IN(SELECT  DrctmonYrId FROM #temp1 WHERE DrctmonYrId <> '')
			--WHERE EXISTS(SELECT 'Y' FROM #temp r WHERE r.DrctmonYrId <> '' AND m.yearmonth<>r.DrctmonYrId)
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank,
			ISNULL(grphDrctRpt,0) AS grphDrctRpt, ISNULL(DrctScoreRank,'') AS DrctScoreRank, DrctmonYrId, DrctmonYr, ISNULL(TotDirctEmps,0) AS TotDirctEmps, ISNULL(DrctPct,0) AS DrctPct,
			ISNULL(grphOrgRpt,0) AS grphOrgRpt, OrgRank,OrgMonYrId,OrgMonYr, TotOrgEmps, OrgPct,
			ISNULL(DrctAvgScore,0) AS DrctAvgScore, ISNULL(AvgDrctRank,'') AS AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, 
			ISNULL(OrgAvgScore,0) AS OrgAvgScore, ISNULL(AvgOrgRank,'') AS AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
			TodayRptEmpCnt, TodayRptEmpRank,'' AS TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank,'' AS TodayOrgEmpList FROM #temp1 
			WHERE DrctmonYrId <> ''
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, 0 AS grphdrctrpt,'' AS drctscorerank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
						ISNULL(grphOrgRpt,0) AS grphOrgRpt, ISNULL(OrgRank,'') AS OrgRank, yearmonth AS OrgMonYrId, monthname  AS OrgMonYr, ISNULL(TotOrgEmps,0) AS TotOrgEmps, ISNULL(OrgPct,0) AS OrgPct,
						0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
						FROM #monthlist m LEFT OUTER JOIN #temp1 s on s.OrgMonYrId = (SELECT  MAX(OrgMonYrId)  FROM #temp1 WHERE OrgMonYrId < yearmonth)
						WHERE yearmonth NOT IN(SELECT  OrgMonYrId  FROM #temp1 WHERE OrgMonYrId <> '') 
						--WHERE EXISTS (SELECT 'Y' FROM  #temp  r WHERE r.OrgMonYrId <> '' AND m.yearmonth<>r.OrgMonYrId  ) 
			UNION
			SELECT   0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank,
			grphDrctRpt, DrctScoreRank, DrctmonYrId, DrctmonYr, TotDirctEmps, DrctPct,
			ISNULL(grphOrgRpt,0) AS grphOrgRpt, ISNULL(OrgRank,'') AS OrgRank, OrgMonYrId,OrgMonYr, ISNULL(TotOrgEmps,0) AS TotOrgEmps,  ISNULL(OrgPct,0) AS OrgPct,
			ISNULL(DrctAvgScore,0) AS DrctAvgScore, ISNULL(AvgDrctRank,'') AS AvgDrctRank,  DrctAvgMonYrId, DrctAvgMonYr, 
			ISNULL(OrgAvgScore,0) AS OrgAvgScore, ISNULL(AvgOrgRank,'') AS AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
			TodayRptEmpCnt, TodayRptEmpRank,'' AS TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank,'' AS TodayOrgEmpList 
			FROM #temp1
			WHERE OrgMonYrId <> ''
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, 0 AS grphdrctrpt,'' AS drctscorerank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
						0 AS grphOrgRpt, '' AS OrgRank, '' AS OrgMonYrId, ''  AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
						ISNULL(DrctAvgScore,0) AS DrctAvgScore, ISNULL(AvgDrctRank,'') AS AvgDrctRank, yearmonth AS DrctAvgMonYrId, monthname AS DrctAvgMonYr, 
						0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
						FROM #monthlist m LEFT OUTER JOIN #temp1 s on s.DrctAvgMonYrId = (SELECT  MAX(DrctAvgMonYrId)  FROM #temp1 WHERE DrctAvgMonYrId < yearmonth)
						WHERE yearmonth NOT IN(SELECT  DrctAvgMonYrId  FROM #temp1  WHERE DrctAvgMonYrId <> '')
						--WHERE EXISTS(SELECT 'Y' FROM #temp r WHERE r.DrctAvgMonYrId <> '' AND  m.yearmonth <> r.DrctAvgMonYrId )
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank,
					 0 AS grphdrctrpt,'' AS drctscorerank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
						0 AS grphOrgRpt, '' AS OrgRank, '' AS OrgMonYrId, ''  AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
						ISNULL(DrctAvgScore,0) AS DrctAvgScore, ISNULL(AvgDrctRank,'') AS AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, 
						0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
			FROM #temp1 WHERE DrctAvgMonYrId <> ''
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, 0 AS grphdrctrpt,'' AS drctscorerank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
						0 AS grphOrgRpt, '' AS OrgRank,  '' AS OrgMonYrId, ''  AS OrgMonYr, 0 as TotOrgEmps, 0 as OrgPct,
						0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						ISNULL(OrgAvgScore,0) AS OrgAvgScore, ISNULL(AvgOrgRank,'') AS AvgOrgRank, yearmonth AS OrgAvgMonYrId, monthname AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
						FROM #monthlist m LEFT OUTER JOIN #temp1 s on s.OrgAvgMonYrId = (SELECT  MAX(OrgAvgMonYrId)  FROM #temp1 WHERE OrgAvgMonYrId < yearmonth)
						WHERE yearmonth NOT IN(SELECT  OrgAvgMonYrId  FROM #temp1 WHERE OrgAvgMonYrId <> '')
						--WHERE EXISTS(SELECT 'Y'  FROM #temp r WHERE r.OrgAvgMonYrId <> ''  AND m.yearmonth <> r.OrgAvgMonYrId )
			UNION
			SELECT  0 AS CntDirectEmps,0 AS DirectRptAvgScore, '' AS DirectRptAvgScoreRank, 0 AS CntOrgEmps,  0 AS OrgRptAvgScore, '' AS OrgRptAvgScoreRank, 0 AS grphdrctrpt,'' AS drctscorerank, 
						'' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirectEmps,  0 AS DrctPct,
						0 AS grphOrgRpt, '' AS OrgRank,  '' AS OrgMonYrId, ''  AS OrgMonYr, 0 as TotOrgEmps, 0 as OrgPct,
						0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						 ISNULL(OrgAvgScore,0) AS OrgAvgScore,  ISNULL(AvgOrgRank,'') AS AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank, '' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList FROM #temp1 WHERE OrgAvgMonYrId <> ''
			UNION
			SELECT  CntDirectEmps, ISNULL(DirectRptAvgScore,0) AS DirectRptAvgScore, DirectRptAvgScoreRank, 
			CntOrgEmps, ISNULL(OrgRptAvgScore,0) AS OrgRptAvgScore, OrgRptAvgScoreRank,
			grphDrctRpt, DrctScoreRank, DrctmonYrId, DrctmonYr, TotDirctEmps, DrctPct,
			grphOrgRpt, OrgRank,OrgMonYrId,OrgMonYr, TotOrgEmps, OrgPct,
			DrctAvgScore, AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, 
			OrgAvgScore, AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
			TodayRptEmpCnt, TodayRptEmpRank, TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank,TodayOrgEmpList
			FROM
			(
				SELECT  CntDirectEmps, DirectRptAvgScore, CASE WHEN DirectRptAvgScore IS NULL THEN '' ELSE DBO.GETScoreRank(DirectRptAvgScore) END AS DirectRptAvgScoreRank,
				CntOrgEmps, OrgRptAvgScore, CASE WHEN OrgRptAvgScore IS NULL THEN '' ELSE DBO.GETScoreRank(OrgRptAvgScore) END AS OrgRptAvgScoreRank,
				grphDrctRpt, DrctScoreRank, DrctmonYrId, DrctmonYr, TotDirctEmps, DrctPct,
				grphOrgRpt, OrgRank,OrgMonYrId,OrgMonYr, TotOrgEmps, OrgPct,
				DrctAvgScore, AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, 
				OrgAvgScore, AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
				TodayRptEmpCnt, TodayRptEmpRank, TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank,TodayOrgEmpList
				FROM
				(
						SELECT  SUM(CntDirectEmps) AS CntDirectEmps, SUM(DirectRptAvgScore) AS DirectRptAvgScore, --DirectRptAvgScoreRank,
						SUM(CntOrgEmps) AS CntOrgEmps, SUM(OrgRptAvgScore) AS OrgRptAvgScore, --OrgRptAvgScoreRank,
						SUM(grphDrctRpt) AS grphDrctRpt, DrctScoreRank, DrctmonYrId, DrctmonYr, SUM(TotDirctEmps) AS TotDirctEmps, 
						SUM(DrctPct) AS DrctPct, SUM(grphOrgRpt) AS grphOrgRpt, OrgRank, OrgMonYrId, OrgMonYr,SUM(TotOrgEmps) AS TotOrgEmps, SUM(OrgPct) AS OrgPct,
						DrctAvgScore, AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, 
						OrgAvgScore, AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr, 
						TodayRptEmpCnt, TodayRptEmpRank, TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank,TodayOrgEmpList
						FROM
						(
						---GET NO OF EMPLOYEES DIRECLTLY REPORT TO MANAGER
						SELECT  Count(empid) AS CntDirectEmps,NULL AS DirectRptAvgScore, 
							--'' AS DirectRptAvgScoreRank, 
							0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,
							--'' AS OrgRptAvgScoreRank,
							0 AS grphDrctRpt,
							'' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, '' AS  OrgMonYrId, '' AS OrgMonYr, 
							0 AS TotOrgEmps, 0 AS OrgPct,
							0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
							0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank, '' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
						FROM #tempEmployeedetails e 
						WHERE --e.bactive = 1 AND e.empstatus = 1 AND
							 e.managerid = @managerId --AND e.companyid = @companyId AND e.departmentId IN (SELECT department FROM #departmentlist)
						UNION
						---GET AVERAGE SCORE OF TODAY EMPLOYEES DIRECLTLY REPORT TO MANAGER
						SELECT  0 AS CntDirectEmps, CASE WHEN DirectRptAvgScore1 IS NULL THEN DirectRptAvgScore2 ELSE DirectRptAvgScore1 END AS DirectRptAvgScore, 0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
							0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
							'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
							0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
							0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
						FROM
						(
							SELECT  0 AS CntDirectEmps, SUM(DirectRptAvgScore1) AS DirectRptAvgScore1, SUM(DirectRptAvgScore2) AS DirectRptAvgScore2,  0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
							0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
							'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
							0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
							0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
							FROM
							(
								SELECT  0 AS CntDirectEmps, CAST(ROUND(AVG(Score),0) AS DECIMAL(10,0)) AS DirectRptAvgScore1, NULL AS DirectRptAvgScore2,
													0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
													0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
													'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
													0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
													0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
													0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
												FROM #tmpViewEmpScore vs
												WHERE CONVERT(DATE, vs.RatingDate, 101) = @LastDate -- CONVERT(DATE, GETDATE(), 101)
													AND vs.managerId = @managerId 
												UNION
								SELECT  0 AS CntDirectEmps, NULL AS DirectRptAvgScore1, CAST(ROUND(AVG(Score),0) AS DECIMAL(10,0)) AS DirectRptAvgScore2,
													0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
													0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
													'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
													0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
													0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
													0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
					   								FROM
													(
													SELECT  k.Score, k.empid, k.empNAme FROM #tmpViewEmpScore k
													where k.managerId = @managerId
													and assessmentid = (SELECT  MAX(assessmentId) AS Id FROM #tmpViewEmpScore s where k.empid = s.empid
													
													) 
													) v
							) k
						) v
					UNION
					---GET AVERAGE SCORE OF TODAY OF ALL EMPLOYEES OF ORGANIZATION
					SELECT  0 AS CntDirectEmps, NULL AS DirectRptAvgScore, 0 AS CntOrgEmps, CASE WHEN OrgRptAvgScore1 IS NULL THEN OrgRptAvgScore2 ELSE OrgRptAvgScore1 END AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
						0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
						'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
						0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
					FROM
					(	
						SELECT  0 AS CntDirectEmps, NULL AS DirectRptAvgScore, 0 AS CntOrgEmps, SUM(OrgRptAvgScore1) AS OrgRptAvgScore1, SUM(OrgRptAvgScore2) AS OrgRptAvgScore2,--'' AS OrgRptAvgScoreRank, 
							0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
							'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
							0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
							0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList 
						FROM
						(					
											SELECT  0 AS CntDirectEmps, NULL AS DirectRptAvgScore,-- '' AS DirectRptAvgScoreRank,
												0 AS CntOrgEmps, CAST(ROUND(AVG(Score), 0) AS DECIMAL(10,0)) AS OrgRptAvgScore1, NULL AS OrgRptAvgScore2, 0 AS grphDrctRpt,
												'' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
												'' AS  OrgMonYrId,'' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
												0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
												0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
												0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
											FROM #tmpViewEmpScore vs
											WHERE CONVERT(DATE, vs.RatingDate, 101) = @LastDate --CONVERT(DATE, GETDATE(), 101) 
											UNION
											SELECT  0 AS CntDirectEmps,  NULL AS DirectRptAvgScore,
												0 AS CntOrgEmps, NULL AS OrgRptAvgScore1, CAST(ROUND(AVG(Score),0) AS DECIMAL(10,0)) AS OrgRptAvgScore2,
												0 AS grphDrctRpt, '' AS DrctScoreRank, '' AS DrctmonYrId,'' AS DrctmonYr,  0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, 
												'' AS  OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
												0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
												0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
												0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
					   							FROM
												(
												SELECT  k.Score, k.empid, k.empNAme FROM #tmpViewEmpScore k WHERE
												 assessmentid = (SELECT  MAX(assessmentId) AS Id FROM #tmpViewEmpScore s where k.empid = s.empid
												)
												) v
						) o
					)p
					UNION					
					---GET NO OF ALL EMPLOYEES OF ORGANIZATION
					SELECT  0 AS CntDirectEmps,NULL AS DirectRptAvgScore, 
						--'' AS DirectRptAvgScoreRank,
						  Count(empid) AS CntOrgEmps,  NULL AS OrgRptAvgScore, 
						  --'' AS OrgRptAvgScoreRank,
						   0 AS grphDrctRpt,
						'' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, '' AS OrgRank, '' AS  OrgMonYrId,'' AS OrgMonYr,
						  0 AS TotOrgEmps, 0 AS OrgPct,
						  0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
						  0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
						  0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank, '' AS TodayOrgEmpList
					FROM #tempEmployeedetails e 
					WHERE e.roleid <> 4 AND empid <> @ManagerId 
					UNION
					---Employee current Score  of direct report
					SELECT  0 AS CntDirectEmps,NULL AS DirectRptAvgScore, 0 AS CntOrgEmps,  NULL AS OrgRptAvgScore,--'' AS OrgRptAvgScoreRank, 
						 0 AS grphDrctRpt,
						'' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, ''  AS OrgRank,
							'' AS OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
							0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
			    			TodayRptEmpCnt, TodayRptEmpRank,TodayRptEmpList, 0 AS TodayOrgEmpCnt, '' AS TodayOrgEmpRank,'' AS TodayOrgEmpList
					FROM
					(
						SELECT  TodayRptEmpRank,  SUM(TodayRptEmpCnt) AS TodayRptEmpCnt,'' AS TodayRptEmpList
						FROM
						(
							SELECT  TodayRptEmpRank, COUNT(empid) AS TodayRptEmpCnt
							FROM
							(
								SELECT  dbo.get_grade_value(dbo.GETSCoreRank(v.Score)) AS TodayRptEmpRank, 
													empid  --, COUNT(empid) OVER (Partition BY monyrid) AS TodayRptEmpCnt
								FROM 
								(
									 SELECT  k.Score, k.empid, k.empNAme FROM #tmpViewEmpScore k where k.managerid = @managerId AND k.companyid = companyId
										and assessmentid = (SELECT  MAX(assessmentId) AS Id FROM #tmpViewEmpScore s where k.empid = s.empid
										--AND DATEPART(yyyy,Ratingdate) = @YearID --DatePart(yyyy, GETDATE())
										--AND s.departmentId IN (SELECT department FROM #departmentlist)
										)
								) v
							) g GROUP BY TodayRptEmpRank
						) v1 GROUP BY TodayRptEmpRank
					) k
					UNION
						---Employee current Score  of whole organization
					SELECT  0 AS CntDirectEmps,NULL AS DirectRptAvgScore, --'' AS DirectRptAvgScoreRank, 
					  0 AS CntOrgEmps,  NULL AS OrgRptAvgScore, --'' AS OrgRptAvgScoreRank,
					  0 AS grphDrctRpt,
						'' AS DrctScoreRank, '' AS DrctmonYrId, '' AS DrctmonYr, 0 AS TotDirctEmps, 0 AS DrctPct, 0 AS grphOrgRpt, ''  AS OrgRank,
							'' AS OrgMonYrId, '' AS OrgMonYr, 0 AS TotOrgEmps, 0 AS OrgPct,
							0 AS DrctAvgScore, '' AS AvgDrctRank, '' AS DrctAvgMonYrId, '' AS DrctAvgMonYr, 
							0 AS OrgAvgScore, '' AS AvgOrgRank, '' AS OrgAvgMonYrId, '' AS OrgAvgMonYr, 
			    			0 AS TodayRptEmpCnt, '' AS TodayRptEmpRank,'' AS TodayRptEmpList, TodayOrgEmpCnt, TodayOrgEmpRank, TodayOrgEmpList
					FROM
					(
						SELECT  TodayOrgEmpRank,  SUM(TodayOrgEmpCnt) AS TodayOrgEmpCnt,'' AS TodayOrgEmpList
						FROM
						(
							SELECT  TodayOrgEmpRank, COUNT(empid) AS TodayOrgEmpCnt
							FROM
							(
								SELECT  dbo.get_grade_value(dbo.GETSCoreRank(v.Score)) AS TodayOrgEmpRank, 
														empid  --, COUNT(empid) OVER (Partition BY monyrid) AS TodayRptEmpCnt
									FROM 
									(
										 SELECT  k.Score, k.empid, k.empNAme FROM #tmpViewEmpScore k where k.companyid = @companyId
											and assessmentid = (SELECT  MAX(assessmentId) AS Id FROM #tmpViewEmpScore s where k.empid = s.empid
											--AND DATEPART(yyyy,Ratingdate) = @YearID
											)
									) v
									
							) g GROUP BY TodayOrgEmpRank
						) v1 GROUP BY TodayOrgEmpRank
					) k 
				) v
				GROUP BY  DrctScoreRank,--DirectRptAvgScoreRank,
				DrctmonYrId,OrgRank,-- OrgRptAvgScoreRank,
				OrgMonYrId,DrctmonYr,OrgMonYr,TodayRptEmpCnt, TodayRptEmpRank, TodayOrgEmpCnt, TodayOrgEmpRank,
				DrctAvgScore, AvgDrctRank, DrctAvgMonYrId, DrctAvgMonYr, OrgAvgScore, AvgOrgRank, OrgAvgMonYrId, OrgAvgMonYr,TodayRptEmpList,TodayOrgEmpList
		) t
	) f
END
END


----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GETCompanyAndYearWiseDepartment]
(
	@CompanyId INT,
	@YearId INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT Companyid, mstDepartmentId, department, departmentid, isSelected, YearId
FROM
(
	SELECT ISNULL(d.companyId,0 ) AS Companyid, d.departmentid AS mstDepartmentId, d.department, ISNULL(d.departmentId,0) AS departmentid,
	CAST(d.bactive AS INT) AS isSelected, FORMAT(d.createdDtstamp,'yyyy') AS YearId
	FROM [dbo].department d
	WHERE d.bactive = 1 
) v WHERE YearId <= CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END --@YearId
ORDER BY yearID, department
END
----------
CREATE OR ALTER   PROCEDURE [dbo].[usp_GetYearWiseAllEmployees]
(
	@YearID INT,
	@companyid INT,
	@managerid INT,
	@PageNumber INT,
	@PageSize   INT,
	@SearchString NVARCHAR(256),
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
DECLARE @TotalRowCount INT
SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

CREATE TABLE #tempEmpAverageAssessmentDet (
	companyid INT, empid INT, avgScore DECIMAL(2,0), AvgScoreRank VARCHAR(100), yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	SELECT * INTO #tempScore
	FROM
	(
		SELECT companyID, empid, Score, ManagerId,  FORMAT(RatingDate, 'yyyyMM') AS monyrID
		FROM vw_EmpAssessmentScore
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear  AND empStatus = 1 
	)t

	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT  companyID, empId, avgscore, avgScoreRank, 0 AS YearID
		FROM
		(
			SELECT companyID, empId, CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS DECIMAL(10,0)) AS avgscore,
			dbo.GetScoreRank(CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS decimal(10,0))) AS AvgScoreRank,
			row_number() OVER (PARTITION BY empid ORDER BY empid) AS rownumber
			FROM #tempScore 
		)v WHERE rownumber = 1
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT companyID , empID, ISNULL(AvgScore,0) AS avgScore, ISNULL(AvgScoreRank, '') AS AvgScoreRank, year AS YearID FROM vw_EmpAverageAssessmentDet 
		WHERE companyid = @companyId AND year = @YearId
	)k
	END

CREATE TABLE #tempEmpLastAssessmentDet (
	companyid INT, empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		--WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END

SET @YearID = CASE WHEN @YearID = 0 THEN YEAR(GETDATE()) ELSE @YearID END
IF(@managerid > 0 and @companyid > 0) 
BEGIN  
	
	SELECT empid, employeeId, companyid, firstname, middlename,  lastname,  email, jobtitle, joiningdate,
		   departmentid, managerid, managerFName, managerLName, empstatus, roleid, currentsalary,empimgpath
	INTO #tempEmployeedetailsMC 
	FROM
	(
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
			   e.departmentid, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath
		FROM EmployeeDetails e
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
		WHERE e.companyid = @companyid AND e.bactive = 1 AND e.managerid = @managerid  AND Format(e.joiningDate, 'yyyy') <= @YearID
		AND e.empstatus = 1 AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
	) v

IF([dbo].[GetRoleByEmpid](@managerid) IN (2,4))
BEGIN
	SELECT empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
		   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary,empimgpath,
		   lastAssessedDate, AvgScoreRank, AvgScore, lastScoreRank, RatingCompleted, ord,managerLastAssessedDate
	INTO #tempEmployeeResultMCE
	FROM
	(		
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
			   e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, e.managerFName, e.managerLName, e.empstatus, e.roleid, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore, 0) AS AvgScore,
		       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted,
			   0 AS ord ,ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM #tempEmployeedetailsMC e 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid
		LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
	)MCE
		SELECT @TotalRowCount = COUNT(empid) FROM #tempEmployeeResultMCE
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount,empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
			   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary,empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   lastScoreRank, RatingCompleted,managerLastAssessedDate
		FROM #tempEmployeeResultMCE
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY firstname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY 
	END
	ELSE
	BEGIN

	
	SELECT * INTO #tempEmployeeResultMC
	FROM(
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
			   e.departmentid, d.department,  ISNULL(e.managerid,0) AS managerid, e.managerFName, e.managerLName, e.empstatus, e.roleid, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore, 0) AS AvgScore,
		       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted,
			   0 AS ord,ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM #tempEmployeedetailsMC e 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid --AND vg.year = DATEPART(yyyy,GETDATE())
		LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
		WHERE e.empstatus = 1
	)MC
		SELECT @TotalRowCount = COUNT(empid) FROM  #tempEmployeeResultMC
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount,empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
			   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary,empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   lastScoreRank, RatingCompleted,managerLastAssessedDate
		FROM  #tempEmployeeResultMC
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY firstname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY 
	END
END


ELSE if (@managerid = 0 and @companyid > 0) 
BEGIN

	SELECT * INTO #tempEmployeedetailsC 
		FROM
		(
			SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
			   e.departmentid, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath
			FROM employeedetails e 
			LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
			WHERE e.companyid = @companyid AND e.bactive = 1 AND e.roleid <> 4 AND e.empstatus = 1 AND Format(e.joiningDate, 'yyyy') <= @YearID
			AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
		) v

	SELECT * INTO #tempEmployeeResultC
	FROM(
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
			   e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, e.managerFName, e.managerLName, e.empstatus, e.roleid, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
		       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted,
			   0 AS ord
		FROM #tempEmployeedetailsC e
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid 
		
	)C
		SELECT @TotalRowCount = COUNT(empid) FROM  #tempEmployeeResultC
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount,empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
			   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary,empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   lastScoreRank, RatingCompleted
		FROM  #tempEmployeeResultC
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY empid DESC, firstname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY 
END
END
----------
CREATE OR ALTER   PROCEDURE [dbo].[GetYearWiseAllEmpDetByManager] 
(
	@YearID INT,
	@companyid INT,
	@managerid INT,
	@PageNumber INT,
	@PageSize   INT,
	@SearchString NVARCHAR(256),
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d;
	
	DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

	CREATE TABLE #tempEmpAverageAssessmentDet (
	companyid INT, empid INT, avgScore DECIMAL(2,0), AvgScoreRank VARCHAR(100), yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	SELECT * INTO #tempScore
	FROM
	(
		SELECT companyID, empid, Score, ManagerId,  FORMAT(RatingDate, 'yyyyMM') AS monyrID
		FROM vw_EmpAssessmentScore
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear AND empStatus = 1 
	)t

	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT  companyID, empId, avgscore, avgScoreRank, 0 AS YearID
		FROM
		(
			SELECT companyID, empId, CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS DECIMAL(10,0)) AS avgscore,
			dbo.GetScoreRank(CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS decimal(10,0))) AS AvgScoreRank,
			row_number() OVER (PARTITION BY empid ORDER BY empid) AS rownumber
			FROM #tempScore 
		)v WHERE rownumber = 1
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT companyID , empID, avgScore, AvgScoreRank, year AS YearID FROM vw_EmpAverageAssessmentDet 
		WHERE companyid = @companyId AND year = @YearId
	)k
	END

	CREATE TABLE #tempEmpLastAssessmentDet (
	companyid INT, empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		--WHERE FORMAT(RatingDate, 'yyyyMM') >= @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END

	SET @YearID = CASE WHEN @YearID = 0 THEN YEAR(GETDATE()) ELSE @YearID END;

	WITH #tempEmployeedetails  (empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
		 departmentid, managerid,  empstatus,  roleid, currentsalary)
	AS
	(
	SELECT empid,  employeeId,  companyid,  firstname, ISNULL(middlename, '') AS middlename,  lastname,  email, ISNULL( jobtitle,'') AS jobtitle, CONVERT(DATE,  joiningdate,101) AS joiningdate,
			    departmentid, ISNULL(managerid,0) AS managerid,  empstatus,  roleid, currentsalary
	FROM employeedetails AS FirstLevel
	WHERE bactive = 1 AND empstatus = 1 AND managerID = @managerid 
	AND companyId = @companyId 
	UNION ALL
	SELECT NextLevel.empid, NextLevel.employeeId, NextLevel.companyid, NextLevel.firstname, ISNULL(NextLevel.middlename, '') AS middlename, NextLevel.lastname, NextLevel.email, ISNULL(NextLevel.jobtitle,'') AS jobtitle, CONVERT(DATE, NextLevel.joiningdate,101) AS joiningdate,
			   NextLevel.departmentid, ISNULL(NextLevel.managerid,0) AS managerid, NextLevel.empstatus, NextLevel.roleid, NextLevel.currentsalary
	FROM employeedetails AS NextLevel
	INNER JOIN #tempEmployeedetails AS t ON NextLevel.managerid = t.empId
	WHERE NextLevel.bactive = 1 AND NextLevel.empstatus = 1 
		AND NextLevel.companyid = @companyid 
	)SELECT * INTO #tempEmployeedetails
	FROM #tempEmployeedetails WHERE Format(joiningDate, 'yyyy') <= @YearID
  AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR departmentId IN (SELECT department FROM #departmentlist))
  	

DECLARE @TotalRowCount INT
IF(@managerid > 0 and @companyid > 0) 
BEGIN  	

	SELECT * INTO #tempEmployeeResult
	FROM(
	SELECT e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
		   e.departmentid, e.managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, d.department, e.currentsalary,
		   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
		   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
		   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
		   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted,
		  0 AS ord,ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
	FROM #tempEmployeedetails e
	LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
	LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
	LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid 
	LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
	--WHERE (e.managerId = @managerId --OR n.managerid = @managerId
	--OR (',' + RTRIM(e.inDirectManager) + ',') LIKE '%,' + CAST(@managerId AS VARCHAR(1000)) + ',%' )
	--AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))	
	)s
	
	SELECT @TotalRowCount = COUNT(empid) FROM #tempEmployeeResult
	WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
	OR employeeId like '%'+ @SearchString + '%'
	OR lastname like '%'+ @SearchString + '%'
	OR jobtitle like '%'+ @SearchString + '%'
	OR department like '%'+ @SearchString + '%'
	OR lastAssessedDate like '%'+ @SearchString + '%'
	OR lastScoreRank like '%'+ @SearchString + '%'
	OR RatingCompleted like '%'+ @SearchString + '%'
	OR firstname like '%'+ @SearchString + '%')

	SELECT @TotalRowCount AS totalrowcount,empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
		   departmentid, managerid, managerFName, managerLName, empstatus, roleid, department, currentsalary,
		   lastAssessedDate, AvgScoreRank, AvgScore,lastScoreRank, RatingCompleted,managerLastAssessedDate
	FROM #tempEmployeeResult
	WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
	OR employeeId like '%'+ @SearchString + '%'
	OR lastname like '%'+ @SearchString + '%'
	OR jobtitle like '%'+ @SearchString + '%'
	OR department like '%'+ @SearchString + '%'
	OR lastAssessedDate like '%'+ @SearchString + '%'
	OR lastScoreRank like '%'+ @SearchString + '%'
	OR RatingCompleted like '%'+ @SearchString + '%'
	OR firstname like '%'+ @SearchString + '%')
	ORDER BY firstname
	OFFSET @PageSize * (@PageNumber - 1) ROWS 
    FETCH NEXT @PageSize ROWS ONLY 
END
END



----------
CREATE OR ALTER   PROCEDURE [dbo].[usp_GetYearlyMonAvgScoreWiseEmp]  
(
	@YearId INT,
	@CompanyId INT,
	@ManagerId INT,
	@Grade VARCHAR(10),
	@Month VARCHAR(10),
	@PageNumber INT,
	@PageSize   INT,
	@SearchString NVARCHAR(256),
	@departmentlist VARCHAR(256)
)
AS 
BEGIN
DECLARE @TotalRowCount INT
SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

DECLARE @CurrYear INT = FORMAT(GETDATE(), 'yyyy')
DECLARE @LastYear INT = FORMAT(GETDATE(), 'yyyy') - 1
DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

CREATE TABLE #tempEmpAssessmentScore (
	companyid INT, empid INT, RatingDate DATETIME, Score DECIMAL(2,0), managerId INT, yearId INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyid, empid,  RatingDate, score, managerid, YearID FROM vw_EmpAssessmentScore 
		WHERE  empStatus = 1 AND FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear  AND companyid = @companyId
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyid, empid, RatingDate, score, managerid, YearID FROM vw_EmpAssessmentScore 
		WHERE  empStatus = 1 AND yearid = @YearId  AND companyid = @companyId
	)k
	END

	CREATE TABLE #tempEmpLastAssessmentDet (
	companyid INT, empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END

SELECT avgscore, ScoreRank, monyrid, monyr,empid,managerid, companyid INTO #temp FROM
(
	SELECT avgscore, dbo.get_grade_value(dbo.GETSCoreRank(avgscore)) AS ScoreRank, monyrid, monyr,empid,managerid, companyid
	FROM (
			SELECT DISTINCT CAST(ROUND(AVG(score) OVER (PARTITION BY empid, FORMAT(RatingDate,'yyyyMM')),0) AS DECIMAL(10,0)) AS avgscore,  empid,  
				   FORMAT(RatingDate,'yyyyMM') AS monyrid, FORMAT(RatingDate,'MMM') AS monyr, managerid, companyid
			FROM #tempEmpAssessmentScore 
	) v
) m


	CREATE TABLE #monthlist (yearmonth VARCHAR(20), monthname VARCHAR(20));

	IF(@YearId = 0)
	BEGIN
			INSERT INTO #monthlist
			SELECT yearmonth, monthname FROM
			(
				SELECT Format(getdate(),'yyyy')+FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MM') AS yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number between 1 and MONTH(GETDATE())
				UNION		
				SELECT  CAST(@LastYear AS VARCHAR) + FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@LastYear AS VARCHAR) AS DATE), 'MM') AS yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number > MOnth(DATEADD(month,-12,GETDATE()))
			) v
		END
		ELSE
		BEGIN
			INSERT INTO #monthlist
			SELECT  yearmonth, monthname  
			FROM
			(
				SELECT CAST(@yearID AS VARCHAR) + FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@yearID AS VARCHAR) AS DATE), 'MM') AS yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@yearID AS VARCHAR) AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number between 1 and CASE WHEN @CurrYear <> @YearId THEN MONTH(DATEFROMPARTS(@yearID,12,31)) ELSE MONTH(GETDATE()) END
			)v
		END

	SELECT companyid, avgscore, ScoreRank, yearmonth AS monyrid, monthname  AS monyr, empid, managerid 
	INTO #tempScore 
	FROM #monthlist m 
	LEFT OUTER JOIN #temp s ON s.monyrid = (SELECT MAX(monyrid) FROM #temp WHERE monyrid < yearmonth AND companyid = s.companyid AND empid = s.empid)
	WHERE yearmonth NOT IN(SELECT monyrid FROM #temp WHERE monyrid <> '' AND companyid = s.companyid AND empid = s.empid) AND companyid = s.companyid AND empid = s.empid
	UNION
	SELECT companyid, avgscore, ScoreRank, monyrid, monyr, empid, managerid from #temp WHERE monyrid <> '' 
	
SET @YearId = CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END
IF(@ManagerId > 0 AND @CompanyId > 0) 
BEGIN
	SELECT empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
		   departmentid, managerid, managerFName, managerLName, empstatus,  roleid, currentsalary, empimgpath
	INTO #tempEmployeedetailsMC
	FROM
	(
		SELECT e.empid,  e.employeeId,  e.companyid,  e.firstname, ISNULL(e.middlename, '') AS middlename,  e.lastname,  e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE,  e.joiningdate,101) AS joiningdate,
			   e.departmentid, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus,  
			   e.roleid, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath
		FROM employeedetails e 
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
		WHERE e.companyid = @companyid AND e.bactive = 1 AND e.managerid = @managerid
		AND Format(e.joiningDate, 'yyyy') <= @YearId
		AND e.empstatus = 1 AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
	) v

	SELECT * INTO #tempEmployeeResult
	FROM(
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
		       e.departmentid, e.managerid, e.managerFName, e.managerLName, e.empstatus, e.roleid, d.department, e.currentsalary,e.empimgpath,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.ScoreRank, '') AS AvgScoreRank, ISNULL(vg.avgscore,0) AS AvgScore,
			   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearId) AS VARCHAR),'') AS RatingCompleted
			   ,0 AS ord ,ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM #tempEmployeedetailsMC e 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempScore vg ON vg.empId = e.empid AND e.empstatus = 1 AND vg.monyr = @Month
		LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
		WHERE CASE WHEN ScoreRank IS NULL THEN '' ELSE ScoreRank END = @Grade
	)s
	
	SELECT @TotalRowCount = COUNT(empid) FROM #tempEmployeeResult
	WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

	SELECT @TotalRowCount AS totalrowcount,empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
		   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary, empimgpath,
		   lastAssessedDate, AvgScoreRank, AvgScore,
		   LastScoreRank, RatingCompleted ,managerLastAssessedDate
	FROM #tempEmployeeResult
	WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
	ORDER BY firstname
	OFFSET @PageSize * (@PageNumber - 1) ROWS 
	FETCH NEXT @PageSize ROWS ONLY
END
ELSE -- IF (@ManagerId = 0 AND CompanyId > 0)
	BEGIN
	--SELECT TOP 1 @managerId = empid FROM employeedetails WHERE companyid =@companyid AND bactive = 1 AND empstatus = 1 AND roleid =2
	SELECT empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
		  departmentid, managerid, managerFName, managerLName, empstatus,  roleid, currentsalary,empimgpath
	INTO #tempEmployeedetails
	FROM
	(
		SELECT e.empid,  e.employeeId,  e.companyid,  e.firstname, ISNULL(e.middlename, '') AS middlename,  e.lastname,  e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE,  e.joiningdate,101) AS joiningdate,
			   e.departmentid, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, 
			   ISNULL(m.lastname, '') AS managerLName, e.empstatus,  e.roleid, e.currentsalary ,ISNULL(e.empimgpath,'') AS empimgpath
		FROM employeedetails e 
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
		WHERE e.companyid = @companyid AND e.bactive = 1 AND e.empstatus = 1
		AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
	) v
	SELECT * INTO #tempEmployeeResultC
	FROM(
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
			   e.departmentid, d.department, e.managerid, e.managerFName, e.managerLName, e.empstatus, e.roleid, e.currentsalary,e.empimgpath,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.ScoreRank, '') AS AvgScoreRank, ISNULL(vg.avgscore,0) AS AvgScore,
			   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearId) AS VARCHAR),'') AS RatingCompleted
			   ,0 AS ord
		FROM #tempEmployeedetails e 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempScore vg ON vg.empId = e.empid AND e.empstatus = 1 AND vg.monyr = @Month
		WHERE CASE WHEN ScoreRank IS NULL THEN '' ELSE ScoreRank END = @Grade
	)s
	SELECT @TotalRowCount = COUNT(empid) FROM #tempEmployeeResultC
	WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

	SELECT @TotalRowCount AS totalrowcount,empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
		   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary,empimgpath,
		   lastAssessedDate, AvgScoreRank, AvgScore,
		   LastScoreRank, RatingCompleted
	FROM #tempEmployeeResultC	
	WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
	ORDER BY firstname
	OFFSET @PageSize * (@PageNumber - 1) ROWS 
	FETCH NEXT @PageSize ROWS ONLY
END
END

----------

CREATE OR ALTER   PROCEDURE [dbo].[usp_GetYearlyMonAvgScoreWiseEmpByManager]  
(
	@YearId INT,
	@CompanyId INT,
	@ManagerId INT,
	@Grade VARCHAR(10),
	@Month VARCHAR(10),
	@PageNumber INT,
	@PageSize   INT,
	@SearchString NVARCHAR(256),
	@departmentlist VARCHAR(256)
)
AS 
BEGIN

SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d;

DECLARE @CurrYear INT = FORMAT(GETDATE(), 'yyyy')
DECLARE @LastYear INT = FORMAT(GETDATE(), 'yyyy') - 1
DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

CREATE TABLE #tempEmpAssessmentScore (
	companyid INT, empid INT, RatingDate DATETIME, Score DECIMAL(2,0), managerId INT, yearId INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyid, empid, RatingDate, score, managerid, YearID  FROM vw_EmpAssessmentScore 
		WHERE  empStatus = 1 AND FORMAT(RatingDate, 'yyyyMM') >= @Last12MonthYear  AND companyid = @companyId
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyid, empid, RatingDate, score, managerid, YearID  FROM vw_EmpAssessmentScore 
		WHERE  empStatus = 1 AND yearid = @YearId  AND companyid = @companyId
	)k
	END

	CREATE TABLE #tempEmpLastAssessmentDet (
	companyid INT, empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), GeneralScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);


	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, GeneralScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		WHERE FORMAT(RatingDate, 'yyyyMM') >= @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, GeneralScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END


SELECT * INTO #temp FROM
(
	SELECT avgscore, dbo.get_grade_value(dbo.GETSCoreRank(avgscore)) AS ScoreRank, monyrid, monyr,empid,managerid, companyid
	FROM (
			SELECT distinct CAST(ROUND(AVG(score) OVER (Partition BY empid, FORMAT(RatingDate,'yyyyMM')),0) AS decimal(10,0)) as avgscore,  empid,  
				 FORMAT(RatingDate,'yyyyMM') as monyrid, FORMAT(RatingDate,'MMM') as monyr, managerid, companyid
				 FROM #tempEmpAssessmentScore 
	) v
) m

CREATE TABLE #monthlist (yearmonth VARCHAR(20), monthname VARCHAR(20));

	IF(@YearId = 0)
	BEGIN
			INSERT INTO #monthlist
			SELECT yearmonth, monthname FROM
			(
				SELECT Format(getdate(),'yyyy')+FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MM')yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number between 1 and MONTH(GETDATE())
				UNION		
				SELECT  CAST(@LastYear AS VARCHAR) + FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@LastYear AS VARCHAR) AS DATE), 'MM')yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number > MOnth(DATEADD(month,-12,GETDATE()))
			) v
		END
		ELSE
		BEGIN
			INSERT INTO #monthlist
			SELECT  yearmonth, monthname  
			FROM
			(
				SELECT CAST(@yearID AS VARCHAR) + FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@yearID AS VARCHAR) AS DATE), 'MM')AS yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@yearID AS VARCHAR) AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number between 1 and CASE WHEN @CurrYear <> @YearId THEN MONTH(DATEFROMPARTS(@yearID,12,31)) ELSE MONTH(GETDATE()) END
			)v
		END

	SELECT companyid, avgscore, ScoreRank, yearmonth AS monyrid, monthname  AS monyr, empid, managerid INTO #tempScore
	FROM #monthlist m LEFT OUTER JOIN #temp s on s.monyrid = (SELECT MAX(monyrid) FROM #temp WHERE monyrid < yearmonth and companyid = s.companyid and empid = s.empid)
	WHERE yearmonth NOT IN(SELECT monyrid FROM #temp WHERE monyrid <> '' and companyid = s.companyid and empid = s.empid) and companyid = s.companyid and empid = s.empid
	UNION
	SELECT companyid, avgscore, ScoreRank, monyrid, monyr, empid, managerid from #temp WHERE monyrid <> '' 
	
	SET @YearId = CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END

IF(@ManagerId > 0 AND @CompanyId > 0) 
BEGIN
	WITH #tempEmployeedetailsMC  (empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
		  departmentid, managerid,  empstatus,  roleid, currentsalary,empimgpath)
	AS
	(
	SELECT empid,  employeeId,  companyid,  firstname, ISNULL(middlename, '') AS middlename,  lastname,  email, ISNULL( jobtitle,'') AS jobtitle, CONVERT(DATE,  joiningdate,101) AS joiningdate,
		  departmentid, ISNULL(managerid,0) AS managerid,  empstatus,  roleid, currentsalary,ISNULL(FIrstLevel.empimgpath,'') AS empimgpath
	FROM employeedetails AS FirstLevel
	WHERE bactive = 1 AND empstatus = 1 AND managerID = @ManagerId 
	AND companyId = @companyId 
	UNION ALL
	SELECT NextLevel.empid, NextLevel.employeeId, NextLevel.companyid, NextLevel.firstname, ISNULL(NextLevel.middlename, '') AS middlename, NextLevel.lastname, NextLevel.email, ISNULL(NextLevel.jobtitle,'') AS jobtitle, CONVERT(DATE, NextLevel.joiningdate,101) AS joiningdate,
		   NextLevel.departmentid, ISNULL(NextLevel.managerid,0) AS managerid, NextLevel.empstatus, NextLevel.roleid, NextLevel.currentsalary,ISNULL(NextLevel.empimgpath,'') AS empimgpath
	FROM employeedetails AS NextLevel
	INNER JOIN #tempEmployeedetailsMC AS t ON NextLevel.managerid = t.empId 
    WHERE NextLevel.bactive = 1 AND NextLevel.empstatus = 1 
		AND NextLevel.companyid = @companyid 
	)SELECT * INTO #tempEmployeedetailsMC 
 FROM #tempEmployeedetailsMC WHERE Format(joiningDate, 'yyyy') <= @YearID
  AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR departmentId IN (SELECT department FROM #departmentlist))


	SELECT * INTO #tempEmployeeResult
	FROM(
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
			   e.departmentid, d.department, e.managerid , ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, e.currentsalary,e.empimgpath,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.ScoreRank, '') AS AvgScoreRank, ISNULL(vg.avgscore,0) AS AvgScore,
			   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted
			   ,0 AS ord,ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM #tempEmployeedetailsMC e 
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempScore vg ON vg.empId = e.empid AND e.empstatus = 1 AND vg.monyr = @Month
		LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
		WHERE CASE WHEN ScoreRank IS NULL THEN '' ELSE ScoreRank END = @Grade 
	)v
		DECLARE @TotalRowCount INT
		SELECT @TotalRowCount = COUNT(empid) FROM #tempEmployeeResult
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount,empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
			   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary,empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   LastScoreRank, RatingCompleted,managerLastAssessedDate
		FROM #tempEmployeeResult
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY firstname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY
END
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_AddDeviceInfo]
 -- Add the parameters for the stored procedure here
 (
 @UserId INT,
 @DeviceType VARCHAR(50),
 @DeviceId VARCHAR(MAX),
 @Result int out
 )
AS
BEGIN
	INSERT INTO DeviceRegistration
	 (
	 UserId,
	 DeviceType,
	 DeviceId,
	 CreatedBy,
	 CreatedDtStamp
	 )
	VALUES
	 (
	 @UserId,
	 @DeviceType,
	 @DeviceId,
	 @UserId,
	 getdate()
	 )

 set @Result = 1
 select @Result
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_AddEmployeeDetails_New] 
(
		@employeeId VARCHAR(100),
		@companyid INT,
        @firstName NVARCHAR(100),
        @middleName NVARCHAR(100),
        @lastName NVARCHAR(100),
		@suffix VARCHAR(8),
        @email VARCHAR(100),
        @jobTitle VARCHAR(100),
        @joiningDate DATE,
        @workCity NVARCHAR(100),
        @workState NVARCHAR(100),
        @workZipcode VARCHAR(25),
        @departmentId SMALLINT,
        @managerId INT,
        @managerFName NVARCHAR(200),
		@managerLName NVARCHAR(200),
        @empStatus BIT,
        @roleId TINYINT,
        @dateOfBirth DATE,
        @raceorethanicityId SMALLINT,
        @jobGroupId SMALLINT,
		@jobGroup NVARCHAR(100),
        @gender NVARCHAR(50),
        @jobCategoryId SMALLINT,
		@jobCategory NVARCHAR(100),
        @empLocation NVARCHAR(100),
        @jobCodeId SMALLINT,
		@jobCode NVARCHAR(100),
        @lastPromodate DATE,
        @currentSalary DECIMAL(12,2),
        @lastIncDate DATE,
        @countryId INT,
        @regionId INT,
        @empImgPath VARCHAR(100),
		@phoneNumber VARCHAR(25),
        @createdBy INT,
		@resultid INT OUTPUT
) 
AS
BEGIN
IF([dbo].[GetRole](@createdby) = 2 or [dbo].[GetRole](@createdby) = 1 OR @createdby=@createdby)
BEGIN

	IF EXISTS(SELECT * FROM employeedetails WHERE companyid = @companyid AND employeeid = @employeeId AND @employeeId <> '' )
	BEGIN
		SET @resultid = -2;
		SELECT @resultid; ---Employee id already Exists 
	END
	ELSE IF EXISTS(SELECT * FROM employeedetails WHERE @email <> '' AND email = @email AND bactive = 1)  --AND companyid = @companyid
	BEGIN
		SET @resultid = 0 -- For email already exists
		SELECT @resultid
	END
	ELSE IF (@phoneNumber <> '' AND EXISTS(SELECT * FROM employeedetails WHERE @phoneNumber <> '' AND phonenumber = @phoneNumber) )   
	BEGIN
		SET @resultid = -3 -- For phone number already exists
		SELECT @resultid
	END
	--newly Added
	ELSE IF EXISTS(SELECT * FROM employeedetails WHERE @email <> '' AND companyid = @companyid AND email = @email AND bactive = 0)
	BEGIN
		UPDATE employeedetails SET bactive =1 WHERE email = @email

		SELECT @resultid  = empid FROM employeedetails WHERE email = @email AND bactive = 1
		SELECT @resultid;
	END
	---Newly added for employeeID
	ELSE IF EXISTS(SELECT * FROM [dbo].[employeedetails] WHERE @employeeId <> '' AND companyid = @companyid AND employeeId = @employeeId AND bactive = 0)
		BEGIN
			UPDATE employeedetails SET bactive =1
						WHERE employeeId = @employeeId


			SELECT @resultid  = empid FROM employeedetails WHERE employeeId = @employeeId AND bactive = 1
			SELECT @resultid;
		END

	---
	ELSE IF NOT EXISTS(SELECT * FROM [dbo].[employeedetails] WHERE (email = @email AND employeeId = @employeeId))
	BEGIN
		INSERT INTO employeedetails  
		(
		employeeId,
		companyid,
		firstName,
		middleName,
		lastName,
		suffix,
		email,
		jobTitle,
		joiningDate,
		workCity,
		workState,
		workZipcode,
		departmentId,
		managerId,
		managername,
		managerlname,
		empStatus,
		roleId,
		dateOfBirth,
		raceorethanicityId,
		jobGroupid,
		jobGroup,
		gender,
		jobCategoryId,
		jobCategory,
		empLocation,
		jobCodeid,
		jobCode,
		lastPromodate,
		currentSalary,
		lastIncDate,
		countryid,
		regionId,
		empImgPath,
		phoneNumber,
		bactive,
		createdBy,
		createdDtstamp
		)
		VALUES 
		(
		@employeeId,
		@companyid,
		@firstName,
		@middleName,
		@lastName,
		@suffix, 
		@email,
		@jobTitle,
		@joiningDate,
		@workCity,
		@workState,
		@workZipcode,
		@departmentId,
		@managerId,
		@managerFName,
		@managerLName,
		@empStatus,
		@roleId,
		@dateOfBirth,
		@raceorethanicityId,
		@jobGroupId,
		@jobGroup,
		@gender,
		@jobCategoryId,
		@jobCategory,
		@empLocation,
		@jobCodeId,
		@jobCode,
		@lastPromodate,
		@currentSalary,
		@lastIncDate,
		@countryId,
		@regionId,
		@empImgPath,
		@phoneNumber,
		1,
		@createdBy,
		GETDATE()
        
		);

		SET @resultid =SCOPE_IDENTITY()
		IF EXISTS(SELECT * FROM employeedetails WHERE companyID = @companyID AND employeeID = CAST(@resultID AS VARCHAR(25)))
		BEGIN  
			UPDATE employeedetails SET employeeId = CASE WHEN @employeeId = '' THEN LEFT(NEWID(),5) + CAST(@resultId AS VARCHAR(100)) ELSE @employeeId END WHERE empid = @resultid
		END
		ELSE
		BEGIN
			UPDATE employeedetails SET employeeId = CASE WHEN @employeeId = '' THEN CAST(@resultId AS VARCHAR(100)) ELSE @employeeId END WHERE empid = @resultid
		END	
		SELECT @resultid
	END
	--ELSE 
	--BEGIN
	--IF EXISTS(SELECT * FROM [dbo].[employeedetails] WHERE (email = @email OR employeeId = @employeeId) AND bactive = 0)
	--BEGIN
	--	UPDATE employeedetails SET bactive =1
	--	WHERE (email = @email OR employeeId = @employeeId)
	
	--	SELECT @resultid  = empid FROM employeedetails WHERE (email = @email OR employeeId = @employeeId) AND bactive = 1
	--	SELECT @resultid;
	--	END
	--END
	END
	ELSE
	BEGIN
		SET @resultid = -1; -- For not authorized user
		SELECT @resultid 
	END
END
----------

CREATE OR ALTER PROCEDURE [dbo].[usp_AddEmpolyeeFromCSV_New] 
(
     @tblEmployee Type_employeedetails_1 READONLY,
      @result int output
)
AS
	
BEGIN

	MERGE INTO employeedetails e1 USING
	( 
			 	SELECT companyid, firstName,middleName,lastName,suffix, email, ISNULL(phonenumber, '') AS phonenumber, jobTitle, convert(date,joiningDate,110) AS joiningDate, 
				workCity,workState,workZipcode, 
				departmentId,managerId,managername,managerlname, empStatus,roleId,convert(date,dateOfBirth,110) AS dateOfBirth,raceorethanicityId,jobGroup,gender,jobCategory,
				empLocation,jobCode,convert(date,lastPromodate,110) AS lastPromodate ,currentSalary,
				CONVERT(date,lastIncDate,110) AS lastIncDate, countryid, regionId, empImgPath, bactive,createdBy,  updatedBy,  employeeid, CSVManagerId FROM @tblEmployee   
			 	) i ON (e1.companyid = i.companyid AND (e1.email = CASE WHEN i.email = '' THEN i.employeeId ELSE i.email END OR e1.employeeId = i.employeeId)) 
			 WHEN NOT MATCHED THEN INSERT 
			 (
				companyid, firstName,middleName, lastName, suffix, email, phonenumber, jobTitle, joiningDate,
				workCity, workState,workZipcode, departmentId, managerId, managername ,managerlname,
				empStatus, roleId,dateOfBirth,raceorethanicityId, jobGroup, gender, jobCategory,
				empLocation, jobCode, lastPromodate, currentSalary, lastIncDate,countryid,regionId,
				empImgPath, bactive, createdBy, createdDtstamp, updatedBy, updateddtstamp, employeeid,phoneconfirmed,optforsms
			) 
			 	VALUES (i.companyid, i.firstName, i.middleName, i.lastName, i.suffix, i.email, i.phonenumber, i.jobTitle, i.joiningDate,
					i.workCity, i.workState, i.workZipcode, i.departmentId, i.managerId, i.managername, i.managerlname,
					i.empStatus, i.roleId, i.dateOfBirth, i.raceorethanicityId, i.jobGroup, i.gender, i.jobCategory,
					i.empLocation, i.jobCode, i.lastPromodate, i.currentSalary,i.lastIncDate,i.countryid,i.regionId,
					i.empImgPath, 1, i.createdBy, GETDATE(), null, null, i.employeeid,0,0
				) 
				WHEN MATCHED THEN UPDATE SET e1.companyid = i.companyid, e1.firstName = i.firstName, e1.middleName = i.middleName, e1.lastName = i.lastName, 
					e1.suffix = i.suffix, e1.email = i.email, e1.phonenumber = i.phonenumber, e1.jobTitle = i.jobTitle, e1.joiningDate = i.joiningDate,
					e1.workCity = i.workCity, e1.workState = i.workState, e1.workZipcode = i.workZipcode, e1.departmentId = i.departmentId,
					e1.managerId = i.managerId, e1.managername = i.managername, e1.managerlname = i.managerlname,
					e1.empStatus = i.empStatus, e1.roleId = i.roleId, e1.dateOfBirth = i.dateOfBirth, e1.raceorethanicityId = i.raceorethanicityId,
					e1.jobGroup = i.jobGroup, e1.gender = i.gender, e1.jobCategory = i.jobCategory,
					e1.empLocation = i.empLocation, e1.jobCode = i.jobCode, e1.lastPromodate = i.lastPromodate,
					e1.currentSalary = i.currentSalary, e1.lastIncDate = i.lastIncDate, e1.countryid = i.countryid, e1.regionId = i.regionId,
					e1.empImgPath = i.empImgPath, e1.bactive = 1, e1.createdBy = i.createdBy, e1.createdDtstamp = GETDATE(), e1.updatedBy = i.updatedBy,
					e1.updateddtstamp = GETDATE(), ismailsent = (case when i.roleid =5 then 0 else ismailsent end), e1.employeeid = i.employeeid,
					e1.phoneconfirmed=(case when i.phonenumber <> e1.phonenumber then 0 else e1.phoneconfirmed end),
					e1.optforsms=(case when i.phonenumber <> e1.phonenumber then 0 else e1.phoneconfirmed end);


UPDATE employeedetails SET employeeid= CAST(CASE WHEN ISNULL(employeeid, '') = '' THEN CAST(empid AS VARCHAR) ELSE CAST(ISNULL(employeeid, empid) AS VARCHAR) END AS VARCHAR) 
WHERE CASE WHEN email = '' THEN employeeId ELSE email END in (SELECT CASE WHEN email = '' THEN employeeId ELSE email END FROM @tblEmployee)


MERGE INTO employeedetails e2 USING
(
	SELECT t.email, t.CSVManagerId, e.empId AS ManagerId,t.companyId,t.employeeId FROM @tblEmployee t
	INNER JOIN employeedetails e ON e.employeeId = t.CSVManagerId  AND t.companyId = e.companyId  
	WHERE t.CSVManagerId <> ''
) i ON (e2.companyid = i.companyid AND (e2.email = CASE WHEN i.email = '' THEN i.employeeId ELSE i.email END OR e2.employeeId = i.employeeId)) 
WHEN MATCHED THEN UPDATE SET e2.ManagerId = i.ManagerId ;
SET @result = 1
RETURN @result
END
----------

CREATE OR ALTER PROCEDURE [dbo].[usp_AddUserDetails]
(
	@empid INT, 
	@username VARCHAR(150),
	@password NVARCHAR(150),
	@bactive BIT,
	@createdby INT,
	@result INT OUT
)
	
AS
BEGIN
	
	
	SET NOCOUNT ON;

IF NOT EXISTS(SELECT * FROM UserLogin WHERE username = @username)
	BEGIN
		INSERT INTO UserLogin 
			(
				empid,
				username,
				password,
				bactive, 
				createdby,
				createddtstamp
			)
			VALUES
			(
				@empid,
				@username,
				@password,
				@bactive,
				@createdby,
				GETDATE()   
			)
		SET @result = 1
		SELECT @result 
	END
ELSE
IF EXISTS(SELECT * FROM UserLogin WHERE username = @username AND bactive = 0)
	BEGIN
		UPDATE UserLogin SET bactive = 1 WHERE username = @username
		SET @result  = 1 
		SELECT @result ;
	END
ELSE
BEGIN
	SET @result = 0;
	SELECT @result 
END
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_AddUserLoginFromCSV]
(
      @tbluserlogin Type_userLogin_1 READONLY
)
AS
BEGIN    
 
			 MERGE INTO userlogin e1 USING ( 
			 	SELECT username, [password], empid, bactive,createdBy, updatedby FROM @tbluserlogin
			 	) i ON (e1.empid = i.empid) 
			 WHEN NOT MATCHED THEN INSERT 
			 (
				username,[password],empid,bactive,createdBy,createdDtstamp, updatedby,updateddtstamp
			 ) 
			 	VALUES (i.username, i.[password], i.empid, i.bactive, i.createdBy, GETDATE(), NULL, NULL) 
			 WHEN MATCHED THEN UPDATE SET e1.username = i.username, e1.[password] = i.[password], e1.empid = i.empid, e1.bactive = i.bactive, 
				e1.createdBy = i.createdBy, e1.createdDtstamp = GETDATE(), e1.updatedby = i.updatedby, e1.updateddtstamp = GETDATE();
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_DeleteEmployee] 
(
	@companyid INT,
	@empid INT,
	@updatedby VARCHAR(50),
	@result INT OUT	
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
--IF([dbo].[GetRole](@updatedby) = 2 or [dbo].[GetRole](@updatedby) = 1)
BEGIN
    -- Insert statements for procedure here
	IF EXISTS (SELECT * FROM employeedetails WHERE companyid = @companyid AND empid = @empid AND bactive = 1)
	BEGIN
		UPDATE employeedetails SET bactive  = 0,
				updatedby = @updatedby,
				updateddtstamp = GETDATE()
		WHERE companyid = @companyid AND empid = @empid  
		SET @result = 1;  ----Sucessfully  Deleted
		SELECT @result;
	END
	ELSE
	BEGIN
		SET @result = 0;
		SELECT @result;  ---------Employee not exists
	END
END 
--ELSE 
--	SET @result = -1; ---------Not authorised to delete
--	SELECT @result;
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GetAllNotificationById]
(
	@managerId int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [ID],[EmpId]
	  ,[Message]
      ,[ManagerId]
      ,[Action]
      ,[IsSent]
      ,[MarkAs]
      ,[CreatedBy]
      ,[CreatedDtStamp],[Type] FROM [dbo].[NotificationsLogs] WHERE ManagerId = @managerId order by [ID] desc
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GetDeviceInfoById]
(
	@UserId int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [ID],[UserId],[DeviceType],[DeviceId],[CreatedBy],[CreatedDtStamp] FROM [dbo].[DeviceRegistration] WHERE UserId = @UserId ORDER BY [ID] DESC
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GetEmpAssessmentScore]
(
	@managerId int,
	@companyid int, 
	@departmentlist VARCHAR(1000)
)
As Begin

DECLARE @curryear int= DATEPART(yyyy,GETDATE());
SELECT department INTO #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d;

	SELECT 	e.CompanyId, e.Empid, dbo.[GetEmpNameById](null,e.firstname,e.middlename,e.lastname) AS empName, CAST(a.score as DECIMAL(2,0)) AS score, a.scoreid,
	    [dbo].[Get_grade_value]([dbo].[GetScoreRankById](a.scoreid)) AS ScoreRank, 
		 a.assessmentdate AS RatingDate, a.Assessmentby AS Assessmentbyid,
		dbo.[GetEmpNameById](u.empid,null,null,null) AS Assessmentby, a.id AS assessmentId, 
		e.managerId, e.departmentid, e.roleid, e.empstatus, CAST(0 as int) AS inDirectManager,
		datepart(yyyy,a.assessmentdate) AS yearId
	FROM employee_assessment  a
	LEFT JOIN EmployeeDetails e ON a.empid= e.empid
	INNER JOIN userlogin u ON a.Assessmentby = u.userid
	WHERE e.companyid=@companyId AND e.bactive = 1
		AND e.empStatus = 1 
		AND e.empid <> @ManagerId
		AND e.departmentId IN (SELECT department FROM #departmentlist) 
		AND datepart(yyyy,a.assessmentdate) = @curryear

END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GETEmployeeByCompanyId] 
(
	@CompId INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT  e.[empid] ,[companyid] ,[firstname],[middlename],[lastname],[suffix] ,[email]
      ,[jobtitle],[joiningdate],[workcity],[workstate] ,[workzipcode],[departmentid]
      ,[managerid] ,[managername],[managerlname],[empstatus],[roleid]
      ,[dateofbirth] ,[raceorethanicityid] ,[gender] ,[jobcategoryid],[jobcodeid]
      ,[jobgroupid],[lastpromodate],[currentsalary],[lastincdate],[emplocation]
      ,[countryid] ,[regionid],[empimgpath],e.[bactive],e.[createdby],e.[createddtstamp]
      ,e.[updatedby],e.[updateddtstamp] ,[EmployeeId],[jobcategory],[jobcode],[JobGroup]
	  , '' AS password
	  FROM employeedetails e 
	  INNER JOIN userlogin u on u.empid = e.empid 
	  where companyid = @CompId 
END

----------

CREATE  OR ALTER PROCEDURE [dbo].[usp_GetEmployeeByEmpId] 
(
	@empid INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT e.empid, e.employeeid, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.phonenumber,'') as phonenumber, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(datetime, e.joiningdate,101) AS joiningdate, ISNULL(e.suffix, '') AS suffix,
			   e.workcity, e.workstate, e.workzipcode, e.departmentid, '' AS department,  ISNULL(e.managerid,0) AS managerid , ISNULL(e.managername, '') AS managerfname, ISNULL(e.managerlname, '') AS managerlname, e.empstatus, e.roleid,
			   [dbo].[GetDimensionValueByID](1,e.roleid) AS role, e.raceorethanicityid, e.gender, ISNULL(e.jobcategoryid,0) AS jobcategoryid, ISNULL(e.jobcategory,'') AS jobcategory, 
			   ISNULL(e.jobgroupId,0) AS jobgroupId, ISNULL(e.jobgroup, '') AS jobgroup, ISNULL(e.jobcodeId,0) AS jobcodeId, ISNULL(e.jobcode,0) AS jobcode,
			   CASE WHEN e.lastpromodate IS NULL THEN '1900-01-01' ELSE e.lastpromodate END AS lastpromodate,
			   CASE WHEN e.lastincdate IS NULL THEN '1900-01-01' ELSE e.lastincdate END AS lastincdate ,
			   CASE WHEN e.dateofbirth IS NULL THEN '1900-01-01' ELSE e.dateofbirth END AS dateofbirth ,
			   ISNULL(e.raceorethanicityid, 0) AS raceorethanicityId, ISNULL(rc.raceorethnicity, '') AS raceorethanicity, e.currentsalary,
			   ISNULL(e.countryId, 0) AS countryId, ISNULL(c.country, '') AS country, e.emplocation, e.regionid, rg.region, ISNULL(e.empimgpath,'') AS empimgpath,comp.CompImgPath AS companyLogoPath,
			   ISNULL(e.IsMailSent, 0) AS IsMailSent
		FROM employeedetails e
		INNER JOIN companydetails comp ON comp.compid = e.companyid 
		LEFT JOIN regions rg ON rg.regionid = e.regionid AND rg.bactive = 1
		LEFT JOIN country c ON c.id = e.countryid AND c.bactive = 1
		LEFT JOIN raceorethnicity rc ON rc.id = e.raceorethanicityid AND rc.bactive = 1
		WHERE e.empid = @empid AND e.bactive = 1
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GetNotificationById]
(
	@managerId INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT [ID],[EmpId]
	  ,[Message]
      ,[ManagerId]
      ,[Action]
      ,[IsSent]
      ,[MarkAs]
      ,[CreatedBy]
      ,[CreatedDtStamp],[Type] FROM [dbo].[NotificationsLogs] WHERE ManagerId = @managerId AND [IsSent] = 0 order by [ID] desc
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GetYearlyAllEmployeesWithGrade]
(
	@YearID INT,
	@companyid INT,
	@managerid INT,
	@Grade VARCHAR(20),
	@PageNumber INT,
	@PageSize   INT,
	@SearchString NVARCHAR(256),
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	DECLARE @TotalRowCount INT
	SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

CREATE TABLE #tempEmpAverageAssessmentDet (
	companyid INT, empid INT, avgScore DECIMAL(2,0), AvgScoreRank VARCHAR(100), yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	SELECT * INTO #tempScore
	FROM
	(
		SELECT companyID, empid, Score, ManagerId,  FORMAT(RatingDate, 'yyyyMM') AS monyrID
		FROM vw_EmpAssessmentScore
		WHERE FORMAT(RatingDate, 'yyyyMM') > FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')  AND empStatus = 1 
	)t

	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT  companyID, empId, avgscore, avgScoreRank, 0 AS YearID
		FROM
		(
			SELECT companyID, empId, CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS DECIMAL(10,0)) AS avgscore,
			dbo.GetScoreRank(CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS decimal(10,0))) AS AvgScoreRank,
			row_number() OVER (PARTITION BY empid ORDER BY empid) AS rownumber
			FROM #tempScore 
		)v WHERE rownumber = 1
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT companyID , empID, avgScore, AvgScoreRank, year AS YearID FROM vw_EmpAverageAssessmentDet 
		WHERE companyid = @companyId AND year = @YearId
	)k
	END

	CREATE TABLE #tempEmpLastAssessmentDet (
	companyId INT,empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END
SET @YearId = CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END

IF(@managerid > 0 and @companyid > 0 AND @Grade <> '') 
BEGIN  

	SELECT empid,  employeeId,  companyid,  firstname, middlename, lastname,  email, jobtitle,
		   joiningdate, departmentid, managerid, managerFName, managerLName, empstatus,  roleId, currentsalary,empimgpath
	INTO #tempEmployeedetailsMC
	FROM
	(
		SELECT e.empid,  e.employeeId,  e.companyid,  e.firstname, ISNULL(e.middlename, '') AS middlename,  e.lastname,  e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE,  e.joiningdate,101) AS joiningdate,
			    e.departmentid, ISNULL(e.managerid,0) AS managerid,  e.empstatus,  e.roleid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath
		FROM employeedetails e 
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
		WHERE e.companyid = @companyid AND e.bactive = 1 AND e.managerid = @managerid AND Format(e.joiningDate, 'yyyy') <= @YearID
		AND e.empstatus = 1 AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
	) v

	SELECT * INTO #tempEmployeeResult
	FROM(
		SELECT empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
			   departmentid, managerid,  empstatus,  roleid, department, managerFName, managerLName, currentsalary,empimgpath, 
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   lastScoreRank, RatingCompleted
		FROM(
				SELECT e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
					   e.departmentid, d.department,  e.managerid , e.empstatus, e.roleid, managerFName, managerLName, e.currentsalary,e.empimgpath,
					   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
					   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
					   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
					   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted
					   ,0 AS ord
				FROM #tempEmployeedetailsMC e 
				LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
				LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid 
		) v
		WHERE CASE WHEN dbo.Get_grade_value(LastScoreRank) IS NULL THEN '' ELSE dbo.Get_grade_value(LastScoreRank) END = @Grade
	)t		
		
		SELECT @TotalRowCount = COUNT(empid) FROM #tempEmployeeResult
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount,empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
			   departmentid, department, managerid, empstatus, roleid, managerFName, managerLName,currentsalary,empimgpath, 
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   lastScoreRank, RatingCompleted
		FROM #tempEmployeeResult
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY firstname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY

END
ELSE if (@managerid = 0 and @companyid > 0 AND @Grade <> '') 
BEGIN

	SELECT empid,  employeeId,  companyid,  firstname, middlename, lastname,  email, jobtitle,
		   joiningdate, departmentid, managerid, managerFName, managerLName,  empstatus,  roleId, currentsalary,empimgpath
	INTO #tempEmployeedetailsC
	FROM
	(
		SELECT  e.empid,  e.employeeId,  e.companyid,  e.firstname, ISNULL(e.middlename, '') AS middlename,  e.lastname,  e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE,  e.joiningdate,101) AS joiningdate,
			    e.departmentid, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus,  e.roleid, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath 
		FROM employeedetails e 
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
		WHERE e.companyid = @companyid AND e.bactive = 1  AND Format(e.joiningDate, 'yyyy') <= @YearID
		AND e.empstatus = 1 AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
	) v

	SELECT empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
			   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary,empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   LastScoreRank, RatingCompleted
	INTO #tempEmployeeResultC
	FROM(
		SELECT empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
			   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary,empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   LastScoreRank, RatingCompleted
		FROM(
			SELECT e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
				   e.departmentid, d.department, e.managerid, managerFName, managerLName,  e.empstatus, e.roleid, e.currentsalary, ISNULL(e.empimgpath,'') AS empimgpath,
				   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
				   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
				   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
				   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted
				   ,0 AS ord
			FROM #tempEmployeedetailsC e 
			LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
			LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid 
			WHERE e.companyid = @companyid 
			GROUP BY e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
					 e.departmentid, d.department, e.managerid, e.managerFName, e.managerLName, e.empstatus, e.roleid,
					 ISNULL(vg.AvgScoreRank, ''),vg.AvgScore, e.currentsalary,ISNULL(e.empimgpath,'') 
					 
		) v
		WHERE CASE WHEN dbo.Get_grade_value(LastScoreRank) IS NULL THEN '' ELSE dbo.Get_grade_value(LastScoreRank) END = @Grade	
	)t

	SELECT @TotalRowCount = COUNT(empid) FROM #tempEmployeeResultC
	WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
		OR lastname LIKE '%'+ @SearchString + '%'
		OR firstname LIKE '%'+ @SearchString + '%'
		OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
	SELECT @TotalRowCount AS totalrowcount, empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
		departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary,empimgpath,
		lastAssessedDate, AvgScoreRank, AvgScore,
		LastScoreRank, RatingCompleted
	FROM #tempEmployeeResultC
	WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
		OR lastname LIKE '%'+ @SearchString + '%'
		OR firstname LIKE '%'+ @SearchString + '%'
		OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
	ORDER BY empid DESC, firstname
	OFFSET @PageSize * (@PageNumber - 1) ROWS 
	FETCH NEXT @PageSize ROWS ONLY
END
ELSE 
BEGIN

	SELECT * INTO #tempEmployeedetails
	FROM
	(
		SELECT e.empid,  e.employeeId,  e.companyid, c.companyname,  e.firstname, e.middlename, e.lastname,  e.email, e.jobtitle,
		   e.joiningdate, e.departmentid, managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus,  e.roleId, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath
		FROM employeedetails e 
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
		LEFT JOIN companydetails cd ON cd.compid = e.companyId AND cd.bactive = 1 WHERE e.bactive = 1 AND cd.bactive = 1 and e.roleid in (2)--,3,4,5,6) 
		AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
	) v

	SELECT @TotalRowCount = COUNT(e.empid) FROM #tempEmployeedetails e 
	LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
	WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
		OR e.lastname LIKE '%'+ @SearchString + '%'
		OR e.firstname LIKE '%'+ @SearchString + '%'
		OR e.firstname + ' ' + e.lastname LIKE '%'+ @SearchString + '%')

	SELECT @TotalRowCount as totalrowcount,e.empid, e.employeeId, e.companyid, cd.companyName, e.firstname, e.middlename, e.lastname, e.email, jobtitle, joiningdate,
		   e.departmentid, d.department, e.managerid, e.managerFName, e.managerLName, e.empstatus, e.roleid, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath,
		   '' AS lastAssessedDate, 0 as AvgScore, '' AS AvgScoreRank, '' AS LastScoreRank, '' AS RatingCompleted 
			,0 AS ord
	FROM #tempEmployeedetails e
	LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
	LEFT JOIN companydetails cd ON cd.compid = e.companyId AND cd.bactive = 1
	WHERE cd.bactive = 1 and e.roleid in (2)--,3,4,5,6) 
				AND (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
				OR e.lastname LIKE '%'+ @SearchString + '%'
				OR e.firstname LIKE '%'+ @SearchString + '%'
				OR e.firstname + ' ' + e.lastname LIKE '%'+ @SearchString + '%')
			ORDER BY e.empid DESC, cd.companyName, e.firstname
			OFFSET @PageSize * (@PageNumber - 1) ROWS 
			FETCH NEXT @PageSize ROWS ONLY
END
END



----------

CREATE OR ALTER   PROCEDURE [dbo].[usp_GetYearlyAllEmployeesWithGradeByManager] 
(
	@YearID INT,
	@companyid INT,
	@managerid INT,
	@Grade VARCHAR(20),
	@PageNumber INT,
	@PageSize   INT,
	@SearchString NVARCHAR(256),
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

DECLARE @LastYear INT = FORMAT(GETDATE(), 'yyyy') - 1
DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

CREATE TABLE #tempEmpAverageAssessmentDet (
	companyid INT, empid INT, avgScore DECIMAL(2,0), AvgScoreRank VARCHAR(100), yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	SELECT * INTO #tempScore
	FROM
	(
		SELECT companyID, empid, empName, Score, ManagerId,  FORMAT(RatingDate, 'yyyyMM') AS monyrID
		FROM vw_EmpAssessmentScore
		WHERE FORMAT(RatingDate, 'yyyyMM') > FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')  AND empStatus = 1 
	)t

	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT  companyID, empId, avgscore, avgScoreRank, 0 AS YearID
		FROM
		(
			SELECT companyID, empId, empName, CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS DECIMAL(10,0)) AS avgscore,
			dbo.GetScoreRank(CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS decimal(10,0))) AS AvgScoreRank,
			row_number() OVER (PARTITION BY empid ORDER BY empid) AS rownumber
			FROM #tempScore 
		)v WHERE rownumber = 1
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT companyID , empID, avgScore, AvgScoreRank, year AS YearID FROM vw_EmpAverageAssessmentDet 
		WHERE companyid = @companyId AND year = @YearId
	)k
	END


	CREATE TABLE #tempEmpAssessmentScore (
	companyid INT, empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END

	SET @YearId = CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END
IF(@managerid > 0 and @companyid > 0 AND @Grade <> '') 
BEGIN  

WITH #tempEmployeedetailsMC  (empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
	 departmentid, managerid,  empstatus,  roleid, currentsalary,empimgpath)
	AS
	(
	SELECT empid,  employeeId,  companyid,  firstname, ISNULL(middlename, '') AS middlename,  lastname,  email, ISNULL( jobtitle,'') AS jobtitle, CONVERT(DATE,  joiningdate,101) AS joiningdate,
		   departmentid, ISNULL(managerid,0) AS managerid,  empstatus,  roleid, currentsalary,ISNULL(FirstLevel.empimgpath,'') AS empimgpath
	FROM employeedetails AS FirstLevel
	WHERE bactive = 1 AND empstatus = 1 AND managerID = @Managerid 
	AND companyId = @companyId
	UNION ALL
	SELECT NextLevel.empid, NextLevel.employeeId, NextLevel.companyid, NextLevel.firstname, ISNULL(NextLevel.middlename, '') AS middlename, NextLevel.lastname, NextLevel.email, ISNULL(NextLevel.jobtitle,'') AS jobtitle, CONVERT(DATE, NextLevel.joiningdate,101) AS joiningdate,
		   NextLevel.departmentid, ISNULL(NextLevel.managerid,0) AS managerid, NextLevel.empstatus, NextLevel.roleid, NextLevel.currentsalary,ISNULL(NextLevel.empimgpath,'') AS empimgpath
	FROM employeedetails AS NextLevel
	INNER JOIN #tempEmployeedetailsMC AS t ON NextLevel.managerid = t.empId 
	WHERE NextLevel.bactive = 1 AND NextLevel.empstatus = 1 AND NextLevel.companyid = @companyid
	)SELECT * INTO #tempEmployeedetailsMC 
 FROM #tempEmployeedetailsMC WHERE Format(joiningDate, 'yyyy') <= @YearID
	AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR departmentId IN (SELECT department FROM #departmentlist))


SELECT * INTO #tempEmployeeResult
FROM(
		SELECT empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
			   departmentid, department, managerid, managerFName, managerLName, empstatus,  roleid, currentsalary,empimgpath, lastAssessedDate, AvgScoreRank, AvgScore,
			   LastScoreRank, RatingCompleted,managerLastAssessedDate
		FROM(
				SELECT e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
					   e.departmentid, d.department,  e.managerid , ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName,  e.empstatus,
					    e.roleid, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath,
					   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
					   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
					   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpAssessmentScore WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
					   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted
					   ,0 AS ord,ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
				FROM #tempEmployeedetailsMC e 
				LEFT JOIN employeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
				LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
				LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid AND e.empstatus = 1
				LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
				
		)s
		WHERE CASE WHEN dbo.Get_grade_value(LastScoreRank) IS NULL THEN '' ELSE dbo.Get_grade_value(LastScoreRank) END = @Grade
) vc

	DECLARE @TotalRowCount INT
	SELECT @TotalRowCount = COUNT(empid) FROM #tempEmployeeResult
	WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
	OR lastname LIKE '%'+ @SearchString + '%'
	OR firstname LIKE '%'+ @SearchString + '%'
	OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

	SELECT @TotalRowCount AS totalrowcount,empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
		   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary,empimgpath,
		   lastAssessedDate, AvgScoreRank, AvgScore,
		   LastScoreRank, RatingCompleted,managerLastAssessedDate
	FROM #tempEmployeeResult
	WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
	OR lastname LIKE '%'+ @SearchString + '%'
	OR firstname LIKE '%'+ @SearchString + '%'
	OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
	ORDER BY firstname
	OFFSET @PageSize * (@PageNumber - 1) ROWS 
	FETCH NEXT @PageSize ROWS ONLY
	END
END

----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetYearWiseHierachywiseDetails]
(
	@YearID INT,
	@managerId int,
	@companyid int, 
	@departmentlist VARCHAR(1000)
)
As 
BEGIN


SELECT department INTO #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d;

;WITH #tempEmployee (empid, companyid, managerid, firstname, middlename, lastname, departmentId, roleid, empstatus, JoiningYearId)
AS (
	SELECT empId, companyId, managerId, 
		   FirstLevel.firstname,FirstLevel.middlename,FirstLevel.lastname,departmentId, roleId, empStatus,
	Format(joiningdate, 'yyyy') AS JoiningYearId
	FROM EmployeeDetails AS FirstLevel
	WHERE ManagerID=@managerId and companyid = @companyid AND bactive = 1 AND empStatus = 1
	UNION ALL
	SELECT NextLevel.empId, NextLevel.companyId,  
			NextLevel.managerid,NextLevel.firstname,NextLevel.middlename, NextLevel.lastname,NextLevel.departmentId, NextLevel.roleId,
			NextLevel.empStatus, Format(NextLevel.joiningdate, 'yyyy') AS JoiningYearId
	FROM [dbo].EmployeeDetails AS NextLevel
	INNER JOIN #tempEmployee AS t ON NextLevel.managerId = t.empId
	WHERE NextLevel.companyId = @companyid AND NextLevel.bactive = 1 AND NextLevel.empStatus = 1
	)SELECT * INTO #tempEmployee
FROM #tempEmployee WHERE departmentId IN (SELECT department FROM #departmentlist)
AND JoiningYearId <= CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END

IF(@YearID = 0)
BEGIN
DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM');
	SELECT	e.CompanyId, e.Empid, dbo.[GetEmpNameById](null,e.firstname,e.middlename,e.lastname) AS empName,
			CAST(a.score AS DECIMAL(2,0)) AS score, [dbo].[Get_grade_value]([dbo].[GetScoreRankById](scoreid)) AS ScoreRank, 
			a.assessmentdate AS RatingDate, a.Assessmentby AS Assessmentbyid,
			dbo.[GetEmpNameById](u.empid,null,null,null) AS Assessmentby, a.id AS assessmentId,managerId,departmentid,roleid,e.empstatus, CAST(0 as int) AS inDirectManager,
			DATEPART(yyyy,a.assessmentdate) AS yearId
	FROM employee_assessment  a
	LEFT JOIN #tempEmployee e ON a.empid= e.empid
	LEFT JOIN userlogin u ON a.Assessmentby = u.userid
	WHERE e.companyid=@companyid AND u.bactive= 1 AND FORMAT(assessmentdate, 'yyyyMM') > @Last12MonthYear
END
ELSE
BEGIN
	SELECT	e.CompanyId, e.Empid, dbo.[GetEmpNameById](null,e.firstname,e.middlename,e.lastname) AS empName,
			CAST(a.score AS DECIMAL(2,0)) AS score, [dbo].[Get_grade_value]([dbo].[GetScoreRankById](scoreid)) AS ScoreRank, 
			a.assessmentdate AS RatingDate, a.Assessmentby AS Assessmentbyid,
			dbo.[GetEmpNameById](u.empid,null,null,null) AS Assessmentby, a.id AS assessmentId,managerId,departmentid,roleid,e.empstatus, CAST(0 as int) AS inDirectManager,
			DATEPART(yyyy,a.assessmentdate) AS yearId
	FROM employee_assessment  a
	LEFT JOIN #tempEmployee e ON a.empid= e.empid
	LEFT JOIN userlogin u ON a.Assessmentby = u.userid
	WHERE e.companyid=@companyid AND u.bactive= 1 AND DATEPART(yyyy,a.assessmentdate) =  @YearID
END
END
----------

CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateEmpAssessmentScore]
(
	    @EmpId int,
		@AssessmentId INT,
		@Result INT OUT
)	
AS
BEGIN
    	 
IF EXISTS(SELECT 1 FROM employee_assessment WHERE id = @assessmentid)
BEGIN
	    DECLARE @score INT,@scoreid INT

 		SELECT @score = e.score, @scoreid = e.scoreid FROM [dbo].[vw_AssessmentScoreCalculation] e 
		WHERE empid = @empid and e.assessmentid= @assessmentid

		UPDATE employee_assessment SET score=@score, scoreid=@scoreid
		WHERE id=@assessmentid

		SET @result = 1
		SELECT @result 
END
ELSE
	 BEGIN
		SET @result = 0;
		SELECT @result 
	 END
	
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateEmployeeDetails_New] 
	(
		@empid INT,
		@employeeId VARCHAR(100),
		@companyid INT,
        @firstName NVARCHAR(100),
        @middleName NVARCHAR(100),
        @lastName NVARCHAR(100),
		@suffix VARCHAR(8),
        @email VARCHAR(100),
		@phoneNumber VARCHAR(25),
        @jobTitle VARCHAR(100),
        @joiningDate DATE,
        @workCity NVARCHAR(100),
        @workState NVARCHAR(100),
        @workZipcode VARCHAR(25),
        @departmentId SMALLINT,
        @managerId INT,
        @managerFName NVARCHAR(200),
		@managerLName NVARCHAR(200),
        @empStatus BIT,
        @roleId TINYINT,
        @dateOfBirth DATE,
        @raceorethanicityId SMALLINT,
        @jobGroupId SMALLINT,
		@jobGroup NVARCHAR(100),
        @gender NVARCHAR(50),
        @jobCategoryId SMALLINT,
		@jobCategory NVARCHAR(100),
        @empLocation NVARCHAR(100),
		@jobCodeId SMALLINT,
		@jobCode NVARCHAR(100),
        @lastPromodate DATE,
        @currentSalary DECIMAL(12,2),
        @lastIncDate DATE,
        @countryId INT,
        @regionId INT,
        @updatedBy INT,
		@result INT OUT	
)	
AS
BEGIN
DECLARE @AspNetUser NVARCHAR(MAX)
--IF([dbo].[GetRole](@updatedBy) = 2  or [dbo].[GetRole](@updatedBy) = 1)
BEGIN
	IF EXISTS(SELECT * FROM employeedetails WHERE companyid = @companyid AND employeeid = @employeeId AND empid <> @empid )
	BEGIN
		SET @result = 3;
		SELECT @result; ---Employee id already Exists
	END 
	ELSE IF EXISTS(SELECT * FROM employeedetails WHERE @email <> '' AND companyid = @companyid AND email = @email AND empid <> @empid)
	BEGIN
		SET @result = 2;
		SELECT @result; ---Email id already Exists 
	END
	ELSE IF (@phoneNumber <> '' AND EXISTS(SELECT * FROM employeedetails WHERE @phoneNumber <> '' AND phonenumber = @phoneNumber AND empid != @empid ))
	BEGIN
		SET @result = 4;
		SELECT @result; ---phone number already Exists 
	END
	ELSE
	BEGIN
		IF EXISTS (SELECT * FROM employeedetails WHERE companyid = @companyid AND empid = @empid)
		BEGIN
			--IF NOT EXISTS(SELECT * FROM [dbo].[employeedetails] WHERE email=@email AND empid <> @empid AND bactive = 1)
			--BEGIN
				UPDATE employeedetails SET 
						employeeId = @employeeId,
						firstName =@firstName ,
						middleName = @middleName,
						lastName = @lastName,
						suffix = @suffix,
						email =@email,
						phoneNumber=@phoneNumber,
						jobTitle =@jobTitle,
						joiningDate =@joiningDate,
						workCity = @workCity,
						workState = @workState,
						workZipcode = @workZipcode,
						departmentId = @departmentId,
						managerId = @managerId,
						managername = @managerFName,
						managerlname = @managerLName,
						empStatus = @empStatus,
						roleId = @roleId,
						dateOfBirth = @dateOfBirth,
						raceorethanicityId = @raceorethanicityId,
						gender = @gender,
						empLocation = @empLocation,
						jobGroupId = @jobGroupId,
						jobGroup = @jobGroup,
						jobCategoryId = @jobCategoryId,
						jobCategory = @jobCategory,
						jobCodeId = @jobCodeId,
						jobCode = @jobCode,
						lastPromodate = @lastPromodate,
						currentSalary = @currentSalary,
						lastIncDate = @lastIncDate,
						countryId = @countryId,
						regionId = @regionId,
						bactive = 1,
						updatedBy = @updatedBy,
						updateddtstamp = GETDATE(),
						ismailsent = (CASE WHEN @roleId =5 THEN 0 ELSE ismailsent END),
						phoneconfirmed=(CASE WHEN @phoneNumber <> phonenumber THEN 0 ELSE phoneconfirmed END),
						optforsms=(CASE WHEN @phoneNumber <> phonenumber THEN 0 ELSE optforsms END)
				WHERE companyid = @companyid AND empid = @empid

			--	SELECT @AspNetUser = UserId FROM AspNetUserClaims WHERE ClaimType ='EmpId' AND ClaimValue = CAST(@empId AS NVARCHAR)
			--	UPDATE AspNetUserClaims SET ClaimValue = @roleId WHERE UserId = @AspNetUser AND ClaimType ='RoleId'
				SET @result = 1;  --updated successfully
				SELECT @result;
		END
		--ELSE
		--BEGIN
		--	SET @result = 2;
		--	SELECT @result; ---Email id already Exists 
		--END
	END
	--ELSE
	--BEGIN
	--	SET @result = 0;
	--	SELECT @result; ---Not Exists data
	--END
END
--ELSE 
--BEGIN
--	SET @result = -1;
--		SELECT @result; ---For not authorized user
--END
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateEmpProfile]
(
		@empid INT,
		@companyid INT,
        @empImgPath VARCHAR(100),
        @updatedBy INT,
		@result INT OUT	
)	
AS
BEGIN
BEGIN
	IF EXISTS (SELECT * FROM employeedetails WHERE companyid = @companyid AND empid = @empid)
	BEGIN
			UPDATE employeedetails SET 
					empImgPath = @empImgPath, 
					updatedBy = @updatedBy,
					updateddtstamp = GETDATE()
			WHERE companyid = @companyid AND empid = @empid
			SET @result = 1;  --updated successfully
			SELECT @result;
	END
	ELSE
	BEGIN
		SET @result = 0;
		SELECT @result; ---Not Exists data
	END
END
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateNotificationFlagIsSent] 
(
 @Ids VARCHAR(MAX),
 @result INT OUTPUT 
)
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

    UPDATE [dbo].[NotificationsLogs] SET [IsSent] = 1 WHERE ID IN(SELECT * from dbo.StringSplit(@Ids, ','))
	SET @result = 1;
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateNotificationMarkAsRead]
(
 @Ids VARCHAR(MAX),
 @result INT OUT
 
)
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;
	IF EXISTS(SELECT * FROM [NotificationsLogs] WHERE ID IN(SELECT * from dbo.StringSplit(@Ids, ',')))
		BEGIN
			UPDATE [dbo].[NotificationsLogs] SET [MarkAs] = 1 WHERE ID IN(SELECT * from dbo.StringSplit(@Ids, ','))
			SET @result  = 1 
		END
	ELSE

		BEGIN
			SET @result  = 0
		END
		 
	SELECT @result ;
END

----------
CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateUserDetails]
(
	
	@empid INT, 
	@username VARCHAR(150),
	@bactive BIT,
	@updatedby INT,
	@result INT OUT
)
	
AS
BEGIN
	
	
	SET NOCOUNT ON;

IF EXISTS(SELECT * FROM userlogin WHERE empid = @empid)
	BEGIN
		UPDATE userlogin SET 
				username = @username,
				bactive = @bactive, 
				updatedby = @updatedby,
				updateddtstamp = GETDATE()
		WHERE empid = @empid    
		SET @result = 1
		SELECT @result 
	END
ELSE
	BEGIN
	SET @result = 0;
	SELECT @result 
	END
END

----------
CREATE   OR ALTER  PROCEDURE [dbo].[usp_UserLogin]
(
	@username VARCHAR(60),
	@result VARCHAR(25) =''
)
AS
BEGIN
 DECLARE @empid INT 
 DECLARE @companyId INT

IF NOT EXISTS(SELECT * FROM userlogin WHERE username = @username AND bactive = 1)
BEGIN 
SET @result = 'User does not exists'
SELECT @result AS Error
END
ELSE IF NOT EXISTS(SELECT * FROM userlogin u INNER JOIN employeedetails e ON e.empid = u.empid 
  INNER JOIN companydetails c ON c.bactive = 1 AND c.compid = e.companyid
  WHERE u.username = @username AND c.bactive = 1 AND e.roleid <> 5 
  AND Convert(Date, c.ContractStartDate,101) <= Convert(Date, GETDATE(),101) )
BEGIN
SET @result = 'User does not exists'
SELECT @result AS Error
END
ELSE
BEGIN

 SELECT @companyId = compid FROM userlogin u INNER JOIN employeedetails e ON e.empid = u.empid 
  INNER JOIN companydetails c ON c.bactive = 1 AND c.compid = e.companyid
  WHERE u.username = @username AND c.bactive = 1 AND e.roleid <> 5;

IF (@companyId IS NOT NULL)
BEGIN
	IF EXISTS(SELECT * FROM companydetails c 
  		WHERE compid  = @companyId AND  
		DATEADD(day,isnull(gracePeriod,0),Convert(Date, c.ContractEndDate,101)) >= Convert(Date, GETDATE(),101))

BEGIN
	IF EXISTS(SELECT username  FROM userlogin u INNER JOIN employeedetails e ON e.bactive = 1 AND e.empid = u.empid 
	    INNER JOIN companydetails c ON c.bactive = 1 AND c.compid = e.companyid
	    WHERE u.username = @username AND u.bactive = 1 AND e.roleid <> 5 AND
	    DATEADD(day,isnull(gracePeriod,0),Convert(Date, c.ContractEndDate,101)) >= Convert(Date, GETDATE(),101))
BEGIN
IF EXISTS(SELECT * FROM userlogin WHERE username = @username AND bactive = 1)
   BEGIN 
	SELECT @empid =  empid FROM userlogin u WHERE username = @username AND bactive = 1 

	DECLARE @roleId INT
	SELECT @roleId =  roleid FROM employeedetails  WHERE empId = @empid AND bactive = 1 
   
	SELECT e.empid, e.companyid, u.userid, u.username, e.email, c.companyname,  e.roleid, [dbo].[GetDimensionValueByID](1,e.roleid) AS role, ISNULL(e.jobtitle,'') AS jobtitle, e.suffix,
	e.firstname, e.middlename, e.lastname, e.email,  0 AS departmentid, '' AS department, 
	e.empImgPath, c.CompImgPath AS compLogoPath,ISNULL(e.phonenumber,'') AS phonenumber             
	FROM employeedetails e
	INNER JOIN userlogin u ON u.empid = e.empid 
	INNER JOIN companydetails c ON c.compid = e.companyid 
	--INNER JOIN rolemaster r ON r.roleid = e.roleid 
	WHERE e.empid = @empid AND e.bactive = 1 AND u.bactive = 1 AND e.empStatus = 1
	  
   END
  ELSE
   BEGIN
SET @result = 'Invalid password'
SELECT @result AS Error --1 For 'Invalid Password'
   END
END
ELSE
BEGIN
BEGIN 
  SET @result = 'User does not exists'
  --SET @result =0
  SELECT @result AS Error --0 For 'User Not Exists'
END

END
END
ELSE 
BEGIN
SET @result = 'Company contract expired';
  SELECT @result AS Error
END
END
ELSE
BEGIN
  SET @result = 'Company is inactivated';
  SELECT @result AS Error
END
END
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_DeleteUserLogin] 
(
	@empid INT,
	@updatedby VARCHAR(50),
	@result INT OUT	
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	
IF([dbo].[GetRole](@updatedby) = 2)
BEGIN
    -- Insert statements for procedure here
	IF EXISTS (SELECT * FROM userlogin WHERE empid = @empid)
	BEGIN
		IF EXISTS(SELECT * FROM employeedetails WHERE empid = @empid AND (bactive = 0 OR roleid=5))
		BEGIN
			UPDATE userlogin SET bactive  = 0,
					updatedby = @updatedby,
					updateddtstamp = GETDATE()
			WHERE empid = @empid  
			SET @result = 1;  ----Sucessfully  Deleted
			SELECT @result;
		END
	END
	ELSE
	BEGIN
		SET @result = 0;
		SELECT @result;  ---------login not exists or already deleted
	END
END 
ELSE 
	SET @result = -1; ---------Not authorised to delete
	SELECT @result;
END

----------
CREATE or ALTER PROCEDURE [dbo].[usp_DeleteDeviceInfo]
(
	@DeviceId VARCHAR(MAX),
	@Result int out
 )
AS
BEGIN
	DELETE  From DeviceRegistration WHERE DeviceId = @DeviceId;
	SET @Result = 1
	SELECT @Result
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetEmployeeByEmpIdsForMails]
(
	@CompanyId INT,
	@empIdList VARCHAR(200)
)
AS
BEGIN 
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT empIds into #empIdList FROM (SELECT value AS empIds FROM  STRING_SPLIT(@empIdList, ',')) d;
		
	SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, 
		   e.email, e.departmentid, ISNULL(e.managerid,0) AS managerid, e.empstatus, e.roleid,
		   cd.contractstartDate AS companyContractStartDate,
		   cd.contractEndDate AS companyContractEndDate
	FROM employeedetails e 
	LEFT JOIN companydetails cd ON cd.bactive = 1 AND cd.compId = e.companyID
	WHERE e.companyid = @Companyid AND e.bactive = 1 AND e.empStatus = 1 AND ISNULL(e.IsMailSent, 0) = 0
	AND e.roleid <> 5 AND (Convert(Date, cd.ContractStartDate,101) <= Convert(Date, GETDATE(),101)
	AND DATEADD(day,isnull(cd.gracePeriod,0),Convert(Date, cd.ContractEndDate,101)) >= Convert(Date, GETDATE(),101))
	AND e.empID in (SELECT empIds FROM #empIdList)

END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateEmpForIsMailSent] 
(
		@empid INT,
		@companyid INT,
        @UpdatedBy INT,
		@result INT OUT	
)	
AS
BEGIN
IF EXISTS(SELECT * FROM employeedetails WHERE companyid = @companyid AND empid = @empid AND empstatus =1  AND bactive = 1 )
BEGIN
		UPDATE employeedetails SET  
			   isMailSent = 1,
			   updatedBy = @updatedBy,
			   updateddtstamp = GETDATE()
		WHERE companyid = @companyid AND empid = @empid
		AND empstatus =1  AND bactive = 1
	
				SET @result = 1;  --updated successfully
				SELECT @result;
END
ELSE 
BEGIN
		SET @result = -1 
		SELECT @result;
END
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateEmployeeCurrentSalary] 
(
		@empid INT,
		@companyid INT,
        @currentSalary DECIMAL(12,2),
        @updatedBy INT,
		@result INT OUT	
)	
AS
BEGIN
IF([dbo].[GetRole](@updatedBy) NOT IN (1,2,5))
BEGIN
	IF EXISTS(SELECT * FROM employeedetails WHERE companyid = @companyid AND empid = @empid AND empstatus = 0 AND bactive = 1)
	BEGIN
		SET @result = 2; --For inactive employee
		SELECT @result;
	END
	ELSE IF EXISTS (SELECT * FROM employeedetails WHERE companyid = @companyid AND empid = @empid AND empstatus = 1 AND bactive = 1)
	BEGIN
			UPDATE employeedetails SET 
				   currentSalary = @currentSalary, 
				   updatedBy = @updatedBy,
				   updateddtstamp = GETDATE()
			WHERE companyid = @companyid AND empid = @empid
			SET @result = 1;  --updated successfully
			SELECT @result;
	END
	ELSE
	BEGIN
		SET @result = 0;
		SELECT @result; ---Not Exists data
	END
END
ELSE 
BEGIN
		SET @result = -1;
		SELECT @result; ---For not authorized user
END
END
----------
CREATE  OR ALTER   PROCEDURE [dbo].[usp_GetInactivityManagers]
(
	@companyid INT,
	@inactivitydays INT,
	@reminderdays INT
)
AS
BEGIN

SELECT  *  FROM
 (
	SELECT  e.empid,e.roleid,e.firstname,e.lastname,e.email,e.phonenumber,
			[dbo].[GetInactivityDays](e.empid) AS inactivitydays,ISNULL(ismailsent,0)AS ismailsent,
			ISNULL(optforsms,0) AS optforsms,ISNULL(phoneconfirmed,0) AS phoneconfirmed
	FROM employeedetails e LEFT JOIN companydetails c
	ON c.compid = e.companyid AND c.bactive=1
	WHERE e.companyid=@companyid
			AND e.roleid NOT IN (1,5)
			AND DATEDIFF(DAY,e.joiningdate,GETDATE()) >= @inactivitydays
			AND DATEDIFF(DAY,c.ContractStartDate,GETDATE()) >= @inactivitydays
			AND ismailsent=1
			AND e.bactive=1 
			AND e.empstatus=1
)T
WHERE inactivitydays > @inactivitydays
AND ISNULL(DATEDIFF(DAY,(SELECT MAX(createddtstamp) FROM InactivityLog
WHERE empid=T.empid),GETDATE()),(@reminderdays+1)) > @reminderdays

END
----------
CREATE  OR ALTER  PROCEDURE [dbo].[usp_AddInActivityLog]
(
	@empid INT,
	@firstname VARCHAR(100),
	@lastname VARCHAR(100),
	@email VARCHAR(100),
	@smstext VARCHAR(500),
	@emailtext VARCHAR(1000),
	@phonenumber VARCHAR(25),
	@inactivitydays INT,
	@createdby INT,
	@result INT OUT
)
AS
BEGIN

	INSERT INTO [dbo].[InactivityLog]
			(
				[empid]
			   ,[firstname]
			   ,[lastname]
			   ,[email]
			   ,[smstext]
			   ,[emailtext]
			   ,[phonenumber]
			   ,[inactivitydays]
			   ,[bactive]
			   ,[createdby]
			   ,[createddtstamp]
		   )
     VALUES
           (
				@empid,
				@firstname,
				@lastname,
				@email,
				@smstext,
				@emailtext,
				@phonenumber,
				@inactivitydays,
				1,
				@createdby,
				GETDATE()
		   )
	SELECT @result=1
END
----------
CREATE OR ALTER   PROCEDURE [dbo].[usp_GetEmployeeByEmpIdForProfile] 
(
	@empid INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT empid, ISNULL(employeeId, '' ) AS EmployeeId, firstname, lastname, ISNULL(phonenumber,'') as phonenumber, ISNULL(empimgpath,'') as empimgpath, workcity, workstate, workzipcode,
		   ISNULL(phoneConfirmed, 0) AS phoneConfirmed, ISNULL(optforsms, 0) AS optforsms
    FROM employeedetails	
	WHERE empid = @empid AND empStatus = 1 AND bactive = 1 AND roleid <> 5

END
----------
CREATE OR ALTER   PROCEDURE [dbo].[usp_GetUsersByPhoneNumbersTenant] 
(
      @tblAspNetUsers Type_PhoneNumber READONLY
)
AS
BEGIN
                                   
SELECT u.phonenumber FROM employeedetails AS u
INNER JOIN @tblAspNetUsers AS p
ON (p.PhoneNumber =u.PhoneNumber AND p.email <> u.email  AND (ISNULL(p.email, '') <> '' AND ISNULL(u.email, '') <> '')) 
 OR (p.PhoneNumber =u.PhoneNumber AND p.employeeid <> u.employeeid AND (ISNULL(p.email, '') = '' OR ISNULL(u.email, '') = '' ))
 WHERE p.phonenumber <> '' 
END

----------
CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateEmployeeProfile] 
(
		@empid INT,
        @workCity NVARCHAR(100),
        @workState NVARCHAR(100),
        @workZipcode VARCHAR(25),
		@phonenumber VARCHAR(25),
        @updatedBy INT,
		@result INT OUT	
)	
AS
BEGIN
IF dbo.GetRoleByEmpid(@empId) NOT IN (1,5)
BEGIN
		IF (@phoneNumber <> '' AND EXISTS(SELECT * FROM employeedetails WHERE @phoneNumber <> '' AND phonenumber = @phoneNumber AND empid <> @empid ))
		BEGIN
					SET @result = 4;
					SELECT @result; ---phone number already Exists 
		END
		ELSE IF EXISTS(SELECT * FROM employeedetails WHERE bactive = 1 AND empID = @empID AND empStatus = 0)
		BEGIN
					SET @result = 2;
					SELECT @result; ----Employee Is inactive
		END
		ELSE IF EXISTS(SELECT * FROM employeedetails WHERE bactive = 0 AND empID = @empID)
		BEGIN
					SET @result = 0;
					SELECT @result; ----Employee Not exists
		END
		ELSE 
		BEGIN
			DECLARE @existingPhoneNumber VARCHAR(25)
			SELECT @existingPhoneNumber = phonenumber FROM employeedetails WHERE empId = @EmpID AND bactive =1 AND empstatus = 1
			
				UPDATE employeedetails SET 
						workCity = @workCity,
						workState = @workState,
						workZipcode = @workZipcode,
						phoneNumber= @phonenumber,
						bactive = 1,
						phoneconfirmed = CASE WHEN @existingPhoneNumber <> @phonenumber THEN 0 ELSE phoneconfirmed END, 
						optforsms = CASE WHEN @existingPhoneNumber <> @phonenumber THEN 0 ELSE optforsms END,
						updatedBy = @updatedBy,
						updateddtstamp = GETDATE()
				WHERE  empid = @empid
				SET @result = 1;  --updated successfully
				SELECT @result;
		END
END
ELSE 
BEGIN
		SET @result = -1
		SELECT @result;
END
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_DeleteDeviceByLoginUserId]
(
   @UserId INT,
   @Result INT OUT
)
AS 
BEGIN
  DELETE FROM DeviceRegistration WHERE UserID=@UserId
  SET @Result = 1
  SELECT @Result
END
----------
CREATE OR ALTER    PROCEDURE [dbo].[usp_AddUserSmsVerificationCode]
(
	@Empid INT,
	@VerificationCode INT,
	@VerificationCodeTimeout INT,
	@PhoneNumber VARCHAR(25),
	@CreatedBy INT,
	@Result INT OUT
)	
AS
BEGIN
	
DECLARE @role INT
SELECT @role = roleid FROM EmployeeDetails WHERE empid=@empid
SET @Result=0;

IF @role NOT IN(1,5)
BEGIN
	SELECT @Result= isnull(PhoneConfirmed,0) FROM EmployeeDetails WHERE empstatus=1  AND bactive=1 AND empid=@empid
	IF @Result =1
	BEGIN
			SET @Result =-3 --phone number already verified
	END
	ELSE
	BEGIN
	   SELECT @Result=count(1) FROM EmployeeDetails WHERE  empstatus=1 AND bactive=1  
	   AND empid=@empid AND  replace(PhoneNumber,' ','')= replace(@PhoneNumber,' ','')
	   AND ISNULL(phonenumber,'') <> ''

	   IF @Result=1

		BEGIN
		IF NOT EXISTS(SELECT * FROM [UserSmsVerificationCode]  WHERE  Empid=@Empid AND bactive=1) 
		BEGIN
				INSERT INTO [UserSmsVerificationCode] 
				(
							Empid,
							Verificationcode,
							VerificationcodeTimeout, 
							Bactive,
							CreatedBy, 
							CreateddtStamp  
				 )
				 VALUES 
				(
							@Empid, 
							@VerificationCode,
							@VerificationCodeTimeout,
							1,
							@CreatedBy,
							GETDATE() 
				);
				SET @result = 1
		END
		ELSE
		BEGIN
			 UPDATE [UserSmsVerificationCode] SET 
					  Verificationcode=@Verificationcode,
					  VerificationcodeTimeout=@VerificationcodeTimeout,
					  UpdatedBy=@Createdby,
					  UpdateddtStamp=GETDATE() 
					  WHERE Empid=@Empid
			  SET @Result = 1;
		END
	END	
	ELSE
	BEGIN
			SET @Result = -2; --phone number not registered for user
	END
	END
	END
    ELSE
		BEGIN
			SET @Result = -1;--unauthorized access
		END
	END
----------
CREATE   OR ALTER PROCEDURE [dbo].[usp_VerifySMSVerificationCode]
(
	@Empid INT,
	@Verificationcode INT,
	@UpdatedBy INT,
	@Result INT OUT
)
AS
BEGIN
	
	IF EXISTS(SELECT *  FROM [UserSmsVerificationCode] WHERE Empid=@Empid AND Verificationcode=@Verificationcode AND bactive=1)
		BEGIN
			SELECT @Result=COUNT(1)  FROM [UserSmsVerificationCode] 
			WHERE Empid=@Empid 
			AND VerificationCode=@VerificationCode
			AND DATEDIFF(MINUTE,ISNULL(updateddtstamp,createddtstamp),GETDATE()) <= VerificationCodeTimeout

			IF @Result > 0
			 BEGIN
			   IF EXISTS(SELECT * FROM Employeedetails WHERE Empid=@Empid AND ISNULL(PhoneConfirmed,0)=0)
				   BEGIN
					 UPDATE Employeedetails 
					 SET [PhoneConfirmed] =1,
					 UpdatedBy=@UpdatedBy,
					 UpdateddtStamp=GETDATE()
					 WHERE EmpId=@Empid;

					 SET @Result=1
					END
				ELSE
					BEGIN
					  SET @Result=-3
					 END
			 END 
			 ELSE
			  BEGIN
				SET @Result=-2
			  END  
		END
	ELSE
		BEGIN
		 SET @Result=0
		END		
	END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateOptForSMS] 
(
		@empid INT,
		@optforsms BIT,
        @updatedBy INT,
		@result INT OUT	
)	
AS
BEGIN
IF dbo.GetRoleByEmpid(@empId) NOT IN (1,5) 
BEGIN
	DECLARE @existingOptForSms BIT
	SELECT @existingOptForSms = optforsms FROM employeedetails WHERE empId = @empid AND empstatus = 1 AND bactive = 1 AND phoneConfirmed = 1;

		IF EXISTS(SELECT * FROM employeedetails WHERE empId = @empid AND empstatus = 1 AND bactive = 1 AND phoneConfirmed = 0)
		BEGIN
					SET @result = 3;
					SELECT @result; ---phone number is not varified
		END
		ELSE IF EXISTS(SELECT * FROM employeedetails WHERE bactive = 1 AND empID = @empID AND empStatus = 0)
		BEGIN
					SET @result = 2;
					SELECT @result; ----Employee Is inactive
		END
		ELSE IF EXISTS(SELECT * FROM employeedetails WHERE bactive = 0 AND empID = @empID)
		BEGIN
					SET @result = 0;
					SELECT @result; ----Employee Not exists
		END
		ELSE IF (@existingOptForSms = @optForSms)
		BEGIN
					SET @result = 4;
					SELECT @result; ----passed Opt For sms status is same as database
		END
		ELSE 
		BEGIN
				
				UPDATE employeedetails SET 
						optforsms= @optforsms,
						updatedBy = @updatedBy,
						updateddtstamp = GETDATE()
				WHERE  empid = @empid AND empstatus = 1 AND bactive = 1 AND phoneconfirmed = 1
				SET @result = 1;  --updated successfully
				SELECT @result;
		END
END
ELSE 
BEGIN
		SET @result = -1
		SELECT @result;
END
END
----------

CREATE OR ALTER   PROCEDURE [dbo].[usp_GetAllEmployeesWithoutPagination]
(
	@companyid INT,
	@managerid INT,
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
SET NOCOUNT ON;
DECLARE @curryear INT = DATEPART(yyyy,GETDATE());

SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d


IF(@managerid > 0 and @companyid > 0) 
BEGIN  

		SELECT e.empid, e.employeeId, e.companyid, '' AS companyName, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email,
			   ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, 
			   ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
			   e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, e.empstatus, e.roleid,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
			   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From vw_EmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid) AS VARCHAR),'') AS RatingCompleted,
				0 AS ord, ISNULL(e.IsMailSent,0) AS IsMailSent, e.currentsalary,
				 ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM EmployeeDetails e 
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN vw_EmpAverageAssessmentDet vg ON vg.empId = e.empid AND year = @curryear
		LEFT JOIN GetLastAssessmentDateByManager(ABS(@managerid)) assmnt on assmnt.empid=e.empid
		WHERE e.companyid = @companyid AND e.bactive = 1 AND e.managerid = @managerid  
		AND e.empstatus = 1 AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
		ORDER BY e.empid DESC, e.lastname
		
END
ELSE if (@managerid < 0 and @companyid > 0) 
BEGIN
		
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, e.jobtitle, 
			   CONVERT(DATE, e.joiningdate,101) AS joiningdate,
			   ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, 
			   e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, e.empstatus, e.roleid,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment 
			   WHERE empid = e.empid), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
			   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From vw_EmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS 			   lastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid) AS VARCHAR),'')                            AS RatingCompleted,
			   CASE WHEN ISNULL(e.managerid,0) = ABS(@managerid) THEN 0 ELSE 1 END AS ord, ISNULL(e.IsMailSent,0) AS IsMailSent, e.currentsalary ,
			   ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM Employeedetails e 
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN vw_EmpAverageAssessmentDet vg ON vg.empId = e.empid AND year = @curryear
		LEFT JOIN GetLastAssessmentDateByManager(ABS(@managerid)) assmnt on assmnt.empid=e.empid
		WHERE e.companyid = @companyid AND e.bactive = 1 
			AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
		GROUP BY e.empid, e.employeeId,e.companyid, e.firstname, ISNULL(e.middlename, ''), e.lastname, e.email, e.jobtitle,  e.joiningdate,
				 e.departmentid, d.department, ISNULL(e.managerid,0) , e.empstatus, e.roleid, ISNULL(m.firstname, ''), ISNULL(m.lastname, ''),
				ISNULL(vg.AvgScoreRank, ''),vg.AvgScore, ISNULL(e.IsMailSent,0), e.currentsalary,assmnt.assessmentdate
		ORDER BY CASE WHEN ISNULL(e.managerid,0) = ABS(@managerid) THEN 0 ELSE 1 END, e.empid DESC, e.lastname
		
END
ELSE if (@managerid = 0 and @companyid > 0) 
BEGIN
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, 
		       ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, 
		       ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate, e.departmentid, d.department, 
		       ISNULL(e.managerid,0) AS managerid, e.empstatus, e.roleid,
		       ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid), 101),'') AS lastAssessedDate, 
		       ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
		       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From vw_EmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
		       ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid) AS VARCHAR),'') AS RatingCompleted,
		      0 AS ord, ISNULL(e.IsMailSent,0) AS IsMailSent, e.currentsalary,'' as managerLastAssessedDate
		FROM employeedetails e
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN vw_EmpAverageAssessmentDet vg ON vg.empId = e.empid AND year = @curryear
		WHERE e.companyid = @companyid AND e.bactive = 1 AND e.roleid <> 4 AND e.empstatus = 1
			AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
		ORDER BY e.empid DESC, e.lastname
END

ELSE 
BEGIN
	SELECT e.empid, e.employeeId, e.companyid, cd.companyName, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, 
		   e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate, e.departmentid, 
		   d.department, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid,
		   '' AS lastAssessedDate, 0 as AvgScore, '' AS AvgScoreRank, '' AS lastScoreRank, '' AS RatingCompleted,
		   0 AS ord, e.IsMailSent, e.currentsalary,'' as managerLastAssessedDate
	FROM employeedetails e
	LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
	LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
	LEFT JOIN companydetails cd ON cd.compid = e.companyId AND cd.bactive = 1
	WHERE e.bactive = 1 AND cd.bactive = 1 and e.roleid in (2)
	AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
	ORDER BY e.empid DESC, cd.companyName, e.lastname
END
END
----------
CREATE OR ALTER   PROCEDURE [dbo].[usp_GetYearWiseAllEmployeesWithoutPagination]
(
	@yearID INT,
	@companyid INT,
	@managerid INT,
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

CREATE TABLE #tempEmpAverageAssessmentDet (
	companyid INT, empid INT, avgScore DECIMAL(2,0), AvgScoreRank VARCHAR(100), yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	SELECT * INTO #tempScore
	FROM
	(
		SELECT companyID, empid, Score, ManagerId,  FORMAT(RatingDate, 'yyyyMM') AS monyrID
		FROM vw_EmpAssessmentScore
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear  AND empStatus = 1 
	)t

	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT  companyID, empId, avgscore, avgScoreRank, 0 AS YearID
		FROM
		(
			SELECT companyID, empId, CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS DECIMAL(10,0)) AS avgscore,
			dbo.GetScoreRank(CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS decimal(10,0))) AS AvgScoreRank,
			row_number() OVER (PARTITION BY empid ORDER BY empid) AS rownumber
			FROM #tempScore 
		)v WHERE rownumber = 1
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT companyID , empID, ISNULL(AvgScore,0) AS avgScore, ISNULL(AvgScoreRank, '') AS AvgScoreRank, year AS YearID FROM vw_EmpAverageAssessmentDet 
		WHERE companyid = @companyId AND year = @YearId
	)k
	END

	CREATE TABLE #tempEmpLastAssessmentDet (
		companyid INT, empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		--WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END

SET @YearID = CASE WHEN @YearID = 0 THEN YEAR(GETDATE()) ELSE @YearID END

IF(@managerid > 0 and @companyid > 0) 
BEGIN  
	IF([dbo].[GetRoleByEmpid](@managerid) IN (2,4))
	BEGIN
	
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, 		       CONVERT(DATE, e.joiningdate,101) AS joiningdate,
		       e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, 
		       ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, e.currentsalary,
		       ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
		       ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore, 0) AS AvgScore,
		       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
		       ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted, 0 AS ord,
			   ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM EmployeeDetails e
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid
		LEFT JOIN GetLastAssessmentDateByManager(ABS(@managerid)) assmnt on assmnt.empid=e.empid
		WHERE e.companyid = @companyid AND e.bactive = 1 AND e.managerid = @managerid  AND Format(e.joiningDate, 'yyyy') <= @YearID
		AND e.empstatus = 1 AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
		ORDER BY e.empid DESC, e.lastname

	END
	ELSE
	BEGIN
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
			   e.departmentid, d.department,  ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, e.currentsalary,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore, 0) AS AvgScore,
		       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted,
			   0 AS ord,ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM EmployeeDetails e 
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid --AND vg.year = DATEPART(yyyy,GETDATE())
		LEFT JOIN GetLastAssessmentDateByManager(ABS(@managerid)) assmnt on assmnt.empid=e.empid
		WHERE e.companyid = @companyid AND e.bactive = 1 AND e.managerid = @managerid  AND Format(e.joiningDate, 'yyyy') <= @YearID
		AND e.empstatus = 1 AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist)) 
		ORDER BY e.empid DESC, e.lastname
	END
END


ELSE if (@managerid = 0 and @companyid > 0) 
BEGIN

		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, 		       CONVERT(DATE, e.joiningdate,101) AS joiningdate,
		       e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, 
		       ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, e.currentsalary,
		       ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
		       ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
		       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
		       ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted, 0 AS ord,
			   '' as managerLastAssessedDate
		FROM employeedetails e
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid
		WHERE e.companyid = @companyid AND e.bactive = 1 AND e.roleid <> 4 AND e.empstatus = 1 AND Format(e.joiningDate, 'yyyy') <= @YearID
		AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
		ORDER BY e.empid DESC, e.lastname
	
END
END
----------
CREATE OR ALTER   PROCEDURE [dbo].[GetYearWiseAllEmpDetByManagerWithoutPagination] 
(
	@yearID INT,
	@companyid INT,
	@managerid INT,
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d;
	
	DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

	CREATE TABLE #tempEmpAverageAssessmentDet (
	companyid INT, empid INT, avgScore DECIMAL(2,0), AvgScoreRank VARCHAR(100), yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	SELECT * INTO #tempScore
	FROM
	(
		SELECT companyID, empid, Score, ManagerId,  FORMAT(RatingDate, 'yyyyMM') AS monyrID
		FROM vw_EmpAssessmentScore
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear AND empStatus = 1 
	)t

	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT  companyID, empId, avgscore, avgScoreRank, 0 AS YearID
		FROM
		(
			SELECT companyID, empId, CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS DECIMAL(10,0)) AS avgscore,
			dbo.GetScoreRank(CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS decimal(10,0))) AS AvgScoreRank,
			row_number() OVER (PARTITION BY empid ORDER BY empid) AS rownumber
			FROM #tempScore 
		)v WHERE rownumber = 1
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT companyID , empID, avgScore, AvgScoreRank, year AS YearID FROM vw_EmpAverageAssessmentDet 
		WHERE companyid = @companyId AND year = @YearId
	)k
	END

	CREATE TABLE #tempEmpLastAssessmentDet (
	companyid INT, empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		--WHERE FORMAT(RatingDate, 'yyyyMM') >= @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END

	SET @YearID = CASE WHEN @YearID = 0 THEN YEAR(GETDATE()) ELSE @YearID END;

	WITH #tempEmployeedetails  (empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
		 departmentid, managerid,  empstatus,  roleid, currentsalary)
	AS
	(
	SELECT empid,  employeeId,  companyid,  firstname, ISNULL(middlename, '') AS middlename,  lastname,  email, ISNULL( jobtitle,'') AS jobtitle, 
	       CONVERT(DATE,  joiningdate,101) AS joiningdate,
	       departmentid, ISNULL(managerid,0) AS managerid,  empstatus,  roleid, currentsalary
	FROM employeedetails AS FirstLevel
	WHERE bactive = 1 AND empstatus = 1 AND managerID = @managerid 
	AND companyId = @companyId 
	UNION ALL
	SELECT NextLevel.empid, NextLevel.employeeId, NextLevel.companyid, NextLevel.firstname, ISNULL(NextLevel.middlename, '') AS middlename, NextLevel.lastname, 	       NextLevel.email, ISNULL(NextLevel.jobtitle,'') AS jobtitle, CONVERT(DATE, NextLevel.joiningdate,101) AS joiningdate,
	       NextLevel.departmentid, ISNULL(NextLevel.managerid,0) AS managerid, NextLevel.empstatus, NextLevel.roleid, NextLevel.currentsalary
	FROM employeedetails AS NextLevel
	INNER JOIN #tempEmployeedetails AS t ON NextLevel.managerid = t.empId 
	WHERE NextLevel.bactive = 1 AND NextLevel.empstatus = 1 
		  AND NextLevel.companyid = @companyid 
	)SELECT * INTO #tempEmployeedetails 
    FROM #tempEmployeedetails WHERE Format(joiningDate, 'yyyy') <= @YearID
  AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR departmentId IN (SELECT department FROM #departmentlist))
  	
IF(@managerid > 0 and @companyid > 0) 
BEGIN  	

	
	SELECT e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
	       e.departmentid, e.managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, d.department,                e.currentsalary,
	       ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
	       ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
	       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
	       ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted, 0 AS ord,
		   ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
	FROM #tempEmployeedetails e
	LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
	LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
	LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid 
	LEFT JOIN GetLastAssessmentDateByManager(ABS(@managerid)) assmnt on assmnt.empid=e.empid
	ORDER BY e.empid DESC,  e.lastname
END
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetCompanyWiseEmployeeListWithoutPagination]
(
	@companyid INT,
	@departmentlist VARCHAR(256)
)
AS
BEGIN

SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

DECLARE @curryear INT = DATEPART(yyyy,GETDATE());


		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate, 
			   ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName,
			   e.workcity, e.workstate, e.workzipcode,ISNULL(e.suffix, '') AS suffix,  e.departmentid, d.department,  ISNULL(e.managerid,0) AS managerid , 			   e.empstatus, e.roleid,
			   '' AS role, CONVERT(DATE, e.dateofbirth,101) AS dateofbirth, e.gender, ISNULL(e.jobcategoryid,0) AS jobcategoryid, 
			   ISNULL(e.jobcategory,'') AS jobcategory, 
			   ISNULL(e.jobgroupId,0) AS jobgroupId, ISNULL(e.jobgroup, '') AS jobgroup, ISNULL(e.jobcodeId,0) AS jobcodeId, 
			   ISNULL(e.jobcode,'') AS jobcode, CONVERT(DATE, e.lastpromodate ,101) AS lastpromodate, CONVERT(DATE, e.lastincdate,101) AS lastincdate,
			   ISNULL(e.raceorethanicityid, 0) AS raceorethanicityId, '' AS raceorethanicity, e.currentsalary,  ISNULL(e.countryId, 0) AS countryId, 
			   '' AS country, 
			   e.emplocation, ISNULL(e.regionid, 0) AS regionid ,'' AS region, ISNULL(e.empimgpath,'') AS empimgpath,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessmentDate FROM employee_assessment WHERE empid = e.empid), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
			   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From vw_EmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = DATEPART(yyyy, GETDATE())) AS VARCHAR),'') AS RatingCompleted,
			   ISNULL(e.IsMailSent,0) AS IsMailSent
		FROM employeedetails e 
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN vw_EmpAverageAssessmentDet vg ON vg.empId = e.empid AND year = DATEPART(yyyy,GETDATE()) 
		WHERE e.companyid = @companyid AND e.bactive = 1 
		AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
		ORDER BY e.empid DESC, e.lastname

END
----------
CREATE OR  ALTER   PROCEDURE [dbo].[usp_GetYearlyAllEmployeesWithGradeByManagerWithoutPagination] 
(
	@yearID INT,
	@companyid INT,
	@managerid INT,
	@grade VARCHAR(20),
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

DECLARE @LastYear INT = FORMAT(GETDATE(), 'yyyy') - 1
DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

CREATE TABLE #tempEmpAverageAssessmentDet (
	companyid INT, empid INT, avgScore DECIMAL(2,0), AvgScoreRank VARCHAR(100), yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	SELECT * INTO #tempScore
	FROM
	(
		SELECT companyID, empid, empName, Score, ManagerId,  FORMAT(RatingDate, 'yyyyMM') AS monyrID
		FROM vw_EmpAssessmentScore
		WHERE FORMAT(RatingDate, 'yyyyMM') > FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')  AND empStatus = 1 
	)t

	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT  companyID, empId, avgscore, avgScoreRank, 0 AS YearID
		FROM
		(
			SELECT companyID, empId, empName, CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS DECIMAL(10,0)) AS avgscore,
			dbo.GetScoreRank(CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS decimal(10,0))) AS AvgScoreRank,
			row_number() OVER (PARTITION BY empid ORDER BY empid) AS rownumber
			FROM #tempScore 
		)v WHERE rownumber = 1
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT companyID , empID, avgScore, AvgScoreRank, year AS YearID FROM vw_EmpAverageAssessmentDet 
		WHERE companyid = @companyId AND year = @YearId
	)k
	END


	CREATE TABLE #tempEmpAssessmentScore (
	companyid INT, empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END

	SET @YearId = CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END

IF(@managerid > 0 and @companyid > 0 AND @Grade <> '') 
BEGIN  

WITH #tempEmployeedetailsMC  (empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
	 departmentid, managerid,  empstatus,  roleid, currentsalary)
	AS
	(
	SELECT empid,  employeeId,  companyid,  firstname, ISNULL(middlename, '') AS middlename,  lastname,  email, ISNULL( jobtitle,'') AS jobtitle, CONVERT(DATE,  joiningdate,101) AS joiningdate,
		   departmentid, ISNULL(managerid,0) AS managerid,  empstatus,  roleid, currentsalary
	FROM employeedetails AS FirstLevel
	WHERE bactive = 1 AND empstatus = 1 AND managerID = @Managerid 
	AND companyId = @companyId
	UNION ALL
	SELECT NextLevel.empid, NextLevel.employeeId, NextLevel.companyid, NextLevel.firstname, ISNULL(NextLevel.middlename, '') AS middlename, NextLevel.lastname, NextLevel.email, ISNULL(NextLevel.jobtitle,'') AS jobtitle, CONVERT(DATE, NextLevel.joiningdate,101) AS joiningdate,
		   NextLevel.departmentid, ISNULL(NextLevel.managerid,0) AS managerid, NextLevel.empstatus, NextLevel.roleid, NextLevel.currentsalary
	FROM employeedetails AS NextLevel
	INNER JOIN #tempEmployeedetailsMC AS t ON NextLevel.managerid = t.empId 
	WHERE NextLevel.bactive = 1 AND NextLevel.empstatus = 1 AND NextLevel.companyid = @companyid
	)SELECT * INTO #tempEmployeedetailsMC 
    FROM #tempEmployeedetailsMC WHERE Format(joiningDate, 'yyyy') <= @YearID
	AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR departmentId IN (SELECT department FROM #departmentlist))



		SELECT empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
			   departmentid, department, managerid, managerFName, managerLName, empstatus,  roleid, currentsalary, lastAssessedDate, AvgScoreRank, AvgScore,
			   LastScoreRank, RatingCompleted,managerLastAssessedDate
		FROM(
				SELECT e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
					   e.departmentid, d.department,  e.managerid , ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName,  e.empstatus, e.roleid, e.currentsalary,
					   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
					   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
					   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpAssessmentScore WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
					   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted
					   ,0 AS ord,ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
				FROM #tempEmployeedetailsMC e 
				LEFT JOIN employeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
				LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
				LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid AND e.empstatus = 1
				LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
				
		)s
		WHERE CASE WHEN dbo.Get_grade_value(LastScoreRank) IS NULL THEN '' ELSE dbo.Get_grade_value(LastScoreRank) END = @Grade
		ORDER BY lastname

	END
END

----------
CREATE OR ALTER   PROCEDURE [dbo].[usp_GetYearlyAllEmployeesWithGradeWithoutPagination] 
(
	@yearID INT,
	@companyid INT,
	@managerid INT,
	@grade VARCHAR(20),
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

	DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

	CREATE TABLE #tempEmpAverageAssessmentDet (
	companyid INT, empid INT, avgScore DECIMAL(2,0), AvgScoreRank VARCHAR(100), yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	SELECT * INTO #tempScore
	FROM
	(
		SELECT companyID, empid, Score, ManagerId,  FORMAT(RatingDate, 'yyyyMM') AS monyrID
		FROM vw_EmpAssessmentScore
		WHERE FORMAT(RatingDate, 'yyyyMM') > FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')  AND empStatus = 1 
	)t

	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT  companyID, empId, avgscore, avgScoreRank, 0 AS YearID
		FROM
		(
			SELECT companyID, empId, CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS DECIMAL(10,0)) AS avgscore,
			dbo.GetScoreRank(CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS decimal(10,0))) AS AvgScoreRank,
			row_number() OVER (PARTITION BY empid ORDER BY empid) AS rownumber
			FROM #tempScore 
		)v WHERE rownumber = 1
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT companyID , empID, avgScore, AvgScoreRank, year AS YearID FROM vw_EmpAverageAssessmentDet 
		WHERE companyid = @companyId AND year = @YearId
	)k
	END

	CREATE TABLE #tempEmpLastAssessmentDet (
	companyId INT,empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END
	SET @YearId = CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END

IF(@managerid > 0 and @companyid > 0 AND @Grade <> '') 
BEGIN  


		SELECT empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
			   departmentid, managerid,  empstatus,  roleid, department, managerFName, managerLName, currentsalary,empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   lastScoreRank, RatingCompleted,managerLastAssessedDate
		FROM(
				SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename,  e.lastname, e.email, 
				       ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE,  e.joiningdate,101) AS joiningdate,
				       e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, e.empstatus, e.roleid, 
				       ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath,
				       ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
				       ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
				       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
				       ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted,0 AS ord,
					   ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
				FROM employeedetails e 
				LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
				LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
				LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid 
				LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
				WHERE e.companyid = @companyid AND e.bactive = 1 AND e.managerid = @managerid AND Format(e.joiningDate, 'yyyy') <= @YearID
				AND e.empstatus = 1 AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
		) v
		WHERE CASE WHEN dbo.Get_grade_value(LastScoreRank) IS NULL THEN '' ELSE dbo.Get_grade_value(LastScoreRank) END = @Grade
		ORDER BY empid DESC, lastname
END
ELSE if (@managerid = 0 and @companyid > 0 AND @Grade <> '') 
BEGIN

	
		SELECT empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
			   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary, empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   LastScoreRank, RatingCompleted
		FROM(
			SELECT e.empid, e.employeeId, e.companyid, e.firstname,  ISNULL(e.middlename, '') AS middlename,   e.lastname, e.email, 
			       ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE,  e.joiningdate,101) AS joiningdate,
			       e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, 
			       ISNULL(m.lastname, '') AS managerLName,  e.empstatus, e.roleid, e.currentsalary, ISNULL(e.empimgpath,'') AS empimgpath,
			       ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			       ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
			       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
			       ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted, 0 AS ord
				   
			FROM employeedetails e 
			LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
			LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
			LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid 
			WHERE e.companyid = @companyid AND e.bactive = 1  AND Format(e.joiningDate, 'yyyy') <= @YearID
			AND e.empstatus = 1 AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
			GROUP BY e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
					 e.departmentid, d.department, e.managerid, ISNULL(m.firstname, ''), ISNULL(m.lastname, ''), e.empstatus, e.roleid,
					 ISNULL(vg.AvgScoreRank, ''),vg.AvgScore, e.currentsalary ,ISNULL(e.empimgpath,'')
					 
		) v
		WHERE CASE WHEN dbo.Get_grade_value(LastScoreRank) IS NULL THEN '' ELSE dbo.Get_grade_value(LastScoreRank) END = @Grade	
		ORDER BY empid DESC, lastname

END
ELSE 
BEGIN

	SELECT e.empid, e.employeeId, e.companyid, cd.companyName, e.firstname, e.middlename, e.lastname, e.email, jobtitle, joiningdate,
	       e.departmentid, d.department, e.managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath,
	       '' AS lastAssessedDate, 0 as AvgScore, '' AS AvgScoreRank, '' AS LastScoreRank, '' AS RatingCompleted,0 AS ord
	FROM employeedetails e
	LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
	LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
	LEFT JOIN companydetails cd ON cd.compid = e.companyId AND cd.bactive = 1
	WHERE cd.bactive = 1 and e.roleid =2
		AND e.bactive = 1 AND cd.bactive = 1 and e.roleid =2
		AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
	ORDER BY e.empid DESC, cd.companyName, e.firstname
			
END
END


----------
CREATE OR  ALTER   PROCEDURE [dbo].[usp_GetYearlyMonAvgScoreWiseEmpWithoutPagination] 
(
	@yearId INT,
	@companyId INT,
	@managerId INT,
	@grade VARCHAR(10),
	@month VARCHAR(10),
	@departmentlist VARCHAR(256)
)
AS 
BEGIN
SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

DECLARE @CurrYear INT = FORMAT(GETDATE(), 'yyyy')
DECLARE @LastYear INT = FORMAT(GETDATE(), 'yyyy') - 1
DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

CREATE TABLE #tempEmpAssessmentScore (
	companyid INT, empid INT, RatingDate DATETIME, Score DECIMAL(2,0), managerId INT, yearId INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyid, empid,  RatingDate, score, managerid, YearID FROM vw_EmpAssessmentScore 
		WHERE  empStatus = 1 AND FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear  AND companyid = @companyId
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyid, empid, RatingDate, score, managerid, YearID FROM vw_EmpAssessmentScore 
		WHERE  empStatus = 1 AND yearid = @YearId  AND companyid = @companyId
	)k
	END

	CREATE TABLE #tempEmpLastAssessmentDet (
	companyid INT, empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END

	SELECT avgscore, ScoreRank, monyrid, monyr,empid,managerid, companyid INTO #temp FROM
	(
		SELECT avgscore, dbo.get_grade_value(dbo.GETSCoreRank(avgscore)) AS ScoreRank, monyrid, monyr,empid,managerid, companyid
		FROM (
				SELECT DISTINCT CAST(ROUND(AVG(score) OVER (PARTITION BY empid, FORMAT(RatingDate,'yyyyMM')),0) AS DECIMAL(10,0)) AS avgscore,  empid,  
					   FORMAT(RatingDate,'yyyyMM') AS monyrid, FORMAT(RatingDate,'MMM') AS monyr, managerid, companyid
				FROM #tempEmpAssessmentScore 
		) v
	) m


	CREATE TABLE #monthlist (yearmonth VARCHAR(20), monthname VARCHAR(20));

	IF(@YearId = 0)
	BEGIN
			INSERT INTO #monthlist
			SELECT yearmonth, monthname FROM
			(
				SELECT Format(getdate(),'yyyy')+FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MM') AS yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number between 1 and MONTH(GETDATE())
				UNION		
				SELECT  CAST(@LastYear AS VARCHAR) + FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@LastYear AS VARCHAR) AS DATE), 'MM') AS yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number > MOnth(DATEADD(month,-12,GETDATE()))
			) v
		END
		ELSE
		BEGIN
			INSERT INTO #monthlist
			SELECT  yearmonth, monthname  
			FROM
			(
				SELECT CAST(@yearID AS VARCHAR) + FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@yearID AS VARCHAR) AS DATE), 'MM') AS yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@yearID AS VARCHAR) AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number between 1 and CASE WHEN @CurrYear <> @YearId THEN MONTH(DATEFROMPARTS(@yearID,12,31)) ELSE MONTH(GETDATE()) END
			)v
		END

	SELECT companyid, avgscore, ScoreRank, yearmonth AS monyrid, monthname  AS monyr, empid, managerid 
	INTO #tempScore 
	FROM #monthlist m 
	LEFT OUTER JOIN #temp s ON s.monyrid = (SELECT MAX(monyrid) FROM #temp WHERE monyrid < yearmonth AND companyid = s.companyid AND empid = s.empid)
	WHERE yearmonth NOT IN(SELECT monyrid FROM #temp WHERE monyrid <> '' AND companyid = s.companyid AND empid = s.empid) AND companyid = s.companyid AND empid = s.empid
	UNION
	SELECT companyid, avgscore, ScoreRank, monyrid, monyr, empid, managerid from #temp WHERE monyrid <> '' 
	
SET @YearId = CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END
IF(@ManagerId > 0 AND @CompanyId > 0) 
BEGIN
	
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, 		       CONVERT(DATE,  e.joiningdate,101) AS joiningdate,
		       e.departmentid, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, 		       e.empstatus, e.roleid, d.department, e.currentsalary,
		       ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
		      ISNULL(vg.ScoreRank, '') AS AvgScoreRank, ISNULL(vg.avgscore,0) AS AvgScore,
		      ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
		      ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearId) AS VARCHAR),'') AS RatingCompleted,0 AS ord,
			  ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate

		FROM employeedetails e 
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempScore vg ON vg.empId = e.empid AND e.empstatus = 1 AND vg.monyr = @Month
		LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
		WHERE CASE WHEN ScoreRank IS NULL THEN '' ELSE ScoreRank END = @Grade
			AND e.companyid = @companyid AND e.bactive = 1 AND e.managerid = @managerid
			AND Format(e.joiningDate, 'yyyy') <= @YearId
			AND e.empstatus = 1 AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
		ORDER BY e.lastname
END
ELSE
BEGIN
	
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
			   e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, 
			   ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, e.currentsalary,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.ScoreRank, '') AS AvgScoreRank, ISNULL(vg.avgscore,0) AS AvgScore,
			   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearId) AS VARCHAR),'') AS RatingCompleted,0 AS ord
		FROM employeedetails e 
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempScore vg ON vg.empId = e.empid AND e.empstatus = 1 AND vg.monyr = @Month
		WHERE CASE WHEN ScoreRank IS NULL THEN '' ELSE ScoreRank END = @Grade
			AND e.companyid = @companyid AND e.bactive = 1 AND e.empstatus = 1
			AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
		ORDER BY e.lastname
	
END
END

----------
CREATE OR ALTER   PROCEDURE [dbo].[usp_GetYearlyMonAvgScoreWiseEmpByManagerWithoutPagination] 
(
	@yearId INT,
	@companyId INT,
	@managerId INT,
	@grade VARCHAR(10),
	@month VARCHAR(10),
	@departmentlist VARCHAR(256)
)
AS 
BEGIN

SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d;

DECLARE @CurrYear INT = FORMAT(GETDATE(), 'yyyy')
DECLARE @LastYear INT = FORMAT(GETDATE(), 'yyyy') - 1
DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

CREATE TABLE #tempEmpAssessmentScore (
	companyid INT, empid INT, RatingDate DATETIME, Score DECIMAL(2,0), managerId INT, yearId INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyid, empid, RatingDate, score, managerid, YearID  FROM vw_EmpAssessmentScore 
		WHERE  empStatus = 1 AND FORMAT(RatingDate, 'yyyyMM') >= @Last12MonthYear  AND companyid = @companyId
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyid, empid, RatingDate, score, managerid, YearID  FROM vw_EmpAssessmentScore 
		WHERE  empStatus = 1 AND yearid = @YearId  AND companyid = @companyId
	)k
	END

	CREATE TABLE #tempEmpLastAssessmentDet (
	companyid INT, empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), GeneralScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);


	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, GeneralScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		WHERE FORMAT(RatingDate, 'yyyyMM') >= @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, GeneralScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END


SELECT * INTO #temp FROM
(
	SELECT avgscore, dbo.get_grade_value(dbo.GETSCoreRank(avgscore)) AS ScoreRank, monyrid, monyr,empid,managerid, companyid
	FROM (
			SELECT distinct CAST(ROUND(AVG(score) OVER (Partition BY empid, FORMAT(RatingDate,'yyyyMM')),0) AS decimal(10,0)) as avgscore,  empid,  
				 FORMAT(RatingDate,'yyyyMM') as monyrid, FORMAT(RatingDate,'MMM') as monyr, managerid, companyid
				 FROM #tempEmpAssessmentScore 
	) v
) m

CREATE TABLE #monthlist (yearmonth VARCHAR(20), monthname VARCHAR(20));

	IF(@YearId = 0)
	BEGIN
			INSERT INTO #monthlist
			SELECT yearmonth, monthname FROM
			(
				SELECT Format(getdate(),'yyyy')+FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MM')yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number between 1 and MONTH(GETDATE())
				UNION		
				SELECT  CAST(@LastYear AS VARCHAR) + FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@LastYear AS VARCHAR) AS DATE), 'MM')yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number > MOnth(DATEADD(month,-12,GETDATE()))
			) v
		END
		ELSE
		BEGIN
			INSERT INTO #monthlist
			SELECT  yearmonth, monthname  
			FROM
			(
				SELECT CAST(@yearID AS VARCHAR) + FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@yearID AS VARCHAR) AS DATE), 'MM')AS yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@yearID AS VARCHAR) AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number between 1 and CASE WHEN @CurrYear <> @YearId THEN MONTH(DATEFROMPARTS(@yearID,12,31)) ELSE MONTH(GETDATE()) END
			)v
		END

	SELECT companyid, avgscore, ScoreRank, yearmonth AS monyrid, monthname  AS monyr, empid, managerid INTO #tempScore
	FROM #monthlist m LEFT OUTER JOIN #temp s on s.monyrid = (SELECT MAX(monyrid) FROM #temp WHERE monyrid < yearmonth and companyid = s.companyid and empid = s.empid)
	WHERE yearmonth NOT IN(SELECT monyrid FROM #temp WHERE monyrid <> '' and companyid = s.companyid and empid = s.empid) and companyid = s.companyid and empid = s.empid
	UNION
	SELECT companyid, avgscore, ScoreRank, monyrid, monyr, empid, managerid from #temp WHERE monyrid <> '' 
	
	SET @YearId = CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END

IF(@ManagerId > 0 AND @CompanyId > 0) 
BEGIN
	WITH #tempEmployeedetailsMC  (empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
		  departmentid, managerid,  empstatus,  roleid, currentsalary)
	AS
	(
	SELECT empid,  employeeId,  companyid,  firstname, ISNULL(middlename, '') AS middlename,  lastname,  email, ISNULL( jobtitle,'') AS jobtitle, CONVERT(DATE,  joiningdate,101) AS joiningdate,
		  departmentid, ISNULL(managerid,0) AS managerid,  empstatus,  roleid, currentsalary
	FROM employeedetails AS FirstLevel
	WHERE bactive = 1 AND empstatus = 1 AND managerID = @ManagerId 
	AND companyId = @companyId 
	UNION ALL
	SELECT NextLevel.empid, NextLevel.employeeId, NextLevel.companyid, NextLevel.firstname, ISNULL(NextLevel.middlename, '') AS middlename, NextLevel.lastname, NextLevel.email, ISNULL(NextLevel.jobtitle,'') AS jobtitle, CONVERT(DATE, NextLevel.joiningdate,101) AS joiningdate,
		   NextLevel.departmentid, ISNULL(NextLevel.managerid,0) AS managerid, NextLevel.empstatus, NextLevel.roleid, NextLevel.currentsalary
	FROM employeedetails AS NextLevel
	INNER JOIN #tempEmployeedetailsMC AS t ON NextLevel.managerid = t.empId 
    WHERE NextLevel.bactive = 1 AND NextLevel.empstatus = 1 
		AND NextLevel.companyid = @companyid 
	)SELECT * INTO #tempEmployeedetailsMC 
    FROM #tempEmployeedetailsMC WHERE Format(joiningDate, 'yyyy') <= @YearID
  AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR departmentId IN (SELECT department FROM #departmentlist))


		SELECT e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
			   e.departmentid, d.department, e.managerid , ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, e.currentsalary,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.ScoreRank, '') AS AvgScoreRank, ISNULL(vg.avgscore,0) AS AvgScore,
			   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted
			   ,0 AS ord,ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM #tempEmployeedetailsMC e 
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempScore vg ON vg.empId = e.empid AND e.empstatus = 1 AND vg.monyr = @Month
		LEFT JOIN GetLastAssessmentDateByManager(ABS(@managerid)) assmnt on assmnt.empid=e.empid
		WHERE CASE WHEN ScoreRank IS NULL THEN '' ELSE ScoreRank END = @Grade 
		ORDER BY e.lastname
END
END
----------
CREATE OR ALTER    PROCEDURE [dbo].[usp_AddDimensionElements] 
(
@dimensionId SMALLINT,
@dimensionValues VARCHAR(50),
@createdby INT,
@result INT OUT
) 
AS
BEGIN
 IF EXISTS(SELECT * FROM [dbo].[DimensionElements] WHERE dimensionId=@dimensionId AND dimensionValues = @dimensionValues AND bactive = 0)
 BEGIN
	  UPDATE DimensionElements SET bActive = 1 WHERE dimensionId = @dimensionId AND dimensionValues=@dimensionValues
	  SET @result=1
	  SELECT @result
 END
 ELSE IF EXISTS(SELECT * FROM [dbo].[DimensionElements] WHERE dimensionValues = @dimensionValues AND bactive = 1)
 BEGIN
	  SET @result=0
	  SELECT @result;
 END
 ELSE
  IF NOT EXISTS(SELECT * FROM [dbo].[DimensionElements] WHERE dimensionId=@dimensionId AND dimensionValues = @dimensionValues AND bactive = 1)
  BEGIN
	DECLARE @dimensionValueId INT

	SELECT @dimensionValueId = ISNULL(MAX(dimensionValueId),0) + 1 FROM [dbo].[DimensionElements] WHERE dimensionId = @dimensionId

	   INSERT INTO [DimensionElements]
	   (
		dimensionId,
		dimensionValueId,
		dimensionValues, 
		bactive,
		Createdby, 
		Createddtstamp  
		)
		VALUES 
	   (
		@dimensionId, 
		@dimensionValueId,
		@dimensionValues,
		1,
		@Createdby,
		GETDATE() 
	   );
	   SET @result = 1;
	   SELECT @result;
  END
  ELSE
   BEGIN
		SET @result=0
		SELECT @result;
   END
END

----------
CREATE	OR ALTER   PROCEDURE [dbo].[usp_GetDimensionsWiseElements]
AS
BEGIN
-- SET NOCOUNT ON added to prevent extra result sets from
-- interfering with SELECT statements.
SET NOCOUNT ON;

    -- Insert statements for procedure here
SELECT dv.id, dv.DimensionId, d.dimensionType,  dv.DimensionValueId, dv.DimensionValues, 
    CAST(CASE WHEN dv.DimensionId = 1 AND dv.DimensionValueId < 6 THEN 1 
    WHEN dv.DimensionId = 2 AND dv.DimensionValueId < 4 THEN 1 
	WHEN dv.DimensionId = 3 THEN 1 
	WHEN dv.DimensionId = 4 THEN 1 ELSE 0 END  AS BIT) AS  IsDefault
FROM DimensionElements dv
LEFT JOIN dimensions d ON d.bactive = 1 AND d.Id = dv.dimensionId
WHERE dv.bactive = 1 AND dv.id NOT IN (1,2,5)
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateDimensionElement] 
(
	@id INT,
	@dimensionId SMALLINT,
	@dimensionValues VARCHAR(50),
	@updatedby INT,
	@result INT OUTPUT
) 
AS
BEGIN
	IF NOT EXISTS(SELECT * FROM [dbo].[DimensionElements] WHERE id = @id)
	BEGIN
			SET @result = -2;
			SELECT @result;
	END
	ELSE IF EXISTS(SELECT * FROM [dbo].[DimensionElements] WHERE dimensionId = dimensionId AND dimensionValues = @dimensionValues AND id <> @id AND bactive = 1)
	BEGIN
			SET @result = 0;
			SELECT @result;
		
	END
	ELSE IF EXISTS(SELECT * FROM [dbo].[DimensionElements] WHERE dimensionId = @dimensionId AND dimensionValues = @dimensionValues AND id <> @id AND bactive = 0)
	BEGIN
			UPDATE dimensionElements 
			       SET bactive=1,
			       updatedby=@updatedby, 
			       updateddtstamp=GETDATE()    
			WHERE dimensionId = @dimensionId AND dimensionValues = @dimensionValues
			SET @result = 0;
			SELECT @result;
	END
	ELSE 
	BEGIN

		IF EXISTS( SELECT * FROM [dbo].[DimensionElements] WHERE id=@id AND dimensionId = @dimensionId AND bactive = 1)
		BEGIN 
			  UPDATE dimensionElements 
			     SET dimensionValues = @dimensionValues,
				 dimensionId = @dimensionId,
				 bactive=1,
				 updatedby=@updatedby, 
				 updateddtstamp=GETDATE()    
			  WHERE id = @id 
	     
			  SET @result = 1;
			  SELECT @result;
		END
	END
END
----------
CREATE OR ALTER   PROCEDURE [dbo].[usp_DeleteDimensionelement] 
(
  @Dimensionid INT, 
  @DimensionValueid INT, 
  @Updatedby  INT,
  @Result INT OUTPUT
)
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 -- Insert statements for procedure here
IF NOT EXISTS(SELECT * FROM [dbo].[DimensionElements] WHERE Dimensionid = @Dimensionid  AND DimensionValueid = @DimensionValueid)
BEGIN
  SET @result = -2; --Not exists
  SELECT @result;
END
ELSE IF EXISTS(SELECT * FROM [dbo].[DimensionElements] WHERE Dimensionid = @Dimensionid  AND DimensionValueid = @DimensionValueid AND bactive = 0)
BEGIN
  SET @result = -1; --already deleted
  SELECT @result;
END
ELSE
BEGIN
IF EXISTS (SELECT * FROM dimensionElements WHERE dimensionId = @Dimensionid AND DimensionValueid = @DimensionValueid AND bactive = 1)
BEGIN
	 IF EXISTS(SELECT 1 FROM DimensionWiseActionPermission e WHERE dimensionId = @Dimensionid AND DimensionValueid = @DimensionValueid AND (canView <> 0 OR canAdd <> 0 OR canEdit <> 0 OR canDelete <> 0))
	 OR EXISTS(SELECT * FROM  employeedetails e WHERE roleid = @DimensionValueid AND @Dimensionid = 1)
	 OR (1 = (SELECT CAST(CASE WHEN DimensionId = 1 AND DimensionValueId < 6 THEN 1 
	   WHEN DimensionId = 2 AND DimensionValueId < 4 THEN 1 ELSE 0 END  AS BIT) AS  IsDefault 
		FROM [DimensionElements] WHERE DimensionId = @Dimensionid AND DimensionValueId = @DimensionValueid))
	 BEGIN
	 SET @result = 0;
	 SELECT @result;  ---------Can't deleted 
	 END

ELSE
 BEGIN
 
		IF EXISTS(SELECT * FROM DimensionWiseActionPermission e WHERE dimensionId = @Dimensionid AND DimensionValueid = @DimensionValueid AND (CanView = 0 AND canAdd = 0 AND canEdit = 0 AND canDelete = 0))
		BEGIN
		DELETE FROM DimensionWiseActionPermission WHERE dimensionId = @dimensionId AND dimensionValueId = @dimensionValueId AND bactive = 1
		END

     UPDATE dimensionElements SET bactive  = 0,
	 updatedby = @updatedby,
	 updateddtstamp = GETDATE()
     WHERE dimensionId = @Dimensionid AND DimensionValueid = @DimensionValueid
     SET @result = 1;  ----Sucessfully  Deleted
     SELECT @result;
 END
 END
 END 
END
----------
CREATE  OR ALTER PROCEDURE [dbo].[usp_GetActionWisePermission] 
(
	@managerId INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @roleId INT
	SELECT @roleid = [dbo].[GetRoleByEmpid](@managerId)
	
    SELECT a.id AS actionId, a.actions, d.id AS dimensionId, d.dimensionType, dv.dimensionValueId AS dimensionValueId, dv.dimensionValues, 
		   dp.CanView, dp.CanAdd, dp.CanEdit, dp.CanDelete 
	FROM [dbo].[DimensionWiseActionPermission] dp
	LEFT JOIN actionmaster a ON a.bactive = 1 AND a.id = dp.actionId
	LEFT JOIN dimensions d ON d.bactive = 1 AND d.id = dp.dimensionId
	LEFT JOIN dimensionElements dv ON dv.bactive = 1 
		AND dv.dimensionId = dp.dimensionId AND dv.dimensionValueId = dp.dimensionvalueID
	WHERE dp.bactive =1 AND d.id = 1 AND dv.dimensionValueId = @roleid --AND a.Id = @actionId
	UNION
	SELECT a.id AS actionId, a.actions, d.id AS dimensionId, d.dimensionType, dv.dimensionValueId AS dimensionValueId, dv.dimensionValues, 
		   dp.CanView, dp.CanAdd, dp.CanEdit, dp.CanDelete 
	FROM [dbo].[DimensionWiseActionPermission] dp
	LEFT JOIN actionmaster a ON a.bactive = 1 AND a.id = dp.actionId
	LEFT JOIN dimensions d ON d.bactive = 1 AND d.id = dp.dimensionId
	LEFT JOIN dimensionElements dv ON dv.bactive = 1 
		AND dv.dimensionId = dp.dimensionId AND dv.dimensionValueId = dp.dimensionvalueID
	WHERE dp.bactive =1 AND d.id <> 1 AND (CanView <> 0 OR CanAdd <> 0 OR CanEdit <> 0 OR CanDelete <> 0)
	
END
----------
CREATE  OR ALTER PROCEDURE [dbo].[usp_GetDimensionsWiseActionPermission]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT a.id AS actionId, a.actions, d.id AS dimensionId, d.dimensionType, dv.dimensionvalueID AS dimensionValueId, dv.dimensionValues, 
	   dp.CanView, dp.CanAdd, dp.CanEdit, dp.CanDelete 
	FROM [dbo].[DimensionWiseActionPermission] dp
	LEFT JOIN actionmaster a ON a.bactive = 1 AND a.id = dp.actionId
	LEFT JOIN dimensions d ON d.bactive = 1 AND d.id = dp.dimensionId
	LEFT JOIN dimensionElements dv ON dv.bactive = 1 
		AND dv.dimensionId = dp.dimensionId AND dv.DimensionValueid = dp.dimensionvalueID
	WHERE dp.bactive = 1
END
----------
CREATE  OR ALTER   PROCEDURE [dbo].[usp_AddDimensionWiseActionPermission] 
(
	@dimensionId SMALLINT,
	@dimensionValueId VARCHAR(50),
	@actionId SMALLINT, 
	@canView  BIT,
	@canAdd   BIT,
	@canEdit  BIT,
	@canDelete BIT,
	@createdby INT,
	@result INT OUT
) 
AS
BEGIN
IF EXISTS(SELECT * FROM [dbo].[DimensionWiseActionPermission] WHERE dimensionId = @dimensionId AND dimensionValueId = @dimensionValueId AND actionId = @actionId AND bactive = 1)
BEGIN
	 DELETE FROM DimensionWiseActionPermission WHERE dimensionId = @dimensionId AND dimensionValueId = @dimensionValueId AND actionId = @actionId AND bactive = 1
END

	  INSERT INTO [DimensionWiseActionPermission]
	  (
		   dimensionId,
		   dimensionValueId, 
		   actionId, 
		   canView,
		   canAdd,
		   canEdit,
		   canDelete,
		   bactive,
		   createdby, 
		   createddtstamp  
	   )
	   VALUES 
	  (
		   @dimensionId, 
		   @dimensionValueId,
		   @actionId, 
		   @canView,
		   @canAdd,
		   @canEdit,
		   @canDelete,
		   1,
		   @createdby,
		   GETDATE() 
	  );
	  SET @result = 1;
	
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetAllEmployeesHierachyWithPagination]
(
	@companyid INT,
	@managerid INT,
	@PageNumber INT,
	@PageSize   INT,
	@SearchString NVARCHAR(256),
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
SET NOCOUNT ON;

DECLARE @TotalRowCount INT

SET @managerid = ABS(@managerid)

SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

		
	;WITH #tempEmployeedetails  (empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
	 departmentid, managerid,  empstatus,  roleid, IsMailSent, currentsalary)
	AS
	(
	SELECT empid,  employeeId,  companyid,  firstname, ISNULL(middlename, '') AS middlename,  lastname,  email, ISNULL(jobtitle,'') AS jobtitle, CONVERT(DATE,  joiningdate,101) AS joiningdate,
		   departmentid, ISNULL(managerid,0) AS managerid,  empstatus,  roleid, IsMailSent, currentsalary 
	FROM employeedetails AS FirstLevel
	WHERE bactive = 1 AND managerID = @Managerid 
	AND companyId = @companyId
	UNION ALL
	SELECT NextLevel.empid, NextLevel.employeeId, NextLevel.companyid, NextLevel.firstname, ISNULL(NextLevel.middlename, '') AS middlename, NextLevel.lastname, NextLevel.email, ISNULL(NextLevel.jobtitle,'') AS jobtitle, CONVERT(DATE, NextLevel.joiningdate,101) AS joiningdate,
		   NextLevel.departmentid, ISNULL(NextLevel.managerid,0) AS managerid, NextLevel.empstatus, NextLevel.roleid, NextLevel.IsMailSent, NextLevel.currentsalary
	FROM employeedetails AS NextLevel
	INNER JOIN #tempEmployeedetails AS t ON NextLevel.managerid = t.empId 
	WHERE NextLevel.bactive = 1 AND NextLevel.companyid = @companyid
	)SELECT * INTO #tempEmployeedetails 
    FROM #tempEmployeedetails WHERE (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR departmentId IN (SELECT department FROM #departmentlist))

	SELECT * INTO #tempEmployeeResult
	FROM
	(
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
			   e.departmentid, d.department, e.managerid, e.empstatus, e.roleid,
			   CASE WHEN e.managerid = @managerid THEN 0 ELSE 1 END AS ord, IsMailSent, e.currentsalary 
		FROM #tempEmployeedetails e 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
	)er
		SELECT @TotalRowCount = COUNT(empid) FROM  #tempEmployeeResult
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount, empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
			  departmentid, department, managerid, empstatus, roleid,IsMailSent, currentsalary 
		FROM  #tempEmployeeResult
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY CASE WHEN managerid = @managerid THEN 0 ELSE 1 END, empid DESC, firstname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY 

END
----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GetAllEmployeeWithHierachy] 
(
	@managerId int,
	@companyid int
)
As 
BEGIN

SET @managerID = ABS(@ManagerId)

;WITH #tempEmployee (empid, employeeId, companyid, managerid, firstname, middlename, lastname, departmentId, email, roleid, empstatus)
AS (
	SELECT FirstLevel.empId, FirstLevel.employeeId, FirstLevel.companyId, FirstLevel.managerId, 
		   FirstLevel.firstname, FirstLevel.middlename, FirstLevel.lastname, FirstLevel.departmentId, FirstLevel.email,
		   FirstLevel.roleId, FirstLevel.empStatus
	FROM EmployeeDetails AS FirstLevel
	WHERE ManagerID=@managerId and companyid = @companyid AND bactive = 1
	UNION ALL
	SELECT NextLevel.empId, NextLevel.employeeId, NextLevel.companyId,  
			NextLevel.managerid, NextLevel.firstname, NextLevel.middlename, NextLevel.lastname, 
			NextLevel.departmentId, NextLevel.email, NextLevel.roleId,
			NextLevel.empStatus
	FROM [dbo].EmployeeDetails AS NextLevel
	INNER JOIN #tempEmployee AS t ON NextLevel.managerId = t.empId
	WHERE NextLevel.companyId = @companyid AND NextLevel.bactive = 1
	)SELECT * INTO #tempEmployee
FROM #tempEmployee 

IF(@managerid > 0 and @companyid > 0) 
BEGIN  
	
	SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, 
	       e.email, e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, e.empstatus, e.roleid, 
	       ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid), 101),'') AS lastAssessedDate, 
	      ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = DATEPART(yyyy, GETDATE())) AS VARCHAR),'') AS RatingCompleted
	FROM #tempEmployee e 
	LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
	WHERE e.companyid = @Companyid 
	ORDER BY e.firstname
END
END
----------
CREATE OR ALTER  PROCEDURE [dbo].[usp_GetAllEmpRole]
AS
BEGIN
	SELECT dimensionvalueid AS roleid, dimensionvalues AS role FROM [dbo].[dimensionelements] WHERE dimensionid=1 AND dimensionvalueid <> 1 AND bactive = 1
END
----------
CREATE  OR ALTER PROCEDURE [dbo].[usp_GetYearWiseHierachy] 
(
	@YearID INT,
	@managerId int,
	@companyid int, 
	@departmentlist VARCHAR(1000)
)
As 
BEGIN

	SELECT department INTO #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d;

	;WITH #tempEmployee (empid, companyid, managerid, firstname, middlename, lastname, departmentId, roleid, empstatus, JoiningYearId)
	AS (
		SELECT empId, companyId, managerId, 
			   FirstLevel.firstname,FirstLevel.middlename,FirstLevel.lastname,departmentId, roleId, empStatus,
		Format(joiningdate, 'yyyy') AS JoiningYearId
		FROM EmployeeDetails AS FirstLevel
		WHERE ManagerID=@managerId and companyid = @companyid AND bactive = 1 AND empStatus = 1
		UNION ALL
		SELECT NextLevel.empId, NextLevel.companyId,  
				NextLevel.managerid,NextLevel.firstname,NextLevel.middlename, NextLevel.lastname,NextLevel.departmentId, NextLevel.roleId,
				NextLevel.empStatus, Format(NextLevel.joiningdate, 'yyyy') AS JoiningYearId
		FROM [dbo].EmployeeDetails AS NextLevel
		INNER JOIN #tempEmployee AS t ON NextLevel.managerId = t.empId
		WHERE NextLevel.companyId = @companyid AND NextLevel.bactive = 1 AND NextLevel.empStatus = 1
		)SELECT * INTO #tempEmployee
	FROM #tempEmployee WHERE departmentId IN (SELECT department FROM #departmentlist)
	AND JoiningYearId <= CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END
	SELECT CompanyId, Empid,managerId,departmentid,roleid,empstatus FROM #tempEmployee
END
----------
CREATE OR ALTER   PROCEDURE [dbo].[usp_AddEmpAssessmentWithAttachement]
(
	    @empid INT,
        @assessmentdate DATETIME,
        @assessmentby INT,
		@remarks NVARCHAR(MAX),
		@DocumentName VARCHAR(1000),
		@CloudFilePath VARCHAR(500),
        @CreatedBy INT,
		@id INT OUT 
		
)	
AS
BEGIN
IF NOT EXISTS(SELECT * FROM employee_assessment WHERE empid=@empid AND assessmentby = @assessmentby AND  CONVERT(DATE,assessmentdate,110) = CONVERT(DATE,@assessmentdate,110))
	BEGIN
	-- Insert statements for procedure here
			INSERT INTO employee_assessment
			(
										 empid,
										 assessmentdate,
										 assessmentby,
										 remarks,
										 GeneralDocPath,
										 CloudFilePath,
										 bactive,
										 CreatedBy,
										 Createddtstamp
        
			 )
			 VALUES 
			(
										@empid,
										@assessmentdate,
										@assessmentby,
										@remarks,
										@DocumentName,
										@CloudFilePath,
										1,
										@CreatedBy,
										GETDATE()
        
			);

			 SET @id =SCOPE_IDENTITY() 
			SELECT @id;
	
	END
END
----------
CREATE  OR ALTER PROCEDURE [dbo].[usp_AddEmpAssessmentDetailsWithAttachment]  
  (
	 @assessmentid INT,
	 @questionid INT,
	 @answerid INT,
	 @remarks NVARCHAR(MAX),
	 @DocumentName VARCHAR(1000),
	 @CloudFilePath VARCHAR(500),
	 @Createdby INT,
	 @result INT OUT
 )
AS 
BEGIN 

IF NOT EXISTS(SELECT * FROM assessmentdetails WHERE assessmentid = @assessmentid AND questionid = @questionid)
	BEGIN
		INSERT INTO [dbo].[assessmentdetails]
		(
			assessmentid,
			questionid,
			answerid,
			remarks,
			DocumentPath,
			CloudFilePath,
			bactive,
			Createdby,
			Createddtstamp
		)
		VALUES
		(
			@assessmentid,
			@questionid,
			@answerid,
			@remarks,
			@DocumentName,
			@CloudFilePath,
			1,
			@Createdby,
			GETDATE()
		) 
	SET @result = 1
	SELECT @result
	END
	ELSE
		BEGIN
		SET @result =0 
		SELECT @result
		END
    
END
----------
CREATE  OR ALTER PROCEDURE [dbo].[usp_DeleteAssessmentAttachment]  
 (
	 @assessmentId INT,
	 @remarkId INT,
	 @updatedBy INT,
	 @CommentUpdDateTime DATETIME,
	 @result INT OUT
 )
AS 
BEGIN 

IF NOT EXISTS(SELECT * FROM Employee_Assessment WHERE id=@assessmentId AND AssessmentBy=@updatedBy)
BEGIN
	SET @result = -1
	SELECT @result
END
ELSE
IF @remarkId =0 
BEGIN
	UPDATE [dbo].[Employee_Assessment]
		SET GeneralDocPath='',
			UpdatedBy=@updatedBy,
			Updateddtstamp = @CommentUpdDateTime
		WHERE  ID=@assessmentId

		SET @result = 1
		SELECT @result
END
ELSE
 IF EXISTS(SELECT * FROM assessmentdetails WHERE assessmentid = @assessmentId AND questionid = @remarkId)
	BEGIN
		UPDATE [dbo].[assessmentdetails]
		SET DocumentPath='',
			UpdatedBy=@updatedBy,
			Updateddtstamp = @CommentUpdDateTime
		WHERE  assessmentid=@assessmentId
		AND questionid = @remarkId
		
		SET @result = 1
		SELECT @result
	END
	ELSE
		BEGIN
			SET @result =0 
			SELECT @result
		END
    
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateAssessmentComment]  
 (
	 @assessmentId INT,
	 @remarkId INT,
	 @remarks VARCHAR(MAX),
	 @DocumentName VARCHAR(1000),
	 @CloudFilePath VARCHAR(500),
	 @CommentUpdDateTime DATETIME,
	 @updatedBy INT,
	 @result INT OUT
 )
AS 
BEGIN 

IF NOT EXISTS(SELECT * FROM Employee_Assessment WHERE id=@assessmentId AND AssessmentBy=@updatedBy)
BEGIN
	SET @result = -1
	SELECT @result
END
ELSE
IF @remarkId =0 
BEGIN
	UPDATE [dbo].[Employee_Assessment]
		SET Remarks=@remarks,
			GeneralDocPath = @DocumentName,
			CloudFilePath=@CloudFilePath,
			UpdatedBy=@updatedBy,
			Updateddtstamp =@CommentUpdDateTime
		WHERE ID=@assessmentId

		SET @result = 1
		SELECT @result
END
ELSE
 IF EXISTS(SELECT * FROM assessmentdetails WHERE assessmentid = @assessmentId AND questionid = @remarkId)
	BEGIN
		UPDATE [dbo].[assessmentdetails]
		SET Remarks=@remarks,
			DocumentPath = @DocumentName,
			CloudFilePath=@CloudFilePath,
			UpdatedBy=@updatedBy,
			Updateddtstamp =@CommentUpdDateTime
		WHERE  assessmentid=@assessmentId
		AND questionid = @remarkId
		
		SET @result = 1
		SELECT @result
	END
	ELSE
		BEGIN
			SET @result =0 
			SELECT @result
		END
END
----------
CREATE OR ALTER  PROCEDURE [dbo].[usp_GetAssessmentDocumentName]
(
	@assessmentId INT,
	@remarkId INT
)
AS
BEGIN
IF @remarkId = 0 
BEGIN
	SELECT ISNULL(Generaldocpath,'') AS documentName FROM employee_assessment WHERE id = @assessmentId 
END
ELSE
BEGIN
	SELECT ISNULL(documentpath,'') AS documentName FROM AssessmentDetails WHERE assessmentid = @assessmentId  AND questionid=@remarkId
END
END
----------
CREATE OR ALTER  PROCEDURE [dbo].[usp_DeleteAssessmentComment]  
 (
	 @assessmentId INT,
	 @remarkId INT,
	 @updatedBy INT,
	 @result INT OUT
 )
AS 
BEGIN 

IF NOT EXISTS(SELECT * FROM Employee_Assessment WHERE id=@assessmentId AND AssessmentBy=@updatedBy)
BEGIN
	SET @result = -1
	SELECT @result
END
ELSE
IF @remarkId =0 
BEGIN
	UPDATE [dbo].[Employee_Assessment]
		SET GeneralDocPath='',
			Remarks='',
			UpdatedBy=@updatedBy,
			Updateddtstamp =GETDATE()
		WHERE  ID=@assessmentId

		SET @result = 1
		SELECT @result
END
ELSE
 IF EXISTS(SELECT * FROM assessmentdetails WHERE assessmentid = @assessmentId AND questionid = @remarkId)
	BEGIN
		UPDATE [dbo].[assessmentdetails]
		SET DocumentPath='',
			Remarks='',
			UpdatedBy=@updatedBy,
			Updateddtstamp =GETDATE()
		WHERE  assessmentid=@assessmentId
		AND questionid = @remarkId
		
		SET @result = 1
		SELECT @result
	END
	ELSE
		BEGIN
			SET @result =0 
			SELECT @result
		END
    
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_AddTeamConfiguration]
(
    -- Add the parameters for the stored procedure here
    @Name VARCHAR(100),
	@Description VARCHAR(500),
	@StartDate DATETIME,
	@EndDate DATETIME,
	@TriggerActivityDays INT,
    @CreatedBy INT,
	@TeamId INT OUTPUT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

IF EXISTS(SELECT * FROM TeamConfiguration WHERE [Name] = @Name)
BEGIN
	SET @TeamId =  -1;
	SELECT @TeamId;
END
ELSE
BEGIN
    -- Insert statements for procedure here
    INSERT INTO [TeamConfiguration]
	(
		[Name],
		[Description],
		StartDate,
		EndDate,
		TriggerActivityDays,
		[Status],
		BActive,
		CreatedBy,
		CreatedDtstamp
	)
	VALUES
	(
		@Name,
		@Description,
		@StartDate,
		@EndDate,
		@TriggerActivityDays,
		1,
		1,
		@CreatedBy,
		GETDATE()
	);

	 SET @TeamId =SCOPE_IDENTITY();
	 SELECT @TeamId;
END
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_AddTeamManagers]
(
    -- Add the parameters for the stored procedure here
    @TeamId INT,
	@ManagerId INT,
    @CreatedBy INT,
	@Result INT OUTPUT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

IF NOT EXISTS(SELECT * FROM TeamManagers WHERE TeamId = @TeamId AND ManagerId = @ManagerId AND Bactive = 1)
BEGIN
	
    -- Insert statements for procedure here
    INSERT INTO [TeamManagers]
	(
		TeamId,
		ManagerId,
		BActive,
		CreatedBy,
		CreatedDtstamp
	)
	VALUES
	(
		@TeamId,
		@ManagerId,
		1,
		@CreatedBy,
		GETDATE()
	);

	 SET @Result =1;
	 SELECT @Result;
END
ELSE
BEGIN
		SET @Result = 0; 
		SELECT @Result;
END
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_AddTeamEmployees]
(
    -- Add the parameters for the stored procedure here
    @TeamId INT,
	@EmpId INT,
    @CreatedBy INT,
	@Result INT OUTPUT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

IF NOT EXISTS(SELECT * FROM TeamEmployees WHERE TeamId = @TeamId AND EmpId = @EmpId AND BActive = 1)
BEGIN
	
    -- Insert statements for procedure here
    INSERT INTO [TeamEmployees]
	(
		TeamId,
		EmpId,
		BActive,
		CreatedBy,
		CreatedDtstamp
	)
	VALUES
	(
		@TeamId,
		@EmpId,
		1,
		@CreatedBy,
		GETDATE()
	);

	 SET @Result =1;
	 SELECT @Result;
END
ELSE
BEGIN
		SET @Result = 0; 
		SELECT @Result;
END
END

----------
CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateTeamConfiguration]
(
    -- Add the parameters for the stored procedure here
	@TeamId INT,
	@Name VARCHAR(100),
	@Description VARCHAR(500),
	@StartDate DATETIME,
	@EndDate DATETIME,
	@Status BIT,
	@TriggerActivityDays INT,
	@UpdatedBy INT,
	@Result INT OUTPUT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
IF EXISTS(SELECT * FROM TeamConfiguration WHERE [Name] = @Name AND Id <> @TeamId)
BEGIN
	SET @Result = -1;
	SELECT @Result;
END
ELSE
BEGIN
	IF EXISTS(SELECT * FROM TeamConfiguration WHERE Id = @TeamId)
	BEGIN
	
		-- Insert statements for procedure here
	   UPDATE TeamConfiguration 
	   SET [Name] = @Name,
		   [Description] = @Description,
		   StartDate = @StartDate,
		   EndDate = @EndDate,
		   TriggerActivityDays = @TriggerActivityDays,
		   [Status] = @Status, --CASE WHEN EndDate <> @EndDate THEN 1 ELSE 0 END,
		   UpdatedBy = @UpdatedBy,
		   UpdatedDtstamp = GETDATE()
		WHERE Id = @TeamId;


		 SET @Result =1;
		 SELECT @Result;
	END
END
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_DeleteTeamConfiguration]
(
   @TeamId INT,
   @UpdatedBy INT,
   @Result INT OUTPUT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
IF EXISTS(SELECT * FROM TeamConfiguration WHERE Id = @TeamId AND Status = 1)
BEGIN
    -- Insert statements for procedure here
   UPDATE TeamConfiguration 
   SET Status = 0,
	   UpdatedBy = @UpdatedBy,
	   UpdatedDtstamp = GETDATE()
   WHERE Id = @TeamId;

   SET @Result =1;
   SELECT @Result;
END
ELSE 
	SET @Result =0; --For already inactivated
   SELECT @Result;
END

----------
CREATE OR ALTER PROCEDURE [dbo].[usp_DeleteTeamManagers]
(
    -- Add the parameters for the stored procedure here
    @TeamId INT,
	@Result INT OUTPUT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
IF EXISTS(SELECT * FROM TeamManagers WHERE TeamId = @TeamId)
BEGIN
	DELETE FROM TeamManagers WHERE TeamId = @TeamId
	SET @Result = 1;
	SELECT @Result;
END

END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_DeleteTeamEmployees]
(
    -- Add the parameters for the stored procedure here
    @TeamId INT,
	@Result INT OUTPUT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
IF EXISTS(SELECT * FROM TeamEmployees WHERE TeamId = @TeamId)
BEGIN
	DELETE FROM TeamEmployees WHERE TeamId = @TeamId
	SET @Result = 1;
	SELECT @Result;
END

END

----------
CREATE OR ALTER   PROCEDURE [dbo].[usp_GetAllTeams]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT id AS TeamId, [name], [description], StartDate, EndDate, TriggerActivityDays, Status, CASE WHEN [Status] = 1 THEN 'Active' ELSE 'Inactive' END AS TeamStatus,
	STUFF(
	(
		SELECT ',' + [dbo].[GetEmployeeName](tm.ManagerId)
		FROM TeamManagers tm 
		INNER JOIN employeedetails e ON e.bactive = 1 AND e.empid = tm.managerId
		WHERE tm.TeamId = tc.Id AND e.IsMailSent=1 AND e.roleid NOT IN (2,5) AND tm.bactive=1 AND e.empstatus =1 For XML PATH('')), 1,1,''
		) AS Managers, 
	STUFF(
	(
		SELECT ',' + CAST(tm.ManagerId AS VARCHAR(1000))
		FROM TeamManagers tm 
		INNER JOIN employeedetails e ON e.bactive = 1 AND e.empid = tm.managerId
		WHERE tm.TeamId = tc.Id AND tm.bactive=1 AND e.IsMailSent=1 AND e.roleid NOT IN (2,5) AND e.empstatus =1 For XML PATH('')), 1,1,'') AS ManagerIds,
	tc.createdBy, ISNULL(e.FirstName, '') AS CreatedByFName, ISNULL(e.LastName, '')  AS CreatedByLName
	FROM TeamConfiguration tc
	LEFT JOIN userlogin u ON u.userId = tc.createdBy
	LEFT JOIN employeedetails e ON e.empId = u.empId
	ORDER BY id DESC
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetTeamDetailsByTeamId]
(
    -- Add the parameters for the stored procedure here
    @TeamId INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Id AS TeamId, [name], [description], StartDate, EndDate, TriggerActivityDays, Status
	FROM TeamConfiguration
	WHERE id = @TeamId
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetTeamEmployeesByManagerId]
(
	@ManagerId INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

   	SELECT DISTINCT te.empId FROM teamemployees te 
	LEFT JOIN teammanagers tm ON te.teamid = tm.teamid
	LEFT JOIN teamconfiguration tc ON tc.id = tm.teamId
	INNER JOIN employeedetails e ON te.empId = e.empId
	WHERE tc.status = 1 AND tm.managerid = @ManagerId AND e.bactive = 1 AND e.empstatus = 1
		AND (CONVERT (DATE, tc.startdate, 105) <= CONVERT(DATE, GETDATE(),105) AND 
		(CONVERT(DATE, tc.enddate, 105) >= CONVERT(DATE, GETDATE(),105) OR CONVERT(DATE, tc.enddate, 105) = '1900-01-01'))

END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GetEmpTeamRelationByManagerId]
(
	@ManagerId INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

   	SELECT te.empId, 1 AS teamType FROM teamemployees te 
	LEFT JOIN teammanagers tm ON te.teamid = tm.teamid
	LEFT JOIN teamconfiguration t ON t.id = tm.teamId
	INNER JOIN employeedetails e ON te.empId = e.empId
	WHERE t.status = 1 AND tm.managerid = @ManagerId AND tm.BActive=1 AND e.bactive = 1 AND e.empstatus = 1
		AND (CONVERT (DATE, t.startdate, 105) <= CONVERT(DATE, GETDATE(),105) AND 
		(CONVERT(DATE, t.enddate, 105) >= CONVERT(DATE, GETDATE(),105) OR CONVERT(DATE, t.enddate, 105) = '1900-01-01'))
	GROUP BY te.empId
	UNION
	SELECT  empId, 2 AS teamType FROM TeamEmployees 
	WHERE teamid IN ( 
						SELECT tc.id FROM employeedetails e  
						INNER JOIN TeamEmployees te ON te.empid =e.empid AND managerid=@ManagerId
						INNER JOIN TeamConfiguration tc ON tc.id=te.teamid  
						WHERE tc.status = 1  AND e.bactive = 1 AND e.empstatus = 1
						AND (CONVERT (DATE, tc.startdate, 105) <= CONVERT(DATE, GETDATE(),105) AND 
						(CONVERT(DATE, tc.enddate, 105) >= CONVERT(DATE, GETDATE(),105) OR CONVERT(DATE, tc.enddate, 105) = '1900-01-01'))
						)
	AND empid NOT IN ( 
						SELECT te.EmpId FROM employeedetails e  -- where managerid=19
						INNER JOIN TeamManagers tm ON tm.ManagerId =e.empid AND  tm.ManagerId=@ManagerId
						INNER JOIN TeamConfiguration tc ON tc.id=tm.teamid
						INNER JOIN TeamEmployees te ON te.TeamId=tc.id
						WHERE tc.status = 1  AND e.bactive = 1 AND tm.BActive=1 AND e.empstatus = 1 AND tm.BActive=1
					AND (CONVERT (DATE, tc.startdate, 105) <= CONVERT(DATE, GETDATE(),105) AND 
					(CONVERT(DATE, tc.enddate, 105) >= CONVERT(DATE, GETDATE(),105) OR CONVERT(DATE, tc.enddate, 105) = '1900-01-01'))
					)
	AND empid NOT IN (
						SELECT empid FROM employeedetails WHERE managerid =@ManagerId
					)

END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetTeamEmployeesByTeamId]
(
    -- Add the parameters for the stored procedure here
    @TeamId INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

   	--Team Employees
	SELECT Id, TeamId, t.EmpId
	FROM TeamEmployees t
	INNER JOIN employeedetails e ON e.bactive = 1 AND e.empId = t.empId
	WHERE TeamId = @TeamId AND e.empstatus = 1

END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetTeamEmployeesDetailsByTeamId]
(
    @TeamId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ed.EmpId, ed.firstname,ed.lastname
	FROM TeamEmployees tm
	INNER JOIN Employeedetails ed ON ed.empid =tm.EmpId
	WHERE TeamId = @TeamId  AND ed.BActive=1 AND ed.empstatus=1
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetTeamManagersByTeamId] 
(
    -- Add the parameters for the stored procedure here
    @TeamId INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Team Managers
	SELECT Id, tm.TeamId, tm.ManagerId
	FROM TeamManagers tm
	INNER JOIN Employeedetails ed ON ed.empid =tm.managerid
	WHERE TeamId = @TeamId AND ed.BActive=1 AND ed.empstatus=1 AND tm.BActive=1
	 AND ed.roleId NOT IN(2,5) AND ed.ismailSent=1
		
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetTeamManagersDetailsByTeamId]
(
    @TeamId INT
)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ed.EmpId,ed.firstname,ed.lastname
	FROM TeamManagers tm
	INNER JOIN Employeedetails ed ON ed.empid =tm.managerid
	WHERE TeamId = @TeamId AND ed.BActive=1 AND ed.empstatus=1 AND tm.BActive=1
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetTeamManagersEmailIds]
(
	@teamid INT
)
AS
BEGIN
	SELECT ed.email 
	FROM TeamManagers tm
	INNER JOIN EmployeeDetails ed ON ed.empid=tm.ManagerId
	WHERE TeamId = @teamid AND ed.BActive=1 AND ed.empstatus=1 AND tm.Bactive=1
END
----------
CREATE OR ALTER  PROCEDURE [dbo].[usp_GetYearWiseTeamList]
(
	@YearId INT,
	@ManagerId INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	
    -- Insert statements for procedure here
IF [dbo].[GetRoleByEmpid](@ManagerId) = 2
BEGIN
	SELECT DISTINCT tc.Id AS TeamId, [Name] AS Team, 0 AS TeamType
	FROM teamconfiguration tc
	INNER JOIN teamManagers tm ON tm.teamId = tc.Id 
	WHERE [STATUS] = 1 AND YEAR(StartDate) <= @YearID
	ORDER BY [Name]
END
ELSE
BEGIN
	SELECT TeamId, Team, TeamType
	FROM
	(
		SELECT tc.Id AS TeamId, [Name] AS Team, 
			--CASE WHEN tm.ManagerId = @ManagerId THEN 1 ELSE 2 END AS TeamType
			CASE WHEN tm.ManagerId = @ManagerId THEN 1 END AS TeamType
		FROM teamconfiguration tc
		INNER JOIN teamManagers tm ON tm.teamId = tc.Id 
		WHERE [STATUS] = 1 AND tm.ManagerId = @ManagerId AND YEAR(StartDate) <= @YearID
		AND tm.BActive=1
		UNION
		SELECT te.teamId, [Name] AS Team, 2 AS TeamType
		FROM teamemployees te
		INNER JOIN
		(
			SELECT te.empID, te.teamId 
			FROM teamemployees te
			LEFT JOIN employeedetails e ON e.empId = te.empId
			WHERE managerId = @ManagerId AND e.bactive = 1 AND e.empstatus = 1 
			AND teamId NOT IN(SELECT teamId FROM teamManagers tm WHERE managerId = @ManagerId AND bactive =1)
		) v ON v.teamId = te.teamId AND v.empId <> te.empId
		INNER JOIN TeamConfiguration tc on tc.id=te.teamid  
		WHERE tc.status = 1  AND YEAR(StartDate) <= @YearId
		GROUP BY te.TeamId, [Name]
	)v ORDER BY Team
END
END
----------
CREATE OR ALTER   PROCEDURE[dbo].[usp_GetYearAndTeamWiseTeamDashboard]
(
    @YearId INT,
	@TeamId INT,
	@ManagerId INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
DECLARE @CurrYear INT = YEAR(GETDATE())
DECLARE @LastYear INT = YEAR(GETDATE()) - 1
DECLARE @LastDate DATE = CASE WHEN @YearID <> 0  AND @CurrYear <> @YearId THEN DATEFROMPARTS(@yearID,12,31) ELSE CONVERT(DATE, GETDATE(), 101) END

CREATE TABLE #tmpEmpAssessment
(
	Empid INT,
	AssessmentId INT,
	RatingDate DATETIME,
	Score DECIMAL(2,0)
);

IF EXISTS(SELECT * FROM teammanagers WHERE ManagerId = @ManagerId AND TeamId = @TeamId)
		OR (dbo.GetRoleByEmpid(@ManagerId) = 2)
BEGIN
	INSERT INTO #tmpEmpAssessment
	SELECT * FROM
	(
		SELECT at.empId, at.Id AS AssessmentId, assessmentDate AS RatingDate, score
		FROM employee_assessment at
		INNER JOIN TeamEmployees t ON at.empId = t.empId
		INNER JOIN employeedetails e ON e.bactive = 1 AND e.empId = t.empId 
		INNER JOIN 
		(
			SELECT userId 
			FROM teammanagers tm
			LEFT JOIN userlogin u ON u.empId = tm.managerId
			WHERE teamid = @TeamId
		) u ON u.userId = at.assessmentby
		WHERE TeamId = @TeamId AND e.empstatus = 1 
		AND YEAR(assessmentDate) = @YearId
	)k
END
ELSE 
BEGIN
	INSERT INTO #tmpEmpAssessment
	SELECT * FROM
	(
		SELECT at.empId, at.Id AS AssessmentId, assessmentDate AS RatingDate, score
		FROM employee_assessment at
		INNER JOIN TeamEmployees t ON at.empId = t.empId
		INNER JOIN employeedetails e ON e.bactive = 1 AND e.empId = t.empId 
		INNER JOIN 
		(
			SELECT userId 
			FROM teammanagers tm
			LEFT JOIN userlogin u ON u.empId = tm.managerId
			WHERE teamid = @TeamId
		) u ON u.userId = at.assessmentby
		WHERE TeamId = @TeamId AND e.empstatus = 1 AND e.managerID <> @ManagerId
		AND YEAR(assessmentDate) = @YearId
	)k
END

	SELECT CAST(CAST(ROUND(AVG(CAST(score AS DECIMAL(2,0))),0) AS DECIMAL(10,0)) AS INT) AS TeamAvgscore, 
	       dbo.GETSCORERANK(CAST(ROUND(AVG(score),0) AS DECIMAL(10,0))) AS TeamAvgscoreRank 
	FROM #tmpEmpAssessment

	SELECT CASE WHEN TodayAvgScore IS NULL THEN CAST(LastAvgScore AS INT) ELSE CAST(TodayAvgScore AS INT) END AS AvgScoreByDay,
			CASE WHEN TodayAvgScore IS NULL THEN dbo.GETScoreRank(LastAvgScore) ELSE dbo.GETScoreRank(TodayAvgScore) END AS AvgScoreByDayRank
	FROM
	(
		SELECT SUM(TodayAvgScore) AS TodayAvgScore, SUM(LastAvgScore) AS LastAvgScore
		FROM
		(
			SELECT CAST(ROUND(AVG(CAST(score AS DECIMAL(2,0))),0) AS DECIMAL(10,0)) AS TodayAvgScore, NULL AS LastAvgScore
			FROM #tmpEmpAssessment k
			WHERE CONVERT(DATE, RatingDate, 101) = @LastDate
			UNION
			SELECT  NULL AS TodayAvgScore, CAST(ROUND(AVG(CAST(score AS DECIMAL(2,0))),0) AS DECIMAL(10,0)) AS LastAvgScore 
			FROM #tmpEmpAssessment k 
			WHERE AssessmentId = (SELECT MAX(AssessmentId) AS AssessmentId FROM #tmpEmpAssessment s WHERE k.empid = s.empid) 
		) v
	) v1 

END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetTeamDetailsForTeamNotification]
AS
BEGIN
	SELECT Id AS TeamId,Name,Description,StartDate,EndDate,TriggerActivityDays 
	FROM TeamConfiguration 
	WHERE Status=1 AND (CONVERT(DATE, startdate, 105) <= CONVERT(DATE, GETDATE(),105))
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_AddTeamInActivityLog]
(
	@teamid INT,
	@emailto VARCHAR(100),
	@emailtext VARCHAR(1000),
	@triggeractivitydays INT,
	@createdby INT,
	@result INT OUT
)
AS
BEGIN

	INSERT INTO [dbo].[TeamInactivityLog]
			(
				[teamid]
			   ,[emailto]
			   ,[emailtext]
			   ,[triggeractivitydays]
			   ,[bactive]
			   ,[createdby]
			   ,[createddtstamp]
		   )
     VALUES
           (
				@teamid,
				@emailto,
				@emailtext,
				@triggeractivitydays,
				1,
				@createdby,
				GETDATE()
		   )
	SELECT @result=1
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetTeamInactivityEmployee]
(
	@triggerActivitydays INT,
	@teamid INT
)
AS
BEGIN
	IF((SELECT ISNULL( DATEDIFF(DAY,MAX(createddtstamp),GETDATE()),@triggerActivitydays+1) AS lastassessmentdays FROM TeamInactivityLog WHERE teamid=@teamid) > @triggerActivitydays)
		BEGIN
			SELECT * INTO #tempemployeeAssessment FROM 
			(
					SELECT empid, DATEDIFF(DAY,MAX(assessmentdate),GETDATE()) AS lastassessmentdays FROM Employee_Assessment 
					GROUP BY empid
			) b  

			SELECT ed.empid, ed.firstname,ed.lastname 
			FROM TeamEmployees te
			INNER JOIN employeedetails ed on ed.empid=te.empid AND te.teamid=@teamid
			LEFT JOIN #tempemployeeAssessment es on es.empid=te.EmpId
			WHERE (es.lastassessmentdays > @triggerActivitydays OR (es.empid IS NULL AND ed.joiningdate < DATEADD(DAY,-(@triggerActivitydays),GETDATE()))) AND  ed.BActive=1 AND ed.empstatus=1
 
		END
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetManagersDetByManagerIds] 
(
	@EmpIdList VARCHAR(200)
)
AS
BEGIN 
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT managerId into #EmpIdList FROM (SELECT value AS managerId FROM  STRING_SPLIT(@EmpIdList, ',')) d;
		
	SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, 
		   e.email, e.departmentid, e.empstatus, e.roleid,
		   cd.contractstartDate AS companyContractStartDate,
		   cd.contractEndDate AS companyContractEndDate
	FROM employeedetails e 
	LEFT JOIN companydetails cd ON cd.bactive = 1 AND cd.compId = e.companyID
	WHERE e.bactive = 1 AND e.empStatus = 1
	AND e.roleid <> 5 AND (CONVERT(Date, cd.ContractStartDate,101) <= CONVERT(Date, GETDATE(),101)
	AND DATEADD(DAY,ISNULL(cd.gracePeriod,0),CONVERT(Date, cd.ContractEndDate,101)) >= CONVERT(Date, GETDATE(),101))
	AND e.empID IN (SELECT managerId FROM #EmpIdList)

END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetAllNonManagers]
(
	@Companyid INT
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, 
		   e.email, e.departmentid, ISNULL(e.managerid,0) AS managerid, e.empstatus, e.roleid
	FROM employeedetails e 
	WHERE e.companyid = @Companyid AND e.bactive = 1 AND roleId = 5
	AND e.empstatus = 1
	ORDER BY e.firstname
END
----------
CREATE OR ALTER  PROCEDURE [dbo].[usp_GetAllClassifications]
AS
BEGIN

	SELECT id AS ClassificationId, Classification FROM Classifications WHERE bactive = 1

END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetSparksByEmpId]
(
	@EmpId INT
)
AS
BEGIN

	SELECT e.id AS SparkId,e.EmpId,ISNULL(e.Categoryid,'') AS Categoryid,ISNULL(q.Category,'') AS Category,ISNULL(e.ClassificationId,'') AS ClassificationId,
		ISNULL(c.Classification,'') AS Classification,d.FirstName AS SparkByFirstName,d.LastName AS SparkByLastName,e.Remarks, e.DocumentPath AS DocumentName, 
		ISNULL(e.CloudFilePath,'') AS CloudFilePath,ISNULL(e.UpdatedDtStamp,e.SparkDate) AS SparkDate, e.SparkBy,e.Bactive,e.CreatedBy,e.UpdatedBy,
		ISNULL(d.empimgpath,'') AS SparkByImgPath
	FROM EmployeeSparkDetails e 
	LEFT JOIN Classifications c ON e.classificationid=c.id
	LEFT JOIN userlogin u ON e.sparkby=u.userid
	LEFT JOIN EmployeeDetails d ON u.empid=d.empid
	LEFT JOIN QuestionCategories q ON e.categoryid = q.categoryid
	WHERE e.Empid=@EmpId AND e.Bactive = 1 AND e.ApprovalStatus=1
	order by ISNULL(e.UpdatedDtStamp,e.SparkDate) desc

END
----------
CREATE  OR ALTER   PROCEDURE [dbo].[usp_GetSparksBySparkId]
(
	@EmpId INT,
	@SparkId INT
)
AS
BEGIN

	SELECT e.id AS SparkId,e.EmpId,ISNULL(e.Categoryid,'') AS Categoryid,ISNULL(q.Category,'') AS Category,ISNULL(e.ClassificationId,'') AS ClassificationId,
		ISNULL(c.Classification,'') AS Classification,d.FirstName AS SparkByFirstName,d.LastName AS SparkByLastName,e.Remarks, e.DocumentPath AS DocumentName, 
		ISNULL(e.UpdatedDtStamp,e.SparkDate) AS SparkDate, e.SparkBy,e.Bactive,e.CreatedBy,e.UpdatedBy,ISNULL(d.empimgpath,'') AS SparkByImgPath,
		ISNULL(e.CloudFilePath,'') AS CloudFilePath
	FROM EmployeeSparkDetails e 
	LEFT JOIN Classifications c ON e.classificationid=c.id
	LEFT JOIN userlogin u ON e.sparkby=u.userid
	LEFT JOIN EmployeeDetails d ON u.empid=d.empid
	LEFT JOIN QuestionCategories q ON e.categoryid = q.categoryid
	WHERE  e.empid=@EmpId  AND e.id=@SparkId AND e.Bactive = 1
	order by ISNULL(e.UpdatedDtStamp,e.SparkDate) desc

END
----------
CREATE  OR ALTER PROCEDURE [dbo].[usp_GetSparkDocumentName]
(
	@SparkId INT,
	@EmpId INT
)
AS
BEGIN
	SELECT ISNULL(documentpath,'') AS documentName FROM EmployeeSparkDetails WHERE id = @SparkId and Empid=@EmpId
END
----------
CREATE  OR ALTER  PROCEDURE [dbo].[usp_AddEmployeeSparkDetails]
(
	@SparkDate DATETIME,
	@EmpId INT,
    @SparkBy INT,
	@CategoryId SMALLINT,
    @ClassificationId INT,
    @Remarks NVARCHAR(4000),
    @DocumentName VARCHAR(1000),
	@CloudFilePath VARCHAR(500),
    @ViaSms BIT,
	@SenderPhoneNumber VARCHAR(25),
	@CreatedBy INT,
	@ApprovalStatus INT,
	@ApprovalBy INT,
	@Result INT OUT
)
AS
BEGIN

  INSERT INTO [dbo].[EmployeeSparkDetails]
  (
		EmpId,
		SparkDate,
		SparkBy,
		CategoryId,
		ClassificationId,
		Remarks,
		DocumentPath,
		CloudFilePath,
		ViaSms,
		SenderPhoneNumber,
		BActive,
		ApprovalStatus,
		ApprovalBy, 
		ApprovalDate,
		CreatedBy,
		CreatedDtstamp
  )
  VALUES
  (
		@EmpId,
        @SparkDate,
        @SparkBy, 
		@CategoryId,
        @ClassificationId,
        @Remarks,
        @DocumentName,
		@CloudFilePath,
        @ViaSms,
		@SenderPhoneNumber,
        1,
		@ApprovalStatus,
		@ApprovalBy,
		GETDATE(),
        @CreatedBy,
        GETDATE()
  )

 SET @result = Scope_Identity() 
 SELECT @result

END
----------
CREATE  OR ALTER PROCEDURE [dbo].[usp_UpdateEmployeeSparkDetails]
(
	@SparkId INT,
	@EmpId INT,
	@SparkDate DATETIME,
	@CategoryId SMALLINT,
    @ClassificationId INT,
    @Remarks NVARCHAR(4000),
    @DocumentName VARCHAR(1000),
	@CloudFilePath VARCHAR(500),
    @UpdatedBy INT,
	@Result INT OUT
)
AS
BEGIN
 IF EXISTS(SELECT ID FROM EmployeeSparkDetails WHERE ID=@SparkId AND EmpId=@EmpId AND Bactive=1)
 BEGIN
  IF EXISTS(SELECT ID FROM EmployeeSparkDetails WHERE ID=@SparkId AND EmpId=@EmpId AND Bactive=1 AND SparkBy=@UpdatedBy)
    BEGIN
		UPDATE [dbo].[EmployeeSparkDetails]
		SET  [ClassificationId] = @ClassificationId
			,[CategoryId]=@CategoryId
			,[Remarks] = @Remarks
			,[DocumentPath] = @DocumentName
			,[CloudFilePath]=@CloudFilePath
			,[Updatedby] = @UpdatedBy
			,[UpdatedDtstamp] = @SparkDate
		WHERE ID=@SparkId 
			AND EmpId=@EmpId

		SET @Result = 1
		SELECT @Result
	END
	ELSE
	BEGIN
		SET @Result = -1
		SELECT @Result
	END
 END
 ELSE
 BEGIN
	 SET @Result = 0
	 SELECT @Result
 END

END
----------
CREATE OR ALTER  PROCEDURE [dbo].[usp_DeleteEmployeeSparkAttachment]
(
	@SparkId int,
	@EmpId int,
	@SparkDate datetime,
    @UpdatedBy int,
	@Result INT OUT
)
AS
BEGIN
 IF EXISTS(SELECT ID FROM EmployeeSparkDetails WHERE ID=@SparkId AND EmpId=@EmpId AND Bactive=1)
 BEGIN
  IF EXISTS(SELECT ID FROM EmployeeSparkDetails WHERE ID=@SparkId AND EmpId=@EmpId AND Bactive=1 AND SparkBy=@UpdatedBy)
    BEGIN
		UPDATE [dbo].[EmployeeSparkDetails]
		SET  [DocumentPath] = ''
			,[Updatedby] = @UpdatedBy
			,[UpdatedDtstamp] = @SparkDate
		WHERE ID=@SparkId 
			AND EmpId=@EmpId

		SET @Result = 1
		SELECT @Result
	END
	ELSE
	BEGIN
		SET @Result = -1
		SELECT @Result
	END
 END
 ELSE
 BEGIN
	 SET @Result = 0
	 SELECT @Result
 END

END
----------
CREATE OR ALTER  PROCEDURE [dbo].[usp_DeleteEmployeeSpark]
(
	@SparkId INT,
	@EmpId INT,
	@UpdatedBy INT,
	@Result INT OUT
)
AS
BEGIN
 IF EXISTS(SELECT ID FROM EmployeeSparkDetails WHERE ID=@SparkId AND EmpId=@EmpId AND Bactive=1)
 BEGIN
  IF EXISTS(SELECT ID FROM EmployeeSparkDetails WHERE ID=@SparkId AND EmpId=@EmpId AND Bactive=1 AND SparkBy=@UpdatedBy)
    BEGIN
		DELETE FROM [dbo].[EmployeeSparkDetails]
		WHERE ID=@SparkId 
		      AND EmpId=@EmpId

		SET @Result = 1
		SELECT @Result
	END
	ELSE
	BEGIN
		SET @Result = -1
		SELECT @Result
	END
 END
 ELSE
 BEGIN
	 SET @Result = 0
	 SELECT @Result
 END

END
----------
CREATE OR ALTER PROCEDURE  [dbo].[usp_GetEmployeeDetailsByEmployeeId]
(
  @EmployeeId VARCHAR(100)
)
AS
BEGIN
	SELECT EmpId,Email,EmpStatus,BActive,e.managerid FROM EmployeeDetails e
	WHERE E.EmployeeId=@EmployeeId AND Bactive=1 AND EmpStatus=1
END
----------

CREATE OR ALTER   PROCEDURE [dbo].[usp_GetUnApprovedSparkByManager]
(
	@EmpId INT
)
AS
BEGIN

	SELECT e.id AS SparkId,e.EmpId,ISNULL(e.Categoryid,'') AS Categoryid,ISNULL(q.Category,'') AS Category,ISNULL(e.ClassificationId,'') AS ClassificationId,
		   ISNULL(c.Classification,'') AS Classification,d.FirstName AS SparkByFirstName,d.LastName AS SparkByLastName,e.Remarks, e.DocumentPath AS DocumentName, 
		   ISNULL(e.UpdatedDtStamp,e.SparkDate) AS SparkDate, e.SparkBy,e.Bactive,e.CreatedBy,e.UpdatedBy,ed.firstname,ed.lastname,ed.EmployeeId,ISNULL(d.empimgpath,'') AS SparkByImgPath
	FROM EmployeeSparkDetails e 
	INNER JOIN EmployeeDetails ed on e.empid=ed.empid
	LEFT JOIN Classifications c ON e.classificationid=c.id
	LEFT JOIN userlogin u ON e.sparkby=u.userid
	LEFT JOIN EmployeeDetails d ON u.empid=d.empid
	LEFT JOIN QuestionCategories q ON e.categoryid = q.categoryid
	WHERE  e.Bactive = 1 AND e.ApprovalStatus=0 AND ed.managerid=@EmpId
	order by ISNULL(e.UpdatedDtStamp,e.SparkDate) desc

END

----------

CREATE OR ALTER   PROCEDURE [dbo].[usp_UpdateSparkApprovalStatus]
(
@sparkId INT,
@approvalStatus INT,
@rejectionRemark nvarchar(4000),
@approvalBy INT,
@Result INT OUT
)
AS
BEGIN
	IF EXISTS(SELECT ID FROM EmployeeSparkDetails WHERE ID=@SparkId AND Bactive=1 AND ApprovalStatus=0)
	BEGIN
		update EmployeeSparkDetails 
		set ApprovalStatus=@approvalStatus ,
			RejectionRemark=@rejectionRemark ,
			ApprovalDate=GETDATE(),
			ApprovalBy =@approvalBy

		where Id=@sparkId

		IF(@approvalStatus=1)
		BEGIN
			SET @Result = 1
			SELECT @Result
		END
		ELSE
		BEGIN
			SET @Result = 2
			SELECT @Result
		END
	END
	ELSE
	BEGIN

		SET @Result = 0
		SELECT @Result
	END
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GetEmployeeDetailsByEmpIds]
(
  @empIdList VARCHAR(100)
)
AS
BEGIN
	SELECT empid,firstname,lastname,email FROM employeedetails
	WHERE empid IN (SELECT * FROM STRING_SPLIT (@empIdList,',') a)
END

----------

CREATE OR ALTER PROCEDURE [dbo].[usp_GetUnApprovedSparkCountByManager]
(
	@EmpId INT
)
AS
BEGIN
	SELECT COUNT(Id)
	FROM EmployeeSparkDetails e 
	INNER JOIN EmployeeDetails ed ON e.empid=ed.empid
	WHERE  e.Bactive = 1 AND e.ApprovalStatus=0 AND ed.managerid=@EmpId
END

----------
CREATE OR ALTER   PROCEDURE [dbo].[usp_GetSparkRejectionDetails] 
(
	@EmpId INT,
	@SparkId INT
)
AS
BEGIN
	SELECT s.EmpId,[dbo].[GetEmpNameById](s.EmpId,NULL,NULL,NULL) EmployeeName,s.ApprovalBy, e.FirstName + ' '  + e.LastName AS RejectedByName,
		   s.SenderPhoneNumber,s.RejectionRemark
		FROM EmployeeSparkDetails s 
	LEFT JOIN UserLogin u ON s.ApprovalBy=u.userid
	LEFT JOIN EmployeeDetails e ON  e.empid=u.empid
	WHERE s.EmpId=@EmpId AND s.ID=@SparkId AND ApprovalStatus=2
END
----------
CREATE OR ALTER   PROCEDURE [dbo].[usp_UpdateAssessmentScoreFeedback]
(
	@AssessmentId INT,
	@ScoreFeedback BIT,
	@ExpectedScoreId INT,
	@FeedbackRemark VARCHAR(4000),
	@result INT OUT
)
AS
BEGIN
IF EXISTS(SELECT * FROM employee_assessment WHERE id=@AssessmentId)
	BEGIN
		UPDATE  employee_assessment SET ScoreFeedback=@ScoreFeedback , ExpectedScoreId=@ExpectedScoreId,FeedbackRemark=@FeedbackRemark WHERE id=@AssessmentId
			SET @result=1
	END
	ELSE
	BEGIN
		SET @result=-1
	END
END

----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetScoreRanks]
AS
BEGIN
SELECT id AS ScoreId,ScoreRank FROM scoreremarks WHERE Bactive=1 ORDER BY GradeOrder
END

----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetActiveManagerList]
AS
BEGIN
	SELECT empid,EmployeeId,email,firstname,lastname 
	FROM employeedetails 
	WHERE bactive = 1 AND roleId NOT IN (2,5) AND empstatus = 1 AND IsMailSent=1
	ORDER BY firstname
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetTeamAssessmentYear] 
(
	@ManagerId INT
)
AS
BEGIN
	DECLARE @UserRole INT;
	DECLARE @MinYear INT;
	SELECT  @UserRole = dbo.GetRoleByEmpid(@ManagerId)
	IF(@UserRole=2)
		BEGIN
			SELECT    DATEPART(YEAR,MIN(tc.StartDate)) AS YearId, 0 AS TeamType FROM teamconfiguration tc WHERE [STATUS] = 1 
		END
	ELSE
		BEGIN
			SELECT   DATEPART(YEAR,MIN(tc.StartDate)) AS YearId , 1 AS TeamType FROM teamconfiguration tc
			LEFT JOIN teamManagers tm ON tm.teamId = tc.Id AND tm.ManagerId = @ManagerId
			WHERE [STATUS] = 1 AND tm.BActive=1
			HAVING  MIN(tc.StartDate) IS NOT NULL
			UNION 

			SELECT  DATEPART(YEAR,MIN(tc.StartDate)) AS YearId, 2 AS TeamType
			FROM teamemployees te
			INNER JOIN
			(
				SELECT te.empID, te.teamId 
				FROM teamemployees te
				LEFT JOIN employeedetails e ON e.empId = te.empId
				WHERE managerId = @ManagerId AND e.bactive = 1 AND e.empstatus = 1 
				AND teamId NOT IN(SELECT teamId FROM teamManagers tm WHERE managerId = @ManagerId AND tm.BActive=1)
			) v ON v.teamId = te.teamId AND v.empId <> te.empId
			INNER JOIN TeamConfiguration tc on tc.id=te.teamid  
			WHERE tc.status = 1 
			HAVING  MIN(tc.StartDate) IS NOT NULL
		END
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_GetEmployeeNotPartOfTeamByManagerId] 
(
	@ManagerId INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

   	SELECT te.empId, 1 AS teamType FROM teamemployees te 
	LEFT JOIN teammanagers tm ON te.teamid = tm.teamid
	LEFT JOIN teamconfiguration t ON t.id = tm.teamId
	INNER JOIN employeedetails e ON te.empId = e.empId
	WHERE t.status = 1 AND tm.managerid = @ManagerId AND e.bactive = 1 AND e.empstatus = 1 AND tm.bactive = 1
		AND (CONVERT (DATE, t.startdate, 105) <= CONVERT(DATE, GETDATE(),105) AND 
		(CONVERT(DATE, t.enddate, 105) >= CONVERT(DATE, GETDATE(),105) OR CONVERT(DATE, t.enddate, 105) = '1900-01-01'))
	GROUP BY te.empId
	UNION
	SELECT te1.empId, 2 AS teamType
	From employeedetails e 
	LEFT JOIN (SELECT empId, teamId FROM teamemployees) te ON te.empId = e.empId
	RIGHT JOIN teamemployees te1 ON te.teamId = te1.teamID AND te.empId <> te1.empId
	RIGHT JOIN teammanagers tm ON tm.teamId = te1.teamID
	LEFT JOIN teamconfiguration t on t.Id = te.teamId
	WHERE t.status =1 AND e.bactive = 1 AND e.empstatus = 1 AND tm.bactive = 1
		AND (CONVERT (DATE, t.startdate, 105) <= CONVERT(DATE, GETDATE(),105) AND 
		(CONVERT(DATE, t.enddate, 105) >= CONVERT(DATE, GETDATE(),105) OR CONVERT(DATE, t.enddate, 105) = '1900-01-01'))
		AND e.managerid = @ManagerId AND tm.managerId NOT IN(SELECT managerId FROM teammanagers WHERE managerID = @ManagerId AND bactive = 1)
	GROUP BY te1.empId, te1.teamId

END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_DeleteTeamManagersByTeamIdAndManagerId]
(
    -- Add the parameters for the stored procedure here
    @TeamId INT,
    @ManagerId INT,
	@result INT OUTPUT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
IF EXISTS (SELECT * FROM teamManagers WHERE TeamId = @TeamId AND ManagerId = @ManagerId AND bactive =1)
BEGIN
	UPDATE teamManagers 
	SET bactive = 0
	WHERE TeamId = @TeamId AND ManagerId = @ManagerId
	SET @result = 1;
	SELECT @result;
END
    
END
----------
CREATE OR ALTER PROCEDURE [dbo].[usp_UpdateTeamManagersByTeamId]
(
    -- Add the parameters for the stored procedure here
    @TeamId INT,
    @ManagerId INT,
	@CreatedBy INT,
	@result INT OUTPUT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON

    -- Insert statements for procedure here
IF EXISTS (SELECT * FROM teamManagers WHERE TeamId = @TeamId AND ManagerId = @ManagerId AND bactive =0)
BEGIN
	UPDATE teamManagers 
	SET bactive = 1,
		Updatedby = @CreatedBy,
		UpdatedDtstamp = GETDATE()
	WHERE TeamId = @TeamId AND ManagerId = @ManagerId
	
	SET @result = 1;
	SELECT @result;
END
ELSE
BEGIN
	
	INSERT INTO [TeamManagers]
	(
		TeamId,
		ManagerId,
		BActive,
		CreatedBy,
		CreatedDtstamp
	)
	VALUES
	(
		@TeamId,
		@ManagerId,
		1,
		@CreatedBy,
		GETDATE()
	);

	
	SET @result = 1;
	SELECT @result;
END
    
END
----------
CREATE OR ALTER   PROCEDURE [dbo].[usp_GetTeamListForTeamCount] 
(
	@ManagerId INT
)
AS
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON
	
    -- Insert statements for procedure here
IF [dbo].[GetRoleByEmpid](@ManagerId) = 2
BEGIN
	SELECT DISTINCT tc.Id AS TeamId, [Name] AS Team, 0 AS TeamType
	FROM teamconfiguration tc
	INNER JOIN teamManagers tm ON tm.teamId = tc.Id 
	WHERE [STATUS] = 1 AND YEAR(StartDate) <= YEAR(GETDATE())
	AND tm.BActive=1
	ORDER BY [Name]
END
ELSE
BEGIN
	SELECT TeamId, Team, TeamType
	FROM
	(
		SELECT tc.Id AS TeamId, [Name] AS Team, 
			CASE WHEN tm.ManagerId = @ManagerId THEN 1 END AS TeamType
		FROM teamconfiguration tc
		INNER JOIN teamManagers tm ON tm.teamId = tc.Id 
		WHERE [STATUS] = 1 AND tm.ManagerId = @ManagerId AND YEAR(StartDate) <= YEAR(GETDATE())
		AND tm.BActive=1

		UNION
		SELECT te.teamId, [Name] AS Team, 2 AS TeamType
		FROM teamemployees te
		INNER JOIN
		(
			SELECT te.empID, te.teamId 
			FROM teamemployees te
			LEFT JOIN employeedetails e ON e.empId = te.empId
			WHERE e.managerid = @ManagerId AND e.bactive = 1 AND e.empstatus = 1 
			AND te.teamId NOT IN(SELECT teamId FROM teamManagers tm WHERE managerId = @ManagerId AND bactive =1)
		) v ON v.teamId = te.teamId AND v.empId <> te.empId
		INNER JOIN TeamConfiguration tc on tc.id=te.teamid  
		WHERE tc.status = 1  AND YEAR(StartDate) <= YEAR(GETDATE())
		GROUP BY te.TeamId, [Name]
	)v ORDER BY Team
END
END
----------
CREATE OR ALTER  PROCEDURE [dbo].[usp_GetAllEmployeesForActionByManager] 
(
	@Managerid INT
)
AS
BEGIN
	SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, 
		   e.email, e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, e.empstatus, e.roleid, 
		   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid), 101),'') AS lastAssessedDate, 
		   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = DATEPART(yyyy, GETDATE())) AS VARCHAR),'') AS RatingCompleted,
		   ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
	FROM employeedetails e 
	LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
	LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
	WHERE e.bactive = 1 AND e.empstatus = 1
	ORDER BY e.firstname
END

----------

CREATE OR ALTER       PROCEDURE [dbo].[usp_GetYearWiseAllOrgEmployeesWithoutPagination]
(
	@yearID INT,
	@companyid INT,
	@managerid INT,
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

CREATE TABLE #tempEmpAverageAssessmentDet (
	companyid INT, empid INT, avgScore DECIMAL(2,0), AvgScoreRank VARCHAR(100), yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	SELECT * INTO #tempScore
	FROM
	(
		SELECT companyID, empid, Score, ManagerId,  FORMAT(RatingDate, 'yyyyMM') AS monyrID
		FROM vw_EmpAssessmentScore
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear  AND empStatus = 1 
	)t

	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT  companyID, empId, avgscore, avgScoreRank, 0 AS YearID
		FROM
		(
			SELECT companyID, empId, CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS DECIMAL(10,0)) AS avgscore,
			dbo.GetScoreRank(CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS decimal(10,0))) AS AvgScoreRank,
			row_number() OVER (PARTITION BY empid ORDER BY empid) AS rownumber
			FROM #tempScore 
		)v WHERE rownumber = 1
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT companyID , empID, ISNULL(AvgScore,0) AS avgScore, ISNULL(AvgScoreRank, '') AS AvgScoreRank, year AS YearID FROM vw_EmpAverageAssessmentDet 
		WHERE companyid = @companyId AND year = @YearId
	)k
	END

	CREATE TABLE #tempEmpLastAssessmentDet (
		companyid INT, empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		--WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END

SET @YearID = CASE WHEN @YearID = 0 THEN YEAR(GETDATE()) ELSE @YearID END

BEGIN

		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, 		       CONVERT(DATE, e.joiningdate,101) AS joiningdate,
		       e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, 
		       ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, e.currentsalary,
		       ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
		       ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
		       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
		       ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted, 0 AS ord,
			   ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM employeedetails e
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid
		LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
		WHERE e.companyid = @companyid AND e.bactive = 1 AND e.roleid <> 4 AND e.empstatus = 1 AND Format(e.joiningDate, 'yyyy') <= @YearID
		AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
		ORDER BY e.empid DESC, e.lastname
	
END
END

----------

CREATE OR ALTER  PROCEDURE [dbo].[usp_GetYearlyMonAvgScoreWiseOrgEmpWithoutPagination] 
(
	@yearId INT,
	@companyId INT,
	@managerId INT,
	@grade VARCHAR(10),
	@month VARCHAR(10),
	@departmentlist VARCHAR(256)
)
AS 
BEGIN
SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

DECLARE @CurrYear INT = FORMAT(GETDATE(), 'yyyy')
DECLARE @LastYear INT = FORMAT(GETDATE(), 'yyyy') - 1
DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

CREATE TABLE #tempEmpAssessmentScore (
	companyid INT, empid INT, RatingDate DATETIME, Score DECIMAL(2,0), managerId INT, yearId INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyid, empid,  RatingDate, score, managerid, YearID FROM vw_EmpAssessmentScore 
		WHERE  empStatus = 1 AND FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear  AND companyid = @companyId
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyid, empid, RatingDate, score, managerid, YearID FROM vw_EmpAssessmentScore 
		WHERE  empStatus = 1 AND yearid = @YearId  AND companyid = @companyId
	)k
	END

	CREATE TABLE #tempEmpLastAssessmentDet (
	companyid INT, empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END

	SELECT avgscore, ScoreRank, monyrid, monyr,empid,managerid, companyid INTO #temp FROM
	(
		SELECT avgscore, dbo.get_grade_value(dbo.GETSCoreRank(avgscore)) AS ScoreRank, monyrid, monyr,empid,managerid, companyid
		FROM (
				SELECT DISTINCT CAST(ROUND(AVG(score) OVER (PARTITION BY empid, FORMAT(RatingDate,'yyyyMM')),0) AS DECIMAL(10,0)) AS avgscore,  empid,  
					   FORMAT(RatingDate,'yyyyMM') AS monyrid, FORMAT(RatingDate,'MMM') AS monyr, managerid, companyid
				FROM #tempEmpAssessmentScore 
		) v
	) m


	CREATE TABLE #monthlist (yearmonth VARCHAR(20), monthname VARCHAR(20));

	IF(@YearId = 0)
	BEGIN
			INSERT INTO #monthlist
			SELECT yearmonth, monthname FROM
			(
				SELECT Format(getdate(),'yyyy')+FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MM') AS yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number between 1 and MONTH(GETDATE())
				UNION		
				SELECT  CAST(@LastYear AS VARCHAR) + FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@LastYear AS VARCHAR) AS DATE), 'MM') AS yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number > MOnth(DATEADD(month,-12,GETDATE()))
			) v
		END
		ELSE
		BEGIN
			INSERT INTO #monthlist
			SELECT  yearmonth, monthname  
			FROM
			(
				SELECT CAST(@yearID AS VARCHAR) + FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@yearID AS VARCHAR) AS DATE), 'MM') AS yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@yearID AS VARCHAR) AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number between 1 and CASE WHEN @CurrYear <> @YearId THEN MONTH(DATEFROMPARTS(@yearID,12,31)) ELSE MONTH(GETDATE()) END
			)v
		END

	SELECT companyid, avgscore, ScoreRank, yearmonth AS monyrid, monthname  AS monyr, empid, managerid 
	INTO #tempScore 
	FROM #monthlist m 
	LEFT OUTER JOIN #temp s ON s.monyrid = (SELECT MAX(monyrid) FROM #temp WHERE monyrid < yearmonth AND companyid = s.companyid AND empid = s.empid)
	WHERE yearmonth NOT IN(SELECT monyrid FROM #temp WHERE monyrid <> '' AND companyid = s.companyid AND empid = s.empid) AND companyid = s.companyid AND empid = s.empid
	UNION
	SELECT companyid, avgscore, ScoreRank, monyrid, monyr, empid, managerid from #temp WHERE monyrid <> '' 
	
SET @YearId = CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END
BEGIN
	
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
			   e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, 
			   ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, e.currentsalary,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.ScoreRank, '') AS AvgScoreRank, ISNULL(vg.avgscore,0) AS AvgScore,
			   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearId) AS VARCHAR),'') AS RatingCompleted,0 AS ord,
			   ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM employeedetails e 
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempScore vg ON vg.empId = e.empid AND e.empstatus = 1 AND vg.monyr = @Month
		LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
		WHERE CASE WHEN ScoreRank IS NULL THEN '' ELSE ScoreRank END = @Grade
			AND e.companyid = @companyid AND e.bactive = 1 AND e.empstatus = 1
			AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
		ORDER BY e.lastname
	
END
END
----------

CREATE OR ALTER       PROCEDURE [dbo].[usp_GetYearlyAllOrgEmployeesWithGradeWithoutPagination] 
(
	@yearID INT,
	@companyid INT,
	@managerid INT,
	@grade VARCHAR(20),
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

	DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

	CREATE TABLE #tempEmpAverageAssessmentDet (
	companyid INT, empid INT, avgScore DECIMAL(2,0), AvgScoreRank VARCHAR(100), yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	SELECT * INTO #tempScore
	FROM
	(
		SELECT companyID, empid, Score, ManagerId,  FORMAT(RatingDate, 'yyyyMM') AS monyrID
		FROM vw_EmpAssessmentScore
		WHERE FORMAT(RatingDate, 'yyyyMM') > FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')  AND empStatus = 1 
	)t

	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT  companyID, empId, avgscore, avgScoreRank, 0 AS YearID
		FROM
		(
			SELECT companyID, empId, CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS DECIMAL(10,0)) AS avgscore,
			dbo.GetScoreRank(CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS decimal(10,0))) AS AvgScoreRank,
			row_number() OVER (PARTITION BY empid ORDER BY empid) AS rownumber
			FROM #tempScore 
		)v WHERE rownumber = 1
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT companyID , empID, avgScore, AvgScoreRank, year AS YearID FROM vw_EmpAverageAssessmentDet 
		WHERE companyid = @companyId AND year = @YearId
	)k
	END

	CREATE TABLE #tempEmpLastAssessmentDet (
	companyId INT,empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END
	SET @YearId = CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END

if ( @companyid > 0 AND @Grade <> '') 
BEGIN

	
		SELECT empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
			   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary, empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   LastScoreRank, RatingCompleted,managerLastAssessedDate
		FROM(
			SELECT e.empid, e.employeeId, e.companyid, e.firstname,  ISNULL(e.middlename, '') AS middlename,   e.lastname, e.email, 
			       ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE,  e.joiningdate,101) AS joiningdate,
			       e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, 
			       ISNULL(m.lastname, '') AS managerLName,  e.empstatus, e.roleid, e.currentsalary, ISNULL(e.empimgpath,'') AS empimgpath,
			       ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			       ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
			       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
			       ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted, 0 AS ord,
				   ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
				   
			FROM employeedetails e 
			LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
			LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
			LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid 
			LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
			WHERE e.companyid = @companyid AND e.bactive = 1  AND Format(e.joiningDate, 'yyyy') <= @YearID
			AND e.empstatus = 1 AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
			GROUP BY e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
					 e.departmentid, d.department, e.managerid, ISNULL(m.firstname, ''), ISNULL(m.lastname, ''), e.empstatus, e.roleid,
					 ISNULL(vg.AvgScoreRank, ''),vg.AvgScore, e.currentsalary ,ISNULL(e.empimgpath,''),assmnt.assessmentdate
					 
		) v
		WHERE CASE WHEN dbo.Get_grade_value(LastScoreRank) IS NULL THEN '' ELSE dbo.Get_grade_value(LastScoreRank) END = @Grade	
		ORDER BY empid DESC, lastname

END
ELSE 
BEGIN

	SELECT e.empid, e.employeeId, e.companyid, cd.companyName, e.firstname, e.middlename, e.lastname, e.email, jobtitle, joiningdate,
	       e.departmentid, d.department, e.managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath,
	       '' AS lastAssessedDate, 0 as AvgScore, '' AS AvgScoreRank, '' AS LastScoreRank, '' AS RatingCompleted,0 AS ord,
		   ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate

	FROM employeedetails e
	LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
	LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
	LEFT JOIN companydetails cd ON cd.compid = e.companyId AND cd.bactive = 1
	LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
	WHERE cd.bactive = 1 and e.roleid =2
		AND e.bactive = 1 AND cd.bactive = 1 and e.roleid =2
		AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
	ORDER BY e.empid DESC, cd.companyName, e.firstname
			
END
END
----------
CREATE OR ALTER   PROCEDURE [dbo].[usp_GetYearWiseAllOrgEmployees] 
(
	@YearID INT,
	@companyid INT,
	@managerid INT,
	@PageNumber INT,
	@PageSize   INT,
	@SearchString NVARCHAR(256),
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
DECLARE @TotalRowCount INT
SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

CREATE TABLE #tempEmpAverageAssessmentDet (
	companyid INT, empid INT, avgScore DECIMAL(2,0), AvgScoreRank VARCHAR(100), yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	SELECT * INTO #tempScore
	FROM
	(
		SELECT companyID, empid, Score, ManagerId,  FORMAT(RatingDate, 'yyyyMM') AS monyrID
		FROM vw_EmpAssessmentScore
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear  AND empStatus = 1 
	)t

	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT  companyID, empId, avgscore, avgScoreRank, 0 AS YearID
		FROM
		(
			SELECT companyID, empId, CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS DECIMAL(10,0)) AS avgscore,
			dbo.GetScoreRank(CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS decimal(10,0))) AS AvgScoreRank,
			row_number() OVER (PARTITION BY empid ORDER BY empid) AS rownumber
			FROM #tempScore 
		)v WHERE rownumber = 1
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT companyID , empID, ISNULL(AvgScore,0) AS avgScore, ISNULL(AvgScoreRank, '') AS AvgScoreRank, year AS YearID FROM vw_EmpAverageAssessmentDet 
		WHERE companyid = @companyId AND year = @YearId
	)k
	END

CREATE TABLE #tempEmpLastAssessmentDet (
	companyid INT, empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		--WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END

SET @YearID = CASE WHEN @YearID = 0 THEN YEAR(GETDATE()) ELSE @YearID END

	SELECT * INTO #tempEmployeedetailsC 
		FROM
		(
			SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
			   e.departmentid, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath
			FROM employeedetails e 
			LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
			WHERE e.companyid = @companyid AND e.bactive = 1 AND e.roleid <> 4 AND e.empstatus = 1 AND Format(e.joiningDate, 'yyyy') <= @YearID
			AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
		) v

	SELECT * INTO #tempEmployeeResultC
	FROM(
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename, e.lastname, e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE, e.joiningdate,101) AS joiningdate,
			   e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, e.managerFName, e.managerLName, e.empstatus, e.roleid, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
		       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS lastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted,
			   0 AS ord,ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM #tempEmployeedetailsC e
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid 
		LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
	)C
		SELECT @TotalRowCount = COUNT(empid) FROM  #tempEmployeeResultC
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount,empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
			   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary,empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   lastScoreRank, RatingCompleted,managerLastAssessedDate
		FROM  #tempEmployeeResultC
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY empid DESC, firstname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY 

END

----------
CREATE OR ALTER     PROCEDURE [dbo].[usp_GetYearlyMonAvgScoreWiseOrgEmp]  
(
	@YearId INT,
	@CompanyId INT,
	@ManagerId INT,
	@Grade VARCHAR(10),
	@Month VARCHAR(10),
	@PageNumber INT,
	@PageSize   INT,
	@SearchString NVARCHAR(256),
	@departmentlist VARCHAR(256)
)
AS 
BEGIN
DECLARE @TotalRowCount INT
SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

DECLARE @CurrYear INT = FORMAT(GETDATE(), 'yyyy')
DECLARE @LastYear INT = FORMAT(GETDATE(), 'yyyy') - 1
DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')

CREATE TABLE #tempEmpAssessmentScore (
	companyid INT, empid INT, RatingDate DATETIME, Score DECIMAL(2,0), managerId INT, yearId INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyid, empid,  RatingDate, score, managerid, YearID FROM vw_EmpAssessmentScore 
		WHERE  empStatus = 1 AND FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear  AND companyid = @companyId
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAssessmentScore
	SELECT * FROM
	(
		SELECT companyid, empid, RatingDate, score, managerid, YearID FROM vw_EmpAssessmentScore 
		WHERE  empStatus = 1 AND yearid = @YearId  AND companyid = @companyId
	)k
	END

	CREATE TABLE #tempEmpLastAssessmentDet (
	companyid INT, empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END

SELECT avgscore, ScoreRank, monyrid, monyr,empid,managerid, companyid INTO #temp FROM
(
	SELECT avgscore, dbo.get_grade_value(dbo.GETSCoreRank(avgscore)) AS ScoreRank, monyrid, monyr,empid,managerid, companyid
	FROM (
			SELECT DISTINCT CAST(ROUND(AVG(score) OVER (PARTITION BY empid, FORMAT(RatingDate,'yyyyMM')),0) AS DECIMAL(10,0)) AS avgscore,  empid,  
				   FORMAT(RatingDate,'yyyyMM') AS monyrid, FORMAT(RatingDate,'MMM') AS monyr, managerid, companyid
			FROM #tempEmpAssessmentScore 
	) v
) m


	CREATE TABLE #monthlist (yearmonth VARCHAR(20), monthname VARCHAR(20));

	IF(@YearId = 0)
	BEGIN
			INSERT INTO #monthlist
			SELECT yearmonth, monthname FROM
			(
				SELECT Format(getdate(),'yyyy')+FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MM') AS yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number between 1 and MONTH(GETDATE())
				UNION		
				SELECT  CAST(@LastYear AS VARCHAR) + FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@LastYear AS VARCHAR) AS DATE), 'MM') AS yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + Format(getdate(),'yyyy') AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number > MOnth(DATEADD(month,-12,GETDATE()))
			) v
		END
		ELSE
		BEGIN
			INSERT INTO #monthlist
			SELECT  yearmonth, monthname  
			FROM
			(
				SELECT CAST(@yearID AS VARCHAR) + FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@yearID AS VARCHAR) AS DATE), 'MM') AS yearmonth ,
				FORMAT(CAST(CAST(number AS VARCHAR) + '/01/' + CAST(@yearID AS VARCHAR) AS DATE), 'MMM') AS monthname
				FROM dbo.spt_values
				WHERE Type = 'P' and number between 1 and CASE WHEN @CurrYear <> @YearId THEN MONTH(DATEFROMPARTS(@yearID,12,31)) ELSE MONTH(GETDATE()) END
			)v
		END

	SELECT companyid, avgscore, ScoreRank, yearmonth AS monyrid, monthname  AS monyr, empid, managerid 
	INTO #tempScore 
	FROM #monthlist m 
	LEFT OUTER JOIN #temp s ON s.monyrid = (SELECT MAX(monyrid) FROM #temp WHERE monyrid < yearmonth AND companyid = s.companyid AND empid = s.empid)
	WHERE yearmonth NOT IN(SELECT monyrid FROM #temp WHERE monyrid <> '' AND companyid = s.companyid AND empid = s.empid) AND companyid = s.companyid AND empid = s.empid
	UNION
	SELECT companyid, avgscore, ScoreRank, monyrid, monyr, empid, managerid from #temp WHERE monyrid <> '' 
	
SET @YearId = CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END
	--SELECT TOP 1 @managerId = empid FROM employeedetails WHERE companyid =@companyid AND bactive = 1 AND empstatus = 1 AND roleid =2
	SELECT empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
		  departmentid, managerid, managerFName, managerLName, empstatus,  roleid, currentsalary,empimgpath
	INTO #tempEmployeedetails
	FROM
	(
		SELECT e.empid,  e.employeeId,  e.companyid,  e.firstname, ISNULL(e.middlename, '') AS middlename,  e.lastname,  e.email, ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE,  e.joiningdate,101) AS joiningdate,
			   e.departmentid, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, 
			   ISNULL(m.lastname, '') AS managerLName, e.empstatus,  e.roleid, e.currentsalary ,ISNULL(e.empimgpath,'') AS empimgpath
		FROM employeedetails e 
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
		WHERE e.companyid = @companyid AND e.bactive = 1 AND e.empstatus = 1
		AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
	) v
	SELECT * INTO #tempEmployeeResultC
	FROM(
		SELECT e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
			   e.departmentid, d.department, e.managerid, e.managerFName, e.managerLName, e.empstatus, e.roleid, e.currentsalary,e.empimgpath,
			   ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			   ISNULL(vg.ScoreRank, '') AS AvgScoreRank, ISNULL(vg.avgscore,0) AS AvgScore,
			   ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
			   ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearId) AS VARCHAR),'') AS RatingCompleted
			   ,0 AS ord,ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM #tempEmployeedetails e 
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN #tempScore vg ON vg.empId = e.empid AND e.empstatus = 1 AND vg.monyr = @Month
		LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
		WHERE CASE WHEN ScoreRank IS NULL THEN '' ELSE ScoreRank END = @Grade
	)s
	SELECT @TotalRowCount = COUNT(empid) FROM #tempEmployeeResultC
	WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

	SELECT @TotalRowCount AS totalrowcount,empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
		   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary,empimgpath,
		   lastAssessedDate, AvgScoreRank, AvgScore,
		   LastScoreRank, RatingCompleted,managerLastAssessedDate
	FROM #tempEmployeeResultC	
	WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
	ORDER BY firstname
	OFFSET @PageSize * (@PageNumber - 1) ROWS 
	FETCH NEXT @PageSize ROWS ONLY
END

----------

CREATE OR ALTER     PROCEDURE [dbo].[usp_GetYearlyAllEmployeesWithGradeWithPagination] 
(
	@yearID INT,
	@companyid INT,
	@managerid INT,
	@grade VARCHAR(20),
	@PageNumber INT,
	@PageSize   INT,
	@SearchString NVARCHAR(256),
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

	DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')
	DECLARE @TotalRowCount INT
	CREATE TABLE #tempEmpAverageAssessmentDet (
	companyid INT, empid INT, avgScore DECIMAL(2,0), AvgScoreRank VARCHAR(100), yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	SELECT * INTO #tempScore
	FROM
	(
		SELECT companyID, empid, Score, ManagerId,  FORMAT(RatingDate, 'yyyyMM') AS monyrID
		FROM vw_EmpAssessmentScore
		WHERE FORMAT(RatingDate, 'yyyyMM') > FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')  AND empStatus = 1 
	)t

	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT  companyID, empId, avgscore, avgScoreRank, 0 AS YearID
		FROM
		(
			SELECT companyID, empId, CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS DECIMAL(10,0)) AS avgscore,
			dbo.GetScoreRank(CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS decimal(10,0))) AS AvgScoreRank,
			row_number() OVER (PARTITION BY empid ORDER BY empid) AS rownumber
			FROM #tempScore 
		)v WHERE rownumber = 1
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT companyID , empID, avgScore, AvgScoreRank, year AS YearID FROM vw_EmpAverageAssessmentDet 
		WHERE companyid = @companyId AND year = @YearId
	)k
	END

	CREATE TABLE #tempEmpLastAssessmentDet (
	companyId INT,empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END
	SET @YearId = CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END

IF(@managerid > 0 and @companyid > 0 AND @Grade <> '') 
BEGIN  

	SELECT * INTO #tempEmployeeResult
	FROM
	(
		SELECT empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
			   departmentid, managerid,  empstatus,  roleid, department, managerFName, managerLName, currentsalary,empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   lastScoreRank, RatingCompleted,managerLastAssessedDate
		FROM(
				SELECT e.empid, e.employeeId, e.companyid, e.firstname, ISNULL(e.middlename, '') AS middlename,  e.lastname, e.email, 
				       ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE,  e.joiningdate,101) AS joiningdate,
				       e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, e.empstatus, e.roleid, 
				       ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath,
				       ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
				       ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
				       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
				       ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted,0 AS ord,
					   ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
				FROM employeedetails e 
				LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId 
				LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
				LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid 
				LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
				WHERE e.companyid = @companyid AND e.bactive = 1 AND e.managerid = @managerid AND Format(e.joiningDate, 'yyyy') <= @YearID
				AND e.empstatus = 1 AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
		) v1
		WHERE CASE WHEN dbo.Get_grade_value(LastScoreRank) IS NULL THEN '' ELSE dbo.Get_grade_value(LastScoreRank) END = @Grade
	)v
		
		SELECT @TotalRowCount = COUNT(empid) FROM #tempEmployeeResult
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount, empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
			   departmentid, managerid,  empstatus,  roleid, department, managerFName, managerLName, currentsalary,empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   lastScoreRank, RatingCompleted,managerLastAssessedDate
		FROM #tempEmployeeResult
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY empid DESC, lastname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY

		
END
ELSE if (@managerid = 0 and @companyid > 0 AND @Grade <> '') 
BEGIN

	SELECT * INTO #tempEmployeeResultCA
	FROM
	(
		SELECT empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
			   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary, empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   LastScoreRank, RatingCompleted
		FROM(
			SELECT e.empid, e.employeeId, e.companyid, e.firstname,  ISNULL(e.middlename, '') AS middlename,   e.lastname, e.email, 
			       ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE,  e.joiningdate,101) AS joiningdate,
			       e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, 
			       ISNULL(m.lastname, '') AS managerLName,  e.empstatus, e.roleid, e.currentsalary, ISNULL(e.empimgpath,'') AS empimgpath,
			       ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			       ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
			       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
			       ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted, 0 AS ord
			FROM employeedetails e 
			LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
			LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
			LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid 
			WHERE e.companyid = @companyid AND e.bactive = 1  AND Format(e.joiningDate, 'yyyy') <= @YearID
			AND e.empstatus = 1 AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
			GROUP BY e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
					 e.departmentid, d.department, e.managerid, ISNULL(m.firstname, ''), ISNULL(m.lastname, ''), e.empstatus, e.roleid,
					 ISNULL(vg.AvgScoreRank, ''),vg.AvgScore, e.currentsalary ,ISNULL(e.empimgpath,'')
					 
		) v1
	WHERE CASE WHEN dbo.Get_grade_value(LastScoreRank) IS NULL THEN '' ELSE dbo.Get_grade_value(LastScoreRank) END = @Grade	
	)v
		SELECT @TotalRowCount = COUNT(empid) FROM #tempEmployeeResultCA
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount, empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
			   departmentid, managerid,  empstatus,  roleid, department, managerFName, managerLName, currentsalary,empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   lastScoreRank, RatingCompleted
		FROM #tempEmployeeResultCA
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY empid DESC, lastname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY
		--ORDER BY empid DESC, lastname

END
ELSE 
BEGIN
	SELECT * INTO #tempEmployeeResultTA
	FROM
	(
		SELECT e.empid, e.employeeId, e.companyid, cd.companyName, e.firstname, e.middlename, e.lastname, e.email, jobtitle, joiningdate,
			   e.departmentid, d.department, e.managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath,
			   '' AS lastAssessedDate, 0 as AvgScore, '' AS AvgScoreRank, '' AS LastScoreRank, '' AS RatingCompleted,0 AS ord
		FROM employeedetails e
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN companydetails cd ON cd.compid = e.companyId AND cd.bactive = 1
		WHERE cd.bactive = 1 and e.roleid =2
			AND e.bactive = 1 AND cd.bactive = 1 and e.roleid =2
			AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
	)v
		
		SELECT @TotalRowCount = COUNT(empid) FROM #tempEmployeeResultTA
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount, empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
			   departmentid, managerid,  empstatus,  roleid, department, managerFName, managerLName, currentsalary,empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   lastScoreRank, RatingCompleted
		FROM #tempEmployeeResultTA
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY empid DESC, lastname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY
		--ORDER BY e.empid DESC, cd.companyName, e.firstname
			
END
END

----------
CREATE OR ALTER     PROCEDURE [dbo].[usp_GetYearlyAllOrgEmployeesWithGradeWithPagination] 
(
	@yearID INT,
	@companyid INT,
	@managerid INT,
	@grade VARCHAR(20),
	@PageNumber INT,
	@PageSize   INT,
	@SearchString NVARCHAR(256),
	@departmentlist VARCHAR(256)
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	SELECT department into #departmentlist FROM (SELECT value AS department FROM  STRING_SPLIT(@departmentlist, ',')) d

	DECLARE @Last12MonthYear  VARCHAR(10)  = FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')
	DECLARE @TotalRowCount INT
	CREATE TABLE #tempEmpAverageAssessmentDet (
	companyid INT, empid INT, avgScore DECIMAL(2,0), AvgScoreRank VARCHAR(100), yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	SELECT * INTO #tempScore
	FROM
	(
		SELECT companyID, empid, Score, ManagerId,  FORMAT(RatingDate, 'yyyyMM') AS monyrID
		FROM vw_EmpAssessmentScore
		WHERE FORMAT(RatingDate, 'yyyyMM') > FORMAT(DATEADD(month,-12,GETDATE()),'yyyyMM')  AND empStatus = 1 
	)t

	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT  companyID, empId, avgscore, avgScoreRank, 0 AS YearID
		FROM
		(
			SELECT companyID, empId, CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS DECIMAL(10,0)) AS avgscore,
			dbo.GetScoreRank(CAST(ROUND(AVG(score) OVER (PARTITION BY empid),0) AS decimal(10,0))) AS AvgScoreRank,
			row_number() OVER (PARTITION BY empid ORDER BY empid) AS rownumber
			FROM #tempScore 
		)v WHERE rownumber = 1
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpAverageAssessmentDet
	SELECT * FROM
	(
		SELECT companyID , empID, avgScore, AvgScoreRank, year AS YearID FROM vw_EmpAverageAssessmentDet 
		WHERE companyid = @companyId AND year = @YearId
	)k
	END

	CREATE TABLE #tempEmpLastAssessmentDet (
	companyId INT,empid INT, Score DECIMAL(2,0), ScoreRank VARCHAR(100), RatingDate DATETIME, MonyrYearID INT, yearID INT
	);

	IF(@YearId = 0)
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet
		WHERE FORMAT(RatingDate, 'yyyyMM') > @Last12MonthYear 
	)k
	END
	ELSE
	BEGIN
	INSERT INTO #tempEmpLastAssessmentDet
	SELECT * FROM
	(
		SELECT companyId, empId, Score, ScoreRank, RatingDate, MonYearID, yearID FROM vw_EmpLastAssessmentDet WHERE yearId = @YearID
	)k
	END
	SET @YearId = CASE WHEN @YearId = 0 THEN YEAR(GETDATE()) ELSE @YearID END

 if ( @companyid > 0 AND @Grade <> '') 
BEGIN

	SELECT * INTO #tempEmployeeResultCA
	FROM
	(
		SELECT empid, employeeId, companyid, firstname, middlename, lastname, email, jobtitle, joiningdate,
			   departmentid, department, managerid, managerFName, managerLName, empstatus, roleid, currentsalary, empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   LastScoreRank, RatingCompleted,managerLastAssessedDate
		FROM(
			SELECT e.empid, e.employeeId, e.companyid, e.firstname,  ISNULL(e.middlename, '') AS middlename,   e.lastname, e.email, 
			       ISNULL(e.jobtitle,'') AS jobtitle, CONVERT(DATE,  e.joiningdate,101) AS joiningdate,
			       e.departmentid, d.department, ISNULL(e.managerid,0) AS managerid, ISNULL(m.firstname, '') AS managerFName, 
			       ISNULL(m.lastname, '') AS managerLName,  e.empstatus, e.roleid, e.currentsalary, ISNULL(e.empimgpath,'') AS empimgpath,
			       ISNULL(CONVERT(VARCHAR,(SELECT ISNULL(MAX(assessmentdate), NULL) AS lastAssessedDate FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID), 101),'') AS lastAssessedDate, 
			       ISNULL(vg.AvgScoreRank, '') AS AvgScoreRank, ISNULL(vg.AvgScore,0) AS AvgScore,
			       ISNULL((SELECT Top 1 ISNULL(ScoreRank, '') AS ScoreRank From #tempEmpLastAssessmentDet WHERE empid = e.empid ORDER BY RatingDate DESC),'') AS LastScoreRank,
			       ISNULL(CAST((SELECT ISNULL(COUNT(assessmentdate), NULL) AS RatingCompleted FROM employee_assessment WHERE empid = e.empid AND DATEPART(yyyy,assessmentdate) = @YearID) AS VARCHAR),'') AS RatingCompleted, 0 AS ord,
				   ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
			FROM employeedetails e 
			LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
			LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
			LEFT JOIN #tempEmpAverageAssessmentDet vg ON vg.empId = e.empid 
			LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
			WHERE e.companyid = @companyid AND e.bactive = 1  AND Format(e.joiningDate, 'yyyy') <= @YearID
			AND e.empstatus = 1 AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
			GROUP BY e.empid, e.employeeId, e.companyid, e.firstname, e.middlename, e.lastname, e.email, e.jobtitle, e.joiningdate,
					 e.departmentid, d.department, e.managerid, ISNULL(m.firstname, ''), ISNULL(m.lastname, ''), e.empstatus, e.roleid,
					 ISNULL(vg.AvgScoreRank, ''),vg.AvgScore, e.currentsalary ,ISNULL(e.empimgpath,''),
					 assmnt.assessmentdate
					 
		) v1
	WHERE CASE WHEN dbo.Get_grade_value(LastScoreRank) IS NULL THEN '' ELSE dbo.Get_grade_value(LastScoreRank) END = @Grade	
	)v
		SELECT @TotalRowCount = COUNT(empid) FROM #tempEmployeeResultCA
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount, empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
			   departmentid, managerid,  empstatus,  roleid, department, managerFName, managerLName, currentsalary,empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   lastScoreRank, RatingCompleted,managerLastAssessedDate,managerLastAssessedDate
		FROM #tempEmployeeResultCA
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY empid DESC, lastname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY
		

END
ELSE 
BEGIN
	SELECT * INTO #tempEmployeeResultTA
	FROM
	(
		SELECT e.empid, e.employeeId, e.companyid, cd.companyName, e.firstname, e.middlename, e.lastname, e.email, jobtitle, joiningdate,
			   e.departmentid, d.department, e.managerid, ISNULL(m.firstname, '') AS managerFName, ISNULL(m.lastname, '') AS managerLName, e.empstatus, e.roleid, e.currentsalary,ISNULL(e.empimgpath,'') AS empimgpath,
			   '' AS lastAssessedDate, 0 as AvgScore, '' AS AvgScoreRank, '' AS LastScoreRank, '' AS RatingCompleted,0 AS ord,
			   ISNULL(CONVERT(VARCHAR,assmnt.assessmentdate,101),'') as managerLastAssessedDate
		FROM employeedetails e
		LEFT JOIN EmployeeDetails m ON m.bactive = 1 AND m.empId = e.managerId
		LEFT JOIN department d ON d.departmentid = e.departmentid AND d.bactive = 1
		LEFT JOIN companydetails cd ON cd.compid = e.companyId AND cd.bactive = 1
		LEFT JOIN GetLastAssessmentDateByManager(@Managerid) assmnt on assmnt.empid=e.empid
		WHERE cd.bactive = 1 and e.roleid =2
			AND e.bactive = 1 AND cd.bactive = 1 and e.roleid =2
			AND (1=(CASE WHEN @departmentlist = '0' THEN 1 ELSE 0 END) OR e.departmentId IN (SELECT department FROM #departmentlist))
	)v
		
		SELECT @TotalRowCount = COUNT(empid) FROM #tempEmployeeResultTA
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')

		SELECT @TotalRowCount AS totalrowcount, empid,  employeeId,  companyid,  firstname, middlename,  lastname,  email, jobtitle, joiningdate,
			   departmentid, managerid,  empstatus,  roleid, department, managerFName, managerLName, currentsalary,empimgpath,
			   lastAssessedDate, AvgScoreRank, AvgScore,
			   lastScoreRank, RatingCompleted,managerLastAssessedDate
		FROM #tempEmployeeResultTA
		WHERE (1=(CASE WHEN @SearchString = 'all records' THEN 1 ELSE 0 END) 
			OR lastname LIKE '%'+ @SearchString + '%'
			OR firstname LIKE '%'+ @SearchString + '%'
			OR firstname + ' ' + lastname LIKE '%'+ @SearchString + '%')
		ORDER BY empid DESC, lastname
		OFFSET @PageSize * (@PageNumber - 1) ROWS 
		FETCH NEXT @PageSize ROWS ONLY
				
END
END


----------