(function () {
    'use strict';
    var files = angular.module('files', []);
    //controller
    files.controller("filesCtrl", function ($scope, $http, $window, FilesSvc, $rootScope, $timeout) {
        $scope.keys = "";
        $scope.list;
        $scope.model;
        $scope.orderby;
        $scope.isShow = true;
        $scope.cols = [
            { name: "默认排序", col: "0" },
            { name: "编号降序", col: "1" },
            { name: "编号升序", col: "2" }
        ];
        //查询
        $scope.search = function () {
            loadData();
        };
        //获取列表
        function loadData() {
            var promise = FilesSvc.GetFileList($scope.keys);
            promise.then(function (data) {
                $scope.list = data;
            });
        }
        loadData();
        //获取详细
        $scope.detail = function (id) {
            var promise = FilesSvc.GetDetail(id);
            promise.then(function (data) {
                $scope.model = data;
            });
        }
       
    }
    );
    //service
    files.service('FilesSvc', function ($http, $q, $rootScope) {
        this.GetFileList = function (keys) {
            var deferred = $q.defer();//声明延后执行
            var promise = deferred.promise;
            $http({
                method: 'GET', url: "/Files/GetList", params: {
                    'keys': keys,
                }
            }).success(function (data) {
                deferred.resolve(data);//执行成功
            }).error(function (data) {
                deferred.reject();//执行失败
            });
            return promise; //返回承诺，返回获取数据的API
        };

        this.GetDetail = function (id) {
            var deferred = $q.defer();
            var promise = deferred.promise;
            $http({
                method: 'POST', url: "/Files/GetDetail", params: {
                    'id': id
                }
            }).success(function (data) {
                deferred.resolve(data);
            }).error(function (data) {
                deferred.reject();
            });
            return promise;
        }

    });

})();
