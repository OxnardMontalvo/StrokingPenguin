(function () {
    "use strict";

    angular.module("app")
        // The start controller for displaying the info and getting the user information.
        .controller("userCtrl", function (user, userInfo, userAll) {
            var vm = this;
            
            vm.cats = [];
            userInfo.get(function (data) {
                angular.copy(data, vm.cats);
            });

            vm.checkChange = function (cat) {

                userInfo.savePut({ id: cat.ID }, cat.Flyers, function (response) {
                    userInfo.get(function (data) {
                        angular.copy(data, vm.cats);
                    });
                });
            };

            vm.all;
            vm.checkAllChange = function (cat) {
                cat.bAll ? SelectAllFlyers() : DeselectAllFlyers();

                function SelectAllFlyers() {

                    userAll.selectAllFlyers.save({ id: cat.ID }, cat.ID, function (response) {
                        userInfo.get(function (data) {
                            angular.copy(data, vm.cats);
                        });
                    });
                };

                function DeselectAllFlyers() {
                    userAll.deselectAllFlyers.save({ id: cat.ID }, cat.ID, function (response) {
                        userInfo.get(function (data) {
                            angular.copy(data, vm.cats);
                        });
                    });
                };
            };


        });
})();