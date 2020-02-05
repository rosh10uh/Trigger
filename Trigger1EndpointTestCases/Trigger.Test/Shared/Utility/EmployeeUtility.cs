using System;
using System.Collections.Generic;
using Trigger.DTO;
using Trigger.DTO.Organization;

namespace Trigger.Test.Shared.Utility
{
    public static class EmployeeUtility
    {
        //EmployeeModel
        public static EmployeeModel GetEmployeeModel()
        {
            return new EmployeeModel
            {
                RoleId = 2,
                EmpId = 1,
                EmployeeId = "CSS_1",
                CompanyId = 1,
                CompanyName = "CSS",
                FirstName = "Herry",
                MiddleName = "Mack",
                LastName = "Patel",
                Suffix = "Dr",
                Email = "herry@abc.com",
                JobTitle = "Devloper",
                JoiningDate = DateTime.Today,
                WorkCity = "Valsad",
                WorkState = "Gujarat",
                WorkZipCode = "390001",
                DepartmentId = 1,
                Department = "HR",
                ManagerId = 1,
                ManagerFName = "Herry",
                ManagerLName = "Patel",
                EmpStatus = true,
                Role = "Admin",
                DateOfBirth = DateTime.Now.AddYears(-25),
                RaceOrEthaniCityId = 1,
                RaceOrEthaniCity = "Valsad",
                Gender = "male",
                JobCategoryId = 1,
                JobCategory = "TL",
                EmpLocation = "Valsad",
                JobCodeId = 1,
                JobCode = "DEV",
                LastPromoDate = DateTime.Now.AddMonths(-3),
                CurrentSalary = 58954,
                LastIncDate = DateTime.Now.AddMonths(-3),
                Country = "India",
                CountryId = 1,
                RegionId = 1,
                Region = "Gujrat",
                EmpImgPath = "Image",
                EmpFolderPath = "Image",
                BActive = true,
                CreatedBy = 1,
                UpdatedBy = 1,
                EmpImage = "HerryProfile",
                LastAssessedDate = "03-03-2019",
                LastAssessmentDate = "03-03-2019",
                AvgTriggerScore = 50,
                AvgScoreRank = "A+",
                LastScoreRank = "A+",
                RatingCompleted = "Done",
                CompanyLogoPath = "D:\\Image",
                IsMailSent = true,
                PhoneNumber = "+91 8956231245",
                EmpIdList = "123",
                CompanyContractStartDate = DateTime.Now.AddMonths(-10),
                CompanyContractEndDate = DateTime.Now.AddMonths(10),
                ResultId = 1,
                Result = 1,
                Password = "Admin@1234",
                TotalRowCount = 5,
                YearId = 1,
                PageNumber = 10,
                PageSize = 10,
                SearchString = "Herry",
                DepartmentList = "HR,QA",
                Grade = "A+",
                Month = "Jun",
                UserId = 1
            };
        }

        public static List<EmployeeModel> GetEmployeeModels()
        {
            return new List<EmployeeModel> { GetEmployeeModel() };
        }

        //Employee Profile
        public static EmployeeProfileModel GetEmployeeProfile()
        {
            return new EmployeeProfileModel
            {
                EmpId = 1,
                EmployeeId = "MT_1",
                FirstName = "ABC",
                LastName = "Xyz",
                WorkCity = "Mno",
                WorkState = "DEF",
                WorkZipcode = "78451",
                EmpImgPath = "D:/abc",
                PhoneNumber = "+1 1247814578",
                PhoneConfirmed = true,
                OptForSms = true,
                UpdatedBy = 1,
                Result = 1
            };
        }

        public static List<EmployeeProfileModel> GetEmployeeProfiles()
        {
            List<EmployeeProfileModel> employeeProfiles = new List<EmployeeProfileModel> {
                    GetEmployeeProfile()
                    };
            return employeeProfiles;
        }

        public static EmployeeSmsNotification GetEmployeeSmsNotification()
        {
            return new EmployeeSmsNotification
            {
                EmpId = 1,
                OptForSms = true,
                UpdatedBy = 1,
                Result = 1
            };
        }

        //UserDetails

        public static UserDetails GetUserDetails()
        {

            return new UserDetails
            {
                EmpId = 1,
                UserName = "abc@yopmail.com",
                Password = "1234",
                CreatedBy = 1,
                UpdatedBy = 1,
                BActive = true,
                Result = 1,
                CompId = 1,
                ExistingEmpId = 2

            };

        }


        //EmployeeSalary
        public static EmployeeSalary GetEmployeeSalary()
        {
            return new EmployeeSalary
            {
                EmpId = 1,
                CompanyId = 1,
                CurrentSalary = 5000,
                Result = 1,
                UpdatedBy = 1
            };

        }

        //EmployeeListModel
        public static EmployeeListModel GetEmployeeListModel()
        {
            return new EmployeeListModel
            {
                EmpId = 1,
                EmployeeId = "CSS_1",
                CompanyId = 1,
                CompanyName = "1Rivet",
                FirstName = "Abc",
                MiddleName = "Mno",
                LastName = "xyz",
                Email = "abc@yopmail.com",
                DepartmentId = 1,
                department = "HR",
                ManagerId = 1,
                EmpStatus = true,
                RoleId = 2,
                LastAssessedDate = DateTime.Now.AddDays(-5).ToShortDateString(),
                RatingCompleted = "A",
                EmpRelation = 1
            };
        }

        public static List<EmployeeListModel> GetEmployeeListModels()
        {
            return new List<EmployeeListModel> { GetEmployeeListModel() };
        }

        //EmployeeProfilePicture
        public static EmployeeProfilePicture GetEmployeeProfilePicture()
        {
            return new EmployeeProfilePicture {
                EmpFolderPath=@"D:\mypic",
                EmpImage= "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",
                EmpId=1,
                CompanyId=1,
                EmpImgPath= "FileForTestCase.png",
                Result=1,
                UpdatedBy=1
            };
        }

        //OrganizationType
        public static OrganizationTypeModel GetOrganizationType()
        {
            return new OrganizationTypeModel
            {
                OrganizationTypeId = 1,
                OrganizationType = "Organization"
            };
        }
    }
}
