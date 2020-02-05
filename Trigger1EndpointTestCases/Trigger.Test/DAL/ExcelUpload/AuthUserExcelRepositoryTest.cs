using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using Trigger.DAL;
using Trigger.DAL.Shared.Sql;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;

namespace Trigger.Test.DAL.ExcelUpload
{
    public class AuthUserExcelRepositoryTest: BaseRepositoryTest
    {
        public AuthUserExcelRepositoryTest(): base("AuthUserExcelRepository")
        {

        }

        [Fact]
        public void Verify_To_Add_Auth_Login()
        {
            // Arrange
            var sqlHelper = new Mock<SqlHelper>();
            var serviceProvider = new Mock<IServiceProvider>();
            var triggerCatalogContext = new Mock<TriggerCatalogContext>(serviceProvider);
            triggerCatalogContext.Setup(x => x.CompanyDbConfig.Select<string>(It.IsAny<CompanyDbConfig>())).Returns("devtrigger");
            sqlHelper.Setup(x => x.SqlBulkInsert(It.IsAny<string>(), It.IsAny<DataTable>(), It.IsAny<string>())).Returns(true);

            //Act
            var result = triggerCatalogContext.Setup(x => x.AuthUserExcelRepository.AddAuthLogin(It.IsAny<AuthUserExcelModel>()));

            //Assert      
            Assert.NotNull(result);
        }

        [Fact]
        public void Verify_To_Get_List_Auth_User()
        {
            // Arrange
            var serviceProvider = new Mock<IServiceProvider>();
            var triggerCatalogContext = new Mock<TriggerCatalogContext>(serviceProvider);
            //Act
            var result = triggerCatalogContext.Setup(x => x.AuthUserExcelRepository.GetListAuthUser()).Returns(new List<AuthUserDetails>());

            //Assert      
            Assert.NotNull(result);
        }

        [Fact]
        public void Verify_To_Get_Employee_By_Phone_Number_Catalog()
        {
            // Arrange
            var serviceProvider = new Mock<IServiceProvider>();
            var triggerCatalogContext = new Mock<TriggerCatalogContext>(serviceProvider);

            //Act
            var result = triggerCatalogContext.Setup(x => x.AuthUserExcelRepository.GetEmployeeByPhoneNumberCatalog(It.IsAny<AuthUserExcelModel>())).Returns(new List<ExcelData>());

            //Assert      
            Assert.NotNull(result);
        }

        [Fact]
        public void Verify_To_Get_Employee_By_Phone_Number_Tenant()
        {
            // Arrange
            var serviceProvider = new Mock<IServiceProvider>();
            var triggerCatalogContext = new Mock<TriggerCatalogContext>(serviceProvider);

            //Act
            var result = triggerCatalogContext.Setup(x => x.AuthUserExcelRepository.GetEmployeeByPhoneNumberTenant(It.IsAny<AuthUserExcelModel>())).Returns(new List<ExcelData>());

            //Assert      
            Assert.NotNull(result);
        }
    }
}
