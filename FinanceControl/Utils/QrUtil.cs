using SkiaSharp;
using ZXing;
using ZXing.SkiaSharp;

namespace FinanceControl.Utils;

public static class QrUtil
{
    public static string? Decode(IFormFile file)
    {
        SKBitmap image;
        try
        {
            using var stream = file.OpenReadStream();
            image = SKBitmap.Decode(stream);
        }
        catch (Exception)
        {
            return null;
        }

        using (image)
        {
            var reader = new BarcodeReader
            {
                AutoRotate = true,
                Options = { TryHarder = true, TryInverted = true},
            };
            return reader.Decode(image)?.Text;
        }
    }
}