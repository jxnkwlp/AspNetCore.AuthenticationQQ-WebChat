using Newtonsoft.Json.Linq;

namespace Microsoft.AspNetCore.Authentication.QQ
{
    class QQAuthenticationHelper
    {
        public static string GetNickname(JObject user) => user.Value<string>("nickname");

        public static string GetFigureUrl(JObject user) => user.Value<string>("figureurl");

        public static string GetFigureUrl_1(JObject user) => user.Value<string>("figureurl_1");

        public static string GetFigureUrl_2(JObject user) => user.Value<string>("figureurl_2");

        public static string GetFigureUrl_QQ_1(JObject user) => user.Value<string>("figureurl_qq_1");

        public static string GetFigureUrl_QQ_2(JObject user) => user.Value<string>("figureurl_qq_2");

        public static string GetGender(JObject user) => user.Value<string>("gender");

        public static string GetIsYellowVip(JObject user) => user.Value<string>("is_yellow_vip");

        public static string GetIsVip(JObject user) => user.Value<string>("vip");

        public static string GetYellowVipLevel(JObject user) => user.Value<string>("yellow_vip_level");

        public static string GetLevel(JObject user) => user.Value<string>("level");

        public static string GetIsYellowYearVip(JObject user) => user.Value<string>("is_yellow_year_vip");

    }
}
