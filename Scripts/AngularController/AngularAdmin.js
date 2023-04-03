var adminapp = angular.module('adminapp', []);

adminapp.controller("AdminController", function myfunction($scope, $http) {
    //Hospital Authority Account Listing

    $scope.adminacc_listing = function () {
        $http({
            method: "GET",
            url: '/Admin/GetAuthorityAccount'
        }).then(function (response) {
            $scope.hosAccRecord = response.data;
            angular.element(document).ready(function () {
                $('#hosAccTable').DataTable();
            });
        }, function (error) {
            alert("Authority Account Listing Loading Error...");
        })
    }

    $scope.adminacc_listing();

    jQuery.validator.addMethod("checkHosUsername", function (value, element) {
        var hos_username = $("#hosUsername").val();
        var isExist = false;

        $.ajax({
            type: "POST",
            url: "/Home/validateuser",
            data: '{_email:"' + hos_username + '"}',
            contentType: "application/json; charset=utf-8",
            async: false,
            dataType: "json",
            success: function (data) {
                console.log(data);
                if (data == 1) {
                    isExist = false;
                } else {
                    isExist = true;
                }
            }, error: function (xhr, textStatus, errorThrown) {
                alert('AJAX Loading error..' + url + query);
                return false;
            }
        });
        return isExist;
    })

    //New Hospital Authority Account
    $scope.saveHosAcc = function () {
        $("form[name='authorityAccount']").validate({
            rules: {
                hosName: "required",
                hosDept: "required",
                hosUsername: {
                    required: true,
                    checkHosUsername: true
                },
                hosPass: "required",
                hosConfirmPass: {
                    required: true,
                    equalTo:"#hosPass"
                }
            },
            messages: {
                hosName: "Hospital name is required",
                hosDept: "Department is required",
                hosUsername: {
                    required:"Login username is required",
                   checkHosUsername: "Username already taken."
                },
                hosPass:"Password is required",
                hosConfirmPass: {
                    required: "Please re-enter your password",
                    equalTo:"Password does not match"
                }
            },
            submitHandler: function(form) {
                $http({
                    method: "POST",
                    url: '/Admin/RegisterAuthority',
                    data: $scope.hosAccount
                }).then(function (response) {
                    var k = JSON.parse(JSON.stringify(response.data));
                    if (k.message == 'Success') {
                        Swal.fire({
                            title: "Hospital Authority Account Successfully Registered!",
                            text: "Secret key for this account is " + k.seKey,
                            icon: "success",
                            footer: "Secret key is used for the authority account to decrypt something important. So make sure you take note of it :)"
                        });

                        $('#hosPass').val('');
                        $scope.hosAccount = null;
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Oops...',
                            text: 'Something went wrong!'
                        })
                    }
                }, function (error) {
                    alert("Register New Hospital Authority Account Fail..." + error);
                })
            }
        })
    }

    //hide hosaccUpdateBox
     $("#hosaccUpdateBox").hide();

    //Update Hospital Authority Account
    $scope.editHos = function (_userid, _name, _department, _status) {
        $(".adminDashboard").hide();
        $("#hosaccUpdateBox").show();

        $scope.currentHosDetails = {
            userId: _userid,
            name: _name,
            dept: _department,
            status: _status
        }
    }

    $scope.updateHos = function () {
        $("#updateHosDetail").validate({
            rules: {
                updateHosName: "required",
                updateHosDept: "required"
            },
            messages: {
                updateHosName: "Hospital name is required",
                updateHosDept: "Department is required"
            },
            submitHandler: function (form) {
                $http({
                    method: "POST",
                    url: '/Admin/UpdateAuthorityAccount',
                    data: $scope.currentHosDetails
                }).then(function (response) {

                    $(".adminDashboard").show();
                    $("#hosaccUpdateBox").hide();
                    location.reload();
                }, function (error) {
                    alert("Update Authority Account Error...");
                })
            }
        });
      
    }

    //check current password
    jQuery.validator.addMethod("checkPass", function (value, element) {
        var currentPass = $("#currentPass").val();
        var userid = $scope.currentHosDetails.userId;
        var isExist = true;

        $.ajax({
            type: "POST",
            url: "/Admin/ValidatePass",
            data: '{ currentPassword:"' + currentPass + '",' + 'currentid:"' + userid + '"}',
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
                alert("AJAX loading error...");
            }
        });
        return isExist;
    })

    //Update Hospital Authority Password
    $scope.updateHosPass = function (_userid, _newpass) {
        $scope.newpassword = {
            userid: _userid,
            password: _newpass
        };

        $("#updateHosPass").validate({
            rules: {
                currentPass: {
                    required: true,
                    checkPass: true
                },
                newHosPass: "required",
                newHosConfirmPass: {
                    required: true,
                    equalTo: "#newHosPass"
                }
            },
            messages: {
                currentPass: {
                    required: "Current password is required",
                    checkPass:"Current password is invalid"
                },
                newHosPass: "New password is required",
                newHosConfirmPass: {
                    required: "Please re-enter password",
                    equalTo: "Password do not match"
                }
            },
            submitHandler: function (form) {
                $http({
                    method: "POST",
                    url: '/Admin/UpdateHosPass',
                    data: $scope.newpassword
                }).then(function (response) {
                    if (response.status == 200) {
                        Swal.fire({
                            title: "Password Update Success!",
                            icon: "success"
                        }).then(function () {
                            $("#currentPass").val("");
                            $("#newHosPass").val("");
                            $("#newHosConfirmPass").val("");
                            location.reload();
                        });
                    }
                }, function (error) {
                    alert("Update HosPass fail..." + error)
                });
            }
        })

    }

    //Validate admin password - reset hos password
    $scope.validateadminpass = function (password) {
        //var currentPass = $("#verifyadminpass").val();
        $scope.password = {
            _password: password
        }

        $("#verifyadminform").validate({
            rules: {
                verifyadminpass: {
                    required: true
                }
            },
            messages: {
                verifyadminpass: {
                    required:"Password is required."
                }
            },
            submitHandler: function (form) {
                $http({
                    method: "POST",
                    url: "/Admin/ValidateAdminPass",
                    data: $scope.password
                    //data: '{_password:"' + currentPass + '"}'
                }).then(function (response) {
                    var k = JSON.parse(JSON.stringify(response.data));
                    if (k == 1) {
                        $(".adminDashboard").hide();
                        $("#hosaccUpdateBox").hide();
                        $("#myModal").hide();
                        $("#hosaccPassBox").show();
                    } else {
                        Swal.fire({
                            title: "Invalid Credentials",
                            icon: "error"
                        }).then(function () {
                            $("#verifyadminpass").val("");
                        })
                    }
                   
                }, function (error) {
                    alert("Validate admin password fail..." + error);
                })
            }
        })
    }

    //check current password
    jQuery.validator.addMethod("checkSK", function (value, element) {
        var currentSK = $("#secretKey").val();
        var userid = $scope.currentHosDetails.userId;
        var isExist = true;

        $.ajax({
            type: "POST",
            url: "/Admin/ValidateSK",
            data: '{ currentSK:"' + currentSK + '",' + 'currentid:"' + userid + '"}',
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
                alert("AJAX loading error...");
            }
        });
        return isExist;
    })

    //Update Hospital Account Password - reset hos password
    $scope.resetUpdateHosPass = function (userid, password) {
        $scope.password = {
            userid: userid,
            password: password
        }

        $("#resetHosPass").validate({
            rules: {
                secretKey: {
                    required: true,
                    checkSK: true
                },
                resetNewHosPass: "required",
                resetNewHosConfirmPass: {
                    required: true,
                    equalTo: "#resetNewHosPass"
                }
            },
            messages: {
                secretKey: {
                    required: "Secret key is required",
                    checkSK:"Invalid secret key"
                },
                resetNewHosPass: "New password is required",
                resetNewHosConfirmPass: {
                    required: "Please enter password again.",
                    equalTo:"Password do not match"
                }
            },
            submitHandler: function (form) {
                $http({
                    method: "POST",
                    url: '/Admin/UpdateHosPass',
                    data: $scope.password
                }).then(function (response) {
                    if (response.status == 200) {
                        Swal.fire({
                            title: "Password Update Success!",
                            icon: "success"
                        }).then(function () {
                            $("#secretKey").val("");
                            $("#resetNewHosPass").val("");
                            $("#resetNewHosConfirmPass").val("");
                            location.reload();
                        });
                    }
                }, function (error) {
                    alert("Reset Hospital Authority Account Password fail " + error);
                })
            }
        })
    }

    //Feedbacks Listing
    $scope.feedback_listing = function () {
        $http({
            method: "GET",
            url: '/Admin/GetFeedbacksListing'
        }).then(function (response) {
            $scope.fbrecords = response.data;
        }, function (error) {
            alert("Authority Account Listing Loading Error...");
        })
    }

    $scope.feedback_listing();
})