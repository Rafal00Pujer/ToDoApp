using AutoMapper;
using ToDoApp.Data.Entities;
using ToDoApp.Data.Models;

namespace ToDoApp.MapperProfile;

public class ToDoListProfile : Profile
{
    public ToDoListProfile()
    {
        CreateMap<ToDoListEntity, ToDoListModel>();
    }
}
