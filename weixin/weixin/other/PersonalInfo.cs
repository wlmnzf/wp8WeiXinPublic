using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace weixin
{
    public static class loginInfo//保存登陆后返回的信息
    {
        /// <summary>
        /// 登录后得到的令牌
        /// </summary>        
        public static string Token { get; set; }
        /// <summary>
        /// 登录后得到的cookie
        /// </summary>
        public static CookieContainer LoginCookie { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public static DateTime CreateDate { get; set; }

        public static string Err { get; set; }

        public static string Fakeid { get; set; }

        public static string NickName {get;set; }

        public static string UserName { get; set; }

        public static string Type { get; set; }

        public static string HasStar { get; set; }

        public static string Seq { get; set; }

        public static string Ticket { get; set; }

        public static string UniformUserName { get; set; }
    }

}
