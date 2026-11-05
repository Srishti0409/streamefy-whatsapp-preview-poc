
using System;
using System.IO;
using SkiaSharp;

public static class ThumbnailGenerator
{
    public static void CreateThumbnail(string inputPath, string outputPath)
    {
        // Step 1: Load the original image
        using var original = SKBitmap.Decode(inputPath);

        if (original == null)
        {
            throw new InvalidOperationException(
                $"Unable to load image: {inputPath}"
            );
        }

        // Step 2: Define thumbnail dimensions
        const int targetWidth = 1200;
        const int targetHeight = 630;

        // Step 3: Calculate centre crop dimensions
        float targetRatio = (float)targetWidth / targetHeight;
        float sourceRatio = (float)original.Width / original.Height;

        int cropWidth;
        int cropHeight;

        if (sourceRatio > targetRatio)
        {
            cropHeight = original.Height;
            cropWidth = (int)Math.Round(cropHeight * targetRatio);
        }
        else
        {
            cropWidth = original.Width;
            cropHeight = (int)Math.Round(cropWidth / targetRatio);
        }

        // Step 4: Calculate crop position
        int cropX = (original.Width - cropWidth) / 2;
        int cropY = (original.Height - cropHeight) / 2;

        var sourceRect = new SKRectI(
            cropX,
            cropY,
            cropX + cropWidth,
            cropY + cropHeight
        );

        // Step 5: Create the resized thumbnail
        using var thumbnail = new SKBitmap(targetWidth, targetHeight);

        using (var canvas = new SKCanvas(thumbnail))
        {
            canvas.Clear(SKColors.White);

            var samplingOptions = new SKSamplingOptions(
                SKFilterMode.Linear,
                SKMipmapMode.Linear
            );

            canvas.DrawBitmap(
                original,
                sourceRect,
                new SKRect(0, 0, targetWidth, targetHeight),
                samplingOptions
            );
        }

        // Step 6: Encode as JPEG
        using var image = SKImage.FromBitmap(thumbnail);

        using var data = image.Encode(
            SKEncodedImageFormat.Jpeg,
            90
        );

        // Step 7: Create output directory if necessary
        string? directory = Path.GetDirectoryName(outputPath);

        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Step 8: Save the generated thumbnail
        using var output = File.Create(outputPath);
        data.SaveTo(output);

        Console.WriteLine("Thumbnail generated successfully!");
        Console.WriteLine($"Original dimensions: {original.Width} x {original.Height}");
        Console.WriteLine($"Thumbnail dimensions: {targetWidth} x {targetHeight}");
        Console.WriteLine($"Output file: {outputPath}");
    }
}
