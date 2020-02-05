CREATE TRIGGER [dbo].[TRG_EMPLOYEE_DETAIL] ON  [dbo].[EmployeeDetails]
AFTER INSERT
AS
BEGIN
	--For Manager
	INSERT INTO NotificationsLogs(Empid,message,managerid,action,[IsSent],[MarkAs],[CreatedBy],[CreatedDtStamp],[Type],reportingtoid)
	SELECT empid, [firstname] + ' Added Under You.',managerid,'Added',0,0,managerid,getdate(),'Employee',(SELECT managerid FROM employeedetails WHERE empid= i.managerid) 
	FROM INSERTED i
	
	--For CEO
	INSERT into NotificationsLogs(Empid,ManagerId,Message,action,[IsSent],[MarkAs],[CreatedBy],[CreatedDtStamp],[Type],reportingtoid)  
	SELECT e.ManagerId,reportingtoid,CAST(count(e.ManagerId) AS VARCHAR) + ' Employee Added Under ' + emp.firstname + '.','Added',0,0,0,getdate(),'Employee',0
	FROM (SELECT managerid,(SELECT managerid from employeedetails where empid= i.managerid) AS reportingtoid FROM inserted i) e
	JOIN employeedetails emp ON e.managerid = emp.empid
	WHERE reportingtoid > 0	GROUP BY reportingtoid,e.managerid,emp.firstname

END
----------
CREATE TRIGGER [dbo].[TRG_EMPLOYEE_DETAIL_UPD] ON  [dbo].[EmployeeDetails]
AFTER UPDATE
AS
BEGIN   
	---For Manager
	INSERT INTO [NotificationsLogs](Empid,message,managerid,action,[IsSent],[MarkAs],[CreatedBy],[CreatedDtStamp],[Type],[reportingtoid])
	SELECT empid,  [firstname] +' Added Under You.' ,managerid,'Added',0,0,managerid,GETDATE(),'Employee',(SELECT managerid FROM employeedetails WHERE empid= i.managerid)
	FROM INSERTED i WHERE 1 = (CASE WHEN managerid=(SELECT d.managerid FROM deleted d WHERE d.empid=i.empid) THEN 0 ELSE 1 END)

	INSERT INTO [NotificationsLogs](Empid,message,managerid,action,[IsSent],[MarkAs],[CreatedBy],[CreatedDtStamp],[Type],[reportingtoid])
	SELECT empid,  [firstname] +' Removed From You' ,(SELECT d.managerid FROM deleted d WHERE d.empid=i.empid),'Removed',0,0,managerid,GETDATE(),'Employee',(SELECT managerid FROM employeedetails WHERE empid= (SELECT d.managerid FROM deleted d WHERE d.empid=i.empid))
	FROM INSERTED i WHERE 1 = (CASE WHEN managerid=(SELECT d.managerid FROM deleted d WHERE d.empid=i.empid) THEN 0 ELSE 1 END)

	---For CEO
    INSERT INTO NotificationsLogs(Empid,ManagerId,Message,action,[IsSent],[MarkAs],[CreatedBy],[CreatedDtStamp],[Type],reportingtoid)
	SELECT e.managerid,reportingtoid,CAST(count(action) AS VARCHAR) + ' Employee ' + action + CASE WHEN action = 'Added' THEN ' Under ' ELSE ' From ' end + emp.firstname + '.',action,0,0,0,getdate(),'Employee',0
	FROM (
			SELECT managerid, 'Added' as action,(SELECT managerid FROM employeedetails WHERE empid= i.managerid) as reportingtoid
			FROM INSERTED i WHERE 1 = (case when managerid=(SELECT d.managerid from deleted d WHERE d.empid=i.empid) THEN 0 ELSE 1 END)  
		Union all
			SELECT (SELECT d.managerid FROM deleted d where d.empid=i.empid) as managerid,'Removed' as action,
			(SELECT managerid FROM employeedetails where empid= (SELECT d.managerid FROM deleted d WHERE d.empid=i.empid)) as reportingtoid  
			FROM INSERTED i WHERE 1 = (CASE WHEN managerid=(SELECT d.managerid FROM deleted d WHERE d.empid=i.empid) THEN 0 ELSE 1 END)
		)e join employeedetails emp ON e.managerid = emp.empid
	WHERE reportingtoid > 0
	GROUP BY reportingtoid,e.managerid,action,emp.firstname
END

----------

