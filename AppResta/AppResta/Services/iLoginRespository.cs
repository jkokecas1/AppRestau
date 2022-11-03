using AppResta.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AppResta.Services
{
    public interface iLoginRespository
    {
        List<Empleado> Login(String pin);
    }
}
