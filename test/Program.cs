using System;
using WebApplication1.Models;
using WebApplication1.Model;
namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
             ChartModel tt = new ChartModel();
            tt.DateTime =DateTime.Now;
            tt.Area = "Suspension";
            tt.QueryKind = 1;
        // var tt=   DapperService.SqlHelp.GetChart(tt);


            Console.WriteLine(DapperService.SqlHelp.GetChart(tt));
            Console.ReadKey();


        }
    }
}
