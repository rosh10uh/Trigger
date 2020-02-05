CREATE VIEW  [dbo].[vw_EmpAssessmentScore]
AS
SELECT e.CompanyId, e.Empid, dbo.[GetEmpNameById](null,e.firstname,e.middlename,e.lastname) AS empName,CAST(a.score as decimal(2,0)) AS score, scoreid, d.scorerank as ScoreRank, 
d.ScoreRemarks,d.GeneralScoreRank, d.ManagerAction, d.ScoreSummary, a.assessmentdate AS RatingDate, a.Assessmentby AS Assessmentbyid,
dbo.[GetEmpNameById](u.empid,null,null,null) AS Assessmentby, a.id as assessmentId, managerId, departmentid,roleid,CAST(null as int) AS inDirectManager, e.empstatus,
DATEPART(yyyy,a.assessmentdate) AS yearId
FROM employee_assessment a
LEFT JOIN employeedetails e ON e.empid = a.empid 
LEFT JOIN userlogin u ON a.Assessmentby = u.userid
LEFT JOIN scoreremarks d ON a.scoreid= d.id
WHERE  d.bactive =1 AND e.bactive = 1
	
----------

CREATE VIEW [dbo].[vw_EmpAverageAssessmentDet]
AS
SELECT companyid, empid, empName, avgScore, [dbo].[GetScoreRank](avgscore) AS AvgScoreRank, year 
FROM
(
	SELECT  k.empid, k.companyid, k.empName, CAST(ROUND(AVG(k.avgScore) OVER (PARTITION BY year, k.empid),0) AS INT) AS avgScore, year
	FROM            
	(
		SELECT  l.empid, l.companyid, l.empName, AVG(l.Score) AS avgScore, yearId AS year
		FROM            
		(
			SELECT e.CompanyId, e.Empid, dbo.[GetEmpNameById](null,e.firstname,e.middlename,e.lastname) empName,CAST(a.score as decimal(2,0))score,
					DATEPART(yyyy,a.assessmentdate)yearId
			FROM employee_assessment a
			LEFT  JOIN employeedetails e on e.empid= a.empid
			WHERE e.empstatus=1 and a.bactive=1
		) l
	GROUP BY  l.empid, l.companyid, l.empName, yearId
	) AS k 
GROUP BY  k.companyid, k.empid, k.empName, year,k.avgScore
) k


----------

CREATE VIEW [dbo].[vw_EmpAverageAssessmentDetForDashBoard]
AS
SELECT companyid, empid, empName, avgScore,[dbo].[GetScoreRank](avgScore) AS AvgScoreRank, yearid
FROM
(
	SELECT k.companyid, k.empid, k.empName, CAST(ROUND(AVG(k.Score),0) AS INT) AS avgScore, yearid
	FROM            
	(
		SELECT e.CompanyId, e.Empid, dbo.[GetEmpNameById](null,e.firstname,e.middlename,e.lastname) AS empName,
		CAST(a.score as decimal(2,0)) AS score, DATEPART(yyyy,a.assessmentdate) AS yearId
		FROM employee_assessment a
		LEFT JOIN employeedetails e on e.empid= a.empid
		WHERE  e.bactive =1 and a.bactive=1
	) AS k 
GROUP BY  k.companyid, k.empid, k.empName, k.yearid
) v
----------

CREATE VIEW [dbo].[vw_EmpLastAssessmentDet]
AS
SELECT e.CompanyId, a.Empid, dbo.[GetEmpNameById](null,e.firstname,e.middlename,e.lastname) AS empName,
  CAST(a.score AS DECIMAL(2,0)) AS score, d.scorerank AS ScoreRank, 
  d.GeneralScoreRank, d.ScoreRemarks AS ScoreRemarks, 
  d.ManagerAction, d.ScoreSummary,
  CONVERT(DATE, a.assessmentdate, 101) AS RatingDate, a.Assessmentby AS Assessmentbyid,'' Assessmentby,
  Format(a.assessmentdate, 'yyyyMM') AS MonYearID, Year(a.assessmentdate) AS yearID
FROM employee_assessment a
LEFT JOIN employeedetails e ON e.bactive =1 AND e.empid= a.empid
LEFT JOIN scoreremarks d ON d.bactive =1 AND a.scoreid = d.id
WHERE a.bactive=1 AND e.bactive  = 1
----------

CREATE VIEW [dbo].[vw_AssessmentScoreCalculation]
AS
	SELECT k.empid,k.Score,k.ScoreId,assessmentid
	FROM            
	(
			SELECT a.Empid, v.Score, [dbo].[GetScoreRankID](v.Score) AS ScoreId, a.id Assessmentid
			FROM  dbo.employee_assessment AS a 
		    LEFT JOIN employeedetails e ON a.empid=e.empid
			INNER JOIN
			(
					SELECT CASE WHEN IsForContract = 1 AND (Score>=40 AND Score <= 47) THEN Score + 8 
					WHEN IsForContract = 2 AND Score>=48 THEN Score - (Score - 40) 
					ELSE Score END AS Score, assessmentid
					FROM
					(
						SELECT SUM(p.weightage) +  Mwightage AS Score, assessmentid, SUM(IsForContract) AS IsForContract
						FROM
						(
							SELECT w.weightage, k.wightage AS Mwightage, ad.assessmentid, ad.answerId,
							CASE WHEN ad.answerId = 71 THEN 1 WHEN ad.answerId = 72 THEN 2 ELSE 0 END AS IsForContract
							FROM dbo.assessmentdetails AS ad 
							INNER JOIN dbo.answersconfig AS w ON ad.questionid = w.questionid AND ad.answerid = w.id AND w.bactive = ad.bactive
							LEFT JOIN
							(                        
									SELECT assessmentid, SUM(w1) * SUM(w2) AS wightage
									FROM
									(
										SELECT at.assessmentid, w.weightage AS w1, NULL AS w2 
										FROM assessmentdetails at 
										INNER JOIN dbo.answersconfig AS w ON at.questionid = w.questionid AND at.answerid = w.id AND w.bactive = at.bactive AND w.questionid = 9 --and at.assessmentid = 1
										UNION
										SELECT at.assessmentid, NULL AS w1, w.weightage AS w2
										FROM assessmentdetails at 
										INNER JOIN dbo.answersconfig AS w ON at.questionid = w.questionid AND at.answerid = w.id AND w.bactive = at.bactive AND w.questionid = 10--and at.assessmentid = 1
									) v
									GROUP BY assessmentid
							) k ON k.assessmentid = ad.assessmentid
							WHERE ad.questionid NOT IN (9,10) 
						) p 
							GROUP BY p.assessmentid, p.Mwightage
					) v1
					) AS v ON v.assessmentid = a.id and e.bactive=1 and a.bactive=1
) AS k 

----------


