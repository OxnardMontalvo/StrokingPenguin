(function () {
    "use strict";

    angular.module("app")
    // Admin controller for displaying admin specific tasks.
    .controller("adminCtrl", function (user, currentUser, searchUser) {
        var vm = this;

        // Getting users and display them.
        vm.users = [];
        vm.getUsers = function () {
            user.query(function (data) {
                angular.copy(data, vm.users);
                //angular.copy(data.length, dbUsers);
            });
        };

        vm.show = true;
        vm.hide = true;
        vm.currentModifyUser = '';

        // Make sure we edit correct user.
        vm.editUser = function (getId) {
            if (vm.currentModifyUser == getId) {
                vm.currentModifyUser = '';
            } else {
                vm.currentModifyUser = getId;
            }
            vm.show = false;
            vm.hide = false;
        };

        // Saves the edited user.
        vm.saveEdits = function (getId) {

            var editUserData = {};
            for (var i = 0; i < vm.users.length; i++) {
                if (vm.users[i].Id == getId) {
                    editUserData = vm.users[i];
                    break;
                };
            };
            user.update({ id: getId }, editUserData);
            vm.currentModifyUser = '';
        };

        //Search by name, and other.
        vm.searchString = "";
        vm.search = function () {
            //console.log(vm.searchString);
            searchUser.stringSearch.query({query: vm.searchString}, function (data) {
                //console.log(data);
                angular.copy(data, vm.users);
            });
        };

        //Search by district nr sing or by range.
        vm.searchStringDistrict = null;
        vm.searchDistrict = function () {
            //console.log(vm.searchString);
            searchUser.districtSearch.query({query: vm.searchStringDistrict }, function (data) {
                //console.log(data);
                angular.copy(data, vm.users);
            });
        };

        //Remove user from DB.
        vm.remove = function (getId) {
            if (vm.currentModifyUser != getId) {
                vm.currentModifyUser = '';
            } else {
                vm.currentModifyUser = getId;
            }
            user.delete({ id: getId }, function (data) {
                //console.log(data);
                vm.getUsers();
            });
            
        };

        vm.cancle = function () {
            vm.show = true;
            vm.hide = true;
            vm.currentModifyUser = '';
            vm.getUsers();
        };

    });

})();