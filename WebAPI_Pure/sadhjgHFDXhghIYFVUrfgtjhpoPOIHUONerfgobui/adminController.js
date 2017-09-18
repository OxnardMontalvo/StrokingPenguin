(function () {
    "use strict";

    angular.module("app")
    // Admin controller for displaying admin specific tasks.
    .controller("adminCtrl", function (printMsg, user, userPage, currentUser, searchUser) {
        var vm = this;
        
        vm.printDisplayMsg = [];

        printMsg.get(function (response) {
            if (response.length > 0) {
                angular.copy(response, vm.printDisplayMsg);
                $('#msg').val(vm.printDisplayMsg[0].Bulletin);
            }
        });
        vm.saveMsg = function () {
            var custumorMsg = $('#msg').val();

            if (vm.printDisplayMsg.length > 0) {
                printMsg.save({ id: vm.printDisplayMsg[0].ID }, JSON.stringify(custumorMsg), function (response) {
                });
            } else {
                printMsg.saveFirst( JSON.stringify(custumorMsg), function (response) {
                });
            };
            
        };
        
        
        vm.users = [];
        vm.take = 25;
        var page = 1;
        var isRefreshed = true;
        vm.hasSearch = false;
        vm.errorMsg = "";
        vm.btnText = "Ladda fler";
        vm.disableDuringLoad = true;

        // Method for checking if there are more users left in DB. If not we hide load more btn.
        function checkForMoreUsers(take) {
            userPage.query({ take: take, page: page + 1 }, function (data) {
                if (data.length > 0) {
                    vm.displayLoadMore = true;
                } else {
                    vm.displayLoadMore = false;
                };
            }, function (error) {
                vm.errorMsg = error.statusText;
                });
        };

        // Runs when starting to load the first users from DB.
        vm.firstUsers = function (take) {
            page = 1;
            vm.disableDuringLoad = true;
            vm.btnText = "Laddar...";
            userPage.query({ take: take, page: page }, function (data) {
                angular.copy(data, vm.users);
                vm.disableDuringLoad = false;
                vm.btnText = "Ladda fler";
            }, function (error) {
                vm.errorMsg = error.statusText;
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
                vm.disableDuringLoad = true;
                vm.btnText = "Laddar...";
                userPage.query({ take: take, page: page }, function (data) {
                    // Make sure all queries get pushed in to the array.
                    for (var i = 0; i < data.length; i++) {
                        vm.users.push(data[i]);
                        vm.btnText = "Ladda fler";
                        vm.disableDuringLoad = false;
                    };
                }, function (error) {
                    vm.errorMsg = error.statusText;
                });
                checkForMoreUsers(take);
            };
        };

        // Select a user.
        var copyUser;
        vm.selectUser = function (id) {
            for (var i = 0; i < vm.users.length; i++) {
                if (vm.users[i].Id == id) {
                    vm.selUser = vm.users[i];
                    copyUser = JSON.parse(JSON.stringify(vm.selUser));
                };
            };
        };

        // Cancle operation.
        vm.cancleUserEdit = function (id) {
            for (var i = 0; i < vm.users.length; i++) {
                if (vm.users[i].Id == id) {
                    vm.users[i] = copyUser;
                };
            };
            vm.selUser = "";
        };

        // Save user changes to the DB.
        vm.saveUserEdit = function (id) {
            user.update({ id: id }, vm.selUser, function (data) {
                isRefreshed = false;
                vm.disableDuringLoad = false;
            }, function (error) {
                vm.errorMsg = error.statusText;
            });
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
                }, function (error) {
                    vm.errorMsg = error.statusText;
                });
            };
        };

        // Refresh user list by looping all pages and push the users to the array.
        vm.refreshUserList = function () {
            isRefreshed = true;
            vm.disableDuringLoad = true;
            vm.btnText = "Ladda fler";
            vm.hasSearch = false;
            vm.users = [];
            for (var i = 1; i < page + 1; i++) {
                vm.btnText = "Laddar...";
                userPage.query({ take: vm.take, page: i }, function (data) {
                    for (var i = 0; i < data.length; i++) {
                        vm.users.push(data[i]);
                        vm.btnText = "Ladda fler";
                        vm.disableDuringLoad = false;
                    };
                    // Make sure the list is sortet correct.
                    vm.users.sort(function (a, b) { return a.DistrictNumber - b.DistrictNumber; });
                }, function (error) {
                    vm.errorMsg = error.statusText;
                });
            };
        };

        // Search base on a string for name, county mm.
        vm.searchUser = function (query) {
            vm.displayLoadMore = false;
            vm.disableDuringLoad = true;
            vm.hasSearch = true;
            vm.users = [];
            vm.btnText = "Laddar...";
            searchUser.stringSearch.query({ query: query }, function (data) {
                angular.copy(data, vm.users);
                vm.disableDuringLoad = false;
            }, function (error) {
                vm.errorMsg = error.statusText;
            });
        };

        // Search based on district using a range.
        vm.searchDistrict = function (query) {
            vm.displayLoadMore = false;
            vm.disableDuringLoad = true;
            vm.hasSearch = true;
            vm.users = [];
            vm.btnText = "Laddar...";
            searchUser.districtSearch.query({ query: query }, function (data) {
                angular.copy(data, vm.users);
                vm.disableDuringLoad = false;
            }, function (error) {
                vm.errorMsg = error.statusText;
            });
        };

    })
    .controller("adminCreateCtrl", function (adminCreate) {
        var vm = this;

        vm.activeCat = true;
        vm.formDataCat = {
            ID: 0,
            Active: vm.activeCat
        };

        // Save cats form function.
        vm.saveFormCat = function () {
            // Save cat form to DB
            adminCreate.cats.create(vm.formDataCat, function (data) {
                // Get cats from DB as relode. Give us all cats in drop down.
                adminCreate.cats.get(function (data) {
                    angular.copy(data, vm.cats);
                });

                // Zero out cat form.
                vm.formDataCat = {
                    ID: 0,
                    Active: vm.activeCat
                };
            });
        };

        // Load the cats from DB to display in drop down.
        vm.cats = [];
        adminCreate.cats.get(function (data) {
            angular.copy(data, vm.cats);
        });

        vm.activeFlyer = true;
        // drop down start ID and model.
        vm.selectedCat = {
            ID: 1
        };

        vm.formDataFlyer = {
            ID: 0,
            Active: vm.activeFlyer
        };

        // Save flyer form function.
        vm.saveFormFlyer = function () {
            vm.formDataFlyer.CategoryID = vm.selectedCat.ID;
            // Save flyer form to DB.
            adminCreate.flyers.create(vm.formDataFlyer, function (data) {
                // Zero out flyer form.
                vm.formDataFlyer = {
                    ID: 0,
                    Active: vm.activeFlyer
                };

                adminCreate.flyers.get(function (data) {
                    if (data.length > 0) {
                        angular.copy(data, vm.flyers);
                    };
                });
            });
        };

        vm.flyers = [];
        adminCreate.flyers.get(function (data) {
            if (data.length > 0) {
                angular.copy(data, vm.flyers);
            };
        });

        vm.displayCats = false;
        vm.displayFlyers = false;
        vm.editMode = false;
        var catChangesWithinDisplayCats = false; // Small refresh hack!

        vm.catClick = function () {
            vm.displayCats = true;
            vm.displayFlyers = false;
        };
        vm.flyerClick = function () {
            vm.displayCats = false;
            vm.displayFlyers = true;

            if (catChangesWithinDisplayCats) {
                // Get the changes from DB. Not optimized, but working.
                adminCreate.flyers.get(function (data) {
                    if (data.length > 0) {
                        angular.copy(data, vm.flyers);
                    };
                });
                catChangesWithinDisplayCats = false;
            };
            
        };

        var copy;
        vm.editCats = function (id) {
            vm.editMode = true;
            for (var i = 0; i < vm.cats.length; i++) {
                if (vm.cats[i].ID == id) {
                    vm.selCat = vm.cats[i];
                    copy = JSON.parse(JSON.stringify(vm.selCat));
                };
            };
        };
        
        vm.editFlyer = function (id) {
            vm.editMode = true;
            for (var i = 0; i < vm.flyers.length; i++) {
                if (vm.flyers[i].ID == id) {
                    vm.selFlyer = vm.flyers[i];
                    copy = JSON.parse(JSON.stringify(vm.selFlyer));
                    vm.selectedCat.ID = vm.selFlyer.Category.ID;
                };
            };
        };

        vm.saveCatEdits = function (id) {
            adminCreate.cats.update({ id: id }, vm.selCat, function (data) {
                vm.editMode = false;
                catChangesWithinDisplayCats = true;
            });
            vm.selCat = "";
        };
        vm.saveFlyerEdits = function (id) {
            vm.selFlyer.CategoryID = vm.selectedCat.ID;
            adminCreate.flyers.update({ id: id }, vm.selFlyer, function (data) {
                vm.editMode = false;

                // Get the changes from DB. Not optimized, but working.
                adminCreate.flyers.get(function (data) {
                    if (data.length > 0) {
                        angular.copy(data, vm.flyers);
                    };
                });
            });
            vm.selFlyer = "";
        };
        
        vm.cancleCat = function () {
            for (var i = 0; i < vm.cats.length; i++) {
                if (vm.cats[i].ID == id) {
                    vm.cats[i] = copy;
                };
            };
            vm.selCat = "";
        };
        vm.cancleFlyer = function (id) {
            for (var i = 0; i < vm.flyers.length; i++) {
                if (vm.flyers[i].ID == id) {
                    vm.flyers[i] = copy;
                };
            };
            vm.selFlyer = "";
        };

        // Delete Cat and all its flyers.
        vm.deleteCat = function (id) {
            if (confirm('Vill du verkligen ta bort kategorin från Databasen? Detta kommer också ta bort alla reklamblad för kategorin!')) {

                adminCreate.cats.delete({ id: id }, function (data) {
                    vm.cats.splice(id, 1);

                    adminCreate.flyers.get(function (data) {
                        angular.copy(data, vm.flyers);
                    });
                    adminCreate.cats.get(function (data) {
                        angular.copy(data, vm.cats);
                    });
                });
            };
        };
        // Delete Flyer.
        vm.deleteFlyer = function (id) {
            if (confirm('Vill du verkligen ta bort reklambladet från Databasen?')) {
                adminCreate.flyers.delete({ id: id }, function (data) {
                    vm.flyers.splice(id, 1);

                    adminCreate.flyers.get(function (data) {
                        angular.copy(data, vm.flyers);
                    });
                });
            };
        };
    });

})();