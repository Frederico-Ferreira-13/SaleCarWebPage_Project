/**
 * Lógica de Navegação Luxury Stand
 * O efeito de expansão (Abrir/Fechar) é gerido via CSS (:hover)
 */

document.addEventListener("DOMContentLoaded", function () {
    // Selecionamos todos os itens da lista da navbar
    const list = document.querySelectorAll('.navigation .list');

    // Função para destacar o link clicado
    function activeLink() {
        list.forEach((item) => {
            item.classList.remove('active');
        });
        this.classList.add('active');
    }

    // Adicionamos o evento de clique a cada item
    list.forEach((item) => {
        item.addEventListener('click', activeLink);
    });
});