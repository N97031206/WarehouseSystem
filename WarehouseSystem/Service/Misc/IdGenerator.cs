using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Service.Misc
{
    public class IdGenerator
    {
        private string GetUniqID()
        {
            var ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            double t = ts.TotalMilliseconds / 1000;

            int a = (int)Math.Floor(t);
            int b = (int)((t - Math.Floor(t)) * 1000000);

            return a.ToString("x8") + b.ToString("x5");
        }






        public string GetUserNewID()
        {


            string id = "user_" + this.GetUniqID().Replace(",", "_");

            //string idno = uniqid("user_", false);
            //string id2 = idno.Replace('-', '_');


            // TODO:編碼後加上日期
            //..
            //..

            return id;
        }

        public string GetSID()
        {
            return "A" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
        }

        public string GetSID(int Value)
        {
            System.Threading.Thread.Sleep(Value);
            return "A" + DateTime.Now.ToString("yyyyMMddHHmmssfffffff");
        }
    }
}