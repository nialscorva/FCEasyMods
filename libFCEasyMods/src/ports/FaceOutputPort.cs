using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nialsorva.FCEEasyMods
{
    public interface OutputPort
    {
        bool AllowedEntity(SegmentEntity sourceEntity, Vector3 relativePosition);
        bool AllowedItem(ItemBase i);
    }

    public class FaceOutputPort : OutputPort
    {
        private ModMachineEntity machine;
        private Vector3 face;

        public FaceOutputPort(ModMachineEntity machine, Vector3 face)
        {
            this.machine = machine;
            this.face = face;
        }

        public bool IsActive()
        {
            throw new NotImplementedException();
        }

        public void DeliverOne()
        {
            throw new NotImplementedException();
        }
    }
}
