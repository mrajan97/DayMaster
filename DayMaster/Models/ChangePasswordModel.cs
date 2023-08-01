namespace DayMaster.Models
{
    public class ChangePasswordModel
    {
        public string Username { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswer { get; set; }
        public string NewPassword { get; set; }

       
        public bool IsUsernameValid { get; set; } = true;
        public bool SecurityQuestionFetched { get; set; }
        public bool IsSecurityAnswerValid { get; set; }
        public bool PasswordChanged { get; set; }
        public bool AskForUsername { get; set; } = true;
    }
}
