using System.Web;

namespace Docller.Core.Common
{
    public class DocllerWebSession : IDocllerSession
    {
        private readonly HttpSessionStateBase _sessionStateBase;

        public DocllerWebSession(HttpSessionStateBase sessionStateBase)
        {
            _sessionStateBase = sessionStateBase;
        }

        #region Implementation of IDocllerSession

        public object this[string key]
        {
            get { return _sessionStateBase[key]; }
            set { _sessionStateBase[key] = value; }
        }

        public void Remove(string key)
        {
            this._sessionStateBase.Remove(key);
        }

        public void End()
        {
            this._sessionStateBase.Abandon();
        }

        #endregion
    }
}