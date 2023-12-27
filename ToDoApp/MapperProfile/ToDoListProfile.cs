using AutoMapper;
using ToDoApp.Data.Entities;
using ToDoApp.Data.Models;

namespace ToDoApp.MapperProfile;

internal class ToDoListProfile : Profile
{
    public ToDoListProfile()
    {
        CreateMap<ToDoListEntity, ToDoListModel>();
    }
}
