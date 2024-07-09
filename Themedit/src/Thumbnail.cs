// Version: 1.0.0.64
// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Themedit.src
{
    public class Thumbnail
    {
        /*private async Task<BitmapImage> CreatePreviewAsync(TimeSpan atTime, int width, int height)
        {
            var mediaComposition = new MediaComposition();
            var mediaClip = await MediaClip.CreateFromFileAsync(videoFile);
            mediaComposition.Clips.Add(mediaClip);

            var thumbnail = await mediaComposition.GetThumbnailAsync(atTime, width, height,
                                   VideoFramePrecision.NearestKeyFrame);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(thumbnail);
            PreviewImage.Source = bitmapImage;
        }*/
    }
}
