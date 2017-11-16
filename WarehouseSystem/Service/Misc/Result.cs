using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseSystem.Service.Misc
{
    public class Result : IResult
    {
        public string LastUpdateTime { get; set; }
        public Guid ID
        {
            get;
            private set;
        }

        public bool Success
        {
            get;
            set;
        }

        public bool IsUpLoad
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public Exception Exception
        {
            get;
            set;
        }

        public List<IResult> InnerResults
        {
            get;
            protected set;
        }

        public Result()
            : this(false)
        {

        }

        public Result(bool success)
        {
            ID = Guid.NewGuid();
            Success = success;
            InnerResults = new List<IResult>();
        }

        
    }
}