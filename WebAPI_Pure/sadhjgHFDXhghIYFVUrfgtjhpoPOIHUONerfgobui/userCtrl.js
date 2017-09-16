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
            //console.log(vm.cats);

            vm.checkChange = function (cat) {
                //console.log(cat.Flyers);

                userInfo.savePut({ id: cat.ID }, cat.Flyers, function (response) {
                    //console.log(response);
                    userInfo.get(function (data) {
                        angular.copy(data, vm.cats);
                    });
                });
            };

            vm.all;
            vm.checkAllChange = function (cat) {
                //console.log(cat.bAll);
                //console.log(cat.ID);

                cat.bAll ? SelectAllFlyers() : DeselectAllFlyers();

                function SelectAllFlyers() {
                    //console.log("Select all flyers.");

                    userAll.selectAllFlyers.save({ id: cat.ID }, cat.ID, function (response) {
                        //console.log(response);

                        userInfo.get(function (data) {
                            angular.copy(data, vm.cats);
                        });
                    });
                };

                function DeselectAllFlyers() {
                    //console.log("Deselect all flyers.");

                    userAll.deselectAllFlyers.save({ id: cat.ID }, cat.ID, function (response) {
                        //console.log(response);

                        userInfo.get(function (data) {
                            angular.copy(data, vm.cats);
                        });
                    });
                };
            };


        });
})();