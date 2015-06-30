using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistanceMap.Samples
{
    class Program
    {
        static void Main(string[] args)
        {


            var command = string.Empty;

            while (command != "end" && command != "exit")
            {
                if (command == "uow" || command == "unitofwork")
                {
                    var sample = new UnitOfWorkSample.Sample();
                    sample.Work();
                }
                else if (command == "context")
                {
                    var sample = new ContextSample.Sample();
                    sample.Work();
                }
                else
                {
                    Console.WriteLine("Possible commands:");
                    Console.WriteLine("\tuow\t\tUnitOfWork test");
                    Console.WriteLine("\tUnitOfWork\tUnitOfWork test");
                    Console.WriteLine("\tcontext\t\tContext test");
                    Console.WriteLine("\tend\t\texit programm");
                    Console.WriteLine("\texit\t\texit programm");
                }


                command = Console.ReadLine();
            }
        }
    }
}
