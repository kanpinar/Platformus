BEGIN TRANSACTION;
--
-- Extension: Platformus.Configurations
-- Version: alpha-18
--
INSERT INTO "Configurations" VALUES (1,'Email','Email');
INSERT INTO "Configurations" VALUES (2,'Globalization','Globalization');
INSERT INTO "Variables" VALUES (1,1,'SmtpServer','SMTP server','test',1);
INSERT INTO "Variables" VALUES (2,1,'SmtpPort','SMTP port','25',2);
INSERT INTO "Variables" VALUES (3,1,'SmtpLogin','SMTP login','test',3);
INSERT INTO "Variables" VALUES (4,1,'SmtpPassword','SMTP password','test',4);
INSERT INTO "Variables" VALUES (5,1,'SmtpSenderEmail','SMTP sender email','test',5);
INSERT INTO "Variables" VALUES (6,1,'SmtpSenderName','SMTP sender name','test',6);
INSERT INTO "Variables" VALUES (7,2,'SpecifyCultureInUrl','Specify culture in URL','yes',1);

--
-- Extension: Platformus.Security
-- Version: alpha-18
--
INSERT INTO "Users" VALUES (1,'Administrator','2017-01-01 00:00:00.0000000');
INSERT INTO "CredentialTypes" VALUES (1,'Email','Email',1);
INSERT INTO "Credentials" VALUES (1,1,1,'admin@platformus.net','21-23-2F-29-7A-57-A5-A7-43-89-4A-0E-4A-80-1F-C3');
INSERT INTO "Roles" VALUES (1,'Administrator','Administrator',100);
INSERT INTO "Roles" VALUES (2,'ApplicationOwner','Application owner',200);
INSERT INTO "Roles" VALUES (3,'ContentManager','Content manager',300);
INSERT INTO "UserRoles" VALUES (1,1);
INSERT INTO "UserRoles" VALUES (1,2);
INSERT INTO "Permissions" VALUES (1,'BrowseBackend','Browse backend',1);
INSERT INTO "Permissions" VALUES (2,'DoEverything','Do everything',100);
INSERT INTO "Permissions" VALUES (3,'BrowseConfigurations','Browse configurations',200);
INSERT INTO "Permissions" VALUES (4,'BrowsePermissions','Browse permissions',300);
INSERT INTO "Permissions" VALUES (5,'BrowseRoles','Browse roles',310);
INSERT INTO "Permissions" VALUES (6,'BrowseUsers','Browse users',320);
INSERT INTO "Permissions" VALUES (7,'BrowseFileManager','Browse file manager',400);
INSERT INTO "Permissions" VALUES (8,'BrowseCultures','Browse cultures',500);
INSERT INTO "Permissions" VALUES (9,'BrowseObjects','Browse objects',600);
INSERT INTO "Permissions" VALUES (10,'BrowseDataTypes','Browse data types',610);
INSERT INTO "Permissions" VALUES (11,'BrowseClasses','Browse classes',620);
INSERT INTO "Permissions" VALUES (12,'BrowseMicrocontrollers','Browse microcontrollers',630);
INSERT INTO "Permissions" VALUES (13,'BrowseMenus','Browse menus',700);
INSERT INTO "Permissions" VALUES (14,'BrowseForms','Browse forms',800);
INSERT INTO "Permissions" VALUES (15,'BrowseViews','Browse views',900);
INSERT INTO "Permissions" VALUES (16,'BrowseStyles','Browse styles',910);
INSERT INTO "Permissions" VALUES (17,'BrowseScripts','Browse scripts',920);
INSERT INTO "Permissions" VALUES (18,'BrowseBundles','Browse bundles',930);
INSERT INTO "RolePermissions" VALUES (1,1);
INSERT INTO "RolePermissions" VALUES (2,2);
INSERT INTO "RolePermissions" VALUES (3,7);
INSERT INTO "RolePermissions" VALUES (3,9);
INSERT INTO "RolePermissions" VALUES (3,13);
INSERT INTO "RolePermissions" VALUES (3,14);

--
-- Extension: Platformus.Globalization
-- Version: alpha-18
--
INSERT INTO "Cultures" VALUES (1,'__','Neutral',1,0);
INSERT INTO "Cultures" VALUES (2,'en','English',0,1);

--
-- Extension: Platformus.Domain
-- Version: alpha-18
--
INSERT INTO "DataTypes" VALUES (1,'string','singleLinePlainText','Single line plain text',1);
INSERT INTO "DataTypes" VALUES (2,'string','multilinePlainText','Multiline plain text',2);
INSERT INTO "DataTypes" VALUES (3,'string','html','Html',3);
INSERT INTO "DataTypes" VALUES (4,'string','image','Image',4);
INSERT INTO "DataTypes" VALUES (5,'datetime','date','Date',5);

--
-- Extension: Platformus.Forms
-- Version: alpha-18
--
INSERT INTO "FieldTypes" VALUES (1,'TextBox','Text box',1);
INSERT INTO "FieldTypes" VALUES (2,'TextArea','Text area',2);
INSERT INTO "FieldTypes" VALUES (3,'DropDownList','Drop down list',3);
INSERT INTO "FieldTypes" VALUES (4,'FileUpload','File upload',4);
INSERT INTO "DataTypeParameters" VALUES (1,1,'temp','IsRequired','Is required');
INSERT INTO "DataTypeParameters" VALUES (2,1,'temp','MaxLength','Max length');
INSERT INTO "DataTypeParameters" VALUES (3,2,'temp','IsRequired','Is required');
INSERT INTO "DataTypeParameters" VALUES (4,2,'temp','MaxLength','Max length');
INSERT INTO "DataTypeParameters" VALUES (5,4,'temp','Width','Width');
INSERT INTO "DataTypeParameters" VALUES (6,4,'temp','Height','Height');
INSERT INTO "DataTypeParameters" VALUES (7,5,'temp','IsRequired','Is required');

COMMIT;
