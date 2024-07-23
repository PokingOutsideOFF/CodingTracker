// See https://aka.ms/new-console-template for more information
using System.Configuration;
using CodingSessionLibrary;
using Spectre.Console;

namespace CodingTracker
{
    class Program
    {
        public static void Main()
        {
           var controller = new CodingController();
            controller.Menu();
        }
    }
}
