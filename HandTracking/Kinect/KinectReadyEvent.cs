using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandTracking
{
    public class KinectReadyEvent
    {
        public delegate void AfterReady();
        public AfterReady afterColorReady { get; set; }
        public AfterReady afterDepthReady { get; set; }
        public AfterReady afterSkeletonReady { get; set; }
        
        private void skip() { }

        public KinectReadyEvent()
        {
            this.afterColorReady = skip;
            this.afterDepthReady = skip;
            this.afterSkeletonReady = skip;
        }

        public void setEventColorReady(AfterReady readyEvent)
        {
            this.afterColorReady = readyEvent;
        }
        public void clearEventColorReady()
        {
            this.afterColorReady = skip;
        }


        public void setEventDetpthReady(AfterReady readyEvent)
        {
            this.afterDepthReady = readyEvent;
        }
        public void clearEventDepthReady()
        {
            this.afterDepthReady = skip;
        }

        public void setEventSkeletonReady(AfterReady readyEvent)
        {
            this.afterSkeletonReady = readyEvent;
        }
        public void clearEventSkeletonReady()
        {
            this.afterSkeletonReady = skip;
        }

    }
}
