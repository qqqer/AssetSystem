using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Models.GageModels;
using Models.UniversalModels;
using Service.GageService;
using System.IO;
using System.Net.Http.Headers;
using Service.UniversalService;

namespace WebAPI_QM.Controllers
{
    
    public class GageController : System.Web.Http.ApiController
    {
        [System.Web.Http.HttpPost]
        public IEnumerable<Gage> SelectBy(GageFilter Filter)
        {
            IEnumerable<Gage> Gages =  new GageServiceImpl().SelectBy(Filter);
            return Gages;
        }


        [System.Web.Http.HttpPost]
        public string GetGageLedgerDownloadURL(GageFilter Filter)
        {
            string excel_name =  new GageServiceImpl().ExportGageLedgerToExcel(Filter);

            if (excel_name != null)
                return "http://192.168.9.49:8020/report/" + excel_name;
            else
                return null;
        }

        [System.Web.Http.HttpGet]
        public IEnumerable<Gage> GetBy(string AssetID)
        {
            IEnumerable<Gage> Gages =  new GageServiceImpl().GetBy(AssetID);
            return Gages;
        }

        [System.Web.Http.HttpGet]
        public IEnumerable<LendSlip> GetLendSlipsOf(string AssetID)
        {
            IEnumerable<LendSlip> LendSlips = new GageServiceImpl().GetLendSlipsOf(AssetID);
            return LendSlips;
        }

        [System.Web.Http.HttpGet]
        public IEnumerable<LendSlip> GetAllLendSlips()
        {
            IEnumerable<LendSlip> LendSlips = new GageServiceImpl().GetAllLendSlips();
            return LendSlips;
        }

        [System.Web.Http.HttpGet]
        public IEnumerable<RepairSlip> GetRepairSlipsOf(string AssetID)
        {
            IEnumerable<RepairSlip> RepairSlips = new GageServiceImpl().GetRepairSlipsOf(AssetID);
            return RepairSlips;
        }

        [System.Web.Http.HttpGet]
        public IEnumerable<GageAdjustSlip> GetAdjustSlipsOf(string AssetID)
        {
            IEnumerable<GageAdjustSlip> AdjustSlips = new GageAdjustServiceImpl().GetAdjustSlipsOf(AssetID);
            return AdjustSlips;
        }

        [System.Web.Http.HttpGet]
        public IEnumerable<GageAdjustSlip> GetAllAdjustSlips()
        {
            IEnumerable<GageAdjustSlip> AdjustSlips = new GageAdjustServiceImpl().GetAllAdjustSlips();
            return AdjustSlips;
        }

        [System.Web.Http.HttpGet]
        public IEnumerable<DiscardSlip> GetDiscardSlipsOf(string AssetID)
        {
            IEnumerable<DiscardSlip> DiscardSlips = new GageServiceImpl().GetDiscardSlipsOf(AssetID);
            return DiscardSlips;
        }

        [System.Web.Http.HttpGet]
        public IEnumerable<GageAdjustReport> GetAdjusReportsOf(int AdjustSlip_ID)
        {
            IEnumerable<GageAdjustReport> AdjustReports = new GageAdjustServiceImpl().GetAdjusReportsOf(AdjustSlip_ID);
            return AdjustReports;
        }

        [System.Web.Http.HttpGet]
        public IEnumerable<AssetFile> GetAssetFilesOf(string AssetID, string FileCategory)
        {
            IEnumerable<AssetFile> assetFiles = new GageServiceImpl().GetAssetFilesOf(AssetID, FileCategory);
            return assetFiles;
        }

        [Route("Gage/Add")]
        [System.Web.Http.HttpPost]
        public bool Add(Gage Gage)
        {
            int API = 2;
            string token = HttpContext.Current.Request.Headers.Get("token");

            if (Models.UniversalModels.User.IsValidToken(token))
            {
                User CurrentUser = Models.UniversalModels.User.GetUserByToken(token);
                if (Models.UniversalModels.User.IsContainsCurrentAPIPermission(CurrentUser.UserID, API))
                {
                    Log log = new Log(API, CurrentUser);
                    log.Add(token + "|" + nameof(Gage) + ":" + JsonConvert.SerializeObject(Gage));

                    try
                    {
                        bool s =  new GageServiceImpl().Add(Gage);
                        return s;
                    }
                    catch (Exception ex)
                    {
                        log.Add("Exception:" + ex.Message + "|"+ ex.StackTrace);
                        return false;
                    }
                }
            }

            throw new HttpResponseException(HttpStatusCode.Forbidden);



        }//2

