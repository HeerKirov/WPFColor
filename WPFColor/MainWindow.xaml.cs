using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using System.Drawing;

namespace WPFColor {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Hbox = new ColorSelectBox(ColorHBox, false, 0.5);
            Sbox = new ColorSelectBox(ColorSBox, false, 0.5);
            Lbox = new ColorSelectBox(ColorLBox, false, 0.5);
            Hbox.Paint(getHColor, null);
            Sbox.Paint(getSColor, new Tuple<byte, byte, byte>(255, 0, 0));
            Lbox.Paint(getLColor, new Tuple<byte, byte, byte>(255, 0, 0));
            Rbox = new ColorSelectBox(ColorRBox, false, 0.5);
            Gbox = new ColorSelectBox(ColorGBox, false, 0.5);
            Bbox = new ColorSelectBox(ColorBBox, false, 0.5);
            Rbox.Paint(getRGBcolor, new Tuple<byte, byte, byte>(255,0,0));
            Gbox.Paint(getRGBcolor, new Tuple<byte, byte, byte>(0, 255, 0));
            Bbox.Paint(getRGBcolor, new Tuple<byte, byte, byte>(0, 0, 255));
            CreatePrefabList();
        }

        private ColorSelectBox Hbox, Sbox, Lbox, Rbox, Gbox, Bbox;
        private double pH, pS, pL;
        private byte bR, bG, bB;

        private Tuple<byte,byte,byte> getLColor(double percent,Tuple<byte,byte,byte> basec) {
            byte min = 0, max = 255;
            if (percent == 0.5) return basec;
            else if (percent < 0.5) return new Tuple<byte, byte, byte>(
                    getLinelyValue(min, basec.Item1, percent * 2),
                    getLinelyValue(min, basec.Item2, percent * 2),
                    getLinelyValue(min, basec.Item3, percent * 2)
            );
            else return new Tuple<byte, byte, byte>(
                getLinelyValue(basec.Item1, max, (percent - 0.5) * 2),
                getLinelyValue(basec.Item2, max, (percent - 0.5) * 2),
                getLinelyValue(basec.Item3, max, (percent - 0.5) * 2)
            );
        }
        private Tuple<byte,byte,byte> getSColor(double percent, Tuple<byte,byte,byte> basec) {
            byte min = 255 / 2;
            return new Tuple<byte, byte, byte>(
                getLinelyValue(min, basec.Item1, percent), 
                getLinelyValue(min, basec.Item2, percent), 
                getLinelyValue(min, basec.Item3, percent)
            );
        }
        private Tuple<byte, byte, byte> getHColor(double percent,Tuple<byte,byte,byte> basec = null) {
            double r = 0, g = 0, b = 0;
            while (percent >= 1) percent -= 1;
            int part = (int)(percent * 6);
            double partpercent = (percent * 6) - part;
            if (part == 0) {
                r = 1;
                g = partpercent;
                b = 0;
            }else if (part == 1) {
                r = 1 - partpercent;
                g = 1;
                b = 0;
            }else if (part == 2) {
                r = 0;
                g = 1;
                b = partpercent;
            }else if (part == 3) {
                r = 0;
                g = 1 - partpercent;
                b = 1;
            }else if (part == 4) {
                r = partpercent;
                g = 0;
                b = 1;
            }else if (part == 5) {
                r = 1;
                g = 0;
                b = 1 - partpercent;
            }
            return new Tuple<byte, byte, byte>((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
        }
        private Tuple<byte, byte, byte> getRGBcolor(double percent, Tuple<byte, byte, byte> basec) {
            byte max = 255;
            return new Tuple<byte, byte, byte>(
                getLinelyValue(basec.Item1, max, percent), 
                getLinelyValue(basec.Item2, max, percent), 
                getLinelyValue(basec.Item3, max, percent)
            );
        }
        private byte getLinelyValue(byte min, byte max, double percent) {
            return (byte)((max - min) * percent + min);
        }

        private void MakeHSLChange() {
            if (hslchange_lock.lockit()) {
                var hc = getHColor(pH);
                Sbox.Paint(getSColor, hc);
                Lbox.Paint(getLColor, getSColor(pS, hc));
                var nc = HslColor.HSL2RGB(pH, pS, pL);
                SetColor(Color.FromRgb(nc.R, nc.G, nc.B), SetColorControler.HSL);
                hslchange_lock.unlock();
            }
        }
        private Lock hslchange_lock = new Lock();
        private void RowSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (slider_lock[0].lockit()) {
                pH = RowSlider.Value / (RowSlider.Maximum - RowSlider.Minimum);
                HTxt.Text = (pH * 100).ToString();
                MakeHSLChange();
                slider_lock[0].unlock();
            }
        }
        private void DarkSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (slider_lock[1].lockit()) {
                pS = DarkSlider.Value / (DarkSlider.Maximum - DarkSlider.Minimum);
                STxt.Text = (pS * 100).ToString();
                MakeHSLChange();
                slider_lock[1].unlock();
            }
        }
        private void GraySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (slider_lock[2].lockit()) {
                pL = GraySlider.Value / (GraySlider.Maximum - GraySlider.Minimum);
                LTxt.Text = (pL * 100).ToString();
                var nc = HslColor.HSL2RGB(pH, pS, pL);
                SetColor(Color.FromRgb(nc.R, nc.G, nc.B), SetColorControler.HSL);
                slider_lock[2].unlock();
            }
        }
        private void LTxt_LostFocus(object sender, RoutedEventArgs e) {
            if ((sender as TextBox).Text == "") return;
            var value = double.Parse((sender as TextBox).Text) / 100;
            Slider sl = GraySlider;
            sl.Value = value * (sl.Maximum - sl.Minimum) + sl.Minimum;
        }
        private void STxt_LostFocus(object sender, RoutedEventArgs e) {
            if ((sender as TextBox).Text == "") return;
            var value = double.Parse((sender as TextBox).Text) / 100;
            Slider sl = DarkSlider;
            sl.Value = value * (sl.Maximum - sl.Minimum)+ sl.Minimum;
        }
        private void HTxt_LostFocus(object sender, RoutedEventArgs e) {
            if ((sender as TextBox).Text == "") return;
            var value = double.Parse((sender as TextBox).Text) / 100;
            Slider sl = RowSlider;
            sl.Value = value * (sl.Maximum - sl.Minimum) + sl.Minimum;
        }
        private Lock[] slider_lock = new Lock[] { new Lock(), new Lock(), new Lock() };

        private void MakeRGBChange() {
            if (rgbchange_lock.lockit()) {
                SetColor(Color.FromRgb(bR, bG, bB), SetColorControler.RGB);
                rgbchange_lock.unlock();
            }
        }
        private Lock rgbchange_lock = new Lock();
        private void RTxt_LostFocus(object sender, RoutedEventArgs e) {
            if ((sender as TextBox).Text == "") return;
            var value = double.Parse((sender as TextBox).Text) / 255;
            Slider sl = RSlider;
            sl.Value = value * (sl.Maximum - sl.Minimum) + sl.Minimum;
        }
        private void GTxt_LostFocus(object sender, RoutedEventArgs e) {
            if ((sender as TextBox).Text == "") return;
            var value = double.Parse((sender as TextBox).Text) / 255;
            Slider sl = GSlider;
            sl.Value = value * (sl.Maximum - sl.Minimum) + sl.Minimum;
        }
        private void BTxt_LostFocus(object sender, RoutedEventArgs e) {
            if ((sender as TextBox).Text == "") return;
            var value = double.Parse((sender as TextBox).Text) / 255;
            Slider sl = BSlider;
            sl.Value = value * (sl.Maximum - sl.Minimum) + sl.Minimum;
        }
        private void RSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (slider2_lock[0].lockit()) {
                bR = (byte)(RSlider.Value / (RSlider.Maximum - RSlider.Minimum) * 255);
                RTxt.Text = bR.ToString();
                MakeRGBChange();
                slider2_lock[0].unlock();
            }
        }
        private void GSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (slider2_lock[1].lockit()) {
                bG = (byte)(GSlider.Value / (GSlider.Maximum - GSlider.Minimum) * 255);
                GTxt.Text = bG.ToString();
                MakeRGBChange();
                slider2_lock[1].unlock();
            }
        }
        private void BSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            if (slider2_lock[1].lockit()) {
                bB = (byte)(BSlider.Value / (BSlider.Maximum - BSlider.Minimum) * 255);
                BTxt.Text = bB.ToString();
                MakeRGBChange();
                slider2_lock[1].unlock();
            }
        }
        private Lock[] slider2_lock = new Lock[] { new Lock(), new Lock(), new Lock() };

        private void HexCodeTxt_LostFocus(object sender, RoutedEventArgs e) {
            var c = RGBColor.colorHx16toRGB((sender as TextBox).Text);
            SetColor(c, SetColorControler.Prefab);
        }

        private void CreatePrefabList() {
            Type colortype = typeof(Colors);
            PropertyInfo[] props = propinfos = colortype.GetProperties();
            List<string> names = new List<string>();
            foreach(var i in props) {
                names.Add(i.Name);
            }
            PreColorList.ItemsSource = prefabcolors = names.ToArray(); 
        }
        private string[] prefabcolors = null;
        private PropertyInfo[] propinfos = null;
        private void PreColorList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (PreColorList.SelectedItem != null) {
                Color c = (Color)propinfos[PreColorList.SelectedIndex].GetValue(typeof(Colors));
                SetColor(c, SetColorControler.Prefab);
            }
        }

        

        private Color nowcolor = Color.FromRgb(0, 0, 0);
        public void SetColor(Color color,SetColorControler ct) {
            if (colorprefab_lock.lockit()) {
                if (color == null) return;
                nowcolor = color;
                if (ColorField.Fill == null) ColorField.Fill = new SolidColorBrush();
                (ColorField.Fill as SolidColorBrush).Color = color;
                if (ct == SetColorControler.Prefab || ct == SetColorControler.RGB) {
                    //接下来修改各种参数的显示。
                    double h, s, l;
                    HslColor.RGB2HSL(new ColorRGB(nowcolor), out h, out s, out l);
                    RowSlider.Value = h * (RowSlider.Maximum - RowSlider.Minimum) + RowSlider.Minimum;
                    DarkSlider.Value = s * (DarkSlider.Maximum - DarkSlider.Minimum) + DarkSlider.Minimum;
                    GraySlider.Value = l * (GraySlider.Maximum - GraySlider.Minimum) + GraySlider.Minimum;
                }
                if (ct == SetColorControler.Prefab || ct == SetColorControler.HSL) {
                    RSlider.Value = nowcolor.R * (RSlider.Maximum - RSlider.Minimum) / 255 + RSlider.Minimum;
                    GSlider.Value = nowcolor.G * (GSlider.Maximum - GSlider.Minimum) / 255 + GSlider.Minimum;
                    BSlider.Value = nowcolor.B * (BSlider.Maximum - BSlider.Minimum) / 255 + BSlider.Minimum;
                }
                HexCodeTxt.Text = nowcolor.ToString();
                colorprefab_lock.unlock();
            }
        }
        public enum SetColorControler {
            Prefab,RGB,HSL,Other
        }
        private Lock colorprefab_lock = new Lock();
    }

    class ColorSelectBox {
        Canvas goal;
        double width, height, step;
        bool directver;
        public ColorSelectBox(Canvas canvas,bool DirectToVer = true, double step = 1) {
            goal = canvas;
            width = goal.Width;
            height = goal.Height;
            this.step = step;
            directver = DirectToVer;
            items = new Line[(int)Math.Ceiling((DirectToVer ? height : width + 1) / step)];
            for(int i = 0; i < items.Length; ++i) {
                items[i] = new Line();
                if (DirectToVer) {
                    items[i].X1 = 0;
                    items[i].X2 = width;
                    items[i].Y1 = items[i].Y2 = i * step;
                }else {
                    items[i].X1 = items[i].X2 = i * step;
                    items[i].Y1 = 0;
                    items[i].Y2 = height;
                }
                items[i].Stroke = new SolidColorBrush();
                goal.Children.Add(items[i]);
            }
        }
        public void Paint(Func<double, Tuple<byte, byte, byte>, Tuple<byte, byte, byte>> func, Tuple<byte,byte,byte> basec) {
            for(int i = 0; i < items.Length; ++i) {
                var tup = func(i * 1.0 / items.Length,basec);
                var c = Color.FromRgb(tup.Item1, tup.Item2, tup.Item3);
                (items[i].Stroke as SolidColorBrush).Color = c;
            }
        }

        private Line[] items = null;
    }
    class Lock {
        private bool flag = true;
        public bool lockit() {
            if (!flag) return false;
            flag = false;
            return true;
        }
        public bool unlock() {
            flag = true;
            return true;
        }
    }
}
