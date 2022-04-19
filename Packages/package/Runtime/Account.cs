using System;
using VivoxUnity;

namespace Unity.Services.Vivox
{
    public class Account : AccountId
    {
        public Account(string displayname = null, string[] spokenLanguages = null) : 
            base
            (
                VivoxService.Instance.Issuer,
                VivoxServiceInternal.PlayerId ?? Guid.NewGuid().ToString(),
                VivoxService.Instance.Domain,
                displayname,
                spokenLanguages,
                VivoxServiceInternal.EnvironmentId
            ) 
        { 
        }
    }
}
