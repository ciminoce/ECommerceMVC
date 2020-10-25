using AutoMapper;
using ECommerceMVC.Web.Models;
using ECommerceMVC.Web.ViewModels.Categoria;
using ECommerceMVC.Web.ViewModels.Ciudad;
using ECommerceMVC.Web.ViewModels.Cliente;
using ECommerceMVC.Web.ViewModels.Pais;
using ECommerceMVC.Web.ViewModels.Producto;

namespace ECommerceMVC.Web
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Pais, PaisEditViewModel>();
            CreateMap<Pais, PaisListViewModel>();
            CreateMap<PaisEditViewModel,Pais >();


            CreateMap<Ciudad, CiudadListViewModel>()
                .ForMember(dest=>dest.NombrePais,c=>c.MapFrom(s=>s.Pais.NombrePais));
            CreateMap<Ciudad, CiudadEditViewModel>();
            CreateMap<CiudadEditViewModel, Ciudad>();

            CreateMap<Categoria, CategoriaEditViewModel>();
            CreateMap<Categoria, CategoriaListViewModel>();
            CreateMap<CategoriaEditViewModel, Categoria>();

            CreateMap<Cliente, ClienteListViewModel>()
                .ForMember(dest => dest.Pais, c => c.MapFrom(s => s.Pais.NombrePais))
                .ForMember(dest => dest.Ciudad, c => c.MapFrom(s => s.Ciudad.NombreCiudad));
            CreateMap<Cliente, ClienteEditViewModel>();
            CreateMap<ClienteEditViewModel,Cliente>();

            CreateMap<Producto, ProductoListViewModel>()
                .ForMember(dest => dest.Categoria, c => c.MapFrom(s => s.Categoria.NombreCategoria));
            CreateMap<Producto, ProductoEditViewModel>();
            CreateMap<ProductoEditViewModel,Producto>();


        }

    }
}