using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using Trigger.DAL;
using Trigger.Test.Shared;
using Xunit;
using widgetLibraryModel = Trigger.DTO.WidgetLibrary;

namespace Trigger.Test.DAL.WidgetLibrary
{
    public class WidgetLibraryRepositoryTest : BaseRepositoryTest
    {
        public WidgetLibraryRepositoryTest() : base("WidgetLibraryRepository")
        {

        }

        [Fact]
        public void To_Verify_GetUserwiseWidget_Method()
        {
            // Arrange
            var triggerContext = new TriggerContext(GetServiceCollection<widgetLibraryModel, List<widgetLibraryModel>, TriggerContext>(new List<widgetLibraryModel>()).BuildServiceProvider());

            //Act
            var result = triggerContext.WidgetLibraryRepository.GetUserwiseWidget(It.IsAny<widgetLibraryModel>());

            //Assert      
            Assert.IsType<List<widgetLibraryModel>>(result);
            Assert.NotNull(result);
        }
    }
}
