using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;

namespace weixin
{
    public class RichTextBoxExt : RichTextBox
    {
        #region 富文本Text

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            "Text", typeof(string), typeof(RichTextBoxExt), new PropertyMetadata(default(string), TextChangedCallback));

        private static void TextChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var richTextBox = (RichTextBoxExt)dependencyObject;

            var text = (string)dependencyPropertyChangedEventArgs.NewValue;
            var p = richTextBox.ConvertToElement(text);
            richTextBox.Blocks.Clear();
            richTextBox.Blocks.Add(p);
        }


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion
        
        //1、由于RichTextBox的Xaml属性不支持图片，所以没办法直接通过RichTextBox的Xaml属性直接处理
        //      这里通过构造XAML并使用XamlReader进行读取转换达到富文本的目的
        //      富文本包括：文本，图片，链接三种元素
        //          我们只需要分别对图片和链接进行处理就可以
        /// <summary>
        /// 将文字转为富文本（文字+图片表情+链接）
        /// </summary>
        public Paragraph ConvertToElement(string input)
        {
            if (input == null)
            {
                return new Paragraph();
            }
            //匹配普通链接（遇到空格或非Ascii字符则停止）
            var mc = Regex.Matches(input, @"(www.[\x21-\x7e-[\s]]+|www.[\x21-\x7e-[\s]]+|www.[\x21-\x7e-[\s]]+$)|(http:\\/\\/[\x21-\x7e-[\s]]+|http:\\/\\/[\x21-\x7e-[\s]]+|http:\\/\\/[\x21-\x7e-[\s]]+$)|(https:\\/\\/[\x21-\x7e-[\s]]+|https:\\/\\/[\x21-\x7e-[\s]]+|https\\/\\/[\x21-\x7e-[\s]]+$)");

            //记录是否重复
            var matchs = new List<string>();

            foreach (Match m in mc)
            {
                if (matchs.Contains(m.Value))
                {
                    //如果有重复匹配项，则跳过
                    continue;
                }

                string temp;
                if (!m.Value.Contains("http"))
                   temp = "http://" + m.Value;
                else
                   temp = m.Value;
                temp = temp.Replace("\\/", "/");
                    //这里链接用蓝色显示，不加下划线（注意，这里使用系统的浏览器IE打开）
                    input = input.Replace(m.Value.Substring(0, m.Value.Length),
                        string.Format(@"<Hyperlink NavigateUri=""{0}"" MouseOverTextDecorations=""None"" MouseOverForeground=""Blue"" Foreground=""Blue"" TargetName=""_blank"" >{1}</Hyperlink>",
                            temp,m.Value));

                matchs.Add(m.Value);
            }
            matchs.Clear();


           
            StringBuilder builder = new StringBuilder();
               char [] array=input.ToCharArray();
                for(int i=0;i<array.Length;i++)
                {
                    byte[] emojiEncode = Encoding.Unicode.GetBytes(array[i].ToString());
                    StringBuilder sBuilder = new StringBuilder();
                    for (int k = emojiEncode.Length - 1; k >= 0; k--)
                    {
                        sBuilder.Append(emojiEncode[k].ToString("x2"));
                    }
                    string OXEmoji = sBuilder.ToString().ToUpper();
                    if (Global.emoji.ContainsKey(OXEmoji))
                    {
                        input = input.Replace(array[i].ToString(), string.Format(@"
                                                        <InlineUIContainer>
                                                            <Border>
                                                                <Image Source=""/design/emotions/{0}"" Width=""24"" Height=""24""/>
                                                            </Border>
                                                        </InlineUIContainer>
                                        ", Global.emoji[OXEmoji]));
                    }
                }

            if (input.Contains("\\/"))
            {
                foreach (var m in Global.weixin.Keys)
                {
                    if(input.Contains(m))
                    input = input.Replace("\\/"+m, string.Format(@"
                                <InlineUIContainer>
                                    <Border>
                                        <Image Source=""/design/emotions/{0}"" Width=""24"" Height=""24""/>
                                    </Border>
                                </InlineUIContainer>
                    ", Global.weixin[m] ));
                }
            }

            input = input.Replace("\\/","/");
            char line = '\n';
            char line1 = '\r';
            input = input.Replace("\\n", line.ToString());
            input = input.Replace("\\r", line1.ToString());
            var xaml = string.Format(@"<Paragraph 
                                        xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                        xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                                    <Paragraph.Inlines>
                                    <Run></Run>
                                      {0}
                                    </Paragraph.Inlines>
                                </Paragraph>", input);

            return (Paragraph)XamlReader.Load(xaml);
        }


    }
}
