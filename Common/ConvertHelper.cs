using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class ConvertHelper
    {

        public static List<T> DataTableToList<T>(DataTable dt) where T : class, new()
        {
            if (dt == null || dt.Rows.Count == 0) return null;

            List<T> ts = new List<T>();
            string tempName = string.Empty;
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;
                    if (dt.Columns.Contains(tempName))
                    {
                        object value = dr[tempName];
                        if (value != DBNull.Value)
                        {
                            pi.SetValue(t, value, null);
                        }
                    }
                }
                ts.Add(t);
            }
            return ts;
        }


        public static DataTable ListToTable<T>(List<T> list)
        {
            Type type = typeof(T);
            PropertyInfo[] proInfo = type.GetProperties();
            DataTable dt = new DataTable();
            foreach (PropertyInfo p in proInfo)
            {
                //类型存在Nullable<Type>时，需要进行以下处理，否则异常
                Type t = p.PropertyType;
                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
                    t = t.GetGenericArguments()[0];
                dt.Columns.Add(p.Name, t);
            }
            foreach (T t in list)
            {
                DataRow dr = dt.NewRow();
                foreach (PropertyInfo p in proInfo)
                {
                    object obj = p.GetValue(t);
                    if (obj == null) continue;
                    if (p.PropertyType == typeof(DateTime) && Convert.ToDateTime(obj) < Convert.ToDateTime("1753-01-01"))
                        continue;
                    dr[p.Name] = obj;
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }


        public static string[] dtToArr1(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0) return new string[0];
            string[] sr = new string[dt.Rows.Count];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (Convert.IsDBNull(dt.Rows[i][0])) sr[i] = "";
                else sr[i] = dt.Rows[i][0] + "";
            }
            return sr;
        }


        public static object ConvertToSqlParameterValue<T>(T o)
        {
            if (typeof(T).Name.ToLower() == "string" && (o == null || o is DBNull))
                return "";
            else if (o == null || o is DBNull)
                return DBNull.Value;
            else
                return o;
        }

        public static DataTable GetDistinctTable(DataTable dtSource, bool IsContainBlankLine, string columnName)
        {
            DataTable distinctTable = null;

            if (dtSource != null && dtSource.Rows.Count > 0)
            {
                distinctTable = dtSource.Clone();
                DataView dv = new DataView(dtSource);
                distinctTable = dv.ToTable(true, columnName);
            }

            if (!IsContainBlankLine)
            {
                for (int i = distinctTable.Rows.Count - 1; i >= 0; i--)
                {
                    if (distinctTable.Rows[i][columnName] is DBNull || distinctTable.Rows[i][columnName] == null || distinctTable.Rows[i][columnName].ToString().Trim() == "")
                        distinctTable.Rows.RemoveAt(i);
                }
            }

            return distinctTable;
        }


        public static DataTable UnionDataTable(DataTable dt1, DataTable dt2)
        {
            if (dt1 == null && dt2 == null) return null;

            if (dt1 != null && dt2 == null) return dt1;

            if (dt1 == null && dt2 != null) return dt2;


            object[] obj = new object[dt2.Columns.Count];

            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                dt2.Rows[i].ItemArray.CopyTo(obj, 0);
                dt1.Rows.Add(obj);
            }

            return dt1;
        }

        public static void Mapper<A, B>(B b, ref A a, bool IsCopyNullValue)
        {
            Type Typeb = b.GetType();//获得类型  
            Type Typea = typeof(A);
            foreach (PropertyInfo sp in Typeb.GetProperties())//获得类型的属性字段  
            {
                foreach (PropertyInfo ap in Typea.GetProperties())
                {
                    if (ap.Name == sp.Name)//判断属性名是否相同  
                    {
                        object o = sp.GetValue(b);
                        if (!IsCopyNullValue)
                            if (o is DBNull || o is null) break;

                        ap.SetValue(a, sp.GetValue(b));//获得b对象属性的值复制给a对象的属性  
                        break;
                    }
                }
            }
        }
    }
}
