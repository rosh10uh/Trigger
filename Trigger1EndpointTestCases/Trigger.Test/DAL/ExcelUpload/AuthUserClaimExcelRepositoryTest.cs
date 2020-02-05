using Moq;
using System;
using System.Data;
using Trigger.DAL;
using Trigger.DAL.Shared.Sql;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;

namespace Trigger.Test.DAL.ExcelUpload
{
    public class AuthUserClaimExcelRepositoryTest : BaseRepositoryTest
    {
        public AuthUserClaimExcelRepositoryTest(): base("AuthUserClaimExcelRepository")
        {

        }

        [Fact]
        public void Verify_To_Add_Auth_Claims()
        {
            // Arrange
            var sqlHelper = new Mock<SqlHelper>();
            var serviceProvider = new Mock<IServiceProvider>();
            var triggerCatalogContext = new Mock<TriggerCatalogContext>(serviceProvider);
            triggerCatalogContext.Setup(x => x.CompanyDbConfig.Select<string>(It.IsAny<CompanyDbConfig>())).Returns("devtrigger");
            sqlHelper.Setup(x => x.SqlBulkInsert(It.IsAny<string>(), It.IsAny<DataTable>(), It.IsAny<string>())).Returns(true);

            //Act
            var result = triggerCatalogContext.Setup(x=>x.AuthUserClaimExcelRepository.AddAuthClaims(It.IsAny<AuthUserClaimExcelModel>()));

            //Assert      
            Assert.NotNull(result);
        }
    }
}
