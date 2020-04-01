using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using Models.GageModels;
using Models.UniversalModels;
using Service.UniversalService;
using System.IO;
using ClosedXML.Excel;
using System.Data;

namespace Service.GageService
{
    public class GageServiceImpl : UniversalServiceBase
    {
        public IEnumerable<Gage> SelectBy(GageFilter Filter)
        {
            List<Gage> result1 = Gage.CombineSelectByStatuses(Filter.Statuses);
            List<Gage> result2 = Gage.RangeSelectByNextAdjustDate(Filter.NextAdjustDate);
            List<Gage> result3=  Gage.CombineSelectByStandardAdjustType(Filter.StandardAdjustType);

            if (result1 == null || result2 == null || result3 == null)
                return null;

           
            result1 = result1.Where(a => result2.Exists(t => a.AssetID == t.AssetID)).ToList();
            result1 = result1.Where(a => result3.Exists(t => a.AssetID == t.AssetID)).ToList();

            IEnumerable<Gage> final_result = result1;
            return final_result.Count() == 0 ? null : final_result;
        }

        public  GageFilter GetFilterValue()
        {
            GageFilter FilterValue = GageFilter.GetFilterValue();
            return FilterValue;
        }

        public IEnumerable<Gage> GetBy(string AssetID)
        {
            return Gage.GetBy(AssetID);
        }

        public  bool Add(Gage gage)
        {
            using (SqlConnection conn = new SqlConnection(Common.SQLHelper.Asset_strConn))
            {
                conn.Open();
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                try
                {
                    if (Asset.GetBy(gage.AssetID) != null) throw new Exception("资产编号已存在");

                    Asset asset = new Asset();
                    asset.AssetID = gage.AssetID.Trim();
                    asset.AssetName = gage.AssetName;
                    asset.AssetCategory = "Gage";
                    asset.CurrentWhereOrganization = gage.CurrentWhereOrganization;
                    asset.Status = "在库";
                    asset.Remark = gage.Remark;
                    asset.StorageDate = DateTime.Now;
                    asset.CurrentWhereSpot = gage.CurrentWhereSpot;
                    asset.Department = gage.Department;

                    gage.FK_AssetID = asset.AssetID;

                    //if (gageDetail.StandardAdjustType == "外校" || gageDetail.StandardAdjustType == "内校")
                    //    gageDetail.NextAdjustDate = ((DateTime)gageDetail.AdjustBaseDate).AddDays(Convert.ToDouble(gageDetail.AdjustPeriod));

                    asset.sqlTransaction = gage.sqlTransaction = sqlTransaction;
                    asset.Add();
                    gage.Add();

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

        public bool Update(Gage paras)
        {
            using (SqlConnection conn = new SqlConnection(Common.SQLHelper.Asset_strConn))
            {
                conn.Open();
                SqlTransaction sqlTransaction = conn.BeginTransaction();
                try
                {
                    Asset gage = Asset.GetBy(paras.AssetID);
                    if (gage == null) throw new Exception("资产不存在");
                    paras.CopyTo(gage, false);


                    Gage gageDetail = Gage.GetBy(gage.AssetID).First();

                    ////填充前的检测
                    //if(!gageDetail.Status.Contains(paras.StandardAdjustType))
                    //    throw new Exception("");

                    //使用新数据填充
                    paras.CopyTo(gageDetail, false);


                    if (gageDetail.StandardAdjustType == "免校")
                    {
                        gageDetail.AdjustPeriod = null;
                        gageDetail.NextAdjustDate = null;
                        gageDetail.AdjustNoticeDays = null;
                        gageDetail.DefaultAdjustRequirement = null;
                    }


                    gageDetail.sqlTransaction = gage.sqlTransaction = sqlTransaction;
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

        


        public string ExportGageLedgerToExcel(GageFilter Filter)
        {
            IEnumerable<Gage> Gages = SelectBy(Filter);

            if (Gages != null)
            {
                List<Gage> asList = Gages.ToList();
                Common.ConvertHelper.ListToTable(asList);

                XLWorkbook wb = new XLWorkbook();
                DataTable dt = Common.ConvertHelper.ListToTable(asList);
                wb.Worksheets.Add(dt, "量具信息");

                string CurDir = AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\";

                string excel_name = "gage_ledger_" + new Random().Next() + ".xlsx";
                string filePath = CurDir + excel_name;
                wb.SaveAs(filePath);

                return excel_name;
            }

            return null;
        }
    }
}