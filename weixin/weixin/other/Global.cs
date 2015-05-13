using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weixin
{
    public static class Global
    {
        public static List<string> failedFakeId;
        public static List<string> failedReferer;
        public static string tempSize;
        public static string returnUrl;
        public static bool send;
        public static int isFirstSetupOrUpdate;
        public static int launchTimes;
        public static string task;
        public static string jpgName;
        public static string newAddMessage;
        public static int hasVCode;
        public static int isDel;
        public static int cnt;
        public static double perPerson;
        public static bool test;
        public static bool isFirstLoad;
        public static double pb;
        public static int isFirstLaunch;
        public static bool homeOK;
        public static List<PersonMessage> allInfo;
        public static string tofakeid;
        public static int showCount;
        public static bool getStarMessage;
        public static int personMessageShouldRefresh;
        public static bool hasNewPeople;
        public static Dictionary<string,string> groupsInfo;
        public static bool isFirstPrepare;
        public static void StoErr(string form,Exception err)
        {
            if (err.Message.Contains("The remote server "))
                return;
            string vertion="";
            if (Sto.Info.Contains("Vertion"))
                vertion = Sto.Info["Vertion"].ToString() ;
            if (Sto.Info.Contains("Err"))
            {
                Sto.Info["Err"] = Sto.Info["Err"].ToString() +"V"+vertion+'\n'+ DateTime.Now.ToString() + '\n' + form + '\n' +"内部错误信息："+err.InnerException+'\n'+"异常提示消息："+err.Message+'\n'+"错误源："+err.Source+'\n'+"所在堆栈："+err.StackTrace + '\n' + '\n';
                Sto.Info.Save();
            }
            else
            {
                Sto.stoInfo<string>("Err", "问题日志：" + '\n' + "V" + vertion + '\n' + DateTime.Now.ToString() + '\n' + form + '\n' + "内部错误信息：" + err.InnerException + '\n' + "异常提示消息：" + err.Message + '\n' + "错误源：" + err.Source + '\n' + "所在堆栈：" + err.StackTrace  + '\n' + '\n');
            }
      
           Sto.stoInfo<int>("hasErr",1);
        }
        public static void DelErr()
        {
            Sto.stoInfo<string>("Err", "问题日志：");
            Sto.stoInfo<int>("hasErr", 0);
        }
        public static Dictionary<string, string> emoji;
        public static Dictionary<string, string> weixin;
    }
}
