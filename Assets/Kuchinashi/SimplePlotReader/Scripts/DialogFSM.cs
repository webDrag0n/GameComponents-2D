using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Kuchinashi.Utils;

namespace Kuchinashi.SimplePlotReader
{
    public partial class DialogFSM : MonoBehaviour
    {
        private static DialogFSM mSelf;
        public IState CurrentState;
        public DialogState CurrentStateType;
        public DialogState LastStateType;

        public static bool IsDisplaying;

        public Dictionary<DialogState, IState> States;
        public Sprite[] TachieSprites;
        public Sprite[] HeroineTachieDifferences;
        public SerializableDictionary<string, int> TachieSpritesDictionary = new SerializableDictionary<string, int>();

        public PlotReader Context;

        private CanvasGroup mCanvasGroup;
        private CanvasGroup mBackground;
        private Button mButton;

        private Image mTachieA;
        private Image mTachieB;
        private CanvasGroup mNameBoxA;
        private CanvasGroup mNameBoxB;
        private TMP_Text mNameLabelA;
        private TMP_Text mNameLabelB;
        private TMP_Text mText;

        public static DialogFSM GetInstance()
        {
            return mSelf;
        }

        void Awake()
        {
            mSelf = this;

            States = new Dictionary<DialogState, IState>();

            mCanvasGroup = this.GetComponent<CanvasGroup>();
            mButton = this.GetComponent<Button>();

            mBackground = transform.Find("Background").GetComponent<CanvasGroup>();

            mTachieA = transform.Find("TachieA").GetComponent<Image>();
            mTachieB = transform.Find("TachieB").GetComponent<Image>();
            mNameBoxA = transform.Find("NameBoxA").GetComponent<CanvasGroup>();
            mNameBoxB = transform.Find("NameBoxB").GetComponent<CanvasGroup>();
            mNameLabelA = transform.Find("NameBoxA/Text").GetComponent<TMP_Text>();
            mNameLabelB = transform.Find("NameBoxB/Text").GetComponent<TMP_Text>();
            mText = transform.Find("DialogBox/Text").GetComponent<TMP_Text>();

            mButton.onClick.AddListener(NextLine);

            RegisterState(DialogState.None, new NoneState(this));
            RegisterState(DialogState.Narration, new NarrationState(this));
            RegisterState(DialogState.CharacterATalking, new CharacterATalkingState(this));
            RegisterState(DialogState.CharacterBTalking, new CharacterBTalkingState(this));

            SetInitialState(DialogState.None);
        }

        void Start()
        {
            Context = new PlotReader(0);
            Context.ReadBeforeLines();
            NextLine();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                NextLine();
            }
        }
        
