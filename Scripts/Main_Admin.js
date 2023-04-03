
function displayHospitalPartnersDropdown() {
    if (document.getElementById('dropdown-content-hospital-partners').style.display == "block") {
        document.getElementById('dropdown-content-hospital-partners').style.display = "none";
    }
    else {
        document.getElementById('dropdown-content-hospital-partners').style.display = "block";
    }
}

function displayUsersDropdown() {
    if (document.getElementById('dropdown-content-feedbacks').style.display == "block") {
        document.getElementById('dropdown-content-feedbacks').style.display = "none";
    }
    else {
        document.getElementById('dropdown-content-feedbacks').style.display = "block";
    }
}

function display_ct() {
    var ct = new Date();
    document.getElementById("time").innerHTML = ct.toLocaleTimeString();
    display_c();
}

function display_c() {
    var refresh = 1000; // Refresh rate in milli seconds
    mytime = setTimeout('display_ct()', refresh)
}

/**
 * Pop up sign in
 */

// Get the modal
var modal = document.getElementById("myModal");
var passcon = document.getElementById("hosforgotpass-container");

//onclick function
function openHosModalBox() {
    // When the user clicks the button, open the modal
    modal.style.display = "block";
    passcon.style.display = "block";
}

window.onclick = function (event) {
    if (event.target == modal) {
        console.log({ event });
        modal.style.display = "none";
    }
}