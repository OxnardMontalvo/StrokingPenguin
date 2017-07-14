/// <reference path="C:\Users\deltagare\Source\Repos\StrokingPenguin\WebAPI_Pure\Scripts/angular.js" />
/// <reference path="C:\Users\deltagare\Source\Repos\StrokingPenguin\WebAPI_Pure\Scripts/angular-route.js" />


(function () {
    "use strict";

    angular.module("app", ["ngRoute", "services"])
    .config(function ($routeProvider) {
        $routeProvider
        .when("/", {
            templateUrl: "App/html/startPage.html",
            controller: "startCtrl",
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
            controllerAs: "vm"
        })
        .when("/User", {
            templateUrl: "",
            controller: "",
            controllerAs: ""
        })
        .otherwise({
            redirectTo: "/"
        });
    })

    .factory("currentUser", function () {
        var profile = {
            isLoggedIn: false,
            username: ""
        };

        var setProfile = function (username, isLoggedIn) {
            profile.username = username;
            profile.isLoggedIn = isLoggedIn;

            //sessionStorage.setItem("profile", JSON.stringify(profile));
            sessionStorage.setItem("profile", profile.username);
        };

        var getProfile = function () {
            //return JSON.parse(sessionStorage.getItem("profile"));
            return sessionStorage.getItem("profile", profile);

        };

        return {
            setProfile: setProfile,
            getProfile: getProfile
        };
    })

    .controller("userInfo", function (currentUser, $scope, $location) {
        
        $scope.displayLogOut = false;
        $scope.$watch(function () {
            return currentUser.getProfile();
        }, function (newValue, oldValue) {
            if (newValue != null) {
                $scope.cUser = newValue;
                $scope.displayLogOut = true;
            };
        });
        
        $scope.logOut = function () {
            sessionStorage.clear();
            $scope.cUser = "";
            $scope.displayLogOut = false;
            $location.path("/");
        };

    });

})();