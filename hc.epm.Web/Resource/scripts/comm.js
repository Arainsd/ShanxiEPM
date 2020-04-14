/*
公共方法提取
*/

//弹出层顶部图标
/*
*弹出新增修改详情页面返回页面tittle
*参数说明：接受一个参数为 弹出iframe页面title
*/
function GetLayerTitle(text) {
    return '<img src="/Resource/images/icon-left-user.png" style="margin-right:15px;vertical-align:middle"/><span style="color:#1286a4;font-size:16px;vertical-align:middle">' + text + '</span>';
}

/*
*附件上传筛选附件方法
*参数说明：附件格式  文件对象
*/
function File(typeList, obj) {
    var isType = false;
    var types = obj.name.split(".");
    var type = types[types.length - 1];
    if (typeList.indexOf(type) > -1) {
        isType = true;
    } else {
        isType = false;
    };
    return isType
};


/*
*获取URL参数
*eg：getUrlParam('name')
*参数说明：接受一个参数，为获取URL中参数名
*/
function getUrlParam(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)"); //构造一个含有目标参数的正则表达式对象
    var r = window.location.search.substr(1).match(reg); //匹配目标参数
    if (r != null) return unescape(r[2]);
    return null; //返回参数值
}

/*
*列表内容超过长度截取鼠标悬浮提示方法
*eg:subStrTips();
*参数说明：不接受参数（调用此方法必须依赖layUI的layer组件）
*/
function subStrTips() {
    var tips = "";
    $(".tips").on("mouseover", $("table"), function () {
        tips = layer.tips(this.getAttribute("tips"), this, { time: 10000000, tips: [2, "#2E2929"] });
    });
    $(".tips").on("mouseout", $("table"), function () {
        layer.close(tips);
    })
}
/*
*时间格式化函数
 * 格式化日期，返回字符串 默认格式：yyyy-MM-dd HH:mm:ss
* @param {} time 需要处理的时间字符串eg：/Date(1354116249000)/（从C#的Datatime格式通过Json传到Js里面的） 
* @returns {} 返回日期时间字符串
*/
function formatDateByJson(timeString) {

    if (timeString == null || timeString == "")
        return "";
    if (timeString.replace("/Date(", "").replace(")/", "") == timeString)
        return "";
    if (new Date(parseInt(timeString.replace("/Date(", "").replace(")/", ""))) == "Invalid Date")
        return "";

    var now = new Date(parseInt(timeString.replace("/Date(", "").replace(")/", "")));
    var year = now.getFullYear();
    var month = now.getMonth() + 1;
    var date = now.getDate();
    var hour = now.getHours();
    var minute = now.getMinutes();
    var second = now.getSeconds();
    return year + "-" + month + "-" + date + " " + hour + ":" + minute + ":" + second;
}

