using System;
using System.Collections.Generic;
using Trigger.DTO;
using Trigger.DTO.DimensionMatrix;
using Trigger.DTO.InActivityManager;
using Trigger.DTO.SmsService;

namespace Trigger.Test.Shared.Utility
{
    public static class CommonUtility
    {
        // BLL Comman Response
        public static CustomJsonData GetCustomJsonData<T>(List<T> lstData)
        {
            return new CustomJsonData { data = lstData, message = "Action performed successfully", status = 100 };
        }

        public static CustomJsonData GetCustomJsonData<T>(T data)
        {
            return new CustomJsonData { data = data, message = "Action performed successfully", status = 100 };
        }
        public static CustomJsonData GetCustomJsonData()
        {
            return new CustomJsonData { data = null, message = "Action performed successfully", status = 100 };
        }

        public static JsonData GetJsonData<T>(T data)
        {
            return new JsonData { status = 100, message = "Action performed successfully", data = new object[] { data } };
        }
        public static JsonData GetJsonData()
        {
            return new JsonData { status = 100, message = "Action performed successfully", data = null };
        }

        // Department
        public static DepartmentModel GetDepartmentModel()
        {
            return new DepartmentModel() { companyId = 1, department = "HR", departmentId = 1, isActive = true, result = 1 };
        }
        public static List<DepartmentModel> GetDepartmentModels()
        {
            List<DepartmentModel> list = new List<DepartmentModel>();
            list.Add(new DepartmentModel() { companyId = 1, department = "HR", departmentId = 1, isActive = true, result = 1 });
            return list;
        }
        public static List<CompanyWiseDepartmentModel> GetCompanyWiseDepartmentModels()
        {
            List<CompanyWiseDepartmentModel> list = new List<CompanyWiseDepartmentModel>();
            list.Add(new CompanyWiseDepartmentModel() { companyId = 1, department = "HR", departmentId = 1 });
            return list;
        }

        // Country
        public static List<CountryModel> GetCountryModels()
        {
            List<CountryModel> countryModels = new List<CountryModel>();
            countryModels.Add(new CountryModel { Country = "India", CountryId = 1 });
            return countryModels;
        }

        // Region
        public static List<RegionModel> GetRegionModels()
        {
            List<RegionModel> regionModels = new List<RegionModel>();
            regionModels.Add(new RegionModel { Region = "Delhi", CountryId = 1, RegionId = 1 });
            return regionModels;
        }

        // Role
        public static List<RoleModel> GetRoleModels()
        {
            List<RoleModel> roleModels = new List<RoleModel>();
            roleModels.Add(new RoleModel { Role = "Admin", RoleId = 1 });
            return roleModels;
        }

        // Ethnicity
        public static List<RaceOrEthnicityModel> GetRaceOrEthnicityModels()
        {
            List<RaceOrEthnicityModel> raceOrEthnicityModels = new List<RaceOrEthnicityModel>();
            raceOrEthnicityModels.Add(new RaceOrEthnicityModel { raceOrEthnicity = "Delhi", id = 1 });
            return raceOrEthnicityModels;
        }

        // Industry 
        public static List<IndustryModel> GetIndustryModels()
        {
            List<IndustryModel> industryModels = new List<IndustryModel>();
            industryModels.Add(new IndustryModel { industryType = "Software Development", industryTypeId = 1, createdby = "1", isActive = true });
            return industryModels;
        }

        // Widget
        public static WidgetLibrary GetWidgetLibrary()
        {
            return new WidgetLibrary
            {
                userId = 1,
                widgetId = 1,
                widgetName = "current-score",
                widgetActualName = "Current Score",
                sequenceNumber = 100,
                tileSequence = 100,
                position = 1.1M,
                isActive = true,
                isSelected = 10,
                roleId = 1,
                createdBy = 1,
                result = 1,
                widgetType = 1
            };
        }
        public static List<WidgetLibrary> GetWidgetLibraries()
        {
            List<WidgetLibrary> widgetLibraries = new List<WidgetLibrary>();
            widgetLibraries.Add(new WidgetLibrary
            {
                userId = 1,
                widgetId = 1,
                widgetName = "current-score",
                widgetActualName = "Current Score",
                sequenceNumber = 100,
                tileSequence = 100,
                position = 1.1M,
                isActive = true,
                isSelected = 10,
                roleId = 1,
                createdBy = 1,
                result = 1,
                widgetType = 1
            });
            return widgetLibraries;
        }