        [Route("Gage/Update")]
        [System.Web.Http.HttpPost]
        public bool Update(Gage Gage)
        {
            int API = 3;
            string token = HttpContext.Current.Request.Headers.Get("token");

            if (Models.UniversalModels.User.IsValidToken(token))
            {
                User CurrentUser = Models.UniversalModels.User.GetUserByToken(token);

                if (Models.UniversalModels.User.IsContainsCurrentAPIPermission(CurrentUser.UserID, API))
                {
                    Log log = new Log(API, CurrentUser);
                    log.Add(token + "|" + nameof(Gage) + ":" + JsonConvert.SerializeObject(Gage));

                    try
                    {
                        bool s = new GageServiceImpl().Update(Gage);
                        return s;
                    }
                    catch (Exception ex)
                    {
                        log.Add("Exception:" + ex.Message + "|"+ ex.StackTrace);
                        return false;
                    }
                }
            }
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }//3

        [Route("Gage/AddLendSlip")]
        [System.Web.Http.HttpPost]
        public bool AddLendSlip(LendSlip LendSlip)
        {
            int API = 4;
            string token = HttpContext.Current.Request.Headers.Get("token");

            if (Models.UniversalModels.User.IsValidToken(token))
            {
                User CurrentUser = Models.UniversalModels.User.GetUserByToken(token);

                if (Models.UniversalModels.User.IsContainsCurrentAPIPermission(CurrentUser.UserID, API))
                {
                    Log log = new Log(API, CurrentUser);
                    log.Add(token + "|" + nameof(LendSlip) + ":" + JsonConvert.SerializeObject(LendSlip));

                    try
                    {
                        bool s =new GageServiceImpl().AddLendSlip(LendSlip, CurrentUser);
                        return s;
                    }
                    catch (Exception ex)
                    {
                        log.Add("Exception:" + ex.Message + "|"+ ex.StackTrace);
                        return false;
                    }
                }
            }
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }//4

        [Route("Gage/ArchiveLendSlip")]
        [System.Web.Http.HttpPost]
        public bool ArchiveLendSlip(LendSlip paras)
        {
            int API = 5;
            string token = HttpContext.Current.Request.Headers.Get("token");

            if (Models.UniversalModels.User.IsValidToken(token))
            {
                User CurrentUser = Models.UniversalModels.User.GetUserByToken(token);

                if (Models.UniversalModels.User.IsContainsCurrentAPIPermission(CurrentUser.UserID, API))
                {
                    Log log = new Log(API, CurrentUser);
                    log.Add(token + "|" + nameof(paras) + ":" + JsonConvert.SerializeObject(paras));

                    try
                    {
                        bool s = new GageServiceImpl().ArchiveLendSlip(paras);
                        return s;
                    }
                    catch (Exception ex)
                    {
                        log.Add("Exception:" + ex.Message + "|"+ ex.StackTrace);
                        return false;
                    }
                }
            }
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }//5

        [Route("Gage/AddRepairSlip")]
        [System.Web.Http.HttpPost]
        public bool AddRepairSlip(RepairSlip RepairSlip)
        {
            int API = 6;
            string token = HttpContext.Current.Request.Headers.Get("token");

            if (Models.UniversalModels.User.IsValidToken(token))
            {
                User CurrentUser = Models.UniversalModels.User.GetUserByToken(token);

                if (Models.UniversalModels.User.IsContainsCurrentAPIPermission(CurrentUser.UserID, API))
                {
                    Log log = new Log(API, CurrentUser);
                    log.Add(token + "|" + nameof(RepairSlip) + ":" + JsonConvert.SerializeObject(RepairSlip));

                    try
                    {
                        bool s = new GageServiceImpl().AddRepairSlip(RepairSlip, CurrentUser);
                        return s;
                    }
                    catch (Exception ex)
                    {
                        log.Add("Exception:" + ex.Message + "|"+ ex.StackTrace);
                        return false;
                    }
                }
            }
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }//6

        [Route("Gage/ArchiveRepairSlip")]
        [System.Web.Http.HttpPost]
        public bool ArchiveRepairSlip(RepairSlip RepairSlip)
        {
            int API = 7;
            string token = HttpContext.Current.Request.Headers.Get("token");

            if (Models.UniversalModels.User.IsValidToken(token))
            {
                User CurrentUser = Models.UniversalModels.User.GetUserByToken(token);

                if (Models.UniversalModels.User.IsContainsCurrentAPIPermission(CurrentUser.UserID, API))
                {
                    Log log = new Log(API, CurrentUser);
                    log.Add(token + "|" + nameof(RepairSlip) + ":" + JsonConvert.SerializeObject(RepairSlip));

                    try
                    {
                        bool s = new GageServiceImpl().ArchiveRepairSlip(RepairSlip);
                        return s;
                    }
                    catch (Exception ex)
                    {
                        log.Add("Exception:" + ex.Message + "|"+ ex.StackTrace);
                        return false;
                    }
                }
            }
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }//7

