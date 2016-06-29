using gMVVM.Web.Services.KeHoach.Interfaces.GhiNhanKeHoach;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace gMVVM.Web.Services.KeHoach.Implement
{
    public class ImplementInterface : IGhiNhan, ICM_Domain
    {
        public const string formatDate = "dd/MM/yyyy HH:mm:ss";

        #region IGhiNhan
        public string BaoCaoGhiNhan(string data)
        {
            return "0";
        }
        #endregion

        #region IDomain
        public CM_DOMAIN_InsResult InsertDomain(CM_DOMAIN data)
        {
            try
            {

                using (var dataContext = new AssetDataContext())
                {
                    CM_DOMAIN_InsResult result = dataContext.CM_DOMAIN_Ins(data.DOMAIN_CODE, data.DOMAIN_NAME, data.START_DATE == null ? "" : data.START_DATE.Value.ToString(formatDate), data.END_DATE == null ? "" : data.END_DATE.Value.ToString(formatDate), data.NOTES, data.RECORD_STATUS, data.MAKER_ID,
                         data.CREATE_DT == null ? "" : data.CREATE_DT.Value.ToString(formatDate), data.AUTH_STATUS, data.CHECKER_ID, data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate), data.EDIT_DATE == null ? "" : data.EDIT_DATE.Value.ToString(formatDate)).FirstOrDefault();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new CM_DOMAIN_InsResult() { Result = "-1", ErrorDesc = ex.Message, DOMAIN_ID = "" };
            }
        }

        public CM_DOMAIN_UpdResult UpdateDomain(CM_DOMAIN data)
        {
            try
            {

                using (var dataContext = new AssetDataContext())
                {
                    CM_DOMAIN_UpdResult result = dataContext.CM_DOMAIN_Upd(data.DOMAIN_ID, data.DOMAIN_CODE, data.DOMAIN_NAME, data.START_DATE == null ? "" : data.START_DATE.Value.ToString(formatDate), data.END_DATE == null ? "" : data.END_DATE.Value.ToString(formatDate), data.NOTES, data.RECORD_STATUS, data.MAKER_ID,
                         data.CREATE_DT == null ? "" : data.CREATE_DT.Value.ToString(formatDate), data.AUTH_STATUS, data.CHECKER_ID, data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate), data.EDIT_DATE == null ? "" : data.EDIT_DATE.Value.ToString(formatDate)).FirstOrDefault();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new CM_DOMAIN_UpdResult() { Result = "-1", ErrorDesc = ex.Message, DOMAIN_ID = "" };
            }
        }
        #endregion

        #region [Public Functions]

        public int GetDayOfYear(int year)
        {
            return (new DateTime(year + 1, 1, 1) - new DateTime(year, 1, 1)).Days;
        }

        public string SetAutoId(string id, int maxlength)
        {
            while (id.Length < maxlength)
            {
                id = "0" + id;
            }
            return id;
        }

        #endregion
        /// <summary>
        /// Goi Store thong bang parameter
        /// </summary>
        /// <typeparam name="T">Doi tuong store tra ve</typeparam>
        /// <param name="parameters">Doi so truyen vao store</param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteStoredProcedure<T>(List<gParam> parameters)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    Type genericType = typeof(T);

                    string commandsthing = genericType.Name.Replace("Result", " ");
                    int count = parameters.Count();
                    string[] param = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        if (i > 0) commandsthing += ", ";
                        commandsthing += parameters[i].Name + "={" + i.ToString() + "} ";
                        param[i] = parameters[i].Value == null ? "" : parameters[i].Value;
                    }

                    return dataContext.ExecuteQuery<T>(commandsthing, param).ToList<T>();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


    }

    public class gParam
    {
        public gParam(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
        public string Name { get; set; }
        public string Value { get; set; }
    }

}