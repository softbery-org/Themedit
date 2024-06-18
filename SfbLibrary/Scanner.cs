// Copyright (c) 2024 Softbery by Paweł Tobis
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WIA;

namespace SfbLibrary
{
    class Scanner
    {
        const string WIA_SCAN_COLOR_MODE = "6146";
        const string WIA_HORIZONTAL_SCAN_RESOLUTION_DPI = "6147";
        const string WIA_VERTICAL_SCAN_RESOLUTION_DPI = "6148";
        const string WIA_HORIZONTAL_SCAN_START_PIXEL = "6149";
        const string WIA_VERTICAL_SCAN_START_PIXEL = "6150";
        const string WIA_HORIZONTAL_SCAN_SIZE_PIXELS = "6151";
        const string WIA_VERTICAL_SCAN_SIZE_PIXELS = "6152";
        const string WIA_SCAN_BRIGHTNESS_PERCENTS = "6154";
        const string WIA_SCAN_CONTRAST_PERCENTS = "6155";

        private readonly DeviceInfo _deviceInfo;
        private readonly int _resolution = 100;
        private readonly int _widthPixel = 1275;
        private readonly int _heightPixel = 1755;
        private readonly int _colorMode = 1;

        public abstract class Formats
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public const string wiaFormatBMP = "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}";

            [MarshalAs(UnmanagedType.LPStr)]
            public const string wiaFormatPNG = "{B96B3CAF-0728-11D3-9D7B-0000F81EF32E}";

            [MarshalAs(UnmanagedType.LPStr)]
            public const string wiaFormatGIF = "{B96B3CB0-0728-11D3-9D7B-0000F81EF32E}";

            [MarshalAs(UnmanagedType.LPStr)]
            public const string wiaFormatJPEG = "{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}";

            [MarshalAs(UnmanagedType.LPStr)]
            public const string wiaFormatTIFF = "{B96B3CB1-0728-11D3-9D7B-0000F81EF32E}";
        }

        public Scanner(DeviceInfo deviceInfo)
        {
            this._deviceInfo = deviceInfo;
            _cancel += cancel;

        }

        public string[] GetFormats()
        {
            var list = new List<string>();
            var f = _deviceInfo.Connect().Commands;
            foreach (var item in f)
            {
                list.Add(item.ToString());
            }

            return list.ToArray();
        }

        private CancellationToken _cancellationToken;

        private Action<bool> _cancel;

