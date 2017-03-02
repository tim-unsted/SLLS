using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Ajax.Utilities;
using slls.App_Settings;
using slls.DAO;
using slls.Models;
using slls.Utils.Helpers;
using slls.ViewModels;
using Westwind.Globalization;

namespace slls.Areas.LibraryAdmin
{
    public class LabelsController : CatalogueBaseController
    {
        private readonly DbEntities _db = new DbEntities();
       // private readonly GenericRepository _repository;

        public Dictionary<string, string> GetOrderByOptions()
        {
            return new Dictionary<string, string>
            {
                {"title", "Title"},
                {"filedtitle", "Filed Title"},
                {"author", "Author"},
                {"location", "Location"},
                {"classmark", "Classmark"}
            };
        }

        public ActionResult SetAllCopiesToPrint()
        {
            var newTitlesList = DbRes.T("NewTitlesList", "EntityType");
            var gcvm = new GenericConfirmationViewModel
            {
                PostConfirmController = "Labels",
                PostConfirmAction = "PostSetAllCopiesToPrint",
                DetailsText = "You are about to set the labels for ALL " + DbRes.T("Copies", "EntityType") + " to print.",
                ConfirmationText = "Do you want to continue?",
                ConfirmButtonText = "Yes",
                ConfirmButtonClass = "btn-success",
                CancelButtonText = "Cancel",
                HeaderText = "Print Labels",
                Glyphicon = "glyphicon-ok"
            };
            return PartialView("_GenericConfirmation", gcvm);
        }

        [HttpPost]
        public ActionResult PostSetAllCopiesToPrint(GenericConfirmationViewModel model)
        {
            try
            {
                _db.Database.ExecuteSqlCommand("UPDATE Copies SET PrintLabel = 1");
                TempData["SuccessDialogMsg"] = "All " + DbRes.T("Copies.Copy","FieldDisplayName").ToLower() + " labels have been set to print.";
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message.ToString());
                return Json(new { success = false });
            }
        }

        public ActionResult ClearAllCopiesToPrint()
        {
            var newTitlesList = DbRes.T("NewTitlesList", "EntityType");
            var gcvm = new GenericConfirmationViewModel
            {
                PostConfirmController = "Labels",
                PostConfirmAction = "PostClearAllCopiesToPrint",
                ConfirmationText = "Do you want to continue?",
                DetailsText = "You are about to clear the print labels for ALL " + DbRes.T("Copies","EntityType").ToLower() + ".",
                ConfirmButtonText = "Yes",
                ConfirmButtonClass = "btn-danger",
                CancelButtonText = "Cancel",
                HeaderText = "Clear Copy Print Labels",
                Glyphicon = "glyphicon-ok"
            };
            return PartialView("_GenericConfirmation", gcvm);
        }

