using Hangfire;
using Microsoft.Extensions.Logging;
using OneRPP.Restful.Contracts.Resource;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Trigger.BLL.Shared;
using Trigger.DAL.Employee;
using Trigger.DAL.Shared;
using Trigger.DTO;
using Trigger.Utility;

namespace Trigger.DAL.Company
{
    /// <summary>
    /// Contains method to manage multiple operation.
    /// </summary>
    public class CompanyContext
	{
		/// <summary>
		/// Use to get service of CatalogDbContext
		/// </summary>
		private readonly TriggerCatalogContext _catalogDbContext;

		/// <summary>
		/// Use to get service of IEnvironmentVariables
		/// </summary>
		private readonly IEnvironmentVariables _environmentVariables;

		private readonly string _dbServerName;
		private readonly string _dbUserName;
		private readonly string _dbPassword;
		private readonly string _indexDb;
		private readonly string _catalogConnectionString;
		private readonly string _elasticPool;
		private readonly ILogger<CompanyContext> _iLogger;

		/// <summary>
		/// Use to get service of TriggerResourceReader
		/// </summary>
		private readonly TriggerResourceReader _triggerResourceReader;

        /// <summary>
        /// Initializes a new instance of the roles class using CatalogDbContext
        /// </summary>
        /// <param name="catalogDbContext"></param>
        public CompanyContext(TriggerCatalogContext catalogDbContext, IEnvironmentVariables environmentVariables, ILogger<CompanyContext> iLogger, TriggerResourceReader triggerResourceReader, EmployeeContext employeeContext)
		{
			_catalogDbContext = catalogDbContext;
			_triggerResourceReader = triggerResourceReader;
			_environmentVariables = environmentVariables;
			_dbServerName = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.DBServerName.ToString()];
			_dbUserName = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.DBUserName.ToString()];
			_dbPassword = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.DBPassword.ToString()];
			_indexDb = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.IndexDB.ToString()];
			_catalogConnectionString = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.ConnectionString.ToString()];
			_elasticPool = Dictionary.ConfigDictionary[Dictionary.ConfigurationKeys.ElasticPool.ToString()];
		    _iLogger = iLogger;

        }

		/// <summary>
		/// This method is responsible to insert company with multiple post
		/// </summary>
		/// <param name="companyDetailsModel">The CompanyDetailsModel</param>
		public virtual CompanyDetailsModel InsertCompany(CompanyDetailsModel companyDetailsModel)
		{
			try
			{
				companyDetailsModel.compImgPath = ConvertToDataTable.EvalNullString(companyDetailsModel.compImgPath, string.Empty);
				var company = _catalogDbContext.CompanyRepository.Insert(companyDetailsModel);
				if (company.result > 0)
				{
                    companyDetailsModel.compId = company.result;
                    CompanyConfigModel companyConfigModel = GetCompanyConfig(companyDetailsModel);
					companyConfigModel.dbName = companyDetailsModel.companyName.Split(' ')[0] + "_" + company.result;
					companyConfigModel.companyId = company.result;
					InsertCompanyDbConfig(companyConfigModel);
					company.result = CreateDatabase(companyConfigModel);
                }

				companyDetailsModel.result = company.result;
			}
			catch(Exception ex)
			{
                _iLogger.LogError(ex.Message);
				throw;
			}

			return companyDetailsModel;
		}

        
        /// <summary>
        /// This Method is responsible get company config detail
        /// </summary>
        /// <param name="companyDetailsModel"></param>
        /// <returns></returns>
        private CompanyConfigModel GetCompanyConfig(CompanyDetailsModel companyDetailsModel)
		{
			return new CompanyConfigModel
			{
				companyId = companyDetailsModel.compId,
				companyDomain = companyDetailsModel.companyName,
				serverName = "",
				dbName = companyDetailsModel.companyName,
				userName = "",
				password = "",
				createdBy = companyDetailsModel.createdBy
			};
		}

		/// <summary>
		/// Execute Add Company Config
		/// </summary>
		/// <param name="companyConfigModel"></param>
		/// <returns></returns>
		public CompanyConfigModel InsertCompanyDbConfig(CompanyConfigModel companyConfigModel)
		{
			return _catalogDbContext.CompanyConfigRepository.InsertCompanyConfig(companyConfigModel);
		}

		/// <summary>
		/// Enqueue Create Databse & Schema for Tenant
		/// </summary>
		/// <param name="companyConfig">CompanyConfig model</param>
		public int CreateDatabase(CompanyConfigModel companyConfig)
		{
			string connString = GetConnectionString(companyConfig.dbName);

			string _queryFolderPath = _environmentVariables.EnvironmentVariables[Messages.queryFolderPath];

			return CreateDatabase(companyConfig, connString, _queryFolderPath);
		}

		/// <summary>
		/// This method is used for getting dynamic connection string by db Name
		/// </summary>
		/// <param name="dbName"></param>
		/// <returns></returns>
		public string GetConnectionString(string dbName)
		{
			return string.Format(CompanyResource.DynamicConnectionString, _dbServerName, dbName, _dbUserName, _dbPassword);
		}

		/// <summary>
		/// Create Database & Schema for Tenant
		/// </summary>
		/// <param name="companyConfig"></param>
		/// <param name="connString"></param>
		/// <param name="filePath"></param>
		[AutomaticRetry(Attempts = 0)]
		public int CreateDatabase(CompanyConfigModel companyConfig, string connString, string filePath)
		{
			int result = 1;
			try
			{
				CreateDb(companyConfig.dbName);

				ExecuteDbTablesObjects(connString, filePath + Messages.createTable);
				ExecuteDbExtTablesObjects(connString, filePath + Messages.createExternalTable, companyConfig.dbName);
				ExecuteDbObjects(connString, filePath + Messages.tableType);
				ExecuteDbObjects(connString, filePath + Messages.tableFunctions);
				ExecuteDbObjects(connString, filePath + Messages.createTrigger);
				ExecuteDbViewObjects(connString, filePath + Messages.tableView, companyConfig.companyId);
				ExecuteDbObjects(connString, filePath + Messages.sps);
				ExecuteDbObjects(connString, filePath + Messages.insertMasterData);
				ExecuteDbMstTablesObjects(connString, filePath + Messages.insertDepartmentMaster, companyConfig.companyId);

				AlterDb(string.Format(Messages.alterDBForCompany, companyConfig.dbName, _elasticPool));

				return result;
			}
			catch (Exception ex)
			{
				result = -2;
				DropDb(companyConfig.dbName);
				_iLogger.LogError(ex.Message);
			}

			return result;
		}

		/// <summary>
		///  Execute Create Database Query
		/// </summary>
		/// <param name="dbName">Name of Database</param>
		private void CreateDb(string dbName)
		{
			SqlConnection sqlConnection = new SqlConnection
			{
				ConnectionString = _catalogConnectionString
			};

			try
			{
				sqlConnection.Open();

				SqlCommand sqlCommand = new SqlCommand
				{
					CommandText = string.Format(Messages.createDBForCompany, dbName),
					Connection = sqlConnection,
					CommandTimeout = 0
				};

				sqlCommand.ExecuteNonQuery();

				_iLogger.LogError(Messages.createDBError);
			}
			catch (Exception ex)
			{
				_iLogger.LogError(ex.Message);
			}
			finally
			{
				sqlConnection.Close();
			}
		}

		/// <summary>
		///  Add Database to elastic pool
		/// </summary>
		/// <param name="sqlQuery">alter database query</param>
		private void AlterDb(string sqlQuery)
		{
			SqlConnection sqlConnection = new SqlConnection()
			{
				ConnectionString = _catalogConnectionString
			};

			try
			{
				SqlCommand sqlCommand = new SqlCommand(sqlQuery, sqlConnection)
				{
					CommandTimeout = 0,
				};

				sqlConnection.Open();
				sqlCommand.ExecuteNonQuery();

				_iLogger.LogError(Messages.alterDBError);
			}
			catch (Exception ex)
			{
				_iLogger.LogError(ex.Message);
                throw;
			}
			finally
			{
				sqlConnection.Close();
			}
		}

		/// <summary>
		///  Create table schemas
		/// </summary>
		/// <param name="connString">Database connection string</param>
		/// <param name="fileName">query file name</param>
		private void ExecuteDbTablesObjects(string connString, string fileName)
		{
			SqlConnection sqlConnection = new SqlConnection(connString);

			try
			{
				string script = _triggerResourceReader.GetValue(fileName);
				sqlConnection.Open();
				SqlCommand sqlCommand = new SqlCommand(script, sqlConnection)
				{
					CommandTimeout = 0
				};

				sqlCommand.ExecuteNonQuery();

				_iLogger.LogError(fileName + Messages.created);
			}
			catch (Exception ex)
			{
				_iLogger.LogError(ex.Message.ToString());
                throw;
            }
            finally
			{
				sqlConnection.Close();
			}
		}

		/// <summary>
		///  Create external table schemas
		/// </summary>
		/// <param name="connString">Database connection string</param>
		/// <param name="fileName">query file name</param>
		private void ExecuteDbExtTablesObjects(string connString, string fileName,string dbName)
		{
			SqlConnection dbCon = new SqlConnection(connString);
			try
			{
				var script = _triggerResourceReader.GetValue(fileName);

				script = script.Replace(Messages.dBPassword, _dbPassword)
					.Replace(Messages.dBUserName, _dbUserName)
					.Replace(Messages.dBServerName, _dbServerName)
					.Replace(Messages.indexDB, _indexDb)
                    .Replace(Messages.dbName, dbName);

                dbCon.Open();
				using (var command = new SqlCommand(script, dbCon))
				{
					command.CommandTimeout = 0;
					command.ExecuteNonQuery();
				}
				_iLogger.LogError(fileName + Messages.created);
			}
			catch (Exception ex)
			{
				_iLogger.LogError(ex.Message.ToString());
                throw;
            }
			finally
			{
				dbCon.Close();
			}
		}

		/// <summary>
		///  Insert default master data
		/// </summary>
		/// <param name="connString">databse connection string</param>
		/// <param name="fileName">query file name</param>
		/// <param name="companyId">Company Id</param>
		private void ExecuteDbMstTablesObjects(string connString, string fileName, int companyId)
		{
			SqlConnection dbCon = new SqlConnection(connString);
			var script = _triggerResourceReader.GetValue(fileName);
			script = script.Replace("@companyid", companyId.ToString());
			try
			{
				dbCon.Open();
				using (var command = new SqlCommand(script, dbCon))
				{
					command.CommandTimeout = 0;
					command.ExecuteNonQuery();
				}
				_iLogger.LogError(fileName + Messages.created);
			}
			catch (Exception ex)
			{
				_iLogger.LogError(ex.Message.ToString());
                throw;
            }
			finally
			{
				dbCon.Close();
			}
		}

		/// <summary>
		///  Create Views & stored procedures schemas
		/// </summary>
		/// <param name="connString">databse connection string</param>
		/// <param name="fileName">query file name</param>
		private void ExecuteDbObjects(string connString, string fileName)
		{
			SqlConnection dbCon = new SqlConnection(connString);

			try
			{
				var script = _triggerResourceReader.GetValue(fileName);
				script = script.Replace("�", " ");

				IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*----------\s*$",
							  RegexOptions.Multiline | RegexOptions.IgnoreCase);
				dbCon.Open();
				foreach (string commandString in commandStrings)
				{
					if (commandString.Trim() != "")
					{
						using (var command = new SqlCommand(commandString, dbCon))
						{
							command.CommandTimeout = 0;
							command.ExecuteNonQuery();
						}
					}
				}
				_iLogger.LogError(fileName + Messages.created);
			}
			catch (Exception ex)
			{
				_iLogger.LogError(ex.Message.ToString());
                throw;
            }
			finally
			{
				dbCon.Close();
			}
		}

		/// <summary>
		///  Create Views & stored procedures schemas
		/// </summary>
		/// <param name="connString">databse connection string</param>
		/// <param name="fileName">query file name</param>
		/// <param name="companyId"></param>
		private void ExecuteDbViewObjects(string connString, string fileName, int companyId)
		{
			SqlConnection dbCon = new SqlConnection(connString);
			try
			{
				var script = _triggerResourceReader.GetValue(fileName);
				script = script.Replace("@companyid", companyId.ToString());

				IEnumerable<string> commandStrings = Regex.Split(script, @"^\s*----------\s*$",
							  RegexOptions.Multiline | RegexOptions.IgnoreCase);

				dbCon.Open();
				foreach (string commandString in commandStrings)
				{
					if (commandString.Trim() != "")
					{
						using (var command = new SqlCommand(commandString, dbCon))
						{
							command.CommandTimeout = 0;
							command.ExecuteNonQuery();
						}
					}
				}

				_iLogger.LogError(fileName + Messages.created);
			}
			catch (Exception ex)
			{
				_iLogger.LogError(ex.Message.ToString());
                throw;
            }
			finally
			{
				dbCon.Close();
			}
		}

		/// <summary>
		///  Execute Drop Database Query if it fails to create
		/// </summary>
		/// <param name="dbName">Name of Database</param>
		private void DropDb(string dbName)
		{
			SqlConnection sqlConnection = new SqlConnection();
			sqlConnection.ConnectionString = _catalogConnectionString;
			SqlCommand myCommand = new SqlCommand(string.Format(Messages.dropDatabase, dbName), sqlConnection);
			try
			{
				sqlConnection.Open();
				myCommand.CommandTimeout = 0;
				myCommand.ExecuteNonQuery();
				_iLogger.LogError(Messages.dbDrop);
			}
			catch (System.Exception ex)
			{
				_iLogger.LogError(ex.Message.ToString());
			}
			finally
			{
				sqlConnection.Close();
			}
		}
        
	}
}
