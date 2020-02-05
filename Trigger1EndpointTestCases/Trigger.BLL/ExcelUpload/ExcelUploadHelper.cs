using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
using Trigger.BLL.Shared;
using Trigger.DAL.ExcelUpload;
using Trigger.DAL.Shared;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;

namespace Trigger.BLL.ExcelUpload
{
    /// <summary>
    /// class for create excel template
    /// </summary>
    public class ExcelUploadHelper
    {
        private readonly IConnectionContext _connectionContext;
        private readonly ILogger<ExcelUploadHelper> _iLogger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AppSettings _appSettings;

        /// <summary>
        /// constructor of the class
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="iLogger"></param>
        /// <param name="hostingEnvironment"></param>
        /// <param name="appSettings"></param>
        public ExcelUploadHelper(IConnectionContext connectionContext, ILogger<ExcelUploadHelper> iLogger,
            IHostingEnvironment hostingEnvironment, IOptions<AppSettings> appSettings)
        {
            _connectionContext = connectionContext;
            _iLogger = iLogger;
            _hostingEnvironment = hostingEnvironment;
            _appSettings = appSettings.Value;
        }

        /// <summary>
        /// Create excel template
        /// </summary>
        /// <param name="companyId"></param>
        public virtual Boolean CreateExcelTemplate(int companyId)
        {
            try
            {
                var file = new FileInfo(_hostingEnvironment.ContentRootPath + Messages.doubleSlash + _appSettings.CSVTemplate);
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
                    AddDataValidations(worksheet, excelMaster, GetMasterDataForExcel(companyId));
                    LockHeaders(worksheet);

                    excelMaster.Hidden = eWorkSheetHidden.VeryHidden;
                    package.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                throw;
            }
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
            excelMaster.Cells["A:A"].LoadFromCollection(masterData.Country.ConvertAll(x => x.Country.ToString()));
            SetDataValidationList(dataValidation, Messages.excelFormulaForCountry + masterData.Country.Count, Messages.promptForCountry, Messages.errorTitleForCountry, Messages.errorForCountry);

            dataValidation = worksheet.Cells["AD:AD"].DataValidation.AddListDataValidation();
            excelMaster.Cells["B:B"].LoadFromCollection(masterData.Regions.ConvertAll(x => x.Region.ToString()));
            SetDataValidationList(dataValidation, Messages.excelFormulaForRegion + masterData.Regions.Count, Messages.promptForRegion, Messages.errorTitleForRegion, Messages.errorForRegion);

            dataValidation = worksheet.Cells["N:N"].DataValidation.AddListDataValidation();
            excelMaster.Cells["C:C"].LoadFromCollection(masterData.Department.ConvertAll(x => x.department.ToString()));
            SetDataValidationList(dataValidation, Messages.excelFormulaForDepartment + masterData.Department.Count, Messages.promptForDepartment, Messages.errorTitleForDepartment, Messages.errorForDepartment);

            dataValidation = worksheet.Cells["P:P"].DataValidation.AddListDataValidation();
            excelMaster.Cells["D:D"].LoadFromCollection(masterData.RoleMaster.ConvertAll(x => x.Role.ToString()));
            SetDataValidationList(dataValidation, Messages.excelFormulaForRole + masterData.RoleMaster.Count, Messages.promptForRole, Messages.errorTitleForRole, Messages.errorForRole);

            dataValidation = worksheet.Cells["Q:Q"].DataValidation.AddListDataValidation();
            excelMaster.Cells["E:E"].LoadFromCollection(masterData.EmployeeDetails.OrderBy(x=>x.ManagerName).ToList().ConvertAll(x => x.ManagerId.ToString()));
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
        /// Get Excel headers columns
        /// </summary>
        /// <returns></returns>
        private static DataTable GetExcelHeaders()
        {
            var columnNames = new string[]{FieldName.EmployeeID,FieldName.FirstName,FieldName.MiddleName,FieldName.LastName,FieldName.Prefix,
                     FieldName.EmailAddress,FieldName.CountryCallingCode,FieldName.PhoneNumber,FieldName.EmployeePosition,string.Concat(FieldName.DateOfHire,Messages.dateHeaderFormat),
                     FieldName.City,FieldName.State,FieldName.Zip,FieldName.Department, FieldName.EmployeeStatus,FieldName.Role,FieldName.ManagersName,
                     FieldName.ExcelManagersEmployeeID,FieldName.Ethnicity,FieldName.Gender,FieldName.JobCategory,FieldName.JobCode,FieldName.JobGroup,
                     string.Concat(FieldName.DateInPosition,Messages.dateHeaderFormat),FieldName.CurrentSalary,string.Concat(FieldName.DateOfLastSalaryIncrease,Messages.dateHeaderFormat),
                     string.Concat(FieldName.DateOfBirth,Messages.dateHeaderFormat),FieldName.LocationName,FieldName.Country,FieldName.Region,FieldName.CompanyId,"" };

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
        /// Get All Country Calling code using third party API
        /// </summary>
        /// <returns> List of calling code </returns>
        private List<string> GetCoutryCallingCode()
        {
            List<string> CountryCallingCode = new List<string>();
            HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(_appSettings.CountryCallingCodeURL);
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
        /// Get Master data from database
        /// </summary>
        /// <param name="companyId">Company Id </param>
        /// <returns>list of master table</returns>
        private MasterTables GetMasterDataForExcel(int companyId)
        {
            var lstExcelMasterData = new MasterTables() { CompanyId = companyId };
            try
            {
                lstExcelMasterData = _connectionContext.TriggerContext.ExcelUploadRepository.GetMasterData(lstExcelMasterData);
            }
            catch (Exception ex)
            {
                _iLogger.LogError(ex.Message);
                throw;
            }
            return lstExcelMasterData;
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
        private static void SetDataValidationList(IExcelDataValidationList dataValidation, string formula, string prompt,
            string title, string error, bool isFormula = true)
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
    }
}
