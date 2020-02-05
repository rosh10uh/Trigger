using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.DTO;
using Trigger.Test.Shared;
using Trigger.Test.Shared.Utility;
using Xunit;

namespace Trigger.Test.DAL.Employee
{
    public class EmployeeRepositoryTest : BaseRepositoryTest
    {
        public EmployeeRepositoryTest() : base("EmployeeRepository")
        {

        }

        [Fact]
        public void To_Verify_GetAllEmployee()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetAllEmployee(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetCompanyWiseEmployeeWithPagination()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetCompanyWiseEmployeeWithPagination(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetCompanyWiseEmployeeWithoutPagination()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetCompanyWiseEmployeeWithoutPagination(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeeWithPagination()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetAllEmployeeWithPagination(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeeWithoutPagination()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetAllEmployeeWithoutPagination(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeeByManagerWithPagination()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetAllEmployeeByManagerWithPagination(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeeForTriggerAdminWithPagination()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetAllEmployeeForTriggerAdminWithPagination(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeeForTriggerAdminWithoutPagination()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetAllEmployeeForTriggerAdminWithoutPagination(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeeYearwise()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetAllEmployeeYearwise(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeeYearwiseWithoutPagination()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetAllEmployeeYearwiseWithoutPagination(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeeByManagerYearwise()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetAllEmployeeByManagerYearwise(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeeByManagerYearwiseWithoutPagination()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetAllEmployeeByManagerYearwiseWithoutPagination(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetEmployeeByGradeYearwise()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetEmployeeByGradeYearwise(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }
        
        [Fact]
        public void To_Verify_GetEmployeeByGradeYearwiseWithoutPagination()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetEmployeeByGradeYearwiseWithoutPagination(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetEmployeeByGradeByManagerYearwise()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetEmployeeByGradeByManagerYearwise(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetEmployeeByGradeByManagerYearwiseWithoutPagination()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetEmployeeByGradeByManagerYearwiseWithoutPagination(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetEmployeeByGradeAndMonthYearwise()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetEmployeeByGradeAndMonthYearwise(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetEmpByGradeAndMonthYearwiseWithoutPagination()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetEmpByGradeAndMonthYearwiseWithoutPagination(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetEmployeeByGradeAndMonthByManagerYearwise()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetEmployeeByGradeAndMonthByManagerYearwise(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetEmpByGradeAndMonthByManagerYearwiseWithoutPagination()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetEmpByGradeAndMonthByManagerYearwiseWithoutPagination(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetEmployeeById()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, EmployeeModel, TriggerContext>(EmployeeUtility.GetEmployeeModel()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetEmployeeById(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<EmployeeModel>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetEmployeeByEmpIdsForMails()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetEmployeeByEmpIdsForMails(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetDeviceInfoById()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<UserLoginModel>, TriggerContext>(CommonUtility.GetUserLoginModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetDeviceInfoById(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<UserLoginModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetNotificationById()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<NotificationModel>, TriggerContext>(CommonUtility.GetNotificationModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetNotificationById(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<NotificationModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetExcelEmployees()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetExcelEmployees(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_DeleteEmployee()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, UserDetails, TriggerContext>(EmployeeUtility.GetUserDetails()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.DeleteEmployee(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<UserDetails>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_DeleteUser()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, UserDetails, TriggerContext>(EmployeeUtility.GetUserDetails()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.DeleteUser(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<UserDetails>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_UpdateEmpForIsMailSent()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, EmployeeModel, TriggerContext>(EmployeeUtility.GetEmployeeModel()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.UpdateEmpForIsMailSent(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<EmployeeModel>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetAspNetUserCountByPhone()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, int, TriggerContext>(1).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetAspNetUserCountByPhone(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<int>(result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeeWithHierachy()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetAllEmployeeWithHierachy(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetAllEmployeesHierachyWithPagination()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetAllEmployeesHierachyWithPagination(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }

        [Fact]
        public void To_Verify_GetYearwiseEmployeeWithHierachy()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<EmployeeModel, List<EmployeeModel>, TriggerContext>(EmployeeUtility.GetEmployeeModels()).BuildServiceProvider());

            //Act
            var result = triggerContext.EmployeeRepository.GetYearwiseEmployeeWithHierachy(It.IsAny<EmployeeModel>());

            //Assert      
            Assert.IsType<List<EmployeeModel>>(result);
            Assert.NotNull(result);
        }
    }
}
