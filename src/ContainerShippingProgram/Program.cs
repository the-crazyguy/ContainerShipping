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

            ContainerController containerController = new ContainerController();
            MainView view = new MainView(containerController);

            while (true)
            {
                //Stops the main thread from closing

                // Sleep for 0.25 seconds for optimization
                Thread.Sleep(250);    //TODO: Remove/Reduce/Increase
            }
        }
    }
}
