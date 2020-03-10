using System;
using System.Collections.Generic;
using System.Linq;
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

namespace radar_simulation
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        /// 随机点生成专用计时变量
        private DispatcherTimer generationTimer = new DispatcherTimer();
        private int generationFrequency = 1; // 单位：s

        private DispatcherTimer disappearanceTimer = new DispatcherTimer();
        private int disappearanceFrequency = 2; // 单位：s

        private double previous_theta = 0.0;

        private double rotate_angle = Math.PI / 12;

        public MainWindow()
        {
            InitializeComponent();

            this.ResizeMode = ResizeMode.NoResize;

            /// 制定计时器执行事件
            this.generationTimer.Tick += new EventHandler(PointSequentialGeneration);
            this.generationTimer.Interval = new TimeSpan(0, 0, this.generationFrequency);
            this.disappearanceTimer.Tick += new EventHandler(PointRemove);
            this.disappearanceTimer.Interval = new TimeSpan(0, 0, this.disappearanceFrequency);
        }

        public void SetGenerationFrequency(int freq)
        {
            this.generationFrequency = freq;
            this.generationTimer.Interval = new TimeSpan(0, 0, this.generationFrequency);
        }

        public void SetDisappearanceFrequency(int freq)
        {
            this.disappearanceFrequency = freq;
            this.disappearanceTimer.Interval = new TimeSpan(0, 0, this.disappearanceFrequency);
        }

        public void SetRotateAngle(double angle)
        {
            this.rotate_angle = angle;
        }

        public int GetGenerationFrequency()
        {
            return this.generationFrequency;
        }

        public int GetDisappearanceFrequency()
        {
            return this.disappearanceFrequency;
        }

        public double GetRotateAngle()
        {
            return this.rotate_angle;
        }

        private int GetRandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }

        private async void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            /// 按照特定时间间隔重复生成随机点
            this.generationTimer.Start();            

            await Task.Delay(this.generationFrequency * 1000);

            /// 按照特定时间间隔删除随机点
            this.disappearanceTimer.Start();
        }

        private void PointSequentialGeneration(object sender, EventArgs e)
        {
            /// 随机添加点
            double diameter = 780.0; //雷达图直径

            double point_x = 0.0;
            double point_y = 0.0;

            Random random = new Random(GetRandomSeed());
            double theta = (random.NextDouble() * (this.rotate_angle) + this.previous_theta) % (2 * Math.PI);
            double radius = random.NextDouble() * diameter / 2;

            this.previous_theta = theta;

            point_x = radius * Math.Sin(theta) + diameter / 2;
            point_y = diameter / 2 - radius * Math.Cos(theta);

            Ellipse ellipse = new Ellipse();
            ellipse.Stroke = Brushes.Black;
            ellipse.Fill = Brushes.Green;
            ellipse.Width = 10;
            ellipse.Height = 10;
            this.point_pool.Children.Add(ellipse);

            Canvas.SetLeft(ellipse, point_x);
            Canvas.SetTop(ellipse, point_y);
        }

        private void PointRemove(object sender, EventArgs e)
        {
            if (this.point_pool.Children.Count>0)
            {
                /// 在一定时间间隔后将添加的点删除
                this.point_pool.Children.RemoveAt(0);
            }
        }

        private void G3MenuItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /// 设置菜单图标显示状态
            this.g300icon.Source = new BitmapImage(new Uri("pack://application:,,,/images/circle_arrow_right.ico"));
            this.g200icon.Source = new BitmapImage(new Uri("pack://application:,,,/images/circle.ico"));
            this.g100icon.Source = new BitmapImage(new Uri("pack://application:,,,/images/circle.ico"));

            /// 设置窗口背景
            ImageBrush bg_brush = new ImageBrush();
            bg_brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/radar300.jpg"));
            bg_brush.Stretch = Stretch.Fill;
            this.grid.Background = bg_brush;
        }

        private void G2MenuItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /// 设置菜单图标显示状态
            this.g200icon.Source = new BitmapImage(new Uri("pack://application:,,,/images/circle_arrow_right.ico"));
            this.g300icon.Source = new BitmapImage(new Uri("pack://application:,,,/images/circle.ico"));
            this.g100icon.Source = new BitmapImage(new Uri("pack://application:,,,/images/circle.ico"));

            /// 设置窗口背景
            ImageBrush bg_brush = new ImageBrush();
            bg_brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/radar200.jpg"));
            bg_brush.Stretch = Stretch.Fill;
            this.grid.Background = bg_brush;
        }

        private void G1MenuItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            /// 设置菜单图标显示状态
            this.g100icon.Source = new BitmapImage(new Uri("pack://application:,,,/images/circle_arrow_right.ico"));
            this.g300icon.Source = new BitmapImage(new Uri("pack://application:,,,/images/circle.ico"));
            this.g200icon.Source = new BitmapImage(new Uri("pack://application:,,,/images/circle.ico"));

            /// 设置窗口背景
            ImageBrush bg_brush = new ImageBrush();
            bg_brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/radar100.jpg"));
            bg_brush.Stretch = Stretch.Fill;
            this.grid.Background = bg_brush;
        }

        private void Help_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            HelpWindow w = new HelpWindow();
            w.Owner = this;

            w.ShowDialog();
        }

        private void Angle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            AngleWindow w = new AngleWindow(this);
            w.Owner = this;

            w.ShowDialog();
        }

        private void Gfreq_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            GfreqWindow w = new GfreqWindow(this);
            w.Owner = this;

            w.ShowDialog();
        }

        private void Dfreq_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            DfreqWindow w = new DfreqWindow(this);
            w.Owner = this;

            w.ShowDialog();
        }

        private void Clear_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.point_pool.Children.Clear();
        }

        private void Reset_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            this.generationTimer.Stop();
            this.disappearanceTimer.Stop();
            this.point_pool.Children.Clear();

            this.generationFrequency = 1;
            this.disappearanceFrequency = 2;
            this.rotate_angle = Math.PI / 12;
            this.previous_theta = 0.0;

            /// 设置窗口背景
            ImageBrush bg_brush = new ImageBrush();
            bg_brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/radar300.jpg"));
            bg_brush.Stretch = Stretch.Fill;
            this.grid.Background = bg_brush;

            Canvas_Loaded(sender, e);
        }
    }
    public class AngleWindow: Window
    {
        private MainWindow parentWindow;

        private TextBox value;

        public AngleWindow(MainWindow current)
        {
            parentWindow = current;

            this.Width = 300;
            this.Height = 200;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Title = "请设置扇区范围";
            this.ResizeMode = ResizeMode.NoResize;

            Canvas info_panel = new Canvas();

            Label label = new Label();
            label.Content = "扇区范围";

            this.value = new TextBox();
            this.value.Text = parentWindow.GetRotateAngle()*180/Math.PI + "";
            this.value.Width = 100;

            Label unit = new Label();
            unit.Content = "度";

            Button confirm = new Button();
            confirm.Content = "确认";
            confirm.PreviewMouseDown += Confirm_MouseDown;

            Button cancel = new Button();
            cancel.Content = "取消";
            cancel.PreviewMouseDown += Cancel_MouseDown;

            info_panel.Children.Add(label);
            Canvas.SetLeft(label, 60);
            Canvas.SetTop(label, 30);

            info_panel.Children.Add(value);
            Canvas.SetLeft(value, 120);
            Canvas.SetTop(value, 35);

            info_panel.Children.Add(unit);
            Canvas.SetLeft(unit, 140);
            Canvas.SetTop(unit, 30);

            info_panel.Children.Add(confirm);
            Canvas.SetLeft(confirm, 100);
            Canvas.SetTop(confirm, 80);

            info_panel.Children.Add(cancel);
            Canvas.SetLeft(cancel, 160);
            Canvas.SetTop(cancel, 80);

            this.AddChild(info_panel);
        }

        private void Confirm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            String v = this.value.Text;
            double angle = Convert.ToDouble(v);

            if (angle < 0)
            {
                MessageBox.Show("扇区设置不能为负数", "参数设置错误");
            }
            else
            {
                parentWindow.SetRotateAngle(angle*Math.PI/180);
            }

            this.Close();
        }

        private void Cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
    public class HelpWindow : Window
    {
        public HelpWindow()
        {
            this.Width = 300;
            this.Height = 200;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Title = "帮助";
            this.ResizeMode = ResizeMode.NoResize;

            ImageBrush bg_brush = new ImageBrush();
            bg_brush.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/bg.png"));
            bg_brush.Stretch = Stretch.Fill;
            this.Background = bg_brush;

            DockPanel info_panel = new DockPanel();
            info_panel.HorizontalAlignment = HorizontalAlignment.Center;

            BitmapImage icon = new BitmapImage(new Uri("pack://application:,,,/images/radar.ico"));
            Image image = new Image();
            image.Source = icon;
            image.Width = 20;
            image.Height = 20;

            TextBlock help_text = new TextBlock();
            help_text.HorizontalAlignment = HorizontalAlignment.Center;
            help_text.Text = "雷达方位图仿真程序。版本：0.1（测试版）";
            Thickness thickness = new Thickness(10, 75, 0, 0);
            help_text.Margin = thickness;

            info_panel.Children.Add(image);
            info_panel.Children.Add(help_text);

            this.AddChild(info_panel);
        }
    }

    public class GfreqWindow : Window
    {
        private MainWindow parentWindow;

        private TextBox value;

        public GfreqWindow(MainWindow current)
        {
            parentWindow = current;

            this.Width = 300;
            this.Height = 200;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Title = "请设置出现间隔";
            this.ResizeMode = ResizeMode.NoResize;

            Canvas info_panel = new Canvas();

            Label label = new Label();
            label.Content = "出现间隔";

            this.value = new TextBox();
            this.value.Text = parentWindow.GetGenerationFrequency() + "";
            this.value.Width = 100;

            Label unit = new Label();
            unit.Content = "秒";

            Button confirm = new Button();
            confirm.Content = "确认";
            confirm.PreviewMouseDown += Confirm_MouseDown;

            Button cancel = new Button();
            cancel.Content = "取消";
            cancel.PreviewMouseDown += Cancel_MouseDown;

            info_panel.Children.Add(label);
            Canvas.SetLeft(label, 60);
            Canvas.SetTop(label, 30);

            info_panel.Children.Add(value);
            Canvas.SetLeft(value, 120);
            Canvas.SetTop(value, 35);

            info_panel.Children.Add(unit);
            Canvas.SetLeft(unit, 140);
            Canvas.SetTop(unit, 30);

            info_panel.Children.Add(confirm);
            Canvas.SetLeft(confirm, 100);
            Canvas.SetTop(confirm, 80);

            info_panel.Children.Add(cancel);
            Canvas.SetLeft(cancel, 160);
            Canvas.SetTop(cancel, 80);

            this.AddChild(info_panel);
        }

        private void Confirm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            String v = this.value.Text;
            int freq = Convert.ToInt32(v);

            if (freq > parentWindow.GetDisappearanceFrequency())
            {
                MessageBox.Show("生成间隔不能大于消失时延", "参数设置错误");
            }
            else
            {
                parentWindow.SetGenerationFrequency(freq);
            }

            this.Close();
        }

        private void Cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }

    public class DfreqWindow : Window
    {
        private MainWindow parentWindow;

        private TextBox value;

        public DfreqWindow(MainWindow current)
        {
            parentWindow = current;

            this.Width = 300;
            this.Height = 200;
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.Title = "请设置消失时延";
            this.ResizeMode = ResizeMode.NoResize;

            Canvas info_panel = new Canvas();

            Label label = new Label();
            label.Content = "消失时延";

            this.value = new TextBox();
            this.value.Text = parentWindow.GetDisappearanceFrequency() + "";
            this.value.Width = 100;

            Label unit = new Label();
            unit.Content = "秒";

            Button confirm = new Button();
            confirm.Content = "确认";
            confirm.PreviewMouseDown += Confirm_MouseDown;

            Button cancel = new Button();
            cancel.Content = "取消";
            cancel.PreviewMouseDown += Cancel_MouseDown;

            info_panel.Children.Add(label);
            Canvas.SetLeft(label, 60);
            Canvas.SetTop(label, 30);

            info_panel.Children.Add(value);
            Canvas.SetLeft(value, 120);
            Canvas.SetTop(value, 35);

            info_panel.Children.Add(unit);
            Canvas.SetLeft(unit, 140);
            Canvas.SetTop(unit, 30);

            info_panel.Children.Add(confirm);
            Canvas.SetLeft(confirm, 100);
            Canvas.SetTop(confirm, 80);

            info_panel.Children.Add(cancel);
            Canvas.SetLeft(cancel, 160);
            Canvas.SetTop(cancel, 80);

            this.AddChild(info_panel);
        }

        private void Confirm_MouseDown(object sender, MouseButtonEventArgs e)
        {
            String v = this.value.Text;
            int freq = Convert.ToInt32(v);

            if (freq < parentWindow.GetGenerationFrequency())
            {
                MessageBox.Show("消失时延不能低于扫描间隔", "参数设置错误");
            }
            else
            {
                parentWindow.SetDisappearanceFrequency(freq);
            }

            this.Close();
        }

        private void Cancel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}


