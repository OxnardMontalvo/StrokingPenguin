(function () {
    "use strict";

    angular.module("app")
        // The start controller for displaying the info and getting the user information.
    .controller("startCtrl", function ($scope, user) {
        var vm = this;

        vm.show = true;
        vm.phaseOneHide = true;
        vm.validMsg = false;
        vm.msg = "Tack för din anmälan."
        vm.errorMsg = "";

        vm.formData = {};

        vm.submitForm = function () {

            vm.waiteForResponse = true;
            user.save(vm.formData, function (response) {
                vm.validMsg = true;
                vm.formData = {};
                //$scope.signUpForm.$setUntouched();
                $scope.signUpForm.$setPristine();
                vm.waiteForResponse = false;
            }, function (error) {
                var statusTxt = error.statusText;
                vm.errorMsg = statusTxt + ", Försök igen.";
            });
        };

    });

})();

//vm.users.splice(vm.user.length - 1, 0, vm.user);