        //Questionnaries
        public static List<Questionnaries> GetQuestionnaries()
        {
            List<Questionnaries> questionnaries = new List<Questionnaries>();
            questionnaries.Add(new Questionnaries
            {
                id = 1,
                categoryid = 1,
                category = "Performance",
                questions = "Employee's achievement of Key Performance Indicators (KPIs) or Assigned Goals=",
                isActive = false,
                createdby = 1,
                updatedby = 1,
                answers = new List<Answers> { new Answers
                    {
                        id = 1,
                        questionId = 1,
                        answers = "Not Acceptable",
                        weightage = 1.1M,
                        isActive = true,
                        createdby = 1,
                        updatedby = 1
                    }
            }
            });
            return questionnaries;
        }
        public static List<QuestionAnswer> GetQuestionAnswers()
        {
            List<QuestionAnswer> questionAnswers = new List<QuestionAnswer>();
            questionAnswers.Add(new QuestionAnswer
            {
                categoryid = 1,
                category = "Performance",
                questions = "Employee's achievement of Key Performance Indicators (KPIs) or Assigned Goals=",
                answer = "Not Acceptable",
                answerid = 1,
                questionid = 1,
                weightage = 1.1M


            });
            return questionAnswers;
        }


        //AuthUserDetails



        public static AuthUserDetails GetAuthUserDetails()
        {
            return new AuthUserDetails
            {
                Id = "1",
                UserId = "1",
                AccessFailedCount = 5,
                ConcurrencyStamp = "0f14a4b9-70ed-4ac9-b93c-9dae3268e392",
                Email = "herry@abc.com",
                EmailConfirmed = true,
                LockoutEnabled = true,
                LockoutEnd = new DateTimeOffset(),
                NormalizedEmail = "herry@abc.com",
                NormalizedUserName = "herry@abc.com",
                PasswordHash = "AQAAAAEAACcQAAAAEHfcJtt2HmovITTQaHmXzx1Cwk25A2CAjnnn4sIjaaAccD3Wd5EYy4eUBL+Uh/WiUg==",
                PhoneNumber = "+914578561256",
                PhoneNumberConfirmed = true,
                SecurityStamp = "07d7dbef-8a2b-4f9d-96ff-cac26a42322f",
                TwoFactorEnabled = true,
                UserName = "abc",
                OldPassword = "Admin@1234",
                NewPassword = "Admin@1234",
                Token = "1254AWQEW",
                UserClient = "Herry",
                Claims = new List<Claims>(),
                Roles = new List<Roles>(),
                TokenExpiration = DateTime.Today.AddMinutes(20),
                FullName = "Herry patel",
                ExistingEmail = "herry@abc.com",
                SubId = "1"
            };
        }

        //UserDataModel
        public static UserDataModel GetUserDataModel()
        {
            return new UserDataModel
            {
                userId = 1,
                userName = "Herry",
                empId = 1,
                roleId = 1,
                companyid = 1,
                companyname = "CSS",
                role = "Admin",
                empEmailId = "herry@abc.com",
                Message = "Action performed successfully",
                dbConnection = "Server=tcp:{server Name},1433;Initial Catalog={dbName};Persist Security Info=False;User ID={username};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=0;",
                key = "Key",
                result = "1",
                Error = null,
                permission = GetActionLists(),
                employee = EmployeeUtility.GetEmployeeModel()
            };
        }

