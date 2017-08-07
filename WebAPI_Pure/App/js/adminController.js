(function () {
    "use strict";

    angular.module("app")
    // Admin controller for displaying admin specific tasks.
    .controller("adminCtrl", function (user, userPage, currentUser, searchUser) {
        var vm = this;

        vm.users = [];
        vm.take = 2;
        var page = 1;
        vm.isRefreshed = true;
        vm.hasSearch = false;

        // Method for checking if there are more users left in DB. If not we hide load more btn.
        function checkForMoreUsers(take) {
            userPage.query({ take: take, page: page + 1 }, function (data) {
                if (data.length > 0) {
                    vm.displayLoadMore = true;
                } else {
                    vm.displayLoadMore = false;
                };
            });
        };

        // Runs when starting to load the first users from DB.
        vm.firstUsers = function (take) {
            page = 1;
            userPage.query({ take: take, page: page }, function (data) {
                angular.copy(data, vm.users);
            });
            checkForMoreUsers(take);
        };
        vm.firstUsers(vm.take);

        // Load more users with rules for search, refreshed and such. Call the DB for the next users and push them too the array.
        // Also calls DB once too check if there are more users left after we grabbed ours.
        vm.loadMoreUsers = function (take) {
            if (vm.hasSearch) {
                vm.firstUsers(take);
                vm.hasSearch = false;
            } else {
                if (!vm.isRefreshed) {
                    vm.refreshUserList();
                }

                page += 1;
                userPage.query({ take: take, page: page }, function (data) {
                    // Make sure all queries get pushed in to the array.
                    for (var i = 0; i < data.length; i++) {
                        vm.users.push(data[i]);
                    };
                });
                checkForMoreUsers(take);
            };
        };

        // Select a user.
        vm.selectUser = function (id) {
            for (var i = 0; i < vm.users.length; i++) {
                if (vm.users[i].Id == id) {
                    vm.selUser = vm.users[i];
                };
            };
        };

        // Cancle operation, Not working correctly yet.
        vm.cancleUserEdit = function () {
            vm.selUser = "";
        };

        // Save user changes to the DB.
        vm.saveUserEdit = function (id) {
            user.update({ id: id }, vm.selUser, function (data) { vm.isRefreshed = false; });
            vm.selUser = "";
        };

        // Remove user from DB.
        vm.deleteUser = function (id) {
            if (confirm('Vill du verkligen ta bort personen från Databasen?')) {
                user.delete({ id: id }, function (data) {
                    var index;
                    for (var i = 0; i < vm.users.length; i++) {
                        if (vm.users[i].Id == id) {
                            index = i;
                        };
                    };
                    vm.users.splice(index, 1);
                });
            };
        };

        // Refresh user list by looping all pages and push the users to the array.
        vm.refreshUserList = function () {
            vm.isRefreshed = true;
            vm.hasSearch = false;
            vm.users = [];
            for (var i = 1; i < page + 1; i++) {
                userPage.query({ take: vm.take, page: i }, function (data) {
                    for (var i = 0; i < data.length; i++) {
                        vm.users.push(data[i]);
                    };
                    // Make sure the list is sortet correct.
                    vm.users.sort(function (a, b) { return a.DistrictNumber - b.DistrictNumber; });
                });
            };
        };

        // Search base on a string for name, county mm.
        vm.searchUser = function (query) {
            vm.displayLoadMore = false;
            vm.hasSearch = true;
            vm.users = [];
            searchUser.stringSearch.query({ query: query }, function (data) {
                angular.copy(data, vm.users);
            });
        };

        // Search based on district using a range.
        vm.searchDistrict = function (query) {
            vm.displayLoadMore = false;
            vm.hasSearch = true;
            vm.users = [];
            searchUser.districtSearch.query({ query: query }, function (data) {
                angular.copy(data, vm.users);
            });
        };

    });

})();