// 对Date的扩展，将 Date 转化为指定格式的String
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符， 
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字) 
// 例子： 
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423 
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18 
Date.prototype.Format = function (fmt) { //author: meizz 
    var o = {
        "M+": this.getMonth() + 1, //月份 
        "d+": this.getDate(), //日 
        "h+": this.getHours(), //小时 
        "m+": this.getMinutes(), //分 
        "s+": this.getSeconds(), //秒 
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度 
        "S": this.getMilliseconds() //毫秒 
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}

//开始时间和结束时间差函数
function DateDiff(sDate1, sDate2) {  //sDate1和sDate2是yyyy-MM-dd格式
    var aDate, oDate1, oDate2, iDays;
    aDate = sDate1.split("-");
    oDate1 = new Date(aDate[1] + '-' + aDate[2] + '-' + aDate[0]);  //转换为yyyy-MM-dd格式
    aDate = sDate2.split("-");
    oDate2 = new Date(aDate[1] + '-' + aDate[2] + '-' + aDate[0]);
    iDays = parseInt(Math.abs(oDate1 - oDate2) / 1000 / 60 / 60 / 24); //把相差的毫秒数转换为天数
    return iDays;  //返回相差天数
}

/// <summary>
/// 判断是否是图片
/// </summary>
function IsImage(ext) {
    var exts = new Array("bmp", "jpeg", "jpg", "gif", "png");
    if (ext.indexOf('.') != -1) {
        ext = ext.substring(1);
    }
    return $.inArray(ext, exts) >= 0;
}

/**
 * 根据图片名称获取图片类型
 * @param {name} 图片名称
 * @param {ext} 文件扩展名
 */
function getImageType(name, ext) {
    name = name || '';
    if (name === '') {
        return '';
    }
    var imageType = '';
    if (IsImage(ext)) {
        if (name.indexOf("epmbig") === 0) {
            imageType = 'big';
        }
        else if (name.indexOf('epmsmall') === 0) {
            imageType = 'small';
        } else {
            imageType = 'start';
        }
    }
    return imageType;
}

function isThumbnailImage(type) {
    type = type || '';
    switch (type) {
        case 'start':
            return false;
        case 'big':
            return true;
        case 'small':
            return true;
        default:
            return false;
    }
}

function getFileImg(name) {
    var arr = name.split(".");
    var s = ("." + arr[arr.length - 1]).toLowerCase();
    switch (s) {
        case ".doc":
        case ".docx":
            return "/Resource/images/word.png";
        case ".xls":
        case ".xlsx":
            return "/Resource/images/excel.png";
        case ".ppt":
        case ".pptx":
            return "/Resource/images/ppt.png";
        case ".pdf":
            return "/Resource/images/flash.png";
        case ".rar":
        case ".zip":
        case ".7z":
            return "/Resource/images/rar.png";
        case ".wav":
        case ".mp3":
        case ".ogg":
        case ".acc":
            return "/Resource/images/mp3.png";
        case ".png":
        case ".jpg":
        case ".jpeg":
        case ".gif":
            return "/Resource/images/jpg.png";
        case ".avi":
        case ".rmvb":
        case ".rm":
        case ".wmv":
        case ".3gp":
        case ".mpg":
        case ".mkv":
        case ".mp4":
        case ".mpeg4":
            return "/Resource/images/rmvb.png";
        case ".dwg":
            return "/Resource/images/dwg.png";
        default:
            return "";
    }
}

//截取文件名
function substrParams(Str, _length) {
    var arr = Str.split(".");
    var strArr = [];
    for (var i = 0; i < arr.length - 1; i++) {
        strArr.push(arr[i]);
    }
    var _str = strArr.toString();
    _str = (_str.length > _length) ? (_str.substring(0, _length) + "...") : _str;
    var _params = _str + '.' + arr[arr.length - 1];
    return _params
}

//附件上传文件大小单位换算
function fileSizeConver(filesize) {
    var key = filesize.replace(/[^a-zA-Z]/g, '');
    var value = filesize.replace(key, '');
    var result = "";
    switch (key) {
        case "B":
            result = value;
            break;
        case "KB":
            result = value * 1024;
            break;
        case "M":
            result = value * 1024 * 1024;
            break;
        case "GB":
            result = value * 1024 * 1024 * 1024;
            break;
        default:
            break;
    }
    return result;
}


/************************************************/

//// 手机号码验证
//jQuery.validator.addMethod("mobile", function (value, element) {
//    var length = value.length;
//    var mobile = /^((1[0-10]{1})+\d{8})$/
//    return this.optional(element) || (length == 11 && mobile.test(value));
//}, "手机号码格式错误");

/************************************************/

$.extend({
    /**
     * 按筛选条件导出数据到 excel
     * @param {} dataUrl 获取数据源的 url 地址，包含查询参数
     * @param {} fileName 导出的文件名称
     * @returns {} 
     */
    exportToExcel: function (dataUrl, fileName) {
        var f = $('<form action="' + dataUrl + '" method="post" id="fm1"><input type="hidden" id="fileName" name="fileName" value="' + fileName + '" /></form>');
        f.appendTo(document.body).submit();
        document.body.removeChild(f.get(0));
        return;
    }
});