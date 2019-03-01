using System;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Core.Commands;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using fr34kyn01535.Uconomy;

namespace CapScroLL.MathQuestions
{
    public class MathQuestions : RocketPlugin<MathQuestionsConfig>
    {
        public static MathQuestions Instance, Automation;
        //Timer
        public DateTime? timer = null;
        public bool NoQuestion = true;
        void FixedUpdate()
        {
            Autoquestion();
        }
        //Translations
        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList()
                {
                    {"mathquestions_broadcast_question", "[MathQuestions]: '{0} {2} {1} = ?' Know the question and earn {3} {4} '/re <answer>'"},
                    {"mathquestions_broadcast_winner", "[MathQuestions]: {0} answered the question correctly and earned {2} {3}! Answer: {1}"},
                    {"mathquestions_true_answer", "[MathQuestions]: Congratulations! You have earned {0} {1} by answering correctly! Your new balance: {2} {1}"},
                    {"mathquestions_wrong_answer", "[MathQuestions]: Wrong answer!"},
                    {"mathquestions_invalid_parameter", "[MathQuestions]: Usage: '/re <answer>' or '/reply <answer>'"},
                    {"mathquestions_no_question", "[MathQuestions]: No active questions yet!"},
                    {"mathquestions_disabled", "MathQuestions is disabled!"}
                };
            }
        }
        //----------------------------------Starter-----------------------------//

        //Load
        protected override void Load()
        {
            Instance = this;
            if (Configuration.Instance.MathQuestionsEnabled)
            {
                Rocket.Core.Logging.Logger.LogWarning("             +------------------Math-Questions-----------------+");
                Rocket.Core.Logging.Logger.LogWarning("             |                  Coder: CapScroLL               |");
                Rocket.Core.Logging.Logger.LogWarning("             |-------------------------------------------------|");
                Rocket.Core.Logging.Logger.LogWarning("             |              youtube.com/CapScroLL              |");
                Rocket.Core.Logging.Logger.LogWarning("             |         steamcommunity.com/id/CapScroLL         |");
                Rocket.Core.Logging.Logger.LogWarning("             +------------------Math-Questions-----------------+");
            }
            else
            {
                Rocket.Core.Logging.Logger.LogError("             +------------------Math-Questions-----------------+");
                Rocket.Core.Logging.Logger.LogError("             |                     DISABLED                    |");
                Rocket.Core.Logging.Logger.LogError("             +------------------Math-Questions-----------------+");
            }
        }

        //Unload
        protected override void Unload()
        {
            Instance = null;
            Automation = null;
            Rocket.Core.Logging.Logger.LogError("[Math Questions] Unloaded!");
        }
        //---------------------------------/Starter-----------------------------//
        //ThisPluginBrain
        private static readonly System.Random autonumber = new System.Random();
        public static int number1, number2, autoperation, result;
        public static string operation;

        private void Autoquestion()
        {
            Automation = this;
            if (Configuration.Instance.MathQuestionsEnabled)
                try
                {
                    if (State == PluginState.Loaded && Configuration.Instance.Interval != 0 && (timer == null || ((DateTime.Now - timer.Value).TotalMinutes > Configuration.Instance.Interval)))
                    {
                        number1 = autonumber.Next(8, 85);
                        number2 = autonumber.Next(10, 97);

                        if (Configuration.Instance.Toplama == true && Configuration.Instance.Cikarma == false)
                        { operation = "+"; result = number1 + number2; }
                        if (Configuration.Instance.Toplama == false && Configuration.Instance.Cikarma == true)
                        { operation = "-"; result = number1 - number2; }

                        if (Configuration.Instance.Addition == Configuration.Instance.Subtraction)
                        {
                            autoperation = autonumber.Next(1, 3);
                            switch (autoperation)
                            {
                                case 1: operation = "+"; result = number1 + number2; break;
                                case 2: operation = "-"; result = number1 - number2; break;
                            }
                        }

                        MathQuestions.ExecuteDependencyCode("Uconomy", (IRocketPlugin plugin) =>
                        {
                            Uconomy Uconomy = (Uconomy)plugin;
                            string broadcast = Translate("mathquestions_broadcast_question", number1.ToString(), number2.ToString(), operation, Configuration.Instance.Reward.ToString(), Uconomy.Configuration.Instance.MoneyName);
                            UnturnedChat.Say(broadcast, UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, UnityEngine.Color.yellow));
                        });
                        timer = DateTime.Now;
                        NoQuestion = false;
                    }
            }
            catch (Exception old)
            {
                Rocket.Core.Logging.Logger.LogException(old);
            }
            else { Automation = null; }
        }

        //ReplyCommand
        [RocketCommand("reply", "Know the questions and make money!", "<answer>", AllowedCaller.Player)]
        [RocketCommandAlias("re")]
        public void ExecuteCommandcevap(IRocketPlayer caller, string[] answer)
        {
            if (NoQuestion == true)
            {
                string noquestion = Translate("mathquestions_no_question");
                UnturnedChat.Say(caller, noquestion, UnturnedChat.GetColorFromName(Configuration.Instance.UnfavorableMessageColor, UnityEngine.Color.red));
            }

                UnturnedPlayer player = (UnturnedPlayer)caller;
                if (answer.Length > 0)
                {
                    if (answer[0] == result.ToString() && NoQuestion == false)
                    {
                        NoQuestion = true;
                        MathQuestions.ExecuteDependencyCode("Uconomy", (IRocketPlugin plugin) =>
                        {
                            Uconomy Uconomy = (Uconomy)plugin;
                            Uconomy.Database.IncreaseBalance(player.CSteamID.ToString(), Configuration.Instance.Reward);
                            {
                                string rightanswer = Translate("mathquestions_true_answer", Configuration.Instance.Reward.ToString(), Uconomy.Configuration.Instance.MoneyName, Uconomy.Database.GetBalance(player.CSteamID.ToString()));
                                UnturnedChat.Say(player, rightanswer, UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, UnityEngine.Color.yellow));

                                string winner = Translate("mathquestions_broadcast_winner", caller.DisplayName, result.ToString(), Configuration.Instance.Reward.ToString(), Uconomy.Configuration.Instance.MoneyName);
                                UnturnedChat.Say(winner, UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, UnityEngine.Color.yellow));
                            }
                        });
                    }
                    if (answer[0] != result.ToString() && NoQuestion == false) UnturnedChat.Say(caller, Translate("mathquestions_wrong_answer"), UnturnedChat.GetColorFromName(Configuration.Instance.UnfavorableMessageColor, UnityEngine.Color.red));
                }
                else UnturnedChat.Say(caller, Translate("mathquestions_invalid_parameter"), UnturnedChat.GetColorFromName(Configuration.Instance.MessageColor, UnityEngine.Color.yellow));
        }
    }
 }
