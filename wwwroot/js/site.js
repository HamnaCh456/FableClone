// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function () {
   const sidebar = document.getElementById('appSidebar');
   const toggleBtn = document.getElementById('sidebarToggle');

   if (sidebar && toggleBtn) {
      toggleBtn.addEventListener('click', function (e) {
         e.stopPropagation(); // Prevent document click from immediately closing it if we implemented outside click logic
         sidebar.classList.toggle('expanded');
      });

      // Optional: Close sidebar when clicking outside (on main content)
      document.addEventListener('click', function (e) {
         if (sidebar.classList.contains('expanded') &&
            !sidebar.contains(e.target) &&
            e.target !== toggleBtn) {
            sidebar.classList.remove('expanded');
         }
      });
   }
});