        [Route("Gage/AddDiscardSlip")]
        [System.Web.Http.HttpPost]
        public bool AddDiscardSlip(DiscardSlip DiscardSlip)
        {
            int API = 8;
            string token = HttpContext.Current.Request.Headers.Get("token");

            if (Models.UniversalModels.User.IsValidToken(token))
            {
                User CurrentUser = Models.UniversalModels.User.GetUserByToken(token);

                if (Models.UniversalModels.User.IsContainsCurrentAPIPermission(CurrentUser.UserID, API))
                {
                    Log log = new Log(API, CurrentUser);
                    log.Add(token + "|" + nameof(DiscardSlip) + ":" + JsonConvert.SerializeObject(DiscardSlip));

                    try
                    {
                        bool s = new GageServiceImpl().AddDiscardSlip(DiscardSlip, CurrentUser);
                        return s;
                    }
                    catch (Exception ex)
                    {
                        log.Add("Exception:" + ex.Message + "|"+ ex.StackTrace);
                        return false;
                    }
                }
            }
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }//8

        [Route("Gage/UndoDiscardSlip")]
        [System.Web.Http.HttpPost]
        public bool UndoDiscardSlip(DiscardSlip DiscardSlip)
        {
            int API = 9;
            string token = HttpContext.Current.Request.Headers.Get("token");

            if (Models.UniversalModels.User.IsValidToken(token))
            {
                User CurrentUser = Models.UniversalModels.User.GetUserByToken(token);

                if (Models.UniversalModels.User.IsContainsCurrentAPIPermission(CurrentUser.UserID, API))
                {
                    Log log = new Log(API, CurrentUser);
                    log.Add(token + "|" + nameof(DiscardSlip) + ":" + JsonConvert.SerializeObject(DiscardSlip));

                    try
                    {
                        bool s = new GageServiceImpl().UndoDiscardSlip(DiscardSlip);
                        return s;
                    }
                    catch (Exception ex)
                    {
                        log.Add("Exception:" + ex.Message + "|"+ ex.StackTrace);
                        return false;
                    }
                }
            }
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }//9

        [Route("Gage/UploadAdjustReport")]
        [System.Web.Http.HttpPost]
        public bool UploadAdjustReport()
        {
            int API = 10;

            string token = HttpContext.Current.Request.Headers.Get("token");
            int AdjustSlip_ID = Convert.ToInt32(HttpContext.Current.Request.Headers.GetValues("AdjustSlip_ID").First());
            HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];

            if (Models.UniversalModels.User.IsValidToken(token))
            {
                User CurrentUser = Models.UniversalModels.User.GetUserByToken(token);

                if (Models.UniversalModels.User.IsContainsCurrentAPIPermission(CurrentUser.UserID, API))
                {
                    Log log = new Log(API, CurrentUser);
                    log.Add(token + "|" + "AdjustSlip_ID:" + AdjustSlip_ID);

                    try
                    {
                        bool s = new GageAdjustServiceImpl().UploadAdjustReport(postedFile, log.RequestID, AdjustSlip_ID);
                        return s;
                    }
                    catch (Exception ex)
                    {
                        log.Add("Exception:" + ex.Message + "|"+ ex.StackTrace);
                        return false;
                    }
                }
            }
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }//10

        [Route("Gage/DeleteAdjustReport")]
        [System.Web.Http.HttpPost]
        public bool DeleteAdjustReport(GageAdjustReport paras)
        {
            int API = 11;

            string token = HttpContext.Current.Request.Headers.Get("token");
            if (Models.UniversalModels.User.IsValidToken(token))
            {
                User CurrentUser = Models.UniversalModels.User.GetUserByToken(token);

                if (Models.UniversalModels.User.IsContainsCurrentAPIPermission(CurrentUser.UserID, API))
                {
                    Log log = new Log(API, CurrentUser);
                    log.Add(token + "|" + nameof(paras) + ":" + JsonConvert.SerializeObject(paras));

                    try
                    {
                        bool s = new GageAdjustServiceImpl().DeleteAdjustReport(paras.ID);
                        return s;
                    }
                    catch (Exception ex)
                    {
                        log.Add("Exception:" + ex.Message + "|"+ ex.StackTrace);
                        return false;
                    }
                }
            }
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }//11

