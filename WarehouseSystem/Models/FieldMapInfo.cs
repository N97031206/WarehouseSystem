using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Models
{
    public class FieldMapInfo
    {
        public CST_FIELD_MAP FieldData = new CST_FIELD_MAP();

        public List<CST_STORAGE_PARKINGBLOCK> ParkingBlockData = new List<CST_STORAGE_PARKINGBLOCK>();
    }
}