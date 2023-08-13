using Domain.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Repositories
{
    public interface IUserRepo : IBaseRepository<User>
    {
        Task<User> GetUserLogin(LoginParam loginParam);
        Task<bool> CheckValidUserSignUp(SignupParam signupParam);
        Task<bool> RateUser(Guid userId, double ratePoint, int rateCount);
        Task<List<User>> GetSuggestFreelancer(string workField, List<string>? listField);
    }
}
