// Copyright (c) 2024 Softbery by Paweł Tobis
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfbLibrary
{
    public class Pdf
    {
        private static string _scanPath = $"/path/scans";
        private static string _pdfPath = $"/path/pdf";

        private string _name;
        public string Name { get => _name; private set => _name = value; }

        public List<string> this[string[] file_name]
        {
            get => ImagesToPdf(file_name).Result;
        }
        public Pdf() { }

        public Pdf(string[] images_files)
        {
            ImagesToPdf(images_files).RunSynchronously();
        }

        public async Task<List<string>> ImagesToPdf(string[] file_name)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var pdfs = new List<string>();

                    foreach (var item in file_name)
                    {
                        try
                        {
                            var image_name = $"{_scanPath}{item}";

                            ImageData imageData = ImageDataFactory.Create($"{image_name}");
                            PdfWriter pdfWriter = new PdfWriter($"{_pdfPath}{item}.pdf");
                            PdfDocument pdfDocument = new PdfDocument(pdfWriter);
                            Document document = new Document(pdfDocument);

                            Image image = new Image(imageData);
                            image.SetWidth(pdfDocument.GetDefaultPageSize().GetWidth() - 50);
                            image.SetAutoScaleHeight(true);

                            document.Add(image);
                            pdfDocument.Close();

                            pdfs.Add($"{_pdfPath}{item}.pdf");
                        }
                        catch (Exception ex)
                        {
                            var log = new Log("Problem z tworzeniem PDF.", ex);
                            Logger.Write(log);
                        }
                    }
                    return pdfs;
                }
                catch (Exception ex)
                {
                    var log = new Log($"Problem przy generowaniu pliku PDF. {ex.Message}");

                    return new List<string>() { log.ToString() };
                }

            });
        }
    }
}
