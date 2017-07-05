(function () {
    "use strict";

    angular.module("app")
    // Register and Login controller, for both users and admin.
    .controller("registerAndLoginCtrl", function (userAccount) {
        var vm = this;

        vm.show = false;
        vm.phaseOneHide = false;

        //Logging in code.

        vm.userData = {
            userName: '',
            email: '',
            password: '',
            confirmPassword: ''
        };

    });

})();