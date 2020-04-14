var PageRowNum = 10;
var LargeLayerWidth = "950px";//三列弹出框宽度
var BigLayerWidth = "800px"; //layer大弹出层宽度
var SmallLayerWidth = "480px";////layer小弹出层宽度

var FileURL = "http://192.168.1.239:8086";
var FileContinueURL = FileURL + "/home/IsContinue";
var FileServerURL = FileURL + "/home/Upload";//附件 .doc|.docx|.xls|.xlsx|.rar|.iso|.zip|.7z|.pdf|.*  .mp3|.avi|.rmvb|.wmv|.rm
var FileDownLoadURL = FileURL + "/home/download?guid=";//附件下载
var MaxFileSize = "100mb";
var ChunkSize = "10mb";
var MaxPicSize = "2mb";//上传图片大小限制

var BimFileURL = "http://192.168.1.239:8088";

//http://210.12.193.4:6014/
var WebAPIURL = "http://10.22.142.161:6014/";
var maxfilesize = "";
var tzmaxfilesize = "52428800";//投资附件最大50M