(function () {
    "use strict";

    var tokenKey = "access_token";

    angular.module("app")
    // Register and Login controller, for both users and admin.
    .controller("registerAndLoginCtrl", function (userAccount, $location) {
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

        vm.login = function () {
            vm.userData.grant_type = "password";
            vm.userData.confirmPassword = vm.userData.password;
            vm.userData.userName = vm.userData.email;
            userAccount.loginUser(vm.userData, function (response, headersGetter) {

                sessionStorage.setItem(tokenKey, response.access_token);
                
                //if (response.roles[0] == "Admin") {
                //    $location.path("/Admin")
                //} else {
                //    $location.path("/Login")
                //}
            });
        };

    });

})();