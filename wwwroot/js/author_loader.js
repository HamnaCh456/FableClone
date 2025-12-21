document.addEventListener("DOMContentLoaded", function () {

    const authorTab = document.querySelector('a[href="#author"]');
    const contentArea = document.getElementById("dynamic-content");

    authorTab.addEventListener("click", function (e) {
        e.preventDefault();

        // AuthorId passed from About.cshtml model
        var authorId = document.getElementById("AuthorIdValue").value;

        fetch(`/Book/AuthorPartial?authorId=${authorId}`)
            .then(res => res.text())
            .then(html => {
                contentArea.innerHTML = html;
            });
    });
});
