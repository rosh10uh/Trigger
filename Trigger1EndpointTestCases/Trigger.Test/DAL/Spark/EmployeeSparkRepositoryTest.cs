using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.DTO.Spark;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;

namespace Trigger.Test.DAL.Spark
{
    public class EmployeeSparkRepositoryTest: BaseRepositoryTest
    {
        public EmployeeSparkRepositoryTest():base("EmployeeSparkRepository")
        {

        }

        [Fact]
        public void To_Verify_Get_Employee_SparkDetails_By_SparkId()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeSparkModel, EmployeeSparkModel, TriggerContext>(ExcelUploadUtility.GetEmployeeSpark()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeSparkRepository.GetEmployeeSparkDetailsBySparkId(It.IsAny<EmployeeSparkModel>());

            //Assert      
            Assert.IsType<EmployeeSparkModel>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Get_Employee_Spark_Details()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeSparkModel, List<EmployeeSparkModel>, TriggerContext>(ExcelUploadUtility.GetEmployeeSparks()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeSparkRepository.GetEmployeeSparkDetails(It.IsAny<EmployeeSparkModel>());

            //Assert      
            Assert.IsType<List<EmployeeSparkModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Get_UnApproved_Spark_Details()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeSparkModel, List<EmployeeSparkModel>, TriggerContext>(ExcelUploadUtility.GetEmployeeSparks()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeSparkRepository.GetUnApprovedSparkDetails(It.IsAny<EmployeeSparkModel>());

            //Assert      
            Assert.IsType<List<EmployeeSparkModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Get_UnApproved_Spark_Count()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeSparkModel, int, TriggerContext>(1).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeSparkRepository.GetUnApprovedSparkCount(It.IsAny<EmployeeSparkModel>());

            //Assert      
            Assert.IsType<int>(result);
        }

        [Fact]
        public void To_Verify_Insert_Employee_Spark_Details()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeSparkModel, EmployeeSparkModel, TriggerContext>(ExcelUploadUtility.GetEmployeeSpark()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeSparkRepository.InsertEmployeeSparkDetails(It.IsAny<EmployeeSparkModel>());

            //Assert      
            Assert.IsType<EmployeeSparkModel>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Update_Spark_Approval_Status()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeSparkModel, EmployeeSparkModel, TriggerContext>(ExcelUploadUtility.GetEmployeeSpark()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeSparkRepository.UpdateSparkApprovalStatus(It.IsAny<EmployeeSparkModel>());

            //Assert      
            Assert.IsType<EmployeeSparkModel>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Update_Employee_Spark_Details()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeSparkModel, EmployeeSparkModel, TriggerContext>(ExcelUploadUtility.GetEmployeeSpark()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeSparkRepository.UpdateEmployeeSparkDetails(It.IsAny<EmployeeSparkModel>());

            //Assert      
            Assert.IsType<EmployeeSparkModel>(result);
            Assert.NotNull(result);
        }
         
        [Fact]
        public void To_Verify_Delete_Employee_Spark_Details()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeSparkModel, EmployeeSparkModel, TriggerContext>(ExcelUploadUtility.GetEmployeeSpark()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeSparkRepository.DeleteEmployeeSparkDetails(It.IsAny<EmployeeSparkModel>());

            //Assert      
            Assert.IsType<EmployeeSparkModel>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Delete_Employee_Spark_Attachment()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeSparkModel, EmployeeSparkModel, TriggerContext>(ExcelUploadUtility.GetEmployeeSpark()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeSparkRepository.DeleteEmployeeSparkAttachment(It.IsAny<EmployeeSparkModel>());

            //Assert      
            Assert.IsType<EmployeeSparkModel>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Get_Aspnet_User_By_PhoneNumber()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeSparkModel, List<AspnetUserDetails>, TriggerContext>(new List<AspnetUserDetails>()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeSparkRepository.GetAspnetUserByPhoneNumber(It.IsAny<EmployeeSparkModel>());

            //Assert      
            Assert.IsType<List<AspnetUserDetails>>(result);
            Assert.NotNull(result);
        }

        
        [Fact]
        public void To_Verify_Get_EmployeeDetails_By_EmployeeId()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeSparkModel, EmployeeModel, TriggerContext>(EmployeeUtility.GetEmployeeModel()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeSparkRepository.GetEmployeeDetailsByEmployeeId(It.IsAny<EmployeeSparkModel>());

            //Assert      
            Assert.IsType<EmployeeModel>(result);
            Assert.NotNull(result);
        }
        

        [Fact]
        public void To_Verify_Get_Trigger_AdminList()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeSparkModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeSparkRepository.GetTriggerAdminList(It.IsAny<EmployeeSparkModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }
        
        [Fact]
        public void To_Verify_Get_Company_AdminList()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeSparkModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeSparkRepository.GetCompanyAdminList(It.IsAny<EmployeeSparkModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_Get_Spark_Rejection_Details()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeSparkModel, SparkRejectionDetails, TriggerContext>(new SparkRejectionDetails()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeSparkRepository.GetSparkRejectionDetails(It.IsAny<EmployeeSparkModel>());

            //Assert      
            Assert.IsType<SparkRejectionDetails>(result);
            Assert.NotNull(result);
        }
    }
}
