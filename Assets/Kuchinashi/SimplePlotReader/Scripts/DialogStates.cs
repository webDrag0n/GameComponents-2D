using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kuchinashi.SimplePlotReader
{
    public enum DialogState
    {
        Narration,
        CharacterATalking,
        CharacterBTalking,
        None
    }

    public class NarrationState : IState
    {
        private DialogFSM mFSM;
        public NarrationState(DialogFSM fsm)
        {
            mFSM = fsm;
        }

        public bool OnCondition()
        {
            return mFSM.CurrentStateType != DialogState.Narration;
        }

        public void OnEnter()
        {
            DialogFSM.FadeCharacter("A", 0);
            DialogFSM.FadeCharacter("B", 0);

            if (mFSM.LastStateType == DialogState.None)
            {
                DialogFSM.FadeLayer(1);
            }
        }
        
        public void OnExit()
        {
            mFSM.LastStateType = DialogState.Narration;
        }
    }

    public class CharacterATalkingState : IState
    {
        private DialogFSM mFSM;
        public CharacterATalkingState(DialogFSM fsm)
        {
            mFSM = fsm;
        }

        public bool OnCondition()
        {
            return mFSM.CurrentStateType != DialogState.CharacterATalking;
        }

        public void OnEnter()
        {
            DialogFSM.FadeCharacter("A", 1);
            DialogFSM.FadeCharacter("B", 0);

            if (mFSM.LastStateType == DialogState.None)
            {
                DialogFSM.FadeLayer(1);
            }
        }

        public void OnExit()
        {
            mFSM.LastStateType = DialogState.CharacterATalking;
        }
    }

    public class CharacterBTalkingState : IState
    {
        private DialogFSM mFSM;
        public CharacterBTalkingState(DialogFSM fsm)
        {
            mFSM = fsm;
        }

        public bool OnCondition()
        {
            return mFSM.CurrentStateType != DialogState.CharacterBTalking;
        }

        public void OnEnter()
        {
            DialogFSM.FadeCharacter("B", 1);
            DialogFSM.FadeCharacter("A", 0);

            if (mFSM.LastStateType == DialogState.None)
            {
                DialogFSM.FadeLayer(1);
            }
        }
        
        public void OnExit()
        {
            mFSM.LastStateType = DialogState.CharacterBTalking;
        }
    }

    public class NoneState : IState
    {
        private DialogFSM mFSM;
        public NoneState(DialogFSM fsm)
        {
            mFSM = fsm;
        }

        public bool OnCondition()
        {
            return mFSM.CurrentStateType != DialogState.None;
        }

        public void OnEnter()
        {
            DialogFSM.FadeLayer(0);
        }
        
        public void OnExit()
        {
            mFSM.LastStateType = DialogState.None;
        }
    }
}