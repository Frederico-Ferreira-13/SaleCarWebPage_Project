using Core.Common;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts.Services
{
    public interface ITokenService
    {
        TokenResponse GenerateToken(Users user);
        Task<Result<int>> GetUserIdFromContextAsync();
        Task InvalidateTokenAsync();
    }
}
