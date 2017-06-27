(function () {
    "use strict";

    angular.module("services")
    .factory("userAccount", ["$resource", "appSettings", userAccount])

    function userAccount($resource, appSettings) {
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
    }

})();