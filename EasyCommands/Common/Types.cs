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

namespace IngameScript {
    partial class Program {
        #region mdk preserve
        public enum BlockType { PISTON, ROTOR, PROGRAM, TIMER, LIGHT, PROJECTOR, MERGE, CONNECTOR, WELDER, GRINDER, DOOR, DISPLAY, SOUND, CAMERA, SENSOR, BEACON, ANTENNA, COCKPIT, REMOTE, THRUSTER, AIRVENT, GUN, REACTOR, GENERATOR, TANK, GEAR, BATTERY, PARACHUTE, SUSPENSION, DETECTOR, DRILL, ENGINE}
        public enum PropertyType { POWER, CONNECTED, LOCKED, RUNNING, PAUSED, STOPPED, COMPLETE, OPEN, TRIGGER, PRODUCE, AUTO, HEIGHT, ANGLE, VELOCITY, RATIO, FONT_SIZE, VOLUME, RANGE, MOVE_INPUT, ROLL_INPUT, NAME, RUN, TEXT, COLOR, SOUND }
        public enum UnitType { SECONDS, TICKS, DEGREES, RADIANS, METERS, RPM }
        public enum DirectionType { UP, DOWN, LEFT, RIGHT, FORWARD, BACKWARD, CLOCKWISE, COUNTERCLOCKWISE }
        public enum ComparisonType { GREATER, GREATER_OR_EQUAL, EQUAL, LESS_OR_EQUAL, LESS, NOT_EQUALS }
        public enum ControlType { START, RESTART, STOP, PARSE, LOOP, PAUSE, RESUME }
        public enum ProgramState { RUNNING, STOPPED, COMPLETE, PAUSED }
        public enum FunctionType { GOTO, GOSUB, SWITCH }
        public enum PrimitiveType { NUMERIC, BOOLEAN, STRING, VECTOR }
        public enum OperandType { ADD, SUBTACT, MULTIPLY, DIVIDE, MOD };
        public enum LogLevel { TRACE, DEBUG, INFO, SCRIPT_ONLY }
        #endregion
    }
}
