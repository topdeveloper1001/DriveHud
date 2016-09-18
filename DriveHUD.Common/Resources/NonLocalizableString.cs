namespace DriveHUD.Common.Resources
{
    public class NonLocalizableString : ILocalizableString
    {
        public NonLocalizableString(string message, params object[] messageParams)
        {
            this.Message = message;
            this.MessageParams = messageParams;
        }

        public string Message
        {
            get;
            private set;
        }

        public object[] MessageParams
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return ToString(null);
        }

        public string ToString(System.Globalization.CultureInfo cultureInfo)
        {
            if (MessageParams == null || MessageParams.Length == 0)
            {
                return Message;
            }
            else
            {
                return string.Format(Message, MessageParams);
            }
        }
    }
}