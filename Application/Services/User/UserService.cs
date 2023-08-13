using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : BaseService<User>, IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;
        public UserService(IUserRepo userRepository, IMapper mapper) : base(userRepository)
        {
            _userRepo = userRepository;
            _mapper = mapper;
        }

        public async Task<List<User>> GetSuggestFreelancer(string workField, string fieldTags)
        {
            var listField = JsonConvert.DeserializeObject<List<string>>(fieldTags);
            var listUser = await _userRepo.GetSuggestFreelancer(workField, listField);
            return listUser;
        }

        public async Task<UserDTO> GetUserLogin(LoginParam loginParam)
        {
            var user = await _userRepo.GetUserLogin(loginParam);
            if (user != null)
            {
                bool verified = BCrypt.Net.BCrypt.Verify(loginParam.Password, user.Password);
                if (verified)
                {
                    var userDto = _mapper.Map<User, UserDTO>(user);
                    return userDto;
                } else
                {
                    return null;
                }
            }
            return null;
        }

        public async Task<bool> RateUser(Guid userId, double ratePoint)
        {
            var user = await _userRepo.GetById(userId);
            var rateCount = user.RatedCount;
            var currentRatedPoint = user.Rating;
            if (currentRatedPoint == null) currentRatedPoint = 0;
            if (rateCount == null) rateCount = 0;
            double rate = (currentRatedPoint * rateCount + ratePoint) / (rateCount + 1);
            rateCount += 1;
            var res = await _userRepo.RateUser(userId, rate, rateCount);
            return res;
        }

        public async Task<bool> SignUp(SignupParam signupParam)
        {
            var valid = await _userRepo.CheckValidUserSignUp(signupParam);
            if (!valid)
            {
                throw new ValidateException("Email hoặc số điện thoại đã được sử dụng.");
            };

            var userEntity = _mapper.Map<SignupParam, User>(signupParam);
            if (userEntity.UserType == UserType.Freelancer)
            {
                userEntity.Rating = 4.5;
            }
            userEntity.Id = Guid.NewGuid();
            userEntity.Password = BCrypt.Net.BCrypt.HashPassword(signupParam.Password);
            if (userEntity.UserType == UserType.Client)
            {
                userEntity.Description = String.Empty;
                userEntity.JobTitle = String.Empty;
                userEntity.Skills = String.Empty;
            }
            var res = await Insert(userEntity);
            if (res > 0) 
            {
                return true;
            }
            return false;
        }
    }
}
