/*
公共方法提取
*/

/*
*后台管理弹出新增修改详情页面返回页面tittle
*eg: GetLayerTitle("组织机构")
*参数说明：接受一个参数为 弹出iframe页面title
*/
function GetLayerTitle(text) {
    return '<img src="/Images/icon-left-user.png" style="margin-right:15px;vertical-align:middle"/><span style="color:#1286a4;font-size:16px;vertical-align:middle">' + text + '</span>';
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
*返回后台列表页面启用禁用、确认状态弹出提示语
*eg:GetIsEnableLayerMes("True，type")
*参数说明：接受两个参数 参数一：当前数据的启用禁用值
*                       参数二：修改类型 1为启用禁用 2为确认未确认（****此参数必须按此填写***）
*/
function GetIsEnableLayerMes(text, type) {
    debugger;
    if (type == "2") {
        if (text == false) {
            return "确认要禁用所选择的项？"
        } else if (text == "False") {
            return "确认要启用所选择的项？"
        } else if (text == "True") {
            return "确认要禁用所选择的项？"
        } else {
            return "确认要启用所选择的项？"
        }
    } else if (type == "3") {
        return "确认修改所选择的项的确认状态吗？"
    } else {
        if (text == true) {
            return "确认所选择的资料为必填资料？"
        } else {
            return "确认所选择的资料不是必填资料？"
        }
    }
}

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
    return year + "-" + month + "-" + date + "   " + hour + ":" + minute + ":" + second;

}

/**
 * 判断是否是图片
 * @param {any} ext
 */
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