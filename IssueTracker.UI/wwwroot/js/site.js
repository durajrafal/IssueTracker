const navbarLinks = document.querySelectorAll("#navbarMain > ul:nth-child(1) > li > a")

document.addEventListener('DOMContentLoaded', () => {
    navbarLinks.forEach(link => {
        if (link.getAttribute('href').toLowerCase() === location.pathname.toLowerCase()) {
            link.classList.add('active');
        } else {
            link.classList.remove('active');
        }
    });
})