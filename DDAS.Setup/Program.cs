using DDAS.API.Identity;
using DDAS.Data.Mongo;
using DDAS.Data.Mongo.Indexes;
using DDAS.Data.Mongo.Maps;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using DDAS.Services.AppAdminService;
using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using Utilities;

namespace DDAS.Setup
{
    class Program
    {
        private static LogText _WriteLog;
        private static UnitOfWork _UOW;
        private static AppAdminService _AppAdminService;
        private static IConfig _config;

        static void Main(string[] args)
        {
            try
            {
                string exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                
                _WriteLog = new LogText(exePath + @"\setup.log", true);
                _WriteLog.LogStart();
                _WriteLog.WriteLog("DDAS Setup Utility");
                _WriteLog.WriteLog("========================================================================");
                _WriteLog.WriteLog(DateTime.Now.ToString(), "Start set up at " + exePath);
                string configFile = ConfigurationManager.AppSettings["APIWebConfigFile"];
                if (configFile != null)
                {
                    CreateFolders(configFile);

                    //Initialize DB for creating Roles and Users:
                    string connString = GetWebConfigConnectionString(configFile, "DefaultConnection");
                     MongoMaps.Initialize();
                    _UOW = new UnitOfWork(connString);
                    CreateRoles();
                    CreateUsers();

                    _config = new Config(); //empty values for config prop.. ?
                    _AppAdminService = new AppAdminService(_UOW, _config);

                    //Executed on FindMeServerOn 1April2017.
                    var sitesInDB = _UOW.SiteSourceRepository.GetAll();

                    //Executed on DDASTEST server on 18Sept2017
                    //SAM site ExtractionMode was not updated to 'DB' - changed
                    //_WriteLog.WriteLog("Removing sites..");
                    //_UOW.SiteSourceRepository.DropAll(
                    //    _UOW.SiteSourceRepository.GetAll().FirstOrDefault());

                    //_WriteLog.WriteLog("Removing Default sites..");
                    //_UOW.DefaultSiteRepository.DropAll(
                    //    _UOW.DefaultSiteRepository.GetAll().FirstOrDefault());

                    _WriteLog.WriteLog("site Sources InDB:", sitesInDB.Count.ToString());
                    if (sitesInDB.Count > 0)
                    {
                        _WriteLog.WriteLog("Sites already exist.  Not added");
                    }
                    else
                    {
                        _WriteLog.WriteLog("Adding Sites");
                        SitesToSearch Sites = new SitesToSearch();
                        _AppAdminService.AddSitesInDbCollection(Sites);
                    }

                    //Executed on 20-4-2017
                    //ModifySiteSource_ChangeLive2DB();

                    //CreateIndexes();
                }
                else
                {
                    _WriteLog.WriteLog("Entry in AppSettings : APIWebConfigFile" + " not found");
                }
             }
            catch (Exception ex)
            {
                _WriteLog.WriteLog("Error: ", ex.Message + " " +  ex.InnerException);
            }
            finally
            {
                _WriteLog.WriteLog(DateTime.Now.ToString(), "End setup");
                _WriteLog.LogEnd();
                _WriteLog.Dispose();
            }
          }

