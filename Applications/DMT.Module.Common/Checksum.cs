using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DMT.Module.Common
{
    public class Checksum
    {
        byte[] data;

        public Checksum(byte[] data)
        {
            this.data = data;
        }

        public string Calculate()
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(data);
                return string.Join(string.Empty, hash.Select(h => h.ToString("X2")));
            }
        }

        public bool Check(string checksum)
        {
            if (checksum == null)
                throw new ArgumentNullException("checksum");

            return this.Calculate() == checksum;
        }

        public static string Calculate(byte[] data)
        {
            return new Checksum(data).Calculate();
        }

        public static bool Check(string checksum, byte[] data)
        {
            return new Checksum(data).Check(checksum);
        }       
    }
}
