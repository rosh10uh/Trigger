namespace Trigger.DTO
{
    public class JsonData
    {
        public string message { get; set; }
        public int status { get; set; }
        public object[] data { get; set; }
    }

    public class CustomJsonData
    {
        public string message { get; set; }
        public int status { get; set; }
        public object data { get; set; }
    }

}