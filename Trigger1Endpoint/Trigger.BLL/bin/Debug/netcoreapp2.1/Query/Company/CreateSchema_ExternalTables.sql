--IF EXISTS (
--		SELECT *
--		FROM sys.databases AS d
--		WHERE d.name = 'DbName'
--		)
--BEGIN
--	ALTER MASTER KEY REGENERATE
--		WITH ENCRYPTION BY PASSWORD = 'DBPassword';
--END
--ELSE
--BEGIN
	CREATE MASTER KEY ENCRYPTION BY PASSWORD = 'DBPassword';
--END

CREATE DATABASE SCOPED CREDENTIAL Trigger_MT WITH IDENTITY = 'DBUserName'
		,SECRET = 'DBPassword';

CREATE EXTERNAL DATA SOURCE Country_Data_Source WITH 
           (TYPE = RDBMS,
            LOCATION = 'DBServerName',
            DATABASE_NAME = 'IndexDB',  
            CREDENTIAL = Trigger_MT
            ) ;


CREATE EXTERNAL TABLE  [dbo].[Country](
	[id] [int] NOT NULL,
	[country] [nvarchar](100) NOT NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL)
WITH 
( DATA_SOURCE = Country_Data_Source);

CREATE EXTERNAL DATA SOURCE Regions_Data_Source WITH 
           (TYPE = RDBMS,
            LOCATION = 'DBServerName',
            DATABASE_NAME = 'IndexDB',  
            CREDENTIAL = Trigger_MT
            ) ;


CREATE EXTERNAL TABLE  [dbo].[Regions](
	[regionid] [int] NOT NULL,
	[countryid] [int] NOT NULL,
	[region] [varchar](100) NOT NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL)
WITH 
( DATA_SOURCE = Regions_Data_Source);

CREATE EXTERNAL DATA SOURCE RoleMaster_Data_Source WITH 
           (TYPE = RDBMS,
            LOCATION = 'DBServerName',
            DATABASE_NAME = 'IndexDB',  
            CREDENTIAL = Trigger_MT
            ) ;


CREATE EXTERNAL TABLE  [dbo].[RoleMaster](
	[roleid] [tinyint] NOT NULL,
	[role] [varchar](50) NOT NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL)
WITH 
( DATA_SOURCE = RoleMaster_Data_Source);

CREATE EXTERNAL DATA SOURCE Raceorethnicity_Data_Source WITH 
           (TYPE = RDBMS,
            LOCATION = 'DBServerName',
            DATABASE_NAME = 'IndexDB',  
            CREDENTIAL = Trigger_MT
            ) ;


CREATE EXTERNAL TABLE  [dbo].[Raceorethnicity](
	[id] [smallint] NOT NULL,
	[raceorethnicity] [nvarchar](60) NOT NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL)
WITH 
( DATA_SOURCE = Raceorethnicity_Data_Source);

CREATE EXTERNAL DATA SOURCE WidgetTypeMaster_Data_Source WITH 
           (TYPE = RDBMS,
            LOCATION = 'DBServerName',
            DATABASE_NAME = 'IndexDB',  
            CREDENTIAL = Trigger_MT
            ) ;

CREATE EXTERNAL TABLE [dbo].[WidgetTypeMaster](
	[id] [int] NOT NULL,
	[WidgetType] [varchar](100) NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL
)WITH (DATA_SOURCE = WidgetTypeMaster_Data_Source);


CREATE EXTERNAL DATA SOURCE WidgetMaster_Data_Source WITH 
           (TYPE = RDBMS,
            LOCATION = 'DBServerName',
            DATABASE_NAME = 'IndexDB',  
            CREDENTIAL = Trigger_MT
            ) ;


CREATE EXTERNAL TABLE [dbo].[WidgetMaster](
	[id] [int] NOT NULL,
	[WidgetName] [varchar](100) NOT NULL,
	[WidgetTypeId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[Createdby] [int] NOT NULL,
	[Createddtstamp] [datetime] NOT NULL,
	[Updatedby] [int] NULL,
	[Updateddtstamp] [datetime] NULL,
	[WidgetActualName] [varchar](150) NULL,
	[Position] [decimal](10, 4) NULL
	)WITH (DATA_SOURCE = WidgetMaster_Data_Source);


CREATE EXTERNAL DATA SOURCE CompanyDetails_Data_Source WITH 
           (TYPE = RDBMS,
            LOCATION = 'DBServerName',
            DATABASE_NAME = 'IndexDB',  
            CREDENTIAL = Trigger_MT
            ) ;


CREATE EXTERNAL TABLE [dbo].[CompanyDetails](
	[compid] [int] NOT NULL,
	[industrytypeid] [tinyint] NOT NULL,
	[companyname] [nvarchar](200) NOT NULL,
	[address1] [nvarchar](250) NOT NULL,
	[address2] [nvarchar](250) NULL,
	[city] [nvarchar](100) NOT NULL,
	[state] [nvarchar](100) NOT NULL,
	[zipcode] [varchar](25) NULL,
	[country] [nvarchar](100) NOT NULL,
	[phoneno1] [bigint] NOT NULL,
	[phoneno2] [bigint] NULL,
	[website] [varchar](100) NULL,
	[keyemployee] [nvarchar](200) NULL,
	[keyemplemail] [nvarchar](100) NULL,
	[remarks] [nvarchar](max) NULL,
	[costperemp] [decimal](10, 2) NULL,
	[fixedamtpermonth] [decimal](10, 2) NULL,
	[dealsremarks] [nvarchar](max) NULL,
	[bactive] [bit] NOT NULL,
	[createdby] [int] NOT NULL,
	[createddtstamp] [datetime] NOT NULL,
	[updatedby] [int] NULL,
	[updateddtstamp] [datetime] NULL,
	[CompanyId] [varchar](100) NULL,
	[CompImgPath] [varchar](max) NULL,
	[ContractStartDate] [date] NULL,
	[ContractEndDate] [date] NULL,
	[gracePeriod] [int] NULL,
	[InActivityDays] [int] NOT NULL,
	[ReminderDays] [int] NOT NULL,
	[OrganizationTypeId] [tinyint] NULL
	)WITH (DATA_SOURCE = CompanyDetails_Data_Source);

CREATE EXTERNAL DATA SOURCE ActionMaster_Data_Source WITH 
           (TYPE = RDBMS,
            LOCATION = 'DBServerName',
            DATABASE_NAME = 'IndexDB',  
            CREDENTIAL = Trigger_MT
            ) ;
CREATE EXTERNAL TABLE [dbo].[ActionMaster]
(
	[Id] [smallint] NOT NULL,
	[Actions] [varchar](50) NOT NULL,
	[Bactive] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDtstamp] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDtstamp] [datetime] NULL
)
WITH (DATA_SOURCE = [ActionMaster_Data_Source]);


CREATE EXTERNAL DATA SOURCE Dimension_Data_Source WITH 
           (TYPE = RDBMS,
            LOCATION = 'DBServerName',
            DATABASE_NAME = 'IndexDB',  
            CREDENTIAL = Trigger_MT
            ) ;

CREATE EXTERNAL TABLE [dbo].[Dimensions]
(
	[Id] [smallint] NOT NULL,
	[DimensionType] [varchar](50) NOT NULL,
	[BActive] [bit] NOT NULL,
	[CreatedBy] [int] NOT NULL,
	[CreatedDtstamp] [datetime] NOT NULL,
	[UpdatedBy] [int] NULL,
	[UpdatedDtstamp] [datetime] NULL
)
WITH (DATA_SOURCE = [Dimension_Data_Source]);






