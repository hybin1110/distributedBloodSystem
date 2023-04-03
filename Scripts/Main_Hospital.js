/*--------------------------------------------------------------
#Hospital Authority Sidebar
--------------------------------------------------------------*/
function displayBloodLevelDropdown() {
    if (document.getElementById('dropdown-content-blood-level').style.display == "block") {
        document.getElementById('dropdown-content-blood-level').style.display = "none";
    }
    else {
        document.getElementById('dropdown-content-blood-level').style.display = "block";
    }
}
function displayBloodQuanlityDropdown() {
    if (document.getElementById('dropdown-content-blood-quanlity').style.display == "block") {
        document.getElementById('dropdown-content-blood-quanlity').style.display = "none";
    }
    else {
        document.getElementById('dropdown-content-blood-quanlity').style.display = "block";
    }
}

function display_ct() {
    var ct = new Date();
    document.getElementById("hos-time").innerHTML = ct.toLocaleTimeString();
    display_c();
}

function display_c() {
    var refresh = 1000; // Refresh rate in milli seconds
    mytime = setTimeout('display_ct()', refresh)
}

function setClicked() {
    var data = sessionStorage.getItem('clicked');
    if (data == 'true') {
        $("#quality-records-container").css("display", "block");
        sessionStorage.setItem('clicked', true);
    }
}

//window.onload = function () {
//    display_ct()

//    if (window.location.href.match('https://localhost:44371/Hospital/BloodQuality_Listing') != null) {

//    }
//}

