using System;
using System.Data;
using System.Reflection;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace iclickpro.AccessCommon
{
    /// <summary>
    /// 实体类之间的转换
    /// </summary>
    /// <typeparam name="T">实体类的类别</typeparam>
    public class ConvertEntityClass<T>
    {
        /// <summary>
        /// 将DataSet转换为实体类
        /// </summary>
        /// <param name="dsDataSource">DataSet数据源</param>
        /// <returns>实体类</returns>
        public static T ConvertEntityClassByDataSet(DataSet dsDataSource)
        {
            var entityClassInstance = Activator.CreateInstance<T>();
            if (dsDataSource != null && dsDataSource.Tables.Count > 0 && dsDataSource.Tables[0].Rows.Count > 0)
            {
                var t = typeof(T);
                var propertys = t.GetProperties();
                foreach (PropertyInfo property in propertys)
                {
                    if (dsDataSource.Tables[0].Columns.Contains(property.Name))
                    {
                        property.SetValue(entityClassInstance,
                                          Convert.ChangeType(dsDataSource.Tables[0].Rows[0][property.Name].ToString(),
                                                             property.PropertyType), null);
                    }
                }
            }
            return entityClassInstance;
        }

        /// <summary>
        /// 将实体类转换为数据库参数
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns>数据库参数</returns>
        public static SqlParameter[] ConvertEntityClassToSqlParameter(T entity)
        {
            var t = typeof(T);
            var propertys = t.GetProperties();
            var param = new SqlParameter[propertys.Length];
            int i = 0;
            foreach (PropertyInfo property in propertys)
            {
                object objValue = property.GetValue(entity, null);
                param[i] = new SqlParameter("@" + property.Name, Convert.ChangeType(objValue, property.PropertyType));
                i++;
            }
            return param;
        }


        /// <summary>
        /// 将实体类转换为DataRow
        /// </summary>
        /// <param name="dt">原始表</param>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public static DataRow ConvertEntityClassToDataRow(DataTable dt, T entity)
        {
            var t = typeof(T);
            var propertys = t.GetProperties();

            DataRow dr = dt.NewRow();

            foreach (PropertyInfo property in propertys)
            {
                object objValue = property.GetValue(entity, null);
                dr[property.Name] = Convert.ChangeType(objValue, property.PropertyType);
            }
            return dr;
        }


        /// <summary>
        /// 将实体类转换为DataTable
        /// </summary>
        /// <param name="entity">实体类</param>
        /// <returns></returns>
        public static DataTable ConvertEntityClassToDataTable(T entity)
        {
            var t = typeof(T);
            var propertys = t.GetProperties();

            var dt = new DataTable();

            foreach (PropertyInfo property in propertys)
            {
                dt.Columns.Add(property.Name, property.PropertyType);
            }
            return dt;
        }

        /// <summary>
        /// 转化为DataTable
        /// </summary>
        /// <param name="entitys"></param>
        /// <returns></returns>
        public static DataTable ConvertEntityClassToDataTable(List<T> entitys)
        {
            if (entitys.Count > 0)
            {
                var dt = ConvertEntityClassToDataTable(entitys[0]);

                foreach (var entity in entitys)
                {
                    var dr = ConvertEntityClassToDataRow(dt, entity);
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            return null;
        }
    }
}
