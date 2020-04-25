﻿using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram 
    {
        //Debug
        private static UpdateFrequency UPDATE_FREQUENCY = UpdateFrequency.Update1;

        //Configuration.  Keep all words lowercase
        private String[] ignoreWords = { "the", "than", "turn", "turned", "rotate", "set", "is", "block", "tell", "to", "from", "then", "of","either" };
        private String[] groupWords = { "blocks", "group" };
        private String[] activateWords = { "move", "go", "on", "start", "begin" };
        private String[] deactivateWords = { "stop", "off", "terminate", "exit", "cancel", "end" };
        private String[] reverseWords = { "reverse", "switch direction", "turn around" };
        private String[] increaseWords = { "increase", "raise", "extend", "expand", "forward", "forwards", "up" };
        private String[] decreaseWords = { "decrease", "lower", "retract", "reduce", "backward", "backwards", "down" };
        private String[] clockwiseWords = { "clockwise", "clock" };
        private String[] counterclockwiseWords = { "counter", "counterclock", "counterclockwise" };

        private String[] relativeWords = { "by" };
        private String[] increaseRelativeWords = { "add" };
        private String[] decreaseRelativeWords = { "subtact" };
        private String[] speedWords = { "speed", "velocity", "rate", "pace" };
        private String[] waitWords = { "wait", "hold", "pause" };
        private String[] connectWords = { "connect", "join", "attach", "connected", "joined", "attached" };
        private String[] disconnectWords = { "disconnect", "separate", "detach", "disconnected", "separated", "detached" };

        private String[] lockWords = { "lock", "freeze" };
        private String[] unlockWords = { "unlock", "unfreeze" };

        private String[] ifWords = { "if" };
        private String[] elseWords = { "else", "otherwise" };
        private String[] unlessWords = { "unless" };
        private String[] whileWords = { "while" };
        private String[] untilWords = { "until" };
        private String[] whenWords = { "when" };
        private String[] asyncWords = { "async" };

        private String[] lessWords = { "less", "<", "below" };
        private String[] lessEqualWords = { "<=" };
        private String[] equalWords = { "equal", "equals", "=", "==" };
        private String[] greaterEqualWords = { ">=" };
        private String[] greaterWords = { "greater", ">", "above", "more" };

        private String[] anyWords = { "any" };
        private String[] allWords = { "all" };
        private String[] noneWords = { "none" };

        private String[] andWords = { "and", "&", "&&", "but", "yet" };
        private String[] orWords = { "or", "|", "||" };
        private String[] notWords = { "not", "!", "isn't", "isnt" };
        private String[] openParenthesisWords = { "("};
        private String[] closeParenthesisWords = { ")" };

        private String[] restartWords = { "restart", "reset", "reboot" };

        public static String[] RUN_WORDS = { "run", "execute" };
        private String[] runningWords = { "running", "executing" };

        private String[] completeWords = { "done", "complete", "finished", "built" };
        private String[] progressWords = { "progress", "completion"};

        private String[] loopWords = { "loop", "iterate", "repeat", "rerun", "replay" };

        private Dictionary<String, UnitType> unitTypeWords = new Dictionary<String, UnitType>()
        {
            { "second", UnitType.SECONDS },
            { "seconds", UnitType.SECONDS },
            { "tick", UnitType.TICKS },
            { "ticks", UnitType.TICKS },
            { "degree", UnitType.DEGREES },
            { "degrees", UnitType.DEGREES },
            { "meter", UnitType.METERS },
            { "meters", UnitType.METERS },
            { "rpm", UnitType.RPM }
        };

        //Internal (Don't touch!)
        private Dictionary<String, List<CommandParameter>> propertyWords = new Dictionary<string, List<CommandParameter>>();

        static MultiActionCommand RUNNING_COMMANDS;
        static MyGridProgram PROGRAM;

        static bool isRunning = false;

        static void Print(String str)
        {
            PROGRAM.Echo(str);
        }

        public Program()
        {
            PROGRAM = this;
            addWords(groupWords, new GroupCommandParameter());
            addWords(activateWords, new BooleanCommandParameter(true));
            addWords(deactivateWords, new BooleanCommandParameter(false));
            addWords(increaseWords, new DirectionCommandParameter(DirectionType.UP));
            addWords(decreaseWords, new DirectionCommandParameter(DirectionType.DOWN));
            addWords(clockwiseWords, new DirectionCommandParameter(DirectionType.CLOCKWISE));
            addWords(counterclockwiseWords, new DirectionCommandParameter(DirectionType.COUNTERCLOCKWISE));
            addWords(reverseWords, new ReverseCommandParameter());
            addWords(relativeWords, new RelativeCommandParameter());
            addWords(speedWords, new NumericPropertyCommandParameter(NumericPropertyType.VELOCITY));
            addWords(connectWords, new BooleanPropertyCommandParameter(BooleanPropertyType.CONNECTED));
            addWords(disconnectWords, new BooleanPropertyCommandParameter(BooleanPropertyType.CONNECTED), new BooleanCommandParameter(false));
            addWords(lockWords, new BooleanPropertyCommandParameter(BooleanPropertyType.LOCKED));
            addWords(unlockWords, new BooleanPropertyCommandParameter(BooleanPropertyType.LOCKED), new BooleanCommandParameter(false));
            addWords(waitWords, new WaitCommandParameter());
            addWords(ifWords, new IfCommandParameter(false, false, false));
            addWords(unlessWords, new IfCommandParameter(true, false, false));
            addWords(whileWords, new IfCommandParameter(false, true, false));
            addWords(untilWords, new IfCommandParameter(true, true, false));
            addWords(whenWords, new IfCommandParameter(true, true, true));
            addWords(asyncWords, new AsyncCommandParameter());
            addWords(elseWords, new ElseCommandParameter());
            addWords(lessWords, new ComparisonCommandParameter(ComparisonType.LESS));
            addWords(lessEqualWords, new ComparisonCommandParameter(ComparisonType.LESS_OR_EQUAL));
            addWords(equalWords, new ComparisonCommandParameter(ComparisonType.EQUAL));
            addWords(greaterEqualWords, new ComparisonCommandParameter(ComparisonType.GREATER_OR_EQUAL));
            addWords(greaterWords, new ComparisonCommandParameter(ComparisonType.GREATER));
            addWords(anyWords, new AggregationModeCommandParameter(AggregationMode.ANY));
            addWords(allWords, new AggregationModeCommandParameter(AggregationMode.ALL));
            addWords(noneWords, new AggregationModeCommandParameter(AggregationMode.NONE));
            addWords(andWords, new AndCommandParameter());
            addWords(orWords, new OrCommandParameter());
            addWords(notWords, new NotCommandParameter());
            addWords(openParenthesisWords, new OpenParenthesisCommandParameter());
            addWords(closeParenthesisWords, new CloseParenthesisCommandParameter());
            addWords(restartWords, new RestartCommandParameter());
            addWords(RUN_WORDS, new StringPropertyCommandParameter(StringPropertyType.RUN));
            addWords(runningWords, new BooleanPropertyCommandParameter(BooleanPropertyType.RUNNING));
            addWords(completeWords, new BooleanPropertyCommandParameter(BooleanPropertyType.COMPLETE));
            addWords(progressWords, new NumericPropertyCommandParameter(NumericPropertyType.PROGRESS));
            addWords(loopWords, new LoopCommandParameter());
        }

        public void addWords(String[] words, params CommandParameter[] commands)
        {
            foreach (String word in words) propertyWords.Add(word, commands.ToList<CommandParameter>());
        }

        private void validateParsed()
        {
            if (RUNNING_COMMANDS == null) Start();
        }

        public void Main(string argument, UpdateType updateSource)
        {
            if (String.IsNullOrEmpty(argument))
            {
                validateParsed();
                if (!isRunning) RUNNING_COMMANDS.Reset();
                isRunning = true;
                if (Execute())
                {
                    Print("Execution Complete");
                    isRunning = false;
                    Runtime.UpdateFrequency = UpdateFrequency.None;
                }
                else
                {
                    Runtime.UpdateFrequency = UPDATE_FREQUENCY;
                }
            }
            else if (argument.ToLower() == "restart") //Restart execution of existing commands
            {
                validateParsed();
                Print("Restarting Commands");
                RUNNING_COMMANDS.Reset();
                Runtime.UpdateFrequency = UPDATE_FREQUENCY;
                isRunning = true;
            }
            else if (argument.ToLower() == "loop") //Loop execution of existing commands.  TODO: "Loop 3"
            {
                Print("Looping Commands");
                validateParsed();
                if (!isRunning) {
                    RUNNING_COMMANDS.Reset();
                }
                else {
                    RUNNING_COMMANDS.Loop(1);
                }
                Runtime.UpdateFrequency = UPDATE_FREQUENCY;
                isRunning = true;
            }
            else if (argument.ToLower() == "start") //Parse custom data and run
            {
                Print("Starting Commands");
                Start();
                isRunning = true;
                Runtime.UpdateFrequency = UPDATE_FREQUENCY;
            }
            else if (argument.ToLower() == "parse") // Parse Custom Data only.  Useful for debugging.
            {
                Print("Parsing Custom Data");
                ParseCommands();
                isRunning = false;
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }
            else if (argument.ToLower() == "stop") //Stop execution
            {
                Print("Stopping Command Execution");
                Runtime.UpdateFrequency = UpdateFrequency.None;
                isRunning = false;
                RUNNING_COMMANDS = null;
            }
        }

        private void Start()
        {
            List<Command> commands = ParseCommands();
            RUNNING_COMMANDS = new MultiActionCommand(commands);
        }

        private bool Execute()
        {
            if (RUNNING_COMMANDS == null) Start();
            return RUNNING_COMMANDS.Execute();
        }

        private List<Command> ParseCommands()
        {
            String[] commandList = Me.CustomData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

            return commandList
                .Select(command => parseTokens(command))
                .Select(tokens => parseCommandParameters(tokens))
                .Select(parameters => parseCommand(parameters))
                .ToList();
        }

        private List<CommandParameter> parseCommandParameters(List<String> tokens)
        {
            Print("Command: " + String.Join(" | ", tokens));

            List<CommandParameter> commandParameters = new List<CommandParameter>();
            foreach (var token in tokens)
            {
                if (ignoreWords.Contains(token)) continue;

                if (new BlockTypeParser().process(token, commandParameters)) continue;

                UnitType unitType;
                if (unitTypeWords.TryGetValue(token, out unitType))
                {
                    commandParameters.Add(new UnitCommandParameter(unitType));
                    continue;
                }

                if(increaseRelativeWords.Contains(token))
                {
                    commandParameters.Add(new RelativeCommandParameter());
                    commandParameters.Add(new DirectionCommandParameter(DirectionType.UP));
                    continue;
                }

                if (decreaseRelativeWords.Contains(token))
                {
                    commandParameters.Add(new RelativeCommandParameter());
                    commandParameters.Add(new DirectionCommandParameter(DirectionType.DOWN));
                    continue;
                }

                if (propertyWords.ContainsKey(token))
                {
                    commandParameters.AddList(propertyWords[token]);
                    continue;
                }

                double numericValue;
                if (Double.TryParse(token, out numericValue)) {
                    commandParameters.Add(new NumericCommandParameter((float)numericValue));
                    continue;
                }

                //If nothing else matches, must be a string
                commandParameters.Add(new StringCommandParameter(token));
            }

            return commandParameters;
        }

        private Command parseCommand(List<CommandParameter> parameters)
        {
            Print("Pre Processed Parameters:");
            parameters.ForEach(param => Print("Type: " + param.GetType()));

            ParameterProcessorRegistry.process(parameters);

            Print("Post Prossessed Parameters:");
            parameters.ForEach(param => Print("Type: " + param.GetType()));

            return CommandParserRegistry.ParseCommand(parameters);
        }
    }
}
