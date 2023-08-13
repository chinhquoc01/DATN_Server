using Domain.DTOs;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IUserService : IBaseService<User>
    {
        Task<UserDTO> GetUserLogin(LoginParam loginParam);

        Task<bool> SignUp(SignupParam signupParam);

        Task<bool> RateUser(Guid userId, double ratePoint);

        Task<List<User>> GetSuggestFreelancer(string workField, string fieldTags);
    }
}
