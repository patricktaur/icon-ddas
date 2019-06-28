using DDAS.API.Identity;
using DDAS.Data.Mongo;
using DDAS.Data.Mongo.Indexes;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Entities.Domain.SiteData;
using DDAS.Models.Enums;
using DDAS.Models.Interfaces;
using DDAS.Services.AppAdminService;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Utilities;
using MongoDB.Driver;
using System.Threading.Tasks;
using DDAS.Data.Mongo.Maps;
using System.Diagnostics;

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
                _WriteLog = new LogText(exePath + @"\archive.log", true);
                _WriteLog.LogStart();
                _WriteLog.WriteLog("DDAS Setup Utility");
                _WriteLog.WriteLog("========================================================================");
                _WriteLog.WriteLog(DateTime.Now.ToString(), "Start Archive at " + exePath);
                string configFile = ConfigurationManager.AppSettings["APIWebConfigFile"];
                if (configFile != null)
                {
                    //CreateFolders(configFile);
                    Console.WriteLine("config file: " + configFile);
                    //Initialize DB for creating Roles and Users:
                    string connString = GetWebConfigConnectionString(configFile, "DefaultConnection");
                    string DBName = GetWebConfigAppSetting(configFile, "DBName");

                    MongoMaps.Initialize();
                    _UOW = new UnitOfWork(connString, DBName);

                    _WriteLog.WriteLog(DateTime.Now.ToString(), "Initialized");
                    //CreateRoles();
                    //CreateUsers();

                    _config = new Config(); //empty values for config prop.. ?
                    _AppAdminService = new AppAdminService(_UOW, _config);

                    //Executed on FindMeServerOn 1April2017.
                    //var sitesInDB = _UOW.SiteSourceRepository.GetAll();

                    //Executed on DDASTEST server on 18Sept2017
                    //SAM site ExtractionMode was not updated to 'DB' - changed
                    //_WriteLog.WriteLog("Removing sites..");
                    //_UOW.SiteSourceRepository.DropAll(
                    //    _UOW.SiteSourceRepository.GetAll().FirstOrDefault());

                    //_WriteLog.WriteLog("Removing Default sites..");
                    //_UOW.DefaultSiteRepository.DropAll(
                    //    _UOW.DefaultSiteRepository.GetAll().FirstOrDefault());

                    //For execution on DDAS Prod server on ..
                    //Error in application: User deleted a record in SiteSourceRepository 
                    //which was referenced in CountryRepository
                    //Clear the orphaned record in CountryRepository



                    //_WriteLog.WriteLog("site Sources InDB:", sitesInDB.Count.ToString());
                    //if (sitesInDB.Count > 0)
                    //{
                    //    _WriteLog.WriteLog("Sites already exist. Not added");
                    //}
                    //else
                    //{
                    //    _WriteLog.WriteLog("Adding Sites");
                    //    SitesToSearch Sites = new SitesToSearch();
                    //    _AppAdminService.AddSitesInDbCollection(Sites);
                    //}

                    //Executed on 20-4-2017
                    //ModifySiteSource_ChangeLive2DB();

                    //Executred on Prod Server on 
                    //DeleteOrphanedRecordsInDefaultSiteRepository();
                    //DeleteOrphanedRecordsInCountryRepository();
                    //DeleteOrphanedRecordsInSponsorProtocolRepository();
                    //CreateIndexes(DBName, connString);
                    //_WriteLog.WriteLog(DateTime.Now.ToString(), "Start of GetCollectionCount");
                    //GetCollectionCount();
                    //_WriteLog.WriteLog(DateTime.Now.ToString(), "End of GetCollectionCount");

                    //var Doc = _UOW.FDADebarPageRepository.GetLatestDocument();

                    //if (canArchiveData())
                    //{
                    //ArchiveSiteData();
                    //_WriteLog.WriteLog(DateTime.Now.ToString(), "Delete Documents Begins====");
                    //_WriteLog.WriteLog("Deleting documents till date: " + new DateTime(2018, 11, 1).Date);
                    //deleteDocumentsByDate(new DateTime(2018, 11, 1));
                    //_WriteLog.WriteLog(DateTime.Now.ToString(), "Delete Documents Ends====");
                    //}
                    //else
                    //{
                    //    _WriteLog.WriteLog("Could not archive data");
                    //}

                    TestCache(_UOW);

                    string firstArg = "";
                    if (args.Length != 0)
                    {
                        firstArg = args[0];
                    }
                    switch (firstArg)
                    {
                        case "cleardb":
                            break;
                        case "complianceFormIndex":
                            ListComplianceFormIndexes(DBName, connString);
                            break;
                        case "indexing":
                            CreateIndexes(DBName, connString);
                            ListComplianceFormIndexes(DBName, connString);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    _WriteLog.WriteLog("Entry in AppSettings : APIWebConfigFile" + " not found");
                }
             }
            catch (Exception ex)
            {
                _WriteLog.WriteLog("Error: ", ex.Message + " " + ex.InnerException);
            }
            finally
            {
                _WriteLog.WriteLog(DateTime.Now.ToString(), "End archive");
                _WriteLog.LogEnd();
                _WriteLog.Dispose();
                Console.ReadKey();
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
        static  void ModifySiteSource_ChangeLive2DB()
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

        static  void CreateIndexes(string DBName, string connectionString)
        {
            _WriteLog.WriteLog(string.Format("Connecting to {0}, connection string: {1}", DBName, connectionString));
            var mongo = new MongoClient(connectionString);

            var db = mongo.GetDatabase(DBName);
            _WriteLog.WriteLog("Connected");
            var collection = db.GetCollection<ComplianceForm>("ComplianceForm");
            _WriteLog.WriteLog("Start creating SearchStartedOn key");

            //await collection.Indexes.CreateOneAsync(Builders<ComplianceForm>.IndexKeys.Ascending(_ => _.SearchStartedOn));
            collection.Indexes.CreateOne(Builders<ComplianceForm>.IndexKeys.Ascending(_ => _.SearchStartedOn));
            _WriteLog.WriteLog("Start creating AssignedTo key");
            //await collection.Indexes.CreateOneAsync(Builders<ComplianceForm>.IndexKeys.Ascending(_ => _.AssignedTo));
            collection.Indexes.CreateOne(Builders<ComplianceForm>.IndexKeys.Ascending(_ => _.AssignedTo));

            collection.Indexes.CreateOne(Builders<ComplianceForm>.IndexKeys.Ascending("InvestigatorDetails.Name"));
            collection.Indexes.CreateOne(Builders<ComplianceForm>.IndexKeys.Ascending("InvestigatorDetails.ReviewCompletedSiteCount"));

            collection.Indexes.CreateOne(Builders<ComplianceForm>.IndexKeys.Ascending("InvestigatorDetails.FirstName, InvestigatorDetails.MiddleName, InvestigatorDetails.LastName"));
           
            collection.Indexes.CreateOne(Builders<ComplianceForm>.IndexKeys.Ascending("Reviews.Status"));

            collection.Indexes.CreateOne(Builders<ComplianceForm>.IndexKeys.Ascending(_ => _.InputSource));

            _WriteLog.WriteLog("Key Creation completed");
            //var indexColl = collection.Indexes.
        }

        static void ListComplianceFormIndexes(
            string DBName, string connectionString)
        {
            _WriteLog.WriteLog(string.Format("Connecting to {0}, connection string: {1}", DBName, connectionString));
            var mongo = new MongoClient(connectionString);
            var db = mongo.GetDatabase(DBName);
            _WriteLog.WriteLog("Connected");

            var collection = db.GetCollection<ComplianceForm>("ComplianceForm");
            var Indexes = collection.Indexes.List();
            _WriteLog.WriteLog("Listing Indexes for ComplianceForm collection");

            while (Indexes.MoveNext())
            {
                var CurrentIndex = Indexes.Current;
                foreach (var Document in CurrentIndex)
                {
                    var DocNames = Document.Names;
                    foreach (string Name in DocNames)
                    {
                        var Value = Document.GetValue(Name);
                        _WriteLog.WriteLog(string.Concat(Name, ": ", Value));
                    }
                }
            }
        }

        static void DeleteWSDDASLogRecords()
        {
            Console.WriteLine("Deleting WSDDAS Log");
            _UOW.LogWSDDASRepository.DropAll(new Models.Entities.LogWSDDAS());
            Console.WriteLine("Deleted WSDDAS Log");
        }

        static void DeleteOrphanedRecordsInDefaultSiteRepository()
        {
            _WriteLog.WriteLog("Deleting orphaned records from Default Site Repository", "Start");
            var DefaultSites = _UOW.DefaultSiteRepository.GetAll()
                .ToList();
            foreach (DefaultSite defaultSite in DefaultSites)
            {
                try
                {
                    var site = _UOW.SiteSourceRepository.FindById(defaultSite.SiteId);
                    if (site == null)
                    {
                        _WriteLog.WriteLog("Record not found in Site Source", defaultSite.Name);
                        _WriteLog.WriteLog("Deleting from Default Site Repository", "");
                        _UOW.DefaultSiteRepository.RemoveById(defaultSite.RecId);
                        _WriteLog.WriteLog("Deleted", "");
                    }

                }
                catch (Exception ex)
                {
                    _WriteLog.WriteLog("Error", ex.Message);
                }

            }
        }

        static void DeleteOrphanedRecordsInCountryRepository()
        {
            _WriteLog.WriteLog("Deleting orphaned records from Country Repository", "Start");
            var Countries = _UOW.CountryRepository.GetAll()
                .OrderBy(x => x.CountryName)
                .ToList();
            foreach (Country country in Countries)
            {
                try
                {
                    var site = _UOW.SiteSourceRepository.FindById(country.SiteId);
                    if (site == null)
                    {
                        _WriteLog.WriteLog("Record not found in Site Source", country.CountryName);
                        _WriteLog.WriteLog("Deleting from Country Repository", "");
                        _UOW.CountryRepository.RemoveById(country.RecId);
                        _WriteLog.WriteLog("Deleted", "");

                    }

                }
                catch (Exception ex)
                {
                    _WriteLog.WriteLog("Error", ex.Message);
                }

            }
        }

        static void DeleteOrphanedRecordsInSponsorProtocolRepository()
        {
            _WriteLog.WriteLog("Deleting orphaned records from Sponsor Protocol Repository", "Start");
            var SponsorProtocols = _UOW.SponsorProtocolRepository.GetAll()
                .ToList();
            foreach (SponsorProtocol sponsorProtocol in SponsorProtocols)
            {
                try
                {
                    var site = _UOW.SiteSourceRepository.FindById(sponsorProtocol.SiteId);
                    if (site == null)
                    {
                        _WriteLog.WriteLog("Record not found in Site Source", sponsorProtocol.Name);
                        _WriteLog.WriteLog("Deleting from Sponsor Protocol Repository", "");
                        _UOW.SponsorProtocolRepository.RemoveById(sponsorProtocol.RecId);
                        _WriteLog.WriteLog("Deleted", "");
                    }

                }
                catch (Exception ex)
                {
                    _WriteLog.WriteLog("Error", ex.Message);
                }

            }
        }

        static void ClearDB()
        {

        }

        static void GetCollectionCount()
        {
            _WriteLog.WriteLog("Compliance Forms till date: " + _UOW.ComplianceFormRepository.GetAll().Count());//42961
            _WriteLog.WriteLog("Total records per site source:");
            _WriteLog.WriteLog("FDADebarPage " + _UOW.FDADebarPageRepository.GetAll().Count());//512
            _WriteLog.WriteLog("FDADebarPage " + _UOW.FDADebarPageRepository.GetAll().Count());//512
            _WriteLog.WriteLog("ClinicalInvestigatorInspectionList " + _UOW.ClinicalInvestigatorInspectionListRepository.GetAll().Count());//543
            _WriteLog.WriteLog("FDAWarningLetters " + _UOW.FDAWarningLettersRepository.GetAll().Count());//312
            _WriteLog.WriteLog("ERRProposalToDebar " + _UOW.ERRProposalToDebarRepository.GetAll().Count());//500
            _WriteLog.WriteLog("AdequateAssuranceList {0}" + _UOW.AdequateAssuranceListRepository.GetAll().Count());//52
            _WriteLog.WriteLog("ClinicalInvestigatorDisqualification " + _UOW.ClinicalInvestigatorDisqualificationRepository.GetAll().Count());//541
            _WriteLog.WriteLog("PHSAdministrativeActionListing " + _UOW.PHSAdministrativeActionListingRepository.GetAll().Count());//564
            _WriteLog.WriteLog("CBERClinicalInvestigator " + _UOW.CBERClinicalInvestigatorRepository.GetAll().Count());//506
            _WriteLog.WriteLog("ExclusionDatabase " + _UOW.ExclusionDatabaseRepository.GetAll().Count());//68886
            _WriteLog.WriteLog("CorporateIntegrityAgreement " + _UOW.CorporateIntegrityAgreementRepository.GetAll().Count());//513
            _WriteLog.WriteLog("SystemForAwardManagement " + _UOW.SystemForAwardManagementRepository.GetAll().Count());//260
            _WriteLog.WriteLog("SpeciallyDesignatedNationals " + _UOW.SpeciallyDesignatedNationalsRepository.GetAll().Count());//555
        }

        #region canArchiveData

        static bool canArchiveData()
        {
            var archiveComplianceForm = false;
            var archiveFDADebar = false;
            var archiveCIIL = false;
            var archiveFDAWarning = false;
            var archiveErrProposalToDebar = false;
            var archiveAdequateAssuranceList = false;
            var archiveClinicalInvestigatorDisqualification = false;
            var archivePHSAdministrativeAction = false;
            var archiveCBERClinicalInvestigator = false;
            var archiveExclusionDatabase = false;
            var archiveCorporateIntegrityAgreementList = false;
            var archiveSAM = false;
            var archiveSDN = false;

            if (canArchiveComplianceForm("ArchiveComplianceForm"))
            {
                archiveComplianceForm = true;
            }

            if (canArchiveFDADebarSiteData("ArchiveFDADebarSiteData"))
            {
                archiveFDADebar = true;
            }

            if (canArchiveCIILSiteData("ArchiveClinicalInvestigatorInspectionListSiteData"))
            {
                archiveCIIL = true;
            }

            if (canArchiveFDAWarningLettersSiteData("ArchiveFDAWarningLettersSiteData"))
            {
                archiveFDAWarning = true;
            }

            if (canArchiveERRProposalToDebarSiteData("ArchiveERRProposalToDebarPageSiteData"))
            {
                archiveErrProposalToDebar = true;
            }

            if (canArchiveAdequateAssuranceListSiteData("ArchiveAdequateAssuranceListSiteData"))
            {
                archiveAdequateAssuranceList = true;
            }

            if (canArchiveClinicalInvestigatorDisqualificationSiteData("ArchiveClinicalInvestigatorDisqualificationSiteData"))
            {
                archiveClinicalInvestigatorDisqualification = true;
            }

            if (canArchivePHSAdministrativeActionListingSiteData("ArchivePHSAdministrativeActionListingSiteData"))
            {
                archivePHSAdministrativeAction = true;
            }

            if (canArchiveCBERClinicalInvestigatorInspectionSiteData("ArchiveCBERClinicalInvestigatorInspectionSiteData"))
            {
                archiveCBERClinicalInvestigator = true;
            }

            if (canArchiveExclusionDatabaseSearchPageSiteData("ArchiveExclusionDatabaseSearchPageSiteData"))
            {
                archiveExclusionDatabase = true;
            }

            if (canArchiveCorporateIntegrityAgreementListSiteData("ArchiveCorporateIntegrityAgreementListSiteData"))
            {
                archiveCorporateIntegrityAgreementList = true;
            }

            if (canArchiveSystemForAwardManagementPageSiteData("ArchiveSystemForAwardManagementPageSiteData"))
            {
                archiveSAM = true;
            }

            if (canArchiveSpeciallyDesignatedNationalsListSiteData("ArchiveSpeciallyDesignatedNationalsListSiteData"))
            {
                archiveSDN = true;
            }

            if(archiveComplianceForm &&
                archiveFDADebar &&
                archiveCIIL &&
                archiveFDAWarning &&
                archiveErrProposalToDebar &&
                archiveAdequateAssuranceList &&
                archiveClinicalInvestigatorDisqualification &&
                archivePHSAdministrativeAction &&
                archiveCBERClinicalInvestigator &&
                archiveExclusionDatabase &&
                archiveCorporateIntegrityAgreementList &&
                archiveSAM &&
                archiveSDN)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static bool canArchiveComplianceForm(string moveToCollection)
        {
            if (_UOW.ComplianceFormRepository.CollectionExists(moveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + moveToCollection + " already exists");
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool canArchiveFDADebarSiteData(string moveToCollection)
        {
            if (_UOW.FDADebarPageRepository.CollectionExists(moveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + moveToCollection + " already exists");
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool canArchiveCIILSiteData(string moveToCollection)
        {
            if (_UOW.ClinicalInvestigatorInspectionListRepository.CollectionExists(moveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + moveToCollection + " already exists");
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool canArchiveFDAWarningLettersSiteData(string moveToCollection)
        {
            if (_UOW.FDAWarningLettersRepository.CollectionExists(moveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + moveToCollection + " already exists");
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool canArchiveERRProposalToDebarSiteData(string moveToCollection)
        {
            if (_UOW.ERRProposalToDebarRepository.CollectionExists(moveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + moveToCollection + " already exists");
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool canArchiveAdequateAssuranceListSiteData(string moveToCollection)
        {
            if (_UOW.AdequateAssuranceListRepository.CollectionExists(moveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + moveToCollection + " already exists");
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool canArchiveClinicalInvestigatorDisqualificationSiteData(string moveToCollection)
        {
            if (_UOW.ClinicalInvestigatorDisqualificationRepository.CollectionExists(moveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + moveToCollection + " already exists");
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool canArchivePHSAdministrativeActionListingSiteData(string moveToCollection)
        {
            if (_UOW.PHSAdministrativeActionListingRepository.CollectionExists(moveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + moveToCollection + " already exists");
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool canArchiveCBERClinicalInvestigatorInspectionSiteData(string moveToCollection)
        {
            if (_UOW.CBERClinicalInvestigatorRepository.CollectionExists(moveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + moveToCollection + " already exists");
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool canArchiveExclusionDatabaseSearchPageSiteData(string moveToCollection)
        {
            if (_UOW.ExclusionDatabaseSearchRepository.CollectionExists(moveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + moveToCollection + " already exists");
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool canArchiveCorporateIntegrityAgreementListSiteData(string moveToCollection)
        {
            if (_UOW.CorporateIntegrityAgreementRepository.CollectionExists(moveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + moveToCollection + " already exists");
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool canArchiveSystemForAwardManagementPageSiteData(string moveToCollection)
        {
            if (_UOW.SystemForAwardManagementRepository.CollectionExists(moveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + moveToCollection + " already exists");
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool canArchiveSpeciallyDesignatedNationalsListSiteData(string moveToCollection)
        {
            if (_UOW.SpeciallyDesignatedNationalsRepository.CollectionExists(moveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + moveToCollection + " already exists");
                return false;
            }
            else
            {
                return true;
            }
        }

        #endregion

        #region MoveCollections

        static void ArchiveSiteData()
        {
            //_WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving ComplianceForm Collection as ArchiveComplianceForm");
            //MoveComplianceForms("ArchiveComplianceForm");
            //_WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving Completed");

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving FDADebarSiteData Collection as ArchiveFDADebarSiteData");
            MoveFDADebarSiteData("ArchiveFDADebarSiteData");
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving Completed");

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "Archiving ClinicalInvestigatorInspectionListSiteData Collection as ClinicalInvestigatorInspectionListSiteData");
            MoveCIILSiteData("ArchiveClinicalInvestigatorInspectionListSiteData");
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving Completed");

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving FDAWarningLettersSiteData Collection as FDAWarningLettersSiteData");
            MoveFDAWarningLettersSiteData("ArchiveFDAWarningLettersSiteData");
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving Completed");

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving ERRProposalToDebarPageSiteData Collection as ERRProposalToDebarPageSiteData");
            MoveERRProposalToDebarSiteData("ArchiveERRProposalToDebarPageSiteData");
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving Completed");

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving AdequateAssuranceListSiteData Collection as AdequateAssuranceListSiteData");
            MoveAdequateAssuranceListSiteData("ArchiveAdequateAssuranceListSiteData");
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving Completed");

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "Archiving ClinicalInvestigatorDisqualificationSiteData Collection as ClinicalInvestigatorDisqualificationSiteData");
            MoveClinicalInvestigatorDisqualificationSiteData("ArchiveClinicalInvestigatorDisqualificationSiteData");
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving Completed");

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "Archiving PHSAdministrativeActionListingSiteData Collection as PHSAdministrativeActionListingSiteData");
            MovePHSAdministrativeActionListingSiteData("ArchivePHSAdministrativeActionListingSiteData");
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving Completed");

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving CBERClinicalInvestigatorInspectionSiteData Collection");
            MoveCBERClinicalInvestigatorInspectionSiteData("ArchiveCBERClinicalInvestigatorInspectionSiteData");
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving Completed");

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "Archiving ExclusionDatabaseSearchPageSiteData Collection as ArchiveExclusionDatabaseSearchPageSiteData");
            MoveExclusionDatabaseSearchPageSiteData("ArchiveExclusionDatabaseSearchPageSiteData");
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving Completed");

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "Archiving CorporateIntegrityAgreementListSiteData Collection as ArchiveCorporateIntegrityAgreementListSiteData");
            MoveCorporateIntegrityAgreementListSiteData("ArchiveCorporateIntegrityAgreementListSiteData");
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving Completed");

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "Archiving SystemForAwardManagementPageSiteData Collection as ArchiveSystemForAwardManagementPageSiteData");
            MoveSystemForAwardManagementPageSiteData("ArchiveSystemForAwardManagementPageSiteData");
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving Completed");

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "Archiving SpeciallyDesignatedNationalsListSiteData Collection as ArchiveSpeciallyDesignatedNationalsListSiteData");
            MoveSpeciallyDesignatedNationalsListSiteData("ArchiveSpeciallyDesignatedNationalsListSiteData");
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Archiving Completed");
        }

        static void MoveComplianceForms(string MoveToCollection)
        {
            if (_UOW.ComplianceFormRepository.CollectionExists(MoveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + MoveToCollection + " already exists. Moving will overwrite existing collection. Data not moved");
            }
            else
            {
                _WriteLog.WriteLog("Total Records to archive: " + _UOW.ComplianceFormRepository.GetAll().Count);
                _UOW.ComplianceFormRepository.MoveCollection(new ComplianceForm(), MoveToCollection);
            }
        }

        static void MoveFDADebarSiteData(string MoveToCollection)
        {
            if (_UOW.FDADebarPageRepository.CollectionExists(MoveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + MoveToCollection + " already exists. Moving will overwrite existing collection. Data not moved");
            }
            else
            {
                _WriteLog.WriteLog("Total Records to archive: " + _UOW.FDADebarPageRepository.GetAll().Count);
                _UOW.FDADebarPageRepository.MoveCollection(new FDADebarPageSiteData(), MoveToCollection);
            }
        }

        static void MoveCIILSiteData(string MoveToCollection)
        {
            if (_UOW.FDADebarPageRepository.CollectionExists(MoveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + MoveToCollection + " already exists. Moving will overwrite existing collection. Data not moved");
            }
            else
            {
                _WriteLog.WriteLog("Total Records to archive: " + _UOW.ClinicalInvestigatorInspectionListRepository.GetAll().Count);
                _UOW.ClinicalInvestigatorInspectionListRepository.MoveCollection(new ClinicalInvestigatorInspectionSiteData(), MoveToCollection);
            }
        }

        static void MoveFDAWarningLettersSiteData(string MoveToCollection)
        {
            if (_UOW.FDADebarPageRepository.CollectionExists(MoveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + MoveToCollection + " already exists. Moving will overwrite existing collection. Data not moved");
            }
            else
            {
                _WriteLog.WriteLog("Total Records to archive: " + _UOW.FDAWarningLettersRepository.GetAll().Count);
                _UOW.FDAWarningLettersRepository.MoveCollection(new FDAWarningLettersSiteData(), MoveToCollection);
            }
        }

        static void MoveERRProposalToDebarSiteData(string MoveToCollection)
        {
            if (_UOW.FDADebarPageRepository.CollectionExists(MoveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + MoveToCollection + " already exists. Moving will overwrite existing collection. Data not moved");
            }
            else
            {
                _WriteLog.WriteLog("Total Records to archive: " + _UOW.ERRProposalToDebarRepository.GetAll().Count);
                _UOW.ERRProposalToDebarRepository.MoveCollection(new ERRProposalToDebarPageSiteData(), MoveToCollection);
            }
        }

        static void MoveAdequateAssuranceListSiteData(string MoveToCollection)
        {
            if (_UOW.FDADebarPageRepository.CollectionExists(MoveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + MoveToCollection + " already exists. Moving will overwrite existing collection. Data not moved");
            }
            else
            {
                _WriteLog.WriteLog("Total Records to archive: " + _UOW.AdequateAssuranceListRepository.GetAll().Count);
                _UOW.AdequateAssuranceListRepository.MoveCollection(new AdequateAssuranceListSiteData(), MoveToCollection);
            }
        }

        static void MoveClinicalInvestigatorDisqualificationSiteData(string MoveToCollection)
        {
            if (_UOW.FDADebarPageRepository.CollectionExists(MoveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + MoveToCollection + " already exists. Moving will overwrite existing collection. Data not moved");
            }
            else
            {
                _WriteLog.WriteLog("Total Records to archive: " + _UOW.ClinicalInvestigatorDisqualificationRepository.GetAll().Count);
                _UOW.ClinicalInvestigatorDisqualificationRepository.MoveCollection(new ClinicalInvestigatorDisqualificationSiteData(), MoveToCollection);
            }
        }

        static void MovePHSAdministrativeActionListingSiteData(string MoveToCollection)
        {
            if (_UOW.FDADebarPageRepository.CollectionExists(MoveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + MoveToCollection + " already exists. Moving will overwrite existing collection. Data not moved");
            }
            else
            {
                _WriteLog.WriteLog("Total Records to archive: " + _UOW.PHSAdministrativeActionListingRepository.GetAll().Count);
                _UOW.PHSAdministrativeActionListingRepository.MoveCollection(new PHSAdministrativeActionListingSiteData(), MoveToCollection);
            }
        }

        static void MoveCBERClinicalInvestigatorInspectionSiteData(string MoveToCollection)
        {
            if (_UOW.FDADebarPageRepository.CollectionExists(MoveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + MoveToCollection + " already exists. Moving will overwrite existing collection. Data not moved");
            }
            else
            {
                _WriteLog.WriteLog("Total Records to archive: " + _UOW.CBERClinicalInvestigatorRepository.GetAll().Count);
                _UOW.CBERClinicalInvestigatorRepository.MoveCollection(new CBERClinicalInvestigatorInspectionSiteData(), MoveToCollection);
            }
        }

        static void MoveExclusionDatabaseSearchPageSiteData(string MoveToCollection)
        {
            if (_UOW.FDADebarPageRepository.CollectionExists(MoveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + MoveToCollection + " already exists. Moving will overwrite existing collection. Data not moved");
            }
            else
            {
                _WriteLog.WriteLog("Total Records to archive: " + _UOW.ExclusionDatabaseSearchRepository.GetAll().Count);
                _UOW.ExclusionDatabaseSearchRepository.MoveCollection(new ExclusionDatabaseSearchPageSiteData(), MoveToCollection);
            }
        }

        static void MoveCorporateIntegrityAgreementListSiteData(string MoveToCollection)
        {
            if (_UOW.FDADebarPageRepository.CollectionExists(MoveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + MoveToCollection + " already exists. Moving will overwrite existing collection. Data not moved");
            }
            else
            {
                _WriteLog.WriteLog("Total Records to archive: " + _UOW.CorporateIntegrityAgreementRepository.GetAll().Count);
                _UOW.CorporateIntegrityAgreementRepository.MoveCollection(new CorporateIntegrityAgreementListSiteData(), MoveToCollection);
            }
        }

        static void MoveSystemForAwardManagementPageSiteData(string MoveToCollection)
        {
            if (_UOW.FDADebarPageRepository.CollectionExists(MoveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + MoveToCollection + " already exists. Moving will overwrite existing collection. Data not moved");
            }
            else
            {
                _WriteLog.WriteLog("Total Records to archive: " + _UOW.SystemForAwardManagementRepository.GetAll().Count);
                _UOW.SystemForAwardManagementRepository.MoveCollection(new SystemForAwardManagementPageSiteData(), MoveToCollection);
            }
        }

        static void MoveSpeciallyDesignatedNationalsListSiteData(string MoveToCollection)
        {
            if (_UOW.FDADebarPageRepository.CollectionExists(MoveToCollection))
            {
                _WriteLog.WriteLog("Collection: " + MoveToCollection + " already exists. Moving will overwrite existing collection. Data not moved");
            }
            else
            {
                _WriteLog.WriteLog("Total Records to archive: " + _UOW.SpeciallyDesignatedNationalsRepository.GetAll().Count);
                _UOW.SpeciallyDesignatedNationalsRepository.MoveCollection(new SpeciallyDesignatedNationalsListSiteData(), MoveToCollection);
            }
        }

        #endregion

        #region DeleteDocumentsByDate

        static void deleteDocumentsByDate(DateTime deleteDocumentsTillThisDate)
        {
            //_WriteLog.WriteLog(DateTime.Now.ToString(), "deleting ComplianceForm documents...");
            //deleteComplianceForms(deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "deleting FDADebarSiteData documents...");
            DeleteFDADebarSiteData(deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "deleting CIILSiteData documents...");
            deleteCIILSiteData(deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "deleting FDAWarningLetterSiteData documents...");
            deleteFDAWarningLetterSiteData(deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "deleting ERRProposalToDebarSiteData documents...");
            deleteERRProposalToDebarSiteData(deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "deleting AdequateAssuranceListSiteData documents...");
            deleteAdequateAssuranceListSiteData(deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "deleting ClinicalInvestigatorDisqualificationSiteData documents...");
            deleteClinicalInvestigatorDisqualificationSiteData(deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "deleting PHSAdministrativeActionListingSiteData documents...");
            deletePHSAdministrativeActionListingSiteData(deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "deleting CBERClinicalInvestigatorInspectionSiteData documents...");
            deleteCBERClinicalInvestigatorInspectionSiteData(deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "deleting ExclusionDatabaseSearchPageSiteData documents...");
            deleteExclusionDatabaseSearchPageSiteData(deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "deleting CorporateIntegrityAgreementListSiteData documents...");
            deleteCorporateIntegrityAgreementListSiteData(deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "deleting SystemForAwardManagementPageSiteData documents...");
            deleteSystemForAwardManagementPageSiteData(deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "deleting SpeciallyDesignatedNationalsListSiteData documents...");
            deleteSpeciallyDesignatedNationalsListSiteData(deleteDocumentsTillThisDate);
        }

        static void deleteComplianceForms(DateTime deleteDocumentsTillThisDate)
        {
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Before Delete, ComplianceFormRepository Count: " + _UOW.ComplianceFormRepository.GetAll().Count);
            var Documents = _UOW.ComplianceFormRepository.GetRecordsByDate(
                new ComplianceForm(),
                deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Documents to delete: " + Documents.Count);

            foreach (var document in Documents)
            {
                _UOW.ComplianceFormRepository.RemoveById(document.RecId);
            }

            _WriteLog.WriteLog(DateTime.Now.ToString(), "After Delete, ComplianceFormRepository Count: " + _UOW.ComplianceFormRepository.GetAll().Count);
        }

        static void DeleteFDADebarSiteData(DateTime deleteDocumentsTillThisDate)
        {
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Before Delete, FDADebarPageRepository Count: " + _UOW.FDADebarPageRepository.GetAll().Count);
            var Documents = _UOW.FDADebarPageRepository.GetRecordsByDate(
                new FDADebarPageSiteData(),
                deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Documents to delete: " + Documents.Count);

            foreach (var document in Documents)
            {
                _UOW.FDADebarPageRepository.RemoveById(document.RecId);
            }

            _WriteLog.WriteLog(DateTime.Now.ToString(), "After Delete, FDADebarPageRepository Count: " + _UOW.FDADebarPageRepository.GetAll().Count);
        }

        static void deleteCIILSiteData(DateTime deleteDocumentsTillThisDate)
        {
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Before Delete, ClinicalInvestigatorInspectionListRepository Count: " + 
                _UOW.ClinicalInvestigatorInspectionListRepository.GetAll().Count);

            var Documents = _UOW.ClinicalInvestigatorInspectionListRepository.GetRecordsByDate(
                new ClinicalInvestigatorInspectionSiteData(),
                deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Documents to delete: " + Documents.Count);

            foreach (var document in Documents)
            {
                _UOW.ClinicalInvestigatorInspectionListRepository.RemoveById(document.RecId);
            }

            _WriteLog.WriteLog(DateTime.Now.ToString(), 
                "After Delete, ClinicalInvestigatorInspectionListRepository Count: " + _UOW.ClinicalInvestigatorInspectionListRepository.GetAll().Count);
        }

        static void deleteFDAWarningLetterSiteData(DateTime deleteDocumentsTillThisDate)
        {
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Before Delete, FDAWarningLettersRepository Count: " +
                _UOW.FDAWarningLettersRepository.GetAll().Count);

            var Documents = _UOW.FDAWarningLettersRepository.GetRecordsByDate(
                new FDAWarningLettersSiteData(),
                deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Documents to delete: " + Documents.Count);

            foreach (var document in Documents)
            {
                _UOW.FDAWarningLettersRepository.RemoveById(document.RecId);
            }

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "After Delete, FDAWarningLettersRepository Count: " + _UOW.FDAWarningLettersRepository.GetAll().Count);
        }

        static void deleteERRProposalToDebarSiteData(DateTime deleteDocumentsTillThisDate)
        {
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Before Delete, ERRProposalToDebarRepository Count: " +
                _UOW.ERRProposalToDebarRepository.GetAll().Count);

            var Documents = _UOW.ERRProposalToDebarRepository.GetRecordsByDate(
                new ERRProposalToDebarPageSiteData(),
                deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Documents to delete: " + Documents.Count);

            foreach (var document in Documents)
            {
                _UOW.ERRProposalToDebarRepository.RemoveById(document.RecId);
            }

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "After Delete, ERRProposalToDebarRepository Count: " + _UOW.ERRProposalToDebarRepository.GetAll().Count);
        }

        static void deleteAdequateAssuranceListSiteData(DateTime deleteDocumentsTillThisDate)
        {
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Before Delete, AdequateAssuranceListRepository Count: " +
                _UOW.AdequateAssuranceListRepository.GetAll().Count);

            var Documents = _UOW.AdequateAssuranceListRepository.GetRecordsByDate(
                new AdequateAssuranceListSiteData(),
                deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Documents to delete: " + Documents.Count);

            foreach (var document in Documents)
            {
                _UOW.AdequateAssuranceListRepository.RemoveById(document.RecId);
            }

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "After Delete, AdequateAssuranceListRepository Count: " + _UOW.AdequateAssuranceListRepository.GetAll().Count);
        }

        static void deleteClinicalInvestigatorDisqualificationSiteData(DateTime deleteDocumentsTillThisDate)
        {
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Before Delete, ClinicalInvestigatorDisqualificationRepository Count: " +
                _UOW.ClinicalInvestigatorDisqualificationRepository.GetAll().Count);

            var Documents = _UOW.ClinicalInvestigatorDisqualificationRepository.GetRecordsByDate(
                new ClinicalInvestigatorDisqualificationSiteData(),
                deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Documents to delete: " + Documents.Count);

            foreach (var document in Documents)
            {
                _UOW.ClinicalInvestigatorDisqualificationRepository.RemoveById(document.RecId);
            }

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "After Delete, ClinicalInvestigatorDisqualificationRepository Count: " + _UOW.ClinicalInvestigatorDisqualificationRepository.GetAll().Count);
        }

        static void deletePHSAdministrativeActionListingSiteData(DateTime deleteDocumentsTillThisDate)
        {
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Before Delete, PHSAdministrativeActionListingRepository Count: " +
                _UOW.PHSAdministrativeActionListingRepository.GetAll().Count);

            var Documents = _UOW.PHSAdministrativeActionListingRepository.GetRecordsByDate(
                new PHSAdministrativeActionListingSiteData(),
                deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Documents to delete: " + Documents.Count);

            foreach (var document in Documents)
            {
                _UOW.PHSAdministrativeActionListingRepository.RemoveById(document.RecId);
            }

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "After Delete, PHSAdministrativeActionListingRepository Count: " + _UOW.PHSAdministrativeActionListingRepository.GetAll().Count);
        }

        static void deleteCBERClinicalInvestigatorInspectionSiteData(DateTime deleteDocumentsTillThisDate)
        {
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Before Delete, CBERClinicalInvestigatorRepository Count: " +
                _UOW.CBERClinicalInvestigatorRepository.GetAll().Count);

            var Documents = _UOW.CBERClinicalInvestigatorRepository.GetRecordsByDate(
                new CBERClinicalInvestigatorInspectionSiteData(),
                deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Documents to delete: " + Documents.Count);

            foreach (var document in Documents)
            {
                _UOW.CBERClinicalInvestigatorRepository.RemoveById(document.RecId);
            }

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "After Delete, CBERClinicalInvestigatorRepository Count: " + _UOW.CBERClinicalInvestigatorRepository.GetAll().Count);
        }

        static void deleteExclusionDatabaseSearchPageSiteData(DateTime deleteDocumentsTillThisDate)
        {
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Before Delete, ExclusionDatabaseSearchRepository Count: " +
                _UOW.ExclusionDatabaseSearchRepository.GetAll().Count);

            var Documents = _UOW.ExclusionDatabaseSearchRepository.GetRecordsByDate(
                new ExclusionDatabaseSearchPageSiteData(),
                deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Documents to delete: " + Documents.Count);

            foreach (var document in Documents)
            {
                _UOW.ExclusionDatabaseSearchRepository.RemoveById(document.RecId);
            }

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "After Delete, ExclusionDatabaseSearchRepository Count: " + _UOW.ExclusionDatabaseSearchRepository.GetAll().Count);
        }

        static void deleteCorporateIntegrityAgreementListSiteData(DateTime deleteDocumentsTillThisDate)
        {
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Before Delete, CorporateIntegrityAgreementRepository Count: " +
                _UOW.CorporateIntegrityAgreementRepository.GetAll().Count);

            var Documents = _UOW.CorporateIntegrityAgreementRepository.GetRecordsByDate(
                new CorporateIntegrityAgreementListSiteData(),
                deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Documents to delete: " + Documents.Count);

            foreach (var document in Documents)
            {
                _UOW.CorporateIntegrityAgreementRepository.RemoveById(document.RecId);
            }

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "After Delete, CorporateIntegrityAgreementRepository Count: " + _UOW.CorporateIntegrityAgreementRepository.GetAll().Count);
        }

        static void deleteSystemForAwardManagementPageSiteData(DateTime deleteDocumentsTillThisDate)
        {
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Before Delete, SystemForAwardManagementRepository Count: " +
                _UOW.SystemForAwardManagementRepository.GetAll().Count);

            var Documents = _UOW.SystemForAwardManagementRepository.GetRecordsByDate(
                new SystemForAwardManagementPageSiteData(),
                deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Documents to delete: " + Documents.Count);

            foreach (var document in Documents)
            {
                _UOW.SystemForAwardManagementRepository.RemoveById(document.RecId);
            }

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "After Delete, SystemForAwardManagementRepository Count: " + _UOW.SystemForAwardManagementRepository.GetAll().Count);
        }

        static void deleteSpeciallyDesignatedNationalsListSiteData(DateTime deleteDocumentsTillThisDate)
        {
            _WriteLog.WriteLog(DateTime.Now.ToString(), "Before Delete, SpeciallyDesignatedNationalsRepository Count: " +
                _UOW.SpeciallyDesignatedNationalsRepository.GetAll().Count);

            var Documents = _UOW.SpeciallyDesignatedNationalsRepository.GetRecordsByDate(
                new SpeciallyDesignatedNationalsListSiteData(),
                deleteDocumentsTillThisDate);

            _WriteLog.WriteLog(DateTime.Now.ToString(), "Documents to delete: " + Documents.Count);

            foreach (var document in Documents)
            {
                _UOW.SpeciallyDesignatedNationalsRepository.RemoveById(document.RecId);
            }

            _WriteLog.WriteLog(DateTime.Now.ToString(),
                "After Delete, SpeciallyDesignatedNationalsRepository Count: " + _UOW.SpeciallyDesignatedNationalsRepository.GetAll().Count);
        }
        #endregion

        public static void TestCache(
            UnitOfWork uow
            //SearchEngine searchEngine,
            //IConfig configFile,
            //CachedSiteScanData cachedData
            )
        {
            var cacheObj = new CachedData();
            //var compFormServ = new ComplianceFormService(uow, searchEngine, configFile, cachedData);
            var LastRecord = uow.AdequateAssuranceListRepository.GetAll()
                .OrderByDescending(s => s.CreatedOn).FirstOrDefault();
            var SiteDataId = LastRecord.RecId;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var FDASearchResult = cacheObj.AdequateAssuranceListSiteDataFromCache();
            sw.Start();
            var step1ElapsedTime = sw.Elapsed;

            sw.Reset();
            sw.Start();
            var FDASearchResult1 = cacheObj.GetFDADebarPageRepositoryCache(SiteDataId);
            sw.Stop();
            var step2ElapsedTime = sw.Elapsed;
        }
    }
}