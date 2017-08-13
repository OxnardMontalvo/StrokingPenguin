(function () {
    "use strict";

    angular.module("app")
        // The start controller for displaying the info and getting the user information.
        .controller("userCtrl", function (user, userInfo) {
            var vm = this;

            vm.maxRow = 4;

            vm.cats = [];
            userInfo.get(function (data) {
                console.log(data);
                angular.copy(data, vm.cats);
            });

            vm.allFlyers = [];
            vm.checkChange = function (cat) {
                vm.allFlyers = [];
                //console.log(id);
                //console.log(name);
                //console.log(sel);
                console.log(cat);
                for (var i = 0; i < cat.length; i++) {
                    console.log(vm.cats[i]);
                    for (var f = 0; f < vm.cats[i].Flyers.length; f++) {
                        console.log(vm.cats[i].Flyers[f]);
                        //console.log("a")
                        vm.allFlyers.push(vm.cats[i].Flyers[f]);
                    };
                };
                console.log(vm.allFlyers);

                //console.log(cat.Flyers)

                userInfo.save(vm.allFlyers, function (response) {
                    console.log(response);
                });
            };

        }).directive("userDir", function () {
            return {
                restrict: "E",
                scope: {
                    test: "="
                },
                template: '',

                link: function (scope, element, attrs) {

                    //console.log(scope);
                    //console.log(attrs.test);
                    //console.log(element);

                    var a = element.add('<div>');
                    a.css('border', '5px black solid');
                    
                    for (var i = 0; i < attrs.test; i++) {
                        var p = element.add('<p>');
                        a.append(p);
                        p.html(i);
                    };

                },
                controller: function () {

                }
            };
        });
})();