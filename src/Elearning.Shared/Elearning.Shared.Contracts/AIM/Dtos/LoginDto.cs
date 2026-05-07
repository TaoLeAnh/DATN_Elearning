using System;
using System.Collections.Generic;
using System.Text;

namespace Elearning.Shared.Contracts.AIM.Dtos
{
    public class LoginDto
    {
#pragma warning disable CA1707
#pragma warning disable CS8618
#pragma warning disable IDE1006 
        public string access_token { get; set; }


        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; } = "Bearer";



#pragma warning restore IDE1006 
#pragma warning restore CS8618
#pragma warning restore CA1707


    }
}
