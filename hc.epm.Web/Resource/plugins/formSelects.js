/**
 * name: formSelects
 * 基于Layui Select多选
 * version: 2.5.3
 * https://faysunshine.com/layui/template/formSelects.js
 */
(function(layui, window, factory) {
	if(typeof exports === 'object') { // 支持 CommonJS
		module.exports = factory();
	} else if(typeof define === 'function' && define.amd) { // 支持 AMD
		define(factory);
	} else if(window.layui && layui.define) { //layui加载
		if(layui.form && layui.jquery){
			window.formSelects = factory();
		}
		layui.define(function(exports) {
			exports('formSelects', window.formSelects ? window.formSelects : factory());
		});
	} else {
		window.formSelects = factory();
	}
})(layui, this, function() {
	var $ = layui.jquery || $,
		form = layui.form;
	var obj = {
		array: function(name) {
			return commons.array[name] ? commons.array[name].data : [];
		},
		selects: function(options) {
			if(!layui.jquery && !$) {
				commons.log(' %c 需要引入jquery模块, 或单独使用jquery', 'color: red;');
				return;
			}
			commons.array[options.name] = {
				data: []
			}
			
			commons.array[options.name].options = options = $.extend({}, commons.STANDARD_OPTIONS, options);
			commons.log("当前配置: ", options);
			commons.log("已配置数据配置: ", commons.array);
			form.render(options.model, options.filter);
			var res = commons.init(commons.array[options.name].data, options);
			if(res == 1) {
				return;
			}
			form.on(`${options.model}(${options.filter})`, function(data) {
				var dataArr = commons.array[options.name].data;
				if(options.model == 'checkbox') {
					if(data.elem.checked) {
						commons.push(dataArr, data.othis);
					} else {
						commons.remove(dataArr, commons.findval(data.othis));
					}
				} else {
					if(!data.value) {
						commons.selectShow(dataArr, options);
						return;
					}
					var $info = $(data.othis).find(`dd[lay-value=${data.value}]`);
					var val = commons.findval($info);
					if(commons.remove(dataArr, val)) { //删除成功

					} else {
						commons.push(dataArr, $info);
					}
					options.$dom.find('dl').css('display', 'block');
					commons.selectShow(dataArr, options);
				}
				commons.show(dataArr, options);
				if(options.change && typeof options.change == 'function') {
					options.change(data, dataArr)
				}
			});
		},
		value: function(name, vals) {
			if(!name) {
				return;
			}
			var option = commons.array[name];
			if(option && vals && vals instanceof Array) {
				var options = option.options;
				var dataArr = [];
				if(options.$dom) {
					options.$dom.find('dd').removeClass('layui-this');
					for(var i in vals) {
						var val = vals[i];
						var $dd = options.$dom.find(`dd[lay-value=${val}]`);
						$dd.addClass('layui-this');
						commons.push(dataArr, $dd);
					}
				} else {
					var $options = $(options.el);
					$options.parent().find('.layui-form-checked').removeClass('layui-form-checked');
					for(var i in vals) {
						var val = vals[i];
						var $input = $options.parent().find(`input[type=checkbox][value=${val}]`).next();
						$input.addClass('layui-form-checked');
						commons.push(dataArr, $input);
					}
				}
				option.data = dataArr;
				commons.show(dataArr, options);
				commons.selectShow(dataArr, options);
			}else{
				var arr = commons.array[name] ? commons.array[name].data : [];
				for(var i in arr){
					arr[i] = arr[i].val;
				}
				return arr;
			}
		},
		debug: function(flag) {
			commons.isDebug = true && flag;
		},

	};

	var commons = {
		STANDARD_OPTIONS: {
			model: 'select',
			filter: '',
			show: '',
			left: '【',
			right: '】',
			separator: '',
			reset: false,
			change: null,
			init: null,
		},
		array: {},
		clicks: {},
		isDebug: false,
		init: function(arr, options) {
			var checks, $option = $(options.el);

			if(options.init && options.init instanceof Array) {
				if($option.is('input')) {
					$option.removeAttr('checked');
				} else {
					$option.find('option').removeAttr('selected');
				}
				for(var i in options.init) {
					var val = options.init[i];
					if($option.is('input')) {
						for(var input in $option) {
							if($(input).val() == val) {
								$(input).attr('checked', 'checked');
							}
						}
					} else if($option.is('select')) {
						$option.find(`option[value=${val}]`).attr('selected', 'selected');
					}
				}
			}

			if($option.is('input')) {
				checks = $option.parent().find('.layui-form-checked');
			} else if($option.is('select')) {
				var $dom = options.$dom = $option.next();
				$dom.find('dl').css('display', 'none');
				$(document).off('click', `${options.el} + div input,${options.el} + div i`).on('click', `${options.el} + div input,${options.el} + div i`, function() {
					var opt = commons.array[options.name];
					commons.selectShow(opt.data, opt.options);
				});
				$(document).off('click', `body:not(${options.el} + div)`).on('click', `body:not(${options.el} + div)`, function(e) {
					var showFlag = $(e.target).parents('.layui-form-select').prev().is($option);
					var thisFlag = $dom.find('dl').css('display') == 'block';
					if(showFlag) { //点击的input框
						$dom.find('dl').css('display', thisFlag ? 'none' : 'block');
					} else {
						if(thisFlag) {
							$dom.find('dl').css('display', 'none');
						}
					}
				});
				checks = $option.find('option[selected]');
			} else {
				commons.log('传入的配置错误, 无法识别select或input[checkbox]', options);
				return 1;
			}

			//监听reset, 处理
			var resetBtn = $option.parents('form').find('button[type=reset]');
			if(options.reset && resetBtn.length) {
				commons.resetone(resetBtn, options);
			}
			for(var i = 0; i < checks.length; i++) {
				commons.push(arr, checks[i]);
			}
			commons.log('初始化已选择元素: ', arr);
			commons.show(arr, options);
			if($option.is('select')) {
				commons.selectShow(arr, options);
			}
			return 0;
		},
		resetone: function(target, options) {
			$(target).one('click', function(e) {
				setTimeout(function() {
					obj.selects(options);
				}, 200);
			});
		},
		findval: function(option) {
			var $option = $(option);
			return $option.is('dd') ? {
				name: $option.text(),
				val: $option.attr('lay-value')
			} : ($option.is('option') ? {
				name: $option.text(),
				val: $option.attr('value')
			} : {
				name: $option.find('span').text(),
				val: $option.prev('input').val()
			});
		},
		push: function(arr, option) {
			arr.push(commons.findval(option));
		},
		show: function(arr, options) {
			if(options.show) {
				$(options.show).text(JSON.stringify(arr));
			} else {
				commons.log(arr);
			}
		},
		selectShow: function(arr, options) {
			options.$dom.find('.layui-this').removeClass('layui-this');
			var input_val = '';
			for(var i in arr) {
				var obj = arr[i];
				input_val += options.separator + options.left + obj.name + options.right;
				options.$dom.find(`dd[lay-value='${obj.val}']`).addClass('layui-this');
			}
			if(options.separator && options.separator.length > 0 && input_val.startsWith(options.separator)) {
				input_val = input_val.substr(options.separator.length);
			}
			options.$dom.find('.layui-select-title input').val(input_val);
		},
		indexOf: function(arr, val) {
			for(var i = 0; i < arr.length; i++) {
				if(arr[i] == val || JSON.stringify(arr[i]) == JSON.stringify(val)) {
					return i;
				}
			}
			return -1;
		},
		remove: function(arr, val) {
			var index = commons.indexOf(arr, val);
			if(index > -1) {
				arr.splice(index, 1);
				return true;
			}
			return false;
		},
		log: function(message, params) {
			if(commons.isDebug) {
				if(message instanceof Array && !params) {
					console.table(message)
					return;
				}
				if(params instanceof Array) {
					console.log(message);
					console.table(params)
					return;
				}
				if(!params) {
					console.log(message)
					return;
				}
				console.log(message, params)
			}
		}
	}
	return obj;
});