using ContainerShippingProgram.Controllers;
using ContainerShippingProgram.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContainerShippingProgram.Views
{
    class MainView
    {
        private IContainerController containerController = null;

        public MainView(IContainerController containerController)
        {
            this.containerController = containerController;

            ShowSetupInformation();
            SubscribeToEvents();
        }

        private void ShowSetupInformation()
        {
            Console.WriteLine("Info for testing and PuTTY set-up:\nIp: 127.0.0.1\nPort: 1234\nConnection type: Raw\nClose window on exit: Never");
        }

        private void SubscribeToEvents()
        {
            containerController.DisconnectionRequested += ContainerControllerDisconnectionRequestedEventHandler;
            containerController.MessageToPrintReceived += ContainerControllerMessageToPrintReceivedEventHandler;
        }

        private void ContainerControllerMessageToPrintReceivedEventHandler(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private async void ContainerControllerDisconnectionRequestedEventHandler(object sender, DisconnectEventArgs e)
        {
            //Note: try-catch block to avoid making the event handlers async but handle the exception if one is thrown
            //The event-raising code will not get the exception when async void is used so it has to be handled here
            try
            {
                await ShowContainersReport();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task ShowContainersReport()
        {
            string report = await containerController.GetFullReportAsync();

            Console.WriteLine(report);
        }
    }
}