        //UserChangePassword
        public static UserChangePassword GetUserChangePassword()
        {
            return new UserChangePassword
            {
                userId = 1,
                oldPassword = "Admin@1234",
                newPassword = "Admin@1234",
                result = 1,
                updatedBy = 1,
                userName = "Herry"
            };

        }

        //UserLoginModel
        public static UserLoginModel GetUserLoginModel()
        {
            return new UserLoginModel
            {
                deviceID = "1",
                deviceType = "Android",
                password = "Admin@1234",
                userId = 1,
                username = "Herry",
                result = 1
            };
        }

        public static List<UserLoginModel> GetUserLoginModels()
        {
            return new List<UserLoginModel>() { GetUserLoginModel() };
        }

        //UserLogin
        public static UserLogin GetUserLogin()
        {
            return new UserLogin
            {
                empId = 1,
                companyid = 1,
                companyname = "CSS",
                bActive = true,
                createdBy = 1,
                empEmailId = "herry@abc.com",
                employee = EmployeeUtility.GetEmployeeModel(),
                existingEmpId = 1,
                password = "Admin@1234",
                userId = 1,
                role = "Admin",
                roleId = 1,
                result = 1,
                key = "Key",
                updatedBy = 1,
                userName = "Herry"

            };

        }

        //UserDetails
        public static UserDetails GetUserDetails()
        {
            return new UserDetails
            {
                EmpId = 1,
                UserName = "abc@yopmail.com",
                Password = "abc@1234",
                CreatedBy = 1,
                UpdatedBy = 1,
                BActive = true,
                Result = 1,
                CompId = 1,
                ExistingEmpId = 1
            };
        }

        //NotificationModel
        public static NotificationModel GetNotificationModel()
        {
            return new NotificationModel
            {
                id = 1,
                empId = 1,
                message = "New Notificaion Arrive.",
                managerId = 1,
                action = "New Employee assigned",
                isSent = true,
                markAs = true,
                createdBy = 1,
                createdDtStamp = DateTime.Today,
                type = "New Employee Added",
                result = 1,
                ids = "1,2,3"
            };
        }
        public static List<NotificationModel> GetNotificationModels()
        {
            List<NotificationModel> notificationModel = new List<NotificationModel>();
            notificationModel.Add(new NotificationModel
            {
                id = 1,
                empId = 1,
                message = "New Notificaion Arrive.",
                managerId = 1,
                action = "New Employee assigned",
                isSent = true,
                markAs = true,
                createdBy = 1,
                createdDtStamp = DateTime.Today,
                type = "New Employee Added",
                result = 1,
                ids = "1,2,3"
            });
            return notificationModel;
        }

        //SmsVerificationCode
        public static SmsVerificationCode GetSmsVerificationCode()
        {
            return new SmsVerificationCode
            {
                email = "herry@abc.com",
                empId = "CSS_1",
                verificationCode = 457854,
                verificationCodeTimeOut = 2,
                phoneNumber = "+91 8866553322",
                smsCodeMessage = "Please submit verification code.",
                createdBy = 1,
                updatedBy = 1,
                result = 1
            };
        }

        // Dashboard Data

        //EmpDashboard 
        public static List<EmpDashboard> GetEmpDashboards()
        {
            List<EmpDashboard> empDashboards = new List<EmpDashboard>();
            empDashboards.Add(new EmpDashboard
            {
                empId = 1,
                empName = "Herry",
                noOfRatings = 10,
                lyrNoOfRatings = 10,
                lastScoreRank = "A+",
                currentYrAvgScore = 10,
                currentYrAvgScoreRank = "A+",
                lyrAvgScore = 10,
                lyrAvgScoreRank = "A+",
                lastAssessedDate = DateTime.Today.ToString(),
                lastGeneralScoreRank = "A+",
                lastManagerAction = "Herry",
                lastScore = 10,
                lastScoreRemarks = "Remarks",
                lastScoreSummary = "A+",
                remarks = GetRemarks(),
                graphCategories = GetGraphCategories()
            });

            return empDashboards;
        }

