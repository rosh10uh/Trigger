using Trigger.BLL.Shared;
using Trigger.DTO;

namespace Trigger.Utility
{
    public static class JsonSettings
    {
        public static JsonData UserDataWithStatusMessage(object obj, int iStatus, string message)
        {
            JsonData jsonData = new JsonData();
            jsonData.status = iStatus;
            jsonData.message = message;
            if (obj == null)
            {
                obj = new object();
            }
            jsonData.data = new object[1];
            jsonData.data[0] = obj;
            return jsonData;
        }

        public static CustomJsonData UserCustomDataWithStatusMessage(object obj, int iStatus, string message)
        {
            CustomJsonData jsonData = new CustomJsonData();
            jsonData.status = iStatus;
            jsonData.message = message;
            if (obj == null)
            {
                obj = new object();
            }

            jsonData.data = obj;
            return jsonData;
        }

        public static string ResponseStatusMessage(int iStatus)
        {
            string message = null;
            switch (iStatus)
            {
                case 200:
                    message = Messages.ok;
                    break;
                case 400:
                    message = Messages.badRequest;
                    break;
                case 401:
                    message = Messages.unauthorizedAcces;
                    break;
                case 404:
                    message = Messages.notFound;
                    break;
                case 500:
                    message = Messages.internalServerError;
                    break;
            }
            return message;
        }

    }
}