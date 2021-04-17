
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Linq;

namespace src.Utilities
{
    public static class Helpers
    {
        public static string GetModelStateError(ModelStateDictionary modelState)
        {
            var error = "";
            foreach (var values in modelState.Values)
            {
                foreach (var item in values.Errors)
                {
                    
                    if (error=="")
                    {
                        error = error + item.ErrorMessage;
                    }
                    else
                    {
                        error = error + ", "+ item.ErrorMessage;
                    }
                }
            }

            return error;
        }

        public static string GetIdentityResultError(IdentityResult identityResult)
        {
            return String.Join(" ", identityResult.Errors.Select(p => p.Description).ToArray());
        }
    }
}
