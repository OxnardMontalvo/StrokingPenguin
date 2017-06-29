(function () {
    "use strict";

    angular.module("app")
    .controller("startCtrl", function (noticeOfIntrest) {
        var vm = this;

        vm.show = true;
        vm.phaseOneHide = true;

        vm.formData = {};

        vm.submitForm = function () {

            noticeOfIntrest.save(vm.formData);

            vm.formData = {};
        };

    })
    .controller("adminLoginCtrl", function (userAccount) {
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

    })
    .controller("adminDisplayCtrl", function (displayUsers, saveEditUser) {
        var vm = this;

        vm.users = [];

        vm.getList = function () {
            displayUsers.query(function (data) {
                angular.copy(data, vm.users);
            });
        };

        vm.show = true;
        vm.hide = true;
        vm.currentEdit = '';
        var editData = {
            Id: '',
            Name: '',
            Address: '',
            PostalCode: '',
            County: '',
            Email: '',
            DistrictNumber: '',
            DeliveryOrderNumber: ''
        };
        vm.clickTest = function (getId) {
            if (vm.currentEdit == getId) {
                vm.currentEdit = '';
            } else {
                vm.currentEdit = getId;
            }
            vm.show = false;
            vm.hide = false;

            saveEditUser.get({ id: getId }, function (data) {
                angular.copy(data, editData);
            });

            console.log(editData);
        };

        vm.saveEdits = function () {

            console.log(vm.users);


        };

    });

})();