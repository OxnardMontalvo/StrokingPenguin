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
            controller: "adminLoginCtrl",
            controllerAs: "vm"
        })
        .otherwise({
            redirectTo: "/"
        });
    });

})();