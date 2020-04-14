var PageRowNum = 10;
var LargeLayerWidth = "950px";//三列弹出框宽度
var BigLayerWidth = "800px"; //layer大弹出层宽度
var SmallLayerWidth = "480px";////layer小弹出层宽度

var FileURL = "http://192.168.1.239:8085";
var FileContinueURL = FileURL + "/home/IsContinue";
var FileServerURL = FileURL + "/home/Upload";//附件
var FileDownLoadURL = FileURL + "/home/download?guid=";//附件下载
var FileResourceURL = FileURL + "/home/UploadResource";//传输静态资源
var MaxFileSize = "100mb";
var ChunkSize = "20mb";
var MaxPicSize = "20mb";//上传图片大小限制

var SimpleFileResultURL = "http://192.168.1.239:8086/Home/UESimpleFile";
//var SimpleFileResultURL = "http://10.206.129.185:1112/Home/UESimpleFile";
