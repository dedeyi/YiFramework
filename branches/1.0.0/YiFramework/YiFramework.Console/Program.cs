using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using YiFramework.Core;
using YiFramework.DBModel;
using YiFramework.Service;
using YiFramework.ViewModel.Sys;

namespace YiFramework.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            UserService userService = new UserService();
            //var res = userService.Add(new T_SYS_USER { DisplayName="44459",LoginName="44q5",LoginPass="1133" });
            //UserVM user = userService.GetByID("6c312bcaac4a4075aa1517622cb499b7");
            //ContextManager.Dispose();
            //System.Console.WriteLine(user.DisplayName);

            MapTest();
            System.Console.ReadKey();
        }

        static void MapTest() 
        {
            People p = new People { ID = 1, Name = "dedeyi", CreateTime = DateTime.Now };

            Mapper.CreateMap<People, PeopleVM>()
                .ForMember(dest=>dest.PID,opt=>opt.MapFrom(src=>src.ID));
            Mapper.CreateMap<PeopleVM,People>();

            PeopleVM pVM = Mapper.Map<People,PeopleVM>(p);

            //People p2 = Mapper.Map<PeopleVM, People>(pVM);
            System.Console.WriteLine(pVM.PID+"  "+pVM.CreateTime);
        }

    }

    class People 
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
    }

    class PeopleVM
    {
        public int PID { get; set; }
        public string Name { get; set; }
        public DateTime CreateTime { get; set; }
    }


}
