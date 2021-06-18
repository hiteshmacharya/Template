Before executing command, select default project as project for which you are running migration.
Set as Default project for which migration is being executing.

How to add Migration for DataContext
Add-Migration [MigrationName] -Context AdminContext -OutputDir "Infrastructure/Migrations"

How to update migration in database
Update-Database -Context AdminContext -Migration [MigrationName]


