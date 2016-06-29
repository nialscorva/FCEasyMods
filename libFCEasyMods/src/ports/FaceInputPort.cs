using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nialsorva.FCEEasyMods
{
    public interface InputPort
    {
        bool Allow(SegmentEntity sourceEntity, Vector3 relativePosition, ItemBase item);
        bool Allow(SegmentEntity sourceEntity, Vector3 relativePosition);
        void Take(ItemBase item);
        void Take(StorageMachineInterface storage);
    }
    public class FaceInputPort : InputPort
    {
        private ModMachineEntity machine;
        private Vector3 face;

        public FaceInputPort(ModMachineEntity machine, Vector3 face)
        {
            this.machine = machine;
            this.face = face;
        }

        public void AcceptOne()
        {
            throw new NotImplementedException();
        }

        public bool IsActive()
        {
            throw new NotImplementedException();
        }
    }
}
