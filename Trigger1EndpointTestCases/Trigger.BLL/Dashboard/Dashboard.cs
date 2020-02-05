using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.BLL.Shared.Interfaces;
using Trigger.DAL;
using Trigger.DAL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.DTO.Spark;
using Trigger.Utility;

namespace Trigger.BLL.Dashboard
{
    public class Dashboard
    {
        private readonly ILogger<Dashboard> _logger;
        private readonly IConnectionContext _connectionContext;
        private readonly string _blobContainerEmployee = Messages.profilePic;
        private readonly IClaims _iClaims;
        private readonly TriggerCatalogContext _catalogDbContext;
        private readonly IActionPermission _permission;
<<<<<<< HEAD
        private readonly TeamDashboard _teamDashboard;

        public Dashboard(IConnectionContext connectionContext, ILogger<Dashboard> logger, IClaims Claims, TriggerCatalogContext catalogDbContext, IActionPermission permission, TeamDashboard teamDashboard)
=======
        private readonly AppSettings _appSettings;

        public Dashboard(IConnectionContext connectionContext, ILogger<Dashboard> logger,
            IClaims Claims, TriggerCatalogContext catalogDbContext, 
            IActionPermission permission, IOptions<AppSettings> appSettings)
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        {
            _connectionContext = connectionContext;
            _logger = logger;
            _iClaims = Claims;
            _catalogDbContext = catalogDbContext;
            _permission = permission;
<<<<<<< HEAD
            _teamDashboard = teamDashboard;
=======
            _appSettings = appSettings.Value;
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        }

