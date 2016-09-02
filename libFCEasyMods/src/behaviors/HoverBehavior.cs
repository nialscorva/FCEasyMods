using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nialscorva.FCEEasyMods.Behaviors
{
    public class HoverBehavior
    {
        protected long viewUntil = ModMachineEntity.Now;
        protected long lingerTimeInMS = 10;

        public HoverBehavior(ModMachineEntity e) : this(e, 100) { }
        public HoverBehavior(ModMachineEntity e,long lingerTimeInMS)
        {
            e.HoverEvent += SetActive;
            this.lingerTimeInMS = lingerTimeInMS;
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
