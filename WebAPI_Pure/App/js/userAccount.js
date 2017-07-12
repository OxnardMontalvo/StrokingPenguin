﻿(function () {
    "use strict";
    angular.module("services")

    .factory("userAccount", ["$resource", "appSettings", function ($resource, appSettings) {
        return $resource(appSettings.serverPath + "/Token", null,
            {
                'loginUser': {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
                    transformRequest: function (data, headersGetter) {
                        var str = [];
                        for (var d in data)
                            str.push(encodeURIComponent(d) + "=" + encodeURIComponent(data[d]));
                        return str.join("&");
                    }
                }
            });
    }])

    .factory("user", ["$resource", "appSettings", function ($resource, appSettings) {
        return $resource(appSettings.serverPath + "api/Users/:id", {}, {
            'update': {
                method: 'PUT',
                headers: { "Authorization": tokenHeader }
            },
            'query': {
                method: 'GET',
                isArray: true,
                headers: { "Authorization": tokenHeader }
            }
        });
    }])

    .factory("searchUser", ["$resource", "appSettings", function ($resource, appsetting) {
        return $resource(appsetting.serverPath + "api/Users/Query/:query", {}, {
            'query': {
                method: 'GET',
                isArray: true,
                headers: { "Authorization": tokenHeader }
            }
        });
    }]);

    function tokenHeader() {
        var t = sessionStorage.getItem("access_token");
        var headers = {};
        if (t) {
            headers = 'Bearer ' + t;
        }
        return headers;
    }

})();

//$http({
//    url: "http://localhost:52609/Token",
//    data: vm.userData,
//    method: "POST",
//    headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
//    transformRequest: function (data, headersGetter) {
//        var str = [];
//        for (var d in data)
//            str.push(encodeURIComponent(d) + "=" + encodeURIComponent(data[d]));
//        return str.join("&");
//    }
//}).then(function (response) {
//    currentUser = response.data.userName;
//    console.log(response);
//    sessionStorage.setItem(tokenKey, response.data.access_token);

//    if (response.data.roles[0] == "Admin") {
//        $location.path("/Admin")
//    } else {
//        $location.path("/Login")
//    }
//});

//var t = sessionStorage.getItem(tokenKey);
//var headers = {};
//if (t) {
//    headers.Authorization = 'Bearer ' + t;
//}