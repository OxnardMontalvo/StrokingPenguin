(function () {
    "use strict";

    angular.module("app")
        // The start controller for displaying the info and getting the user information.
        .controller("userCtrl", function (user, adminCreate) {
            var vm = this;

            vm.maxRow = 4;

            vm.cats = [];
            adminCreate.cats.get(function (data) {
                console.log(data);
                angular.copy(data, vm.cats);
            });

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