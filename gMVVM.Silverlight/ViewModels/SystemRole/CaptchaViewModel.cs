using mvvmCommon;
using System;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace gMVVM.ViewModels.SystemRole
{
    public class CaptchaViewModel : ViewModelBase
    {
        /// <summary>
        ///     Key for clientside reference
        /// </summary>
        private const string CAPTCHA_KEY = "SilverCaptcha";

        /// <summary>
        ///     Array
        /// </summary>
        private static readonly char[] _charArray = "ABCEFGHJKLMNPRSTUVWXYZ2346789".ToCharArray();

        /// <summary>
        ///     The captcha text
        /// </summary>
        private string captchaText = "";
        public string CaptchaText 
        {
            get
            {
                return this.captchaText;
            }
            set
            {
                this.captchaText = value;
                this.OnPropertyChanged("CaptchaText");
            }
        }

        /// <summary>
        ///     Constructor builds the captcha challenge
        /// </summary>
        public CaptchaViewModel()
        {
            //char[] captcha = new char[8];

            //Random random = new Random();

            //for (int x = 0; x < captcha.Length; x++)
            //{
            //    captcha[x] = _charArray[random.Next(_charArray.Length)];
            //}

            CaptchaText = CreateCaptcha();

            //HtmlPage.RegisterScriptableObject(CAPTCHA_KEY, this);
        }

        public string CreateCaptcha()
        {
            char[] captcha = new char[8];

            Random random = new Random();

            for (int x = 0; x < captcha.Length; x++)
            {
                captcha[x] = _charArray[random.Next(_charArray.Length)];
            }
            return new string(captcha);
        }

        public void CreatNewCaptcha()
        {
            CaptchaText = CreateCaptcha();
        }

        /// <summary>
        ///     Returns true if, based on the response, the user appears to be human
        /// </summary>
        /// <param name="challengeResponse">The reponse to the captcha challenge</param>
        /// <returns>True if there is a match</returns>
        //[ScriptableMember]
        //public bool IsHuman(string challengeResponse)
        //{
        //    return challengeResponse.Trim().ToUpper().Equals(CaptchaText);
        //}
    }
}
