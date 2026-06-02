
using LMS.Data.Entities;
using LMS.Data.Services;
using LMS.Data.Security;
using LMS.Data.Repositories;
using Microsoft.Extensions.Logging;
using System.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace LMS.Data.Services
{
    public class UserServiceDb : IUserService
    {
        private readonly DatabaseContext db;
      
        public UserServiceDb(DatabaseContext db) 
        {
            this.db = db; 
        }

        public void Initialise()
        {
           db.Initialise(); 
        }

        // ------------------ User Related Operations ------------------------

        // retrieve list of Users
        public Task<List<User>> GetUsersListByIdAsync()
        {
            return db.Users.ToListAsync();
        }

        // retrieve paged list of users
        public async Task<Paged<User>> GetUsersPagedAsync(int page = 1, int size = 10, string orderBy = "uid", string direction = "asc")
        {          
            var query = (orderBy.ToLower(),direction.ToLower()) switch
            {
                ("id","asc")     => db.Users.OrderBy(r => r.UId),
                ("id","desc")    => db.Users.OrderByDescending(r => r.UId),
                ("forename","asc")   => db.Users.OrderBy(r => r.Forename),
                ("forename","desc")  => db.Users.OrderByDescending(r => r.Forename),
                ("surname","asc") => db.Users.OrderBy(r => r.Surname),
                ("surname","desc") => db.Users.OrderByDescending(r => r.Surname),
                ("role","asc")  => db.Users.OrderBy(r => r.Role),
                ("role","desc") => db.Users.OrderByDescending(r => r.Role),
                _                => db.Users.OrderBy(r => r.UId)
            };

            return await query.ToPaged(page,size,orderBy,direction);
        }

        // Retrive User by Id 
        public Task<User?> GetUserByIdAsync(int id)
        {
            return db.Users.FirstOrDefaultAsync(s => s.UId == id);
        }

        // Find a user with specified email address
        public Task<User?> GetUserByEmailAsync(string email)
        {
            return db.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        // Add a new User checking a User with same email does not exist
        public async Task<User?> AddUserAsync(string forename, string surname, string password, Role role, DateOnly dateOfRegistration, string email, string address, string gender, DateTime dob, string contactNumber)
        {     
            var existing = await GetUserByEmailAsync(email);
            if (existing != null)
            {
                return null;
            } 

            var user = new User
            {            
                Forename = forename,
                Surname = surname,
                Password = Hasher.CalculateHash(password), // can hash if required
                Role = role,
                DateOfRegistration = dateOfRegistration,
                Email = email,
                Address = address,
                Gender = gender,
                DoB = dob,
                ContactNumber = contactNumber            
            };
            db.Users.Add(user);
            await db.SaveChangesAsync();
            return user; // return newly added User
        }

        // Delete the User identified by Id returning true if deleted and false if not found
        public async Task<bool> DeleteUserByIdAsync(int id)
        {
            var s = await GetUserByIdAsync(id);
            if (s == null)
            {
                return false;
            }
            db.Users.Remove(s);
            await db.SaveChangesAsync();
            return true;
        }

        // Verify if email is available or registered to specified user
        public async Task<bool> IsEmailAvailableAsync(string email, int userId)
        {
            return !await db.Users.AnyAsync(u => u.Email == email && u.UId != userId);  // changed to AnyAsync() from FirstOrDefaultAsync()
                                                                                        // as it is more efficient and loading of the user
                                                                                        // if found is not needed. Existence check only!
        }

        // Update the User with the details in updated 
        public async Task<User?> UpdateUserByIdAsync(User updated)
        {
            // verify the User exists
            var User = await GetUserByIdAsync(updated.UId);
            if (User == null)
            {
                return null;
            }
            // verify email address is registered or available to this user
            if (!await IsEmailAvailableAsync(updated.Email, updated.UId))
            {
                return null;
            }
            // update the details of the User retrieved and save
            User.Forename = updated.Forename;
            User.Surname = updated.Surname;
            User.Password = Hasher.CalculateHash(updated.Password);
            User.Role = updated.Role;
            User.DateOfRegistration = updated.DateOfRegistration;
            User.Email = updated.Email;
            User.Address = updated.Address;
            User.Gender = updated.Gender;
            User.DoB = updated.DoB;
            User.ContactNumber = updated.ContactNumber;

            await db.SaveChangesAsync();          
            return User;
        }


        public List<User> GetUsersQuery(Func<User,bool> q)
        {
            return db.Users.Where(q).ToList();
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            // retrieve the user based on the EmailAddress (assumes EmailAddress is unique)
            var user = await GetUserByEmailAsync(email);

            // Verify the user exists and Hashed User password matches the password provided
            return (user != null && Hasher.ValidateHash(user.Password, password)) ? user : null;  // dont need await here as this is populated from the User info already pulled from DB in GetUserByEmail
            //return (user != null && user.Password == password ) ? user: null;
        }

        public async Task<string?> ForgotPasswordAsync(string email)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null) {
                // invalidate any previous tokens
                db.ForgotPasswords
                    .Where(t => t.Email == email && t.ExpiresAt > DateTime.Now).ToList()
                    .ForEach(t => t.ExpiresAt = DateTime.Now);
                var f = new ForgotPassword { Email = email };
                db.ForgotPasswords.Add(f);
                await db.SaveChangesAsync();
                return f.Token;
            }
            return null;
        }
        
        public async Task<User?> ResetPasswordAsync(string email, string token, string password)
        {
            // find user by email
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null) 
            {
                return null; // user not found
            }
            // find valid reset token for user
            var reset = db.ForgotPasswords
                           .FirstOrDefault(t => t.Email == email && t.Token == token && t.ExpiresAt > DateTime.Now);
            if (reset == null) 
            {
                return null; // reset token invalid
            }

            // valid token and user so update password, invalidate the token and return the user           
            reset.ExpiresAt = DateTime.Now;
            user.Password = Hasher.CalculateHash(password);
            await db.SaveChangesAsync();
            return user;
        }

        public async Task<IList<string>> GetValidPasswordResetTokensAsync() {
            // return non expired tokens
            return await db.ForgotPasswords.Where(t => t.ExpiresAt > DateTime.Now)
                                      .Select(t => t.Token)
                                      .ToListAsync();
        }
   
    }
}