        static void CreateFolders(string configFile)
        {
            var appRootFolder = Path.GetDirectoryName(configFile);
            //Folders:
            _WriteLog.WriteLog("Reading Web.config for:", "DataExtractionLogFile");
            string DataExtractionLogFile = GetWebConfigAppSetting(configFile, "DataExtractionLogFile");
            string folder = Path.GetDirectoryName(DataExtractionLogFile);
            CreateFolder(appRootFolder + @"\" + folder);

            _WriteLog.WriteLog("Reading Web.config for:", "AppDataDownloadFolder");
            string DownloadFolder = GetWebConfigAppSetting(configFile, "AppDataDownloadsFolder");
            CreateFolder(appRootFolder + @"\" + DownloadFolder);

            _WriteLog.WriteLog("Reading Web.config for:", "UploadsFolder");
            string UploadFolder = GetWebConfigAppSetting(configFile, "UploadsFolder");
            CreateFolder(appRootFolder + @"\" + UploadFolder);

            _WriteLog.WriteLog("Reading Web.config for:", "ExcelTemplateFolder");
            string ExcelTemplateFolder = GetWebConfigAppSetting(configFile, "ExcelTemplateFolder");
            CreateFolder(appRootFolder + @"\" + ExcelTemplateFolder);
            //Copy Excel Template from ...

            _WriteLog.WriteLog("Reading Web.config for:", "WordTemplateFolder");
            string WordTemplateFolder = GetWebConfigAppSetting(configFile, "WordTemplateFolder");
            CreateFolder(appRootFolder + @"\" + WordTemplateFolder);
            //Copy Word Template from ...

            _WriteLog.WriteLog("Reading Web.config for:", "ErrorScreenCaptureFolder");
            string ErrorScreenCaptureFolder = GetWebConfigAppSetting(configFile, "ErrorScreenCaptureFolder");
            CreateFolder(appRootFolder + @"\" + ErrorScreenCaptureFolder);

            _WriteLog.WriteLog("Reading Web.config for:", "OutputFileFolder");
            string OutputFileFolder = GetWebConfigAppSetting(configFile, "OutputFileFolder");
            CreateFolder(appRootFolder + @"\" + OutputFileFolder);

            _WriteLog.WriteLog("Reading Web.config for:", "AttachmentsFolder");
            string AttachmentsFolder = GetWebConfigAppSetting(configFile, "AttachmentsFolder");
            CreateFolder(appRootFolder + @"\" + AttachmentsFolder);

            _WriteLog.WriteLog("Reading Web.config for:", "ComplianceFormFolder");
            string ComplianceFormFolder = GetWebConfigAppSetting(configFile, "ComplianceFormFolder");
            CreateFolder(appRootFolder + @"\" + ComplianceFormFolder);

            _WriteLog.WriteLog("Reading Web.config for:", "CIILFolder");
            string CIILFolder = GetWebConfigAppSetting(configFile, "CIILFolder");
            CreateFolder(appRootFolder + @"\" + CIILFolder);

            _WriteLog.WriteLog("Reading Web.config for:", "FDAWarningLettersFolder");
            string FDAWarningLettersFolder = GetWebConfigAppSetting(configFile, "FDAWarningLettersFolder");
            CreateFolder(appRootFolder + @"\" + FDAWarningLettersFolder);

            _WriteLog.WriteLog("Reading Web.config for:", "ExclusionDatabaseFolder");
            string ExclusionDatabaseFolder = GetWebConfigAppSetting(configFile, "ExclusionDatabaseFolder");
            CreateFolder(appRootFolder + @"\" + ExclusionDatabaseFolder);

            _WriteLog.WriteLog("Reading Web.config for:", "SAMFolder");
            string SAMFolder = GetWebConfigAppSetting(configFile, "SAMFolder");
            CreateFolder(appRootFolder + @"\" + SAMFolder);

            _WriteLog.WriteLog("Reading Web.config for:", "SDNFolder");
            string SDNFolder = GetWebConfigAppSetting(configFile, "SDNFolder");
            CreateFolder(appRootFolder + @"\" + SDNFolder);
        }

        static void CreateRoles()
        {
            CreateRole("admin");
            CreateRole("user");
            CreateRole("app-admin");
        }

        static void CreateUsers()
        {
            CreateUser("user1", "user", "Pass!234");
            CreateUser("admin1", "admin", "Pass!234");
            CreateUser("appadmin1", "app-admin", "Pass!234");
        }

        static void CreateFolder(string path)
        {
            _WriteLog.WriteLog("Creating Directory: " + path);
            try
            {
                if (Directory.Exists(path))
                {
                    _WriteLog.WriteLog("Directory exists" );
                }
                else
                {
                    Directory.CreateDirectory(path);
                    _WriteLog.WriteLog("Directory created");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        static string GetWebConfigAppSetting(string configFile, string keyName)
        {
            string error;
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            Configuration config =
                ConfigurationManager.OpenMappedExeConfiguration
                (fileMap, ConfigurationUserLevel.None);
            if (config == null)
            {
                error = "Config file : " + configFile + " could not be loaded.";
                _WriteLog.WriteLog(error);
                throw new Exception(error);
            }
            else
            {
                KeyValueConfigurationElement settings = config.AppSettings.Settings[keyName];
                if (settings != null)
                {
                    _WriteLog.WriteLog("Key : " + keyName + ", Value: " + settings.Value);
                    return settings.Value;
                }
                else
                {
                    //cannot use settings.value when settings == null
                    //error = "Key : " + keyName + ", Value: " + settings.Value + " could not be read";
                    error = "Key : " + keyName + " could not be read";
                    _WriteLog.WriteLog(error);
                    throw new Exception(error);
                }
            }
      
        }

        static string GetWebConfigConnectionString(string configFile, string keyName)
        {
            string error;
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
            fileMap.ExeConfigFilename = configFile;
            Configuration config =
                ConfigurationManager.OpenMappedExeConfiguration
                (fileMap, ConfigurationUserLevel.None);
            if (config == null)
            {
                error = "Config file : " + configFile + " could not be loaded.";
                _WriteLog.WriteLog(error);
                throw new Exception(error);
            }
            else
            {
               string connStr = config.ConnectionStrings.ConnectionStrings[keyName].ConnectionString;
                if (connStr != null)
                {
                    _WriteLog.WriteLog("Connection String: " + connStr);
                    return connStr;
                }
                else
                {
                    error = "ConnectionString could not be read";
                    _WriteLog.WriteLog(error);
                    throw new Exception(error);
                }
            }
        }

        static void CreateRole(string roleName)
        {
            _WriteLog.WriteLog("Creating Role: " + roleName);
            Role role = _UOW.RoleRepository.FindByName(roleName);
            if (role == null)
            {
                Role newRole = new Role(roleName);
                _UOW.RoleRepository.Add(newRole);
                _WriteLog.WriteLog("Role created.");
            }
            else
            {
                _WriteLog.WriteLog("Role exists.");
            }

        }

        static void CreateUser(string userName, string roleName, string password)
        {
            _WriteLog.WriteLog("Creating User: " + userName + " Role: " + roleName);
            UserStore userStore = new UserStore(_UOW);
            var userManager = new UserManager<IdentityUser, Guid>(userStore);

            var user = userManager.FindByName(userName);
            if (user == null)
            {
                var newUser = new IdentityUser();
                newUser.UserName = userName;
                newUser.SecurityStamp = Guid.NewGuid().ToString();
                newUser.Active = true;
                userManager.CreateAsync(newUser, password);

                newUser = userManager.FindByName(userName);

                //CreateAsync does not set the password:
                String hashedNewPassword = userManager.PasswordHasher.HashPassword(password);
                userStore.SetPasswordHashAsync(newUser, hashedNewPassword);

                userStore.AddToRoleAsync(newUser, roleName);

                _WriteLog.WriteLog("User created. " + roleName + " role assigned" + ", Password set");
            }
            else
            {
                IdentityUser existingUser = userManager.FindAsync(userName, password).Result;
                
                if (existingUser == null)
                {
                    _WriteLog.WriteLog("User exists, password does not match.");
                    String hashedNewPassword = userManager.PasswordHasher.HashPassword(password);
                    userStore.SetPasswordHashAsync(user, hashedNewPassword);
                    _WriteLog.WriteLog("Password reset.");
                }
                else
                {
                    _WriteLog.WriteLog("User exists, password matches, checking for role");
                    //checking for role.
                    if (userStore.IsInRoleAsync(user, roleName).Result == true)
                    {
                        _WriteLog.WriteLog("User with role and password exists");
                    }
                    else
                    {
                        userStore.AddToRoleAsync(user, roleName);
                        _WriteLog.WriteLog("Role updated");
                    }
                }
             }
        }

        //20Apr2017
        static void ModifySiteSource_ChangeLive2DB()
        {
            var sites = _AppAdminService.GetAllSiteSources();
            //var FDAWarningLettersSite = sites.Find(x => x.SiteEnum == SiteEnum.FDAWarningLettersPage);
            //if (FDAWarningLettersSite != null)
            //{
            //    FDAWarningLettersSite.ExtractionMode = "DB";
            //    _AppAdminService.UpdateSiteSource(FDAWarningLettersSite);
            //}
            //else
            //{
            //    throw new Exception("FDAWarningLettersSite not found");
            //}

            //var ClinicalInvestigatorDisqualificationSite = sites.Find(x => x.SiteEnum == SiteEnum.ClinicalInvestigatorDisqualificationPage);
            //if (ClinicalInvestigatorDisqualificationSite != null)
            //{
            //    ClinicalInvestigatorDisqualificationSite.ExtractionMode = "DB";
            //    _AppAdminService.UpdateSiteSource(ClinicalInvestigatorDisqualificationSite);
            //}
            //else
            //{
            //    throw new Exception("ClinicalInvestigatorDisqualificationSite not found");
            //}

            var SAMSite = sites.Find(x => x.SiteEnum == SiteEnum.SystemForAwardManagementPage);
            if(SAMSite != null)
            {
                SAMSite.ExtractionMode = "DB";
                _AppAdminService.UpdateSiteSource(SAMSite);
            }
            else
            {
                throw new Exception("SystemForAwardManagementSite not found");
            }

            //_AppAdminService.UpdateSiteSource()
        }

        static void CreateIndexes()
        {
            Indexes idx = new Indexes();
            var x = idx.CreateIndex();
        }
    }
}