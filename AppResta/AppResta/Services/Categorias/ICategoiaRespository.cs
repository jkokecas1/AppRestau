using System;
using System.Collections.Generic;
using System.Text;
using AppResta.Model;
using System.Threading.Tasks;

namespace AppResta.Services.Categorias
{
    public interface ICategoiaRespository
    {
        List<Model.Categorias> Categorias2();
        //Task<Model.Categorias> Categorias();
    }
}
