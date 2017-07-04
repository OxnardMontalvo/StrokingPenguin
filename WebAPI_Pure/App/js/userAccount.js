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
        //console.log(currentUser.getProfile().token);
        return $resource(appSettings.serverPath + "api/Users/:id", null, {
            'update': {
                method: 'PUT'
            }
        });
    }]);

    //.factory("currentUser", function ($cookies) {
    //    var profile = {};
    //    var authentication = $cookies.getObject("authentication");
    //    //console.log(authentication)
    //    if (authentication != null && authentication != undefined) {
    //        profile = {
    //            isLoggedIn: authentication.isLoggedIn,
    //            username: authentication.username,
    //            token: authentication.token
    //        };
    //    } else {
    //        profile = {
    //            isLoggedIn: false,
    //            username: '',
    //            token: ''
    //        };
    //    }

    //    //var profile = {
    //    //    isLoggedIn: false,
    //    //    username: '',
    //    //    token: ''
    //    //};

    //    var setProfile = function (username, token) {
    //        profile.username = username;
    //        profile.token = token;
    //        profile.isLoggedIn = true;
    //        $cookies.put("authentication", JSON.stringify(profile));
    //    };

    //    var getProfile = function () {
    //        return profile;
    //    };

    //    return {
    //        setProfile: setProfile,
    //        getProfile: getProfile
    //    };

    //});

})();