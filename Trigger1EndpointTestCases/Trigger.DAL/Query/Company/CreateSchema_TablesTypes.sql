CREATE TYPE [dbo].[Type_employeedetails_1] AS TABLE(
	[empid] [int] NULL,
	[companyid] [int] NOT NULL,
	[firstname] [nvarchar](100) NOT NULL,
	[middlename] [nvarchar](100) NULL,
	[lastname] [nvarchar](100) NOT NULL,
	[suffix] [varchar](8) NULL,
	[email] [varchar](100) NULL,
	[jobtitle] [varchar](100) NOT NULL,
	[joiningdate] [varchar](15) NOT NULL,
	[workcity] [varchar](100) NOT NULL,
	[workstate] [varchar](100) NOT NULL,
	[workzipcode] [varchar](25) NOT NULL,
	[departmentid] [smallint] NOT NULL,
	[department] [varchar](100) NULL,
	[managerid] [int] NULL,
	[managername] [nvarchar](200) NULL,
	[managerlname] [nvarchar](200) NULL,
	[empstatus] [bit] NOT NULL,
	[roleid] [tinyint] NOT NULL,
	[dateofbirth] [varchar](15) NULL,
	[raceorethanicityid] [smallint] NULL,
	[gender] [nvarchar](50) NULL,
	[jobcategory] [nvarchar](100) NULL,
	[jobcode] [nvarchar](100) NULL,
	[jobgroup] [nvarchar](100) NULL,
	[lastpromodate] [varchar](15) NULL,
	[currentsalary] [decimal](12, 2) NULL,
	[lastincdate] [varchar](15) NULL,
	[emplocation] [nvarchar](100) NULL,
	[countryid] [int] NULL,
	[regionid] [int] NULL,
	[empimgpath] [varchar](1000) NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [varchar](15) NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [varchar](15) NULL,
	[EmployeeId] [varchar](100) NULL,
	[CSVManagerId] [varchar](100) NULL,
	[phonenumber] [varchar](25) NULL
);

CREATE TYPE [dbo].[Type_userLogin_1] AS TABLE(
	[username] [varchar](150) NOT NULL,
	[password] [varchar](max) NOT NULL,
	[empid] [int] NOT NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[updatedby] [int] NULL
);

CREATE TYPE [dbo].[Type_PhoneNumber] AS TABLE(
	[phoneNumber] [varchar](25) NULL,
	[email] [varchar](100) NULL,
	[employeeid] [varchar](100) NULL
);


