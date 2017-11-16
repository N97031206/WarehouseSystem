using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarehouseSystem.Service.Misc
{
    public interface IResult
    {
        string LastUpdateTime { get; set; }
        Guid ID { get; }
        bool Success { get; set; }
        string Message { get; set; }
        Exception Exception { get; set; }
        List<IResult> InnerResults { get; }

        bool IsUpLoad { get; set; }

        string ErrorMessage { get; set; }
    }
}
