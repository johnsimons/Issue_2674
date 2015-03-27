using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;

namespace Issue_2674
{
    class Program
    {
        static void Main()
        {
            var busConfiguration = new BusConfiguration();
            busConfiguration.UsePersistence<InMemoryPersistence>();
            busConfiguration.EnableInstallers();
            var startableBus = Bus.Create(busConfiguration);
            using (var bus = startableBus.Start())
            {
                Console.Out.WriteLine("Press enter to send 50 messages that do async calls");
                while (true)
                {
                    Console.ReadLine();
                    Parallel.For(0, 50, i => bus.SendLocal(new ExecuteAsyncCode()));
                }
            }
        }
    }

    public class ExecuteAsyncCode : ICommand
    {
    }

    public class EventMessageHandler : IHandleMessages<ExecuteAsyncCode>
    {
        public void Handle(ExecuteAsyncCode message)
        {
            Console.Out.WriteLine("Before Thread {0}", Thread.CurrentThread.ManagedThreadId);
            DoAsync().Wait();
            Console.Out.WriteLine("After Thread {0}", Thread.CurrentThread.ManagedThreadId);
        }

        async Task DoAsync()
        {
            /*
              
            Create a table called Table_1:
              CREATE TABLE [dbo].[Table_1](
	            [Name] [nvarchar](50) NULL
                ) ON [PRIMARY]
              
            */
            using (SqlConnection conn = new SqlConnection(@"Server=localhost\sqlexpress;Database=nservicebus;Trusted_Connection=True;Asynchronous Processing=true;"))
            {
                await conn.OpenAsync();

                using (SqlCommand command = new SqlCommand("INSERT INTO Table_1 (Name) VALUES (@Name)", conn))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = "Hello";

                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                    Console.Out.WriteLine("Record saved {0}", Thread.CurrentThread.ManagedThreadId);
                }
                using (SqlCommand command = new SqlCommand("INSERT INTO Table_1 (Name) VALUES (@Name)", conn))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.Add("@Name", SqlDbType.NVarChar, 50).Value = "Hello";

                    await command.ExecuteNonQueryAsync();

                    Console.Out.WriteLine("Record saved {0}", Thread.CurrentThread.ManagedThreadId);
                }

                
            }
        }
    }
}