        //GraphCategories

        public static List<GraphCategories> GetGraphCategories()
        {
            List<GraphCategories> graphCategories = new List<GraphCategories>();

            graphCategories.Add(new GraphCategories
            {
                lstWeekly = GetWeeklies(),
                lstMonthly = GetMonthlies(),
                lstYearly = GetYearlies()
            });
            return graphCategories;
        }

        //Weekly
        public static List<Weekly> GetWeeklies()
        {
            List<Weekly> weeklies = new List<Weekly>();
            weeklies.Add(new Weekly
            {
                empid = 1,
                weekNo = "First",
                weekScore = 1,
                weekScoreRank = "A+"
            });

            return weeklies;
        }

        //Monthly
        public static List<Monthly> GetMonthlies()
        {
            List<Monthly> monthlies = new List<Monthly>();
            monthlies.Add(new Monthly
            {
                empid = 1,
                monthNo = "First",
                monthScore = 1,
                monthScoreRank = "A+"
            });

            return monthlies;
        }

        //Year
        public static List<Yearly> GetYearlies()
        {
            List<Yearly> yearlies = new List<Yearly>();

            yearlies.Add(new Yearly
            {
                empid = 1,
                yearNo = "First",
                yearScore = 111,
                yearScoreRank = "A+"
            });

            return yearlies;
        }

        //Remarks
        public static List<Remarks> GetRemarks()
        {
            List<Remarks> remarks = new List<Remarks>();
            remarks.Add(new Remarks
            {
                empid = 1,
                category = "category1",
                remark = "remark",
                status = 1,
                assessmentDate = DateTime.Today,
                firstName = "Herry",
                lastName = "Patel",
                assessmentByImgPath = "D:\\abc"
            });

            return remarks;
        }

        //ManagerDashBoardModel
        public static List<ManagerDashBoardModel> GetManagerDashBoardModels()
        {
            List<ManagerDashBoardModel> managerDashBoardModels = new List<ManagerDashBoardModel>();
            managerDashBoardModels.Add(new ManagerDashBoardModel
            {
                companyId = 1,
                managerId = 1,
                deparmentList = "HR,QA,Dev",
                yearId = 1,
                directRptCnt = 5,
                directRptAvgScore = 10,
                directRptAvgScoreRank = "A+",
                orgRptAvgScore = 10,
                orgRptCnt = 5,
                orgRptAvgScoreRank = "A+",
                lstGraphDirectRptPct = GetGraphDirectRptPcts(),
                lstGraphDirectRptRank = GetGraphDirectRptRanks(),
                lstGraphOrgRptPct = GetGraphOrgRptPcts(),
                lstGraphOrgRptRank = GetGraphOrgRptRanks(),
                lstGraphTodayDirectRpt = GetGraphTodayDirectRpts(),
                lstGraphTodayOrgRpt = GetGraphTodayOrgRpts()
            });

            return managerDashBoardModels;
        }


        //GraphDirectRptPct
        public static List<GraphDirectRptPct> GetGraphDirectRptPcts()
        {
            List<GraphDirectRptPct> graphDirectRptPcts = new List<GraphDirectRptPct>();
            graphDirectRptPcts.Add(new GraphDirectRptPct
            {
                directMonYr = "2019",
                directMonYrId = 1,
                directRptEmpCnt = 5,
                directRptEmpPct = 5,
                directScoreRank = "A+"
            });
            return graphDirectRptPcts;
        }

        //GraphDirectRptRank
        public static List<GraphDirectRptRank> GetGraphDirectRptRanks()
        {
            List<GraphDirectRptRank> graphDirectRptRanks = new List<GraphDirectRptRank>();
            graphDirectRptRanks.Add(new GraphDirectRptRank
            {
                directAvgMonYr = "2019",
                directAvgMonYrId = 1,
                directAvgScoreRank = "A+",
                directRptAvgScore = 10
            });

            return graphDirectRptRanks;
        }

