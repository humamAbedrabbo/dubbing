CREATE VIEW [dbo].[userRolesView]
AS
SELECT     UserId, RoleId
FROM         dbo.webpages_UsersInRoles
;
