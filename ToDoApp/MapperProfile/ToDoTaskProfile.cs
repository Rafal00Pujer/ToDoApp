using AutoMapper;
using ToDoApp.Data.Entities;
using ToDoApp.Data.Models;

namespace ToDoApp.MapperProfile;

public class ToDoTaskProfile : Profile
{
    public ToDoTaskProfile()
    {
        CreateMap<ToDoTaskEntity, ToDoTaskModel>();
    }
}
