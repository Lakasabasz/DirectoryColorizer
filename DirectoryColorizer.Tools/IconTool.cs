using System.Drawing;
using System.Runtime.InteropServices;
using IniParser;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace DirectoryColorizer.Tools;

public static class IconTool
{
    [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
    private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

    [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern void SHChangeNotify(int wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
    
    private static float Mod(float x, float m) => (x%m + m)%m;
    
    public static Icon? Extract(string file, int number, bool largeIcon)
    {
        ExtractIconEx(file, number, out var large, out var small, 1);
        try
        {
            return Icon.FromHandle(largeIcon ? large : small);
        }
        catch
        {
            return null;
        }
    }

    public static Bitmap ShiftColors(Bitmap icon, float shift)
    {
        var bitmapOriginal = icon.ToSKBitmap();
        bitmapOriginal.Pixels = bitmapOriginal.Pixels.Select(color =>
        {
            color.ToHsv(out var h, out var s, out var v);
            return SKColor.FromHsv(Mod(h + shift, 360f), s, v, color.Alpha);
        }).ToArray();

        var bitmap = bitmapOriginal.ToBitmap();
        return bitmap;
    }

    public static Bitmap ShiftColors(Icon icon, float shift) => ShiftColors(icon.ToBitmap(), shift);

    public static Bitmap? PrepareIcon(string file, int index, float shift)
    {
        Icon? icon = Extract(file, index, true);
        return icon is null ? null : ShiftColors(icon, shift);
    }

    public static Bitmap? GetCurrentIcon(string folderPath)
    {
        if (!Directory.Exists(folderPath)) return null;
        Icon? extract = null;
        if (!File.Exists(folderPath + "/desktop.ini"))
            extract = Extract("C:\\WINDOWS\\system32\\imageres.dll", -3, true);
        else
        {
            var config = new FileIniDataParser().ReadFile(folderPath + "/desktop.ini");
            if (config is null) return null;
            var currentIconAddress = config?.Sections[".ShellClassInfo"]["IconResource"];
            if (currentIconAddress is null) return null;
            var currentIcon = currentIconAddress.Split(",");
            extract = Path.IsPathFullyQualified(currentIcon[0]) ? 
                Extract(currentIcon[0], int.Parse(currentIcon[1]), true) :
                Extract(folderPath + "/" + currentIcon[0], int.Parse(currentIcon[1]), true);
        }

        return extract?.ToBitmap();
    }

    public static void SetNewIcon(string path, Bitmap current)
    {
        throw new NotImplementedException();
    }

    public static void SetOriginalIcon(string path)
    {
        throw new NotImplementedException();
    }
}