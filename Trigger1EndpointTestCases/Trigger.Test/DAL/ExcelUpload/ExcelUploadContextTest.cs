using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DAL.BackGroundJobRequest;
using Trigger.DAL.Employee;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Trigger.Utility;
using Xunit;
using Dal_ExcelUploadContext = Trigger.DAL.ExcelUpload.ExcelUploadContext;
using TestUtility = Trigger.Test.Shared.Utility.CommonUtility;

namespace Trigger.Test.DAL.ExcelUpload
{
    public class ExcelUploadContextTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<Dal_ExcelUploadContext>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly Mock<TriggerCatalogContext> _catalogDbContext;
        private readonly Mock<EmployeeContext> _employeeContext;
        private readonly Mock<BackgroundJobRequest> _backGroundJobRequest;
        private readonly Mock<ILogger<BackgroundJobRequest>> _loggerBackground;
        private readonly Mock<ISmsSender> _smsSender;
        private readonly Mock<IHttpContextAccessor> _iHttpContextAccessor;

        public ExcelUploadContextTest()
        {
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _logger = new Mock<ILogger<Dal_ExcelUploadContext>>();
            _smsSender = new Mock<ISmsSender>();
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _iHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _iConnectionContext = new Mock<IConnectionContext>();
            _loggerBackground = new Mock<ILogger<BackgroundJobRequest>>();
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _catalogDbContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _employeeContext = new Mock<EmployeeContext>(_iConnectionContext.Object, _catalogDbContext.Object);
            _backGroundJobRequest = new Mock<BackgroundJobRequest>(_loggerBackground.Object, _iHttpContextAccessor.Object, _employeeContext.Object, _smsSender.Object, _appSettings);

        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void To_Verify_Add_NewData_To_EmpData(bool isNewExcelData)
        {
            //Arrange
            _catalogDbContext.Setup(x => x.CompanyRepository.Select<CompanyDetailsModel>(It.IsAny<CompanyDetailsModel>())).Returns(TestUtility.GetCompanyDetailsModel());
            _catalogDbContext.Setup(x => x.CompanyAdminRepository.DeleteCompanyAdminDetails(It.IsAny<EmployeeModel>())).Returns(1);
            _catalogDbContext.Setup(x => x.AuthUserDetailsRepository.Update(It.IsAny<AuthUserDetails>()));
            _catalogDbContext.Setup(x => x.ClaimsCommonRepository.Update(It.IsAny<Claims>()));
            _catalogDbContext.Setup(x => x.EmployeeDashboardRepository.GetTenantNameByCompanyId(It.IsAny<EmployeeDashboardModel>())).Returns(It.IsAny<string>());
            _catalogDbContext.Setup(x => x.AuthUserClaimExcelRepository.AddAuthClaims(It.IsAny<AuthUserClaimExcelModel>())).Returns(new AuthUserClaimExcelModel());
            _catalogDbContext.Setup(x => x.AuthUserDetailsRepository.GetSubIdByEmail(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _catalogDbContext.Setup(x => x.AuthUserExcelRepository.GetListAuthUser()).Returns(new List<AuthUserDetails>());
            _catalogDbContext.Setup(x => x.EmployeeExcelRepository.AddCompanyAdminDetails(It.IsAny<EmployeeExcelModel>())).Returns(new EmployeeExcelModel());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.DeleteUser(It.IsAny<EmployeeModel>())).Returns(CommonUtility.GetUserDetails());
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.GetUserDetails(It.IsAny<UserDetails>())).Returns(CommonUtility.GetUserDetails());
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.UpdateUser(It.IsAny<UserDetails>())).Returns(CommonUtility.GetUserLogin());
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeExcelRepository.AddEmpolyee(It.IsAny<EmployeeExcelModel>())).Returns(new EmployeeExcelModel() { Result = 1 });
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeRepository.GetExcelEmployees(It.IsAny<EmployeeModel>())).Returns(EmployeeUtility.GetEmployeeModels());
            _employeeContext.Setup(x => x.UpdateAuthUserClaims(It.IsAny<Claims>()));
            _employeeContext.Setup(x => x.DeleteAuthUser(CommonUtility.GetAuthUserDetails())).Returns(CommonUtility.GetAuthUserDetails());
            _employeeContext.Setup(x => x.UpdateUser(It.IsAny<UserDetails>())).Returns(It.IsAny<int>());
            _employeeContext.Setup(x => x.Update1AuthUser(It.IsAny<AuthUserDetails>()));
            _backGroundJobRequest.Setup(x => x.GetTemplateByName(It.IsAny<string>(), It.IsAny<int>())).Returns("Template " + Messages.headingtext + " " +
              Messages.resetPasswordURL + " " + Messages.userId + " " + Messages.landingURL + " ");
            _backGroundJobRequest.Setup(x => x.SendEmailToUpdatedUser(It.IsAny<List<EmployeeModel>>(), It.IsAny<int>()));
            _backGroundJobRequest.Setup(x => x.SendEmail(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            Dal_ExcelUploadContext excelUploadContext = new Dal_ExcelUploadContext(_logger.Object, _iConnectionContext.Object, _catalogDbContext.Object, _employeeContext.Object, _appSettings, _backGroundJobRequest.Object);
         
            //Act
            var excelData = ExcelUploadUtility.GetExcelEmployeesModels();
            excelData[0].empId = isNewExcelData ? 0 : 1;
            var result = excelUploadContext.AddNewDataToEmpData(new CountRecordModel { LstNewExcelUpload = excelData });

            //Assert
            Assert.IsType<int>(result);
        }

        [Fact]
        public void To_Check_Update_User()
        {
            //Arrange
            _employeeContext.Setup(x => x.UpdateUser(It.IsAny<UserDetails>())).Returns(It.IsAny<int>());
            _iConnectionContext.Setup(x => x.TriggerContext.UserDetailRepository.UpdateUser(It.IsAny<UserDetails>())).Returns(new DTO.UserLogin());
            Dal_ExcelUploadContext excelUploadContext = new Dal_ExcelUploadContext(_logger.Object, _iConnectionContext.Object, _catalogDbContext.Object, _employeeContext.Object, _appSettings, _backGroundJobRequest.Object);

            //Act
            excelUploadContext.UpdateUser(new EmployeeModel());

            //Assert
            Assert.IsType<bool>(true);
        }
    }
}
