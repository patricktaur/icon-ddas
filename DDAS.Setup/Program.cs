using DDAS.API.Identity;
using DDAS.Data.Mongo;
using DDAS.Data.Mongo.Indexes;
using DDAS.Data.Mongo.Maps;
using DDAS.Models.Entities.Domain;
using DDAS.Models.Enums;
using DDAS.Services.AppAdminService;
using Microsoft.AspNet.Identity;
using System;
using System.Configuration;
using System.IO;
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
                    CreateFolders(configFile);

                    //Initialize DB for creating Roles and Users:
                    string connString = GetWebConfigConnectionString(configFile, "DefaultConnection");
                      MongoMaps.Initialize();
                     _UOW = new UnitOfWork(connString);
                    CreateRoles();
                    CreateUsers();

                    _AppAdminService = new AppAdminService(_UOW);

                    //Executed on FindMeServerOn 1April2017.
                    var sitesInDB = _UOW.SiteSourceRepository.GetAll();
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
                    ModifySiteSource_ChangeLive2DB();

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
            CreateUser("clarityadmin", "app-admin", "Clarity@148");
            CreateUser("clarityadmin", "admin", "Clarity@148");
            CreateUser("admin", "admin", "Clarity@148"); 

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

        #region Add All Sites to DB
        public void AddSitesInDbCollection(SitesToSearch Site)
        {
            //_UOW.SiteSourceRepository.Add(Site);
            var s1 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "FDA Debarment List", SiteShortName = "FDA Debarment List", SiteEnum = SiteEnum.FDADebarPage, SiteUrl = "http://www.fda.gov/ora/compliance_ref/debar/default.htm" };
            _UOW.SiteSourceRepository.Add(s1);
            var s2 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "Clinical Investigator Inspection List (CLIL)(CDER", SiteShortName = "Clinical Investigator Insp...", SiteEnum = SiteEnum.ClinicalInvestigatorInspectionPage, SiteUrl = "http://www.accessdata.fda.gov/scripts/cder/cliil/index.cfm" };
            _UOW.SiteSourceRepository.Add(s2);
            var s3 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "FDA Warning Letters and Responses", SiteShortName = "FDA Warning Letters ...", SiteEnum = SiteEnum.FDAWarningLettersPage, SiteUrl = "http://www.fda.gov/ICECI/EnforcementActions/WarningLetters/default.htm" };
            _UOW.SiteSourceRepository.Add(s3);
            var s4 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "Notice of Opportunity for Hearing (NOOH) – Proposal to Debar", SiteShortName = "NOOH – Proposal to Debar", SiteEnum = SiteEnum.ERRProposalToDebarPage, SiteUrl = "http://www.fda.gov/RegulatoryInformation/FOI/ElectronicReadingRoom/ucm143240.htm" };
            _UOW.SiteSourceRepository.Add(s4);
            var s5 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "Adequate Assurances List for Clinical Investigators", SiteShortName = "Adequate Assurances List ...", SiteEnum = SiteEnum.AdequateAssuranceListPage, SiteUrl = "http://www.fda.gov/ora/compliance_ref/bimo/asurlist.htm" };
            _UOW.SiteSourceRepository.Add(s5);
            var s6 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "Clinical Investigators – Disqualification Proceedings (FDA Disqualified/Restricted)", SiteShortName = "Disqualification Proceedings ...", SiteEnum = SiteEnum.ClinicalInvestigatorDisqualificationPage, SiteUrl = "http://www.accessdata.fda.gov/scripts/SDA/sdNavigation.cfm?sd=clinicalinvestigatorsdisqualificationproceedings&previewMode=true&displayAll=true" };
            _UOW.SiteSourceRepository.Add(s6);
            var s7 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "PHS Administrative Actions Listing ", SiteShortName = "PHS Administrative Actions", SiteEnum = SiteEnum.PHSAdministrativeActionListingPage, SiteUrl = "https://ori.hhs.gov/ORI_PHS_alert.html?d=update" };
            _UOW.SiteSourceRepository.Add(s7);
            var s8 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "Clinical Investigator Inspection List (CBER)", SiteShortName = "CBER Clinical Investigator ...", SiteEnum = SiteEnum.CBERClinicalInvestigatorInspectionPage, SiteUrl = "http://www.fda.gov/BiologicsBloodVaccines/GuidanceComplianceRegulatoryInformation/ComplianceActivities/ucm195364.htm" };
            _UOW.SiteSourceRepository.Add(s8);
            var s9 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "HHS/OIG/ EXCLUSIONS DATABASE SEARCH/ FRAUD", SiteShortName = "HHS/OIG/ EXCLUSIONS ...", SiteEnum = SiteEnum.ExclusionDatabaseSearchPage, SiteUrl = "https://oig.hhs.gov/exclusions/exclusions_list.asp" };
            _UOW.SiteSourceRepository.Add(s9);
            var s10 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "HHS/OIG Corporate Integrity Agreements/Watch List", SiteShortName = "HHS/OIG Corporate Integrity", SiteEnum = SiteEnum.CorporateIntegrityAgreementsListPage, SiteUrl = "http://oig.hhs.gov/compliance/corporate-integrity-agreements/cia-documents.asp" };
            _UOW.SiteSourceRepository.Add(s10);
            var s11 =
                    new SitesToSearch { ExtractionMode = "Live", SiteName = "SAM/SYSTEM FOR AWARD MANAGEMENT", SiteShortName = "SAM/SYSTEM FOR AWARD ...", SiteEnum = SiteEnum.SystemForAwardManagementPage, SiteUrl = "https://www.sam.gov/portal/public/SAM" };
            _UOW.SiteSourceRepository.Add(s11);
            var s12 =
                    new SitesToSearch { ExtractionMode = "DB", SiteName = "LIST OF SPECIALLY DESIGNATED NATIONALS", SiteShortName = "SPECIALLY DESIGNATED ...", SiteEnum = SiteEnum.SpeciallyDesignedNationalsListPage, SiteUrl = "http://www.treasury.gov/resource-center/sanctions/SDN-List/Pages/default.aspx" };
            _UOW.SiteSourceRepository.Add(s12);
            var s13 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "World Check (Only for PI)", SiteShortName = "World Check...", SiteEnum = SiteEnum.WorldCheckPage, SiteUrl = "http://www.truthtechnologies.com/" };
            _UOW.SiteSourceRepository.Add(s13);


            var site1 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Pfizer DMC Checks", SiteShortName = "Pfizer DMC Checks", SiteEnum = SiteEnum.PfizerDMCChecksPage, SiteUrl = " http://ecf12.pfizer.com/sites/clinicaloversightcommittees/default.aspx" };
            _UOW.SiteSourceRepository.Add(site1);
            var site2 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Pfizer Unavailable Checks", SiteShortName = "Pfizer DMC Checks", SiteEnum = SiteEnum.PfizerUnavailableChecksPage, SiteUrl = "http://ecf12.pfizer.com/" };
            _UOW.SiteSourceRepository.Add(site2);
            var site3 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "GSK Do Not Use Check", SiteShortName = "GSK DNU Check", SiteEnum = SiteEnum.GSKDoNotUseCheckPage, SiteUrl = "" };
            _UOW.SiteSourceRepository.Add(site3);
            var site4 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Regeneron Usability Check", SiteShortName = "Regeneron Usability Check", SiteEnum = SiteEnum.RegeneronUsabilityCheckPage, SiteUrl = "" };
            _UOW.SiteSourceRepository.Add(site4);
            var site5 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "AUSTRALIAN HEALTH PRACTITIONER REGULATION AGENCY", SiteShortName = "HEALTH PRACTITIONER ...", SiteEnum = SiteEnum.AustralianHealthPratitionerRegulationPage, SiteUrl = "http://www.ahpra.gov.au/Registration/Registers-of-Practitioners.aspx" };
            _UOW.SiteSourceRepository.Add(site5);
            var site6 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Belgium1 - ZOEK EEN ARTS", SiteShortName = "ZOEK EEN ARTS", SiteEnum = SiteEnum.ZoekEenArtsPage, SiteUrl = "https://ordomedic.be/nl/zoek-een-arts/" };
            _UOW.SiteSourceRepository.Add(site6);
            var site7 =
                new SitesToSearch { ExtractionMode = "Manual", SiteName = "Belgium2 - RIZIV - Zoeken", SiteShortName = "RIZIV - Zoeken", SiteEnum = SiteEnum.RizivZoekenPage, SiteUrl = "https://www.riziv.fgov.be/webprd/appl/psilverpages/nl" };
            _UOW.SiteSourceRepository.Add(site7);
            var site8 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Brazil - CONSELHOS DE MEDICINA", SiteShortName = "CONSELHOS DE MEDICINA", SiteEnum = SiteEnum.ConselhosDeMedicinaPage, SiteUrl = "http://portal.cfm.org.br/index.php?option=com_medicos&Itemid=59" };
            _UOW.SiteSourceRepository.Add(site8);
            var site9 =
                new SitesToSearch { ExtractionMode = "Manual", SiteName = "Colombia - EL TRIBUNAL NACIONAL DE ÉTICA MÉDICA", SiteShortName = "EL TRIBUNAL NACIONAL...", SiteEnum = SiteEnum.TribunalNationalDeEticaMedicaPage, SiteUrl = "http://www.tribunalnacionaldeeticamedica.org/site/biblioteca_documental" };
            _UOW.SiteSourceRepository.Add(site9);
            var site10 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Finland - VALVIRA", SiteShortName = "VALVIRA", SiteEnum = SiteEnum.ValviraPage, SiteUrl = "https://julkiterhikki.valvira.fi/" };
            _UOW.SiteSourceRepository.Add(site10);
            var site11 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "France - CONSEIL NATIONAL DE L'ORDRE DES MEDECINS", SiteShortName = "CONSEIL NATIONAL DE L'ORDRE...", SiteEnum = SiteEnum.ConseilNationalDeMedecinsPage, SiteUrl = "http://www.conseil-national.medecin.fr/annuaire" };
            _UOW.SiteSourceRepository.Add(site11);
            var site12 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "MEDICAL COUNCIL OF INDIA", SiteShortName = "MEDICAL COUNSIL OF INDIA", SiteEnum = SiteEnum.MedicalCouncilOfIndiaPage, SiteUrl = "http://online.mciindia.org/online//Index.aspx?qstr_level=01" };
            _UOW.SiteSourceRepository.Add(site12);
            var site13 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Israel - MINISTRY OF HEALTH ISRAEL", SiteShortName = "MINISTRY OF HEALTH ISRAEL", SiteEnum = SiteEnum.MinistryOfHealthIsraelPage, SiteUrl = "http://www.health.gov.il/UnitsOffice/HR/professions/postponements/Pages/default.aspx" };
            _UOW.SiteSourceRepository.Add(site13);
            var site14 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "New Zeland - LIST OF REGISTERED DOCTORS", SiteShortName = "LIST OF REGISTERED DOCTORS", SiteEnum = SiteEnum.ListOfRegisteredDoctorsPage, SiteUrl = "https://www.mcnz.org.nz/support-for-doctors/list-of-registered-doctors/" };
            _UOW.SiteSourceRepository.Add(site14);
            var site15 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Poland - NACZELNA IZBA LEKARSKA", SiteShortName = "NACZELNA IZBA LEKARSKA", SiteEnum = SiteEnum.NaczelnaIzbaLekarskaPage, SiteUrl = "http://rejestr.nil.org.pl/xml/nil/rejlek/hurtd" };
            _UOW.SiteSourceRepository.Add(site15);
            var site16 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Portugal - PORTAL OFICIAL DA ORDEM DOS MEDICOS", SiteShortName = "PORTAL OFICIAL DA ORDEM...", SiteEnum = SiteEnum.PortalOficialDaOrdemDosMedicosPage, SiteUrl = "https://www.ordemdosmedicos.pt/" };
            _UOW.SiteSourceRepository.Add(site16);
            var site17 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "Spain - ORGANIZACION MEDICA COLEGIAL DE ESPANA", SiteShortName = "ORGANIZACION MEDICA COLEGIAL...", SiteEnum = SiteEnum.OrganizacionMedicaColegialDeEspanaPage, SiteUrl = "http://www.cgcom.es/consultapublicacolegiados" };
            _UOW.SiteSourceRepository.Add(site17);
            var site18 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "SINGAPORE MEDICAL COUNCIL", SiteShortName = "SINGAPORE MEDICAL COUNCIL...", SiteEnum = SiteEnum.SingaporeMedicalCouncilPage, SiteUrl = "http://www.healthprofessionals.gov.sg/content/hprof/smc/en.html" };
            _UOW.SiteSourceRepository.Add(site18);
            var site19 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "SRI LANKA MEDICAL COUNCIL", SiteShortName = "SRI LANKA MEDICAL COUNCIL...", SiteEnum = SiteEnum.SriLankaMedicalCouncilPage, SiteUrl = "http://www.srilankamedicalcouncil.org/registry.php" };
            _UOW.SiteSourceRepository.Add(site19);
            var site20 =
                    new SitesToSearch { ExtractionMode = "Manual", SiteName = "HEALTH GUIDE USA", SiteShortName = "HEALTH GUIDE USA", SiteEnum = SiteEnum.HealthGuideUSAPage, SiteUrl = "http://www.healthguideusa.org/medical_license_lookup.htm" };
            _UOW.SiteSourceRepository.Add(site20);
        }
        #endregion

        static void CreateIndexes()
        {
            Indexes idx = new Indexes();
            var x = idx.CreateIndex();
        }
    }
}