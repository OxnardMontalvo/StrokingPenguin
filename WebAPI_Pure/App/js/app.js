/// <reference path="C:\Users\deltagare\Source\Repos\StrokingPenguin\WebAPI_Pure\Scripts/angular.js" />
/// <reference path="C:\Users\deltagare\Source\Repos\StrokingPenguin\WebAPI_Pure\Scripts/angular-route.js" />


(function () {
    "use strict";

    angular.module("app", ["ngRoute", "services"])
        .config(function ($routeProvider, $qProvider) {
        $routeProvider
        .when("/", {
            templateUrl: "App/html/startPage.html",
            controller: "registerAndLoginCtrl",
            controllerAs: "vm"
        })
        .when("/Login", {
            templateUrl: "App/html/startPage.html",
            controller: "registerAndLoginCtrl",
            controllerAs: "vm"
        })
        .when("/Admin", {
            templateUrl: "App/html/adminPage.html",
            controller: "adminCtrl",
            controllerAs: "vm",
            resolve: {
                
                checkRoleValidation: function (checkRole, $location) {
                    if (checkRole.getARole().$$state.value == false) {
                        $location.path("/Login");
                    } else {
                        return true;
                    }
                }
            }
         })
        .when("/AdminCreate", {
            templateUrl: "App/html/adminCreate.html",
            controller: "adminCreateCtrl",
            controllerAs: "vm",
            resolve: {

                checkRoleValidation: function (checkRole, $location) {
                    if (checkRole.getARole().$$state.value == false) {
                        $location.path("/Login");
                    } else {
                        return true;
                    }
                }
            }
        })
        .when("/ForgotPassword", {
            templateUrl: "App/html/forgotPasswordPage.html",
            controller: "forgotPassCtrl",
            controllerAs: "vm"
        })
        .when("/RecoverPassword/:userId/:code", {
            templateUrl: "App/html/restorePasswordPage.html",
            controller: "recoverPassCtrl",
            controllerAs: "vm"
        })
        .when("/ChangePassword", {
            templateUrl: "App/html/changePasswordPage.html",
            controller: "changePswCtrl",
            controllerAs: "vm",
            resolve: {
                checkRoleValidation: function (checkRole, $location) {
                    if (checkRole.getAURole().$$state.value == false) {
                        $location.path("/Login");
                    } else {
                        return true;
                    }
                }
            }
        })
        .when("/ConfirmEmail/:userId/:code", {
            templateUrl: "App/html/confirmMail.html",
            controller: "confirmMailCtrl",
            controllerAs: "vm"
        })
        .when("/User", {
            templateUrl: "App/html/userPage.html",
            controller: "userCtrl",
            controllerAs: "vm",
            resolve: {
                checkRoleValidation: function (checkRole, $location) {
                    if (checkRole.getAURole().$$state.value == false) {
                        $location.path("/Login");
                    } else {
                        console.log(true);
                        return true;
                    }
                }
            }
        })
        .otherwise({
            redirectTo: "/"
        });
    })

    .factory("checkRole", function ($q, currentUser) {
        return {
            getARole: function () {
                var deferred = $q.defer();
                if (currentUser.getProfile() != null && currentUser.getProfile().isLoggedIn && currentUser.getProfile().role === "Admin") {
                    deferred.resolve(true);
                    return deferred.promise;
                } else {
                    deferred.resolve(false);
                    return deferred.promise;
                }
            },

            getAURole: function () {
                var deferred = $q.defer();
                if (currentUser.getProfile() != null && currentUser.getProfile().isLoggedIn && (currentUser.getProfile().role === "Admin" || currentUser.getProfile().role === "User")) {
                    deferred.resolve(true);
                    return deferred.promise;
                } else {
                    deferred.resolve(false);
                    return deferred.promise;
                }
            }
        };
    })

    .factory("currentUser", function () {
        var profile = {
            isLoggedIn: false,
            username: "",
            role: ""
        };

        var setProfile = function (username, isLoggedIn, role) {
            profile.username = username;
            profile.isLoggedIn = isLoggedIn;
            profile.role = role;

            sessionStorage.setItem("profile", JSON.stringify(profile));
        };

        var getProfile = function () {
            return JSON.parse(sessionStorage.getItem("profile"));

        };

        return {
            setProfile: setProfile,
            getProfile: getProfile
        };
    })

    .controller("userInfo", function (currentUser, $scope, $location) {
        
        $scope.displayLogOut = false;
        $scope.$watch(function () {
            if (currentUser.getProfile() != null) {
                return currentUser.getProfile().username;
            }
        }, function (newValue, oldValue) {
            if (newValue != null) {
                $scope.cUser = newValue;
                $scope.displayLogOut = true;
            };
        });

        $scope.$watch(function () {
            if (currentUser.getProfile() != null) {
                return currentUser.getProfile().role;
            }
        }, function (newValue, oldValue) {
            if (newValue != null) {
                $scope.currentRole = newValue;
            };
        });
        
        $scope.logOut = function () {
            sessionStorage.clear();
            $scope.cUser = "";
            $scope.displayLogOut = false;
            $location.path("/");
        };

        $scope.changePass = function () {
            $location.path("/ChangePassword");
        };

    });

})();