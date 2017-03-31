using DDAS.API.Identity;
using DDAS.Data.Mongo;
using DDAS.Data.Mongo.Maps;
using DDAS.Models.Entities.Domain;
using DDAS.Services.AppAdminService;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DDAS.Setup
{
    class Program
    {
        private static LogText _WriteLog;
        private static UnitOfWork _UOW;
        private static AppAdminService _AppAdminService;

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
                    //CreateFolders(configFile);

                    //Initialize DB for creating Roles and Users:
                    string connString = GetWebConfigConnectionString(configFile, "DefaultConnection");
                      MongoMaps.Initialize();
                     _UOW = new UnitOfWork(connString);
                    CreateRoles();
                    CreateUsers();

                    _AppAdminService = new AppAdminService(_UOW);
                    //SitesToSearch Sites = new SitesToSearch();
                    //_AppAdminService.AddSitesInDbCollection(Sites);

                    //Executed on 31-3-2017
                    //ModifySiteSource_ChangeLive2DB();
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
            string DownloadFolder = GetWebConfigAppSetting(configFile, "AppDataDownloadFolder");
            CreateFolder(appRootFolder + @"\" + DownloadFolder);

            _WriteLog.WriteLog("Reading Web.config for:", "UploadsFolder");
            string UploadFolder = GetWebConfigAppSetting(configFile, "UploadsFolder");
            CreateFolder(appRootFolder + @"\" + UploadFolder);

            _WriteLog.WriteLog("Reading Web.config for:", "ExcelTemplateFolder");
            string ExcelTemplateFolder = GetWebConfigAppSetting(configFile, "ExcelTemplateFolder");
            CreateFolder(appRootFolder + @"\" + UploadFolder);
            //Copy Excel Template from ...

            _WriteLog.WriteLog("Reading Web.config for:", "WordTemplateFolder");
            string WordTemplateFolder = GetWebConfigAppSetting(configFile, "WordTemplateFolder");
            CreateFolder(appRootFolder + @"\" + UploadFolder);
            //Copy Word Template from ...

            _WriteLog.WriteLog("Reading Web.config for:", "ComplianceFormFolder");
            string ComplianceFormFolder = GetWebConfigAppSetting(configFile, "ComplianceFormFolder");
            CreateFolder(appRootFolder + @"\" + ComplianceFormFolder);
        }

        static void CreateRoles()
        {
            CreateRole("admin");
            CreateRole("user");
            CreateRole("app-admin");
        }

        static void CreateUsers()
        {
            CreateUser("clarityadmin", "app-admin", "Clarity@148");

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
            System.Configuration.Configuration config =
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
                System.Configuration.KeyValueConfigurationElement settings = config.AppSettings.Settings[keyName];
                if (settings != null)
                {
                    _WriteLog.WriteLog("Key : " + keyName + ", Value: " + settings.Value);
                    return settings.Value;
                }
                else
                {
                    error = "Key : " + keyName + ", Value: " + settings.Value + " could not be read";
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
            System.Configuration.Configuration config =
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

        //31Mar2017
        static void ModifySiteSource_ChangeLive2DB()
        {
            var sites = _AppAdminService.GetAllSiteSources();
            var FDAWarningLettersSite = sites.Find(x => x.SiteEnum == Models.Enums.SiteEnum.FDAWarningLettersPage);
            if (FDAWarningLettersSite != null)
            {
                FDAWarningLettersSite.ExtractionMode = "DB";
                _AppAdminService.UpdateSiteSource(FDAWarningLettersSite);
            }
            else
            {
                throw new Exception("FDAWarningLettersSite not found");
            }

            var ClinicalInvestigatorDisqualificationSite = sites.Find(x => x.SiteEnum == Models.Enums.SiteEnum.ClinicalInvestigatorDisqualificationPage);
            if (ClinicalInvestigatorDisqualificationSite != null)
            {
                ClinicalInvestigatorDisqualificationSite.ExtractionMode = "DB";
                _AppAdminService.UpdateSiteSource(ClinicalInvestigatorDisqualificationSite);
            }
            else
            {
                throw new Exception("ClinicalInvestigatorDisqualificationSite not found");
            }
            //_AppAdminService.UpdateSiteSource()
        }
    }
}