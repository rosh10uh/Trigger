using OneRPP.Restful.DAO;
using OneRPP.Restful.DataAnnotations;
using System.Collections.Generic;
using Trigger.BLL.Shared;
using Trigger.DTO;

namespace Trigger.DAL.WidgetLibraryRepo
{
    [QueryPath("Trigger.DAL.Query.WidgetLibrary.WidgetLibrary")]
    public class WidgetLibraryRepository : DaoRepository<WidgetLibrary>
    {
        public List<WidgetLibrary> GetUserwiseWidget(WidgetLibrary widgetLibrary)
        {
            return ExecuteQuery<List<WidgetLibrary>>(widgetLibrary, Messages.invokeGetUserwiseWidget);
        }
    }

    [QueryPath("Trigger.DAL.Query.WidgetLibrary.WidgetLibrary")]
    public class WidgetPositionRepository : DaoRepository<List<WidgetLibrary>>
    {
        
    }
}
