(function () {
    "use strict";

    angular.module("app")
        // The start controller for displaying the info and getting the user information.
        .controller("userCtrl", function (user, userInfo) {
            var vm = this;

            vm.cats = [];
            userInfo.get(function (data) {
                //console.log(data);
                angular.copy(data, vm.cats);
            });

            console.log(vm.cats);

            vm.allFlyers = [];
            vm.curCatFlyers = [];
            vm.checkChange = function (cat) {
                console.log(cat.Flyers);

                userInfo.savePut({ id: cat.ID }, cat.Flyers, function (response) {
                    consol.log(cat.ID);
                    console.log(response);
                });
            };

        });
})();