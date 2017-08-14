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
            return $resource(appSettings.serverPath + "api/Users/:id", {}, {
                'update': {
                    method: 'PUT',
                    headers: { "Authorization": tokenHeader }
                },
                'query': {
                    method: 'GET',
                    isArray: true,
                    headers: { "Authorization": tokenHeader }
                },
                'delete': {
                    method: 'DELETE',
                    isArray: false,
                    headers: { "Authorization": tokenHeader }
                }
            });
        }])

        .factory("userPage", ["$resource", "appSettings", function ($resource, appSettings) {
            return $resource(appSettings.serverPath + "api/Users/:take/:page", {}, {
                'query': {
                    method: 'GET',
                    isArray: true,
                    headers: { "Authorization": tokenHeader }
                }
            });
        }])

        .factory("searchUser", ["$resource", "appSettings", function ($resource, appSettings) {
            return {
                stringSearch: $resource(appSettings.serverPath + "api/Users/Query/:query", {}, {
                    'query': {
                        method: 'GET',
                        isArray: true,
                        headers: { "Authorization": tokenHeader }
                    }
                }),
                districtSearch: $resource(appSettings.serverPath + "api/Users/Districts/:query", {}, {
                    'query': {
                        method: 'GET',
                        isArray: true,
                        headers: { "Authorization": tokenHeader }
                    }
                })
            };
        }])

        .factory("adminCreate", ["$resource", "appSettings", function ($resource, appSettings) {
            return {
                cats: $resource(appSettings.serverPath + "api/Cats/:id", {}, {
                    'create': {
                        method: 'POST',
                        isArray: false,
                        headers: { "Authorization": tokenHeader }
                    },
                    'get': {
                        method: 'GET',
                        isArray: true,
                        headers: { "Authorization": tokenHeader }
                    },
                    'update': {
                        method: 'PUT',
                        isArray: false,
                        headers: { "Authorization": tokenHeader }
                    },
                    'delete': {
                        method: 'DELETE',
                        isArray: false,
                        headers: { "Authorization": tokenHeader }
                    }
                }),
                flyers: $resource(appSettings.serverPath + "api/Flyers/:id", {}, {
                    'create': {
                        method: 'POST',
                        isArray: false,
                        headers: { "Authorization": tokenHeader }
                    },
                    'get': {
                        method: 'GET',
                        isArray: true,
                        headers: { "Authorization": tokenHeader }
                    },
                    'update': {
                        method: 'PUT',
                        isArray: false,
                        headers: { "Authorization": tokenHeader }
                    },
                    'delete': {
                        method: 'DELETE',
                        isArray: false,
                        headers: { "Authorization": tokenHeader }
                    }
                })
            };
        }])

        .factory("userInfo", ["$resource", "appSettings", function ($resource, appSettings) {
            return $resource(appSettings.serverPath + "api/UserFlyers/:id", {}, {
                'get': {
                    method: 'GET',
                    isArray: true,
                    headers: { "Authorization": tokenHeader }
                },
                'save': {
                    method: 'POST',
                    isArray: true,
                    headers: { "Authorization": tokenHeader }
                }
            });
        }])

        .factory("printMsg", ["$resource", "appSettings", function ($resource, appSettings) {
            return $resource(appSettings.serverPath + "api/Messages/:id", {}, {
                'get': {
                    method: 'GET',
                    isArray: true,
                    headers: { "Authorization": tokenHeader }
                },
                'save': {
                    method: 'POST',
                    isArray: false,
                    headers: { "Authorization": tokenHeader }
                }
            });
        }])

        .factory("fp", ["$resource", "appSettings", function ($resource, appSettings) {
            return $resource(appSettings.serverPath + "ForgotPassword", {}, {
                'get': {
                    method: 'GET'
                }
            });
        }])

        .factory("cm", ["$resource", "appSettings", function ($resource, appSettings) {
            return $resource(appSettings.serverPath + "ConfirmEmail", {}, {
                'get': {
                    method: 'GET'
                }
            });
        }])

        .factory("rp", ["$resource", "appSettings", function ($resources, appSettings) {
            return $resources(appSettings.serverPath + "ResetPassword", {}, {
                'post': {
                    method: 'POST'
                }
            });
        }])

        .factory("changePass", ["$resource", "appSettings", function ($resource, appSettings) {
            return $resource(appSettings.serverPath + "api/Users/ChangePassword", {}, {
                'update': {
                    method: 'POST',
                    isArray: false,
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