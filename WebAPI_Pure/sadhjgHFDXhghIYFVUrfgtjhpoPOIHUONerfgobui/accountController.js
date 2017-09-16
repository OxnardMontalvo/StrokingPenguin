(function () {
    "use strict";

    var tokenKey = "access_token";

    angular.module("app")
    // Register and Login controller, for both users and admin.
    .controller("registerAndLoginCtrl", function (user, userAccount, changePass, $scope, $location, currentUser) {
        var vm = this;

        // Get modal for login and register.
        var modalReg = document.getElementById('reg');
        var modalLogIn = document.getElementById('login');

        // Function to display the modals.
        vm.displayReg = function () {
            window.addEventListener('click', clickOutside);
            modalReg.style.display = 'block';
        };
        vm.displayLogin = function () {
            window.addEventListener('click', clickOutside);
            modalLogIn.style.display = 'block';
        }

        // Function to exit by clicking on the modal.
        function clickOutside(e) {
            if (e.target == modalReg) {
                modalReg.style.display = 'none';
                window.removeEventListener('click', clickOutside);
            }
            else if (e.target == modalLogIn) {
                modalLogIn.style.display = 'none';
                window.removeEventListener('click', clickOutside);
            }
        };
        
        vm.exitModal = function () {
            modalReg.style.display = 'none';
            modalLogIn.style.display = 'none';
            window.removeEventListener('click', clickOutside);
        };

        vm.show = false;
        vm.phaseOneHide = false;

        vm.formData = {};
        vm.regForm = function () {
            user.save(vm.formData, function (response) {
                console.log(response);
                vm.formData = {};
                $scope.regForm.$setPristine();
            });
        };

        //Logging in code.
        vm.userData = {};

        vm.login = function () {
            vm.userData.grant_type = "password";
            vm.userData.confirmPassword = vm.userData.password;
            vm.userData.userName = vm.userData.email;
            userAccount.loginUser(vm.userData, function (response, headersGetter) {
                currentUser.setProfile(response.userName, true, response.roles);
                sessionStorage.setItem(tokenKey, response.access_token);

                if (response.roles == "Admin") {
                    $location.path("/Admin");
                } else if (response.roles = "User") {
                    $location.path("/User");
                } else {
                    $location.path("/Login");
                };
                
            }, function (error) {
                vm.errorMsg = error.statusText;
            });
            window.removeEventListener('click', clickOutside);
        };

        vm.forgotPassword = function () {
            window.removeEventListener('click', clickOutside);
            $location.path("/ForgotPassword");
            
        };

    })
    .controller("changePswCtrl", function ($scope, changePass) {
        var vm = this;

        vm.formData = {};

        vm.changePassword = function () {
            changePass.update(vm.formData, function (response) {
                if (response.Succeeded) {
                    vm.formData = {};
                    $scope.changeForm.$setPristine();
                    vm.msg = "Lösenordet ändrat."
                };
            }, function (error) {
                vm.errorMsg = error.statusText;
            });
        };

    })
    .controller("forgotPassCtrl", function ($scope, fp) {
        var vm = this;

        vm.formData = {};
        vm.msg = "";
        vm.validMsg = false;

        vm.send = function () {
            fp.get(vm.formData, function (response) {
                vm.msg = "Länken är nu skickad till den angivna eposten.";
                vm.validMsg = true;
                vm.formData = {};
                $scope.forgotForm.$setPristine();
            }, function (error) {
                vm.errorMsg = error.statusText;
            });
        };

    })
    .controller("recoverPassCtrl", function (rp, $routeParams, $location) {
        var vm = this;

        vm.formData = {
            'ID': $routeParams.userId,
            'Code': $routeParams.code,
            'Email': "",
            'Password': "",
            'ConfirmPassword': ""
        }

        vm.confirmNewPass = function () {
            rp.post(vm.formData, function (response) {
                $location.path("/Login");
            }, function (error) {
                vm.errorMsg = error.statusText;
            });
        };

    })
    .controller("confirmMailCtrl", function (cm, $routeParams) {
        var vm = this;

        cm.get({ userId: $routeParams.userId, code: $routeParams.code }, function (response) {
            console.log(response[0]);
            if (response[0] == "C") {
                vm.cmMsg = "Tack, Du har nu bekräftat din epost.";
            } else {
                vm.cmMsg = "Något gick fel, försök igen.";
            };
        }, function (error) {
            vm.errorMsg = error.statusText;
        });
    });

})();