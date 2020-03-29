using BrickBundle.Lib;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrickBundle.Model
{
    internal class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(a => a.ID);
            builder.HasIndex(a => a.Username).IsUnique();
            builder.HasIndex(a => a.EmailAddress).IsUnique();
            builder.Property<byte[]>("Password").HasField("password").IsRequired();
        }
    }

    public class User : IO
    {
        #region Constants
        public const int maxResetPasswordAttempts = 3;
        public const int maxResetPasswordAttemptsMinutes = 30;
        public const int maxResetPasswordCodesSent = 3;
        public const int maxResetPasswordCodesSentHours = 24;
        public const int emailVerificationCodeValidHours = 24;
        #endregion

        #region Properties
        public long ID { get; set; }
        public string Username { get; set; }
        public string EmailAddress { get; set; }
        private byte[] password;

        public int ResetPasswordAttempts { get; set; } = 0;
        public int ResetPasswordCodesSent { get; set; } = 0;
        public string ResetPasswordCode { get; set; }
        public DateTime? ResetPasswordCodeCreated { get; set; }

        public bool IsEmailVerified { get; set; } = false;
        public string EmailVerificationCode { get; set; }
        public DateTime? EmailVerificationCodeCreated { get; set; }

        public ICollection<UserPart> UserParts { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Hashes and saves user's password if it meets the password requirements.
        /// </summary>
        private void SetPassword(string password)
        {
            if (!Functions.IsValidPassword(password))
            {
                throw new InvalidUserException("invalid password");
            }
            this.password = Functions.HashPassword(password);
        }

        /// <summary>
        /// Returns whether specified password matches the user's password.
        /// </summary>
        private bool VerifyPassword(string password)
        {
            return Functions.VerifyHashedPassword(this.password, password);
        }

        /// <summary>
        /// Generates a new code for <see cref="EmailVerificationCode"/> which is valid for <see cref="emailVerificationCodeValidHours"/> hours.
        /// </summary>
        public async Task<bool> GenerateEmailVerificationCode()
        {
            if (!IsEmailVerified)
            {
                EmailVerificationCode = Functions.GenerateCode(6);
                EmailVerificationCodeCreated = DateTime.UtcNow;
                return await SaveChangesAsync();
            }
            return false;
        }

        /// <summary>
        /// Verifies the user's email if the code is correct and less than <see cref="emailVerificationCodeValidHours"/> hours old.
        /// </summary>
        public async Task<bool> VerifyEmail(string code)
        {
            if (!IsEmailVerified)
            {
                if (EmailVerificationCodeCreated.HasValue && EmailVerificationCodeCreated.Value > DateTime.UtcNow.AddHours(-emailVerificationCodeValidHours))
                {
                    if (code == EmailVerificationCode)
                    {
                        IsEmailVerified = true;
                        EmailVerificationCode = null;
                        EmailVerificationCodeCreated = null;
                        return await SaveChangesAsync();
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Generates a new code for <see cref="ResetPasswordCode"/> which is valid for <see cref="maxResetPasswordAttemptsMinutes"/> minutes.
        /// Returns false if <see cref="maxResetPasswordCodesSent"/> codes have been sent in the last <see cref="maxResetPasswordCodesSentHours"/> hours.
        /// </summary>
        public async Task<bool> GenerateResetPasswordCode()
        {
            if (ResetPasswordCodeCreated.HasValue && ResetPasswordCodeCreated.Value < DateTime.UtcNow.AddHours(-maxResetPasswordCodesSentHours))
            {
                ResetPasswordCodesSent = 0;
            }
            if (ResetPasswordCodesSent < maxResetPasswordCodesSent)
            {
                ResetPasswordAttempts = 0;
                ++ResetPasswordCodesSent;
                ResetPasswordCode = Functions.GenerateCode(6);
                ResetPasswordCodeCreated = DateTime.UtcNow;
                return await SaveChangesAsync();
            }
            return false;
        }

        /// <summary>
        /// Resets user's password if code is correct, less than <see cref="maxResetPasswordAttemptsMinutes"/> minutes old and has had fewer than <see cref="maxResetPasswordAttempts"/> attempts.
        /// </summary>
        public async Task<bool> ResetPassword(string code, string password)
        {
            if (!string.IsNullOrEmpty(ResetPasswordCode)
                && ResetPasswordCodeCreated.HasValue
                && ResetPasswordCodeCreated.Value > DateTime.UtcNow.AddMinutes(-maxResetPasswordAttemptsMinutes)
                && ResetPasswordAttempts < maxResetPasswordAttempts)
            {
                if (code == ResetPasswordCode)
                {
                    SetPassword(password);
                    return await SaveChangesAsync();
                }
            }
            ++ResetPasswordAttempts;
            await SaveChangesAsync();
            return false;
        }

        /// <summary>
        /// Updates user's parts.
        /// </summary>
        public async Task<bool> UpdateParts(DTO.UserPartDTO[] dtos)
        {
            await DbContext.Entry(this)
                .Collection(a => a.UserParts)
                .LoadAsync();

            foreach (var dto in dtos)
            {
                foreach (var colorQuantity in dto.ColorQuantities)
                {
                    var foundUserPart = UserParts.FirstOrDefault(a => a.PartID == dto.Part.ID && a.ColorID == colorQuantity.Color.ID);
                    if (foundUserPart != null)
                    {
                        foundUserPart.Quantity = colorQuantity.Quantity;
                    }
                    else
                    {
                        UserParts.Add(new UserPart()
                        {
                            UserID = this.ID,
                            PartID = dto.Part.ID,
                            ColorID = colorQuantity.Color.ID,
                            Quantity = colorQuantity.Quantity
                        });
                    }
                }
            }
            await SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Saves changes to user in database.
        /// </summary>
        internal override Task<bool> SaveChangesAsync()
        {
            string errorMessage = "";
            if (!Functions.IsValidUsername(Username))
            {
                errorMessage += "invalid username|";
            }
            if (!Functions.IsValidEmail(EmailAddress))
            {
                errorMessage += "invalid emailaddress|";
            }

            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new InvalidUserException(errorMessage);
            }
            var usernameExistsTask = DbContext.Users.AnyAsync(a => a.Username == Username && a.ID != ID);
            var emailAddressExistsTask = DbContext.Users.AnyAsync(a => a.EmailAddress == EmailAddress && a.ID != ID);
            Task.WaitAll(usernameExistsTask, emailAddressExistsTask);
            bool usernameAlreadyExists = usernameExistsTask.Result;
            bool emailAddressAlredyExists = emailAddressExistsTask.Result;
            if (usernameAlreadyExists)
            {
                errorMessage += "username already exists|";
            }
            if (emailAddressAlredyExists)
            {
                errorMessage += "emailaddress already exists|";
            }
            if (!string.IsNullOrWhiteSpace(errorMessage))
            {
                throw new UserAlreadyExistsException(errorMessage);
            }
            return base.SaveChangesAsync();
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Retuturns user from the database with matching ID.
        /// </summary>
        public static async Task<User> Find(long id)
        {
            return await IO.FindAsync<User>(id);
        }

        /// <summary>
        /// Returns user from the database with matching username or wmail address.
        /// </summary>
        public static async Task<User> FindByUsernameOrEmail(string usernameOrEmail)
        {
            var context = IO.CreateDbContext();
            var user = await context.Users.FirstOrDefaultAsync(a => a.Username == usernameOrEmail || a.EmailAddress == usernameOrEmail);
            if (user != null)
            {
                user.SetDbcontext(context);
            }
            return user;
        }

        /// <summary>
        /// Creates a new user in the database.
        /// </summary>
        public static async Task<User> Register(string username, string emailAddress, string password)
        {
            var newUser = await IO.CreateAsync<User>();
            newUser.Username = username;
            newUser.EmailAddress = emailAddress;
            newUser.SetPassword(password);
            if (await newUser.SaveChangesAsync())
            {
                return newUser;
            }
            return null;
        }

        /// <summary>
        /// Returns user with matching username or email address if the password is correct.
        /// </summary>
        public static async Task<User> Login(string usernameOrEmail, string password)
        {
            var user = await FindByUsernameOrEmail(usernameOrEmail);
            if (user == null)
            {
                throw new InvalidUsernameOrPasswordException();
            }
            if (!user.VerifyPassword(password))
            {
                throw new InvalidUsernameOrPasswordException();
            }
            return user;
        }

        /// <summary>
        /// Lists all the parts for the user.
        /// </summary>
        public static async Task<DTO.UserPartDTO[]> ListPartsForUser(long userID)
        {
            using (var context = IO.CreateDbContext())
            {
                return await context.UserParts
                    .Where(a => a.UserID == userID)
                    .Where(a => a.Quantity > 0)
                    .GroupBy(a => new { a.PartID, a.Part.Code, a.Part.Name, CategoryName = a.Part.Category.Name })
                    .Select(a => new DTO.UserPartDTO()
                    {
                        Part = new DTO.PartListItemDTO()
                        {
                            ID = a.Key.PartID,
                            Code = a.Key.Code,
                            Name = a.Key.Name,
                            Category = a.Key.CategoryName
                        },
                        ColorQuantities = a.Select(b => new DTO.ColorQuantityDTO()
                        {
                            Color = new DTO.ColorListItemDTO()
                            {
                                ID = b.ColorID,
                                Name = b.Color.Name
                            },
                            Quantity = b.Quantity
                        }).ToArray()
                    })
                    .ToArrayAsync();
            }
        }

        /// <summary>
        /// Lists the sets the user can build with the parts they own.
        /// </summary>
        public static async Task<DTO.SetListItemDTO[]> ListSetsUserCanBuild(long userID)
        {
            using (var context = IO.CreateDbContext())
            {
                return await context.Sets
                    .Where(s => s.Inventories
                        .Where(i => !i.InventorySets.Any())
                        .Where(i => i.InventoryParts.Any())
                        .Any(i => i.InventoryParts
                            .All(ip => ip.IsSpare || context.UserParts
                                .Where(up => up.UserID == userID)
                                .Where(up => up.PartID == ip.PartID)
                                // Uncomment this to match part colors
                                //.Where(up => up.ColorID == ip.ColorID)
                                .Any(up => up.Quantity >= ip.Quantity))))
                    .Select(s => new DTO.SetListItemDTO()
                    {
                        ID = s.ID,
                        SetNum = s.SetNum,
                        Name = s.Name,
                        Year = s.Year,
                        ThemeID = s.ThemeID,
                        ThemeName = s.Theme.Name,
                        NumParts = s.NumParts,
                        Percentage = 100f
                    })
                    .ToArrayAsync();
            }
        }
        #endregion

        #region Exceptions
        public class InvalidUsernameOrPasswordException : Exception { }

        public class InvalidUserException : Exception
        {
            public InvalidUserException(string message) : base(message) { }
        }

        public class UserAlreadyExistsException : Exception
        {
            public UserAlreadyExistsException(string message) : base(message) { }
        }
        #endregion
    }
}