        //GraphOrgRptPct
        public static List<GraphOrgRptPct> GetGraphOrgRptPcts()
        {
            List<GraphOrgRptPct> graphOrgRptPct = new List<GraphOrgRptPct>();
            graphOrgRptPct.Add(new GraphOrgRptPct
            {
                orgMonYr = "2019",
                orgMonYrId = 1,
                orgRptEmpCnt = 5,
                orgRptEmpPct = 5,
                orgScoreRank = "A+"
            });

            return graphOrgRptPct;
        }

        //GraphOrgRptRank
        public static List<GraphOrgRptRank> GetGraphOrgRptRanks()
        {
            List<GraphOrgRptRank> graphOrgRptRanks = new List<GraphOrgRptRank>();
            graphOrgRptRanks.Add(new GraphOrgRptRank
            {
                orgAvgMonYr = "2019",
                orgAvgMonYrId = 1,
                orgAvgScoreRank = "A+",
                orgRptAvgScore = 10
            });

            return graphOrgRptRanks;
        }

        //GraphTodayDirectRpt
        public static List<GraphTodayDirectRpt> GetGraphTodayDirectRpts()
        {
            List<GraphTodayDirectRpt> graphTodayDirectRpts = new List<GraphTodayDirectRpt>();
            graphTodayDirectRpts.Add(new GraphTodayDirectRpt
            {
                TodayDirectRptCnt = 5,
                TodayDirectRptRank = "A+",
                TodayRptEmpList = "Herry,Tom,Mickle"
            });

            return graphTodayDirectRpts;
        }

        //GraphTodayOrgRpt
        public static List<GraphTodayOrgRpt> GetGraphTodayOrgRpts()
        {
            List<GraphTodayOrgRpt> graphTodayOrgRpts = new List<GraphTodayOrgRpt>();
            graphTodayOrgRpts.Add(new GraphTodayOrgRpt
            {
                TodayOrgEmpList = "herry,Tom,Mickle",
                TodayOrgRptCnt = 5,
                TodayOrgRptRank = "A+"
            });
            return graphTodayOrgRpts;
        }

        //EmployeeDashboardModel
        public static List<EmployeeDashboardModel> GetEmployeeDashboardModels()
        {
            List<EmployeeDashboardModel> employeeDashboardModels = new List<EmployeeDashboardModel>();

            employeeDashboardModels.Add(new EmployeeDashboardModel
            {
                empId = 1,
                companyId = 1,
                lastScore = 1.1M,
                lastRank = "A+",
                noOfCount = 5,
                lyrNoOfCount = 5,
                currAvgScore = 10,
                currAvgScoreRank = "A+",
                lyrAvgScore = 1.1M,
                lyrAvgScoreRank = "A+",
                lastAssessedDate = DateTime.Now.ToString(),
                lastScoreRemarks = "Last Remark",
                lastManagerAction = "Manager Action",
                lastScoreSummary = "Score summary",
                lastGeneralScoreRank = "Last General Score Rank",
                empName = "Herry ",
                category = "catagory_1",
                remark = "Remark",
                generalRemark = "General Remark",
                status = 2,
                generalStatus = 2,
                assessmentDate = DateTime.Today,
                assessmentById = 1,
                firstName = "Herry",
                lastName = "Patel",
                assessmentByImgPath = "D:\\ABC",
                monYrId = "First",
                monYr = "2019",
                dayScore = 10,
                dayScoreRank = "A+",
                monthAvgScore = 10,
                monthAvgScoreRank = "A+",
                weekNo = "First",
                wkNo = 1,
                weekAvgScore = 10,
                weekAvgScoreRank = "A+",
                year = "2019",
                yearId = 1,
                yearAvgScore = 1.1M,
                yeatAvgScoreRank = "A+",
            });
            return employeeDashboardModels;
        }

