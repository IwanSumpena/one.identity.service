using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using src.Models;
using src.Models.Entities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace src
{
    public class DbInitializer
    {
		public static async Task Initialize(OneDbContext context, UserManager<UserOne> userManager,
			RoleManager<RoleOne> roleManager, IConfiguration configuration)
		{
			context.Database.EnsureCreated();

			// the idea is that we want to make sure that everytime app is run, 
			// there will always be a backend-admin-account belong to backend-admin-role 
			//
			await CreateBackendAdminRoleAndUser(userManager, roleManager, configuration);
		}

		private static async Task CreateBackendAdminRoleAndUser(UserManager<UserOne> userManager,
			RoleManager<RoleOne> roleManager, IConfiguration configuration)
		{
			// automatically create initial super admin role & associated user account
			string backendAdminRole = configuration["Initializer:BackendAdminRole"];

			string backendAdminUserName = configuration["Initializer:BackendAdminUserName"];
			string backendAdminPassword = configuration["Initializer:BackendAdminPassword"];

			UserOne initialAppUser;
			await CreateInitialAppRole(roleManager, backendAdminRole);

			initialAppUser = await CreateInitialUserAccount(userManager, backendAdminUserName, backendAdminPassword);
			await AddUserToRole(userManager, backendAdminRole, initialAppUser);
		}

		private static async Task CreateInitialAppRole(RoleManager<RoleOne> roleManager,
			string administratorRoleName)
		{
			IdentityResult identityOperationResult;

			var existingSuperAdminAppRole = await roleManager.FindByNameAsync(administratorRoleName);
			if (existingSuperAdminAppRole == null)
			{
				// no backend-admin role yet,
				// let us create one
				identityOperationResult = await roleManager.CreateAsync(new RoleOne(administratorRoleName));
				if (!identityOperationResult.Succeeded)
				{
					//string errMessage = $"Data peran untuk setup data `{administratorRoleName}` gagal dibuat.";
					//Log.ForContext("EventSource", "Sistem Altius").Error(errMessage);

					//throw new ApplicationException(errMessage);
				}
				else
				{
					//Log.ForContext("EventSource", "Sistem Altius").Information($"Data peran untuk setup data `{administratorRoleName}` sukses dibuat.");

					existingSuperAdminAppRole = await roleManager.FindByNameAsync(administratorRoleName);

					await roleManager.UpdateAsync(existingSuperAdminAppRole);
				}
			}

			// backend-admin app role is there
			// no role claims is necessary, since he is suppose to access only his dashboard page

			// remove possibly existing claims for this role
			var possibleExistingClaims = await roleManager.GetClaimsAsync(existingSuperAdminAppRole);
			if (possibleExistingClaims.Any())
			{
				foreach (var possibleExistingClaimRecord in possibleExistingClaims)
				{
					identityOperationResult = await roleManager
						.RemoveClaimAsync(existingSuperAdminAppRole, possibleExistingClaimRecord);
				}
			}
		}

		private static async Task<UserOne> CreateInitialUserAccount(UserManager<UserOne> userManager,
			string userEmail, string userPassword)
		{
			var existingInitialSetupUserAccount = await userManager.FindByEmailAsync(userEmail);
			if (existingInitialSetupUserAccount == null)
			{
				//Log.ForContext("EventSource", "Sistem Altius").Information($"Membuat data akun pengguna khusus untuk setup data dengan email `{userEmail}`.");

				var userAccountToBeCreated = new UserOne
				{
					Email = userEmail,
					UserName = userEmail
				};

				var identityOperationResult = await userManager.CreateAsync(userAccountToBeCreated, userPassword);
				if (identityOperationResult.Succeeded)
				{
					//Log.ForContext("EventSource", "Sistem Altius").Information($"Data pengguna khusus untuk setup data `{userEmail}` sukses dibuat.");

					existingInitialSetupUserAccount = await userManager.FindByEmailAsync(userEmail);
				}
				else
				{
					string errMessage = $"Data pengguna khusus untuk setup data `{userEmail}` gagal dibuat.";
					//Log.ForContext("EventSource", "Sistem Altius").Error(errMessage);

					var exception = new ApplicationException(errMessage);

					throw exception;
				}
			}

			return existingInitialSetupUserAccount;
		}

		private static async Task AddUserToRole(UserManager<UserOne> userManager,
			string administratorRole, UserOne initialSetupUserAccount)
		{
			var initialUserCurrentRoles = await userManager.GetRolesAsync(initialSetupUserAccount);
			var isInitialSetupUserAccoutnHaveAdministratorRole = initialUserCurrentRoles.Where(p => p == administratorRole);

			if (isInitialSetupUserAccoutnHaveAdministratorRole.Count() == 0)
			{
				var identityOperationResult = await userManager.AddToRoleAsync(initialSetupUserAccount, administratorRole);
				if (identityOperationResult.Succeeded)
				{
					//Log.ForContext("EventSource", "Sistem Altius").Information($"Sukses menambahkan data pengguna untuk setup data `{initialSetupUserAccount.Email}` ke dalam peran '{administratorRole}'.");
				}
				else
				{
					string errMessage = $"Gagal menambahkan data pengguna khusus untuk setup data `{initialSetupUserAccount.Email}` ke dalam peran '{administratorRole}'.";
					//Log.ForContext("EventSource", "Sistem Altius").Error(errMessage);

					var exception = new ApplicationException(errMessage);

					throw exception;
				}
			}
		}
	}
}
