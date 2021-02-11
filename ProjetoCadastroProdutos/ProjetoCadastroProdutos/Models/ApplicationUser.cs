using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjetoCadastroFuncionarios.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Cidade { get; set; }
       // public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }

    }
}