        //AssessmentScoreModel
        public static AssessmentScoreModel GetAssessmentScoreModel()
        {
            return new AssessmentScoreModel
            {
                empId = 1,
                companyId = 1,
                empName = "Herry patel",
                empScore = 10,
                empScoreRank = "A+",
                assessmentPeriod = "Last Month",
                assessmentById = 1,
                assessmentBy = "Herry",
                generalScoreRank = "A+",
                scoreRemarks = "no remark",
                scoreSummary = "summary",
                ratingDate = DateTime.Today,
                managerAction = "Add"
            };
        }


        //EmpAssessmentModel
        public static EmpAssessmentModel GetEmpAssessmentModel()
        {
            return new EmpAssessmentModel
            {
                id = 1,
                empId = 1,
                assessmentDate = DateTime.Today,
                assessmentPeriod = "Last Month",
                assessmentBy = 1,
                remarks = "remarks",
                generalStatus = 2,
                remarksType = 2,
                bactive = true,
                createdBy = 1,
                updatedBy = 1,
                empassessmentdet = GetEmpAssessmentDets(),
                questCategories = GetQuestionnariesCategories()
            };
        }

        //EmpAssessmentDet
        public static List<EmpAssessmentDet> GetEmpAssessmentDets()
        {
            List<EmpAssessmentDet> empAssessmentDet = new List<EmpAssessmentDet>();
            empAssessmentDet.Add(new EmpAssessmentDet
            {
                assessmentId = 1,
                questionId = 1,
                answerid = 1,
                remarks = "remark",
                kpiStatus = 2,
                bactive = true,
                createdBy = 1,
                updatedBy = 1,
                result = 1
            });
            return empAssessmentDet;
        }

        //QuestionnariesCategory
        public static List<QuestionnariesCategory> GetQuestionnariesCategories()
        {
            List<QuestionnariesCategory> questionnariesCategories = new List<QuestionnariesCategory>();
            questionnariesCategories.Add(new QuestionnariesCategory
            {
                categoryid = 1,
                category = "Category1",
                isActive = true,
                createdby = "1",
                updatedby = "1",
                lstQuestionneries = GetQuestionnaries()
            });

            return questionnariesCategories;
        }

        // EmpAssessmentScore
        public static EmpAssessmentScore GetEmpAssessmentScore()
        {
            return new EmpAssessmentScore
            {
                empId = 1,
                assessmentId = 1,
                result = 1
            };
        }

        //AssessmentYearModel
        public static List<AssessmentYearModel> GetAssessmentYearModels()
        {
            List<AssessmentYearModel> assessmentYearModels = new List<AssessmentYearModel>();
            assessmentYearModels.Add(new AssessmentYearModel
            {
                AssessedYear = "2019",
                CompanyId = 1,
                ManagerId = 1
            });

            return assessmentYearModels;
        }

