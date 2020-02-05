using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DAL.ExcelUpload;
using Trigger.DAL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DAL.Shared.Sql;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.ExcelUpload
{
    /// <summary>
    /// Contains method to manage employee data operations using excel.
    /// </summary>
    public class ExcelUpload
    {
        private readonly IConnectionContext _connectionContext;
        private readonly ExcelUploadContext _excelUploadContext;
        private readonly TriggerCatalogContext _catalogDbContext;
        private readonly ILogger<ExcelUpload> _iLogger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly AppSettings _appSettings;
        private readonly ExcelUploadHelper _excelUploadHelper;
        private readonly SqlHelper _sqlHelper;

        /// <summary>
        ///  constructor of the class 
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="excelUploadContext"></param>
        /// <param name="catalogDbContext"></param>
        /// <param name="iLogger"></param>
        /// <param name="contextAccessor"></param>
        /// <param name="appSettings"></param>
        /// <param name="excelUploadHelper"></param>
        /// <returns></returns>
        public ExcelUpload(IConnectionContext connectionContext, ExcelUploadContext excelUploadContext,
            TriggerCatalogContext catalogDbContext, ILogger<ExcelUpload> iLogger, IHttpContextAccessor contextAccessor,
            IOptions<AppSettings> appSettings, ExcelUploadHelper excelUploadHelper, SqlHelper sqlHelper)
        {
            _connectionContext = connectionContext;
            _excelUploadContext = excelUploadContext;
            _catalogDbContext = catalogDbContext;
            _iLogger = iLogger;
            _contextAccessor = contextAccessor;
            _appSettings = appSettings.Value;
            _excelUploadHelper = excelUploadHelper;
            _sqlHelper = sqlHelper;
        }

        /// <summary>
        ///  This async method is responsible to get list of all employee master data
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> SelectAsync(int companyId)
        {
            try
            {
                var hostName = _contextAccessor.HttpContext.Request.Scheme + Messages.doubleCollen + _contextAccessor.HttpContext.Request.Host.Value + Messages.slash;
                await Task.FromResult(_excelUploadHelper.CreateExcelTemplate(companyId));
                return JsonSettings.UserCustomDataWithStatusMessage(hostName + Messages.slash + _appSettings.CSVTemplate, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                return JsonSettings.UserCustomDataWithStatusMessage(ex.Message, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        ///  This async method is responsible to insert list of all employee master data
        /// </summary>
        /// <param name="countRecord"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> InsertAsync(CountRecordModel countRecord)
        {
            try
            {
                var lstCountRcd = await Task.FromResult(GetNewEmployeeData(countRecord));
                if (lstCountRcd.Count > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(lstCountRcd, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_404), Messages.noDataFound);
                }
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                return JsonSettings.UserCustomDataWithStatusMessage(ex.Message, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        ///  This async method is responsible to upload list of all employee master data
        /// </summary>
        /// <param name="countRecord"></param>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> UploadAsync(CountRecordModel countRecord)
        {
            try
            {
                var result = await Task.FromResult(_excelUploadContext.AddNewDataToEmpData(countRecord));
                if (result > 0)
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.importData);
                }
                else
                {
                    return JsonSettings.UserCustomDataWithStatusMessage(null, Convert.ToInt32(Enums.StatusCodes.status_404), Messages.noDataFound);
                }
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                return JsonSettings.UserCustomDataWithStatusMessage(ex.Message, Convert.ToInt32(Enums.StatusCodes.status_500), Messages.internalServerError);
            }
        }

        /// <summary>
        /// Compare & get New employee count 
        /// </summary>
        /// <param name="countRecordModel"></param>
        /// <returns> list of New & missed matched counts model </returns>
        private List<CountRecordModel> GetNewEmployeeData(CountRecordModel countRecordModel)
        {
            List<ExcelEmployeesModel> excelEmployeesModels = countRecordModel.LstNewExcelUpload;
            string companyId = countRecordModel.CompanyId;
            var lstCountRecords = new List<CountRecordModel>();
            try
            {
                if (SqlBulkInsert(companyId, excelEmployeesModels))
                {
                    List<ExcelEmployeesModel> lstExistPhoneNumber = new List<ExcelEmployeesModel>();
                    var masterTable = new MasterTables { CompanyId = Convert.ToInt32(companyId) };
                    var excelData = _connectionContext.TriggerContext.ExcelUploadRepository.GetExcelDataCount(masterTable);
                    if (excelEmployeesModels.Select(x => x.phonenumber != "").Any())
                    {
                        List<string> lstDuplicatePhone = GetExcelPhoneExistsEmployee(excelData);
                        if (lstDuplicatePhone.Count > 0)
                        {
                            lstExistPhoneNumber = GetExcelEmployeesPhoneExist(excelEmployeesModels, lstDuplicatePhone);
                            excelData = RemoveDuplicatePhonenumberEmployee(excelData, lstDuplicatePhone);
                        }
                    }

                    lstCountRecords = PrepareResponse(excelData, lstExistPhoneNumber);
                }
                _connectionContext.TriggerContext.ExcelUploadRepository.DeleteTempEmployee();
            }
            catch (Exception ex)
            {
                _connectionContext.TriggerContext.ExcelUploadRepository.DeleteTempEmployee();
                _iLogger.LogError(ex.Message);
                throw;
            }
            return lstCountRecords;
        }

        /// <summary>
        /// prepare response of excel upload
        /// </summary>
        /// <param name="excelData">list of excel Employee</param>
        /// <param name="lstExistPhoneNumber">list of exist phone numbers</param>
        /// <returns> list of records which contain new record list,mismatch employee list and phone number exist employee list </returns>
        private List<CountRecordModel> PrepareResponse(List<ExcelData> excelData, List<ExcelEmployeesModel> lstExistPhoneNumber)
        {
            var lstCountRecords = new List<CountRecordModel>();
            if (excelData.Any())
            {
                List<ExcelEmployeesModel> lstNewEmp = GetExcelEmployees(excelData);
                List<ExcelEmployeesModel> lstMismatchEmp = GetExcelMismatchEmployees(excelData);

                int newEmpCount = lstNewEmp.Count;
                int mismatchCount = lstMismatchEmp.Count > 0 ? excelData.Count(x => x.Source == "CSV") : 0;

                lstCountRecords.Add(new CountRecordModel()
                {
                    NewlyInserted = newEmpCount,
                    MismatchRecord = mismatchCount,
                    LstNewExcelUpload = lstNewEmp,
                    LstMisMatchExcelUpload = lstMismatchEmp,
                    LstExistPhoneExcelUpload = lstExistPhoneNumber
                });
            }
            return lstCountRecords;
        }

        /// <summary>
        /// bulk insert of excel uploaded data or records
        /// </summary>
        /// <param name="companyId">company id</param>
        /// <param name="excelEmployeesModels">list of excel records</param>
        /// <returns> list of Excel employee which have no duplicate phone number </returns>
        private Boolean SqlBulkInsert(string companyId, List<ExcelEmployeesModel> excelEmployeesModels)
        {
            string tenantName = _catalogDbContext.CompanyDbConfig.Select<string>(new DTO.CompanyDbConfig { CompanyId = int.Parse(companyId) });
            string connString = ConnectionExtention.GetConnectionString(tenantName);

            DataTable dtbEmployee = ConvertToDataTable.ToDataTable(excelEmployeesModels);
            dtbEmployee.TableName = FieldName.tmpEmployeedetails;

            return _sqlHelper.SqlBulkInsert(connString, dtbEmployee, FieldName.tmpEmployeedetails);
        }

        /// <summary>
        /// Remove employee records if there phone number already exist
        /// </summary>
        /// <param name="excelData">list of excel Employee</param>
        /// <param name="lstDuplicatePhone">list of duplicate phone numbers</param>
        /// <returns> list of excel employee which have no duplicate phone number </returns>
        private List<ExcelData> RemoveDuplicatePhonenumberEmployee(List<ExcelData> excelData, List<string> lstDuplicatePhone)
        {
            var lstMismatchEmp = excelData.Where(x => lstDuplicatePhone.Contains(x.PhoneNumber) && x.Source == "CSV" && x.MismatchRecord == 0 && x.NewlyInserted == 0).Select(x => x.EmpId);
            if (lstMismatchEmp.Any())
            {
                var removeDuplicatePhoneRecords = excelData.Where(x => !lstMismatchEmp.ToList().Contains(x.EmpId));
                if (removeDuplicatePhoneRecords.Any())
                {
                    excelData = removeDuplicatePhoneRecords.ToList();
                }
            }

            var lstnewEmp = excelData.Where(x => lstDuplicatePhone.Contains(x.PhoneNumber) && x.Source == "" && x.MismatchRecord == 0 && x.NewlyInserted == 0).Select(x => x.PhoneNumber);
            if (lstnewEmp.Any())
            {
                var removeDuplicatePhoneRecords = excelData.Where(x => !lstnewEmp.ToList().Contains(x.PhoneNumber));
                if (removeDuplicatePhoneRecords.Any())
                {
                    excelData = removeDuplicatePhoneRecords.ToList();
                }
            }
            return excelData;
        }

        /// <summary>
        /// Prepare list for find duplicate phone number
        /// </summary>
        /// <param name="excelEmployee">list of employee data</param>
        /// <returns> DataTable </returns>
        private DataTable GetEmpForCheckDuplicatePhoneNumber(List<ExcelData> excelEmployee)
        {
            List<ExcelData> lstEmp = new List<ExcelData>();
            var lstMisMatchEmp = from e1 in excelEmployee
                                 join e2 in excelEmployee
                                 on e1.EmpId equals e2.EmpId
                                 where e1.Source == "DB"
                                 where e2.Source == "CSV"
                                 select new ExcelData
                                 {
                                     PhoneNumber = e2.PhoneNumber,
                                     Email = e1.Email,
                                     EmployeeId = e1.EmployeeId
                                 };

            var lstNewEmp = from e in excelEmployee
                            where e.Source == ""
                            where e.NewlyInserted == 0
                            where e.MismatchRecord == 0
                            select new ExcelData
                            {
                                PhoneNumber = e.PhoneNumber,
                                Email = e.Email,
                                EmployeeId = e.EmployeeId
                            };

            if (lstMisMatchEmp.Any())
            {
                lstEmp.AddRange(lstMisMatchEmp);
            }
            if (lstNewEmp.Any())
            {
                lstEmp.AddRange(lstNewEmp);
            }

            return ConvertToDataTable.ToDataTable(lstEmp);
        }

        /// <summary>
        /// Get mismatch employee list
        /// </summary>
        /// <param name="excelDatas">employee details </param>
        /// <returns> List of excel EmployeesModel </returns>
        private List<ExcelEmployeesModel> GetExcelMismatchEmployees(List<ExcelData> excelDatas)
        {
            var employeeDatas = excelDatas.Where(x => x.NewlyInserted == 0 && x.MismatchRecord == 0 && x.Source != string.Empty)
                .OrderBy(x => x.EmpId).ThenBy(x => x.Ord);
            return (from ExcelData data in employeeDatas
                    select new ExcelEmployeesModel()
                    {
                        empId = data.EmpId,
                        companyId = data.CompanyId,
                        employeeId = data.EmployeeId + '-' + data.Source,
                        firstName = data.Firstname,
                        middleName = data.MiddleName,
                        lastName = data.LastName,
                        suffix = data.Suffix,
                        email = data.Email,
                        jobTitle = data.JobTitle,
                        joiningDate = data.JoiningDate,
                        workCity = data.WorkCity,
                        workState = data.WorkState,
                        workZipcode = data.WorkZipCode,
                        departmentId = Convert.ToString(data.DepartmentId),
                        department = data.Department,
                        managerId = data.ManagerId,
                        managerName = data.ManagerName,
                        managerLName = data.ManagerLName,
                        empStatus = Convert.ToBoolean(data.EmpStatus),
                        roleId = Convert.ToString(data.RoleId),
                        dateOfBirth = data.DateOfBirth,
                        raceorethanicityId = Convert.ToString(data.RaceOrEthanicityId),
                        gender = Convert.ToString(data.Gender),
                        jobCategory = data.JobCategory,
                        jobCode = data.JobCode,
                        jobGroup = data.JobGroup,
                        lastPromodate = data.LastPromodate,
                        currentSalary = data.CurrentSalary,
                        lastIncDate = data.LastIncDate,
                        empLocation = data.EmpLocation,
                        countryId = Convert.ToString(data.CountryId),
                        regionId = Convert.ToString(data.RegionId),
                        empImgPath = data.EmpImgPath,
                        CSVManagerId = data.CSVManagerId,
                        phonenumber = data.PhoneNumber,
                    }).ToList();
        }

        /// <summary>
        /// Get newly added employee list
        /// </summary>
        /// <param name="excelDatas">employee details</param>
        /// <returns> List of excel EmployeesModel </returns>
        private List<ExcelEmployeesModel> GetExcelEmployees(List<ExcelData> excelDatas)
        {
            return (from ExcelData data in excelDatas.Where(x => x.NewlyInserted == 0 && x.MismatchRecord == 0 && x.Source == string.Empty)
                    select new ExcelEmployeesModel()
                    {
                        empId = data.EmpId,
                        companyId = data.CompanyId,
                        employeeId = data.EmployeeId,
                        firstName = data.Firstname,
                        middleName = data.MiddleName,
                        lastName = data.LastName,
                        suffix = data.Suffix,
                        email = data.Email,
                        jobTitle = data.JobTitle,
                        joiningDate = data.JoiningDate,
                        workCity = data.WorkCity,
                        workState = data.WorkState,
                        workZipcode = data.WorkZipCode,
                        departmentId = data.DepartmentId.ToString(),
                        department = data.Department,
                        managerId = data.ManagerId,
                        managerName = data.ManagerName,
                        managerLName = data.ManagerLName,
                        empStatus = Convert.ToBoolean(data.EmpStatus),
                        roleId = Convert.ToString(data.RoleId),
                        dateOfBirth = data.DateOfBirth,
                        raceorethanicityId = Convert.ToString(data.RaceOrEthanicityId),
                        gender = Convert.ToString(data.Gender),
                        jobCategory = data.JobCategory,
                        jobCode = data.JobCode,
                        jobGroup = data.JobGroup,
                        lastPromodate = data.LastPromodate,
                        currentSalary = data.CurrentSalary,
                        lastIncDate = data.LastIncDate,
                        empLocation = data.EmpLocation,
                        countryId = Convert.ToString(data.CountryId),
                        regionId = Convert.ToString(data.RegionId),
                        empImgPath = data.EmpImgPath,
                        CSVManagerId = data.CSVManagerId,
                        phonenumber = data.PhoneNumber,
                    }).ToList();
        }

        /// <summary>
        /// Get employee list which have duplicate phone number
        /// </summary>
        /// <param name="excelEmployeesModels">employee details list</param>
        /// <param name="duplicatePhones">phone</param>
        /// <returns> List of CsvEmployeesModel </returns>
        private List<ExcelEmployeesModel> GetExcelEmployeesPhoneExist(List<ExcelEmployeesModel> excelEmployeesModels, List<string> duplicatePhones)
        {
            var lstEmployee = from employee in excelEmployeesModels
                              where duplicatePhones.Contains(employee.phonenumber)
                              select new ExcelEmployeesModel
                              {
                                  companyId = employee.companyId,
                                  employeeId = employee.employeeId,
                                  firstName = employee.firstName,
                                  middleName = employee.middleName,
                                  lastName = employee.lastName,
                                  suffix = employee.suffix,
                                  email = employee.email,
                                  jobTitle = employee.jobTitle,
                                  joiningDate = employee.joiningDate,
                                  workCity = employee.workCity,
                                  workState = employee.workState,
                                  workZipcode = employee.workZipcode,
                                  departmentId = employee.departmentId,
                                  department = employee.department,
                                  managerId = employee.managerId,
                                  managerName = employee.managerName,
                                  managerLName = employee.managerLName,
                                  empStatus = Convert.ToBoolean(employee.empStatus),
                                  roleId = employee.roleId,
                                  dateOfBirth = employee.dateOfBirth,
                                  raceorethanicityId = employee.raceorethanicityId,
                                  gender = employee.gender,
                                  jobCategory = employee.jobCategory,
                                  jobCode = employee.jobCode,
                                  jobGroup = employee.jobGroup,
                                  lastPromodate = employee.lastPromodate,
                                  currentSalary = Convert.ToInt32(employee.currentSalary),
                                  lastIncDate = employee.lastIncDate,
                                  empLocation = employee.empLocation,
                                  countryId = employee.countryId,
                                  regionId = employee.regionId,
                                  empImgPath = employee.empImgPath,
                                  CSVManagerId = employee.CSVManagerId,
                                  phonenumber = employee.phonenumber
                              };

            if (lstEmployee.Any())
                return lstEmployee.ToList();
            else
                return new List<ExcelEmployeesModel>();
        }

        /// <summary>
        ///  Find and get duplicate phone number list
        /// </summary>
        /// <param name="excelData"> employee list</param>
        /// <returns> List of duplicate phone number </returns>
        private List<string> GetExcelPhoneExistsEmployee(List<ExcelData> excelData)
        {
            List<string> existPhoneList = new List<string>();
            DataTable dtEmployee = GetEmpForCheckDuplicatePhoneNumber(excelData);

            DataTable dtEmployeeCatalog = new DataView(dtEmployee).ToTable(false, new[] { nameof(ExcelData.PhoneNumber).ToString(), nameof(ExcelData.Email).ToString() });
            var lstPhoneCatatalog = _catalogDbContext.AuthUserExcelRepository.GetEmployeeByPhoneNumberCatalog(new AuthUserExcelModel() { TblAspNetUsers = dtEmployeeCatalog });
            DataTable dtEmployeeTanant = new DataView(dtEmployee).ToTable(false, new[] { nameof(ExcelData.PhoneNumber).ToString(), nameof(ExcelData.Email).ToString(), nameof(ExcelData.EmployeeId).ToString() });
            var lstPhoneTanant = _connectionContext.TriggerContext.AuthUserExcelRepository.GetEmployeeByPhoneNumberTenant(new AuthUserExcelModel() { TblAspNetUsers = dtEmployeeTanant });

            if (lstPhoneCatatalog != null && lstPhoneCatatalog.Count > 0)
            {
                existPhoneList.AddRange(lstPhoneCatatalog.Select(x => x.PhoneNumber).ToList());
            }
            if (lstPhoneTanant != null && lstPhoneTanant.Count > 0)
            {
                existPhoneList.AddRange(lstPhoneTanant.Select(x => x.PhoneNumber).ToList());
            }
            return existPhoneList;
        }
    }
}
