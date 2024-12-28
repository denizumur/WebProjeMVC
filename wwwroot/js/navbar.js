feather.replace()
document.addEventListener('DOMContentLoaded', () => {
    const burger = document.querySelector('.burger');
    burger.addEventListener('click', () => {
        document.body.classList.toggle('open');
    });
});