CREATE FUNCTION [dbo].[StringSplit]
(
    @String  VARCHAR(MAX), @Separator CHAR(1)
)
RETURNS @RESULT TABLE(Value VARCHAR(MAX))
AS
BEGIN     
 DECLARE @SeparatorPosition INT = CHARINDEX(@Separator, @String ),
        @Value VARCHAR(MAX), @StartPosition INT = 1
 
 IF @SeparatorPosition = 0  
  BEGIN
   INSERT INTO @RESULT VALUES(@String)
   RETURN
  END
     
 SET @String = @String + @Separator
 WHILE @SeparatorPosition > 0
  BEGIN
   SET @Value = SUBSTRING(@String , @StartPosition, @SeparatorPosition- @StartPosition)
 
   IF( @Value <> ''  ) 
    INSERT INTO @RESULT VALUES(@Value)
   
   SET @StartPosition = @SeparatorPosition + 1
   SET @SeparatorPosition = CHARINDEX(@Separator, @String , @StartPosition)
  END    
     
 RETURN
END
----------

CREATE FUNCTION [dbo].[Get_grade_value]
(
	@grade varchar(5)
)
RETURNS varchar(5)
AS
BEGIN
	-- Declare the return variable here

	DECLARE @gradevalue varchar(5)
	DECLARE @gradeid int


	-- Add the T-SQL statements to compute the return value here
	SELECT @gradeid  = floor(gradeorder)  FROM scoreremarks s 
	WHERE scorerank=@grade

	select @gradevalue=scorerank from scoreremarks where
	id = @gradeid
	-- Return the result of the function
	RETURN @gradevalue

END
----------

CREATE FUNCTION [dbo].[GetEmpNameById]
(
	@EmpId INT,
	@FirstName varchar(100),
	@MiddleName varchar(100),
	@LastName varchar(100)
)
RETURNS VARCHAR(300)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @empname VARCHAR(300)

	IF @EmpId IS NULL 
		SELECT  @empname =ISNULL(@FirstName,'') + CASE WHEN ISNULL(@MiddleName,'') <> '' THEN ' ' +
		ISNULL(@MiddleName,'') + ' ' ELSE ' ' END + ISNULL(@LastName,'')  
	ELSE
		SELECT  @empname =ISNULL(firstName,'') + CASE WHEN ISNULL(MiddleName,'') <> '' THEN ' ' +
		ISNULL(MiddleName,'') + ' ' ELSE ' ' END + ISNULL(lastName,'')   FROM employeedetails WHERE empid=@EmpId
			
	RETURN @empname
END

----------

CREATE FUNCTION [dbo].[GetRole] 
(
	@userid INT
)
RETURNS INT
AS
BEGIN
	-- Declare the return variable here
	DECLARE @empid INT
	DECLARE @roleid INT

	SELECT @empid = empid FROM userlogin WHERE userid = @userid AND bactive = 1
	
	-- Add the T-SQL statements to compute the return value here
	SELECT @roleid  = roleid  FROM employeedetails WHERE empid = @empid AND bactive = 1

	-- Return the result of the function
	RETURN @roleId

END
----------

CREATE FUNCTION [dbo].[GetRoleByEmpid] 
(
	@empid INT
)
RETURNS INT
AS
BEGIN
	-- Declare the return variable here

	DECLARE @roleid INT


	-- Add the T-SQL statements to compute the return value here
	SELECT @roleid  = roleid  FROM employeedetails WHERE empid = @empid AND bactive = 1

	-- Return the result of the function
	RETURN @roleId

END

----------

CREATE OR ALTER FUNCTION [dbo].[GetScoreRank]
(
@score INT
)
RETURNS VARCHAR(5)
AS
BEGIN
-- Declare the return variable here
DECLARE @scoreRank VARCHAR(5)

--Added by Vivek Bhavsar on 24-12-2018, take socrerank from master table "scoreremarks" using score range fromscore & toscore
IF @score IS NULL 
	SET @scoreRank=''
IF @score < 0 
	SELECT  @scoreRank =scorerank FROM scoreremarks WHERE fromscore = 0
ELSE
	SELECT  @scoreRank =scorerank FROM scoreremarks WHERE @score BETWEEN fromscore AND toscore

-- Return the result of the function
RETURN @scoreRank

END
----------

CREATE FUNCTION [dbo].[GetScoreRankById]
(
	@gradeid INT
)
RETURNS VARCHAR(5)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @scoreRank VARCHAR(5)

	SELECT  @scoreRank =scorerank FROM scoreremarks WHERE id=@gradeid

		-- Return the result of the function
	RETURN @scoreRank

END


----------

CREATE OR ALTER FUNCTION [dbo].[GetScoreRankID]
(
@score INT
)
RETURNS VARCHAR(5)
AS
BEGIN
-- Declare the return variable here
DECLARE @scoreid int

IF @score IS NULL
BEGIN 
	set @scoreid=0
END
ELSE IF  @score < 0 
BEGIN
	SELECT @scoreid= id FROM scoreremarks WHERE fromscore = 0
END
ELSE
BEGIN 
	SELECT  @scoreid =id FROM scoreremarks WHERE @score BETWEEN fromscore AND toscore
END
-- Return the result of the function
RETURN @scoreid
END
----------
CREATE  OR ALTER  FUNCTION [dbo].[GetInactivityDays]
(
	@managerid INT
)
RETURNS INT
AS
BEGIN
	DECLARE @days INT

	SELECT  @days = DATEDIFF(DAY,(SELECT MAX(assessmentdate) FROM Employee_Assessment a
	LEFT JOIN Userlogin u ON u.userid=a.assessmentby
		WHERE u.empid=@managerid),GETDATE())

	IF @days IS NULL
	 SELECT @days=DATEDIFF(DAY,joiningdate,GETDATE()) FROM EmployeeDetails
	 WHERE empid=@managerid
	
	RETURN (CASE WHEN @days=0 THEN -1 ELSE @days END)

END
----------
CREATE	OR ALTER FUNCTION [dbo].[GetDimensionValueByID] (@dimensionid INT,@dimensionvalueid INT)
RETURNS VARCHAR(50)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @dimensionvalue VARCHAR(50)

	SELECT @dimensionvalue = Dimensionvalues
	FROM DimensionElements
	WHERE dimensionid = @dimensionid
		AND dimensionvalueid = @dimensionvalueid
		AND bActive = 1

	RETURN @dimensionvalue
END
----------
CREATE OR ALTER FUNCTION [dbo].[GetEmployeeName]
(
    -- Add the parameters for the function here
    @EmpId INT
)
RETURNS VARCHAR(300)
AS
BEGIN
    -- Declare the return variable here
    DECLARE @Empname VARCHAR(300)

	SELECT  @Empname =ISNULL(firstName,'') + CASE WHEN ISNULL(MiddleName,'') <> '' THEN ' ' +
		ISNULL(MiddleName,'') + ' ' ELSE ' ' END + ISNULL(lastName,'')   FROM employeedetails WHERE empid=@EmpId
			
	RETURN @Empname
END
----------

