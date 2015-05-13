using System.Data.Linq;

namespace weixin
{
   public class WeixinDataContext:DataContext
    {
       // 数据库链接字符串
        public static string DBConnectionString = "Data Source=isostore:/Weixin.sdf";

        // 传递数据库连接字符串到DataContext基类
        public WeixinDataContext(string connectionString)
            : base(connectionString)
        { }

        // 定义一个 消息表
        public Table<MessageTable> Messages;
        public Table<TalkMessageTable> TalkMessages;
    }
}
