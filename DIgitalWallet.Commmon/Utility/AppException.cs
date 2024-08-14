using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIgitalWallet.Commmon.Utility
{
    public class AppException:Exception
    {
        public int Code { get; set; }
        public string? CustomMessage { get; set; }
        public object[] MessageParams { get; set; } // اضافه کردن این ویژگی
        public object? AdditionalData { get; set; }

        public AppException(int code, string? customMessage, object[] messageParams, object? additionalData = null)
            : base(customMessage)
        {
            Code = code;
            CustomMessage = customMessage;
            MessageParams = messageParams;
            AdditionalData = additionalData;
        }
    }
}
