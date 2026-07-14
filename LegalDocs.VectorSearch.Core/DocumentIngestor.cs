using System.Text;
using UglyToad.PdfPig;

namespace LegalDocs.VectorSearch.Core
{
    public class DocumentIngestor
    {
        public string ExtractTextFromDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException($"Directory not found at : {directoryPath}");
            }

            var textBuilder = new StringBuilder();

            var pdfFiles = Directory.GetFiles(directoryPath, "*.pdf");

            if (pdfFiles.Length == 0)
            {
                Console.WriteLine($"Warning: No PDF files found in : {directoryPath}");
            }

            foreach (var file in pdfFiles)
            {
                Console.WriteLine($"Loading file: {Path.GetFileName(file)}...");

                using (PdfDocument document = PdfDocument.Open(file))
                {
                    foreach (var page in document.GetPages())
                    {
                        textBuilder.AppendLine(page.Text.Trim());
                    }
                }
            }

            return textBuilder.ToString();
        }
    }
}