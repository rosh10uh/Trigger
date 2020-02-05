using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Trigger.BLL.ExcelUpload;
using Trigger.DTO;
using Trigger.Middleware;

namespace Trigger.Controllers.V1
{
    /// <summary>
    /// API's for Excel Upload Module(Part of Employee Module)
    /// </summary>
    [ApiVersion("1.0")]
    [ApiController]
    public class ExcelUploadController : ControllerBase
    {
        private readonly ExcelUpload _excelUpload;

        /// <summary>
        /// Constructor for Excel Upload
        /// </summary>
        /// <param name="excelUpload"></param>
        public ExcelUploadController(ExcelUpload excelUpload)
        {
            _excelUpload = excelUpload;
        }

        /// <summary>
        /// Create excel template  and return excel template file path
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        [HttpGet("api/ExcelUpload/{companyId}")]
        [DynamicConnection]
        public async Task<ActionResult<CustomJsonData>> Get(int companyId)
        {
            return await _excelUpload.SelectAsync(companyId);
        }

        /// <summary>
        /// POST: api/ExcelUpload
        /// Compare excel and database data
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employees"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ExcelUpload/{companyId}")]
        [DynamicConnection]
        public async Task<ActionResult<CustomJsonData>> Post([FromRoute]string companyId, [FromBody] List<ExcelEmployeesModel> employees)
        {
            var countRecord = new CountRecordModel()
            {
                CompanyId = companyId,
                LstNewExcelUpload = employees
            };

            return await _excelUpload.InsertAsync(countRecord);
        }

        // POST: api/ExcelUpload
        /// <summary>
        /// Upload Excel for Employees
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="employees"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/excelupload/upload/{companyId}")]
        [DynamicConnection]
        public async Task<ActionResult<CustomJsonData>> PostExcel([FromRoute]string companyId, [FromBody] List<ExcelEmployeesModel> employees)
        {
            var countRecord = new CountRecordModel()
            {
                CompanyId = companyId,
                LstNewExcelUpload = employees
            };

            return await _excelUpload.UploadAsync(countRecord);
        }
    }
}
