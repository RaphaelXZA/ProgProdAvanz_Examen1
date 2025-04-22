using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProgProdAvanz_Examen1
{
    internal class MessageQueue
    {
        private Queue<string> messages;
        private readonly int maxMessages;

        public MessageQueue(int maxMessages = 5)
        {
            this.messages = new Queue<string>();
            this.maxMessages = maxMessages;
        }

        public void AddMessage(string message)
        {
            messages.Enqueue(message);

            //Si excede el número máximo de mensajes, elimina el más antiguo
            if (messages.Count > maxMessages)
            {
                messages.Dequeue();
            }
        }

        public void DisplayMessages()
        {
            if (messages.Count == 0)
            {
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow; 
            Console.WriteLine("\n╔══════════════════ MENSAJES ══════════════════╗");

            int count = 1;
            foreach (string message in messages)
            {
                Console.WriteLine($"║ {count}. {message.PadRight(42)} ║");
                count++;
            }

            Console.WriteLine("╚═══════════════════════════════════════════════╝");
            Console.ResetColor();
        }

        public void ShowImportantMessage(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n╔══════════════════ AVISO ═══════════════════╗");
            Console.WriteLine($"║ {message.PadRight(42)} ║");
            Console.WriteLine("╚═══════════════════════════════════════════════╝");
            Console.ResetColor();

            //Añadir el mensaje a la cola también
            AddMessage(message);
        }

        public void ClearMessages()
        {
            messages.Clear();
        }
    }
}
