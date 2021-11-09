using AutoMapper;


namespace mysqlapi.Models
{
    public class StudentDetailMappings : Profile
    {
        public StudentDetailMappings()
        {
            CreateMap<StudentDetail,StudentDetailDto>().ReverseMap();
        }
    }
}
