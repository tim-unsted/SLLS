var sllsApp = angular.module("sllsApp", []);

sllsApp.controller("sllsController", function ($scope, $http) {
    $http({
        method: "GET",
        url: "LibraryAdmin/Titles/GetTitles"
    })
     .success(function (data)
     {             
         $scope.titles = data;
     });       

});



sllsApp.factory("sllsService", ["$http", function ($http) {

    var sllsService = {};

    sllsService.getTitles = function () {
        return $http.get("/Titles/GetTitles");
    };

    return sllsService;

}]);
