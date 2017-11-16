using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace WarehouseSystem.Service.Misc
{
    public class Encrypt
    {
        public string EncryptSHA(string SourceMessage, EnumSHAType SHAType = EnumSHAType.SHA1)
        {
            if (string.IsNullOrEmpty(SourceMessage))
            {
                return string.Empty;
            }

            byte[] Message = Encoding.ASCII.GetBytes(SourceMessage);
            HashAlgorithm HashImplement = null;

            switch (SHAType)
            {
                case EnumSHAType.SHA1:
                    HashImplement = new SHA1Managed();
                    break;
                case EnumSHAType.SHA256:
                    HashImplement = new SHA256Managed();
                    break;
                case EnumSHAType.SHA384:
                    HashImplement = new SHA384Managed();
                    break;
                case EnumSHAType.SHA512:
                    HashImplement = new SHA512Managed();
                    break;

            }

            byte[] HashValue = HashImplement.ComputeHash(Message);

            return BitConverter.ToString(HashValue).Replace("-", "").ToLower();
        }

        public enum EnumSHAType
        {
            SHA1,
            SHA256,
            SHA384,
            SHA512
        }
    }
}