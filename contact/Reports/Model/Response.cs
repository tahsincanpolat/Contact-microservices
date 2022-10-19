namespace Reports.Model
{
    public class Response
    {
        public object Data { get; set; }
        public string ResponseMessage { get; set; }
        public int ResponseCode { get; set; }

        public Response(int responseCode,string responseMessage,object data)
        {
            this.Data = data;
            this.ResponseCode = responseCode;   
            this.ResponseMessage = responseMessage; 
        }
    }
}
