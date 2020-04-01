using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Models.UniversalModels
{
    public class Personnel
    {
        public string Name { get; set; }
        public string Department { get; set; }

        public static Personnel GetBy(string OALoginID)
        {
            string sql = "select hd.departmentname as Department,lastname as Name from [dbo].[HrmResource] left join HrmDepartment hd on departmentid = hd.id  where loginid = '" + OALoginID + "'";

            DataTable dt = Common.SQLHelper.ExecuteQueryToDataTable(Common.SQLHelper.OA_strConn, sql);

            if (dt == null)
                return null;

            Personnel Personnel = Common.ConvertHelper.DataTableToList<Personnel>(dt).First();
            return Personnel;
        }
    }
}