var authorityapp = angular.module('authorityapp', []);

authorityapp.controller("AuthorityController", function myfunction($scope, $http, $window, $filter) {
/*--------------------------------------------------------------
# BLOOD LEVEL MODULE
--------------------------------------------------------------*/

    //Blood Level Search Bar and Pagination
    $scope.bloodlvlrecord = [];

    //Blood Level Listing
    $scope.bloodlvllisting = function () {
        $http({
            method: "GET",
            url: '/Hospital/GetBloodLevel'
        }).then(function (response) {
            $scope.bloodlvlrecord = response.data;
            angular.element(document).ready(function () {
                //Method 1 
                //dTable = $('#bloodlvltable')
                //dTable.DataTable();
                  $('#bloodlvltable').DataTable();
            });
        }, function (error) {
            alert("Blood Level Listing Loading Error...");
        })
    }

    $scope.bloodlvllisting();

    //Update Blood Level
    $scope.savebloodlvl = function (bloodlvldate, bloodlvlqty) {

        $scope.bloodlvl = {
            bloodlvldate: $filter('date')(bloodlvldate, "yyyy-MM-dd"),
            bloodlvlqty: bloodlvlqty
        }

        $("form[name='bloodlvl']").validate({
            rules: {
                bloodDate: "required",
                bloodQty: "required"
            },
            messages: {
                bloodDate: "Date is required",
                bloodQty: "Quantity is required"
            },
            submitHandler: function (form) {
                $http({
                    method: "POST",
                    url: '/Hospital/UpdateBloodLevel',
                    data: $scope.bloodlvl
                }).then(function (response) {
                    if (response.status == 200) {
                        Swal.fire({
                            title: "Blood Level Update Success!",
                            icon: "success"
                        }).then(function () {
                            $("#bloodDate").val("");
                            $("#bloodQty").val("");
                            $scope.bloodlvllisting();
                        });
                    }
                }, function (error) {
                    alert("Blood Level Update Fail...");
                })
            }
        })
    }


/*--------------------------------------------------------------
# BLOOD QUALITY MODULE
--------------------------------------------------------------*/
    //Save Blood Quality
    $scope.options = [{ name: "Pass", id: 1 }, { name: "Fail", id: 2 }];
    $scope.rst = {
        'result': $scope.options[0]
    };

    jQuery.validator.addMethod("aptIdValid", function (value, element) {
        var aptID = $("#rstDonorId").val();
        var isExist = false;

        $.ajax({
            type: "POST",
            url: "/Hospital/validateaptid",
            data: '{_aptid:"' + aptID + '"}',
            contentType: "application/json; charset=utf-8",
            async: false,
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data == 1) {
                    isExist = true;
                } else {
                    isExist = false;
                }
            }, error: function (xhr, textStatus, errorThrown) {
                alert('AJAX Loading error..' + url + query);
                return false;
            }
        });
        return isExist;
    })

    $scope.savebloodquality = function (donorid, selectedOption) {
        var aptID = $("#rstDonorId").val();
        var result = $scope.rst.result.name;

        $("form[name='save-quality-record-form']").validate({
            rules: {
                rstDonorId: {
                    required: true,
                    aptIdValid: true
                },

            },
            messages: {
                rstDonorId: {
                    required: "Appointment ID is required",
                    aptIdValid: "Invalid Appointment ID/Status"
                }
            },
            submitHandler: function (form) {
                $http({
                    method: "POST",
                    url: "/Hospital/UpdateBloodQuality",
                    data: '{ result:"' + result + '",' + 'donorid:"' + aptID + '"}'
                }).then(function (response) {
                    if (response.status == 200) {
                        Swal.fire({
                            title: "Blood Test Result Save Success!",
                            icon: "success"
                        }).then(function () {
                            $("#rstDonorId").val("");
                            location.reload();
                        });
                    }
                }, function (error) {
                    alert("Blood Quality Update Fail...");
                })
            }

        });

    }

    //Blood Quality Listing - Search
    $scope.searchbloodresult = function () {
        var donorid = $("#searchdonorid").val();

        $("form[name='quality-records-form']").validate({
            rules: {
                searchdonorid: {
                    required: true,
                    digits: true
                }
            },
            messages: {
                searchdonorid: {
                    requried: "Donor ID is required.",
                    digits: "Please enter number only."
                }
            },
            submitHandler: function (form) {
                $http({
                    method: "POST",
                    url: '/Hospital/ViewBloodQuality',
                    data: '{_donorid:"' + donorid + '"}'
                }).then(function (response) {
                    if (response.status == 200) {
                        $scope.searchdonorid = null;
                        console.log(response);
                        location.reload();
                        d = new Date();
                        $("#bloodqr").attr("src", "~/image/QrCode.jpg?" + d.getTime());

                        $("#quality-records-container").css("display", "block");
                        sessionStorage.setItem('clicked', true);
                    }
                }).catch(function (reason) {
                    if (reason.status === 500) {
                        $("#quality-records-container").css("display", "none");
                        sessionStorage.setItem('clicked', false);
                        Swal.fire({
                            title: "Donor ID Not Valid.",
                            icon: "warning"
                        }).then(function () {
                            $("#searchdonorid").val("");
                            location.reload();
                        });
                    } else {
                        $scope.searchdonorid = null;
                        $("#quality-records-container").css("display", "none");
                        sessionStorage.setItem('clicked', false);
                        Swal.fire({
                            title: "Error",
                            text: "Error On Retrieving Data From Smart Contract. Status " + reason.status,
                            icon: "error"
                        }).then(function () {
                            $("#searchdonorid").val("");
                            location.reload();
                        });
                    }
                });
            }
        });
    }

    //window.onload = function () {
    //    var data = sessionStorage.getItem('clicked');
    //    if (data == 'true') {
    //        $("#quality-records-container").css("display", "block");
    //        sessionStorage.setItem('clicked', true);
    //    }
    //};

    $scope.disablesession = function () {
        //clear update blood level textfield
        $scope.bloodlvl = null;
        //clear save blood result textfield 
        $scope.bloodrst = null;
        //clear search blood result textfield 
        $scope.searchdonorid = null;
        //hide search blood result container
        $("#quality-records-container").css("display", "none");
        //clear clicked sessionstorage 
        sessionStorage.setItem('clicked', false);
    }

})