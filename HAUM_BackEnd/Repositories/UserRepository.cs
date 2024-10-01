using AutoMapper;
using HAUM_BackEnd.Context;
using HAUM_BackEnd.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using MapperConfiguration = HAUM_BackEnd.Context.MapperConfiguration;

namespace HAUM_BackEnd.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private Mapper _mapper;
        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _mapper = MapperConfiguration.InitializeAutomapper();

        }

        private string PasswordHashing(string inputString)
        {
            var inputBytes = Encoding.UTF8.GetBytes(inputString);
            var inputHash = SHA256.HashData(inputBytes);
            return Convert.ToHexString(inputHash);
        }

        public async Task<IEnumerable<UserDTO>?> GetUsersAsync()
        {
            var users = await _dbContext.User.ToListAsync();
            if (!users.Any() == false) {
                return _mapper.Map<IEnumerable<UserDTO>>(users);
            }
            else
            {
                return null;
            }
        }

        public async Task<UserDTO?> GetUserByIdAsync(Guid userId)
        {
            var user = await _dbContext.User.FindAsync(userId);
            if(user != null) {
                return _mapper.Map<UserDTO>(user);
            } else
            {
                return null;
            }
        }

        public async Task<bool> AddUserAsync(UserDTO userDTO)
        {
            var user = _mapper.Map<User>(userDTO);
            if (user != null)
            {
                user.Id = Guid.NewGuid();
                user.Password = PasswordHashing(userDTO.Password);
                await _dbContext.User.AddAsync(user);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(UserDTO userDTO)
        {
            var user = _dbContext.User.Where(x => x.Email == userDTO.Email).FirstOrDefault();
            if (user != null)
            {
                user.Email = userDTO.Email;
                user.Username = userDTO.Username;
                if(userDTO.Password != null)
                {
                    user.Password = PasswordHashing(userDTO.Password);
                }
                _dbContext.User.Update(user);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _dbContext.User.FindAsync(userId);
            if (user != null)
            {
                _dbContext.User.Remove(user);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Guid> LoginAsync(UserDTO userDTO)
        {
            var users = await _dbContext.User.ToListAsync();
            foreach(var user in users)
            {
                if (userDTO.Email.Equals(user.Email))
                {
                    if (PasswordHashing(userDTO.Password).Equals(user.Password))
                    {
                        return user.Id;
                    }
                }
            }
            return Guid.Empty;
        }
    }

}
