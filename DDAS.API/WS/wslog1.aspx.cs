using DDAS.Data.Mongo;
using DDAS.Models.Entities;
using DDAS.Models.Entities.Domain;
using System;
using System.Collections.Generic;
using WebScraping.Selenium.SearchEngine;

namespace DDAS.API.WS
{
    public partial class wslog1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RefreshGrid();
        }

        private void RefreshGrid()
        {
            //var objLog = new LogWSDDAS();
            //List<LogWSDDAS> objLog;

            var ConnectionString =
                         System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            var DBName =
                System.Configuration.ConfigurationManager.AppSettings["DBName"];

            var _uow = new UnitOfWork(ConnectionString, DBName);
            var _config = new Config();
            var _SearchEngine = new SearchEngine(_uow, _config);

            var LogDetails = _uow.ComplianceFormRepository.GetAll();

            Response.Write( LogDetails.Count);
            dgGrid.DataSource = LogDetails;
            dgGrid.DataBind();
        }
    }
}