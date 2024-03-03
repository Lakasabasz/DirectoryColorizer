// See https://aka.ms/new-console-template for more information

using DirectoryColorizer;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

var icon = IconExtractor.Extract("C:\\WINDOWS\\system32\\imageres.dll", -3, true);
if (icon is null)
{
    Console.WriteLine("Icon not found");
    return;
}

var shift = 30f;

var bitmapOriginal = icon.ToBitmap().ToSKBitmap();
bitmapOriginal.Pixels = bitmapOriginal.Pixels.Select(color =>
{
    color.ToHsv(out var h, out var s, out var v);
    return SKColor.FromHsv(Mod(h + shift, 360f), s, v, color.Alpha);
}).ToArray();

var bitmap = bitmapOriginal.ToBitmap();
bitmap.Save("shifted.png");

float Mod(float x, float m) => (x%m + m)%m;
