using System;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.ServiceBus;

namespace queue_worker
{
    class Program
    {
	private static QueueClient queueClient;
	private const string SERVICE_BUS = "__SERVICE_BUS_CONNECTION_STRING__";
	private const string QUEUE_NAME = "__QUEUE_NAME__";

        static void Main(string[] args)
        {
		MainAsync(args).GetAwaiter().GetResult();
        }

	private static async Task OnMessageReceived(Message message, CancellationToken token){
		Console.WriteLine($"Receiveved message: {Encoding.UTF8.GetString(message.Body)}");
		await queueClient.CompleteAsync(message.SystemProperties.LockToken);
	}

	private static async Task MainAsync(string[] args) {
		queueClient = new QueueClient(SERVICE_BUS, QUEUE_NAME, ReceiveMode.PeekLock);
		try{
			queueClient.RegisterMessageHandler(OnMessageReceived);
		}catch(Exception exception){
			Console.WriteLine(exception.StackTrace);
		}
        
	    	Console.WriteLine("Pressione qualquer botão");
	        Console.ReadKey();

		await queueClient.CloseAsync();
	}
    }
}