        //CompanyDetailsModel
        public static CompanyDetailsModel GetCompanyDetailsModel()
        {
            return new CompanyDetailsModel
            {
                compId = 1,
                companyId = "1",
                industryTypeId = 1,
                industryType = "Banking",
                companyName = "CSS ltd",
                address1 = "valsad",
                address2 = "gujrat",
                city = "valsad",
                state = "valsad",
                zipcode = "390056",
                country = "india",
                phoneNo1 = 2255446677,
                phoneNo2 = 7845127845,
                website = "abc.com",
                keyEmpName = "xyz",
                keyEmpEmail = "herry@abc.com",
                remarks = "remark",
                costPerEmp = 4512,
                fixedAmtPerMon = 4512,
                dealsRemarks = "deal remarks",
                compImgPath = string.Empty,
                compImage = string.Empty,
                compFolderPath = "E:\\image",
                contractStartDate = DateTime.Today.AddMonths(-2),
                contractEndDate = DateTime.Today.AddMonths(2),
                gracePeriod = 10,
                inActivityDays = 10,
                reminderDays = 10,
                isActive = true,
                createdBy = 1,
                updatedBy = 1,
                result = 1
            };
        }
        public static List<CompanyDetailsModel> GetCompanyDetailsModels()
        {
            List<CompanyDetailsModel> companyDetailsModels = new List<CompanyDetailsModel>();
            companyDetailsModels.Add(new CompanyDetailsModel
            {
                compId = 1,
                companyId = "cmp_1",
                industryTypeId = 2,
                industryType = "Banking",
                companyName = "abc ltd",
                address1 = "valsad",
                address2 = "gujrat",
                city = "valsad",
                state = "valsad",
                zipcode = "390056",
                country = "india",
                phoneNo1 = 2255446677,
                phoneNo2 = 7845127845,
                website = "abc.com",
                keyEmpName = "herry",
                keyEmpEmail = "herry@abc.com",
                remarks = "remark",
                costPerEmp = 4512,
                fixedAmtPerMon = 4512,
                dealsRemarks = "deal remarks",
                compImgPath = "d:\\image",
                compFolderPath = "E:\\image",
                contractStartDate = DateTime.Today.AddMonths(-2),
                contractEndDate = DateTime.Today.AddMonths(2),
                gracePeriod = 10,
                inActivityDays = 10,
                reminderDays = 10,
                isActive = true,
                createdBy = 1,
                updatedBy = 1,
                result = 1
            });
            return companyDetailsModels;
        }

        //CompLogoModel
        public static CompLogoModel GetCompLogoModel()
        {
            return new CompLogoModel
            {
                companyId = 1,
                compImgPath = "D:\\Image",
                compFolderPath = "E:\\Image",
                compImage = "compimage",
                updatedBy = 1
            };
        }

        //CompanyConfigModel
        public static CompanyConfigModel GetCompanyConfigModel()
        {
            return new CompanyConfigModel
            {
                companyId = 1,
                companyDomain = "xyz",
                serverName = "198.255.10.15",
                dbName = "abc_1",
                userName = "mno",
                password = "123456",
                createdBy = 1,
                updatedBy = 1,
                result = 1
            };
        }

        //InActivityManagers
        public static InActivityManagers GetInActivityManagers()
        {
            return new InActivityManagers
            {
                companyId = 1,
                empId = 1,
                firstName = "herry",
                lastName = "patel",
                email = "herry@abc.com",
                smsText = "sms text",
                emailText = "email text",
                phoneNumber = "+84 784512124578",
                inActivityDays = 4,
                createdBy = 1,
                isMailSent = true,
                phoneConfirmed = true,
                optForSms = true,
                result = 1
            };
        }

        // ActionList
        public static ActionList GetActionList()
        {
            return new ActionList
            {
                ActionId = 1,
                ActionPermissions = GetActionwisePermissionModels(),
                Actions = "Spark"
            };
        }

        public static List<ActionList> GetActionLists()
        {
            List<ActionList> actionLists = new List<ActionList>();

            actionLists.Add(new ActionList
            {
                ActionId = 1,
                ActionPermissions = GetActionwisePermissionModels(),
                Actions = "Spark"
            });

            return actionLists;
        }

        public static List<ActionwisePermissionModel> GetActionwisePermissionModels()
        {
            List<ActionwisePermissionModel> actionwisePermissionModels = new List<ActionwisePermissionModel>();
            actionwisePermissionModels.Add(new ActionwisePermissionModel
            {
                ActionId = 1,
                Actions = "spark",
                DimensionId = 1,
                DimensionType = "spark",
                DimensionValueid = 1,
                DimensionValues = "abc",
                CanView = true,
                CanAdd = true,
                CanEdit = true,
                CanDelete = true,
                CreatedBy = 1,
                Result = 1,
                ManagerId = 1
            });
            return actionwisePermissionModels;
        }

        //Claims
        public static Claims GetClaims()
        {
            return new Claims
            {
                Id = "83",
                ClaimType = "CompId",
                ClaimValue = "1",
                AuthUserId = "6A0CCD2D-E0F0-4FD0-AC50-DD18A419EA68"
            };
        }
    }
}
