using Models.UniversalModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Models.GageModels;

namespace Service.UniversalService
{
    abstract public class UniversalServiceBase
    {
        public virtual bool AddLendSlip(LendSlip lendSlip, User CurrentUser)
        {
            using (SqlConnection conn = new SqlConnection(Common.SQLHelper.Asset_strConn))
            {
                conn.Open();
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                try
                {
                    Asset asset = Asset.GetBy(lendSlip.AssetID);
                    if (asset == null) throw new Exception("资产不存在");
                    if (asset.Status != "在库") throw new Exception("资产未处于在库状态");

                    lendSlip.AssetName = asset.AssetName;
                    lendSlip.AssetCategory = asset.AssetCategory;
                    lendSlip.ApplicantName = Personnel.GetBy(lendSlip.ApplicantID).Name;
                    lendSlip.LendDate = DateTime.Now;
                    lendSlip.TransactorName = CurrentUser.Name;
                    lendSlip.ReceiverDepartment = Personnel.GetBy(lendSlip.ApplicantID).Department;

                    asset.CurrentWhereSpot = lendSlip.WhereUsing;// + lendSlip.ApplicantName;
                    asset.CurrentWhereOrganization = lendSlip.ReceiverDepartment;
                    asset.Status = asset.AddStatus("领用");

                    lendSlip.sqlTransaction = asset.sqlTransaction = sqlTransaction;
                    lendSlip.Add();
                    asset.Update();

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

        public virtual bool ArchiveLendSlip(LendSlip paras)
        {
            using (SqlConnection conn = new SqlConnection(Common.SQLHelper.Asset_strConn))
            {
                conn.Open();
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                try
                {
                    LendSlip lendSlip = LendSlip.GetBy(paras.ID);


                    Asset asset = Asset.GetBy(lendSlip.AssetID);
                    if (asset == null) throw new Exception("资产不存在");
                    if (!asset.Status.Contains("领用")) throw new Exception("资产未被领用");


                    asset.RemoveStatus("领用");
                    asset.RefreshPosition();

                    paras.CopyTo(lendSlip, false);
                    lendSlip.ReturnDate = DateTime.Now;

                    asset.sqlTransaction = lendSlip.sqlTransaction = sqlTransaction;
                    asset.Update();
                    lendSlip.Update();

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

        public virtual bool AddRepairSlip(RepairSlip repairSlip, User CurrentUser)
        {
            using (SqlConnection conn = new SqlConnection(Common.SQLHelper.Asset_strConn))
            {
                conn.Open();
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                try
                {
                    Asset asset = Asset.GetBy(repairSlip.AssetID);
                    if (asset == null) throw new Exception("资产不存在");
                    if (!"在库领用".Contains(asset.Status.Substring(asset.Status.Length - 2))) throw new Exception("资产未处于在库或领用状态");

                    repairSlip.AssetName = asset.AssetName;
                    repairSlip.AssetCategory = asset.AssetCategory;
                    repairSlip.ApplicantName = Personnel.GetBy(repairSlip.ApplicantID).Name;
                    repairSlip.RepairDate = DateTime.Now;
                    repairSlip.TransactorName = CurrentUser.Name;


                    asset.Status = asset.AddStatus("维修");
                    asset.CurrentWhereOrganization = repairSlip.Organization;
                    asset.CurrentWhereSpot = repairSlip.Spot;// + repairSlip.ApplicantName;


                    asset.sqlTransaction = repairSlip.sqlTransaction = sqlTransaction;

                    repairSlip.Add();
                    asset.Update();

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

        public virtual bool ArchiveRepairSlip(RepairSlip paras)
        {
            using (SqlConnection conn = new SqlConnection(Common.SQLHelper.Asset_strConn))
            {
                conn.Open();
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                try
                {
                    RepairSlip repairSlip = RepairSlip.GetBy(paras.ID);


                    Asset asset = Asset.GetBy(repairSlip.AssetID);
                    if (asset == null) throw new Exception("资产不存在");
                    if (!asset.Status.Contains("维修")) throw new Exception("资产未被维修");


                    asset.RemoveStatus("维修");
                    asset.RefreshPosition();


                    paras.CopyTo(repairSlip, false);
                    repairSlip.RepairEndDate = DateTime.Now;


                    asset.sqlTransaction = repairSlip.sqlTransaction = sqlTransaction;
                    asset.Update();
                    repairSlip.Update();

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

        public virtual bool AddDiscardSlip(DiscardSlip discardSlip, User CurrentUser)
        {
            using (SqlConnection conn = new SqlConnection(Common.SQLHelper.Asset_strConn))
            {
                conn.Open();
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                try
                {
                    Asset asset = Asset.GetBy(discardSlip.AssetID);

                    if (asset == null) throw new Exception("资产不存在");
                    if (asset.Status.Contains("报废")) throw new Exception("资产处于报废状态");
                    if (asset.Status != "在库") throw new Exception("资产未处于在库状态");


                    discardSlip.AssetName = asset.AssetName;
                    discardSlip.AssetCategory = asset.AssetCategory;
                    discardSlip.DiscardDate = DateTime.Now;
                    discardSlip.TransactorName = CurrentUser.Name;
                    discardSlip.Add();

                    asset.Status = asset.AddStatus("报废");

                    asset.sqlTransaction = discardSlip.sqlTransaction = sqlTransaction;
                    asset.Update();

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

        public virtual bool UndoDiscardSlip(DiscardSlip paras)
        {
            using (SqlConnection conn = new SqlConnection(Common.SQLHelper.Asset_strConn))
            {
                conn.Open();
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                try
                {
                    DiscardSlip discardSlip = DiscardSlip.GetBy(paras.ID);

                    Asset asset = Asset.GetBy(discardSlip.AssetID);
                    if (asset == null) throw new Exception("资产不存在");
                    if (asset.Status != "报废") throw new Exception("资产未被报废");


                    asset.RemoveStatus("报废");
                    asset.RefreshPosition();

                    asset.sqlTransaction = discardSlip.sqlTransaction = sqlTransaction;
                    asset.Update();
                    discardSlip.Delete();
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
     
        public virtual bool UploadAssetFile(HttpPostedFile postedFile, int RequestID, string FileCategory, string AssetID)
        {
            byte[] fileContents = new byte[postedFile.ContentLength];
            postedFile.InputStream.Read(fileContents, 0, fileContents.Length);

            string SavedPath = "/AssetSystem/AssetFile/";

            if (FileCategory == "Certification") SavedPath += "Certification/";
            else if (FileCategory == "Manual") SavedPath += "Manual/";
            else if (FileCategory == "Image") SavedPath += "Image/";

            string SavedName = RequestID + "_" + postedFile.FileName;
            if (Common.FtpRepository.UploadFile(fileContents, SavedPath, SavedName) == true)
            {
                AssetFile file = new AssetFile();

                file.Date = DateTime.Now;
                file.FK_AssetID = AssetID;
                file.Category = FileCategory;
                file.Path = "http://192.168.9.3:8888" + SavedPath + SavedName;
                file.Add();
            }
            return true;
        }

        public virtual bool DeleteAssetFile(int AssetFile_ID)
        {
            AssetFile report = AssetFile.GetBy(AssetFile_ID);
            report.Delete();
            return true;
        }

        public virtual IEnumerable<AssetFile> GetAssetFilesOf(string AssetID, string FileCategory)
        {
            return AssetFile.GetFilesOf(AssetID, FileCategory);
        }

        public virtual IEnumerable<LendSlip> GetAllLendSlips()
        {
            return LendSlip.GetAll();
        }

        public virtual IEnumerable<LendSlip> GetLendSlipsOf(string AssetID)
        {
            return LendSlip.GetBy(AssetID);
        }

        public virtual IEnumerable<RepairSlip> GetRepairSlipsOf(string AssetID)
        {
            return RepairSlip.GetBy(AssetID);
        }

        

        public virtual IEnumerable<DiscardSlip> GetDiscardSlipsOf(string AssetID)
        {
            return DiscardSlip.GetBy(AssetID);
        }


        public static string GetApplicantName(string ApplicantID)
        {
            Personnel personnel = Personnel.GetBy(ApplicantID);
            string ApplicantName = personnel?.Name;
            return ApplicantName;
        }

        public static string Login(string UserID, string Password)
        {
            string token = null;
            if (User.IsValidAccount(UserID, Password))
                token = User.GetBase64TokenOf(UserID);
            return token;
        }
    }
}