        [Route("Gage/UploadAssetFile")]
        [System.Web.Http.HttpPost]
        public bool UploadAssetFile()
        {
            int API = 12;

            string token = HttpContext.Current.Request.Headers.Get("token");
            string AssetID = Convert.ToString(HttpContext.Current.Request.Headers.GetValues("AssetID").First());
            string FileCategory = Convert.ToString(HttpContext.Current.Request.Headers.GetValues("FileCategory").First());
            HttpPostedFile postedFile = HttpContext.Current.Request.Files[0];

            if (Models.UniversalModels.User.IsValidToken(token))
            {
                User CurrentUser = Models.UniversalModels.User.GetUserByToken(token);

                if (Models.UniversalModels.User.IsContainsCurrentAPIPermission(CurrentUser.UserID, API))
                {
                    Log log = new Log(API, CurrentUser);
                    log.Add(token + "|" + "AssetID:" + AssetID + "   FileCategory:" + FileCategory);

                    try
                    {
                        bool s = new GageServiceImpl().UploadAssetFile(postedFile, log.RequestID, FileCategory, AssetID);
                        return s;
                    }
                    catch (Exception ex)
                    {
                        log.Add("Exception:" + ex.Message + "|"+ ex.StackTrace);
                        return false;
                    }
                }
            }
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }//12

        [Route("Gage/DeleteAssetFile")]
        [System.Web.Http.HttpPost]
        public bool DeleteAssetFile(GageAdjustReport paras)
        {
            int API = 13;

            string token = HttpContext.Current.Request.Headers.Get("token");
            if (Models.UniversalModels.User.IsValidToken(token))
            {
                User CurrentUser = Models.UniversalModels.User.GetUserByToken(token);

                if (Models.UniversalModels.User.IsContainsCurrentAPIPermission(CurrentUser.UserID, API))
                {
                    Log log = new Log(API, CurrentUser);
                    log.Add(token + "|" + nameof(paras) + ":" + JsonConvert.SerializeObject(paras));

                    try
                    {
                        bool s = new GageServiceImpl().DeleteAssetFile(paras.ID);
                        return s;
                    }
                    catch (Exception ex)
                    {
                        log.Add("Exception:" + ex.Message + "|"+ ex.StackTrace);
                        return false;
                    }
                }
            }
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }//13

        [Route("Gage/AddAdjustSlip")]
        [System.Web.Http.HttpPost]
        public bool AddAdjustSlip(GageAdjustSlip AdjustSlip)
        {
            int API = 14;
            string token = HttpContext.Current.Request.Headers.Get("token");

            if (Models.UniversalModels.User.IsValidToken(token))
            {
                User CurrentUser = Models.UniversalModels.User.GetUserByToken(token);

                if (Models.UniversalModels.User.IsContainsCurrentAPIPermission(CurrentUser.UserID, API))
                {
                    Log log = new Log(API, CurrentUser);
                    log.Add(token + "|" + nameof(AdjustSlip) + ":" + JsonConvert.SerializeObject(AdjustSlip));

                    try
                    {
                        bool s = new GageAdjustServiceImpl().AddAdjustSlip(AdjustSlip, CurrentUser);
                        return s;
                    }
                    catch (Exception ex)
                    {
                        log.Add("Exception:" + ex.Message + "|"+ ex.StackTrace);
                        return false;
                    }
                }
            }
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }//14

        [Route("Gage/ArchiveAdjustSlip")]
        [System.Web.Http.HttpPost]
        public bool ArchiveAdjustSlip(GageAdjustSlip paras)
        {
            int API = 15;
            string token = HttpContext.Current.Request.Headers.Get("token");

            if (Models.UniversalModels.User.IsValidToken(token))
            {
                User CurrentUser = Models.UniversalModels.User.GetUserByToken(token);

                if (Models.UniversalModels.User.IsContainsCurrentAPIPermission(CurrentUser.UserID, API))
                {
                    Log log = new Log(API, CurrentUser);
                    log.Add(token + "|" + nameof(paras) + ":" + JsonConvert.SerializeObject(paras));

                    try
                    {
                        bool s = new GageAdjustServiceImpl().ArchiveAdjustSlip(paras);
                        return s;
                    }
                    catch (Exception ex)
                    {
                        log.Add("Exception:" + ex.Message + "|"+ ex.StackTrace);
                        return false;
                    }
                }
            }
            throw new HttpResponseException(HttpStatusCode.Forbidden);
        }//15
    }
}