(function () {
    "use strict";

    angular.module("app")
    .controller("startCtrl", function () {
        var vm = this;

        vm.show = true;
        vm.phaseOneHide = true;
    })
    .controller("adminLoginCtrl", ["userAccount", adminLoginCtrl]);

    function adminLoginCtrl(userAccount) {
        var vm = this;

        vm.show = false;
        vm.phaseOneHide = false;

        //Logging in code.
        vm.isLoggedIn = false;
        vm.message = '';
        vm.userData = {
            userName: '',
            email: '',
            password: '',
            confirmPassword: ''
        };

        vm.login = function () {
            vm.userData.grant_type = "password";
            vm.userData.userName = vm.userData.email;

            userAccount.loginUser(vm.userData, function (data) {
                vm.isLoggedIn = true;
                vm.message = "";
                vm.password = "";
                vm.token = data.access_token;
            },
            function (response) {
                vm.password = "";
                vm.isLoggedIn = false;
                vm.message = "ERROR";
            })
        };

    }

})();