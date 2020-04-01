using Models.GageModels;
using Models.UniversalModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Service.GageService
{
    public class GageAdjustServiceImpl
    {
        public bool ArchiveAdjustSlip(GageAdjustSlip paras)
        {
            using (SqlConnection conn = new SqlConnection(Common.SQLHelper.Asset_strConn))
            {
                conn.Open();
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                try
                {
                    GageAdjustSlip adjustSlip = GageAdjustSlip.GetBy(paras.ID);
                    paras.CopyTo(adjustSlip, false);
                    adjustSlip.SysArchiveTime = DateTime.Now;


                    Asset gage = Asset.GetBy(adjustSlip.AssetID);
                    if (gage == null) throw new Exception("资产不存在");
                    if (!gage.Status.Contains("在校")) throw new Exception("资产未处于在校状态");
                    gage.RemoveStatus("在校");
                    gage.RefreshPosition();


                    gage.sqlTransaction = adjustSlip.sqlTransaction = sqlTransaction;
                    gage.Update();
                    adjustSlip.Update();

                    sqlTransaction.Commit();
                    return true;
                }
                catch
                {
                    sqlTransaction.Rollback();
                    throw;
                }
            }
        }

        public bool AddAdjustSlip(GageAdjustSlip adjustSlip, User CurrentUser)
        {
            using (SqlConnection conn = new SqlConnection(Common.SQLHelper.Asset_strConn))
            {
                conn.Open();
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                try
                {
                    Asset gage = Asset.GetBy(adjustSlip.AssetID);
                    if (gage == null) throw new Exception("资产不存在");
                    if (!"在库领用".Contains(gage.Status.Substring(gage.Status.Length - 2))) throw new Exception("资产未处于在库或领用状态");


                    Gage gageDetail = Gage.GetBy(gage.AssetID).First();
                    if (gageDetail.StandardAdjustType == "免校")
                        throw new Exception("无法为免校的资产做校准");
                    gageDetail.NextAdjustDate = null;


                    adjustSlip.AssetName = gage.AssetName;
                    adjustSlip.AdjustDate = DateTime.Now;
                    adjustSlip.TransactorName = CurrentUser.Name;


                    gage.Status = gage.AddStatus("在校");
                    gage.CurrentWhereSpot = adjustSlip.Spot;
                    gage.CurrentWhereOrganization = adjustSlip.Organization;


                    gageDetail.sqlTransaction = gage.sqlTransaction = adjustSlip.sqlTransaction = sqlTransaction;
                    adjustSlip.Add();
                    gage.Update();
                    gageDetail.Update();

                    sqlTransaction.Commit();
                    return true;
                }
                catch
                {
                    sqlTransaction.Rollback();
                    throw;
                }
            }
        }

        public bool UploadAdjustReport(HttpPostedFile postedFile, int RequestID, int AdjustSlip_ID)
        {
            byte[] fileContents = new byte[postedFile.ContentLength];
            postedFile.InputStream.Read(fileContents, 0, fileContents.Length);

            string SavedName = RequestID + "_" + postedFile.FileName;
            if (Common.FtpRepository.UploadFile(fileContents, "/AssetSystem/GageAdjustReport/", SavedName) == true)
            {
                GageAdjustReport Report = new GageAdjustReport();

                Report.FK_AdjustSlip_ID = AdjustSlip_ID;
                Report.Date = DateTime.Now;
                Report.Path = "http://192.168.9.3:8888/AssetSystem/GageAdjustReport/" + SavedName;
                Report.Add();
            }
            return true;
        }

        public bool DeleteAdjustReport(int AdjustReport_ID)
        {
            GageAdjustReport report = GageAdjustReport.GetBy(AdjustReport_ID);
            report.Delete();
            return true;
        }

        public IEnumerable<GageAdjustReport> GetAdjusReportsOf(int AdjustSlip_ID)
        {
            return GageAdjustReport.GetReportsOf(AdjustSlip_ID);
        }

        public IEnumerable<GageAdjustSlip> GetAllAdjustSlips()
        {
            return GageAdjustSlip.GetAll();
        }

        public IEnumerable<GageAdjustSlip> GetAdjustSlipsOf(string AssetID)
        {
            return GageAdjustSlip.GetBy(AssetID);
        }
    }
}
