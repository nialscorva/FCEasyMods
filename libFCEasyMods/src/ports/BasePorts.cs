using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nialsorva.FCEEasyMods
{
    public interface InputPort
    {
        bool CanAcceptFrom(SegmentEntity sourceEntity, ItemBase item);
    }

    public interface OutputPort
    {
        bool CanDeliverTo(SegmentEntity sourceEntity, ItemBase item);

    }

}
