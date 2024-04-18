using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace gupiao
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        private DispatcherTimer timer;


        // 导入Windows API函数
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // 常量定义
        private const int MOD_SHIFT = 0x0004;
        private const int MOD_ALT = 0x0001;
        private const int WM_HOTKEY = 0x0312;


        public MainWindow()
        {
            InitializeComponent();
            InitializeTimer();
            Topmost = true;
            this.Loaded += MainWindow_Loaded;
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 获取窗口句柄
            IntPtr hWnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;

            // 注册 Shift + X 热键
            RegisterHotKey(hWnd, 1, MOD_SHIFT, (int)KeyInterop.VirtualKeyFromKey(Key.C));
        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            var hwndSource = PresentationSource.FromVisual(this) as System.Windows.Interop.HwndSource;
            hwndSource.AddHook(HwndHook);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                // 热键被触发
                int hotkeyId = wParam.ToInt32();
                if (hotkeyId == 1)
                {
                    // 按下 Shift + X 后显示/隐藏窗体
                    if (this.Visibility == Visibility.Visible)
                        this.Visibility = Visibility.Hidden;
                    else
                        this.Visibility = Visibility.Visible;
                }
                handled = true;
            }
            return IntPtr.Zero;
        }

        // 窗口关闭时注销热键
        protected override void OnClosed(EventArgs e)
        {
            // 获取窗口句柄
            IntPtr hWnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;

            UnregisterHotKey(hWnd, 1);
            UnregisterHotKey(hWnd, 2);
            base.OnClosed(e);
        }

        private void InitializeTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1); // 设置时间间隔为1秒
            timer.Tick += Timer_Tick; // 订阅Tick事件
            timer.Start(); // 启动定时器
        }

        string[] dd = { };

        private async void Timer_Tick(object sender, EventArgs e)
        {
            try
            {
                string dma = File.ReadAllText(@".\配置.txt");
                dd = dma.Split(',');
            }
            catch
            {
                using (StreamWriter writer = File.CreateText(@".\配置.txt"))
                {
                    writer.WriteLine("sz002882,10");
                }
                string dma = File.ReadAllText(@".\配置.txt");
                dd = dma.Split(',');
            }

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // 发送 GET 请求并获取响应
                    HttpResponseMessage response = await client.GetAsync("https://qt.gtimg.cn/q=" + dd[0]);

                    // 确保响应成功
                    response.EnsureSuccessStatusCode();

                    // 读取响应内容
                    byte[] bytes = await response.Content.ReadAsByteArrayAsync();

                    // 将字节数组解码为 UTF-8 字符串
                    string content = Encoding.GetEncoding("GB2312").GetString(bytes);

                    string[] qg1 = content.Split('"');
                    string[] qg2 = qg1[1].Split('~');
                    string name = qg2[1];
                    string dm = qg2[2];
                    double now = double.Parse(qg2[3]);
                    double old = double.Parse(qg2[4]);

                    double cz = now - old;

                    double zz = (cz / old) * 100;

                    double zz1 = Math.Round(zz, 2);

                    tdm.Text = dm;
                    tname.Text = name;
                    txj.Text = now.ToString();
                    tzz.Text = zz1 + "%";

                    tcb.Text = dd[1];

                    

                    double cd = double.Parse(dd[1]);

                    double cz1 = now - cd;

                    double zz2 = (cz1 / cd) * 100;

                    double zz3 = Math.Round(zz2, 3);

                    tzz1.Text = zz3 + "%";

                    SolidColorBrush red = new SolidColorBrush(Colors.Red);
                    SolidColorBrush green = new SolidColorBrush(Colors.Green);
                    SolidColorBrush grey = new SolidColorBrush(Colors.Black);

                    if (now < old)
                    {
                        txj.Foreground = green;
                        tzz.Foreground = green;
                    }
                    if (now > old)
                    {
                        txj.Foreground = red;
                        tzz.Foreground = red;
                    }
                    if (now == old)
                    {
                        txj.Foreground = grey;
                        tzz.Foreground = grey;
                    }

                    if (now < cd)
                    {
                        tcb.Foreground = green;
                        tzz1.Foreground = green;
                    }
                    if (now > cd)
                    {
                        tcb.Foreground = red;
                        tzz1.Foreground = red;
                    }
                    if (now == cd)
                    {
                        tcb.Foreground = grey;
                        tzz1.Foreground = grey;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());

                }
            }

        }


        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string filePath = @".\配置.txt"; // 你的配置文件路径
            Process.Start("notepad.exe", filePath);
        }

        private void MenuItem1_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
