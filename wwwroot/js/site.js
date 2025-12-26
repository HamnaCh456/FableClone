// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
   const sidebar = document.getElementById('appSidebar');
   const mainContent = document.querySelector('.main-content');
   const toggleBtn = document.getElementById('sidebarToggle');

   if (sidebar && toggleBtn) {
      toggleBtn.addEventListener('click', function (e) {
         e.stopPropagation();
         sidebar.classList.toggle('collapsed');
         if (mainContent) {
            mainContent.classList.toggle('expanded');
         }
      });
   }
});