        [HttpPost]
        public ActionResult PostClearAllCopiesToPrint(GenericConfirmationViewModel model)
        {
            try
            {
                _db.Database.ExecuteSqlCommand("UPDATE Copies SET PrintLabel = 0");
                TempData["SuccessDialogMsg"] = "Print labels for ALL " + DbRes.T("Copies.Copy","FieldDisplayName").ToLower() + " have been cleared successfully.";
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message.ToString());
                return Json(new { success = false });
            }
        }

        public ActionResult ClearAllVolumesToPrint()
        {
            var newTitlesList = DbRes.T("NewTitlesList", "EntityType");
            var gcvm = new GenericConfirmationViewModel
            {
                PostConfirmController = "Labels",
                PostConfirmAction = "PostClearAllVolumesToPrint",
                ConfirmationText = "Do you want to continue?",
                DetailsText = "You are about to clear the print labels for ALL " + DbRes.T("Volumes","EntityType").ToLower() + ".",
                ConfirmButtonText = "Yes",
                ConfirmButtonClass = "btn-danger",
                CancelButtonText = "Cancel",
                HeaderText = "Clear Volume Print Labels",
                Glyphicon = "glyphicon-ok"
            };
            return PartialView("_GenericConfirmation", gcvm);
        }

        [HttpPost]
        public ActionResult PostClearAllVolumesToPrint(GenericConfirmationViewModel model)
        {
            try
            {
                _db.Database.ExecuteSqlCommand("UPDATE Volumes SET PrintLabel = 0");
                TempData["SuccessDialogMsg"] = "Print labels for ALL volumes have been cleared successfully.";
                return Json(new { success = true });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message.ToString());
                return Json(new { success = false });
            }
        }

        public ActionResult BookLabelsSelectedCopiesPdf()
        {
            var pageMarginLeft = Settings.GetParameterValue("Labels.BookLabels.MarginLeft", "4.7",
                "Book label sheet left margin. The default is 4.7(mm) - Avery L7162 (2x8)", dataType: "double").Replace("mm", "").Trim();
            var pageMarginRight = Settings.GetParameterValue("Labels.BookLabels.MarginRight", "4.7",
                "Book label sheet right margin. The default is 4.7(mm) - Avery L7162 (2x8)", dataType: "double").Replace("mm", "").Trim();
            var pageMarginTop = Settings.GetParameterValue("Labels.BookLabels.MarginTop", "13.7",
                "Book label sheet top margin. The default is 13.7(mm) - Avery L7162 (2x8)", dataType: "double").Replace("mm", "").Trim();
            var pageMarginBottom = Settings.GetParameterValue("Labels.BookLabels.MarginBottom", "4.7",
                "Book label sheet bottom margin. The default is 4.7(mm) - Avery L7162 (2x8)", dataType: "double").Replace("mm", "").Trim();
            var pageRows = Settings.GetParameterValue("Labels.BookLabels.NumberDown", "8",
                "Book label sheet number of labels down the page. The default is 8 - Avery L7162 (2x8)", dataType: "int");
            var pageCols = Settings.GetParameterValue("Labels.BookLabels.NumberAcross", "2",
                "Book label sheet number of labels across the page. The default is 2 - Avery L7162 (2x8)", dataType: "int");
            
            var viewModel = new PrintLabelsViewModel()
            {
                LabelsPending = _db.Volumes.Any(v => v.Copy.PrintLabel),
                AlertMsg = "There are no pending copy labels to print.",
                Locations = SelectListHelper.OfficeLocationList(addDefault:true, msg:"Print by "),
                LocationID = 0,
                PostSelectAction = "PostBookLabelsSelectedCopiesPdf",
                leftMargin = float.Parse(pageMarginLeft),
                rightMargin = float.Parse(pageMarginRight),
                topMargin = float.Parse(pageMarginTop),
                bottomMargin = float.Parse(pageMarginBottom),
                labelsDown = int.Parse(pageRows),
                labelsAcross = int.Parse(pageCols),
                StartPositioncolumn = 1,
                StartPositionRow = 1,
                ShowOrderBy = true
            };

            ViewBag.OrderByOptions = GetOrderByOptions();
            ViewBag.Title = "Print Book Labels for Selected Copies";
            return PartialView("Select", viewModel);
        }
        
        public ActionResult PostBookLabelsSelectedCopiesPdf(PrintLabelsViewModel viewModel)
        {
            // Open a new PDF document - uses Avery L7162 as default label
            ViewBag.Title = "Book Labels for Selected Copies";
            float fltPageMarginLeft = viewModel.leftMargin;
            float fltPageMarginRight = viewModel.rightMargin;
            float fltPageMarginTop = viewModel.topMargin;
            float fltPageMarginBottom = viewModel.bottomMargin;
            int intPageRows = viewModel.labelsDown;
            int intPageColumns = viewModel.labelsAcross;
            int labelsUsed = ((viewModel.StartPositionRow - 1) * viewModel.labelsAcross) + (viewModel.StartPositioncolumn - 1);

            var doc = new Document();
            doc.SetMargins(fltPageMarginLeft, fltPageMarginRight, fltPageMarginTop, fltPageMarginBottom);
            var memoryStream = new MemoryStream();

            var pdfWriter = PdfWriter.GetInstance(doc, memoryStream);
            doc.Open();

            // Create the Label table

            PdfPTable table = new PdfPTable(intPageColumns);
            table.WidthPercentage = 100f;
            table.DefaultCell.Border = 0;

            //Helvetica is very close to Arial, but Arial is not a supported font in iTextSharp
            var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);

            //Get the copies to print ...
            var copiesToPrint = _db.Volumes.Where(v => v.Copy.PrintLabel);
            if (viewModel.LocationID > 0)
            {
                copiesToPrint = copiesToPrint.Where(x => x.Copy.LocationID == viewModel.LocationID);
            }

            Func<Volume, Object> orderByFunc = null;
            switch (viewModel.OrderBy)
            {
                case "title":
                    orderByFunc = item => item.Copy.Title.Title1.Trim();
                    break;
                case "filedtitle":
                    orderByFunc = item => item.Copy.Title.Title1.Substring(item.Copy.Title.NonFilingChars).Trim();
                    break;
                case "author":
                    orderByFunc = item => item.Copy.Title.AuthorString.Trim();
                    break;
                case "Location":
                    orderByFunc = item => item.Copy.Location.LocationString;
                    break;
                case "classmark":
                    orderByFunc = item => item.Copy.Title.Classmark.Classmark1;
                    break;
                default:
                    orderByFunc = item => item.Copy.Title.Title1.Trim();
                    break;
            }

            //Leave any blanks if specified ...
            for (int i = 0; i < labelsUsed; i++)
            {
                #region Label Construction

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.FixedHeight = (doc.PageSize.Height - (fltPageMarginLeft + fltPageMarginRight)) / intPageRows;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;

                var contents = new Paragraph();
                contents.Alignment = Element.ALIGN_LEFT;

                contents.Add(new Chunk(string.Format("{0}\n", ""), new Font(baseFont, 8f)));

                cell.AddElement(contents);
                table.AddCell(cell);

                #endregion
            }

            foreach (var volume in copiesToPrint.OrderBy(orderByFunc))
            {
                #region Label Construction

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.FixedHeight = (doc.PageSize.Height - (fltPageMarginLeft + fltPageMarginRight)) / intPageRows;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;

                var contents = new Paragraph();
                contents.Alignment = Element.ALIGN_LEFT;

                // Title : Edition
                if (!string.IsNullOrEmpty(volume.Copy.Title.Title1) && string.IsNullOrEmpty(volume.Copy.Title.Edition))
                {
                    contents.Add(new Chunk(string.Format("{0}\n", volume.Copy.Title.Title1.Trim()), new Font(baseFont, 8f, Font.BOLD)));
                }
                if (!string.IsNullOrEmpty(volume.Copy.Title.Title1) && !string.IsNullOrEmpty(volume.Copy.Title.Edition))
                {
                    contents.Add(new Chunk(string.Format("{0}: {1}\n", volume.Copy.Title.Title1.Trim(), volume.Copy.Title.Edition.Trim()), new Font(baseFont, 8f, Font.BOLD)));
                }
                
                //Author (miss line if empty)
                if (!string.IsNullOrEmpty(volume.Copy.Title.AuthorString))
                {
                    contents.Add(new Chunk(string.Format("{0}\n", volume.Copy.Title.AuthorString), new Font(baseFont, 8f)));
                }
               
                // Classmark (miss line if empty)
                if (!string.IsNullOrEmpty(volume.Copy.Title.Classmark.Classmark1))
                {
                    contents.Add(new Chunk(string.Format("{0}: {1}\n", DbRes.T("Classmarks.Classmark", "FieldDisplayName"), volume.Copy.Title.Classmark.Classmark1),
                        new Font(baseFont, 8f)));
                }

                // Office - Location (miss line if empty)
                if (!string.IsNullOrEmpty(volume.Copy.Location.LocationString))
                {
                    contents.Add(new Chunk(string.Format("{0}\n", volume.Copy.Location.LocationString),
                        new Font(baseFont, 8f)));
                }

                // Copy Number (miss line if empty)
                if (volume.Copy.CopyNumber > 0)
                {
                    contents.Add(new Chunk(string.Format("{0}: {1}\n", DbRes.T("Copies.Copy_Number", "FieldDisplayName"), volume.Copy.CopyNumber), 
                        new Font(baseFont, 8f)));
                }

                // Label Text : Barcode (miss line if empty)
                if (!string.IsNullOrEmpty(volume.LabelText) && !string.IsNullOrEmpty(volume.Barcode))
                {
                    StringBuilder s = new StringBuilder().Append(volume.LabelText).Append(' ', 15).Append(string.Format("{0}: ", DbRes.T("CopyItems.Barcode", "FieldDisplayName"))).Append(volume.Barcode).AppendLine("");
                    contents.Add(new Chunk(s.ToString(),
                        new Font(baseFont, 9f, Font.BOLD)));
                }
                else if (string.IsNullOrEmpty(volume.LabelText) && !string.IsNullOrEmpty(volume.Barcode))
                {
                    contents.Add(new Chunk(string.Format("{0}: {1}\n", DbRes.T("CopyItems.Barcode", "FieldDisplayName"), volume.Barcode),
                        new Font(baseFont, 9f, Font.BOLD)));
                }
                else if (!string.IsNullOrEmpty(volume.LabelText) && string.IsNullOrEmpty(volume.Barcode))
                {
                    contents.Add(new Chunk(string.Format("{0}\n", volume.LabelText),
                        new Font(baseFont, 9f, Font.BOLD)));
                }

                cell.AddElement(contents);
                table.AddCell(cell);

                #endregion
            }

            table.CompleteRow();
            doc.Add(table);

            // Close PDF document and send

            pdfWriter.CloseStream = false;
            doc.Close();
            memoryStream.Position = 0;

            return File(memoryStream, "application/pdf");
        }

        public ActionResult BookLabelsSelectedVolumesPdf()
        {
            var pageMarginLeft = Settings.GetParameterValue("Labels.BookLabels.MarginLeft", "4.7",
                "Book label sheet left margin. The default is 4.7(mm) - Avery L7162 (2x8)", dataType: "double").Replace("mm", "").Trim();
            var pageMarginRight = Settings.GetParameterValue("Labels.BookLabels.MarginRight", "4.7",
                "Book label sheet right margin. The default is 4.7(mm) - Avery L7162 (2x8)", dataType: "double").Replace("mm", "").Trim();
            var pageMarginTop = Settings.GetParameterValue("Labels.BookLabels.MarginTop", "13.7",
                "Book label sheet top margin. The default is 13.7(mm) - Avery L7162 (2x8)", dataType: "double").Replace("mm", "").Trim();
            var pageMarginBottom = Settings.GetParameterValue("Labels.BookLabels.MarginBottom", "4.7",
                "Book label sheet bottom margin. The default is 4.7(mm) - Avery L7162 (2x8)", dataType: "double").Replace("mm", "").Trim();
            var pageRows = Settings.GetParameterValue("Labels.BookLabels.NumberDown", "8",
                "Book label sheet number of labels down the page. The default is 8 - Avery L7162 (2x8)", dataType: "int");
            var pageCols = Settings.GetParameterValue("Labels.BookLabels.NumberAcross", "2",
                "Book label sheet number of labels across the page. The default is 2 - Avery L7162 (2x8)", dataType: "int");

            var viewModel = new PrintLabelsViewModel()
            {
                LabelsPending = _db.Volumes.Any(v => v.PrintLabel),
                AlertMsg = "There are no pending volume labels to print.",
                Locations = SelectListHelper.OfficeLocationList(addDefault: true, msg: "Print by "),
                LocationID = 0,
                PostSelectAction = "PostBookLabelsSelectedVolumesPdf",
                leftMargin = float.Parse(pageMarginLeft),
                rightMargin = float.Parse(pageMarginRight),
                topMargin = float.Parse(pageMarginTop),
                bottomMargin = float.Parse(pageMarginBottom),
                labelsDown = int.Parse(pageRows),
                labelsAcross = int.Parse(pageCols),
                StartPositioncolumn = 1,
                StartPositionRow = 1,
                ShowOrderBy = true
            };

            ViewBag.OrderByOptions = GetOrderByOptions();
            ViewBag.Title = "Print Book Labels for Selected Volumes";
            return PartialView("Select", viewModel);
        }

        public ActionResult PostBookLabelsSelectedVolumesPdf(PrintLabelsViewModel viewModel)
        {
            // Open a new PDF document - uses Avery L7162 as default label

            float fltPageMarginLeft = viewModel.leftMargin;
            float fltPageMarginRight = viewModel.rightMargin;
            float fltPageMarginTop = viewModel.topMargin;
            float fltPageMarginBottom = viewModel.bottomMargin;
            int intPageRows = viewModel.labelsDown;
            int intPageColumns = viewModel.labelsAcross;
            int lablesUsed = ((viewModel.StartPositionRow - 1) * viewModel.labelsAcross) + (viewModel.StartPositioncolumn - 1);

            var doc = new Document();
            doc.SetMargins(fltPageMarginLeft, fltPageMarginRight, fltPageMarginTop, fltPageMarginBottom);
            var memoryStream = new MemoryStream();

            var pdfWriter = PdfWriter.GetInstance(doc, memoryStream);
            doc.Open();

            // Create the Label table

            PdfPTable table = new PdfPTable(intPageColumns);
            table.WidthPercentage = 100f;
            table.DefaultCell.Border = 0;

            //Helvetica is very close to Arial, but Arial is not a supported font in iTextSharp
            var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);

            //Get the copies to print ...
            var copiesToPrint = _db.Volumes.Where(v => v.PrintLabel);
            if (viewModel.LocationID > 0)
            {
                copiesToPrint = copiesToPrint.Where(x => x.Copy.LocationID == viewModel.LocationID);
            }

            Func<Volume, Object> orderByFunc = null;
            switch (viewModel.OrderBy)
            {
                case "title":
                    orderByFunc = item => item.Copy.Title.Title1.Trim();
                    break;
                case "filedtitle":
                    orderByFunc = item => item.Copy.Title.Title1.Substring(item.Copy.Title.NonFilingChars).Trim();
                    break;
                case "author":
                    orderByFunc = item => item.Copy.Title.AuthorString.Trim();
                    break;
                case "Location":
                    orderByFunc = item => item.Copy.Location.LocationString;
                    break;
                case "classmark":
                    orderByFunc = item => item.Copy.Title.Classmark.Classmark1;
                    break;
                default:
                    orderByFunc = item => item.Copy.Title.Title1.Trim();
                    break;
            }

            //Leave any blanks if specified ...
            for (int i = 0; i < lablesUsed; i++)
            {
                #region Label Construction

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.FixedHeight = (doc.PageSize.Height - (fltPageMarginLeft + fltPageMarginRight)) / intPageRows;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;

                var contents = new Paragraph();
                contents.Alignment = Element.ALIGN_LEFT;

                contents.Add(new Chunk(string.Format("{0}\n", ""), new Font(baseFont, 8f)));

                cell.AddElement(contents);
                table.AddCell(cell);

                #endregion
            }

            foreach (var volume in copiesToPrint.OrderBy(orderByFunc))
            {
                #region Label Construction

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.FixedHeight = (doc.PageSize.Height - (fltPageMarginLeft + fltPageMarginRight)) / intPageRows;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;

                var contents = new Paragraph();
                contents.Alignment = Element.ALIGN_LEFT;

                // Title : Edition
                if (!string.IsNullOrEmpty(volume.Copy.Title.Title1) && string.IsNullOrEmpty(volume.Copy.Title.Edition))
                {
                    contents.Add(new Chunk(string.Format("{0}\n", volume.Copy.Title.Title1.Trim()), new Font(baseFont, 8f, Font.BOLD)));
                }
                if (!string.IsNullOrEmpty(volume.Copy.Title.Title1) && !string.IsNullOrEmpty(volume.Copy.Title.Edition))
                {
                    contents.Add(new Chunk(string.Format("{0}: {1}\n", volume.Copy.Title.Title1.Trim(), volume.Copy.Title.Edition.Trim()), new Font(baseFont, 8f, Font.BOLD)));
                }

                //Author (miss line if empty)
                if (!string.IsNullOrEmpty(volume.Copy.Title.AuthorString))
                {
                    contents.Add(new Chunk(string.Format("{0}\n", volume.Copy.Title.AuthorString), new Font(baseFont, 8f)));
                }

                // Classmark (miss line if empty)
                if (!string.IsNullOrEmpty(volume.Copy.Title.Classmark.Classmark1))
                {
                    contents.Add(new Chunk(string.Format("{0}: {1}\n", DbRes.T("Classmarks.Classmark", "FieldDisplayName"), volume.Copy.Title.Classmark.Classmark1),
                        new Font(baseFont, 8f)));
                }

                // Office - Location (miss line if empty)
                if (!string.IsNullOrEmpty(volume.Copy.Location.LocationString))
                {
                    contents.Add(new Chunk(string.Format("{0}\n", volume.Copy.Location.LocationString),
                        new Font(baseFont, 8f)));
                }

                // Copy Number (miss line if empty)
                if (volume.Copy.CopyNumber > 0)
                {
                    contents.Add(new Chunk(string.Format("{0}: {1}\n", DbRes.T("Copies.Copy_Number", "FieldDisplayName"), volume.Copy.CopyNumber),
                        new Font(baseFont, 8f)));
                }

                // Label Text : Barcode (miss line if empty)
                if (!string.IsNullOrEmpty(volume.LabelText) && !string.IsNullOrEmpty(volume.Barcode))
                {
                    StringBuilder s = new StringBuilder().Append(volume.LabelText).Append(' ', 15).Append(string.Format("{0}: ", DbRes.T("CopyItems.Barcode", "FieldDisplayName"))).Append(volume.Barcode).AppendLine("");
                    contents.Add(new Chunk(s.ToString(),
                        new Font(baseFont, 9f, Font.BOLD)));
                }
                else if (string.IsNullOrEmpty(volume.LabelText) && !string.IsNullOrEmpty(volume.Barcode))
                {
                    contents.Add(new Chunk(string.Format("{0}: {1}\n", DbRes.T("CopyItems.Barcode", "FieldDisplayName"), volume.Barcode),
                        new Font(baseFont, 9f, Font.BOLD)));
                }
                else if (!string.IsNullOrEmpty(volume.LabelText) && string.IsNullOrEmpty(volume.Barcode))
                {
                    contents.Add(new Chunk(string.Format("{0}\n", volume.LabelText),
                        new Font(baseFont, 9f, Font.BOLD)));
                }

                cell.AddElement(contents);
                table.AddCell(cell);

                #endregion
            }

            table.CompleteRow();
            doc.Add(table);

            // Close PDF document and send

            pdfWriter.CloseStream = false;
            doc.Close();
            memoryStream.Position = 0;

            return File(memoryStream, "application/pdf");
        }

        public ActionResult SpineLabelsSelectedCopiesPdf()
        {
            var pageMarginLeft = Settings.GetParameterValue("Labels.SpineLabels.MarginLeft", "4.75",
                "Book label sheet left margin. The default is 4.75(mm) - Avery L7651 (5x13)", dataType: "double").Replace("mm", "").Trim();
            var pageMarginRight = Settings.GetParameterValue("Labels.SpineLabels.MarginRight", "4.75",
                "Book label sheet right margin. The default is 4.75(mm) - Avery L7651 (5x13)", dataType: "double").Replace("mm", "").Trim();
            var pageMarginTop = Settings.GetParameterValue("Labels.SpineLabels.MarginTop", "11.6",
                "Book label sheet top margin. The default is 11.6(mm) - Avery L7651 (5x13)", dataType: "double").Replace("mm", "").Trim();
            var pageMarginBottom = Settings.GetParameterValue("Labels.SpineLabels.MarginBottom", "4.7",
                "Book label sheet bottom margin. The default is 4.7(mm) - Avery L7651 (5x13)", dataType: "double").Replace("mm", "").Trim();
            var pageRows = Settings.GetParameterValue("Labels.SpineLabels.NumberDown", "13",
                "Book label sheet number of labels down the page. The default is 8 - Avery L7651 (5x13))", dataType: "int");
            var pageCols = Settings.GetParameterValue("Labels.SpineLabels.NumberAcross", "5",
                "Book label sheet number of labels across the page. The default is 2 -Avery L7651 (5x13)", dataType: "int");

            var viewModel = new PrintLabelsViewModel()
            {
                LabelsPending = _db.Volumes.Any(v => v.Copy.PrintLabel),
                AlertMsg = "There are no pending copy labels to print.",
                Locations = SelectListHelper.OfficeLocationList(addDefault: true, msg: "Print by "),
                LocationID = 0,
                PostSelectAction = "PostSpineLabelsSelectedCopiesPdf",
                leftMargin = float.Parse(pageMarginLeft),
                rightMargin = float.Parse(pageMarginRight),
                topMargin = float.Parse(pageMarginTop),
                bottomMargin = float.Parse(pageMarginBottom),
                labelsDown = int.Parse(pageRows),
                labelsAcross = int.Parse(pageCols),
                StartPositioncolumn = 1,
                StartPositionRow = 1,
                ShowOrderBy = false
            };

            //ViewBag.OrderByOptions = GetOrderByOptions();
            ViewBag.Title = "Print Spine Labels for Selected Copies";
            return PartialView("Select", viewModel);
        }


        public ActionResult PostSpineLabelsSelectedCopiesPdf(PrintLabelsViewModel viewModel)
        {
            // Open a new PDF document - uses Avery L7651 as default label

            float fltPageMarginLeft = viewModel.leftMargin;
            float fltPageMarginRight = viewModel.rightMargin;
            float fltPageMarginTop = viewModel.topMargin;
            float fltPageMarginBottom = viewModel.bottomMargin;
            int intPageRows = viewModel.labelsDown;
            int intPageColumns = viewModel.labelsAcross;
            int lablesUsed = ((viewModel.StartPositionRow - 1) * viewModel.labelsAcross) + (viewModel.StartPositioncolumn - 1);

            var doc = new Document();
            doc.SetMargins(fltPageMarginLeft, fltPageMarginRight, fltPageMarginTop, fltPageMarginBottom);
            var memoryStream = new MemoryStream();

            var pdfWriter = PdfWriter.GetInstance(doc, memoryStream);
            doc.Open();

            // Create the Label table

            PdfPTable table = new PdfPTable(intPageColumns);
            table.WidthPercentage = 100f;
            table.DefaultCell.Border = 0;

            //Helvetica is very close to Arial, but Arial is not a supported font in iTextSharp
            var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);

            //Leave any blanks if specified ...
            for (int i = 0; i < lablesUsed; i++)
            {
                #region Label Construction

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.FixedHeight = (doc.PageSize.Height - (fltPageMarginLeft + fltPageMarginRight)) / intPageRows;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;

                var contents = new Paragraph();
                contents.Alignment = Element.ALIGN_LEFT;

                contents.Add(new Chunk(string.Format("{0}\n", ""), new Font(baseFont, 8f)));

                cell.AddElement(contents);
                table.AddCell(cell);

                #endregion
            }

            //Get the copies to print ...
            var copiesToPrint = _db.Volumes.Where(v => v.Copy.PrintLabel).ToList();
            foreach (var volume in copiesToPrint.OrderBy(x => x.Copy.Title.Classmark.Classmark1))
            {
                #region Label Construction

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.FixedHeight = (doc.PageSize.Height - (fltPageMarginLeft + fltPageMarginRight)) / intPageRows;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;

                var contents = new Paragraph();
                contents.Alignment = Element.ALIGN_LEFT;

                contents.Add(new Chunk(string.Format("{0}\n", volume.Copy.Title.Classmark.Classmark1), new Font(baseFont, 8f)));
                cell.AddElement(contents);
                table.AddCell(cell);

                #endregion
            }

            table.CompleteRow();
            doc.Add(table);

            // Close PDF document and send

            pdfWriter.CloseStream = false;
            doc.Close();
            memoryStream.Position = 0;

            return File(memoryStream, "application/pdf");
        }


        public ActionResult SpineLabelsSelectedVolumesPdf()
        {
            var pageMarginLeft = Settings.GetParameterValue("Labels.SpineLabels.MarginLeft", "4.75",
                "Book label sheet left margin. The default is 4.75(mm) - Avery L7651 (5x13)", dataType: "double").Replace("mm", "").Trim();
            var pageMarginRight = Settings.GetParameterValue("Labels.SpineLabels.MarginRight", "4.75",
                "Book label sheet right margin. The default is 4.75(mm) - Avery L7651 (5x13)", dataType: "double").Replace("mm", "").Trim();
            var pageMarginTop = Settings.GetParameterValue("Labels.SpineLabels.MarginTop", "11.6",
                "Book label sheet top margin. The default is 11.6(mm) - Avery L7651 (5x13)", dataType: "double").Replace("mm", "").Trim();
            var pageMarginBottom = Settings.GetParameterValue("Labels.SpineLabels.MarginBottom", "4.7",
                "Book label sheet bottom margin. The default is 4.7(mm) - Avery L7651 (5x13)", dataType: "double").Replace("mm", "").Trim();
            var pageRows = Settings.GetParameterValue("Labels.SpineLabels.NumberDown", "13",
                "Book label sheet number of labels down the page. The default is 8 - Avery L7651 (5x13))", dataType: "int");
            var pageCols = Settings.GetParameterValue("Labels.SpineLabels.NumberAcross", "5",
                "Book label sheet number of labels across the page. The default is 2 -Avery L7651 (5x13)", dataType: "int");

            var viewModel = new PrintLabelsViewModel()
            {
                LabelsPending = _db.Volumes.Any(v => v.PrintLabel),
                AlertMsg = "There are no pending volume labels to print.",
                Locations = SelectListHelper.OfficeLocationList(addDefault: true, msg: "Print by "),
                LocationID = 0,
                PostSelectAction = "PostSpineLabelsSelectedVolumesPdf",
                leftMargin = float.Parse(pageMarginLeft),
                rightMargin = float.Parse(pageMarginRight),
                topMargin = float.Parse(pageMarginTop),
                bottomMargin = float.Parse(pageMarginBottom),
                labelsDown = int.Parse(pageRows),
                labelsAcross = int.Parse(pageCols),
                StartPositioncolumn = 1,
                StartPositionRow = 1,
                ShowOrderBy = false
            };

            //ViewBag.OrderByOptions = GetOrderByOptions();
            ViewBag.Title = "Print Spine Labels for Selected Volumes";
            return PartialView("Select", viewModel);
        }


        public ActionResult PostSpineLabelsSelectedVolumesPdf(PrintLabelsViewModel viewModel)
        {
            // Open a new PDF document - uses Avery L7651 as default label

            float fltPageMarginLeft = viewModel.leftMargin;
            float fltPageMarginRight = viewModel.rightMargin;
            float fltPageMarginTop = viewModel.topMargin;
            float fltPageMarginBottom = viewModel.bottomMargin;
            int intPageRows = viewModel.labelsDown;
            int intPageColumns = viewModel.labelsAcross;
            int lablesUsed = ((viewModel.StartPositionRow - 1) * viewModel.labelsAcross) + (viewModel.StartPositioncolumn - 1);

            var doc = new Document();
            doc.SetMargins(fltPageMarginLeft, fltPageMarginRight, fltPageMarginTop, fltPageMarginBottom);
            var memoryStream = new MemoryStream();

            var pdfWriter = PdfWriter.GetInstance(doc, memoryStream);
            doc.Open();

            // Create the Label table

            PdfPTable table = new PdfPTable(intPageColumns);
            table.WidthPercentage = 100f;
            table.DefaultCell.Border = 0;

            //Helvetica is very close to Arial, but Arial is not a supported font in iTextSharp
            var baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);

            //Leave any blanks if specified ...
            for (int i = 0; i < lablesUsed; i++)
            {
                #region Label Construction

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.FixedHeight = (doc.PageSize.Height - (fltPageMarginLeft + fltPageMarginRight)) / intPageRows;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;

                var contents = new Paragraph();
                contents.Alignment = Element.ALIGN_LEFT;

                contents.Add(new Chunk(string.Format("{0}\n", ""), new Font(baseFont, 8f)));

                cell.AddElement(contents);
                table.AddCell(cell);

                #endregion
            }

            //Get the copies to print ...
            var volumesToPrint = _db.Volumes.Where(v => v.PrintLabel).ToList();
            foreach (var volume in volumesToPrint.OrderBy(x => x.Copy.Title.Classmark.Classmark1))
            {
                #region Label Construction

                PdfPCell cell = new PdfPCell();
                cell.Border = 0;
                cell.FixedHeight = (doc.PageSize.Height - (fltPageMarginLeft + fltPageMarginRight)) / intPageRows;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;

                var contents = new Paragraph();
                contents.Alignment = Element.ALIGN_LEFT;

                contents.Add(new Chunk(string.Format("{0}\n", volume.Copy.Title.Classmark.Classmark1), new Font(baseFont, 8f)));
                cell.AddElement(contents);
                table.AddCell(cell);

                #endregion
            }

            table.CompleteRow();
            doc.Add(table);

            // Close PDF document and send

            pdfWriter.CloseStream = false;
            doc.Close();
            memoryStream.Position = 0;

            return File(memoryStream, "application/pdf");
        }

    }
}