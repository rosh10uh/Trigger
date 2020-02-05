using OneRPP.Restful.DAO.Interface;
using Trigger.DAL.Shared.Interfaces;
using Trigger.DTO;

namespace Trigger.DAL.Shared
{
    public class ConnectionContext : IConnectionContext
    {
        private readonly IDaoContextFactory _daoContextFactory;
        private readonly Connection _connection;
        private TriggerContext _triggerContext;
        
        public ConnectionContext(IDaoContextFactory daoContextFactory, Connection connection)
        {
            _daoContextFactory = daoContextFactory;
            _connection = connection;
        }

        public TriggerContext TriggerContext
		{
            get
            {
                if (_triggerContext == null )
                {
                    _triggerContext = _daoContextFactory.GetContext<TriggerContext>(_connection.ConnectionString, OneRPP.Restful.Contracts.Enum.DbEngine.MsSql);
                }

                return _triggerContext;
            }
        }
    }
}
