using ContainerShippingProgram.Controllers;
using ContainerShippingProgram.Views;
using System;
using System.Threading;

namespace ContainerShippingProgram
{
    class Program
    {
        /// <summary>
        /// Entry point of the program
        /// </summary>
        static void Main(string[] args)
        {

            ContainerController c = new ContainerController();

            MainView view = new MainView(c);

            while (true)
            {
                //TODO: Add a way to stop the main thread from closing

                // Sleep for 10 seconds for optimization, the main thread is not used yet
                //Thread.Sleep(10 * 1000);    //TODO: Remove/Reduce
            }
        }
    }
}
