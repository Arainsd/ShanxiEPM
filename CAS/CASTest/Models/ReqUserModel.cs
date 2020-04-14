namespace CASTest.Models
{
    public class ReqUserModel
    {

        public string userId { get; set; }

        public string reqDate { get; set; }

        public string reqTime { get; set; }

        public UserP data { get; set; }

       
    }

    public class UserP
    {
        public string userCode { get; set; }
        public string passWord { get; set; }
    }
}