//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WarehouseSystem
{
    using System;
    using System.Collections.Generic;
    
    public partial class POS_Supplier
    {
        public string SupID { get; set; }
        public string SupCode { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public string EMail { get; set; }
        public Nullable<int> Active { get; set; }
        public Nullable<int> IsDelete { get; set; }
        public string CreateTime { get; set; }
        public string LastUpdateTime { get; set; }
        public string UserGroupID { get; set; }
    }
}