        public static void SetContext(int index)
        {
            mSelf.Context = new PlotReader(index);
        }
    }

    public partial class DialogFSM
    {
        public static void NextLine()
        {
            if (mSelf.Context.CurrentLines.Count > 0)
            {
                var line = mSelf.Context.CurrentLines[0];
                SetContent(line.Text);
                SetIdentity(line.Type, line.Id, line.Kind);

                mSelf.ChangeState((DialogState) line.Type);
                mSelf.Context.CurrentLines.RemoveAt(0);
            }
            else
            {
                mSelf.ChangeState(DialogState.None);
                if (mSelf.Context.CurrentPlot == "BeforeGame")
                {
                    mSelf.StartCoroutine(mSelf.FadeBackgroundCoroutine(0));
                    mSelf.Context.ReadGameLines();
                    NextLine();
                }
                else if (mSelf.Context.CurrentPlot == "AfterGame")
                {
                    mSelf.StartCoroutine(mSelf.FadeBackgroundCoroutine(1));
                }
            }
        }

        public void RegisterState(DialogState state, IState iState)
        {
            States.Add(state, iState);
        }

        public void ChangeState(DialogState targetState)
        {
            if (States[targetState] == null || !States[targetState].OnCondition())
            {
                return;
            }

            CurrentState.OnExit();

            CurrentState = States[targetState];
            CurrentStateType = targetState;

            CurrentState.OnEnter();
        }

        public void SetInitialState(DialogState targetState)
        {
            CurrentState = States[targetState];
            CurrentStateType = targetState;
            LastStateType = targetState;
        }

        public static void SetIdentity(LineType target, string id, string kind)
        {
            switch (target)
            {
                case LineType.CharacterA:
                    mSelf.mNameLabelA.text = Localization.Get(id);
                    mSelf.mTachieA.sprite = mSelf.TachieSprites[mSelf.TachieSpritesDictionary[id]];
                    if (kind != null && id == "P")
                    {
                        mSelf.mTachieA.sprite = mSelf.HeroineTachieDifferences[Int32.Parse(kind) - 1];
                    }
                    break;
                case LineType.CharacterB:
                    mSelf.mNameLabelB.text = Localization.Get(id);
                    mSelf.mTachieB.sprite = mSelf.TachieSprites[mSelf.TachieSpritesDictionary[id]];
                    if (kind != null && id == "P")
                    {
                        mSelf.mTachieA.sprite = mSelf.HeroineTachieDifferences[Int32.Parse(kind) - 1];
                    }
                    break;
                case LineType.Narration:
                    mSelf.mTachieA.sprite = null;
                    mSelf.mTachieB.sprite = null;
                    break;
                default:
                    break;
            }
        }

        public static void SetContent(string content)
        {
            mSelf.mText.text = content;
        }

        public static void FadeCharacter(string target, float targetAlpha)
        {
            switch (target)
            {
                case "A":
                    mSelf.StartCoroutine(mSelf.FadeCharacterCoroutine("A", targetAlpha));
                    break;
                case "B":
                    mSelf.StartCoroutine(mSelf.FadeCharacterCoroutine("B", targetAlpha));
                    break;
            }
        }

        IEnumerator FadeCharacterCoroutine(string target, float targetAlpha)
        {
            switch (target)
            {
                case "A":
                    mTachieA.CrossFadeAlpha(targetAlpha == 1f ? 1f : 0f, 0.3f, true);
                    while (!Mathf.Approximately(mNameBoxA.alpha, targetAlpha))
                    {
                        mNameBoxA.alpha = Mathf.MoveTowards(mNameBoxA.alpha, targetAlpha, 0.3f);
                        yield return null;
                    }
                    mNameBoxA.alpha = targetAlpha;
                    break;
                case "B":
                    mTachieB.CrossFadeAlpha(targetAlpha == 1f ? 1f : 0f, 0.3f, true);
                    while (!Mathf.Approximately(mNameBoxB.alpha, targetAlpha))
                    {
                        mNameBoxB.alpha = Mathf.MoveTowards(mNameBoxB.alpha, targetAlpha, 0.3f);
                        yield return null;
                    }
                    mNameBoxB.alpha = targetAlpha;
                    break;
            }
        }

        IEnumerator FadeBackgroundCoroutine(float targetAlpha)
        {
            mBackground.blocksRaycasts = targetAlpha == 1;

            while (!Mathf.Approximately(mBackground.alpha, targetAlpha))
            {
                mBackground.alpha = Mathf.MoveTowards(mBackground.alpha, targetAlpha, 0.3f);
                yield return new WaitForFixedUpdate();
            }
            mBackground.alpha = targetAlpha;
        }
        

        public static void FadeLayer(float targetAlpha)
        {
            mSelf.StartCoroutine(mSelf.FadeLayerCoroutine(targetAlpha));
        }

        IEnumerator FadeLayerCoroutine(float targetAlpha)
        {
            mCanvasGroup.blocksRaycasts = targetAlpha == 1;
            IsDisplaying = targetAlpha == 1;

            while (!Mathf.Approximately(mCanvasGroup.alpha, targetAlpha))
            {
                mCanvasGroup.alpha = Mathf.MoveTowards(mCanvasGroup.alpha, targetAlpha, 0.3f);
                yield return new WaitForFixedUpdate();
            }
            mCanvasGroup.alpha = targetAlpha;
        }
    }
}