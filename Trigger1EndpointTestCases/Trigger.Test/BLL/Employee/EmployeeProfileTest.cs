using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Trigger.BLL.Employee;
using Trigger.BLL.Shared;
using Trigger.DAL;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Trigger.Utility;
using Xunit;
using SharedClaims = Trigger.DAL.Shared.Claims;

namespace Trigger.Test.BLL.Employee
{
    public class EmployeeProfileTest
    {
        private readonly SetupTest _setupTest;
        private readonly ServiceProvider _serviceProvider;
        private readonly Mock<TriggerContext> _triggerContext;
        private readonly Mock<ILogger<EmployeeProfile>> _logger;
        private readonly Mock<IConnectionContext> _iConnectionContext;
        private readonly Mock<TriggerCatalogContext> _triggerCatalogContext;
        private readonly Mock<EmployeeCommon> _employeeCommon;
        private readonly IOptions<AppSettings> _appSettings;
        private readonly Mock<SharedClaims> _iClaims;
        private EmployeeProfile _employeeProfile;

        public EmployeeProfileTest()
        {
            _logger = new Mock<ILogger<EmployeeProfile>>();
            _setupTest = new SetupTest();
            _serviceProvider = _setupTest.Setup();
            _iConnectionContext = new Mock<IConnectionContext>();
            _appSettings = FakeAppSettingsModel.GetAppSettingsIOption();
            _iClaims = _setupTest.SetupClaims();
            _triggerContext = new Mock<TriggerContext>(_serviceProvider);
            _iConnectionContext.SetupGet(x => x.TriggerContext).Returns(_triggerContext.Object);
            _triggerCatalogContext = new Mock<TriggerCatalogContext>(_serviceProvider);
            _employeeCommon = new Mock<EmployeeCommon>(_triggerCatalogContext.Object, _iConnectionContext.Object, null, null, _appSettings, _iClaims.Object, null);
        }

        private void Init_EmployeeProfile()
        {
            _employeeProfile = new EmployeeProfile(_employeeCommon.Object, _iConnectionContext.Object, _logger.Object, _triggerCatalogContext.Object, _appSettings);
        }

        [Theory]
        [InlineData("Pic1")]
        [InlineData(null)]
        [InlineData("")]
        public void To_Verify_GetEmployeeByIdForEditProfileAsync_WithEmpImgPath(string imgPath)
        {
            //Arrange
            EmployeeProfileModel employeeProfileModel = EmployeeUtility.GetEmployeeProfile();
            employeeProfileModel.EmpImgPath = imgPath;
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeProfileRepository.Select(It.IsAny<EmployeeProfileModel>())).Returns(employeeProfileModel);

            Init_EmployeeProfile();

            //Act
            var result = _employeeProfile.GetEmployeeByIdForEditProfileAsync(1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_GetEmployeeByIdForEditProfileAsync_WithException()
        {
            //Arrange
            Init_EmployeeProfile();

            //Act
            var result = _employeeProfile.GetEmployeeByIdForEditProfileAsync(1);

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_UpdateProfileAsync_WithEmployeeNotExist()
        {
            //Arrange
            _employeeCommon.Setup(x => x.GetExistingEmployeeDetails(1)).Returns((EmployeeModel)null);
            Init_EmployeeProfile();

            //Act
            var result = _employeeProfile.UpdateProfileAsync(1, EmployeeUtility.GetEmployeeProfile());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_UpdateProfileAsync_WithPhoneNumberExist()
        {
            //Arrange
            _employeeCommon.Setup(x => x.GetExistingEmployeeDetails(1)).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.IsPhoneNumberExistAspNetUser(It.IsAny<EmployeeModel>(), false)).Returns(true);
            _employeeCommon.Setup(x => x.GetResponseMessageForUpdate(4)).Returns(CommonUtility.GetCustomJsonData());

            Init_EmployeeProfile();

            //Act
            var result = _employeeProfile.UpdateProfileAsync(1, EmployeeUtility.GetEmployeeProfile());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(2)] //Employee is inactive
        [InlineData(4)] //Phone number not verify
        [InlineData(0)] // Employee not exist
        [InlineData(-1)] //Default
        public void To_Verify_UpdateProfileAsync_WithResponseType(int resultType)
        {
            //Arrange
            EmployeeProfileModel employeeProfileModel = EmployeeUtility.GetEmployeeProfile();
            employeeProfileModel.Result = resultType;

            _employeeCommon.Setup(x => x.GetExistingEmployeeDetails(1)).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.IsPhoneNumberExistAspNetUser(It.IsAny<EmployeeModel>(), false)).Returns(false);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeProfileRepository.Update(It.IsAny<EmployeeProfileModel>())).Returns(employeeProfileModel);

            Init_EmployeeProfile();

            //Act
            var result = _employeeProfile.UpdateProfileAsync(1, EmployeeUtility.GetEmployeeProfile());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_UpdateProfileAsync_WithUpdateSuccessfully_WithPhoneNumberChange()
        {
            //Arrange
            EmployeeModel oldEmployeeModel = EmployeeUtility.GetEmployeeModel();
            oldEmployeeModel.PhoneNumber = "+91 1234567891";

            EmployeeProfileModel employeeProfileModel = EmployeeUtility.GetEmployeeProfile();
            employeeProfileModel.Result = 1;
            employeeProfileModel.PhoneNumber = "+91 9874563219";

            _employeeCommon.Setup(x => x.GetExistingEmployeeDetails(1)).Returns(oldEmployeeModel);
            _employeeCommon.Setup(x => x.IsPhoneNumberExistAspNetUser(It.IsAny<EmployeeModel>(), false)).Returns(false);
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeProfileRepository.Update(It.IsAny<EmployeeProfileModel>())).Returns(employeeProfileModel);
            _triggerCatalogContext.Setup(x => x.AuthUserDetailsRepository.GetSubIdByEmail(It.IsAny<AuthUserDetails>())).Returns(CommonUtility.GetAuthUserDetails());
            _employeeCommon.Setup(x => x.UpdateAuthUser(It.IsAny<EmployeeModel>(), It.IsAny<string>()));
            _employeeCommon.Setup(x => x.InsertCompanyAdmin(It.IsAny<EmployeeModel>()));
            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeProfileRepository.Select(It.IsAny<EmployeeProfileModel>())).Returns(EmployeeUtility.GetEmployeeProfile());

