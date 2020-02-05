using System;
using System.Collections.Generic;
using Trigger.DTO;
using Trigger.DTO.Spark;

namespace Trigger.Test.Shared.Utility
{
    public class ExcelUploadUtility
    {
        //ExcelEmployeesModel

        public static ExcelEmployeesModel GetExcelEmployeesModel()
        {
            return new ExcelEmployeesModel
            {
                empId = 1,
                companyId = "1",
                firstName = "abc",
                middleName = "mno",
                lastName = "xyz",
                suffix = "mr",
                email = "abc@yopmail.com",
                jobTitle = "dev",
                joiningDate = DateTime.Now.AddMonths(-5).ToShortDateString(),
                workCity = "valsad",
                workState = "Gujarat",
                workZipcode = "396005",
                departmentId = "1",
                department = "development",
                managerId = "2",
                managerName = "pqr",
                managerLName = "cde",
                empStatus = true,
                roleId = "2",
                dateOfBirth = DateTime.Now.AddYears(-20).ToShortDateString(),
                raceorethanicityId = "1",
                gender = "Male",
                jobCategory = "ABC",
                jobCode = "DEV05",
                jobGroup = "DEV",
                lastPromodate = DateTime.Now.AddMonths(-2).ToShortDateString(),
                currentSalary = 5000,
                lastIncDate = DateTime.Now.AddMonths(-2).ToShortDateString(),
                empLocation = "Gujrat",
                countryId = "1",
                regionId = "1",
                empImgPath = "D:\abc",
                bactive = true,
                createdBy = 1,
                createddtstamp = DateTime.Now.ToShortDateString(),
                updatedBy = 1,
                updateddtstamp = DateTime.Now.ToShortDateString(),
                employeeId = "abc_1",
                CSVManagerId = "abc_1",
                phonenumber = "+91 89451256899"
            };

        }

        public static List<ExcelEmployeesModel> GetExcelEmployeesModels()
        {
            return new List<ExcelEmployeesModel>() { GetExcelEmployeesModel() };
        }


        //CountRecordModel
        public static CountRecordModel GetCountRecordModel()
        {
            return new CountRecordModel
            {
                CompanyId = "1",
                NewlyInserted = 5,
                MismatchRecord = 5,
                LstNewExcelUpload = GetExcelEmployeesModels(),
                LstMisMatchExcelUpload = GetExcelEmployeesModels(),
                LstExistPhoneExcelUpload = GetExcelEmployeesModels()
            };
        }


        public static List<CountRecordModel> GetCountRecordModels()
        {
            return new List<CountRecordModel>() { GetCountRecordModel() };
        }


        //ExcelData
        public static ExcelData GetExcelData()
        {
            return new ExcelData
            {
                EmpId = 1,
                CompanyId = "1",
                Firstname = "abc",
                MiddleName = "mno",
                LastName = "xyz",
                Suffix = "mr",
                Email = "abc@yopmail.com",
                JobTitle = "dev",
                JoiningDate = DateTime.Now.AddMonths(-5).ToShortDateString(),
                WorkCity = "valsad",
                WorkState = "Gujarat",
                WorkZipCode = "396005",
                DepartmentId = 1,
                Department = "development",
                ManagerId = "2",
                ManagerName = "pqr",
                ManagerLName = "cde",
                EmpStatus = 1,
                RoleId = 2,
                DateOfBirth = DateTime.Now.AddYears(-20).ToShortDateString(),
                RaceOrEthanicityId = 1,
                Gender = "Male",
                JobCategoryId=1,
                JobCategory = "ABC",
                JobCodeId=1,
                JobCode = "DEV05",
                JobGroupId=1,
                JobGroup = "DEV",
                LastPromodate = DateTime.Now.AddMonths(-2).ToShortDateString(),
                CurrentSalary = 5000,
                LastIncDate = DateTime.Now.AddMonths(-2).ToShortDateString(),
                EmpLocation = "Gujrat",
                CountryId = 1,
                RegionId = 1,
                EmpImgPath = "D:\abc",
                EmployeeId = "abc_1",
                CSVManagerId = "abc_1",
                PhoneNumber = "+91 89451256899",
                MismatchRecord=5,
                NewlyInserted=5,
                Ord=1,
                Source="csv",
            };
        }

        public static List<ExcelData> GetExcelDatas()
        {
            return new List<ExcelData>() { GetExcelData() };
        }


        //AuthUserExcelModel

        public static AuthUserExcelModel GetAuthUserExcelModel()
        {
            return new AuthUserExcelModel { };
        }

        //EmployeeSparkModel

        public static List<EmployeeSparkModel> GetEmployeeSparks()
        {
            return new List<EmployeeSparkModel>(){ GetEmployeeSpark() };
        }

        public static EmployeeSparkModel GetEmployeeSpark()
        {
            return new EmployeeSparkModel()
            {
                 EmpId= 1035,
                 SparkId=3,
                 CategoryId= 1,
                 Category="Performance",
                 SparkBy= 1,
                 SparkByFirstName= "SparkTL",
                 SparkByLastName= "Patel",
                 Remarks="Third Spark of Employee",
                 ViaSms= false,
                 BActive= true,
                 CreatedBy= 1,
                 UpdatedBy= 0,
                 Result= 0,
                 DocumentContents= "abcdfgshg",
            };
        }
    }
}
