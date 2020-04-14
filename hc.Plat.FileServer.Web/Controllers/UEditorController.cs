using hc.Plat.FileServer.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace hc.Plat.FileServer.Web.Controllers
{

    public class UEditorController : Controller
    {

        [CORSActionFilter]
        public void Index()
        {
            string action = Request.Params["action"];
            Handler hand = null;
            var context = HttpContext;
            switch (action)
            {
                case "config":
                    hand = new ConfigHandler(context);
                    break;
                case "uploadimage":
                    hand = new UploadHandler(context, new UploadConfig()
                    {
                        AllowExtensions = EditorConfig.GetStringList("imageAllowFiles"),
                        PathFormat = EditorConfig.GetString("imagePathFormat"),
                        SizeLimit = EditorConfig.GetInt("imageMaxSize"),
                        UploadFieldName = EditorConfig.GetString("imageFieldName")
                    });
                    break;
                case "uploadscrawl":
                    hand = new UploadHandler(context, new UploadConfig()
                    {
                        AllowExtensions = new string[] { ".png" },
                        PathFormat = EditorConfig.GetString("scrawlPathFormat"),
                        SizeLimit = EditorConfig.GetInt("scrawlMaxSize"),
                        UploadFieldName = EditorConfig.GetString("scrawlFieldName"),
                        Base64 = true,
                        Base64Filename = "scrawl.png"
                    });
                    break;
                case "uploadvideo":
                    hand = new UploadHandler(context, new UploadConfig()
                    {
                        AllowExtensions = EditorConfig.GetStringList("videoAllowFiles"),
                        PathFormat = EditorConfig.GetString("videoPathFormat"),
                        SizeLimit = EditorConfig.GetInt("videoMaxSize"),
                        UploadFieldName = EditorConfig.GetString("videoFieldName")
                    });
                    break;
                case "uploadfile":
                    hand = new UploadHandler(context, new UploadConfig()
                    {
                        AllowExtensions = EditorConfig.GetStringList("fileAllowFiles"),
                        PathFormat = EditorConfig.GetString("filePathFormat"),
                        SizeLimit = EditorConfig.GetInt("fileMaxSize"),
                        UploadFieldName = EditorConfig.GetString("fileFieldName")
                    });
                    break;
                case "listimage":
                    hand = new ListFileManager(context, EditorConfig.GetString("imageManagerListPath"), EditorConfig.GetStringList("imageManagerAllowFiles"));
                    break;
                case "listfile":
                    hand = new ListFileManager(context, EditorConfig.GetString("fileManagerListPath"), EditorConfig.GetStringList("fileManagerAllowFiles"));
                    break;
                case "catchimage":
                    hand = new CrawlerHandler(context);
                    break;
                default:
                    hand = new NotSupportedHandler(context);
                    break;
            }
            hand.Process();
        }


    }
}