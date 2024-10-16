using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.IO.Image;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Layout.Borders;
using iText.Kernel.Colors;
using Microsoft.EntityFrameworkCore;
using ShopSiloAppFSD.Models;
using iText.Kernel.Pdf.Canvas.Draw;
using System.Globalization;

namespace ShopSiloAppFSD.Services
{
    public class InvoiceService
    {
        private readonly ShopSiloDBContext _context;

        public InvoiceService(ShopSiloDBContext context)
        {
            _context = context;
        }

        public async Task<byte[]> GenerateInvoicePdfAsync(int orderId)
        {
            CultureInfo indianCulture = new CultureInfo("en-IN");

            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.OrderID == orderId);

            if (order == null)
            {
                throw new Exception("Order not found");
            }

            using (MemoryStream ms = new MemoryStream())
            {
                PdfWriter writer = new PdfWriter(ms);
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                // Add company logo
                var logoPath = "D:\\HEXA-SEGUE 201 - .NET FSD\\FullStackSHOPSILO\\ShopSiloAppFSD\\shopsiloappfsd.client\\public\\images\\shopSiloLogo.png";
                ImageData logoImage = ImageDataFactory.Create(logoPath);
                Image logo = new Image(logoImage).ScaleAbsolute(80, 30); // Smaller size for better alignment

                // Create a table for the header section
                Table headerTable = new Table(2);
                headerTable.SetWidth(UnitValue.CreatePercentValue(100));

                // Add logo in the first cell
                Cell logoCell = new Cell().Add(logo).SetBorder(Border.NO_BORDER);
                logoCell.SetVerticalAlignment(VerticalAlignment.MIDDLE);
                headerTable.AddCell(logoCell);

                // Add company name and details in the second cell
                Cell companyDetailsCell = new Cell().Add(new Paragraph("ShopSilo E-commerce Pvt Ltd")
                    .SetFontSize(14)
                    .SetBold())
                    .Add(new Paragraph("Address: 1234, Business Park, Bangalore, India")
                    .SetFontSize(10))
                    .Add(new Paragraph("Phone: +91 9876543210 | Email: support@shopsilo.com")
                    .SetFontSize(10))
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetBorder(Border.NO_BORDER);
                headerTable.AddCell(companyDetailsCell);

                // Add the header table to the document
                document.Add(headerTable);

                // Invoice title
                document.Add(new Paragraph("INVOICE")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontSize(20)
                    .SetBold()
                    .SetMarginTop(10));

                // Add Invoice Number and Date
                document.Add(new Paragraph($"Invoice No: INV-{order.OrderID:0000}")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(12)
                    .SetMarginTop(10));
                document.Add(new Paragraph($"Invoice Date: {DateTime.Now:dd MMM yyyy}")
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(12));

                // Customer Info
                var customer = await _context.CustomerDetails.FirstOrDefaultAsync(cd => cd.CustomerID == order.UserID);
                var fullName = $"{customer.FirstName} {customer.LastName}";

                document.Add(new Paragraph("Bill To:")
                    .SetBold()
                    .SetFontSize(12)
                    .SetMarginTop(20));
                document.Add(new Paragraph($"Customer Name: {fullName}")
                    .SetFontSize(12));
                document.Add(new Paragraph($"Email: {order.User.Email}")
                    .SetFontSize(12));
                document.Add(new Paragraph($"Order Date: {order.OrderDate:dd MMM yyyy}")
                    .SetFontSize(12));

                // Add line separator
                document.Add(new LineSeparator(new SolidLine()).SetMarginTop(20));

                // Table for Order Items
                Table table = new Table(4, false)
                    .SetWidth(UnitValue.CreatePercentValue(100))
                    .SetMarginTop(10);

                // Add table headers
                table.AddHeaderCell(new Cell().Add(new Paragraph("Product")).SetBold().SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Quantity")).SetBold().SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Price")).SetBold().SetBackgroundColor(ColorConstants.LIGHT_GRAY));
                table.AddHeaderCell(new Cell().Add(new Paragraph("Total")).SetBold().SetBackgroundColor(ColorConstants.LIGHT_GRAY));

                // Accumulate total amount
                decimal totalAmount = 0;

                // Add order items to the table
                foreach (var item in order.OrderItems)
                {
                    decimal itemTotal = item.Price * item.Quantity; // Calculate total for each item
                    totalAmount += itemTotal; // Accumulate the total amount

                    table.AddCell(new Cell().Add(new Paragraph(item.Product.ProductName)));
                    table.AddCell(new Cell().Add(new Paragraph(item.Quantity.ToString())));
                    table.AddCell(new Cell().Add(new Paragraph("Rs." + item.Price.ToString("N2"))));
                    table.AddCell(new Cell().Add(new Paragraph("Rs." + (item.Price * item.Quantity).ToString("N2"))));
                }

                // Add table to the document
                document.Add(table);

                // Add total amount
                document.Add(new Paragraph($"Total Amount: Rs.{totalAmount.ToString("N2")}")
                    .SetBold()
                    .SetTextAlignment(TextAlignment.RIGHT)
                    .SetFontSize(14)
                    .SetMarginTop(20));

                // Add footer or thank you note
                document.Add(new Paragraph("Thank you for your purchase!")
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetFontColor(ColorConstants.GRAY)
                    .SetFontSize(12)
                    .SetMarginTop(30));

                // Close the document
                document.Close();

                // Return the PDF as byte array
                return ms.ToArray();
            }
        }
    }
}
