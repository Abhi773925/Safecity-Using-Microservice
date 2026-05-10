using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SafeCity.IAM.DTOs;
using SafeCity.IAM.Utility;
using SafeCity.IAMDB.Data;
using SafeCity.IAMDB.Entities;

namespace SafeCity.IAM.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SafeCityDbContext _context;
        public UserRepository(SafeCityDbContext context)
        {
            _context = context;
        }

        public async Task ChangePassword(ChangePasswordRequest request)
        {
            // Basic validation
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            //  Fetch user from database
            var userDetails = await _context.Users.FirstOrDefaultAsync(temp => temp.Email == request.Email);
            if (userDetails == null)
            {
                throw new ArgumentException("User Not Found");
            }

            var passwordHasher = new PasswordHasher<User>();
            PasswordVerificationResult verifyResult;

            try
            {
                // Try to verify assuming the password in DB is hashed 
                verifyResult = passwordHasher.VerifyHashedPassword(userDetails, userDetails.Password, request.ExistingPassword);
            }
            catch (FormatException)
            {
                if (userDetails.Password == request.ExistingPassword)
                {
                    verifyResult = PasswordVerificationResult.Success;
                }
                else
                {
                    throw new InvalidOperationException("Existing Password does not match.");
                }
            }

            // Final check for verification success
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                throw new InvalidOperationException("Existing Password does not match. Try Forget Password Instead.");
            }

            // Hash the new password before saving!
            userDetails.Password = passwordHasher.HashPassword(userDetails, request.NewPassword);

            _context.Users.Update(userDetails);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteUser(int UserId)
        {
            //check if the userid is negative
            if (UserId < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(UserId));
            }
            //check if the user exist or not
            var userDetails = await _context.Users.FindAsync(UserId);
            if (userDetails == null)
            {
                throw new Exception("User Details Not Found.");
            }
            else
            {
                _context.Users.Remove(userDetails);
                await _context.SaveChangesAsync();
            }
        }

        //Forgot Password Functionality
        public async Task ForgotPassword(ForgotPasswordRequest request)
        {
            //check if the request is null or not
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            //check User with the Email Exist or Not
            var userDetails = await _context.Users.FirstOrDefaultAsync(temp => temp.Email == request.Email);

            if (userDetails == null)
            {
                throw new Exception("User Not Found.");
            }
            else
            {
                //check if the newPassword and the Confirm New Password is matching and following all the valid security purposes.
                if (request.NewPassword == null || request.ConfirmNewPassword == null)
                {
                    throw new Exception(nameof(request));
                }
                else
                {
                    if (request.NewPassword != request.ConfirmNewPassword)
                    {
                        throw new Exception("Passoword and Confirm Password does not matched. Try Again!");
                    }
                    else
                    {
                        var passwordStrength = ValidationHelper.PasswordHelper(request.NewPassword);
                        if (passwordStrength != null)
                        {
                            throw new Exception($"Password strength: {passwordStrength}");
                        }
                        var hashedPassword = new PasswordHasher<User>().HashPassword(userDetails, request.NewPassword);

                        userDetails.Password = hashedPassword;
                        _context.Users.Update(userDetails);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<User> LoginUser(LoginRequest request)
        {
            // check if the request is null or not
            if (request == null)
            {
                throw new ArgumentNullException(ValidationHelper.RequestNull.ToString());
            }

            // validate the password and validation

            // check even if the user exist or not
            var checkUser = await _context.Users.FirstOrDefaultAsync(temp => temp.Email == request.Email);
            if (checkUser == null)
            {
                throw new Exception(string.Join("| ", "User does not exists"));
            }

            // if the user exists

            var verifyPassword = new PasswordHasher<User>()
        .VerifyHashedPassword(checkUser, checkUser.Password, request.Password);

            if (PasswordVerificationResult.Success != verifyPassword)
            {
                throw new Exception(string.Join("| ", "Incorrect Password Details"));
            }

            return checkUser;

        }

        public async Task<UserRegisterResponse> RegisterUser(UserRegisterRequest request)
        {
            // initial validation check
            if (request == null)
            {
                throw new ArgumentNullException(ValidationHelper.RequestNull);
            }


            //try saving data to the database

            var userDetails = request.ToUserRegisterRequest();

            //check if user exists with the same email

            var existingUser = await _context.Users.FirstOrDefaultAsync(temp => temp.Email == request.Email);

            if (existingUser != null)
            {
                throw new Exception("User Already Exists");
            }
            else
            {
                //Hash the password
                userDetails.Password = new PasswordHasher<User>().HashPassword(userDetails, request.Password);

                await _context.Users.AddAsync(userDetails);
                await _context.SaveChangesAsync();
                return UserRegisterResponseExtension.ToUserRegisterResponse(userDetails);
            }

        }

        public async Task<UserUpdateResponse> UpdateUser(UserUpdateRequest request, int UserId)
        {
            //check the request is null or not
            if (request == null)
            {
                throw new ArgumentNullException(ValidationHelper.RequestNull);
            }
            else
            {
                //entity to Model
                var userDetails = request.ToUserUpdateRequest();

                //Check if the user is exist or not
                var checkUserDetails = await _context.Users.FindAsync(UserId);

                if (checkUserDetails == null)
                {
                    throw new Exception("User Not Found");
                }
                else
                {
                    checkUserDetails.Name = request.Name;
                    checkUserDetails.RoleID = request.RoleID;
                    checkUserDetails.Email = request.Email;
                    checkUserDetails.Phone = request.Phone;
                    checkUserDetails.Status = request.Status;
                    checkUserDetails.Password = request.Password;

                    await _context.SaveChangesAsync();


                    //return back the ResponseDto

                    return UserUpdateResponseExtenstion.ToUserUpdateResponse(userDetails);
                }

            }

        }

        public async Task<User?> GetUserById(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }
    }
}
