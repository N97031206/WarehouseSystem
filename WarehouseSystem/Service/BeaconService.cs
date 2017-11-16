using WarehouseSystem.Models.Interface;
using WarehouseSystem.Models.Repository;
using WarehouseSystem.Service.Interface;
using WarehouseSystem.Service.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace WarehouseSystem.Service
{
    public class BeaconService : IBeaconService
    {
        private IRepository<CST_BEACON_LOCATION> _repository;

        public BeaconService(IRepository<CST_BEACON_LOCATION> repository)
        {
            _repository = repository;
        }

        //public IResult Create(CST_BEACON_LOCATION instance)
        //{
        //    if (instance == null)
        //    {
        //        throw new ArgumentNullException();
        //    }

        //    IResult result = new Result(false);

        //    try
        //    {
        //        this._repository.Create(instance);
        //        result.Success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Exception = ex;

        //    }

        //    return result;

        //}

        public IResult Create(string FieldID, short Role, int X, int Y, ref string BeaconInstallLocationID)
        {
            if (string.IsNullOrEmpty(FieldID))
            {
                throw new ArgumentNullException();
            }

            IResult result = new Result(false);

            var NewBeaconLocation = new CST_BEACON_LOCATION();

            try
            {
                IdGenerator idg = new IdGenerator();
                BeaconInstallLocationID = idg.GetSID();

                string InsertTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

                NewBeaconLocation.FieldID = FieldID;
                NewBeaconLocation.BeaconInstallLocationID = BeaconInstallLocationID;
                NewBeaconLocation.BeaconName = "No name";
                NewBeaconLocation.IsDelete = 0;
                NewBeaconLocation.X = X.ToString();
                NewBeaconLocation.Y = Y.ToString();

                NewBeaconLocation.CreateTime = InsertTime;
                NewBeaconLocation.LastUpdateTime = InsertTime;

                _repository.Create(NewBeaconLocation);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        //public IResult Update(CST_BEACON_LOCATION instance)
        //{
        //    if (instance == null)
        //    {
        //        throw new ArgumentNullException();
        //    }

        //    IResult result = new Result(false);

        //    try
        //    {
        //        this._repository.Update(instance);
        //        result.Success = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        result.Exception = ex;

        //    }

        //    return result;
        //}

        public IResult Update(CST_BEACON_LOCATION instance, string PropertyName, object Value)
        {
            Dictionary<string, object> UpdateDic = new Dictionary<string, object>();

            if (instance == null)
            {
                throw new ArgumentNullException();
            }

            IResult result = new Result(false);

            //try
            //{
            //    bool IsPropertyExist = false;

            //    foreach(var prop in instance.GetType().GetProperties())
            //    {

            //        if(propertyName == prop.Name)
            //        {
            //            //找到欄位, 將旗標改為true
            //            IsPropertyExist = true;

            //            if (prop.PropertyType.Equals(typeof(string)))          //string
            //            {
            //                //string type 無須轉換

            //            }
            //            else if (prop.PropertyType.Equals(typeof(short)))          //short
            //            {
            //                short sValue;

            //                if(value==null)
            //                {
            //                    //介面未選擇Yes 會傳回null,所以寫入0(No)
            //                    sValue = 0;
            //                }
            //                else
            //                {
            //                    sValue = Convert.ToInt16(value);
            //                }

            //                value = sValue;

            //            }
            //            else if (prop.PropertyType.Equals(typeof(Int16)))          //short
            //            {
            //                short sValue;

            //                if (value == null)
            //                {
            //                    //介面未選擇Yes 會傳回null,所以寫入0(No)
            //                    sValue = 0;
            //                }
            //                else
            //                {
            //                    sValue = Convert.ToInt16(value);
            //                }

            //                value = sValue;
            //            }
            //            else if (prop.PropertyType.Equals(typeof(Int32)))          //int
            //            {
            //                int nValue;

            //                if (value == null)
            //                {
            //                    //介面未選擇Yes 會傳回null,所以寫入0(No)
            //                    nValue = 0;
            //                }
            //                else
            //                {
            //                    nValue = Convert.ToInt32(value);
            //                }

            //                value = nValue;
            //            }
            //            else if (prop.PropertyType.Equals(typeof(Int64)))          //long
            //            {
            //                long nValue;

            //                if (value == null)
            //                {
            //                    //介面未選擇Yes 會傳回null,所以寫入0(No)
            //                    nValue = 0;
            //                }
            //                else
            //                {
            //                    nValue = Convert.ToInt64(value);
            //                }

            //                value = nValue;
            //            }
            //            else if (prop.PropertyType.Equals(typeof(float)))          //float
            //            {
            //                float nValue;

            //                if (value == null)
            //                {
            //                    //介面未選擇Yes 會傳回null,所以寫入0(No)
            //                    nValue = 0;
            //                }
            //                else
            //                {
            //                    nValue = Convert.ToSingle(value);
            //                }

            //                value = nValue;
            //            }
            //            else if (prop.PropertyType.Equals(typeof(double)))          //double
            //            {
            //                double nValue;

            //                if (value == null)
            //                {
            //                    //介面未選擇Yes 會傳回null,所以寫入0(No)
            //                    nValue = 0;
            //                }
            //                else
            //                {
            //                    nValue = Convert.ToDouble(value);
            //                }

            //                value = nValue;
            //            }

            //            break;
            //        }
            //    }

            //    if (IsPropertyExist == false)    //表示無此屬性
            //    {
            //        result.ErrorMsg = "找無此欄位";
            //        return result;
            //    }


            //    //轉換目前時間準備寫入 LastUpdateTime
            //    DateTimeHelper timeHelper = new DateTimeHelper();
            //    double dt = timeHelper.DateTime2Timestamp(DateTime.Now);
            //    int updateTime = Convert.ToInt32(dt);


            //    UpdateDic.Add(propertyName, value);
            //    UpdateDic.Add("lastUpdateTime", updateTime);

            //    this._repository.Update(instance, UpdateDic);
            //    result.Success = true;


            //}
            //catch (Exception ex)
            //{
            //    result.Exception = ex;
            //}

            return result;
        }

        public IResult UpdateCoordinate(string FieldID, Dictionary<string, Dictionary<string, int>> BeaconCoordinateData)
        {
            if (string.IsNullOrEmpty(FieldID) || BeaconCoordinateData == null)
            {
                throw new ArgumentNullException();
            }

            IResult result = new Result(false);

            //try
            //{
            //    foreach (KeyValuePair<string, Dictionary<string, int>> item in BeaconCoordinateData)
            //    {
            //        var instance = this.GetByID(item.Key);

            //        if (instance == null)
            //        {
            //            result.ErrorMsg = "查無此接收器";
            //            return result;
            //        }

            //        var x = item.Value["x"] + item.Value["offsetX"];
            //        var y = item.Value["y"] + item.Value["offsetY"];

            //        instance.x = x;
            //        instance.y = y;

            //        //轉換目前時間準備寫入 LastUpdateTime
            //        DateTimeHelper timeHelper = new DateTimeHelper();
            //        double dt = timeHelper.DateTime2Timestamp(DateTime.Now);
            //        int updateTime = Convert.ToInt32(dt);

            //        instance.lastUpdateTime = updateTime;

            //        this._repository.Update(instance);

            //    }

            //    result.Success = true;

            //}
            //catch (Exception ex)
            //{

            //    result.Exception = ex;
            //}

            return result;

        }

        public IResult Delete(string BeaconID)
        {
            IResult result = new Result(false);

            if (!IsExists(BeaconID))
            {
                result.Message = "找不到接收器(beacon)資料";
            }

            try
            {
                var instance = GetByID(BeaconID);
                this._repository.Delete(instance);
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
            }

            return result;
        }

        public bool IsExists(string BeaconInstallLocationID)
        {
            return this._repository.GetAll().Any(x => x.BeaconInstallLocationID == BeaconInstallLocationID);
        }

        public CST_BEACON_LOCATION GetByID(string BeaconInstallLocationID)
        {
            return this._repository.Get(x => x.BeaconInstallLocationID == BeaconInstallLocationID);
        }

        public IEnumerable<CST_BEACON_LOCATION> GetByFieldID(string FieldID, int Role, int IsDelete)
        {
            return this._repository.GetAll().Where(x => x.FieldID == FieldID && x.IsDelete == IsDelete);
        }

        public IEnumerable<CST_BEACON_LOCATION> GetAll()
        {
            return this._repository.GetAll();
        }

        //private bool JsonValue2Int(object value, out int transValue)
        //{

        //    if (value == null)   //介面未選擇Yes 會傳回null,所以寫入0(No)
        //    {
        //        transValue = 0;
        //        return true;

        //    }
        //    else if (Int32.TryParse((string)value, out transValue))
        //    {
        //        return true;
        //    }
        //    else //轉換異常
        //    {
        //        return false;
        //    }
        //}

        //private bool JsonValue2Short(object value, out short transValue)
        //{

        //    if (value == null)   //介面未選擇Yes 會傳回null,所以寫入0(No)
        //    {
        //        transValue = 0;
        //        return true;
        //    }
        //    else if (Int16.TryParse((string)value, out transValue))
        //    {
        //        return true;
        //    }
        //    else //轉換異常寫入預設值
        //    {
        //        return false;
        //    }
        //}
    }
}