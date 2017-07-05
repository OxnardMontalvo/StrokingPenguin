(function () {
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
        return $resource(appSettings.serverPath + "api/Users/:id", null, {
            'update': {
                method: 'PUT'
            }
        });
    }]);

})();