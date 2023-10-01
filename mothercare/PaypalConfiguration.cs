using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mothercare
{
    public class PaypalConfiguration
    {
        public readonly static string clientId;
        public readonly static string clientSecret;

        static PaypalConfiguration()
        {
            var config = getconfig();
            clientId = "AbB68jpdBgT7fPiP_zXayOcGPhLXdpHSypEhcRqrq7nOE-29RqMdD1wBXAUs6Nmv2d7ZFrMDzl8UG1Nx";
            clientSecret = "EIaZfbJN3ycNUSM5a7bmU8LXrDayqQ0dCIcOhi_Sm14OH2n0XSwnM2JYGmi9wuR8uc0YQXcaeNdPdyL6";

        }

        private static Dictionary<string, string> getconfig()
        {
            return PayPal.Api.ConfigManager.Instance.GetProperties();
        }

        private static string GetAccessToken()
        {
            string accessToken = new OAuthTokenCredential(clientId, clientSecret, getconfig()).GetAccessToken();
            return accessToken;
        }

        public static APIContext GetAPIContext()
        {
            APIContext apicontext = new APIContext(GetAccessToken());
            apicontext.Config = getconfig();
            return apicontext;
        }
    }
}