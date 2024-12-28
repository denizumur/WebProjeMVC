document.addEventListener('DOMContentLoaded', () => {
    // Menü açma ve kapama fonksiyonu
    const burger = document.querySelector('.burger');
    burger.addEventListener('click', () => {
        document.body.classList.toggle('open');
    });

    // Dropdown menüler için açma ve kapama işlemleri
    const dropdownButtons = document.querySelectorAll('.dropdown .button');
    dropdownButtons.forEach(button => {
        button.addEventListener('click', (event) => {
            const dropdownMenu = button.nextElementSibling;
            dropdownMenu.classList.toggle('open');
            event.stopPropagation();
        });
    });

    // Sayfa dışında bir yere tıklanınca dropdown menüyü kapatma
    document.addEventListener('click', () => {
        const openDropdowns = document.querySelectorAll('.dropdown-menu.open');
        openDropdowns.forEach(menu => {
            menu.classList.remove('open');
        });
    });
});