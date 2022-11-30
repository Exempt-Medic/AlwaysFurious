using Modding;
using System;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using SFCore.Utils;

namespace AlwaysFurious
{
    public class AlwaysFuriousMod : Mod
    {
        private static AlwaysFuriousMod? _instance;

        internal static AlwaysFuriousMod Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"An instance of {nameof(AlwaysFuriousMod)} was never constructed");
                }
                return _instance;
            }
        }

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public AlwaysFuriousMod() : base("AlwaysFurious")
        {
            _instance = this;
        }

        public override void Initialize()
        {
            Log("Initializing");

            On.PlayMakerFSM.OnEnable += OnFsmEnable;
            On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.OnEnter += OnPlayerDataBoolTestAction;

            Log("Initialized");
        }

        private void OnPlayerDataBoolTestAction(On.HutongGames.PlayMaker.Actions.PlayerDataBoolTest.orig_OnEnter orig, PlayerDataBoolTest self)
        {
            if (self.Fsm.Name == "Fury" && self.Fsm.GameObject.name == "Charm Effects" && self.State.Name == "Check HP")
            {
                self.isTrue = FsmEvent.GetFsmEvent("FURY");
            }

            orig(self);
        }

        private void OnFsmEnable(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            orig(self);

            if (self.gameObject.name == "Charm Effects" && self.FsmName == "Fury")
            {
                self.AddFsmGlobalTransitions("CHARM EQUIP CHECK", "Check HP");
                self.ChangeFsmTransition("Init", "FINISHED", "Check HP");
                self.ChangeFsmTransition("Check HP", "CANCEL", "Deactivate");
                self.ChangeFsmTransition("Activate", "HERO HEALED FULL", "Recheck");
                self.ChangeFsmTransition("Stay Furied", "HERO HEALED FULL", "Recheck");
                self.ChangeFsmTransition("Recheck", "FINISHED", "Stay Furied");
            }
        }
    }
}
