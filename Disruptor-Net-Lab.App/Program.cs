using System;
using System.Threading;
using System.Threading.Tasks;
using Disruptor;
using Disruptor_Net_Lab.Core;

namespace Disruptor_Net_Lab.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            DemoNonDSLDisruptor();
            Console.ReadLine();
        }

        static void DemoDSLDisruptor()
        {
            var disruptor = new Disruptor.Dsl.Disruptor<DataVO>(() => new DataVO(), (int)Math.Pow(2,4), TaskScheduler.Default); 

            disruptor.HandleEventsWith(new DataEventHandler("Handler1"));
            var ringBuffer = disruptor.Start(); 
            var sequenceNo = ringBuffer.Next(); 
            var data = ringBuffer[sequenceNo]; 

            data.Value = "Hello"; 
            ringBuffer.Publish(sequenceNo); 
            sequenceNo = ringBuffer.Next(); 

            data = ringBuffer[sequenceNo]; 
            data.Value = "World"; 
            ringBuffer.Publish(sequenceNo); 

            disruptor.Shutdown(); 
        }

        static void DemoNonDSLDisruptor()
        {
            var bufferSize = (int) Math.Pow(2, 4);
            var ringBuffer = RingBuffer<DataVO>.CreateSingleProducer(() => new DataVO(), bufferSize); 
            var barrier = ringBuffer.NewBarrier(); 
            var eventProcessor = new BatchEventProcessor<DataVO>(ringBuffer, barrier, new DataEventHandler("Handler1")); 

            Task.Factory.StartNew(() => eventProcessor.Run()); 
            var task = Task.Run(() => eventProcessor.Run());
            
            var sequenceNo = ringBuffer.Next(); 
            Console.WriteLine($"Current SequenceNo:{sequenceNo.ToString()}");
            var data = ringBuffer[sequenceNo]; 
            data.Value = "Hello"; 
            ringBuffer.Publish(sequenceNo); 

            sequenceNo = ringBuffer.Next(); 
            Console.WriteLine($"Current SequenceNo:{sequenceNo.ToString()}");
            data = ringBuffer[sequenceNo]; 
            data.Value = "World"; 
            ringBuffer.Publish(sequenceNo); 
            
            Thread.Sleep(3000);
            eventProcessor.Halt();

        }
    }
}