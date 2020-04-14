(function () {
    'use strict';
    var tender = angular.module('tender', []);
    //controller
    tender.controller("tenderCtrl", function ($scope, $http, $window, TenderSvc, $rootScope, $timeout) {
        $scope.list;
        //获取列表
        function loadData() {
            var promise = TenderSvc.GetList();
            promise.then(function (data) {
                //转json
                var obj = eval('(' + data + ')');
                $scope.list = obj.Rows;
            });
        }
        loadData();
    }
    );
    //service
    tender.service('TenderSvc', function ($http, $q, $rootScope) {
        this.GetList = function (keys) {
            var deferred = $q.defer();//声明延后执行
            var promise = deferred.promise;
            $http({
                method: 'GET', url: "http://192.168.1.232:8033/api/Tender", params: {
                    //'keys': keys,
                }
            }).success(function (data) {
                deferred.resolve(data);//执行成功
            }).error(function (data) {
                deferred.reject();//执行失败
            });
            return promise; //返回承诺，返回获取数据的API
        };

       
    });

})();
