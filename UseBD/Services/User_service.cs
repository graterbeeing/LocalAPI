namespace UseBD.Services
{
    public class User_service
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int Role { get; set; }

        // פונקציה לאיפוס הנתונים במקרה של יציאה מהמערכת
        public void ClearUser()
        {
            UserId = 0;
            UserName = string.Empty;
            Role = 0;
        }
    }
}
