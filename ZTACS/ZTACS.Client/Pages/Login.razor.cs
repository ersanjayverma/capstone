namespace ZTACS.Client.Pages
{
    public partial class Login
    {
        private string email =string.Empty;
        private string password = string.Empty;

        private void Login1()
        {
            Console.WriteLine($"Logging in: {email} / {password}");
        }


    }
}
