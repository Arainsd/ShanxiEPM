/*
 * author:wmg
 * date:2020-03-16
 * desc:简单封装一些常用的公共函数
 */

;
(function ($) {
    $.epm = {
        /**
         * 选择库站弹窗
         * @param {any} options 库站弹窗相关参数
         */
        openSelectStation: function (options) {
            var defaults = {
                width: '800px',
                height: '600px',
                yes: function () {

                },
                cancel: function () {

                }
            };
            var option = $.extend({}, defaults, options || {});
            $.epm.openFrameDialog({
                title: '请选择库站',
                area: [option.width, option.height],
                content: '/Comm/SelectOilStation',
                yes: function (index, layero) {
                    if (typeof (option.yes) === "function") {
                        var obj = frames['layui-layer-iframe' + index].getSelectData();
                        if (obj.flag === false) {
                            layer.alert(obj.msg, { icon: 2 });
                        } else {
                            option.yes.call(this, index, obj.data[0]);
                        }
                    }
                },
                cancel: function () {
                    option.cancel();
                }
            });
        },

        /**
         * 选择公司弹窗
         * @param {any} options 选择公司弹窗参数
         */
        openSelectCompany: function (options) {
            var defaults = {
                title: '选择分公司',
                width: '800px',
                height: '600px',
                singleSelect: false,
                yes: function () {

                },
                cancel: function () {

                }
            };
            var url = '/Comm/SelectBranchCompany';
            var option = $.extend({}, defaults, options || {});
            if (option.singleSelect === true) {
                url = url + '?selectType=' + selectType;
            }
            $.epm.openFrameDialog({
                title: option.title,
                area: [option.width, option.height],
                content: url,
                yes: function (index, layero) {
                    if (typeof (option.yes) === "function") {
                        var obj = frames['layui-layer-iframe' + index].getSelectData();
                        if (obj.flag === false) {
                            layer.alert(obj.msg, { icon: 2 });
                        } else {
                            option.yes.call(this, index, obj.data[0]);
                        }
                    }
                },
                cancel: function () {
                    option.cancel.call(this);
                }
            });
        },


        openSelectPeople: function (options) {
            var defaults = {
                title: '选择申报人',
                width: '800px',
                height: '600px',
                singleSelect: false,
                companyId: '',
                yes: function () {

                },
                cancel: function () {

                }
            };
            var url = '/Comm/SelectUser';
            var option = $.extend({}, defaults, options || {});
            if (option.singleSelect === true) {
                url = url + '?selectType=' + 2;
            }
            if (option.companyId !== '') {
                url = url + '&companyId=' + option.companyId;
            }
            $.epm.openFrameDialog({
                title: option.title,
                width: option.width,
                height: option.height,
                url: url,
                yes: function (index, layero) {
                    if (typeof (option.yes) === "function") {
                        var obj = frames['layui-layer-iframe' + index].getSelectData();
                        if (obj.flag === false) {
                            layer.alert(obj.msg, { icon: 2 });
                        } else {
                            option.yes.call(this, index, obj.data[0]);
                        }
                    }
                },
                cancel: function () {
                    option.cancel.call(this);
                }
            });
        },

        openFrameDialog: function (options) {
            var defaults = {
                title: '',
                width: '800px',
                height: '600px',
                url:'',
                yes: function () {

                },
                cancel: function () {

                }
            };
            var option = $.extend({}, defaults, options || {});
            if (option.url === '') {
                layer.msg('请指定弹窗要加载的页面路径！', { icon: 2 });
                return;
            }
            return layer.open({
                type: 2,
                title: GetLayerTitle(option.title),
                shadeClose: false,
                area: [option.width, option.height],
                content: option.url,
                move: false,
                shade: 0.3,
                btn: ["确定", "取消"],
                yes: function (index, layero) {
                    if (typeof (option.yes) === "function") {
                        option.yes.call(this, index, layero);
                    }
                },
                btn2: function (index, layero) {
                    if (typeof (option.cancel) === "function") {
                        option.cancel.call(this, index, layero);
                    } 
                },
                cancel: function (index, layero) {
                    parent.layer.close(index);
                }
            });
        }
    };
})(jQuery);