namespace Milimoe.FunGame.Desktop.Library.Interface
{
    public interface ILogin
    {
        public bool LoginAccount(string username, string password);
        public bool CheckLogin(Guid key);
    }
}
