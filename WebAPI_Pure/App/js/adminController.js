(function () {
    "use strict";

    angular.module("app")
    // Admin controller for displaying admin specific tasks.
    .controller("adminCtrl", function (user, currentUser, searchUser) {
        var vm = this;

        var dbUsers;

        user.query(function (data) {
            angular.copy(data.length, dbUsers);
        });
        
        console.log(dbUsers);
        console.log(currentUser.getProfile())

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
        vm.currentEdit = '';

        // Make sure we edit correct user.
        vm.editUser = function (getId) {
            if (vm.currentEdit == getId) {
                vm.currentEdit = '';
            } else {
                vm.currentEdit = getId;
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
            vm.currentEdit = '';
        };

        vm.searchString = "";
        vm.search = function () {
            console.log(vm.searchString);
            searchUser.stringSearch.query({query: vm.searchString}, function (data) {
                console.log(data);
                angular.copy(data, vm.users);
            });
        };

        vm.searchStringDistrict = null;
        vm.searchDistrict = function () {
            console.log(vm.searchString);
            searchUser.districtSearch.query({query: vm.searchStringDistrict }, function (data) {
                console.log(data);
                angular.copy(data, vm.users);
            });
        };


    });

})();