var app = angular.module('homeapp', []);

app.controller("HomeController", function myfunction($scope, $http) {
    /*-----------------------------------------------SIGN IN------------------------------------------ */
    $scope.btnsignin = "Sign In"

    $scope.login = function () {
        $(".signinform").validate({
            rules: {
                signinemail: "required",
                signinpassword: "required"
            },
            messages: {
                signinemail: "Email is required",
                signinpassword: "Password is required"
            },
            submitHandler: function (form) {
                $http({
                    method: "POST",
                    url: '/Home/userlogin',
                    data: $scope.usersignin
                }).then(function (response) {
                    //Parse the data becomes a JavaScript object.
                    //Convert a JavaScript object into json string with JSON.stringify().
                    //cloning an object that get a complete copy that is unique but has the same properties as the cloned object.
                    var k = JSON.parse(JSON.stringify(response.data));
                    if (k.message == 'Login Successful') {
                        if (k.role == 'Admin') {
                            //$('#signinbtn').html('Log Out');
                            window.location = '/Admin/AdminDashboard';
                        } else if (k.role == 'Hospital Authority') {
                            if (k.status == 'Active') {
                                window.location = '/Hospital/Dashboard';
                            } else {
                                Swal.fire(
                                    'Account Inactive',
                                    'Please Contact Site Administrator',
                                    'error'
                                )
                            }
                        } else {
                            window.location = '/Home/Index';
                        }
                    } else {
                        Swal.fire(
                            'Invalid Credentials',
                            'Please check your email or password',
                            'error'
                        )
                    }
                    $scope.user = null;
                }, function (error) {
                    alert("LOGIN ERROR. Contact System Administrator");
                })
            }
        });
    }

    /*-----------------------------------------------SIGN UP------------------------------------------ */
    jQuery.validator.addMethod("checkemail", function (value, element) {
        var inputEmail = $("#signupemail").val();
        var isExist = false;
        //$('.signupform :input[name="signupemail"]');
        $.ajax({
            type: "POST",
            url: "/Home/validateuser",
            data: '{_email:"' + inputEmail + '"}',
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
            }, error: function (data) {
                alert("AJAX loading error...");
            }
            // return false;

        });
        return isExist;
    })

    //jQuery.validator.addMethod("regex", function (value, element) {
    //    return this.optional(element) || /^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$/.test(value);
    //},
    //    "Passsword should contain letter, digit and special character"
    //);

    //jQuery.validator.addMethod("alphanumeric", function (value, element) {
    //    return this.optional(element) || /^[a-zA-Z0-9]+$/.test(value);
    //}, "Letters, numbers, and underscores only please");

    $.validator.addMethod("regx", function (value, element, regexpr) {
        return regexpr.test(value);
    });

    $scope.signup = function (_name, _email, _password) {
        $scope.usersignupdetail = {
            email: _email,
            name: _name,
            password: _password
        };

        $("form[name='signupform']").validate({
            rules: {
                signupname: "required",
                signuppassword: {
                    required: true,
                    equalTo: "#signuppass"
                },
                signupemail: {
                    required: true,
                    email: true,
                    checkemail: true
                },
                signuppass: {
                    required: true,
                    regex: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/
                }
            },
            messages: {
                signupname: "Name is required",
                signuppassword: {
                    required: "Confirm password is required",
                    equalTo: "Password not match"
                },
                signupemail: {
                    required: "Email is required",
                    email: "Email format wrong",
                    checkemail: "Account Exists. Please proceed to login."
                },
                signuppass: {
                    required:"Password is required",
                    regex: "Invalid password format. Minimum eight characters, at least one letter and one digit"
                    }
            },
            //errorPlacement: function (error, element) {
            //    error.insertAfter(element);
            //    alert(error.html());
            //},
            submitHandler: function (form) {
                $http({
                    method: "POST",
                    url: '/Home/usersignup',
                    data: $scope.usersignupdetail
                }).then(function (response) {
                    //console.log(response.status);
                    //var k = JSON.parse(JSON.stringify(response.data));
                    if (response.status == 200) {
                        Swal.fire({
                            title: "Register Success!", text: "You're one step away, let us know you more.",
                            icon: "success"
                        }).then(function () {
                            window.location = '/Profile/completeProfile';
                            //location.reload();
                        });
                    }
                    //window.location = '/Home/Index';
                    //if (response.success == "Success") {
                    //     
                    //}
                }, function (error) {
                    alert("Register fail...");
                    //});
                });
            }

        });
    }

    /*---------------------------------------REQUEST RESET PASSWORD----------------------------------- */
    jQuery.validator.addMethod("checkresetemail", function (value, element) {
        var inputEmail = $("#resetemail").val();
        var isExist = false;

        $.ajax({
            type: "POST",
            url: "/Home/validateuser",
            data: '{_email:"' + inputEmail + '"}',
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

    $scope.forgotpass = function (_resetemail) {

        var inputEmail = $("#resetemail").val();

        $("form[name='requestpassform']").validate({
            rules: {
                resetemail: {
                    required: true,
                    checkresetemail: true
                }
            },
            messages: {
                resetemail: {
                    required: "Email is required.",
                    checkresetemail: "Account not exist"
                }
            },
            submitHandler: function (form) {
                $http({
                    method: "POST",
                    url: '/Home/ForgotPassword',
                    data: '{_email:"' + inputEmail + '"}'
                }).then(function (response) {
                    //console.log(response.status);
                    //var k = JSON.parse(JSON.stringify(response.data));
                    if (response.status == 200) {
                        Swal.fire({
                            title: "Reset Password Email Sent", text: "Please Check Your Inbox", icon:
                                "success"
                        }).then(function () {
                            location.reload();
                        });
                    }
                }, function (error) {
                    alert("Reset password fail...");
                });
            }
        })
    }

    /*-----------------------------------------RESET PASSWORD------------------------------------------ */
    $scope.resetpass = function (_resetpass) {
        var newpassword = $("#resetConfirmPassword").val()

        $("form[name='resetpassform']").validate({
            rules: {
                resetpassword: {
                    required: true,
                    regx: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$/
                    },
                resetConfirmPassword: {
                    required: true,
                    equalTo: "#resetpassword"
                }
            },
            messages: {
                resetpassword: {
                    required: "New password is required",
                    regx:"Invalid password format. Minimum eight characters, at least one letter and one digit"
                },
                resetConfirmPassword: {
                    required: "Please Re-enter your password",
                    equalTo: "Password does not match"
                }
            },
            submitHandler: function (form) {
                $http({
                    method: "POST",
                    url: '/Home/UpdateResetPassword',
                    data: '{_password:"' + newpassword + '"}'
                }).then(function (response) {
                    if (response.status == 200) {
                        Swal.fire({
                            title: "Password Successfully Reset!", text: "Please Proceed To Login", icon:
                                "success"
                        }).then(function () {
                            window.location = '/Home/Index';
                        });
                    }

                }, function (error) {
                    alert("update new password fail...");
                });
            }
        })
    }

    /*-----------------------------------------------FEEDBACKS------------------------------------------ */
    $scope.saveFeedback = function () {
        $("#contact-us-form").validate({
            rules: {
                fname: "required",
                lname: "required",
                cemail: {
                    required: true,
                    email: true
                },
                cmessage: "required"
            },
            messages: {
                fname: "First name is required",
                lname: "Last name is required",
                cemail: {
                    required: "Email Address is required",
                    email: "Invalid email format"
                },
                cmessage: "Message is required"
            },
            submitHandler: function (form) {
                $http({
                    method: "POST",
                    url: '/Home/saveFeedback',
                    data: $scope.feedback
                }).then(function (response) {
                    if (response.status == 200) {
                        Swal.fire({
                            title: "Thank You For Your Message!", text: "We'll Get Back To You Soon.", icon:
                                "success"
                        });

                        $scope.feedback = null;
                    }
                }, function (error) {
                    alert("Save Feedback Fail..." + error);
                })
            }
        })
    }

    /*-----------------------------------------------BLOOD LEVEL------------------------------------------ */
    $scope.bloodlevel = function () {
        $http({
            method: "GET",
            url: '/Home/GetBloodLevel'
        }).then(function (response) {
            $scope.bloodlvlrec = response.data;
        }, function (error) {
            alert("Blood Level Records Loading Error...");
        })
    }

    $scope.bloodlevel();
})