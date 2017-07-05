(function () {
    "use strict";

    angular.module("app")
        // The start controller for displaying the info and getting the user information.
    .controller("startCtrl", function (user) {
        var vm = this;

        vm.show = true;
        vm.phaseOneHide = true;
        vm.validMsg = false;
        vm.msg = "Tack för din anmälan."

        vm.formData = {};

        vm.submitForm = function () {

            user.save(vm.formData, function (response) {
                console.log(response);
                vm.validMsg = response.Succeeded;
            });

            vm.formData = {};
        };

    });

})();

//vm.users.splice(vm.user.length - 1, 0, vm.user);