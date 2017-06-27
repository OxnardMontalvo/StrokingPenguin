(function () {
    "use strict";

    angular.module("app")
    .controller("startCtrl", function () {
        var vm = this;

        vm.show = true;
        vm.phaseOneHide = true;
    })
    .controller("adminLoginCtrl", function () {
        var vm = this;

        vm.show = false;
        vm.phaseOneHide = false;
    });

})();