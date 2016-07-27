using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nialsorva.FCEEasyMods.Behaviors
{
    public class HoverBehavior
    {
        protected long viewUntil = ModMachineEntity.Now;
        protected long lingerTimeInMS = 1000;

        public HoverBehavior(ModMachineEntity e)
        {
            e.HoverEvent += SetActive;
        }

        public void SetActive(ModMachineEntity sender)
        {
            viewUntil = ModMachineEntity.Now + lingerTimeInMS;
        }
        public bool IsActive()
        {
            return ModMachineEntity.Now <= viewUntil;
        }
    }

}
}
