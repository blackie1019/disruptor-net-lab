using System;
using System.Threading;
using Disruptor;

namespace Disruptor_Net_Lab.App
{
    public class DataEventHandler:IEventHandler<DataVO>
    {
        public string Name;

        public DataEventHandler(string Name)
        {
            this.Name = Name;
        }

        public void OnEvent(DataVO data, long sequence, bool endOfBatch)
        {
            Console.WriteLine("Thread = {0}, Handler = {1}, Sequence = {2}, Value = {3}", Thread.CurrentThread.ManagedThreadId.ToString(), this.Name, sequence.ToString(), data.Value); 

        }
    }
}