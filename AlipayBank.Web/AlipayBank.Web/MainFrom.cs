using AlipayBank.Web.utils;
using CsharpHttpHelper;
using Gecko;
using Gecko.Events;
using Gecko.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebSocketClient;
using ZXing;
using HtmlAgilityPack;
using System.IO;

namespace AlipayBank.Web
{
    public class MainFrom : Form
    {
        public class UserBean
        {
            public string id
            {
                get;
                set;
            }

            public string userid
            {
                get;
                set;
            }

            public string username
            {
                get;
                set;
            }

            public string typec
            {
                get;
                set;
            }

            public string sdk
            {
                get;
                set;
            }
        }

        public class orderModel
        {
            public class order
            {
                public string tradeAmount
                {
                    get;
                    set;
                }

                public string tradeNo
                {
                    get;
                    set;
                }

                public string tradeRemark
                {
                    get;
                    set;
                }

                public string tradeTime
                {
                    get;
                    set;
                }

                public string landid
                {
                    get;
                    set;
                }
            }

            public string Json
            {
                get;
                set;
            }

            public string type
            {
                get;
                set;
            }
        }

        public class qrModel2
        {
            public class qrChild2
            {
                public int typec
                {
                    get;
                    set;
                }

                public int state
                {
                    get;
                    set;
                }

                public string qrcode
                {
                    get;
                    set;
                }

                public string url
                {
                    get;
                    set;
                }

                public int land_id
                {
                    get;
                    set;
                }

                public string userid
                {
                    get;
                    set;
                }

                public string money
                {
                    get;
                    set;
                }

                public string money_res
                {
                    get;
                    set;
                }

                public string mark
                {
                    get;
                    set;
                }

                public string info
                {
                    get;
                    set;
                }

                public long create_time
                {
                    get;
                    set;
                }

                public string bank
                {
                    get;
                    set;
                }

                public string bank_name
                {
                    get;
                    set;
                }

                public string h5_link
                {
                    get;
                    set;
                }
            }

            public string type
            {
                get;
                set;
            }

            public qrChild2 data
            {
                get;
                set;
            }
        }

        public class GDI32
        {
            public const int SRCCOPY = 13369376;

            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);

            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);

            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        public class User32
        {
            public struct RECT
            {
                public int left;

                public int top;

                public int right;

                public int bottom;
            }

            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        }

        private IList<UserBean> infoList;

        private UserBean userinfo;

        private WSocketClient client;

        private Dictionary<string, string> dic = new Dictionary<string, string>();

        private HttpHelper http = new HttpHelper();

        private GeckoWebBrowser gecko;

        private GeckoWebBrowser gecko1;

        private bool isLogin;

        private string mCookies = "";

        private List<string> OrderListOk = new List<string>();

        private qrModel2 model;

        private string money = "1";

        private string instIdIndex = "fastPay-boc102";

        private List<string> TaskList = new List<string>();

        private bool IsStart;

        private int startIndex;

        private string orderNo = "";

        private IContainer components;

        private Panel panel1;

        private GroupBox groupBox2;

        private Button button6;

        private Button button1;

        private Button 绑定用户;

        private Label label1;

        private ComboBox comboBox1;

        private Label label2;

        private TextBox txtUserName;

        private Label label3;

        private Button btnAppLogin;

        private TextBox txtUserPwd;

        private GroupBox groupBox1;

        private Button button2;

        private Button button5;

        private ListBox lbLog;

        private Button button3;

        private Panel panel2;