            Init_EmployeeProfile();

            //Act
            var result = _employeeProfile.UpdateProfileAsync(1, EmployeeUtility.GetEmployeeProfile());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_UpdateProfileAsync_WithException()
        {
            //Arrange
            _employeeCommon.Setup(x => x.GetExistingEmployeeDetails(1)).Returns(EmployeeUtility.GetEmployeeModel());
            _employeeCommon.Setup(x => x.IsPhoneNumberExistAspNetUser(It.IsAny<EmployeeModel>(), false)).Returns(false);

            Init_EmployeeProfile();

            //Act
            var result = _employeeProfile.UpdateProfileAsync(1, EmployeeUtility.GetEmployeeProfile());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }


        [Theory]
        [InlineData(1)] //Active sms notification
        [InlineData(2)] //Employee is inactive
        [InlineData(3)] //Phone number not verify
        [InlineData(0)] //Employee not exist
        [InlineData(4)] //Already active sms notification 
        [InlineData(-1)] //Default 
        public void To_Verify_UpdateAllowSmsAsync_WithResultType(int resultType)
        {
            //Arrange
            EmployeeProfileModel employeeProfileModel = EmployeeUtility.GetEmployeeProfile();
            employeeProfileModel.Result = resultType;

            _iConnectionContext.Setup(x => x.TriggerContext.EmployeeProfileRepository.UpdateAllowSMS(It.IsAny<EmployeeProfileModel>())).Returns(employeeProfileModel);

            Init_EmployeeProfile();

            //Act
            var result = _employeeProfile.UpdateAllowSmsAsync(1, EmployeeUtility.GetEmployeeProfile());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_UpdateAllowSmsAsync_WithException()
        {
            //Arrange
            Init_EmployeeProfile();

            //Act
            var result = _employeeProfile.UpdateAllowSmsAsync(1, EmployeeUtility.GetEmployeeProfile());

            //Assert
            Assert.IsType<Task<CustomJsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Theory]
        [InlineData(1)] //Profile Updated successfully
        [InlineData(0)] //Employee Not exist
        [InlineData(-1)] // Default or unauthorize access
        public void To_Verify_UpdateProfilePicAsync_WithResultType(int resultType)  //UploadProfilePic method not cover due to azure file 
        {
            //Arrange 
            EmployeeProfilePicture employeeProfilePicture = EmployeeUtility.GetEmployeeProfilePicture();
            employeeProfilePicture.Result = resultType;

            _iConnectionContext.Setup(x => x.TriggerContext.EmpProfileRepository.Update(It.IsAny<EmployeeProfilePicture>())).Returns(employeeProfilePicture);

            Init_EmployeeProfile();

            //Act
            var result = _employeeProfile.UpdateProfilePicAsync(1, EmployeeUtility.GetEmployeeProfilePicture());

            //Arrange for delete file from azure
            if (resultType == 1)
            {
                var appSetting = _appSettings.Value;
                string fileName = ((EmployeeImage)result.Result.data[0]).EmpImgPath.Replace(appSetting.StorageAccountURL + Messages.slash + Messages.profilePic + Messages.slash, "");

                FileActions.DeleteFileFromBlobStorage(fileName, appSetting.StorageAccountName, appSetting.StorageAccountAccessKey, Messages.profilePic);
            }

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }

        [Fact]
        public void To_Verify_UpdateProfilePicAsync_WithException()
        {
            //Arrange
            Init_EmployeeProfile();

            //Act
            var result = _employeeProfile.UpdateProfilePicAsync(1, EmployeeUtility.GetEmployeeProfilePicture());

            //Assert
            Assert.IsType<Task<JsonData>>(result);
            Assert.NotNull(result.Result);
        }
    }
}
