using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace nialsorva.FCEEasyMods.Behaviors
{
    public class HotKeyBehavior
    {
        //=======================================================
        // Key constants
        //=======================================================
        public const string BUTTON_EXTRACT = "Extract";  // Typically 'Q'
        public const string BUTTON_INTERACT = "Interact";  // Typically 'E'
        public const string BUTTON_STORE = "Store";  // Typically 'T'

        public const int MOD_NONE = 0;
        public const int MOD_SHIFT = 1;
        public const int MOD_CTRL = 2;
        public const int MOD_ALT = 4;

        List<Tuple<string, int, Action>> buttonActions = new List<Tuple<string, int, Action>>();

        public HotKeyBehavior(ModMachineEntity e)
        {
            e.HoverEvent += CheckButtons;
        }

        //=======================================================
        // Button Handling
        //=======================================================
        public void AddButtonAction(string button, Action action) { AddButtonAction(button, MOD_NONE, action); }
        public void AddButtonAction(string button, int modMask, Action action)
        {
            buttonActions.Add(Tuple.Create(button, modMask, action));
        }


        protected void CheckButtons(ModMachineEntity e)
        {
            var actions = buttonActions.Where(a => Input.GetButtonDown(a.Item1));
            if (actions.Count() == 0)
                return;

            int keymask = 0;
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                keymask |= MOD_SHIFT;
            if (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt))
                keymask |= MOD_ALT;
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
                keymask |= MOD_CTRL;

            foreach (var v in actions.Where(a => a.Item2 == keymask))
            {
                v.Item3();
            }
        }
    }

}
}
