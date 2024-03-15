using System;
using Cherry;
using Cherry.Ctrl;
using Pb;
using Script.Model;
using Script.State;

namespace Script.Ctrl
{
    public class CUser : CtrlBase<MUser>
    {
        public override void Initialize(Action onComplete = null)
        {
            onComplete?.Invoke();
        }

        private bool CheckIdPw(string id, string pw, Action<string> callback)
        {
            var idx1 = id.IndexOf("@", StringComparison.Ordinal);
            if (idx1 < 1 || idx1 > id.Length - 4)
            {
                callback("账号错误");
                return false;
            }
            var idx2 = id.IndexOf(".", StringComparison.Ordinal);
            if (idx2 < idx1 || idx1 > id.Length - 2)
            {
                callback("账号错误");
                return false;
            }

            if (pw.Length < 6)
            {
                callback("密码错误");
                return false;
            }

            callback("");
            return true;
        }

        public void SignIn(string id, string pw, Action<string> callback)
        {
            if (!CheckIdPw(id, pw, callback))
            {
                return;
            }
            Game.Ctrl.Get<CConn>().Request<SignInRes>(new SignInReq
            {
                Id = id,
                Password = pw
            }, code =>
            {
                switch (code)
                {
                    case 1:
                        callback("账号或密码错误");
                        break;
                    default:
                        callback("服务器维护中...");
                        break;
                }
            }, res =>
            {
                SetUserData(id, pw, res.Token);
            });
        }

        public void SignUp(string id, string pw, Action<string> callback)
        {
            if (!CheckIdPw(id, pw, callback))
            {
                return;
            }
            Game.Ctrl.Get<CConn>().Request<SignUpRes>(new SignUpReq
            {
                Id = id,
                Password = pw
            }, code =>
            {
                switch (code)
                {
                    case 1:
                        callback("账号错误");
                        break;
                    case 2:
                        callback("密码错误");
                        break;
                    case 3:
                        callback("账号已存在");
                        break;
                }
            }, res =>
            {
                SetUserData(id, pw, res.Token);
            });
        }

        private void SetUserData(string id, string pw, string token)
        {
            Model.SetToken(token);
            Model.SetIdPw(id, pw);
            Game.Fsm.Main.ChangeState<StatePlayer>();
        }
    }
}