using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.io;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.tool.xml;
namespace hc.epm.Common
{

    public class PDFHelper
    {
        /// <summary>
        /// 将html文本输出到pdf文档中
        /// </summary>
        /// <param name="fileName">pdf文档完整路径</param>
        /// <param name="htmlText">html文档</param>
        /// <returns></returns>
        private static Result convertHtmlTextToPDF(string fileName, string htmlText)
        {
            Result model = new Result();
            try
            {
                MemoryStream outputStream = new MemoryStream();
                byte[] data = Encoding.UTF8.GetBytes(htmlText);
                MemoryStream msInput = new MemoryStream(data);
                Document doc = new Document();

                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(fileName, FileMode.Create));
                PdfDestination pdfDest = new PdfDestination(PdfDestination.XYZ, 0, doc.PageSize.Height, 1f);
                doc.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, msInput, null, Encoding.UTF8, new PDFChineseFontFactory());
                PdfAction action = PdfAction.GotoLocalPage(1, pdfDest, writer);
                writer.SetOpenAction(action);
                doc.Close();
                msInput.Close();
                outputStream.Close();

                model.Flag = true;
                model.Message = fileName;
            }
            catch (Exception ex)
            {
                model.Flag = false;
                model.Message = ex.Message;
            }
            return model;
        }


        /// <summary>
        /// 将html文本导出pdf
        /// </summary>
        /// <param name="fileName">pdf文件路径</param>
        /// <param name="htmlText">html文本</param>
        /// <param name="watermarkImgURL">水印图片，可选，如存在，必须是png格式</param>
        /// <param name="absoluteX">水印图片在pdf中x轴位置，默认右下角</param>
        /// <param name="absoluteY">水印图片在pdf中y轴位置，默认右下角</param>
        /// <returns></returns>
        public static Result ExportPDF(string fileName, string htmlText, string watermarkImgURL = "", float absoluteX = 0, float absoluteY = 0)
        {
            Result model = new Result();
            try
            {
                model.Flag = false;
                htmlText = "<p>" + htmlText + "</p>";
                if (string.IsNullOrEmpty(htmlText))
                {
                    model.Message = "html内容不能为空";
                }
                else if (!fileName.EndsWith(".pdf"))
                {
                    model.Message = "指定的文件不是PDF格式";
                }
                else
                {
                    model = convertHtmlTextToPDF(fileName, htmlText);
                    if ((!string.IsNullOrEmpty(watermarkImgURL)) && model.Flag)
                    {
                        if (!watermarkImgURL.EndsWith(".png"))
                        {
                            model.Flag = false;
                            model.Message = "水印图片必须是PNG格式";
                        }
                        else
                        {
                            model = watermarkPDF(fileName, watermarkImgURL, absoluteX, absoluteY);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                model.Flag = false;
                model.Message = ex.ToString();
            }
            return model;
        }
        /// <summary>
        /// pdf文档添加水印图
        /// </summary>
        /// <param name="fileName">pdf文档</param>
        /// <param name="watermarkImgURL">水印图片路径</param>
        /// <param name="absoluteX">水印图片在pdf中x轴位置，默认右下角</param>
        /// <param name="absoluteY">水印图片在pdf中y轴位置，默认右下角</param>
        /// <returns></returns>
        private static Result watermarkPDF(string fileName, string watermarkImgURL, float absoluteX = 0, float absoluteY = 0)
        {
            Result model = new Result();
            try
            {
                if (File.Exists(watermarkImgURL))
                {


                    string temp = "_watermark";
                    string parentPath = System.IO.Path.GetDirectoryName(fileName);
                    string oldFileName = System.IO.Path.GetFileNameWithoutExtension(fileName);
                    string newFileName = parentPath + oldFileName + temp + ".pdf";

                    using (Stream inputPdfStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    using (Stream inputImageStream = new FileStream(watermarkImgURL, FileMode.Open, FileAccess.Read, FileShare.Read)) //水印图片

                    using (Stream outputPdfStream = new FileStream(newFileName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        var reader = new PdfReader(inputPdfStream);
                        var stamper = new PdfStamper(reader, outputPdfStream);
                        int n = reader.NumberOfPages;    //获取页码    
                        var pdfContentByte = stamper.GetOverContent(n);
                        Image image = Image.GetInstance(inputImageStream);//插入图片
                        image.ScaleAbsolute(140F, 140F);

                        if (absoluteX == 0)
                        {
                            absoluteX = pdfContentByte.PdfDocument.Right / 2 + 50;
                        }
                        if (absoluteY == 0)
                        {
                            absoluteY = float.Parse(Element.ALIGN_BOTTOM.ToString()) + 80;
                        }
                        image.SetAbsolutePosition(absoluteX, absoluteY); //图标坐标
                        pdfContentByte.AddImage(image);
                        stamper.Close();
                    }
                    //将打水印的文件重命名回去，删除原文件
                    File.Delete(fileName);
                    Directory.Move(newFileName, fileName);

                    model.Flag = true;
                    model.Message = fileName;
                }
                else
                {
                    model.Flag = false;
                    model.Message = "水印图片不存在";
                }
            }
            catch (Exception ex)
            {
                model.Flag = false;
                model.Message = ex.ToString();
            }

            return model;
        }
        public class PDFChineseFontFactory : FontFactoryImp
        {
            public override Font GetFont(string fontname, string encoding, bool embedded, float size, int style, BaseColor color,
                bool cached)
            {
                var soneFile = "";
                try
                {
                    soneFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                   "SIMSUN.TTC,1");
                }
                catch
                {
                    soneFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts),
                   "MSYH.TTC,1");
                }
                BaseFont baseFont = BaseFont.CreateFont(soneFile, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                return new Font(baseFont, size, style, color);
            }
        }

        public class Result
        {
            public bool Flag { get; set; }
            public string Message { get; set; }
        }
    }
}
