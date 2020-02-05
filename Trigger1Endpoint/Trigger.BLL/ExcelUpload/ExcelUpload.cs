using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OfficeOpenXml;
using OfficeOpenXml.DataValidation.Contracts;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DAL.ExcelUpload;
using Trigger.DAL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.BLL.ExcelUpload
{
    /// <summary>
    /// Contains method to manage employee data operations using excel.
    /// </summary>
    public class ExcelUpload
    {
        /// <summary>
        /// Use to get service of IConnectionContext
        /// </summary>
        private readonly IConnectionContext _connectionContext;

        /// <summary>
        /// Use to get service of IConnectionContext
        /// </summary>
        private readonly ExcelUploadContext _excelUploadContext;

        /// <summary>
        /// Use to get service of CatalogDbContext
        /// </summary>
        private readonly TriggerCatalogContext _catalogDbContext;

        /// <summary>
        /// Use to get service of ILogger
        /// </summary>
        private readonly ILogger<ExcelUpload> _iLogger;

        private readonly string _csvFolderName;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _countryCallingCodeURL;

        public ExcelUpload(IConnectionContext connectionContext, ExcelUploadContext excelUploadContext, TriggerCatalogContext catalogDbContext, ILogger<ExcelUpload> iLogger, IHttpContextAccessor contextAccessor, IHostingEnvironment hostingEnvironment)
        {
            _connectionContext = connectionContext;
            _excelUploadContext = excelUploadContext;
            _catalogDbContext = catalogDbContext;
            _iLogger = iLogger;
            _csvFolderName = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.CSVTemplate.ToString()];
            _countryCallingCodeURL = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.CountryCallingCodeURL.ToString()];
            _contextAccessor = contextAccessor;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        ///  This async method is responsible to get list of all employee master data
        /// </summary>
        /// <returns></returns>
        public virtual async Task<CustomJsonData> SelectAsync(int companyId)
        {
            try
            {
                var hostName = _contextAccessor.HttpContext.Request.Scheme + Messages.doubleCollen + _contextAccessor.HttpContext.Request.Host.Value + Messages.slash;
                var file = new FileInfo(_hostingEnvironment.ContentRootPath + Messages.doubleSlash + _csvFolderName);

                if (file.Exists)
                {
                    file.Delete();
                }

                using (var package = new ExcelPackage(file))
                {
                    package.Workbook.Worksheets.Add("Employee");
                    package.Workbook.Worksheets.Add("MasterData");
                    var worksheet = package.Workbook.Worksheets["Employee"];
                    var excelMaster = package.Workbook.Worksheets["MasterData"];

                    SetHeaderColumns(worksheet, GetExcelHeaders(), companyId);
                    SetExcelSheetStyle(worksheet);
                    AddDataValidations(worksheet, excelMaster, await GetMasterDataForCsv(companyId));
                    LockHeaders(worksheet);

                    excelMaster.Hidden = eWorkSheetHidden.VeryHidden;
                    package.Save();
                }

                return JsonSettings.UserCustomDataWithStatusMessage(hostName + Messages.slash + _csvFolderName, Convert.ToInt32(Enums.StatusCodes.status_200), Messages.ok);
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
        /// <returns></returns>
        public virtual async Task<CustomJsonData> InsertAsync(CountRecordModel countRecord)
        {
            try
            {
                var lstCountRcd = await Task.FromResult(CompareData(countRecord));
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
        /// Compare uploaded data to employee table
        /// </summary>
        /// <param name="countRecord">list of employee</param>
        /// <returns> list of New & missed matched counts model </returns>
        public List<CountRecordModel> CompareData(CountRecordModel countRecord)
        {
            var lstCountRcd = new List<CountRecordModel>();
            try
            {
                DataTable dtbEmployee = ConvertToDataTable.ToDataTable(countRecord.lstNewCsvUpload);
                dtbEmployee.TableName = "tmpEmployeedetails";
                lstCountRcd = GetNewEmployeeData(dtbEmployee, countRecord.companyId);

                return lstCountRcd;
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                return lstCountRcd;
            }
        }

        /// <summary>
        /// Compare & get New employee count 
        /// </summary>
        /// <param name="dtbSource">DataTable employee</param>
        /// <param name="companyId">Company Id</param>
        /// <returns> list of New & missed matched counts model </returns>
        public List<CountRecordModel> GetNewEmployeeData(DataTable dtbSource, string companyId)
        {
            var lstCountRecords = new List<CountRecordModel>();

            string tenantName = _catalogDbContext.CompanyDbConfig.Select<string>(new DTO.CompanyDbConfig { CompanyId = int.Parse(companyId) });
            string connString = ConnectionExtention.GetConnectionString(tenantName);
            try
            {
                List<CsvEmployeesModel> lstExistPhoneNumber = new List<CsvEmployeesModel>();
                List<string> lstDuplicatePhone;

                if (ConvertToDataTable.SqlBulkInsert(connString, dtbSource, "tmpEmployeedetails"))
                {
                    var masterTable = new MasterTables { companyId = Convert.ToInt32(companyId) };
                    var csvData = _connectionContext.TriggerContext.ExcelUploadRepository.GetCsvDataCount(masterTable);
                    if (dtbSource.Select("phonenumber <> ''").Any())
                    {
                        DataTable dtEmployee = getEmpForCheckDuplicatePhoneNumber(csvData);
                        lstDuplicatePhone = GetSCVPhoneExistsEmployee(dtEmployee);

                        if (lstDuplicatePhone.Count > 0)
                        {
                            lstExistPhoneNumber = GetCSVEmployeesPhoneExist(dtbSource, lstDuplicatePhone);
                            csvData = removeDuplicatePhonenumberEmployee(csvData, lstDuplicatePhone);
                        }
                    }

                    var dataWithCount = ToDataTable(csvData);
                    if (csvData.Count > 0)
                    {
                        List<CsvEmployeesModel> lstNewEmp = GetCSVEmployees(dataWithCount);
                        List<CsvEmployeesModel> lstMismatchEmp = GetCSVMismatchEmployees(dataWithCount);

                        int newEmpCount = lstNewEmp.Count;
                        int mismatchCount = lstMismatchEmp.Count > 0 ? csvData.Count(x => x.Source == "CSV") : 0;

                        lstCountRecords.Add(new CountRecordModel()
                        {
                            newlyInserted = newEmpCount,
                            mismatchRecord = mismatchCount,
                            lstNewCsvUpload = GetCSVEmployees(dataWithCount),
                            lstMisMatchCsvUpload = GetCSVMismatchEmployees(dataWithCount),
                            lstExistPhoneCsvUpload = lstExistPhoneNumber
                        });
                    }
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
        /// Remove employee records if there phone number already exist
        /// </summary>
        /// <param name="csvData">list of csv Employee</param>
        /// <param name="lstDuplicatePhone">list of duplicate phone numbers</param>
        /// <returns> list of csv employee which have no duplicate phone number </returns>
        public List<CsvData> removeDuplicatePhonenumberEmployee(List<CsvData> csvData, List<string> lstDuplicatePhone)
        {
            var lstMismatchEmp = csvData.Where(x => lstDuplicatePhone.Contains(x.phonenumber) && x.Source == "CSV" && x.MismatchRecord == 0 && x.NewlyInserted == 0).Select(x => x.empid);
            if (lstMismatchEmp.Any())
            {
                var removeDuplicatePhoneRecords = csvData.Where(x => !lstMismatchEmp.ToList().Contains(x.empid));
                if (removeDuplicatePhoneRecords.Any())
                {
                    csvData = removeDuplicatePhoneRecords.ToList();
                }
            }

            var lstnewEmp = csvData.Where(x => lstDuplicatePhone.Contains(x.phonenumber) && x.Source == "" && x.MismatchRecord == 0 && x.NewlyInserted == 0).Select(x => x.phonenumber);
            if (lstnewEmp.Any())
            {
                var removeDuplicatePhoneRecords = csvData.Where(x => !lstnewEmp.ToList().Contains(x.phonenumber));
                if (removeDuplicatePhoneRecords.Any())
                {
                    csvData = removeDuplicatePhoneRecords.ToList();
                }
            }
            return csvData;
        }

        /// <summary>
        /// Prepare list for find duplicate phone number
        /// </summary>
        /// <param name="items">list of items</param>
        /// <returns> DataTable </returns>
        public DataTable getEmpForCheckDuplicatePhoneNumber(List<CsvData> lstEmployee)
        {
            List<CsvData> lstEmp = new List<CsvData>();

            var lstMisMatchEmp = (from e1 in lstEmployee
                                  join e2 in lstEmployee
                                  on e1.empid equals e2.empid
                                  where e1.Source == "DB"
                                  where e2.Source == "CSV"
                                  select new CsvData
                                  {
                                      phonenumber = e2.phonenumber,
                                      email = e1.email,
                                      employeeid = e1.employeeid
                                  });


            var lstNewEmp = (from e in lstEmployee
                             where e.Source == ""
                             where e.NewlyInserted == 0
                             where e.MismatchRecord == 0
                             select new CsvData
                             {
                                 phonenumber = e.phonenumber,
                                 email = e.email,
                                 employeeid = e.employeeid
                             });

            if (lstMisMatchEmp.Any())
            {
                lstEmp.AddRange(lstMisMatchEmp);
            }
            if (lstNewEmp.Any())
            {
                lstEmp.AddRange(lstNewEmp);
            }

            return ToDataTable(lstEmp);
        }

        /// <summary>
        /// convert list to datatable 
        /// </summary>
        /// <param name="items">list of items</param>
        /// <returns> DataTable </returns>
        public static DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                dataTable.Columns.Add(prop.Name, prop.PropertyType);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }

        /// <summary>
        /// Get mismatch employee list
        /// </summary>
        /// <param name="dataWithCount">employee details datatable</param>
        /// <returns> List of CsvEmployeesModel </returns>
        private List<CsvEmployeesModel> GetCSVMismatchEmployees(DataTable dataWithCount)
        {
            return (from DataRow mr in dataWithCount.Select("NewlyInserted = 0 AND MismatchRecord = 0 AND Source <> ''", "empid,ord")
                    select new CsvEmployeesModel()
                    {
                        empId = Convert.ToInt32(mr["empid"]),
                        companyId = Convert.ToString(mr["companyid"]),
                        employeeId = Convert.ToString(mr["employeeid"]) + '-' + Convert.ToString(mr["Source"]),
                        firstName = Convert.ToString(mr["firstname"]),
                        middleName = Convert.ToString(mr["middleName"]),
                        lastName = Convert.ToString(mr["lastName"]),
                        suffix = Convert.ToString(mr["suffix"]),
                        email = Convert.ToString(mr["email"]),
                        jobTitle = Convert.ToString(mr["jobTitle"]),
                        joiningDate = Convert.ToString(mr["joiningdate"]),
                        workCity = Convert.ToString(mr["workCity"]),
                        workState = Convert.ToString(mr["workState"]),
                        workZipcode = Convert.ToString(mr["WorkZipcode"]),
                        departmentId = Convert.ToString(mr["departmentId"]),
                        department = Convert.ToString(mr["department"]),
                        managerId = Convert.ToString(mr["managerId"]),
                        managerName = Convert.ToString(mr["managerName"]),
                        managerLName = Convert.ToString(mr["managerLName"]),
                        empStatus = Convert.ToBoolean(mr["empStatus"]),
                        roleId = Convert.ToString(mr["roleId"]),
                        dateOfBirth = Convert.ToString(mr["dateOfBirth"]),
                        raceorethanicityId = Convert.ToString(mr["raceorethanicityId"]),
                        gender = Convert.ToString(mr["gender"]),
                        jobCategory = Convert.ToString(mr["jobCategory"]),
                        jobCode = Convert.ToString(mr["jobCode"]),
                        jobGroup = Convert.ToString(mr["jobGroup"]),
                        lastPromodate = Convert.ToString(mr["lastPromodate"]),
                        currentSalary = Convert.ToInt32(mr["currentSalary"]),
                        lastIncDate = Convert.ToString(mr["lastIncDate"]),
                        empLocation = Convert.ToString(mr["empLocation"]),
                        countryId = Convert.ToString(mr["countryId"]),
                        regionId = Convert.ToString(mr["regionId"]),
                        empImgPath = Convert.ToString(mr["empImgPath"]),
                        CSVManagerId = Convert.ToString(mr["CSVManagerId"]),
                        phonenumber = Convert.ToString(mr["phonenumber"])

                    }).ToList();
        }

        private List<CsvEmployeesModel> GetCSVEmployees(DataTable dataWithCount)
        {
            return (from DataRow nr in dataWithCount.Select("NewlyInserted = 0 AND MismatchRecord = 0 AND Source = ''")
                    select new CsvEmployeesModel()
                    {
                        empId = Convert.ToInt32(nr["empid"]),
                        companyId = Convert.ToString(nr["companyid"]),
                        employeeId = Convert.ToString(nr["employeeid"]),
                        firstName = Convert.ToString(nr["firstname"]),
                        middleName = Convert.ToString(nr["middleName"]),
                        lastName = Convert.ToString(nr["lastName"]),
                        suffix = Convert.ToString(nr["suffix"]),
                        email = Convert.ToString(nr["email"]),
                        jobTitle = Convert.ToString(nr["jobTitle"]),
                        joiningDate = Convert.ToString(nr["joiningdate"]),
                        workCity = Convert.ToString(nr["workCity"]),
                        workState = Convert.ToString(nr["workState"]),
                        workZipcode = Convert.ToString(nr["WorkZipcode"]),
                        departmentId = Convert.ToString(nr["departmentId"]),
                        department = Convert.ToString(nr["department"]),
                        managerId = Convert.ToString(nr["managerId"]),
                        managerName = Convert.ToString(nr["managerName"]),
                        managerLName = Convert.ToString(nr["managerLName"]),
                        empStatus = Convert.ToBoolean(nr["empStatus"]),
                        roleId = Convert.ToString(nr["roleId"]),
                        dateOfBirth = Convert.ToString(nr["dateOfBirth"]),
                        raceorethanicityId = Convert.ToString(nr["raceorethanicityId"]),
                        gender = Convert.ToString(nr["gender"]),
                        jobCategory = Convert.ToString(nr["jobCategory"]),
                        jobCode = Convert.ToString(nr["jobCode"]),
                        jobGroup = Convert.ToString(nr["jobGroup"]),
                        lastPromodate = Convert.ToString(nr["lastPromodate"]),
                        currentSalary = Convert.ToInt32(nr["currentSalary"]),
                        lastIncDate = Convert.ToString(nr["lastIncDate"]),
                        empLocation = Convert.ToString(nr["empLocation"]),
                        countryId = Convert.ToString(nr["countryId"]),
                        regionId = Convert.ToString(nr["regionId"]),
                        empImgPath = Convert.ToString(nr["empImgPath"]),
                        CSVManagerId = Convert.ToString(nr["CSVManagerId"]),
                        phonenumber = Convert.ToString(nr["phonenumber"])

                    }).ToList();
        }

        private List<CsvEmployeesModel> GetCSVEmployeesPhoneExist(DataTable dataWithCount, List<string> duplicatePhones)
        {
            var lstEmployee = (from nr in dataWithCount.AsEnumerable()
                               where duplicatePhones.Contains(nr.Field<string>("phoneNumber"))
                               select new CsvEmployeesModel
                               {
                                   companyId = Convert.ToString(nr["companyid"]),
                                   employeeId = Convert.ToString(nr["employeeid"]),
                                   firstName = Convert.ToString(nr["firstname"]),
                                   middleName = Convert.ToString(nr["middleName"]),
                                   lastName = Convert.ToString(nr["lastName"]),
                                   suffix = Convert.ToString(nr["suffix"]),
                                   email = Convert.ToString(nr["email"]),
                                   jobTitle = Convert.ToString(nr["jobTitle"]),
                                   joiningDate = Convert.ToString(nr["joiningdate"]),
                                   workCity = Convert.ToString(nr["workCity"]),
                                   workState = Convert.ToString(nr["workState"]),
                                   workZipcode = Convert.ToString(nr["WorkZipcode"]),
                                   departmentId = Convert.ToString(nr["departmentId"]),
                                   department = Convert.ToString(nr["department"]),
                                   managerId = Convert.ToString(nr["managerId"]),
                                   managerName = Convert.ToString(nr["managerName"]),
                                   managerLName = Convert.ToString(nr["managerLName"]),
                                   empStatus = Convert.ToBoolean(nr["empStatus"]),
                                   roleId = Convert.ToString(nr["roleId"]),
                                   dateOfBirth = Convert.ToString(nr["dateOfBirth"]),
                                   raceorethanicityId = Convert.ToString(nr["raceorethanicityId"]),
                                   gender = Convert.ToString(nr["gender"]),
                                   jobCategory = Convert.ToString(nr["jobCategory"]),
                                   jobCode = Convert.ToString(nr["jobCode"]),
                                   jobGroup = Convert.ToString(nr["jobGroup"]),
                                   lastPromodate = Convert.ToString(nr["lastPromodate"]),
                                   currentSalary = Convert.ToInt32(nr["currentSalary"]),
                                   lastIncDate = Convert.ToString(nr["lastIncDate"]),
                                   empLocation = Convert.ToString(nr["empLocation"]),
                                   countryId = Convert.ToString(nr["countryId"]),
                                   regionId = Convert.ToString(nr["regionId"]),
                                   empImgPath = Convert.ToString(nr["empImgPath"]),
                                   CSVManagerId = Convert.ToString(nr["CSVManagerId"]),
                                   phonenumber = Convert.ToString(nr["phonenumber"])
                               });

            if (lstEmployee.Any())
                return lstEmployee.ToList();
            else
                return new List<CsvEmployeesModel>();
        }

        // <summary>
        ///  Find and get duplicate phone number list
        // </summary>
        /// <param name="dtEmployee"> employee datatable</param>
        /// <returns> List of duplicate phone number </returns>
        private List<string> GetSCVPhoneExistsEmployee(DataTable dtEmployee)
        {
            List<string> existPhoneList = new List<string>();
            DataTable dtEmployeeCatalog = new DataView(dtEmployee).ToTable(false, new[] { "phonenumber", "email" });
            var lstPhoneCatatalog = _catalogDbContext.AuthUserCsvRepository.GetEmployeeByPhoneNumberCatalog(new AuthUserCsvModel() { tblAspNetUsers = dtEmployeeCatalog });
            DataTable dtEmployeeTanant = new DataView(dtEmployee).ToTable(false, new[] { "phonenumber", "email", "employeeid" });
            var lstPhoneTanant = _connectionContext.TriggerContext.AuthUserCsvRepository.GetEmployeeByPhoneNumberTenant(new AuthUserCsvModel() { tblAspNetUsers = dtEmployeeTanant });
            if (lstPhoneCatatalog != null && lstPhoneCatatalog.Count > 0)
            {
                existPhoneList.AddRange(lstPhoneCatatalog.Select(x => x.phonenumber).ToList<string>());
            }
            if (lstPhoneTanant != null && lstPhoneTanant.Count > 0)
            {
                existPhoneList.AddRange(lstPhoneTanant.Select(x => x.phonenumber).ToList<string>());
            }
            return existPhoneList;
        }

        /// <summary>
        /// Set header columns of excel
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="dtColumns"></param>
        /// <param name="companyId"></param>
        private static void SetHeaderColumns(ExcelWorksheet worksheet, DataTable dtColumns, int companyId)
        {
            worksheet.Cells["A1:AD1"].LoadFromDataTable(dtColumns, false);
            worksheet.Cells["AE2:AE2"].Value = companyId;
            worksheet.Cells["AF2:AF2"].Value = 0;
            worksheet.Column(dtColumns.Columns.Count).Hidden = true;
            worksheet.Column(dtColumns.Columns.Count - 1).Hidden = true;
        }

        /// <summary>
        /// Lock header columns of excel
        /// </summary>
        /// <param name="worksheet"></param>
        private static void LockHeaders(ExcelWorksheet worksheet)
        {
            worksheet.Protection.AllowInsertRows = true;
            worksheet.Protection.AllowSort = true;
            worksheet.Protection.AllowAutoFilter = true;
            worksheet.Protection.AllowInsertRows = true;
            worksheet.Protection.AllowFormatCells = true;
            worksheet.Protection.AllowSelectLockedCells = true;
            worksheet.Protection.AllowDeleteColumns = true;
            worksheet.Protection.AllowDeleteRows = true;
            worksheet.Protection.AllowFormatColumns = true;
            worksheet.Protection.AllowFormatRows = true;
            worksheet.Protection.AllowEditObject = true;
            worksheet.Protection.AllowEditScenarios = true;
            worksheet.Protection.IsProtected = true;
            worksheet.Cells.Style.Locked = false;
            worksheet.Cells["A1:AD1"].Style.Locked = true;
        }

        /// <summary>
        /// Add data validation for excel
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="excelMaster"></param>
        /// <param name="masterData"></param>
        private void AddDataValidations(ExcelWorksheet worksheet, ExcelWorksheet excelMaster, MasterTables masterData)
        {
            var dataValidation = worksheet.Cells["AC:AC"].DataValidation.AddListDataValidation();
            excelMaster.Cells["A:A"].LoadFromCollection(masterData.Country.ConvertAll(x => x.country.ToString()));
            SetDataValidationList(dataValidation, Messages.excelFormulaForCountry + masterData.Country.Count, Messages.promptForCountry, Messages.errorTitleForCountry, Messages.errorForCountry);

            dataValidation = worksheet.Cells["AD:AD"].DataValidation.AddListDataValidation();
            excelMaster.Cells["B:B"].LoadFromCollection(masterData.Regions.ConvertAll(x => x.region.ToString()));
            SetDataValidationList(dataValidation, Messages.excelFormulaForRegion + masterData.Regions.Count, Messages.promptForRegion, Messages.errorTitleForRegion, Messages.errorForRegion);

            dataValidation = worksheet.Cells["N:N"].DataValidation.AddListDataValidation();
            excelMaster.Cells["C:C"].LoadFromCollection(masterData.Department.ConvertAll(x => x.department.ToString()));
            SetDataValidationList(dataValidation, Messages.excelFormulaForDepartment + masterData.Department.Count, Messages.promptForDepartment, Messages.errorTitleForDepartment, Messages.errorForDepartment);

            dataValidation = worksheet.Cells["P:P"].DataValidation.AddListDataValidation();
            excelMaster.Cells["D:D"].LoadFromCollection(masterData.RoleMaster.ConvertAll(x => x.role.ToString()));
            SetDataValidationList(dataValidation, Messages.excelFormulaForRole + masterData.RoleMaster.Count, Messages.promptForRole, Messages.errorTitleForRole, Messages.errorForRole);

            dataValidation = worksheet.Cells["Q:Q"].DataValidation.AddListDataValidation();
            excelMaster.Cells["E:E"].LoadFromCollection(masterData.EmployeeDetails.ConvertAll(x => x.managerId.ToString()));
            SetDataValidationList(dataValidation, Messages.excelFormulaForManager + masterData.EmployeeDetails.Count, Messages.promptForManager, Messages.errorTitleForManager, Messages.errorForManager);

            dataValidation = worksheet.Cells["S:S"].DataValidation.AddListDataValidation();
            excelMaster.Cells["F:F"].LoadFromCollection(masterData.RaceOrEthnicity.ConvertAll(x => x.raceOrEthnicity.ToString()));
            SetDataValidationList(dataValidation, Messages.excelFormulaForEthanicity + masterData.RaceOrEthnicity.Count, Messages.promptForEthanicity, Messages.errorTitleForEthanicity, Messages.errorForEthanicity);


            List<string> lstCountryCallingCode = GetCoutryCallingCode();
            dataValidation = worksheet.Cells["G:G"].DataValidation.AddListDataValidation();
            excelMaster.Cells["G:G"].LoadFromCollection(lstCountryCallingCode);
            SetDataValidationList(dataValidation, Messages.excelFormulaForCountryCode + lstCountryCallingCode.Count, Messages.promptForCountryCode, Messages.errorTitleForCountryCode, Messages.errorForCountryCode);

            dataValidation = worksheet.Cells["O:O"].DataValidation.AddListDataValidation();
            SetDataValidationList(dataValidation, Messages.status, Messages.promptForEmloyeeStatus, Messages.errorTitleForEmloyeeStatus, Messages.errorForEmloyeeStatus, false);

            dataValidation = worksheet.Cells["T:T"].DataValidation.AddListDataValidation();
            SetDataValidationList(dataValidation, Messages.gender, Messages.promptForGender, Messages.errorTitleForGender, Messages.errorForGender, false);

            dataValidation = worksheet.Cells["E:E"].DataValidation.AddListDataValidation();
            SetDataValidationList(dataValidation, Messages.prefixValues, Messages.promptForPrefix, Messages.errorTitleForPrefix, Messages.errorForPrefix, false);

        }


        /// <summary>
        /// Get All Country Calling code using third party API
        /// </summary>
        /// <returns> List of calling code </returns>
        public List<string> GetCoutryCallingCode()
        {
            List<string> CountryCallingCode = new List<string>();
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(string.Format(_countryCallingCodeURL));
            WebReq.Method = "GET";
            HttpWebResponse WebResp = (HttpWebResponse)WebReq.GetResponse();

            string jsonString;
            using (Stream stream = WebResp.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, System.Text.Encoding.UTF8);
                jsonString = reader.ReadToEnd();
            }
            var items = JsonConvert.DeserializeObject<dynamic>(jsonString);
            string countryCode;
            foreach (var item in items)
            {
                countryCode = Convert.ToString(item.callingCodes[0]).Replace(" ", "");
                if (!string.IsNullOrEmpty(countryCode) && !CountryCallingCode.Contains("+" + countryCode))
                    CountryCallingCode.Add("+" + countryCode);
            }
            CountryCallingCode.Sort();
            CountryCallingCode.Insert(0, Messages.promptForCountryCallingCode);
            return CountryCallingCode;
        }

        /// <summary>
        /// Set style for excel headers
        /// </summary>
        /// <param name="worksheet"></param>
        private static void SetExcelSheetStyle(ExcelWorksheet worksheet)
        {
            worksheet.Cells["A1:AD1"].Style.Fill.PatternType = ExcelFillStyle.Solid;
            worksheet.Cells["A1:AD1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            worksheet.Cells["A1:AD1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

            worksheet.Cells["A1:Q1"].Style.Fill.BackgroundColor.SetColor(Color.Yellow);
            worksheet.Cells["A1:A1"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
            worksheet.Cells["C1:C1"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
            worksheet.Cells["E1:E1"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
            worksheet.Cells["G1:G1"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
            worksheet.Cells["H1:H1"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);
            worksheet.Cells["R1:AD1"].Style.Fill.BackgroundColor.SetColor(Color.SkyBlue);

            worksheet.Cells["A1:AD1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            worksheet.Cells["A1:AD1"].Style.Font.Bold = true;
            worksheet.Cells["A1:AD1"].AutoFitColumns();

            //worksheet.Cells["H:H"].Style.Numberformat.Format = "@"; //phone number column
            worksheet.Cells["J:J"].Style.Numberformat.Format = Messages.dateFormat;
            worksheet.Cells["X:X"].Style.Numberformat.Format = Messages.dateFormat;
            worksheet.Cells["Z:Z"].Style.Numberformat.Format = Messages.dateFormat;
            worksheet.Cells["AA:AA"].Style.Numberformat.Format = Messages.dateFormat;
            worksheet.Cells["Y:Y"].Style.Numberformat.Format = "#";//current salary column
        }

        /// <summary>
        /// Get Excel headers columns
        /// </summary>
        /// <returns></returns>
        private static DataTable GetExcelHeaders()
        {
            var columnNames = new string[]{"EmployeeID","FirstName","MiddleName","LastName","Prefix",
                     "EmailAddress","CountryCallingCode","PhoneNumber","EmployeePosition",string.Concat("DateOfHire",Messages.dateHeaderFormat),"City","State","Zip","Department", "EmployeeStatus",
                     "Role","ManagersName","ExcelManagersEmployeeID","Ethnicity","Gender","JobCategory","JobCode","JobGroup",string.Concat("DateInPosition",Messages.dateHeaderFormat),
                     "CurrentSalary",string.Concat("DateOfLastSalaryIncrease",Messages.dateHeaderFormat),string.Concat("DateOfBirth",Messages.dateHeaderFormat),"LocationName","Country","Region","CompanyId", "" };

            var dtColumns = new DataTable();
            foreach (var colName in columnNames)
            {
                dtColumns.Columns.Add(colName, typeof(string));
            }

            var dr = dtColumns.NewRow();
            foreach (DataColumn dc in dtColumns.Columns)
            {
                dr[dc] = dc.ColumnName;
            }

            dtColumns.Rows.Add(dr);

            return dtColumns;
        }

        /// <summary>
        /// Set data validation list
        /// </summary>
        /// <param name="dataValidation"></param>
        /// <param name="formula"></param>
        /// <param name="prompt"></param>
        /// <param name="title"></param>
        /// <param name="error"></param>
        /// <param name="isFormula"></param>
        private static void SetDataValidationList(IExcelDataValidationList dataValidation, string formula, string prompt, string title, string error, bool isFormula = true)
        {
            if (isFormula)
            {
                dataValidation.Formula.ExcelFormula = formula;
            }
            else
            {
                dataValidation.Formula.Values.Add(formula);
            }

            dataValidation.ShowInputMessage = true;
            dataValidation.Prompt = prompt;
            dataValidation.ShowErrorMessage = true;
            dataValidation.ErrorTitle = title;
            dataValidation.Error = error;
        }

        /// <summary>
        /// Get Master data from database
        /// </summary>
        /// <param name="companyId">Company Id </param>
        /// <returns>list of master table</returns>
        public async Task<MasterTables> GetMasterDataForCsv(int companyId)
        {
            var lstCsvMasterData = new MasterTables() { companyId = companyId };
            try
            {
                lstCsvMasterData = await Task.FromResult(_connectionContext.TriggerContext.ExcelUploadRepository.GetMasterData(lstCsvMasterData));
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                throw;
            }
            return lstCsvMasterData;
        }
    }
}