        private Button button4;
        private RadioButton radioButton2;
        private RadioButton radioButton1;
        private PictureBox pictureBox1;
        string str = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        private void WriteLog(string msg)
        {
            if (lbLog.InvokeRequired)
            {
                lbLog.Invoke((Action)delegate
                {
                    lbLog.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + msg);
                    LogToFile(msg);
                });
            }
            else
            {
                lbLog.Items.Insert(0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + msg);
                LogToFile(msg);
            }
        }

        /// <summary>
        ///     写入文本日志
        /// </summary>
        /// <param name="msg"></param>
        public static void LogToFile(string msg)
        {
            var dir = Thread.GetDomain().BaseDirectory + "Log";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            var filePath = Path.Combine(dir, DateTime.Now.ToString("yyyy-MM-dd") + "_log.txt");
            using (var stream = new FileStream(filePath, FileMode.Append))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " " + msg);
                }
            }
        }

        public void xintiao()
        {
            Task.Run(delegate
            {
                int num = 0;
                while (true)
                {
                    if (client == null)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        string text = "{\"State\":\"1\",\"landId\":\"" + userinfo.id + "\",\"type\":\"" + userinfo.typec + "\"}";
                        client.SendMessage(text);
                        WriteLog("发送数据：" + text);
                        num++;
                        Thread.Sleep(20000);
                    }
                }
            });
        }

        public string GetTimeStamp()
        {
            return Convert.ToInt64((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds).ToString();
        }

        public string GetOrderId()
        {
            HttpItem item = new HttpItem
            {
                URL = "https://bizfundprod.alipay.com/allocation/deposit/index.htm",
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36",
                Method = "GET",
                Cookie = mCookies
            };
            HttpResult html = http.GetHtml(item);
            string redirectUrl = html.RedirectUrl;
            if (redirectUrl.Contains("login"))
            {
                WriteLog("掉线咯1");
                绑定用户.Enabled = true;
                client.Dispose();
                return "";
            }
            item = new HttpItem
            {
                URL = redirectUrl,
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36",
                Method = "GET",
                Cookie = mCookies
            };
            html = http.GetHtml(item);
            item = new HttpItem
            {
                URL = html.RedirectUrl,
                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36",
                Method = "GET",
                Referer = "https://personalweb.alipay.com/portal/i.htm",
                Cookie = mCookies
            };
            html = http.GetHtml(item);
            if (html.RedirectUrl == "")
            {
                return html.ResponseUri;
            }
            return html.RedirectUrl;
        }

        public void InjectJs(GeckoDomDocument doc, string jsStr)
        {
            GeckoHtmlElement geckoHtmlElement = doc.CreateHtmlElement("script");
            geckoHtmlElement.InnerHtml = jsStr;
            gecko.Document.Head.AppendChild(geckoHtmlElement);
        }

        public MainFrom()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            WriteLog("正在初始化");
            Xpcom.Initialize("Firefox");
            gecko = new GeckoWebBrowser();
            gecko.CreateControl();
            gecko.ShowModal();
            gecko.NoDefaultContextMenu = false;
            gecko.Dock = DockStyle.Fill;
            gecko.DocumentCompleted += Gecko_DocumentCompleted;
            gecko.CreateWindow += Gecko_CreateNewWindow;
            gecko.UseHttpActivityObserver = true;
            string url = "https://b.alipay.com/";
            gecko.Navigate(url);
            panel1.Controls.Add(gecko);
            MyObserver myObserver = new MyObserver();
            myObserver.TicketLoadedEvent += MyObs_TicketLoadedEvent;
            ObserverService.AddObserver(myObserver);
            xintiao();
            Task.Run(delegate
            {
                StartTask();
            });
            gecko1 = new GeckoWebBrowser();
            gecko1.CreateControl();
            gecko1.ShowModal();
            gecko1.NoDefaultContextMenu = false;
            gecko1.Dock = DockStyle.Fill;
            gecko1.DocumentCompleted += Gecko_DocumentCompleted;
            gecko1.Navigating += Navigating;
            gecko1.Navigated += Navigat;
            gecko1.UseHttpActivityObserver = true;
            gecko1.Navigate(url);
            panel2.Controls.Add(gecko1);
            dic.Add("工商银行", "fastPay-icbc105");
            dic.Add("平安银行", "fastPay-spabanknucc103");
            dic.Add("交通银行", "fastPay-commnucc103");
            dic.Add("农业银行", "fastPay-abc101");
            dic.Add("北京银行", "fastPay-bjbanknucc103");
            dic.Add("北京农商银行", "fastPay-bjrcbnucc103");
            dic.Add("中国银行", "fastPay-boc102");
            dic.Add("中国建设银行", "fastPay-ccb103");
            dic.Add("成都银行", "fastPay-cdcbnucc103");
            dic.Add("中国光大银行", "fastPay-cebnucc103");
            dic.Add("兴业银行", "fastPay-cib102");
            dic.Add("中信银行", "fastPay-citicnucc103");
            dic.Add("招商银行", "fastPay-cmb103");
            dic.Add("常熟农商银行", "fastPay-csrcbnucc103");
            dic.Add("富滇银行", "fastPay-fdbnucc103");
            dic.Add("广发银行", "fastPay-gdbnucc103");
            dic.Add("华夏银行", "fastPay-hxbanknucc103");
            dic.Add("杭州银行", "fastPay-hzcbnucc103");
            dic.Add("宁波银行", "fastPay-nbbanknucc103");
            dic.Add("南京银行", "fastPay-njcbnucc103");
            dic.Add("中国邮政储蓄银行", "fastPay-psbcnucc103");
            dic.Add("上海银行", "fastPay-shbanknucc103");
            dic.Add("上海农商行", "fastPay-shrcbnucc103");
            dic.Add("浦发银行", "fastPay-spdbnucc103");
            dic.Add("吴江农村商业银行", "fastPay-wjrcbnucc103");
            dic.Add("温州银行", "fastPay-wzcbnucc103");
        }

        private void Gecko_CreateNewWindow(object sender, GeckoCreateWindowEventArgs e)
        {
            WriteLog("Gecko_CreateNewWindow == " + e.Uri.ToString());
            e.Cancel = true;
        }

        public static int ConvertDateTimeInt(DateTime time)
        {
            DateTime d = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (int)(time - d).TotalSeconds;
        }

        private void Navigat(object sender, GeckoNavigatedEventArgs e)
        {
        }

        private void Navigating(object sender, GeckoNavigatingEventArgs e)
        {
        }

        public void Gecko_DocumentCompleted(object sender, GeckoDocumentCompletedEventArgs e)
        {
            try
            {
                if (e.Uri.ToString() == "https://pceuser.netpay.cmbchina.com/pc-card-epcc/PrePayC.do")
                {
                    Task.Run(delegate
                    {
                        Thread.Sleep(1000);
                        string qrcode2 = "";
                        while (qrcode2 == "")
                        {
                            Invoke((Action)delegate
                            {
                                Image image2 = CaptureWindow(base.Handle);
                                pictureBox1.Image = image2;
                                qrcode2 = Read1(image2);
                                WriteLog("招商二维码链接：" + qrcode2);
                                if (qrcode2 != "")
                                {
                                    IsStart = false;
                                    startIndex = 0;
                                    model.data.h5_link = qrcode2;
                                    model.data.state = 3;
                                    client.SendMessage(JsonConvert.SerializeObject(model));
                                    gecko.Navigate("https://custweb.alipay.com/account/index.htm");
                                }
                            });
                            Thread.Sleep(10000);
                        }
                    });
                }
                if (e.Uri.ToString() == "https://ibsbjstar.ccb.com.cn/CCBIS/CCBWLReqServlet")
                {
                    Task.Run(delegate
                    {
                        Thread.Sleep(1000);
                        string qrcode = "";
                        while (qrcode == "")
                        {
                            Invoke((Action)delegate
                            {
                                Image image = CaptureWindow2(base.Handle);
                                pictureBox1.Image = image;
                                qrcode = Read1(image);
                                WriteLog("建设二维码链接：" + qrcode);
                                if (qrcode != "")
                                {
                                    startIndex = 0;
                                    IsStart = false;
                                    model.data.h5_link = qrcode;
                                    model.data.state = 3;
                                    client.SendMessage(JsonConvert.SerializeObject(model));
                                    gecko.Navigate("https://custweb.alipay.com/account/index.htm");
                                }
                            });
                            Thread.Sleep(1000);
                        }
                    });
                }
                foreach (GeckoHtmlElement link in gecko.Document.Links)
                {
                    link.SetAttribute("target", "_self");
                }
                foreach (GeckoHtmlElement form in gecko.Document.Forms)
                {
                    form.SetAttribute("target", "_self");
                }
                if (e.Uri.ToString().Contains("MerPayB2C"))
                {
                    gecko.Navigate("https://shanghu.alipay.com/i.htm");
                }
                else if (e.Uri.ToString().Contains("entError"))
                {
                    model.data.url = "生成失败";
                    model.data.qrcode = "生成失败";
                    client.SendMessage(JsonConvert.SerializeObject(model));
                }
                else
                {
                    if (e.Uri.ToString() == "about:blank")
                    {
                        return;
                    }
                    if (e.Uri.ToString().Contains("login"))
                    {
                        WriteLog("掉线了");
                        绑定用户.Enabled = true;
                        client.Dispose();
                        return;
                    }
                    if (e.Uri.ToString().Contains("account/index.htm"))
                    {
                        string input = gecko.Document.Body.InnerHtml.ToString();
                        if (new Regex("<th>淘宝会员名</th>\\s+<td>(?<name>.*?)</td>", RegexOptions.Multiline).Match(input).Groups["name"].Value == "未**户")
                        {
                            WriteLog("请先确认淘宝验证，否则很容易掉线");
                            WriteLog("请先确认淘宝验证，否则很容易掉线");
                            WriteLog("请先确认淘宝验证，否则很容易掉线");
                        }
                    }
                    if (e.Uri.ToString().Contains("home.htm"))
                    {
                        gecko.Navigate("https://custweb.alipay.com/account/index.htm");
                        if (!isLogin)
                        {
                            Task.Run(delegate
                            {
                                int num3 = 0;
                                bool flag = false;
                                while (isLogin)
                                {
                                    if (num3 % 20 == 0)
                                    {
                                        if (flag)
                                        {
                                            flag = false;
                                            gecko1.Invoke((Action)delegate
                                            {
                                                WriteLog("调整商户");
                                                if (radioButton1.Checked == true)
                                                {
                                                    gecko1.Navigate("https://shanghu.alipay.com/i.htm");
                                                    WriteLog("回调1");
                                                }
                                                else
                                                {
                                                    gecko1.Navigate("https://mrchportalweb.alipay.com/user/home.htm");
                                                    //https://personalweb.alipay.com/portal/i.htm
                                                    WriteLog("回调2");
                                                }
                                            });
                                        }
                                        else
                                        {
                                            flag = true;
                                            gecko1.Invoke((Action)delegate
                                            {
                                                if (radioButton1.Checked == true)
                                                {
                                                    gecko1.Navigate("https://mbillexprod.alipay.com/enterprise/fundAccountDetail.htm");//账单
                                                }
                                                else
                                                {
                                                    gecko1.Navigate("https://consumeprod.alipay.com/record/advanced.htm");//账单
                                                }
                                            });
                                        }
                                    }
                                    num3++;
                                    Thread.Sleep(1000);
                                }
                            });
                        }
                        isLogin = true;
                    }
                    if (e.Uri.ToString().Contains("fundAccountDetail"))
                    {
                        Task.Run(delegate
                        {
                            Thread.Sleep(3000);
                            gecko1.Invoke((Action)delegate
                            {
                                string innerHtml = gecko1.Document.Body.InnerHtml;
                                Regex regex = new Regex("block;\">(?<date>.*?)</span><span style=\"display: block;\">(?<time>.*?)</span>", RegexOptions.Multiline);
                                Regex regex2 = new Regex("<span data-clipboard-text=\"(?<orderNo>.*?)\" tit", RegexOptions.Multiline);
                                Regex regex3 = new Regex("<span class=\"\">(?<money>.*?)</span>", RegexOptions.Multiline);
                                MatchCollection matchCollection = regex.Matches(innerHtml);
                                MatchCollection matchCollection2 = regex2.Matches(innerHtml);
                                MatchCollection matchCollection3 = regex3.Matches(innerHtml);
                                if (matchCollection.Count > 0)
                                {
                                    WriteLog("Count:" + matchCollection.Count);
                                    for (int i = 0; i < matchCollection.Count; i++)
                                    {
                                        int i2 = i * 2 + 1;
                                        if (i == 0)
                                        {
                                            i2 = 1;
                                        }
                                        if (matchCollection.Count == matchCollection2.Count)
                                        {
                                            i2 = i;
                                        }
                                        string value = matchCollection[i].Groups["time"].Value;
                                        WriteLog("value1:" + value);
                                        string value2 = matchCollection2[i2].Groups["orderNo"].Value;
                                        WriteLog("value2:" + value2);
                                        string text = matchCollection3[i].Groups["money"].Value.Replace("+", "");
                                        WriteLog("value3:" + text);
                                        if (!text.Contains("-") && !OrderListOk.Contains(value2) && userinfo != null && userinfo.userid != null)
                                        {
                                            OrderListOk.Add(value2);
                                            long num = ConvertDateTimeToInt(DateTime.Now.ToString("yyyy-MM-dd ") + value);
                                            orderModel.order order = new orderModel.order();
                                            orderModel orderModel = new orderModel();
                                            order.tradeAmount = text;
                                            order.tradeNo = value2;
                                            order.tradeTime = string.Concat(num);
                                            order.landid = (userinfo.id ?? "");
                                            orderModel.Json = JsonConvert.SerializeObject(order);
                                            orderModel.type = "YeOrderList";
                                            client.SendMessage(JsonConvert.SerializeObject(orderModel));
                                            WriteLog("订单回调：" + JsonConvert.SerializeObject(orderModel));
                                        }
                                    }
                                }
                                Regex regex4 = new Regex("w-level-0\"><td style=\"text-align: left;\" class=\"ant-table-column-has-actions ant-table-column-has-sorters ant-table-row-cell-break-word\"><span><span>(?<money>.*?)</span></span>", RegexOptions.Multiline);
                                Regex regex5 = new Regex("<span data-clipboard-text=\"(?<orderNo>.*?)\" tit", RegexOptions.Multiline);
                                Regex regex6 = new Regex("</span></span></td><td style=\"text-align: left;\" class=\"ant-table-row-cell-break-word\"><span><span>(?<money>.*?)</span></span>");
                                MatchCollection matchCollection4 = regex4.Matches(innerHtml);
                                MatchCollection matchCollection5 = regex5.Matches(innerHtml);
                                MatchCollection matchCollection6 = regex6.Matches(innerHtml);
                                if (matchCollection.Count <= 0 && matchCollection4.Count > 0)
                                {
                                    for (int j = 0; j < matchCollection4.Count; j++)
                                    {
                                        int i3 = j * 2 + 1;
                                        if (j == 0)
                                        {
                                            i3 = 1;
                                        }
                                        int i4 = j * 2;
                                        if (matchCollection4.Count == matchCollection5.Count)
                                        {
                                            i3 = j;
                                        }
                                        string value3 = matchCollection4[j].Groups["money"].Value;
                                        string value4 = matchCollection5[i3].Groups["orderNo"].Value;
                                        string text2 = matchCollection6[i4].Groups["money"].Value.Replace("+", "");
                                        if (!text2.Contains("-") && !OrderListOk.Contains(value4) && userinfo != null && userinfo.userid != null)
                                        {
                                            OrderListOk.Add(value4);
                                            long num2 = ConvertDateTimeToInt(value3);
                                            orderModel.order order2 = new orderModel.order();
                                            orderModel orderModel2 = new orderModel();
                                            order2.tradeAmount = text2;
                                            order2.tradeNo = value4;
                                            order2.tradeTime = string.Concat(num2);
                                            order2.landid = (userinfo.id ?? "");
                                            orderModel2.Json = JsonConvert.SerializeObject(order2);
                                            orderModel2.type = "YeOrderList";
                                            client.SendMessage(JsonConvert.SerializeObject(orderModel2));
                                            WriteLog("订单回调：" + JsonConvert.SerializeObject(orderModel2));
                                        }
                                    }
                                }
                            });
                        });
                    }
                    if (e.Uri.ToString().Contains("advanced"))
                    {
                        Task.Run(delegate
                        {
                            Thread.Sleep(3000);
                            gecko1.Invoke((Action)delegate
                            {
                                string innerHtml = gecko1.Document.Body.InnerHtml;

                                Regex reg = new Regex(@"\s+", RegexOptions.Compiled);
                                Regex reg1 = new Regex(@"(.*流水号:)", RegexOptions.Compiled);
                                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                                doc.LoadHtml(innerHtml);
                                doc.OptionOutputAsXml = true;
                                HtmlNode node = doc.DocumentNode.SelectSingleNode(".//*[@id=\"tradeRecordsIndex\"]/tbody");
                                HtmlNodeCollection trNodeList = node.SelectNodes("tr[td]");

                                foreach (HtmlNode item in trNodeList)
                                {
                                    HtmlNodeCollection tdNodeList1 = item.SelectNodes("td");
                                    //0 3 5
                                    //MessageBox.Show(reg.Replace(tdNodeList1[0].InnerText, " "));

                                    string value = reg.Replace(tdNodeList1[0].InnerText, " ");
                                    string value2 = reg1.Replace(tdNodeList1[3].InnerText, "");
                                    string text = reg.Replace(tdNodeList1[5].InnerText.Replace("+", ""), "");
                                    if (!text.Contains("-") && !OrderListOk.Contains(value2) && userinfo != null && userinfo.userid != null)
                                    {
                                        OrderListOk.Add(value2);
                                        long num = ConvertDateTimeToInt(DateTime.Now.ToString("yyyy-MM-dd ") + value);
                                        orderModel.order order = new orderModel.order();
                                        orderModel orderModel = new orderModel();
                                        order.tradeAmount = text;
                                        order.tradeNo = value2;
                                        order.tradeTime = string.Concat(num);
                                        order.landid = (userinfo.id ?? "");
                                        orderModel.Json = JsonConvert.SerializeObject(order);
                                        orderModel.type = "YeOrderList";
                                        client.SendMessage(JsonConvert.SerializeObject(orderModel));
                                        WriteLog("订单回调-个人：" + JsonConvert.SerializeObject(orderModel));
                                    }
                                }

                                #region
                                //Regex regex = new Regex("block;\">(?<date>.*?)</span><span style=\"display: block;\">(?<time>.*?)</span>", RegexOptions.Multiline);
                                //Regex regex2 = new Regex("<span data-clipboard-text=\"(?<orderNo>.*?)\" tit", RegexOptions.Multiline);
                                //Regex regex3 = new Regex("<span class=\"\">(?<money>.*?)</span>", RegexOptions.Multiline);
                                //MatchCollection matchCollection = regex.Matches(innerHtml);
                                //MatchCollection matchCollection2 = regex2.Matches(innerHtml);
                                //MatchCollection matchCollection3 = regex3.Matches(innerHtml);
                                //if (matchCollection.Count > 0)
                                //{
                                //    for (int i = 0; i < matchCollection.Count; i++)
                                //    {
                                //        int i2 = i * 2 + 1;
                                //        if (i == 0)
                                //        {
                                //            i2 = 1;
                                //        }
                                //        if (matchCollection.Count == matchCollection2.Count)
                                //        {
                                //            i2 = i;
                                //        }
                                //        string value = matchCollection[i].Groups["time"].Value;
                                //        string value2 = matchCollection2[i2].Groups["orderNo"].Value;//2020071322001496221426787956
                                //        string text = matchCollection3[i].Groups["money"].Value.Replace("+", "");
                                //        if (!text.Contains("-") && !OrderListOk.Contains(value2) && userinfo != null && userinfo.userid != null)
                                //        {
                                //            OrderListOk.Add(value2);
                                //            long num = ConvertDateTimeToInt(DateTime.Now.ToString("yyyy-MM-dd ") + value);
                                //            orderModel.order order = new orderModel.order();
                                //            orderModel orderModel = new orderModel();
                                //            order.tradeAmount = text;
                                //            order.tradeNo = value2;
                                //            order.tradeTime = string.Concat(num);
                                //            order.landid = (userinfo.id ?? "");
                                //            orderModel.Json = JsonConvert.SerializeObject(order);
                                //            orderModel.type = "YeOrderList";
                                //            client.SendMessage(JsonConvert.SerializeObject(orderModel));
                                //            WriteLog("订单回调：" + JsonConvert.SerializeObject(orderModel));
                                //        }
                                //    }
                                //}
                                //Regex regex4 = new Regex("w-level-0\"><td style=\"text-align: left;\" class=\"ant-table-column-has-actions ant-table-column-has-sorters ant-table-row-cell-break-word\"><span><span>(?<money>.*?)</span></span>", RegexOptions.Multiline);
                                //Regex regex5 = new Regex("<span data-clipboard-text=\"(?<orderNo>.*?)\" tit", RegexOptions.Multiline);
                                //Regex regex6 = new Regex("</span></span></td><td style=\"text-align: left;\" class=\"ant-table-row-cell-break-word\"><span><span>(?<money>.*?)</span></span>");
                                //MatchCollection matchCollection4 = regex4.Matches(innerHtml);
                                //MatchCollection matchCollection5 = regex5.Matches(innerHtml);
                                //MatchCollection matchCollection6 = regex6.Matches(innerHtml);
                                //if (matchCollection.Count <= 0 && matchCollection4.Count > 0)
                                //{
                                //    for (int j = 0; j < matchCollection4.Count; j++)
                                //    {
                                //        int i3 = j * 2 + 1;
                                //        if (j == 0)
                                //        {
                                //            i3 = 1;
                                //        }
                                //        int i4 = j * 2;
                                //        if (matchCollection4.Count == matchCollection5.Count)
                                //        {
                                //            i3 = j;
                                //        }
                                //        string value3 = matchCollection4[j].Groups["money"].Value;
                                //        string value4 = matchCollection5[i3].Groups["orderNo"].Value;
                                //        string text2 = matchCollection6[i4].Groups["money"].Value.Replace("+", "");
                                //        if (!text2.Contains("-") && !OrderListOk.Contains(value4) && userinfo != null && userinfo.userid != null)
                                //        {
                                //            OrderListOk.Add(value4);
                                //            long num2 = ConvertDateTimeToInt(value3);
                                //            orderModel.order order2 = new orderModel.order();
                                //            orderModel orderModel2 = new orderModel();
                                //            order2.tradeAmount = text2;
                                //            order2.tradeNo = value4;
                                //            order2.tradeTime = string.Concat(num2);
                                //            order2.landid = (userinfo.id ?? "");
                                //            orderModel2.Json = JsonConvert.SerializeObject(order2);
                                //            orderModel2.type = "YeOrderList";
                                //            client.SendMessage(JsonConvert.SerializeObject(orderModel2));
                                //            WriteLog("订单回调：" + JsonConvert.SerializeObject(orderModel2));
                                //        }
                                //    }
                                //}
                                #endregion
                            });
                        });
                    }
                    if (e.Uri.ToString().Contains("templateFlow.htm"))
                    {
                        Task.Run(delegate
                        {
                            string jsStr8 = "";
                            jsStr8 += "function dragandDrop(id, clientX, clientY, distance){var elem = document.getElementById(id),k = 0,interval;iME(elem, \"mousedown\", 0, 0, clientX, clientY);interval = setInterval(function() {k++;iter(k);if (k === distance){clearInterval(interval);iME(elem, \"mouseup\", clientX + k, clientY, 220 + k, 400);}}, 10);function iter(y){iME(elem, \"mousemove\", clientX + y, clientY, clientX + y, clientY);}function iME(obj,event,screenXArg,screenYArg,clientXArg,clientYArg){var mousemove = document.createEvent(\"MouseEvent\");mousemove.initMouseEvent(event, true, true, window, 0, screenXArg, screenYArg, clientXArg, clientYArg, 0, 0, 0, 0, 0, null);obj.dispatchEvent(mousemove);}}window.setTimeout(function() {                        var obj = document.getElementById(\"J_amountInput\"); obj.target = '_self';var _owh = obj.getBoundingClientRect();var _ox = _owh.width / 2, _oh = _owh.height / 2;_ox = Math.floor(Math.random() * _ox+60); _oh = Math.floor(Math.random() * _oh+60);_ox=_ox+_owh.x;_oh=_oh+_owh.y;dragandDrop(\"J_amountInput\", _ox, _oh,50);}, 100);";
                            Invoke((Action)delegate
                            {
                                InjectJs(gecko.Document, jsStr8);
                            });
                            Thread.Sleep(100);
                            string jsStr9 = "document.getElementById(\"" + instIdIndex + "\").click();";
                            Invoke((Action)delegate
                            {
                                InjectJs(gecko.Document, jsStr9);
                            });
                            Thread.Sleep(100);
                            string jsStr10 = "";
                            jsStr10 = jsStr10 + "document.getElementById('J_amountInput').value = '" + money + "';";
                            jsStr10 += "document.getElementById('J_submit').click();";
                            jsStr10 += "document.getElementById('J_submit').click();";
                            Invoke((Action)delegate
                            {
                                InjectJs(gecko.Document, jsStr10);
                            });
                        });
                    }
                    if (!e.Uri.ToString().Contains("ebankDeposit.htm"))
                    {
                        return;
                    }
                    new Thread((ThreadStart)delegate
                    {
                        string jsStr5 = "";
                        jsStr5 += "function dragandDrop(id, clientX, clientY, distance){var elem = document.getElementById(id),k = 0,interval;iME(elem, \"mousedown\", 0, 0, clientX, clientY);interval = setInterval(function() {k++;iter(k);if (k === distance){clearInterval(interval);iME(elem, \"mouseup\", clientX + k, clientY, 220 + k, 400);}}, 10);function iter(y){iME(elem, \"mousemove\", clientX + y, clientY, clientX + y, clientY);}function iME(obj,event,screenXArg,screenYArg,clientXArg,clientYArg){var mousemove = document.createEvent(\"MouseEvent\");mousemove.initMouseEvent(event, true, true, window, 0, screenXArg, screenYArg, clientXArg, clientYArg, 0, 0, 0, 0, 0, null);obj.dispatchEvent(mousemove);}}window.setTimeout(function() {                        var obj = document.getElementById(\"J-depositAmount\"); obj.target = '_self';var _owh = obj.getBoundingClientRect();var _ox = _owh.width / 2, _oh = _owh.height / 2;_ox = Math.floor(Math.random() * _ox+60); _oh = Math.floor(Math.random() * _oh+60);_ox=_ox+_owh.x;_oh=_oh+_owh.y;dragandDrop(\"J-depositAmount\", _ox, _oh,50);}, 100);";
                        Invoke((Action)delegate
                        {
                            InjectJs(gecko.Document, jsStr5);
                        });
                        Thread.Sleep(100);
                        Invoke((Action)delegate
                        {
                            string str = "";
                            str += "var e = document.createEvent(\"MouseEvents\");";
                            str += "e.initEvent(\"click\", true, true);";
                            str += "document.getElementById(\"J-depositAmount\").dispatchEvent(e);";
                            InjectJs(gecko.Document, str);
                        });
                        Thread.Sleep(100);
                        Invoke((Action)delegate
                        {
                            string jsStr7 = "document.querySelector('#J-depositAmount').value= '" + money + "'";
                            InjectJs(gecko.Document, jsStr7);
                        });
                        Thread.Sleep(100);
                        Invoke((Action)delegate
                        {
                            string jsStr6 = "document.querySelector('#J-deposit-submit').click();";
                            InjectJs(gecko.Document, jsStr6);
                        });
                    }).Start();
                    return;
                }
            }
            catch (Exception)
            {
            }
        }

        public static long ConvertDateTimeToInt(string time2)
        {
            DateTime dateTime = DateTime.Parse(time2);
            DateTime dateTime2 = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0, 0));
            return (dateTime.Ticks - dateTime2.Ticks) / 10000;
        }

        private void MyObs_TicketLoadedEvent(HttpChannel p_HttpChannel, object sender, EventArgs e)
        {
            try
            {
                if (p_HttpChannel.Uri.ToString().Contains("ebankDepositConfirm") && sender is StreamListenerTee)
                {
                    byte[] capturedData = (sender as StreamListenerTee).GetCapturedData();
                    string @string = Encoding.UTF8.GetString(capturedData);
                    Regex regex = new Regex("<form id=\"ebankDepositForm\" name=\"ebankDepositForm\" method=\"POST\" action=\"(?<url>.*?)\" target=\"_blank\" >", RegexOptions.Multiline);
                    Regex regex2 = new Regex("<input type=\"hidden\" name=\"epccGwMsg\" value=\"(?<token>.*?)\" />", RegexOptions.Multiline);
                    string value = regex.Match(@string).Groups["url"].Value;
                    string value2 = regex2.Match(@string).Groups["token"].Value;
                    if (value2 != "")
                    {
                        model.data.url = value;
                        model.data.qrcode = value2;
                        model.data.state = 3;
                        client.SendMessage(JsonConvert.SerializeObject(model));
                        WriteLog("二维码链接已返回：" + value);
                        if (value != "https://netpay.cmbchina.com/netpayment/PC_EpccPay.do" && value != "https://ibsbjstar.ccb.com.cn/CCBIS/CCBWLReqServlet")
                        {
                            gecko.Navigate("https://custweb.alipay.com/account/index.htm");
                        }
                    }
                }
                if (!p_HttpChannel.Uri.ToString().Contains("enterpriseSingleChannelDeposit") || !(sender is StreamListenerTee))
                {
                    return;
                }
                byte[] capturedData2 = (sender as StreamListenerTee).GetCapturedData();
                string string2 = Encoding.UTF8.GetString(capturedData2);
                Regex regex3 = new Regex("action=\"(?<url>.*?)\" target=\"_blank\">", RegexOptions.Multiline);
                Regex regex4 = new Regex("<input type=\"hidden\" name=\"epccGwMsg\" value=\"(?<token>.*?)\" />", RegexOptions.Multiline);
                string value3 = regex3.Match(string2).Groups["url"].Value;
                string value4 = regex4.Match(string2).Groups["token"].Value;
                if (value4 != "")
                {
                    model.data.url = value3;
                    model.data.qrcode = value4;
                    model.data.state = 3;
                    client.SendMessage(JsonConvert.SerializeObject(model));
                    WriteLog("二维码链接已返回：" + value3);
                    if (value3 != "https://netpay.cmbchina.com/netpayment/PC_EpccPay.do" && value3 != "https://ibsbjstar.ccb.com.cn/CCBIS/CCBWLReqServlet")
                    {
                        gecko.Navigate("https://custweb.alipay.com/account/index.htm");
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void MessageReceived(string data)
        {
            if (data == "websocket_Opened")
            {
                WriteLog("websocket 链接成功");
                return;
            }
            if (data == "websocket_Closed" || data.Contains("websocket_Error"))
            {
                WriteLog("websocket 链接关闭");
                return;
            }
            if (JObject.Parse(data)["type"]!.ToString() == "qrcodec")
            {
                gecko1.Invoke((Action)delegate
                {
                    mCookies = gecko1.Document.Cookie;
                });
                TaskList.Add(data);
            }
            WriteLog("接收订单数据：" + data);
        }

        public void StartTask()
        {
            string RedirectUrl;
            while (true)
            {
                try
                {
                    string text = "";
                    if (IsStart)
                    {
                        Thread.Sleep(1500);
                        startIndex++;
                        if (startIndex >= 20)
                        {
                            startIndex = 0;
                            IsStart = false;
                        }
                        continue;
                    }
                    if (TaskList.Count > 0)
                    {
                        text = TaskList[0];
                        TaskList.RemoveAt(0);
                    }
                    if (text != "")
                    {
                        model = JsonConvert.DeserializeObject<qrModel2>(text);
                        string orderId = GetOrderId();
                        money = model.data.money;
                        if (model.data.bank_name == "招商银行" || model.data.bank_name == "中国建设银行")
                        {
                            IsStart = true;
                        }
                        if (orderId == "")
                        {
                            WriteLog("掉线咯2");
                            绑定用户.Enabled = true;
                            client.Dispose();
                            return;
                        }
                        WriteLog("生成任务Id：" + orderId);
                        string str = orderId.Split('?')[1];
                        orderNo = orderId.Split('?')[1].Split('&')[0].Split('=')[1];
                        foreach (KeyValuePair<string, string> item2 in dic)
                        {
                            if (item2.Key == model.data.bank_name || model.data.bank_name.Contains(item2.Key))
                            {
                                instIdIndex = item2.Value;
                            }
                        }
                        if (!orderId.Contains("templateFlow"))
                        {
                            HttpItem item = new HttpItem
                            {
                                Method = "POST",
                                URL = "https://cashiergtj.alipay.com/standard/deposit/depositCardForm.htm",
                                ContentType = "application/x-www-form-urlencoded",
                                Referer = "https://cashiergtj.alipay.com/standard/deposit/chooseBank.htm?" + str,
                                Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8",
                                UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36",
                                Cookie = mCookies,
                                Postdata = $"orderId={orderNo}&isCompositeWithBalance=&channelToken={model.data.bank}"
                            };
                            HttpResult html = http.GetHtml(item);
                            RedirectUrl = html.RedirectUrl;
                            WriteLog("生成任务1：" + RedirectUrl);
                            gecko.Invoke((Action)delegate
                            {
                                gecko.Navigate(RedirectUrl);
                            });
                        }
                        else
                        {
                            gecko.Invoke((Action)delegate
                            {
                                gecko.Navigate("https://bizfundprod.alipay.com/allocation/deposit/index.htm");
                            });
                        }
                        Thread.Sleep(5000);
                    }
                    Thread.Sleep(100);
                }
                catch (Exception)
                {
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            mCookies = gecko.Document.Cookie;
            Task.Run(delegate
            {
                string orderId = GetOrderId();
                WriteLog("订单号：" + orderId);
                string str = orderId.Split('?')[1];
                orderNo = orderId.Split('?')[1].Split('=')[1];
                HttpItem item = new HttpItem
                {
                    URL = "https://cashiergtj.alipay.com/standard/deposit/chooseBank.htm?" + str,
                    Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8",
                    UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36",
                    Method = "GET",
                    Referer = orderId,
                    Cookie = mCookies
                };
                string html = http.GetHtml(item).Html;
                MatchCollection matchCollection = new Regex("name=\"channelToken\" id=\"(?<id>.*?)\" value=\"(?<token>.*?)\"    />\\s+<label class=\"icon-box limited-coupon \"  for=\"(?<ff>.*?)\"  data-channel=\"(?<BankName>.*?)type").Matches(html);
                string text = "";
                for (int i = 0; i < matchCollection.Count; i++)
                {
                    string value = matchCollection[i].Groups["token"].Value;
                    string value2 = matchCollection[i].Groups["BankName"].Value;
                    text = text + value + " ==== " + value2 + "\r\n";
                }
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mCookies = gecko.Document.Cookie;
            Task.Run(delegate
            {
                string orderId = GetOrderId();
                if (orderId == "")
                {
                    gecko.Navigate("https://shanghu.alipay.com/i.htm");
                }
                else
                {
                    WriteLog("订单号：" + orderId);
                    string str = orderId.Split('?')[1];
                    if (!orderId.Contains("templateFlow"))
                    {
                        HttpItem item = new HttpItem
                        {
                            Method = "POST",
                            URL = "https://cashiergtj.alipay.com/standard/deposit/depositCardForm.htm",
                            ContentType = "application/x-www-form-urlencoded",
                            Referer = "https://cashiergtj.alipay.com/standard/deposit/chooseBank.htm?" + str,
                            Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8",
                            UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.132 Safari/537.36",
                            Cookie = mCookies,
                            Postdata = $"orderId={orderNo}&isCompositeWithBalance=&channelToken={model.data.bank}"
                        };
                        HttpResult html = http.GetHtml(item);
                        string RedirectUrl = html.RedirectUrl;
                        WriteLog("生成任务2：" + RedirectUrl);
                        gecko.Invoke((Action)delegate
                        {
                            gecko.Navigate(RedirectUrl);
                        });
                    }
                    else
                    {
                        gecko.Invoke((Action)delegate
                        {
                            gecko.Navigate(orderId);
                        });
                    }
                }
            });
        }

        private void btnAppLogin_Click(object sender, EventArgs e)
        {
            Task.Run(delegate
            {
                if (string.IsNullOrEmpty(txtUserName.Text))
                {
                    WriteLog("账号为空");
                }
                else if (string.IsNullOrEmpty(txtUserPwd.Text))
                {
                    WriteLog("密码为空");
                }
                else
                {
                    HttpItem item = new HttpItem
                    {
                        Method = "POST",
                        URL = "https://api.appleui.cn/?a=servlet&b=index&c=applogin",
                        ContentType = "application/x-www-form-urlencoded",
                        Postdata = $"phone={txtUserName.Text}&pwd={txtUserPwd.Text}"
                    };
                    HttpResult html = http.GetHtml(item);
                    JObject jObject = JObject.Parse(html.Html);
                    if (jObject["code"]!.ToString() == "200")
                    {
                        jObject = JObject.Parse(jObject["data"]!.ToString());
                        WriteLog("登录成功：" + jObject["phone"]!.ToString());
                        WriteLog("请选择账号");
                        item = new HttpItem
                        {
                            Method = "POST",
                            URL = "https://api.appleui.cn/?a=servlet&b=index&c=GetLandInfo",
                            ContentType = "application/x-www-form-urlencoded",
                            Postdata = string.Format("userid={0}&phone={1}&token={2}", jObject["id"]!.ToString(), jObject["phone"]!.ToString(), jObject["token"]!.ToString())
                        };
                        html = http.GetHtml(item);
                        jObject = JObject.Parse(html.Html);
                        if (jObject["code"]!.ToString() == "200")
                        {
                            jObject = JObject.Parse(html.Html);
                            JArray jArray = JArray.Parse(jObject["data"]!.ToString());
                            infoList = jArray.ToObject<List<UserBean>>();
                            Invoke((Action)delegate
                            {
                                comboBox1.DataSource = infoList;
                                comboBox1.ValueMember = "id";
                                comboBox1.DisplayMember = "username";
                            });
                        }
                    }
                    else
                    {
                        WriteLog("1:" + jObject["msg"]!.ToString());
                    }
                }
            });
        }

        private void 绑定用户_Click(object sender, EventArgs e)
        {
            if (infoList == null)
            {
                WriteLog("请先登录");
                return;
            }
            button1.Enabled = true;
            绑定用户.Enabled = false;
            userinfo = infoList[comboBox1.SelectedIndex];
            Control.CheckForIllegalCrossThreadCalls = false;
            string url = "ws://api.appleui.cn:8990/" + userinfo.userid;
            client = new WSocketClient(url);
            client.MessageReceived += MessageReceived;
            try
            {
                client.Start();
            }
            catch (Exception ex)
            {
                WriteLog("发生异常链接失败" + ex.ToString());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (client == null)
            {
                WriteLog("请先绑定");
                return;
            }
            button1.Enabled = false;
            绑定用户.Enabled = true;
            client.Dispose();
            WriteLog("断开链接");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string url = "https://b.alipay.com/";
            gecko.Navigate(url);
        }

        private void MainFrom_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Task.Run(delegate
            {
                Invoke((Action)delegate
                {
                    Image image = CaptureWindow(base.Handle);
                    pictureBox1.Image = image;
                });
            });
        }

        public Image CaptureWindow2(IntPtr handle)
        {
            IntPtr windowDC = User32.GetWindowDC(handle);
            User32.RECT rect = default(User32.RECT);
            User32.GetWindowRect(handle, ref rect);
            int nWidth = 200;
            int nHeight = 200;
            IntPtr intPtr = GDI32.CreateCompatibleDC(windowDC);
            IntPtr intPtr2 = GDI32.CreateCompatibleBitmap(windowDC, nWidth, nHeight);
            IntPtr hObject = GDI32.SelectObject(intPtr, intPtr2);
            GDI32.BitBlt(intPtr, 0, 0, nWidth, nHeight, windowDC, 850, 400, 13369376);
            GDI32.SelectObject(intPtr, hObject);
            GDI32.DeleteDC(intPtr);
            User32.ReleaseDC(handle, windowDC);
            Bitmap result = Image.FromHbitmap(intPtr2);
            GDI32.DeleteObject(intPtr2);
            return result;
        }

        public Image CaptureWindow(IntPtr handle)
        {
            IntPtr windowDC = User32.GetWindowDC(handle);
            User32.RECT rect = default(User32.RECT);
            User32.GetWindowRect(handle, ref rect);
            int nWidth = 800;
            int nHeight = 500;
            IntPtr intPtr = GDI32.CreateCompatibleDC(windowDC);
            IntPtr intPtr2 = GDI32.CreateCompatibleBitmap(windowDC, nWidth, nHeight);
            IntPtr hObject = GDI32.SelectObject(intPtr, intPtr2);
            GDI32.BitBlt(intPtr, 0, 0, nWidth, nHeight, windowDC, 250, 420, 13369376);
            GDI32.SelectObject(intPtr, hObject);
            GDI32.DeleteDC(intPtr);
            User32.ReleaseDC(handle, windowDC);
            Bitmap result = Image.FromHbitmap(intPtr2);
            GDI32.DeleteObject(intPtr2);
            return result;
        }

        public string Read1(Image filename)
        {
            try
            {
                BarcodeReader obj = new BarcodeReader
                {
                    Options =
                    {
                        CharacterSet = "UTF-8"
                    }
                };
                Bitmap barcodeBitmap = new Bitmap(filename);
                Result result = obj.Decode(barcodeBitmap);
                return (result == null) ? "" : result.Text;
            }
            catch (Exception)
            {
                WriteLog("该文件不是图片：");
                return "";
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.绑定用户 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnAppLogin = new System.Windows.Forms.Button();
            this.txtUserPwd = new System.Windows.Forms.TextBox();
            this.lbLog = new System.Windows.Forms.ListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel1.Location = new System.Drawing.Point(12, 236);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1026, 789);
            this.panel1.TabIndex = 19;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.groupBox1);
            this.groupBox2.Controls.Add(this.radioButton2);
            this.groupBox2.Controls.Add(this.radioButton1);
            this.groupBox2.Controls.Add(this.button6);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.绑定用户);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.comboBox1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.txtUserName);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.btnAppLogin);
            this.groupBox2.Controls.Add(this.txtUserPwd);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(423, 208);
            this.groupBox2.TabIndex = 30;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "登录区域";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Location = new System.Drawing.Point(3, 145);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(454, 124);
            this.groupBox1.TabIndex = 31;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "登录区域";
            this.groupBox1.Visible = false;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(315, 20);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(97, 44);
            this.button4.TabIndex = 34;
            this.button4.Text = "测试跳转";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(212, 20);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(97, 44);
            this.button3.TabIndex = 33;
            this.button3.Text = "测试跳转";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(109, 20);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(97, 44);
            this.button2.TabIndex = 32;
            this.button2.Text = "测试下单";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(6, 20);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(97, 44);
            this.button5.TabIndex = 24;
            this.button5.Text = "采集银行数据";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(320, 23);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(97, 44);
            this.button6.TabIndex = 32;
            this.button6.Text = "退出登录";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(320, 73);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 44);
            this.button1.TabIndex = 31;
            this.button1.Text = "切换用户";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // 绑定用户
            // 
            this.绑定用户.Location = new System.Drawing.Point(217, 73);
            this.绑定用户.Name = "绑定用户";
            this.绑定用户.Size = new System.Drawing.Size(97, 44);
            this.绑定用户.TabIndex = 30;
            this.绑定用户.Text = "绑定用户";
            this.绑定用户.UseVisualStyleBackColor = true;
            this.绑定用户.Click += new System.EventHandler(this.绑定用户_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 29;
            this.label1.Text = "用户";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(46, 95);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(165, 20);
            this.comboBox1.TabIndex = 28;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 65);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 27;
            this.label2.Text = "密码";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(46, 23);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(165, 21);
            this.txtUserName.TabIndex = 23;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 25;
            this.label3.Text = "账号";
            // 
            // btnAppLogin
            // 
            this.btnAppLogin.Location = new System.Drawing.Point(217, 23);
            this.btnAppLogin.Name = "btnAppLogin";
            this.btnAppLogin.Size = new System.Drawing.Size(97, 44);
            this.btnAppLogin.TabIndex = 24;
            this.btnAppLogin.Text = "登录";
            this.btnAppLogin.UseVisualStyleBackColor = true;
            this.btnAppLogin.Click += new System.EventHandler(this.btnAppLogin_Click);
            // 
            // txtUserPwd
            // 
            this.txtUserPwd.Location = new System.Drawing.Point(46, 62);
            this.txtUserPwd.Name = "txtUserPwd";
            this.txtUserPwd.Size = new System.Drawing.Size(165, 21);
            this.txtUserPwd.TabIndex = 26;
            // 
            // lbLog
            // 
            this.lbLog.ItemHeight = 12;
            this.lbLog.Location = new System.Drawing.Point(449, 12);
            this.lbLog.Name = "lbLog";
            this.lbLog.Size = new System.Drawing.Size(589, 208);
            this.lbLog.TabIndex = 32;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)));
            this.panel2.Location = new System.Drawing.Point(1044, 236);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(502, 789);
            this.panel2.TabIndex = 33;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(1044, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(398, 208);
            this.pictureBox1.TabIndex = 34;
            this.pictureBox1.TabStop = false;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(217, 123);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(71, 16);
            this.radioButton1.TabIndex = 33;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "商户模式";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(320, 123);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(71, 16);
            this.radioButton2.TabIndex = 34;
            this.radioButton2.Text = "个人模式";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // MainFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1563, 1037);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lbLog);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel1);
            this.Name = "MainFrom";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "支付宝";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFrom_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        private void button7_Click(object sender, EventArgs e)
        {

        }
    }
}
