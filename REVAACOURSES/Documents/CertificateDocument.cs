using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using REVAACOURSES.Models;

namespace REVAACOURSES.Documents
{
    public class CertificateDocument : IDocument
    {
        private readonly Certificate _certificate;

        public CertificateDocument(Certificate certificate)
        {
            _certificate = certificate;
        }

        public DocumentMetadata GetMetadata()
        {
            return DocumentMetadata.Default;
        }

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);
                page.PageColor(Colors.White);

                page.Content()
                    .Border(6)
                    .BorderColor(Colors.Amber.Darken2)
                    .Padding(25)
                    .Column(column =>
                    {
                        column.Spacing(15);

                        // Logo
                        column.Item()
                            .AlignCenter()
                            .Width(70)
                            .Image("wwwroot/Images/logo.png");

                        // Title
                        column.Item()
                            .AlignCenter()
                            .Text("CERTIFICATE OF COMPLETION")
                            .FontSize(30)
                            .Bold()
                            .FontColor(Colors.Blue.Darken4);

                        // Subtitle
                        column.Item()
                            .AlignCenter()
                            .Text("This certificate is proudly presented to")
                            .FontSize(16);

                        // Student Name
                        column.Item()
                            .AlignCenter()
                            .Text(_certificate.Student.User.Name)
                            .FontSize(38)
                            .Bold()
                            .FontColor(Colors.Blue.Darken3);
                        // Description
                        column.Item()
                            .AlignCenter()
                            .Text("For successfully completing the course")
                            .FontSize(16);

                        // Course Name
                        column.Item()
                            .AlignCenter()
                            .Text(_certificate.Course.Title)
                            .FontSize(30)
                            .Bold()
                            .FontColor(Colors.Blue.Medium);

                        column.Item().PaddingTop(20);

                        column.Item().LineHorizontal(2);

                        column.Item().PaddingTop(15);

                        column.Item().Row(row =>
                        {
                            row.RelativeItem().Column(left =>
                            {
                                left.Item()
                                    .Text($"Issue Date : {_certificate.IssueDate:dd/MM/yyyy}");

                                left.Item()
                                    .PaddingTop(5)
                                    .Text($"Certificate No : {_certificate.CertificateNumber}");
                            });

                            row.ConstantItem(110)
                                .AlignMiddle()
                                .Image("wwwroot/Images/stamp.png");
                        });
                    });
            });
        }
    }
}