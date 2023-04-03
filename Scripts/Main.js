$(document).ready(function () {
    (function () {
        "use strict";

    /*--------------------------------------------------------------
    #User Navbar 
    --------------------------------------------------------------*/
        /**
        * Pop up sign in
        */

        // Get the <span> element that closes the modal
        var span = document.getElementsByClassName("close")[0];

        // When the user clicks on <span> (x), close the modal
        span.onclick = function () {
            modal.style.display = "none";
        }

        // When the user clicks anywhere outside of the modal, close it
        window.onclick = function (event) {
            if (event.target == modal) {
                console.log({ event });
                modal.style.display = "none";
            }
        }

        const signUpButton = document.getElementById('signUp');
        const signInButton = document.getElementById('signIn');
        const container = document.getElementById('pop-up-container');

        signUpButton.addEventListener('click', () => {
            container.classList.add("right-panel-active");
        });

        signInButton.addEventListener('click', () => {
            container.classList.remove("right-panel-active");
        });

        /**
       * Pop up forgot password
       */
        $("#forgotpass-btn").click(function () {
            $(".forgotpass-container").show();
            $(".otp-container").hide();
            $(".overlay-container").hide();
            $(".sign-in-container").hide();
            $(".sign-up-container").hide();
        });

        $("#forgotpass-backbtn").click(function () {
            $(".forgotpass-container").hide();
            $(".sign-in-container").show();
            $(".sign-up-container").show();
            $(".overlay-container").show();
        });

    /*--------------------------------------------------------------
    #User HomePage
    --------------------------------------------------------------*/

        /**
         * Easy selector helper function
         */
        const select = (el, all = false) => {
            el = el.trim()
            if (all) {
                return [...document.querySelectorAll(el)]
            } else {
                return document.querySelector(el)
            }
        }

        /**
         * Easy event listener function
         */
        const on = (type, el, listener, all = false) => {
            let selectEl = select(el, all)
            if (selectEl) {
                if (all) {
                    selectEl.forEach(e => e.addEventListener(type, listener))
                } else {
                    selectEl.addEventListener(type, listener)
                }
            }
        }

        /**
         * Easy on scroll event listener 
         */
        const onscroll = (el, listener) => {
            el.addEventListener('scroll', listener)
        }

        /**
         * Toggle .header-scrolled class to #header when page is scrolled
         */
        let selectHeader = select('#header')
        let selectTopbar = select('#topbar')
        if (selectHeader) {
            const headerScrolled = () => {
                if (window.scrollY > 100) {
                    selectHeader.classList.add('header-scrolled')
                    if (selectTopbar) {
                        selectTopbar.classList.add('topbar-scrolled')
                    }
                } else {
                    selectHeader.classList.remove('header-scrolled')
                    if (selectTopbar) {
                        selectTopbar.classList.remove('topbar-scrolled')
                    }
                }
            }
            window.addEventListener('load', headerScrolled)
            onscroll(document, headerScrolled)
        }

        /**
         * Back to top button
         */
        let backtotop = select('.back-to-top')
        if (backtotop) {
            const toggleBacktotop = () => {
                if (window.scrollY > 100) {
                    backtotop.classList.add('active')
                } else {
                    backtotop.classList.remove('active')
                }
            }
            window.addEventListener('load', toggleBacktotop)
            onscroll(document, toggleBacktotop)
        }

        /**
         * Mobile nav toggle
         */
        on('click', '.mobile-nav-toggle', function (e) {
            select('#navbar').classList.toggle('navbar-mobile')
            this.classList.toggle('bi-list')
            this.classList.toggle('bi-x')
        })

        /**
         * Mobile nav dropdowns activate
         */
        on('click', '.navbar .dropdown > span', function (e) {
            if (select('#navbar').classList.contains('navbar-mobile')) {
                e.preventDefault()
                this.nextElementSibling.classList.toggle('dropdown-active')
            }
        }, true)

        /**
         * Scrool with ofset on links with a class name .scrollto
         */
        on('click', '.scrollto', function (e) {
            if (select(this.hash)) {
                e.preventDefault()

                let navbar = select('#navbar')
                if (navbar.classList.contains('navbar-mobile')) {
                    navbar.classList.remove('navbar-mobile')
                    let navbarToggle = select('.mobile-nav-toggle')
                    navbarToggle.classList.toggle('bi-list')
                    navbarToggle.classList.toggle('bi-x')
                }
                scrollto(this.hash)
            }
        }, true)

        /**
         * Scroll with ofset on page load with hash links in the url
         */
        window.addEventListener('load', () => {
            if (window.location.hash) {
                if (select(window.location.hash)) {
                    scrollto(window.location.hash)
                }
            }
        });

        /**
         * Preloader
         */
        let preloader = select('#preloader');
        if (preloader) {
            window.addEventListener('load', () => {
                preloader.remove()
            });
        }
    })()

});
