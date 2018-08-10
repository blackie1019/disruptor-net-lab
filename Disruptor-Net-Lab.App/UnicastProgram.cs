using System;
using System.Threading;
using System.Threading.Tasks;
using Disruptor;
using Disruptor_Net_Lab.Core;

namespace Disruptor_Net_Lab.App
{
    partial class Program
    {
        static void DemoDSLUnicast()
        {
            var disruptor = new Disruptor.Dsl. Disruptor<DataVO>(() => new DataVO(), (int)Math .Pow(2,4), TaskScheduler.Default); 
            disruptor.HandleEventsWith(new DataEventHandler("Handler1")); 
            var ringBuffer = disruptor.Start(); 
            var idx = 0; 
            while (true) { 
                var sequenceNo = ringBuffer.Next(); 
                var data = ringBuffer[sequenceNo]; 
                data.Value = idx++.ToString(); 
                ringBuffer.Publish(sequenceNo); 
                Thread.Sleep(250); 
            } 
            disruptor.Shutdown(); 
        }

        static void DemoNonDSLUnicast()
        {
            var ringBuffer = RingBuffer<DataVO>.CreateSingleProducer(() => new DataVO(), (int)Math.Pow(2, 4)); 
            var barrier = ringBuffer.NewBarrier(); 
            var eventProcessor = new BatchEventProcessor<DataVO>(ringBuffer, barrier, new DataEventHandler("Handler1")); 

            Task.Factory.StartNew(() => eventProcessor.Run());

            var idx = 0;
            while (true) { 
                var sequenceNo = ringBuffer.Next(); 
                var data = ringBuffer[sequenceNo]; 
                data.Value = idx++.ToString(); 
                ringBuffer.Publish(sequenceNo); 
                Thread.Sleep(250); 
            }  
            eventProcessor.Halt(); 
        }
       
    }
}