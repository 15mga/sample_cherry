using Cherry.Model;

namespace Script.Model
{
    public class MUser : ModelBase
    {
        public string Token { get; private set; }
        public string Id { get; private set; }
        public string Password { get; private set; }

        public void SetToken(string token)
        {
            Token = token;
        }

        public void SetIdPw(string id, string pw)
        {
            Id = id;
            Password = pw;
        }
    }
}