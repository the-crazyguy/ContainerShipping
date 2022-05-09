using ContainerShippingProgram.Controllers;
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
            Console.WriteLine("Info for testing and PuTTY set-up:\nIp: 127.0.0.1\nPort: 1234\nConnection type: Raw\nClose window on exit: Never");

            //TODO: Create a view object for the "UI"
            //TODO: Event-driven - cw messages received from the server (have event handlers in the view)

            // TODO: Pass to the view
            // Create the controller to start the application
            ContainerController c = new ContainerController();
            

            while (true)
            {
                //TODO: Add a way to stop the main thread from closing

                // Sleep for 10 seconds for optimization, the main thread is not used yet
                Thread.Sleep(10 * 1000);    //TODO: Remove/Reduce
            }
        }
    }
}
