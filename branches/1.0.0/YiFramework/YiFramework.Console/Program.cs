using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YiFramework.Core;
using YiFramework.DBModel;
using YiFramework.Service;

namespace YiFramework.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            UserService userService = new UserService();
            var res = userService.Add(new T_SYS_USER { DisplayName="44459",LoginName="44q5",LoginPass="1133" });
            ContextManager.Dispose();
            System.Console.WriteLine(res.success);
            System.Console.ReadKey();
        }
    }
}
