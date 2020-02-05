using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using OneRPP.Restful.Contracts.Enum;
using OneRPP.Restful.Contracts.Resource;
using OneRPP.Restful.DAO;
using OneRPP.Restful.DAO.Interface;
using OneRPP.Restful.DAO.Model;
using System.Collections.Generic;
using System.Data;
using Trigger.BLL.Shared.Interfaces;

namespace Trigger.Test.Shared
{
    public class BaseRepositoryTest
    {
        private readonly Mock<IExecuteData> _executeData;
        private readonly Mock<IFindDaoRepository> _findDaoRepository;
        private readonly Mock<IUowConnectionManager> _uowConnectionManager;
        private readonly Mock<IClaims> _mockClaims;
        private readonly Mock<ILogger<DaoContext>> _logger;
        private readonly string _repositoryName;

        /// <summary>
        /// Initializes an instance of BaseRepositoryTest class.
        /// </summary>
        /// <param name="repositoryName"></param>
        public BaseRepositoryTest(string repositoryName)
        {
            var mockRepository = new MockRepository(MockBehavior.Default);
            _executeData = mockRepository.Create<IExecuteData>();
            _findDaoRepository = mockRepository.Create<IFindDaoRepository>();
            _uowConnectionManager = mockRepository.Create<IUowConnectionManager>();
            _mockClaims = mockRepository.Create<IClaims>();
            _logger = mockRepository.Create<ILogger<DaoContext>>();
            mockRepository.Create<IDaoRepositoryInitializer>();
            _repositoryName = repositoryName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Input Repository model</typeparam>
        /// <typeparam name="R">Output Repository model</typeparam>
        /// <typeparam name="C">Repository Context class</typeparam>
        /// <param name="returnValue">Object of type R</param>
        /// <returns>ServiceCollection</returns>
        public ServiceCollection GetServiceCollection<T, R, C>(R returnValue) where T : new()
            where R : new()
        {
            //Arrange
            _executeData.Setup(x => x.ExecuteQuery<T, R>(It.IsAny<T>(),
                    It.IsAny<string>(), It.IsAny<DbEngine>(), It.IsAny<IDbConnection>()))
                .Returns(returnValue);
            _uowConnectionManager.Setup(x => x.DbConnectionModel)
                .Returns(new DbConnectionModel(It.IsAny<IDbConnection>(), It.IsAny<DbEngine>()));
            _findDaoRepository.Setup(x => x.FindRepositories(It.IsAny<DaoContext>()))
                .Returns(new List<DaoRepositoryModel>
                {
                    new DaoRepositoryModel { Name = _repositoryName, Type = typeof(C).GetProperty(_repositoryName) }
                });

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient(x => _executeData.Object);
            serviceCollection.AddTransient(x => _findDaoRepository.Object);
            serviceCollection.AddTransient<IDaoRepositoryInitializer>(x => new DaoRepositoryInitializer(_findDaoRepository.Object));
            serviceCollection.AddTransient(x => _uowConnectionManager.Object);
            serviceCollection.AddTransient(x => _logger.Object);
            serviceCollection.AddTransient(x => _mockClaims.Object);
            return serviceCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">Input Repository model</typeparam>
        /// <typeparam name="C">Repository Context class</typeparam>
        /// <param name="returnValue">String type return value</param>
        /// <returns>ServiceCollection</returns>
        public ServiceCollection GetServiceCollectionString<T, C>(string returnValue) where T : new()
        {
            //Arrange
            _executeData.Setup(x => x.ExecuteQuery<T, string>(It.IsAny<T>(),
                    It.IsAny<string>(), It.IsAny<DbEngine>(), It.IsAny<IDbConnection>()))
                .Returns(returnValue);
            _uowConnectionManager.Setup(x => x.DbConnectionModel)
                .Returns(new DbConnectionModel(It.IsAny<IDbConnection>(), It.IsAny<DbEngine>()));
            _findDaoRepository.Setup(x => x.FindRepositories(It.IsAny<DaoContext>()))
                .Returns(new List<DaoRepositoryModel>
                {
                    new DaoRepositoryModel { Name = _repositoryName, Type = typeof(C).GetProperty(_repositoryName) }
                });

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddTransient(x => _executeData.Object);
            serviceCollection.AddTransient(x => _findDaoRepository.Object);
            serviceCollection.AddTransient<IDaoRepositoryInitializer>(x => new DaoRepositoryInitializer(_findDaoRepository.Object));
            serviceCollection.AddTransient(x => _uowConnectionManager.Object);
            serviceCollection.AddTransient(x => _logger.Object);
            serviceCollection.AddTransient(x => _mockClaims.Object);
            return serviceCollection;
        }
    }
}
