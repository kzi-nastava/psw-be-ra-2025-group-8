# Remove migrations folders
Remove-Item -Recurse -Force "Modules/Stakeholders/Explorer.Stakeholders.Infrastructure/Migrations"
Remove-Item -Recurse -Force "Modules/Tours/Explorer.Tours.Infrastructure/Migrations"
Remove-Item -Recurse -Force "Modules/Blog/Explorer.Blog.Infrastructure/Migrations"

# Update StakeholdersContext
Add-Migration -Name Init -Context StakeholdersContext -Project Explorer.Stakeholders.Infrastructure -StartupProject Explorer.API
Update-Database -Context StakeholdersContext -Project Explorer.Stakeholders.Infrastructure -StartupProject Explorer.API

# Update ToursContext
Add-Migration -Name Init -Context ToursContext -Project Explorer.Tours.Infrastructure -StartupProject Explorer.API
Update-Database -Context ToursContext -Project Explorer.Tours.Infrastructure -StartupProject Explorer.API

# Update BlogContext
Add-Migration -Name Init -Context BlogContext -Project Explorer.Blog.Infrastructure -StartupProject Explorer.API
Update-Database -Context BlogContext -Project Explorer.Blog.Infrastructure -StartupProject Explorer.API
