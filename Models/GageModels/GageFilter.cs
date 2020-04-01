using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Models.GageModels
{
    public class GageFilter
    {
        public string [] NextAdjustDate { get; set; }//下次校准日期
        public string [] Statuses { get; set; }//状态
        public string []  StandardAdjustType { get; set; }//标准校准类型

        public static GageFilter GetFilterValue()
        {
            GageFilter FilterValue = new GageFilter();

            //Statuses
            //string sql = "select Status from GAGE t left join Asset a on t.FK_AssetID = a.AssetID where 1=1 ";
            //DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.Asset_strConn, sql, null);
            //DataTable tb_status = TableHelper.GetDistinctTable(dt, false, "Status");
            //FilterValue.Statuses = ConvertHelper.dtToArr1(tb_status);


            return FilterValue;
        }

    }
} 