        private void cancel()
        {
            if (_cancellationToken.IsCancellationRequested)
                _cancellationToken = new CancellationToken(false);
            else
                _cancellationToken = new CancellationToken(true);
        }
        private void cancel(bool cancel)
        {
            _cancellationToken = new CancellationToken(cancel);
        }
        /// <summary>
        /// Opena a scanner tools.
        /// </summary>
        /// <example>
        ///     scan text or image
        ///     scan color or mono
        /// </example>
        public Task<List<string>> MultiImageScan(string scan_path, string image_name)
        {
            return Task.Run(async () =>
            {
                WIA.CommonDialog dialog = new WIA.CommonDialog();
                WIA.Device device = dialog.ShowSelectDevice(WIA.WiaDeviceType.ScannerDeviceType);
                WIA.Items items = dialog.ShowSelectItems(device);
                List<string> images = new List<string>();
                List<string> img = new List<string>();
                // Convet from string to int64
                Int64 no = 0;
                if (image_name != null || image_name != "")
                {
                    try
                    {
                        no = Int64.Parse(image_name);
                    }
                    catch (Exception ex)
                    {
                        Log log = new Log($"Wprowadź jako temat maila numer pierwszego\nzlecenia od którego zaczynasz skanowanie. \n\n\n{ex.Message}");
                        Logger.Write(log);
                        MessageBox.Show(log.ToString());
                        return null;
                    }
                }
                else
                {
                    no = 0;
                }
                try
                {
                    if (items != null)
                    {
                        foreach (WIA.Item item in items)
                        {
                            if (items[1] == item)
                            {
                                ScanImage(Formats.wiaFormatBMP);
                            }
                            else
                            {
                                var dir = scan_path;
                                if (!Directory.Exists(dir))
                                {
                                    Directory.CreateDirectory(dir);
                                }
                                var path = $"{dir}";
                                while (true)
                                {
                                    WIA.ImageFile image = null;
                                    try
                                    {
                                        dialog = new WIA.CommonDialog();
                                        image = (WIA.ImageFile)dialog.ShowTransfer(item, "{B96B3CAB-0728-11D3-9D7B-0000F81EF32E}", true);
                                        if (image != null && image.FileData != null)
                                        {
                                            dynamic binaryData = image.FileData.get_BinaryData();
                                            if (binaryData is byte[])
                                            {
                                                using (MemoryStream stream = new MemoryStream(binaryData))
                                                {
                                                    using (Bitmap bitmap = (Bitmap)Bitmap.FromStream(stream))
                                                    {
                                                        //Logger.Write(new Log($"Skanowanie dokumentu."));

                                                        var random_name = Path.GetRandomFileName();
                                                        var name = $@"{no}";
                                                        no++;
                                                        //var name = $@"scan_{DateTime.Now.Day:00}{DateTime.Now.Month:00}{DateTime.Now.Year}_{DateTime.Now.Hour:00}{DateTime.Now.Minute:00}{DateTime.Now.Second:00}";
                                                        var file_name = $@"{name}.jpg";
                                                        path = dir + file_name;

                                                        bitmap.Save(String.Format(path, file_name), ImageFormat.Bmp);

                                                        //Logger.Write(new Log($"Konwertowanie pliku {file_name} na plik PDF."));

                                                        Pdf pdf = new Pdf();
                                                        await pdf.ImagesToPdf(new string[] { $"{file_name}" });
                                                        images.Add($"{name}");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (COMException)
                                    {
                                        break;
                                    }
                                    finally
                                    {
                                        if (image != null)
                                            Marshal.FinalReleaseComObject(image);
                                    }
                                }
                            }
                        }
                    }
                    return images;
                }
                catch (Exception ex)
                {
                    var log = new Log("Problem podczas skanowania.", ex);
                    Logger.Write(log);
                    MessageBox.Show(log.ToString());
                    return null;
                }
            }, _cancellationToken);
        }

        /// <summary>
        /// Scan an image with the specified format
        /// </summary>
        /// <param name="imageFormat">Expects a WIA.FormatID constant</param>
        /// <returns></returns>
        public ImageFile ScanImage(string imageFormat)
        {
            // Connect to the device and instruct it to scan
            // Connect to the device
            var device = this._deviceInfo.Connect();

            // Select the scanner
            WIA.CommonDialog dlg = new WIA.CommonDialog();

            var item = device.Items[1];

            try
            {
                AdjustScannerSettings(item, _resolution, 0, 0, _widthPixel, _heightPixel, 0, 0, _colorMode);


                object scanResult = dlg.ShowTransfer(item, imageFormat, true);

                if (scanResult != null)
                {
                    var imageFile = (ImageFile)scanResult;

                    // Return the imageFile
                    return imageFile;
                }
            }
            catch (COMException e)
            {
                // Display the exception in the console.
                Console.WriteLine(e.ToString());

                uint errorCode = (uint)e.ErrorCode;

                // Catch 2 of the most common exceptions
                if (errorCode == 0x80210006)
                {
                    MessageBox.Show("Skaner jest zajęty bądź nie jest gotowy.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (errorCode == 0x80210064)
                {
                    MessageBox.Show("Zatrzymano proces skanowania.", "Information", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Nieznany błąd! Sprawdź konsole logów! Skontaktuj się z deweloperem odpowiedzialnym za aplikację.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            return new ImageFile();
        }

        /// <summary>
        /// Adjusts the settings of the scanner with the providen parameters.
        /// </summary>
        /// <param name="scannnerItem">Expects a </param>
        /// <param name="scanResolutionDPI">Provide the DPI resolution that should be used e.g 150</param>
        /// <param name="scanStartLeftPixel"></param>
        /// <param name="scanStartTopPixel"></param>
        /// <param name="scanWidthPixels"></param>
        /// <param name="scanHeightPixels"></param>
        /// <param name="brightnessPercents"></param>
        /// <param name="contrastPercents">Modify the contrast percent</param>
        /// <param name="colorMode">Set the color mode</param>
        private void AdjustScannerSettings(IItem scannnerItem, int scanResolutionDPI, int scanStartLeftPixel, int scanStartTopPixel, int scanWidthPixels, int scanHeightPixels, int brightnessPercents, int contrastPercents, int colorMode)
        {
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_RESOLUTION_DPI, scanResolutionDPI);
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_START_PIXEL, scanStartLeftPixel);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_START_PIXEL, scanStartTopPixel);
            SetWIAProperty(scannnerItem.Properties, WIA_HORIZONTAL_SCAN_SIZE_PIXELS, scanWidthPixels);
            SetWIAProperty(scannnerItem.Properties, WIA_VERTICAL_SCAN_SIZE_PIXELS, scanHeightPixels);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_BRIGHTNESS_PERCENTS, brightnessPercents);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_CONTRAST_PERCENTS, contrastPercents);
            SetWIAProperty(scannnerItem.Properties, WIA_SCAN_COLOR_MODE, colorMode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        private void SetWIAProperty(IProperties properties, object propName, object propValue)
        {
            Property prop = properties.get_Item(ref propName);

            try
            {
                prop.set_Value(ref propValue);
            }
            catch
            {
                // DPI can only be set to values listed in SubTypeValues
                // This sets the DPI to the lowest one supported by the scanner
                if (propName.ToString() == WIA_HORIZONTAL_SCAN_RESOLUTION_DPI || propName.ToString() == WIA_VERTICAL_SCAN_RESOLUTION_DPI)
                {
                    foreach (object test in prop.SubTypeValues)
                    {
                        prop.set_Value(test);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Declare the ToString method
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return (string)this._deviceInfo.Properties["Name"].get_Value();
        }
    }
}
