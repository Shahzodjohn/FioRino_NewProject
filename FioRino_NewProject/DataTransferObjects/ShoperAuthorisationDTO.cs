using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FioRino_NewProject.DataTransferObjects
{
    public class ShoperAuthorisationDTO
    {
        public string access_token { get; set; }
        //public DateTime expires_in { get; set; }
        public string token_type { get; set; }

    }
}
