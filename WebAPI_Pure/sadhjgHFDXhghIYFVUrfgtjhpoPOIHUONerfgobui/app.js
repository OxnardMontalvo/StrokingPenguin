(function () {
    "use strict";

    angular.module("app", ["ngRoute", "services", "oc.lazyLoad"])
        .config(function ($routeProvider, $qProvider) {
        $routeProvider
        .when("/", {
            templateUrl: "App/html/startPage.html",
            controller: "registerAndLoginCtrl",
            controllerAs: "vm"
        })
        .when("/Login", {
            templateUrl: "App/html/startPage.html",
            controller: "registerAndLoginCtrl",
            controllerAs: "vm"
        })
        .when("/Admin", {
            templateUrl: "App/html/adminPage.html",
            controller: "adminCtrl",
            controllerAs: "vm",
            resolve: {

                loadMyCtrl: function ($ocLazyLoad) {
                    // you can lazy load files for an existing module
                    return $ocLazyLoad.load('App/js/uwefvyACodfiusge.js');
                },
                checkRoleValidation: function (checkRole, $location) {
                    if (checkRole.getARole().$$state.value == false) {
                        $location.path("/Login");
                    } else {
                        return true;
                    }
                }
            }
         })
        .when("/AdminCreate", {
            templateUrl: "App/html/adminCreate.html",
            controller: "adminCreateCtrl",
            controllerAs: "vm",
            resolve: {

                checkRoleValidation: function (checkRole, $location) {
                    if (checkRole.getARole().$$state.value == false) {
                        $location.path("/Login");
                    } else {
                        return true;
                    }
                }
            }
        })
        .when("/ForgotPassword", {
            templateUrl: "App/html/forgotPasswordPage.html",
            controller: "forgotPassCtrl",
            controllerAs: "vm"
        })
        .when("/RecoverPassword/:userId/:code", {
            templateUrl: "App/html/restorePasswordPage.html",
            controller: "recoverPassCtrl",
            controllerAs: "vm"
        })
        .when("/ChangePassword", {
            templateUrl: "App/html/changePasswordPage.html",
            controller: "changePswCtrl",
            controllerAs: "vm",
            resolve: {
                checkRoleValidation: function (checkRole, $location) {
                    if (checkRole.getAURole().$$state.value == false) {
                        $location.path("/Login");
                    } else {
                        return true;
                    }
                }
            }
        })
        .when("/ConfirmEmail/:userId/:code", {
            templateUrl: "App/html/confirmMail.html",
            controller: "confirmMailCtrl",
            controllerAs: "vm"
        })
        .when("/User", {
            templateUrl: "App/html/userPage.html",
            controller: "userCtrl",
            controllerAs: "vm",
            resolve: {

                loadMyCtrl: function ($ocLazyLoad) {
                    // you can lazy load files for an existing module
                    return $ocLazyLoad.load('App/js/dfgsihsdfUCaergdstd.js');
                },
                checkRoleValidation: function (checkRole, $location) {
                    if (checkRole.getAURole().$$state.value == false) {
                        $location.path("/Login");
                    } else {
                        return true;
                    }
                }
            }
        })
        .otherwise({
            redirectTo: "/"
        });
    })

    .factory("checkRole", function ($q, currentUser) {
        return {
            getARole: function () {
                var deferred = $q.defer();
                if (currentUser.getProfile() != null && currentUser.getProfile().isLoggedIn && currentUser.getProfile().role === "Admin") {
                    deferred.resolve(true);
                    return deferred.promise;
                } else {
                    deferred.resolve(false);
                    return deferred.promise;
                }
            },

            getAURole: function () {
                var deferred = $q.defer();
                if (currentUser.getProfile() != null && currentUser.getProfile().isLoggedIn && (currentUser.getProfile().role === "Admin" || currentUser.getProfile().role === "User")) {
                    deferred.resolve(true);
                    return deferred.promise;
                } else {
                    deferred.resolve(false);
                    return deferred.promise;
                }
            }
        };
    })

    .factory("currentUser", function () {
        var profile = {
            isLoggedIn: false,
            username: "",
            role: "",
            confirm: ""
        };

        var setProfile = function (username, isLoggedIn, role, confirm) {
            profile.username = username;
            profile.isLoggedIn = isLoggedIn;
            profile.role = role;
            profile.confirm = confirm;

            sessionStorage.setItem("profile", JSON.stringify(profile));
        };

        var getProfile = function () {
            return JSON.parse(sessionStorage.getItem("profile"));

        };

        return {
            setProfile: setProfile,
            getProfile: getProfile
        };
    })

    .controller("userInfo", function (currentUser, $scope, $location, scm) {
        
        $scope.displayLogOut = false;
        $scope.$watch(function () {
            if (currentUser.getProfile() != null) {
                return currentUser.getProfile().username;
            }
        }, function (newValue, oldValue) {
            if (newValue != null) {
                $scope.cUser = newValue;
                $scope.displayLogOut = true;
            };
        });

        $scope.$watch(function () {
            if (currentUser.getProfile() != null) {
                return currentUser.getProfile().role;
            }
        }, function (newValue, oldValue) {
            if (newValue != null) {
                $scope.currentRole = newValue;
            };
        });

        $scope.$watch(function () {
            if (currentUser.getProfile() != null) {
                return currentUser.getProfile().confirm;
            }
        }, function (newValue, oldValue) {
            if (newValue != null) {
                $scope.confirm = newValue;
            };
        });
        
        $scope.logOut = function () {
            sessionStorage.clear();
            $scope.cUser = "";
            $scope.displayLogOut = false;
            $location.path("/");
        };

        $scope.changePass = function () {
            $location.path("/ChangePassword");
        };

        $scope.newConfirmMail = function () {
            scm.get(function (response) {
            });
            // Display the modal.
            window.addEventListener('click', clickOutside);
            modalIsMailConfirmed.style.display = 'block';
        };

        // Get modal for login and register.
        var modalIsMailConfirmed = document.getElementById('sendConfirmModal');
        
        // Function to exit by clicking on the modal.
        function clickOutside(e) {
            if (e.target == modalIsMailConfirmed) {
                modalIsMailConfirmed.style.display = 'none';
                window.removeEventListener('click', clickOutside);
            }
        };

        $scope.exitModal = function () {
            modalIsMailConfirmed.style.display = 'none';
            window.removeEventListener('click', clickOutside);
        };
        
    });

})();