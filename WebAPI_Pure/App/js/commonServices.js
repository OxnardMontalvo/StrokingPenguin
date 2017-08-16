(function () {
	"use strict";

	angular.module("services", ["ngResource"])
	.constant("appSettings", {
		serverPath: "http://reklamtack.nu/"
	});

})();
