(function () {
    "use strict";

    angular.module("app")
    // Admin controller for displaying admin specific tasks.
    .controller("adminCtrl", function (user, userPage, currentUser, searchUser) {
        var vm = this;

        // Getting users and display them.
        vm.users = [];
        //var allUsers;
        //vm.getUsers = function () {
        //    user.query(function (data) {
        //        angular.copy(data.length, allUsers);
        //        //allUsers = data.length;
        //        //angular.copy(data.length, dbUsers);
        //    });
        //};
        //vm.getUsers();
        //console.log(allUsers);
        
        vm.take = 1;
        var page = 0;
        vm.showPagenation = false;
        var hasSearch = false;

        vm.getUsersPage = function (take) {
            //console.log(page);

            page += 1;
            userPage.query({ take: take, page: page }, function(data) {
                if (hasSearch || vm.users < 1) {
                    vm.take = 1;
                    vm.page = 1;
                    vm.showPagenation = false;
                    hasSearch = false;
                    //console.log(vm.page);
                    //console.log(data);
                    angular.copy(data, vm.users);
                    vm.showPagenation = true;
                    //console.log(vm.users);
                } else {
                    //console.log(data);
                    //console.log(vm.users.length);
                    for (var i = 0; i < data.length; i++) {
                        vm.users.push(data[i]);
                    };
                    // Check if there is a user after the current added.
                    userPage.query({ take: take, page: page + 1 }, function (data) {
                        var nextUser = data;
                        //console.log(nextUser);
                        if (nextUser.length == 0) {
                            vm.showPagenation = false;
                        };
                    });
                    //console.log(vm.users);
                };
                //vm.page += 1;
                console.log(page);
            });
        };

        vm.refreshList = function () {
            ////Almost working refresh. cant edit to higher districtnr and then load in more with lower than that.
            //var byDelivNr = vm.users.slice(0);
            //byDelivNr.sort(function (a, b) {
            //    return a.DistrictNumber - b.DistrictNumber;
            //});

            //for (var i = 0; i < vm.users.length; i++) {
            //    vm.users[i] = byDelivNr[i];
            //};

            //console.log(page);
            //var nrOfu = vm.take * page;
            //var temp = [];
            ////vm.users = [];
            //for (var p = 1; p < page + 1; p++) {
            //    //console.log(p);
            //    userPage.query({ take: vm.take, page: p }, function (data) {
            //        //console.log(data);
            //        temp.push(data[0]);
            //    });
            //};
            //angular.copy(temp, vm.users);
            //console.log(temp);

            //for (var i = 0; i < vm.users.length; i++) {
            //    vm.users[i];
            //};
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

                vm.take = 1;
                vm.page = 1;
                vm.showPagenation = false;
                hasSearch = true;
            });
        };

        //Search by district nr sing or by range.
        vm.searchStringDistrict = null;
        vm.searchDistrict = function () {
            //console.log(vm.searchString);
            searchUser.districtSearch.query({query: vm.searchStringDistrict }, function (data) {
                //console.log(data);
                angular.copy(data, vm.users);

                vm.take = 1;
                vm.page = 1;
                vm.showPagenation = false;
                hasSearch = true;
            });
        };

        //Remove user from DB.
        vm.remove = function (getId) {
            //if (vm.currentModifyUser != getId) {
            //    vm.currentModifyUser = '';
            //} else {
            //    vm.currentModifyUser = getId;
            //}

            if (confirm('Vill du verkligen ta bort personen från Databasen?')) {
                user.delete({ id: getId }, function (data) {
                    //console.log(data);
                    //vm.getUsers();
                    //vm.users.remove(data[getId]);
                    var index;
                    for (var i = 0; i < vm.users.length; i++) {
                        if (vm.users[i].Id == getId) {
                            index = i;
                        }
                    }
                    vm.users.splice(index, 1);
                });
            } else {
                //Do nothing...
            };
        };

        vm.cancle = function () {
            vm.show = true;
            vm.hide = true;
            vm.currentModifyUser = '';
            //vm.getUsers();
            for (var i = 0; i < vm.users.length; i++) {
                vm.users[i];
            }
        };

    });

})();