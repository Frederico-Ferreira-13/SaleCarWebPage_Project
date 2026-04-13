using Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Services
{
    public interface IPasswordHasher
    {
        string GenerateSalt();
        Result<HashResult> HashPassword(string password, string salt);
        bool VerifyPassword(string storedHash, string passwordToVerify, string salt);
    }
}