        /// <summary>
        /// get employee dashboard data as per action permission given for loggedin user
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetEmployeeDashBoardAsyncV2(int employeeId)
        {
            try
            {
                bool canView = false;
                if (Convert.ToInt32(_iClaims["RoleId"].Value) != Enums.DimensionElements.CompanyAdmin.GetHashCode())
                {
                    var permissions = _permission.GetPermissions(Convert.ToInt32(_iClaims["EmpId"].Value)).Where(x => x.ActionId == Enums.Actions.EmployeeDashboard.GetHashCode()).ToList();
                    if (permissions.Count > 0)
                    {
                        canView = permissions.FirstOrDefault(x => x.ActionId == Enums.Actions.EmployeeDashboard.GetHashCode()).ActionPermissions.Any(x => x.CanView);
                    }
                }
                if (Convert.ToInt32(_iClaims["RoleId"].Value) == Enums.DimensionElements.CompanyAdmin.GetHashCode() || canView)
                {
                    var employeeDashboardModel = new EmployeeDashboardModel { empId = employeeId };
                    var employeeDashboard = await Task.FromResult(_connectionContext.TriggerContext.EmployeeDashboardRepository.GetEmployeeDashboard(employeeDashboardModel));
                    DataTable dtbEmployeeDashBoard = ConvertToDataTable.ToDataTable(employeeDashboard);
                    List<EmpDashboard> empDashboard = GetEmpDashboard(dtbEmployeeDashBoard);
                    if (empDashboard.Count == 0)
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(empDashboard, Convert.ToInt32(Enums.StatusCodes.status_404), Messages.notInitiatedAssessmentEmployee);
                    }
                    else
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(empDashboard, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(new List<EmpDashboard>(), Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   GetEmployeeDashBoardAsyncV2_1
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   10 August 2019
        /// Purpose      :   Version 2.1 : get employee dashboard data as per action permission given for loggedin user
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> GetEmployeeDashBoardAsyncV2_1(int employeeId)
        {
            try
            {
                bool canView = _permission.CheckActionPermission(_permission.GetPermissionParameters(Enums.Actions.EmployeeDashboard, Enums.PermissionType.CanView));

                if (canView)
                {
                    var employeeDashboardModel = new EmployeeDashboardModel { empId = employeeId };
                    var employeeDashboard = await Task.FromResult(_connectionContext.TriggerContext.EmployeeDashboardRepository.GetEmployeeDashboard(employeeDashboardModel));
                    DataTable dtbEmployeeDashBoard = ConvertToDataTable.ToDataTable(employeeDashboard);
                    List<EmpDashboard> empDashboard = GetEmpDashboard(dtbEmployeeDashBoard);
                    if (empDashboard.Count == 0)
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(empDashboard, Convert.ToInt32(Enums.StatusCodes.status_404), Messages.notInitiatedAssessmentEmployee);
                    }
                    else
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(empDashboard, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(new List<EmpDashboard>(), Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Method Name  :   GetEmployeeDashBoardAsyncV2_2
        /// Author       :   Bhumika Bhavsar
        /// Creation Date:   10 September 2019
        /// Purpose      :   Version 2.2 : get employee dashboard data as per action permission given for loggedin user
        /// </summary>
        /// <param name="employeeId"></param>
        /// <returns></returns>
        public async Task<CustomJsonData> GetEmployeeDashBoardAsyncV2_2(int employeeId)
        {
            try
            {
                bool canView = _permission.CheckActionPermissionV2_1(_permission.GetPermissionParameters(Enums.Actions.EmployeeDashboard, Enums.PermissionType.CanView));

                if (canView)
                {
                    var employeeDashboardModel = new EmployeeDashboardModel { empId = employeeId };
                    var employeeDashboard = await Task.FromResult(_connectionContext.TriggerContext.EmployeeDashboardRepository.GetEmployeeDashboard(employeeDashboardModel));
                    DataTable dtbEmployeeDashBoard = ConvertToDataTable.ToDataTable(employeeDashboard);
                    List<EmpDashboard> empDashboard = GetEmpDashboard(dtbEmployeeDashBoard);
                    if (empDashboard.Count == 0)
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(empDashboard, Convert.ToInt32(Enums.StatusCodes.status_404), Messages.notInitiatedAssessmentEmployee);
                    }
                    else
                    {
                        return JsonSettings.UserCustomDataWithStatusMessage(empDashboard, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                    }
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(new List<EmpDashboard>(), Enums.StatusCodes.status_403.GetHashCode(), Messages.employeeAccessDenied);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }
               

        public virtual async Task<CustomJsonData> GetEmployeeDashBoardAsyncV1(int employeeId)
        {
            try
            {
                var employeeDashboardModel = new EmployeeDashboardModel { empId = employeeId };
                var employeeDashboard = await Task.FromResult(_connectionContext.TriggerContext.EmployeeDashboardRepository.GetEmployeeDashboard(employeeDashboardModel));
                DataTable dtbEmployeeDashBoard = ConvertToDataTable.ToDataTable(employeeDashboard);
                List<EmpDashboard> empDashboard = GetEmpDashboard(dtbEmployeeDashBoard);
                if (empDashboard.Count == 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(empDashboard, Convert.ToInt32(Enums.StatusCodes.status_404), Messages.notInitiatedAssessmentEmployee);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(empDashboard, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

<<<<<<< HEAD
        public List<EmpDashboard> GetEmpDashboard(DataTable empDashboardDetails)
=======
        public virtual async Task<CustomJsonData> GetYearlyDepartmentWiseManagerDashBoardAsync(int companyId, int managerId, int yearId, string departmentList)
        {
            try
            {
                SqlParameter[] sqlParameters;
                DataTable managerDashBoard;


                sqlParameters = new SqlParameter[]
                {
                         new SqlParameter("@managerId", managerId),
                         new SqlParameter("@companyId", companyId),
                         new SqlParameter("@yearId", yearId),
                         new SqlParameter("@departmentlist", departmentList),
                };
                managerDashBoard = GetDataTableADO(companyId, Messages.getYearlyDepartmentWiseManagerDashboard, sqlParameters);

                int unApprovedSparkCount = await Task.FromResult(_connectionContext.TriggerContext.EmployeeSparkRepository.GetUnApprovedSparkCount(new EmployeeSparkModel { EmpId = Convert.ToInt32(_iClaims["EmpId"].Value) }));

                List<ManagerDashBoardModel> managerDashboard = GetDepartmentWiseManagerDashBoard(managerDashBoard, unApprovedSparkCount);
                if (managerDashboard.Count == 0)
                {
                    return await Task.FromResult(JsonSettings.UserCustomDataWithStatusMessage(managerDashboard, Convert.ToInt32(Enums.StatusCodes.status_404), ""));
                }
                else
                {
                    return await Task.FromResult(JsonSettings.UserCustomDataWithStatusMessage(managerDashboard, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return await Task.FromResult(JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError));
            }
        }

        private List<EmpDashboard> GetEmpDashboard(DataTable empDashboardDetails)
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        {
            List<EmpDashboard> empDashboard = new List<EmpDashboard>();
            try
            {
                DataTable uniqueEmp = empDashboardDetails.DefaultView.ToTable(true, "empid", "empName", "CurrAvgScore", "CurrAvgScoreRank", "lastAssessedDate", "lastscore", "lastrank", "lastscoreremarks", "lastGeneralScoreRank", "lastManagerAction", "lastScoreSummary", "lyrAvgScore", "lyrAvgScoreRank", "noofcount", "lyrNoOfCount").DefaultView.ToTable();
                uniqueEmp.DefaultView.RowFilter = "CurrAvgScoreRank <> '' OR lastrank <> '' OR lyrAvgScoreRank <> '' OR noofcount > 0 OR empid > 0";
                DataTable dashBoard = uniqueEmp.DefaultView.ToTable();

                if (empDashboardDetails.Rows.Count > 0)
                {
                    empDashboard = (from DataRow dr in dashBoard.Rows
                                    select new EmpDashboard()
                                    {
                                        empId = Convert.ToInt32(dr["empid"]),
                                        empName = Convert.ToString(dr["empName"]),
                                        currentYrAvgScore = Convert.ToInt32(dr["CurrAvgScore"]),
                                        currentYrAvgScoreRank = Convert.ToString(dr["CurrAvgScoreRank"]),
                                        lastAssessedDate = Convert.ToString(dr["lastAssessedDate"]),
                                        lastScore = Convert.ToInt32(dr["lastscore"]),
                                        lastScoreRank = Convert.ToString(dr["lastrank"]),
                                        lastGeneralScoreRank = Convert.ToString(dr["lastGeneralScoreRank"]),
                                        lastScoreRemarks = Convert.ToString(dr["lastscoreremarks"]),
                                        lastManagerAction = Convert.ToString(dr["lastManagerAction"]),
                                        lastScoreSummary = Convert.ToString(dr["lastScoreSummary"]),
                                        lyrAvgScore = Convert.ToInt32(dr["lyrAvgScore"]),
                                        lyrAvgScoreRank = Convert.ToString(dr["lyrAvgScoreRank"]),
                                        noOfRatings = Convert.ToInt32(dr["noofcount"]),
                                        lyrNoOfRatings = Convert.ToInt32(dr["lyrNoOfCount"]),
                                        graphCategories = GetGraphCategories(empDashboardDetails, Convert.ToInt32(dr["empid"])),
                                        remarks = GetRemarks(empDashboardDetails, Convert.ToInt32(dr["empid"]))
                                    }).ToList();

                }
                return empDashboard;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                throw;
            }
        }

        public DataTable GetDataTableADO(int companyId, string storedProcedureName, SqlParameter[] sqlParameters)
        {
            string connString = string.Format(Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.TenantConnectionString.ToString()], _iClaims["Key"].Value);
            if (_iClaims["RoleId"].Value == Enums.DimensionElements.TriggerAdmin.GetHashCode().ToString())
            {
                string tenantName = _catalogDbContext.CompanyDbConfig.Select<string>(new DTO.CompanyDbConfig { CompanyId = companyId });
                connString = ConnectionExtention.GetConnectionString(tenantName);
            }
            return ExecuteDataTable(connString, storedProcedureName, sqlParameters, null);
        }

        private DataTable ExecuteDataTable(string connectionString, string storedProcedureName, SqlParameter[] sqlParameters, object[] parameterValues)
        {
            var dataTable = new DataTable();

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                using (var cmd = BeginStoredProcedure(sqlConnection, storedProcedureName, sqlParameters, parameterValues))
                {
                    using (var sqlDataAdapter = new SqlDataAdapter(cmd))
                    {
                        sqlDataAdapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        public List<ManagerDashBoardModel> GetDepartmentWiseManagerDashBoard(DataTable dtbManagerDashboard, int unApprovedSparkCount)
        {
            try
            {
                DataTable mainData = dtbManagerDashboard.DefaultView.ToTable(true, "CntDirectEmps", "DirectRptAvgScore", "DirectRptAvgScoreRank", "CntOrgEmps", "OrgRptAvgScore", "OrgRptAvgScoreRank").DefaultView.ToTable();
                mainData.DefaultView.RowFilter = "CntOrgEmps <> 0";

                if (mainData.DefaultView.ToTable().Rows.Count > 0)
                {
                    return (from DataRow mr in mainData.DefaultView.ToTable().Rows
                            select new ManagerDashBoardModel()
                            {
                                directRptCnt = Convert.ToInt32(mr["CntDirectEmps"]),
                                directRptAvgScore = Convert.ToInt32(mr["DirectRptAvgScore"]),
                                directRptAvgScoreRank = Convert.ToString(mr["DirectRptAvgScoreRank"]),
                                orgRptCnt = Convert.ToInt32(mr["CntOrgEmps"]),
                                orgRptAvgScore = Convert.ToInt32(mr["OrgRptAvgScore"]),
                                orgRptAvgScoreRank = Convert.ToString(mr["OrgRptAvgScoreRank"]),
                                lstGraphDirectRptPct = GetGraphDirectRptPcts(dtbManagerDashboard),
                                lstGraphDirectRptRank = GetGraphDirectRptRanks(dtbManagerDashboard),
                                lstGraphOrgRptPct = GetGraphOrgRptPcts(dtbManagerDashboard),
                                lstGraphOrgRptRank = GetGraphOrgRptRanks(dtbManagerDashboard),
                                lstGraphTodayDirectRpt = GetGraphTodayDirectRpts(dtbManagerDashboard),
                                lstGraphTodayOrgRpt = GetGraphTodayOrgRpts(dtbManagerDashboard),
                                UnApprovedSparkCount = unApprovedSparkCount
                            }).ToList();
                }
                return new List<ManagerDashBoardModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                throw;
            }
        }

        private List<GraphCategories> GetGraphCategories(DataTable empDashboardDetails, int empId)
        {
            return (from DataRow cr in empDashboardDetails.Select("empid <> 0")
                    select new GraphCategories()
                    {
                        lstMonthly = GetMonthlyGraph(empDashboardDetails, empId),
                        lstWeekly = GetWeeklyGraph(empDashboardDetails, empId),
                        lstYearly = GetYearlyGraph(empDashboardDetails, empId)
                    }).ToList();
        }

        private List<Monthly> GetMonthlyGraph(DataTable empDashboardDetails, int empId)
        {
            return (from DataRow mr in empDashboardDetails.DefaultView.ToTable(true, "monyrid", "monyr", "MonthAvgScore", "MonthAvgScoreRank").DefaultView.ToTable().Select("MonthAvgScoreRank <> ''", "monyrid")
                    orderby Convert.ToString("monyr")
                    select new Monthly()
                    {
                        empid = empId,
                        monthNo = Convert.ToString(mr["monyr"]),
                        monthScore = Convert.ToInt32(mr["MonthAvgScore"]),
                        monthScoreRank = Convert.ToString(mr["MonthAvgScoreRank"])
                    }).ToList();
        }

        private List<Weekly> GetWeeklyGraph(DataTable empDashboardDetails, int empId)
        {
            return (from DataRow wr in empDashboardDetails.DefaultView.ToTable(true, "year", "weekno", "wkno", "WeekAvgScore", "WeekAvgScoreRank").DefaultView.ToTable().Select("WeekAvgScoreRank <> ''", "year,wkno")
                    select new Weekly()
                    {
                        empid = empId,
                        weekNo = Convert.ToString(wr["weekno"]),
                        weekScore = Convert.ToInt32(wr["WeekAvgScore"]),
                        weekScoreRank = Convert.ToString(wr["WeekAvgScoreRank"])
                    }).ToList();
        }

        private List<Yearly> GetYearlyGraph(DataTable empDashboardDetails, int empId)
        {
            return (from DataRow yr in empDashboardDetails.DefaultView.ToTable(true, "year", "YearAvgScore", "YeatAvgScoreRank").DefaultView.ToTable().Rows
                    orderby Convert.ToString("year")
                    select new Yearly()
                    {
                        empid = empId,
                        yearNo = Convert.ToString(yr["year"]),
                        yearScore = Convert.ToInt32(yr["YearAvgScore"]),
                        yearScoreRank = Convert.ToString(yr["YeatAvgScoreRank"])
                    }).ToList();
        }

        private List<Remarks> GetRemarks(DataTable empDashboardDetails, int empId)
        {
            return (from DataRow comments in GetCommentsList(empDashboardDetails).Rows
                    orderby Convert.ToDateTime(Convert.ToString(comments["assessmentdate"])) descending
                    select new Remarks()
                    {
                        AssessmentId = Convert.ToInt32(comments["AssessmentId"]),
                        RemarkId = Convert.ToInt32(comments["RemarkId"]),
                        AssessmentById = Convert.ToInt32(comments["AssessmentById"]),
                        empid = empId,
                        category = Convert.ToString(comments["category"]),
                        remark = Convert.ToString(comments["remark"]),
                        status = Convert.ToInt32(comments["status"]),
                        assessmentDate = Convert.ToDateTime(comments["assessmentdate"]),
                        firstName = comments["firstName"].ToString(),
                        lastName = comments["lastName"].ToString(),
                        CloudFilePath = comments["CloudFilePath"].ToString(),
                        assessmentByImgPath = (Convert.ToString(comments["AssessmentByImgPath"]) == "" ? Convert.ToString(comments["AssessmentByImgPath"]) : _appSettings.StorageAccountURL + "/" + _blobContainerEmployee + "/" + Convert.ToString(comments["AssessmentByImgPath"])),
                        DocumentName = (Convert.ToString(comments["DocumentPath"]) == "" ? Convert.ToString(comments["DocumentPath"]) : _appSettings.StorageAccountURL + "/" + Messages.AssessmentDocPath + "/" + Convert.ToString(comments["DocumentPath"]))
                    }).ToList();
        }

        private DataTable GetCommentsList(DataTable empDashboardDetails)
        {
            empDashboardDetails.DefaultView.RowFilter = " (remark <> '' OR (documentPath <>'' OR cloudfilepath <>'')) and category <> ''";
            DataTable commentsList = empDashboardDetails.DefaultView.ToTable().Copy();
            commentsList.Rows.Cast<DataRow>().ToList().ForEach(x => x["assessmentdate"] = Convert.ToDateTime(x["kpiupddtstamp"]));

            empDashboardDetails.DefaultView.RowFilter = "";
            empDashboardDetails.DefaultView.RowFilter = "GeneralRemark <> '' OR GeneralDocPath <> '' OR GeneralCloudFilePath<>''";
            DataRow newRow;
            foreach (DataRow dr in empDashboardDetails.DefaultView.ToTable(true, "generalremark", "generalstatus", "assessmentdate", "firstname", "lastname", "assessmentbyimgpath", "generaldocpath","generalcloudfilepath", "assessmentid", "assessmentbyid", "generalupddtstamp").Rows)
            {
                newRow = commentsList.NewRow();
                newRow["AssessmentId"] = Convert.ToInt32(dr["AssessmentId"]);
                newRow["AssessmentById"] = Convert.ToInt32(dr["AssessmentById"]);
                newRow["RemarkId"] = 0;
                newRow["category"] = "General";
                newRow["remark"] = Convert.ToString(dr["generalremark"]);
                newRow["status"] = Convert.ToInt32(dr["generalstatus"]);
                newRow["assessmentdate"] = Convert.ToDateTime(dr["generalupddtstamp"]);
                newRow["firstName"] = Convert.ToString(dr["firstName"]);
                newRow["lastName"] = Convert.ToString(dr["lastName"]);
                newRow["AssessmentByImgPath"] = Convert.ToString(dr["AssessmentByImgPath"]);
                newRow["DocumentPath"] = dr["GeneralDocPath"];
                newRow["CloudFilePath"] = dr["GeneralCloudFilePath"];

                commentsList.Rows.Add(newRow);
            }
            commentsList.AcceptChanges();
            empDashboardDetails.DefaultView.RowFilter = "";

            return commentsList;
        }

<<<<<<< HEAD
        public async Task<CustomJsonData> GetYearlyDepartmentWiseManagerDashBoardAsync(int companyId, int managerId, int yearId, string departmentList)
        {
            try
            {
                SqlParameter[] sqlParameters;
                DataTable managerDashBoard;


                sqlParameters = new SqlParameter[]
                {
                         new SqlParameter("@managerId", managerId),
                         new SqlParameter("@companyId", companyId),
                         new SqlParameter("@yearId", yearId),
                         new SqlParameter("@departmentlist", departmentList),
                };
                managerDashBoard = GetDataTableADO(companyId, Messages.getYearlyDepartmentWiseManagerDashboard, sqlParameters);

                int unApprovedSparkCount = await Task.FromResult(_connectionContext.TriggerContext.EmployeeSparkRepository.GetUnApprovedSparkCount(new EmployeeSparkModel { EmpId = Convert.ToInt32(_iClaims["EmpId"].Value) }));

                int teamListCount = _teamDashboard.GetTeamListCountByPermission();

                List<ManagerDashBoardModel> managerDashboard = GetDepartmentWiseManagerDashBoard(managerDashBoard, unApprovedSparkCount, teamListCount);
                if (managerDashboard.Count == 0)
                {
                    return await Task.FromResult(JsonSettings.UserCustomDataWithStatusMessage(managerDashboard, Convert.ToInt32(Enums.StatusCodes.status_404), ""));
                }
                else
                {
                    return await Task.FromResult(JsonSettings.UserCustomDataWithStatusMessage(managerDashboard, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                return await Task.FromResult(JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError));
            }
        }

        public DataTable GetDataTableADO(int companyId, string storedProcedureName, SqlParameter[] sqlParameters)
        {
            string connString = string.Format(Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.TenantConnectionString.ToString()], _iClaims["Key"].Value);
            if (_iClaims["RoleId"].Value == Enums.DimensionElements.TriggerAdmin.GetHashCode().ToString())
            {
                string tenantName = _catalogDbContext.CompanyDbConfig.Select<string>(new DTO.CompanyDbConfig { CompanyId = companyId });
                connString = ConnectionExtention.GetConnectionString(tenantName);
            }
            return ExecuteDataTable(connString, storedProcedureName, sqlParameters, null);
        }

        public DataTable ExecuteDataTable(string connectionString, string storedProcedureName, SqlParameter[] sqlParameters, object[] parameterValues)
        {
            var dataTable = new DataTable();

            using (var sqlConnection = new SqlConnection(connectionString))
            {
                using (var cmd = BeginStoredProcedure(sqlConnection, storedProcedureName, sqlParameters, parameterValues))
                {
                    using (var sqlDataAdapter = new SqlDataAdapter(cmd))
                    {
                        sqlDataAdapter.Fill(dataTable);
                    }
                }
            }

            return dataTable;
        }

        public List<ManagerDashBoardModel> GetDepartmentWiseManagerDashBoard(DataTable dtbManagerDashboard,int unApprovedSparkCount, int teamListCount)
        {
            try
            {
                DataTable mainData = dtbManagerDashboard.DefaultView.ToTable(true, "CntDirectEmps", "DirectRptAvgScore", "DirectRptAvgScoreRank", "CntOrgEmps", "OrgRptAvgScore", "OrgRptAvgScoreRank").DefaultView.ToTable();
                mainData.DefaultView.RowFilter = "CntOrgEmps <> 0";

                if (mainData.DefaultView.ToTable().Rows.Count > 0)
                {
                    return (from DataRow mr in mainData.DefaultView.ToTable().Rows
                            select new ManagerDashBoardModel()
                            {
                                directRptCnt = Convert.ToInt32(mr["CntDirectEmps"]),
                                directRptAvgScore = Convert.ToInt32(mr["DirectRptAvgScore"]),
                                directRptAvgScoreRank = Convert.ToString(mr["DirectRptAvgScoreRank"]),
                                orgRptCnt = Convert.ToInt32(mr["CntOrgEmps"]),
                                orgRptAvgScore = Convert.ToInt32(mr["OrgRptAvgScore"]),
                                orgRptAvgScoreRank = Convert.ToString(mr["OrgRptAvgScoreRank"]),
                                lstGraphDirectRptPct = GetGraphDirectRptPcts(dtbManagerDashboard),
                                lstGraphDirectRptRank = GetGraphDirectRptRanks(dtbManagerDashboard),
                                lstGraphOrgRptPct = GetGraphOrgRptPcts(dtbManagerDashboard),
                                lstGraphOrgRptRank = GetGraphOrgRptRanks(dtbManagerDashboard),
                                lstGraphTodayDirectRpt = GetGraphTodayDirectRpts(dtbManagerDashboard),
                                lstGraphTodayOrgRpt = GetGraphTodayOrgRpts(dtbManagerDashboard),
                                UnApprovedSparkCount= unApprovedSparkCount,
                                TeamListCount = teamListCount
                            }).ToList();
                }
                return new List<ManagerDashBoardModel>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message.ToString());
                throw;
            }
        }

=======
>>>>>>> 2157f8a3dbc2f1e97f86e394aca030ca64e97686
        private List<GraphDirectRptPct> GetGraphDirectRptPcts(DataTable managerWiseDashboard)
        {
            return (from DataRow dpr in managerWiseDashboard.DefaultView.ToTable(true, "grphDrctRpt", "DrctScoreRank", "DrctmonYrId", "DrctmonYr", "TotDirctEmps", "DrctPct").DefaultView.ToTable().Select("DrctmonYr <> '' or grphDrctRpt <> 0 AND DrctPct <> 0", "DrctmonYrId")
                    select new GraphDirectRptPct()
                    {
                        directMonYrId = Convert.ToInt32(dpr["DrctmonYrId"]),
                        directMonYr = Convert.ToString(dpr["DrctmonYr"]),
                        directRptEmpCnt = Convert.ToInt32(dpr["grphDrctRpt"]),
                        directScoreRank = Convert.ToString(dpr["DrctScoreRank"]),
                        directRptEmpPct = Convert.ToInt32(dpr["DrctPct"])

                    }).ToList();
        }

        private List<GraphDirectRptRank> GetGraphDirectRptRanks(DataTable managerWiseDashboard)
        {
            return (from DataRow dr in managerWiseDashboard.DefaultView.ToTable(true, "DrctAvgScore", "AvgDrctRank", "DrctAvgMonYrId", "DrctAvgMonYr").DefaultView.ToTable().Select("DrctAvgMonYr <> '' or DrctAvgScore <> 0 AND AvgDrctRank <> ''", "DrctAvgMonYrId")
                    select new GraphDirectRptRank()
                    {
                        directAvgMonYrId = Convert.ToInt32(dr["DrctAvgMonYrId"]),
                        directAvgMonYr = Convert.ToString(dr["DrctAvgMonYr"]),
                        directRptAvgScore = Convert.ToInt32(dr["DrctAvgScore"]),
                        directAvgScoreRank = Convert.ToString(dr["AvgDrctRank"])
                    }).ToList();

        }

        private List<GraphOrgRptPct> GetGraphOrgRptPcts(DataTable managerWiseDashboard)
        {
            return (from DataRow opr in managerWiseDashboard.DefaultView.ToTable(true, "grphOrgRpt", "OrgRank", "OrgMonYrId", "OrgMonYr", "TotOrgEmps", "OrgPct").DefaultView.ToTable().Select("OrgMonYr <> '' OR grphOrgRpt <> 0 AND OrgPct <> 0", "OrgMonYrId")
                    select new GraphOrgRptPct()
                    {
                        orgMonYrId = Convert.ToInt32(opr["OrgMonYrId"]),
                        orgMonYr = Convert.ToString(opr["OrgMonYr"]),
                        orgRptEmpCnt = Convert.ToInt32(opr["grphOrgRpt"]),
                        orgScoreRank = Convert.ToString(opr["OrgRank"]),
                        orgRptEmpPct = Convert.ToInt32(opr["OrgPct"])

                    }).ToList();
        }

        private List<GraphOrgRptRank> GetGraphOrgRptRanks(DataTable managerWiseDashboard)
        {
            return (from DataRow or in managerWiseDashboard.DefaultView.ToTable(true, "OrgAvgScore", "AvgOrgRank", "OrgAvgMonYrId", "OrgAvgMonYr").DefaultView.ToTable().Select("OrgAvgMonYr<> '' OR OrgAvgScore <> 0 AND AvgOrgRank <> ''", "OrgAvgMonYrId")
                    select new GraphOrgRptRank()
                    {
                        orgAvgMonYrId = Convert.ToInt32(or["OrgAvgMonYrId"]),
                        orgAvgMonYr = Convert.ToString(or["OrgAvgMonYr"]),
                        orgRptAvgScore = Convert.ToInt32(or["OrgAvgScore"]),
                        orgAvgScoreRank = Convert.ToString(or["AvgOrgRank"])

                    }).ToList();
        }

        private List<GraphTodayDirectRpt> GetGraphTodayDirectRpts(DataTable managerWiseDashboard)
        {
            return (from DataRow tdr in managerWiseDashboard.DefaultView.ToTable(true, "TodayRptEmpCnt", "TodayRptEmpRank", "TodayRptEmpList").DefaultView.ToTable().Select("TodayRptEmpCnt <> 0 AND TodayRptEmpRank <> ''")
                    select new GraphTodayDirectRpt()
                    {
                        TodayDirectRptCnt = Convert.ToInt32(tdr["TodayRptEmpCnt"]),
                        TodayDirectRptRank = Convert.ToString(tdr["TodayRptEmpRank"]),
                        TodayRptEmpList = Convert.ToString(tdr["TodayRptEmpList"])

                    }).ToList();
        }

        private List<GraphTodayOrgRpt> GetGraphTodayOrgRpts(DataTable managerDashboard)
        {
            return (from DataRow tdr in managerDashboard.DefaultView.ToTable(true, "TodayOrgEmpCnt", "TodayOrgEmpRank", "TodayOrgEmpList").DefaultView.ToTable().Select("TodayOrgEmpCnt <> 0 AND TodayOrgEmpRank <> ''")
                    select new GraphTodayOrgRpt()
                    {
                        TodayOrgRptCnt = Convert.ToInt32(tdr["TodayOrgEmpCnt"]),
                        TodayOrgRptRank = Convert.ToString(tdr["TodayOrgEmpRank"]),
                        TodayOrgEmpList = Convert.ToString(tdr["TodayOrgEmpList"])
                    }).ToList();
        }

        private SqlCommand BeginStoredProcedure(SqlConnection sqlConnection, string storedProcedure, SqlParameter[] parameters, object[] parameterValues)
        {
            try
            {
                if (sqlConnection.State != ConnectionState.Open)
                {
                    sqlConnection.Open();  // connect to the database
                }

                return CreateCommand(storedProcedure, sqlConnection, CommandType.StoredProcedure, parameters, parameterValues, UpdateRowSource.None);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex.Message.ToString());
                throw;
            }
        }

        private SqlCommand CreateCommand(
            string storedProcedure,
            SqlConnection sqlConnection,
            CommandType commandType,
            SqlParameter[] sqlParameters,
            object[] parameterValues,
            UpdateRowSource updateRowSource)
        {
            var cmd = new SqlCommand(storedProcedure, sqlConnection) { CommandType = commandType, CommandTimeout = 300 };

            if (sqlParameters != null)
            {
                for (var idx = 0; idx < sqlParameters.Length; ++idx)
                {
                    cmd.Parameters.Add(sqlParameters[idx]);

                    if (parameterValues != null && parameterValues.Length > idx && parameterValues[idx] != null)
                    {
                        cmd.Parameters[sqlParameters[idx].ParameterName].Value = parameterValues[idx];
                    }
                }
            }

            cmd.UpdatedRowSource = updateRowSource;
            return cmd;
        }

        //Unused api to respond with message of app version upgrade
        public async Task<JsonData> GetDashboard()
        {
            return await Task.FromResult(JsonSettings.UserDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_426), Messages.appUpgradeMessage));
        }
    }
}
