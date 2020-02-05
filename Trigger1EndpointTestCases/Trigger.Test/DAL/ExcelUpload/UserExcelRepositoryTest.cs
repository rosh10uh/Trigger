using Microsoft.Extensions.DependencyInjection;
using Moq;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Xunit;

namespace Trigger.Test.DAL.ExcelUpload
{
    public class UserExcelRepositoryTest : BaseRepositoryTest
    {

        public UserExcelRepositoryTest():base("UserExcelRepository")
        {

        }

        [Fact]
        public void Verify_To_Add_User_Login()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<UserExcelModel, UserExcelModel, TriggerContext>(new UserExcelModel()).BuildServiceProvider());

            //Act
            var result = triggerContext.UserExcelRepository.AddUserLogin(It.IsAny<UserExcelModel>());

            //Assert      
            Assert.NotNull(result);
        }
    }
}
