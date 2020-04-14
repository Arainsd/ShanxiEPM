(function () {
    'use strict';

  
    //var config = ["$httpProvider", function( $httpProvider) {
    //        //Initialize get if not there
    //        if (!$httpProvider.defaults.headers.get) {
    //            $httpProvider.defaults.headers.get = {};
    //        }

    //        // Enables Request.IsAjaxRequest() in ASP.NET MVC
    //        $httpProvider.defaults.headers.common['X-Requested-With'] =
    //            'XMLHttpRequest';

    //        // Disable IE ajax request caching
    //        $httpProvider.defaults.headers.get['Cache-Control'] = 'no-cache';
    //        $httpProvider.defaults.headers.get['Pragma'] = 'no-cache';
    //    }
    //];
    var pm = angular.module('ProcurementMethod',
        [
            // Angular modules 
            'ui.bootstrap'
        ]);
    //采购方式controller
    pm.controller("pmCtrl", function ($scope, $http, $window, pmSvc, $rootScope, $timeout) {
        //添加保存
        $scope.Save = function (model) {
            var bv = $('#mainForm').data('bootstrapValidator');
            bv.validate();
            if (bv.isValid()) {
                pmSvc.Add(model).then(function (data) {
                    if (data.Flag) {
                        parent.layer.msg("采购方式添加成功",
                            { time: 1000, icon: 1 },
                            function () {
                                var index = parent.layer.getFrameIndex(window.name); //获取窗口索引
                                parent.layer.close(index); //关闭窗口
                                parent.window.frames["mainFrame"].location.reload();
                            });
                    }
                });
            }
        }
        //编辑保存
        $scope.Edit = function (model) {
            var bv = $('#mainForm').data('bootstrapValidator');
            bv.validate();
            if (bv.isValid()) {
                model.IsEnable = model.IsEnable.Key;
                console.log(model.IsEnable);
                model.Id = model.SId;
                pmSvc.Edit(model).then(function (data) {
                    if (data.Flag) {
                        parent.layer.msg("采购方式修改成功",
                            { time: 1000, icon: 1 },
                            function () {
                                var index = parent.layer.getFrameIndex(window.name); //获取窗口索引
                                parent.layer.close(index); //关闭窗口
                                parent.window.frames["mainFrame"].location.reload();
                            });
                    }
                });
            }
        }
        //删除
        $scope.Delete = function () {
            var chkItems = $('.layui-table tbody input[type="checkbox"]:checked');
            var ids = "";
            chkItems.each(function (index, item) {
                var hfId = $(this).parent().find("input[type='hidden']");
                ids += hfId.val() + ",";
            });
            if (ids != "") {
                parent.layer.confirm('确认删除所选择的项？', {
                    btn: ['确认', '取消'] //按钮
                }, function () {
                    pmSvc.Delete(ids).then(function (data) {
                        if (data.Flag) {
                            parent.layer.msg("删除成功", { time: 1000, icon: 1 });
                            GetList(1, 10);

                        } else {
                            parent.layer.alert(data.Message, { icon: 2 });
                        }
                    });
                }, function () {
                    return;
                });
            } else {
                parent.layer.alert("请先选择要删除的数据", { icon: 2 });
            }
        }
        //初始化下拉列表
        $scope.InitSelect = function () {
            ////获取选项
            //pmSvc.GetSelect().then(function (data) {
            //    $scope.enable = data.enable;
            //    $scope.confirm = data.confirm;
            //});
            //$.get("/ProcurementMethodA/GetSelectItems", {}, function (data) {
            //    $scope.enable = data.enable;
            //    $scope.confirm = data.confirm;
            //    $scope.$apply(); //强制通知
            //});
            $.ajax({
                type: "get",
                async: false,
                url: "/ProcurementMethodA/GetSelectItems",
                dataType: "json",
                success: function (data) {
                    $scope.enable = data.enable;
                    $scope.confirm = data.confirm;

                    //$scope.IsEnable = data.enable[0];
                }
            });
        }
        $scope.InitSelect();
        //展示
        function Show() {
            var Request = new Object();
            Request = GetRequest();
            var id = Request['id'];
            //pmSvc.Show(id).then(function (data) {
            //    $scope.pmModel = data.obj;
            //    $scope.pmModel.IsConfirm = data.isConfirm;
            //    $scope.pmModel.IsEnable = data.isEnable;

            //});
            $.ajax({
                type: "get",
                async: false,
                url: "/ProcurementMethodA/Show",
                data: { id: id },
                dataType: "json",
                success: function (data) {
                    $scope.pmModel = data.obj;
                    $scope.pmModel.IsEnable = data.isEnable;
                    //对象 select 默认选中
                    var keepGoing = true;
                    angular.forEach($scope.enable, function (item, index, array) {
                        if (keepGoing) {
                            if (item.Key == $scope.pmModel.IsEnable) {
                                $scope.pmModel.IsEnable = item;
                                keepGoing = false;
                            }
                        }
                    });
                    $scope.pmModel.IsConfirm = data.isConfirm;
                }
            });
        }
        //分页条数
        $scope.pageSize = 3;
        //获取列表
        function GetList(pageIndex) {
            //pmSvc.GetList($scope.name, $scope.code, $scope.isConfirm, $scope.isEnable, pageIndex, pageSize).then(function (data) {
            //    $scope.list = data;
            //});
            $scope.currentPage = pageIndex;
            $.ajax({
                type: "get",
                async: false,
                url: "/ProcurementMethodA/GetList?timestamp=" + new Date().getTime(),
                data: { name: $scope.name, code: $scope.code, isConfirm: $scope.isConfirm, isEnable: $scope.isEnable, pageIndex: pageIndex, pageSize: $scope.pageSize },
                dataType: "json",
                success: function (data) {
                    $scope.list = data;
                    $scope.count = data.AllRowsCount;
                    var t = $timeout(function () {
                        //重新检测权限，重新渲染控件class
                        CheckRight();
                        layui.form.render();
                        $scope.$apply();
                        $timeout.cancel(t);
                    }, 500);
                }
            });
        }
        //查询
        $scope.Search = function () {
            $scope.currentPage = 1;
            $scope.maxSize = 3;
            GetList(1);
        };
        //翻页事件
        $scope.pageChanged = GetList;
            
        //编辑
        if ($scope.action == "edit") {
           
            Show();
        } else if ($scope.action == "list") {
            GetList(1);
        }

        //公用获取url参数
        function GetRequest() {
            var url = location.search; //获取url中"?"符后的字串
            var theRequest = new Object();
            if (url.indexOf("?") != -1) {
                var str = url.substr(1);
                var strs = str.split("&");
                for (var i = 0; i < strs.length; i++) {
                    theRequest[strs[i].split("=")[0]] = unescape(strs[i].split("=")[1]);
                }
            }
            return theRequest;
        }
    });
    //获取采购方式列表服务
    pm.service('pmSvc', function ($http, $q, $rootScope) {
        //添加
        this.Add = function (model) {
            var deferred = $q.defer();//声明延后执行
            var promise = deferred.promise;
            $http({
                method: 'POST', url: "/ProcurementMethodA/Add", params: model
            }).success(function (data) {
                deferred.resolve(data);//执行成功
            }).error(function (data) {
                deferred.reject();//执行失败
            });
            return promise; //返回承诺，返回获取数据的API
        }
        //详情
        this.Show = function (id) {
            var deferred = $q.defer();//声明延后执行
            var promise = deferred.promise;
            $http({
                method: 'GET', url: "/ProcurementMethodA/Show", params: { id: id }
            }).success(function (data) {
                deferred.resolve(data);//执行成功
            }).error(function (data) {
                deferred.reject();//执行失败
            });
            return promise; //返回承诺，返回获取数据的API
        }
        //编辑
        this.Edit = function (model) {
            var deferred = $q.defer();//声明延后执行
            var promise = deferred.promise;
            $http({
                method: 'POST', url: "/ProcurementMethodA/Edit", params: model
            }).success(function (data) {
                deferred.resolve(data);//执行成功
            }).error(function (data) {
                deferred.reject();//执行失败
            });
            return promise; //返回承诺，返回获取数据的API
        }
        //删除
        this.Delete = function (ids) {
            var deferred = $q.defer();//声明延后执行
            var promise = deferred.promise;
            $http({
                method: 'POST', url: "/ProcurementMethodA/Delete", params: { ids: ids }
            }).success(function (data) {
                deferred.resolve(data);//执行成功
            }).error(function (data) {
                deferred.reject();//执行失败
            });
            return promise; //返回承诺，返回获取数据的API
        }

        //获取列表
        this.GetList = function (name, code, isConfirm, isEnable, pageIndex, pageSize) {
            var deferred = $q.defer();//声明延后执行
            var promise = deferred.promise;
            $http({
                method: 'GET', url: "/ProcurementMethodA/GetList", params: { name: name, code: code, isConfirm: isConfirm, isEnable: isEnable, pageIndex: pageIndex, pageSize: pageSize }
            }).success(function (data) {
                deferred.resolve(data);
            }).error(function (data) {
                deferred.reject();
            });
            return promise; //返回承诺，返回获取数据的API
        }

        //获取下拉框
        this.GetSelect = function () {
            var deferred = $q.defer();//声明延后执行
            var promise = deferred.promise;
            $http({
                method: 'GET', url: "/ProcurementMethodA/GetSelectItems", params: {}
            }).success(function (data) {
                deferred.resolve(data);
            }).error(function (data) {
                deferred.reject();
            });
            return promise; //返回承诺，返回获取数据的API
        }

    });

})();
