namespace src.Models.RequestModel
{
    public class UserSetLockoutEndDateRequest
    {
        public string UserId { get; set; }
        public int UserLockOutEndDateFormDays { get; set; }
    }
}
