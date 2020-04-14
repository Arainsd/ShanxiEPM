/**
 * 结合当前项目原有代码，基于 plupload 上传插件进行的简单二次封装
 * 以尽量减少改动原有代码为目标。
 * 
 * 2020-03-27 wmg
 * 
 * 使用方法：
 *      初始化：$('XXX').InitUploader()，可设置指定参数;
 *      获取附件最终上传结果：$('XXX').InitUploader('getData')，返回数组格式。
 *      初始化赋值：$('XXX').InitUploader('setData', [])
 */

; (function ($, window, document, underfined) {
    var methods = {
        init: function (option) {
            var $this = $(this),
                options = $this.data('InitUploader'),
                mime_types = [];

            if (typeof (options) == 'undefined') {
                options = $.extend({}, $.fn.InitUploader.defaultOptions, option || {});
                $this.data('InitUploader', options);
            } else {
                options = $.extend({}, options, option);
            }

            if (options.isImage) {
                mime_types = [
                    { title: 'Image files', extensions: 'jpg,gif,png,png,bmp' }
                ]
            } else {
                mime_types = [
                    { title: 'All files', extensions: options.mimeTypes || '*' }
                ]
            }

            var uploader = new plupload.Uploader({
                browse_button: options.browerButton, //触发文件选择对话框的按钮，为那个元素id
                url: options.url, //服务器端的上传页面地址
                multi_selection: options.multiSelection,
                filters: {
                    mime_types: mime_types,
                    max_file_size: options.maxFileSize,
                    prevent_duplicates: true
                },
                multipart_params: options.params,
                chunk_size: options.chunkSize,
                flash_swf_url: 'Resource/scripts/plupload_2_1_2/Moxie.swf', //swf文件，当需要使用swf方式进行上传时需要配置该参数
            });

            uploader.init();

            // 当附件添加到上传队列后
            uploader.bind('FilesAdded', function (uploader, files) {
                if (options.beforeUpload.call()) {
                    if (options.fileNumLimit > 0) {
                        if (files.length > options.fileNumLimit) {
                            layer.msg('已上传附件超过系统设置的最大值。系统最大允许附件数为' + options.fileNumLimit);
                            uploader.removeFile(files[files.length - 1]);
                            return false;
                        }
                    }
                    // 立即开始上传
                    uploader.start();
                }
            });

            // 当附件开始上传前
            uploader.bind('BeforeUpload', function (uploader, file) {
                layer.load(2, { shade: 0.3, })
            });

            // 附件上传进度条
            uploader.bind('UploadProgress', function (uploader, file) {

            });

            // 当附件上传失败时
            uploader.bind('Error', function (uploader, errObject) {
                layer.closeAll('loading');
                switch (errObject.code) {
                    case -200:
                        layer.msg('附件上传失败！网络错误！');
                        return false;
                    case -300:
                        layer.msg('附件上传失败！写入磁盘失败！');
                        return false;
                    case -600:
                        layer.msg('附件上传失败！当前所选文件大小超过系统设置的最大值，请压缩后重新上传！');
                        return false;
                    case -601:
                        layer.msg('附件上传失败！当前所选文件类型不在指定类型范围内！');
                        return false;
                    case -602:
                        layer.msg('附件上传失败！当前所选文件在已上传列表中已存在！');
                        return false;
                    case -700:
                        layer.msg('附件上传失败！当前所选图片格式不正确！');
                        return false;
                    default:
                        layer.msg('附件上传失败！系统位置错误，错误代码' + errObject.code) + "！";
                        return false;
                }
            })

            // 附件上传成功
            uploader.bind('FileUploaded', function (uploader, file, responseObject) {
                // todo: 上传成功后处理时间，在这里将附件在列表显示
                if (responseObject.status == 200) {
                    var responseResult = JSON.parse(responseObject.response);

                    var data = responseResult;
                    if (data instanceof Array) {
                        data = data[0];
                    }
                    options.uploadSuccess(uploader, data, file);
                }
                layer.closeAll('loading');
            })

            return true;
        },
        getData: function () {
            var $this = $(this),
                options = $this.data('InitUploader'),
                isUploadToTz = options.isUploadToTz || false,
                returnData = [];

            var $input = $(options.listContainer).find('input[type="hidden"][class="updata"]');
            if ($input.length > 0) {
                $.each($input, function (index, obj) {
                    var size = $(obj).data('size') || '';
                    var data = {
                        GuidId: $(obj).data('id') || '',
                        Url: $(obj).data('url') || '',
                        Name: $(obj).data('name') || '',
                        Size: $(obj).data('size') || '',
                        Group: $(obj).data('group') || '',
                        TypeNo: $(obj).data('typeno') || '',
                        TypeName: $(obj).data('typename') || '',
                        Sort: index + 1 || '',
                        Uploadtype: ''
                    };
                    if (isUploadToTz) {
                        data.Uploadtype = parseFloat(tzmaxfilesize) < parseFloat(fileSizeConver(size)) ? 2 : 1
                    }
                    returnData.push(data);
                });
            }

            return returnData;
        },
        setData: function (data) {
            return this.each(function () {
                var $this = $(this),
                    options = $this.data('InitUploader');
                if (typeof (data) != 'undefined') {
                    if (data instanceof Array) {
                        $.each(data, function (index, item) {
                            options.uploadSuccess(item);
                        })
                    } else {
                        options.uploadSuccess(item);
                    }
                }
            });
        }
    };

    $.fn.InitUploader = function () {
        var method = arguments[0];
        if (methods[method]) {
            method = methods[method];
            arguments = Array.prototype.slice.call(arguments, 1);
        } else if (typeof method === 'object' || !method) {
            method = methods.init;
        } else {
            $.error('Method' + method + ' does not exist on jQuery.InitUploader.');
            return this;
        }
        return method.apply(this, arguments);
    }


    /*
     * @browerButton:附件选择触发按钮
     * @url:附件上传地址
     * @mimeTypes:附件上传类型
     * @maxFileSize:附件大小限制
     * @chunkSize:附件分片上传每片大小
     * @isImage:是否只允许上传附件，如果是 mimeTypes 参数不起作用
     * @multiSelection:是否允许在文件浏览兑换卡中同时选择多个附件
     * @container：
     * @listContainer:附件上传成功后创建 html 代码的父容器
     * @isUploadToTz:是否上传到投资系统，默认否
     * @params:其他自定义参数，object 类型，例如：{key:1,key1:2}
     * @fileNumLimit:附件上传个数，默认不限制
     * @beforeUpload:在附件上传之前需要处理的事情，如果返回 true，会继续执行附件上传操作
     * @getFileType:获取附件类型，附件上传成功后，通过该函数获取该附件指定的上传类型
     * @uploadSuccess:附件上传成功后的回调函数
     * */
    $.fn.InitUploader.defaultOptions = {
        browerButton: 'btnUploadFile',
        url: '/Upload/UploadHB',
        mimeTypes: '',
        maxFileSize: '1024KB',
        chunkSize: '0',
        isImage: false,
        multiSelection: false,
        container: '',
        listContainer: '.fileShow-append',
        isUploadToTz: true,
        params: {

        },
        fileNumLimit: 0,
        getFileType: function () {

        },
        beforeUpload: function () {
            return true;
        },
        uploadSuccess: function (uploader, data, file) {
            var $this = this,
                tepmIndex = $($this.listContainer).find("tr").length - 1,
                isUploadToTz = $this.isUploadToTz,
                fileTypeData = $this.getFileType(),
                typeno = '',
                typename = '';

            if (typeof fileTypeData == "object") {
                typeno = fileTypeData.typeno || '';
                typename = fileTypeData.typename || '';
            }

            //附件归属
            var hidInput =
                `<input type="hidden" class="updata" id="updata-${data.GuidId}" data-id="${data.GuidId}" data-url="${data.Url}"
                data-src="${getFileImg(data.Name)}" data-name="${data.Name}" data-size="${data.Size}" data-group="${data.Group}"
                data-typeno="${typeno}" data-typename="${typename}">`,

                $lastTd = $('<td><a class ="fileDel" href="javascript:void(0)" style="color:#337ab7;">删除</a></td>').append(hidInput),
                $tr = $('<tr></tr>').attr('id', data.GuidId)
                    .append('<td><span class ="sort">' + tepmIndex + '</span></td>')
                    .append('<td class ="text_lf"><span>' + data.Name + '</td>')
                    .append('<td><span>' + typename + '</td>');
            tepmIndex++;

            // 是否上传到投资系统
            if (isUploadToTz) {
                var $checkbox = ''
                var isChecked = parseFloat(tzmaxfilesize) < parseFloat(fileSizeConver(data.Size));
                if (isChecked) {
                    $checkbox = '<input type="checkbox" name="Uploadtype" disabled="disabled" />'
                } else {
                    $checkbox = '<input type="checkbox" name="Uploadtype" checked="checked" />'
                }
                $label = $('<label class ="checkbox-inline"></label>').html($checkbox + '上传到投资系统');
                $tr.append($('<td></td>').append($label));
            }
            $tr.append($lastTd);
            $($this.listContainer).append($tr);
            $($this.listContainer).on('click', '.fileDel', function () {
                var $that = this;
                uploader.removeFile(file);
                $($that).parent().parent().remove();
            })
        }
    }
})(jQuery, window, document);