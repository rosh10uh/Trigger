using OneRPP.Restful.Contracts.Utility;

namespace Trigger.Utility
{
    public class TriggerResourceReader
	{
		public readonly IResourceReader _resourceReader;

		public TriggerResourceReader(IResourceReader resourceReader)
		{
			_resourceReader = resourceReader;
		}

		public string GetValue(string filePath)
		{
			return _resourceReader.ReadResource(filePath);
		}
	}
}
