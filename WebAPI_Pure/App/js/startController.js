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
        vm.btnText = "Anmäl dig";

        vm.formData = {};

        vm.submitForm = function () {
            vm.errorMsg = "";
            vm.btnText = "Skickar anmälan..."
            vm.disableDuringLoading = true;
            user.save(vm.formData, function (response) {
                console.log(response);
                vm.validMsg = true;
                vm.btnText = "Anmäl dig";
                vm.disableDuringLoading = false;
                vm.formData = {};
                $scope.signUpForm.$setPristine();
            }, function (error) {
                var statusTxt = error.statusText;
                vm.errorMsg = statusTxt + ", Försök igen.";
                $scope.signUpForm.$setPristine();
                vm.disableDuringLoading = false;
            });
        };

        vm.regForm = function () {
            user.save(vm.formData, function (response) {
                console.log(response);
            });
        };

    });

})();

//vm.users.splice(vm.user.length - 1, 0, vm.user);