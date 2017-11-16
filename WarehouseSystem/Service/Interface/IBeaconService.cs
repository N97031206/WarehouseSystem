using WarehouseSystem.Service.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseSystem.Service.Interface
{
    public interface IBeaconService
    {
       // IResult Create(CST_BEACON_LOCATION instance);
        IResult Create(string FieldID, short Role, int X, int Y, ref string BeaconID);
       // IResult Update(CST_BEACON_LOCATION instance);
        IResult Update(CST_BEACON_LOCATION instance, string PropertyName, object Value);
        IResult UpdateCoordinate(string FieldID, Dictionary<string, Dictionary<string, int>> BeaconCoordinateData);
        IResult Delete(string BeaconID);
        bool IsExists(string BeaconID);
        CST_BEACON_LOCATION GetByID(string BeaconID);
        IEnumerable<CST_BEACON_LOCATION> GetByFieldID(string BeaconID, int Role, int IsDelete);
        IEnumerable<CST_BEACON_LOCATION> GetAll();
    }
}