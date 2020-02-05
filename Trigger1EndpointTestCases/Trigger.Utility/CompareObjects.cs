using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Trigger.Utility
{
    public static class CompareObjects
    {
        public static bool CompareObject<T>(T newJsonObject, T givenJsonObject)
        {
            string permissionJson = JsonConvert.SerializeObject(newJsonObject).ToLower();
            string parameterJson = JsonConvert.SerializeObject(givenJsonObject).ToLower();
            return JToken.DeepEquals(permissionJson, parameterJson);
        }
    }
}
