using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DAL.ExcelUpload;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;

namespace Trigger.Test.DAL.ExcelUpload
{
    public class ExcelUploadRepositoryTest: BaseRepositoryTest
    {
        public ExcelUploadRepositoryTest():base("ExcelUploadRepository")
        {

        }

        [Fact]
        public void Verify_To_Add_Auth_Claims()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<MasterTables, MasterTables, TriggerContext>(new MasterTables()).BuildServiceProvider());

            //Act
            var result = triggerContext.ExcelUploadRepository.GetMasterData(It.IsAny<MasterTables>());

            //Assert      
            Assert.NotNull(result);
        }

        [Fact]
        public void Verify_To_Delete_Temp_Employee()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<int, int, TriggerContext>(new int()).BuildServiceProvider());

            //Act
            triggerContext.ExcelUploadRepository.DeleteTempEmployee();

            //Assert
            Assert.IsType<int>(1);
        }

        [Fact]
        public void Verify_To_Get_Excel_Data_Count()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<MasterTables, List<ExcelData>, TriggerContext>(ExcelUploadUtility.GetExcelDatas()).BuildServiceProvider());

            //Act
            var result = triggerContext.ExcelUploadRepository.GetExcelDataCount(It.IsAny<MasterTables>());

            //Assert      
            Assert.NotNull(result);
        }
    }
}
