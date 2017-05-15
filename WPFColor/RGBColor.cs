using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WPFColor {
    class RGBColor {
        public static Color colorHx16toRGB(string strHxColor) {
            try {
                if (strHxColor.Length == 0) {//如果为空
                    return Color.FromRgb(0, 0, 0);//设为黑色
                } else {//转换颜色
                    try {
                        return Color.FromRgb(
                        (byte)System.Int32.Parse(strHxColor.Substring(3, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
                        (byte)System.Int32.Parse(strHxColor.Substring(5, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
                        (byte)System.Int32.Parse(strHxColor.Substring(7, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
                    }catch(ArgumentOutOfRangeException e) {
                        return Color.FromRgb(0, 0, 0);
                    }
                }
            } catch {//设为黑色
                return Color.FromRgb(0, 0, 0);
            }
        }
    }
}
