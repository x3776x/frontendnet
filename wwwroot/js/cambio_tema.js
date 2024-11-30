document.addEventListener('DOMContentLoaded', () => {
    const htmlElement = document.documentElement;
    const switchElement = document.getElementById('darkModeSwitch');
    const modoLuz = document.querySelector('.modo-luz');

    // Verifica que los elementos necesarios existan
    if (!switchElement || !modoLuz) {
        console.error('Elementos necesarios para el cambio de tema no encontrados.');
        return;
    }

    // Establece el tema guardado o 'light' como predeterminado
    const currentTheme = localStorage.getItem('bsTheme') || 'light';
    htmlElement.setAttribute('data-bs-theme', currentTheme);
    switchElement.checked = currentTheme === 'dark';
    actualizarIcono(currentTheme);

    // Agrega el listener para el interruptor de tema
    switchElement.addEventListener('change', function () {
        const newTheme = this.checked ? 'dark' : 'light';
        htmlElement.setAttribute('data-bs-theme', newTheme);
        localStorage.setItem('bsTheme', newTheme);
        actualizarIcono(newTheme);
    });

    // Actualiza el icono basado en el tema
    function actualizarIcono(theme) {
        const isDark = theme === 'dark';
        modoLuz.classList.toggle('bi-brightness-high', !isDark);
        modoLuz.classList.toggle('bi-moon-stars', isDark);
    }
});
