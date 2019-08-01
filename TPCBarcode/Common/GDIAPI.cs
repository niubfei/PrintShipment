using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace TPCBarcode.Common
{
    public class GDIAPI
    {
        #region Device Caps Index
        const int DRIVERVERSION = 0;
        const int TECHNOLOGY = 2;
        const int HORZSIZE = 4;
        const int VERTSIZE = 6;
        const int HORZRES = 8;
        const int VERTRES = 10;
        const int BITSPIXEL = 12;
        const int PLANES = 14;
        const int NUMBRUSHES = 16;
        const int NUMPENS = 18;
        const int NUMMARKERS = 20;
        const int NUMFONTS = 22;
        const int NUMCOLORS = 24;
        const int PDEVICESIZE = 26;
        const int CURVECAPS = 28;
        const int LINECAPS = 30;
        const int POLYGONALCAPS = 32;
        const int TEXTCAPS = 34;
        const int CLIPCAPS = 36;
        const int RASTERCAPS = 38;
        const int ASPECTX = 40;
        const int ASPECTY = 42;
        const int ASPECTXY = 44;
        const int SHADEBLENDCAPS = 45;
        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;
        const int SIZEPALETTE = 104;
        const int NUMRESERVED = 106;
        const int COLORRES = 108;
        const int PHYSICALWIDTH = 110;
        const int PHYSICALHEIGHT = 111;
        const int PHYSICALOFFSETX = 112;
        const int PHYSICALOFFSETY = 113;
        const int SCALINGFACTORX = 114;
        const int SCALINGFACTORY = 115;
        const int VREFRESH = 116;
        const int DESKTOPVERTRES = 117;
        const int DESKTOPHORZRES = 118;
        const int BLTALIGNMENT = 119;
        #endregion

        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        internal static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("gdi32.dll")]
        internal static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        internal static extern bool SetProcessDPIAware();

        public static SizeF GetDeviceDPI()
        { 
            SetProcessDPIAware(); 
            IntPtr screenDC = GetDC(IntPtr.Zero);
            float x_dpi = GetDeviceCaps(screenDC, LOGPIXELSX);
            float y_dpi = GetDeviceCaps(screenDC, LOGPIXELSY);
            ReleaseDC(IntPtr.Zero, screenDC);

            return new SizeF(x_dpi, y_dpi);
        }
    }
}
