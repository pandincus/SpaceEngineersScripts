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
        public class RemoteControlBlockHandler : ShipControllerHandler<IMyRemoteControl> {
            public RemoteControlBlockHandler() : base() {
                numericPropertySetters.Add(NumericPropertyType.VELOCITY, new SimpleNumericPropertySetter<IMyRemoteControl>((b) => (float)b.GetShipSpeed(), (b, v) => b.SpeedLimit = v, 10));
                numericPropertySetters.Add(NumericPropertyType.RANGE, new SimpleNumericPropertySetter<IMyRemoteControl>((b) => (float)b.GetShipSpeed(), (b, v) => b.SpeedLimit = v, 10));
                numericPropertyGetters.Add(NumericPropertyType.RANGE, (b) => b.SpeedLimit);
                booleanPropertySetters.Add(BooleanPropertyType.CONNECTED, (b, v) => b.SetDockingMode(v));
                //TODO: better Property
                booleanPropertyGetters.Add(BooleanPropertyType.TRIGGER, (b) => b.IsAutoPilotEnabled);
                booleanPropertySetters.Add(BooleanPropertyType.TRIGGER, (b,v) => b.SetAutoPilotEnabled(v));
            }
        }

        public class ShipControllerHandler<T> : TerminalBlockHandler<T> where T : class, IMyShipController {
            public ShipControllerHandler() {
                numericPropertyGetters.Add(NumericPropertyType.VELOCITY, (b) => (float)b.GetShipSpeed());
                numericPropertyDirectionGetters.Add(NumericPropertyType.INPUT, GetPilotInput);
                defaultDirection = DirectionType.UP;
                defaultNumericProperties.Add(DirectionType.UP, NumericPropertyType.VELOCITY);

            }

            float GetPilotInput(T block, DirectionType direction) {
                var pilotInput = block.MoveIndicator;
                switch(direction) {
                    case DirectionType.UP: return pilotInput.Y;
                    case DirectionType.DOWN: return -pilotInput.Y;
                    case DirectionType.LEFT: return -pilotInput.X;
                    case DirectionType.RIGHT: return pilotInput.X;
                    case DirectionType.FORWARD: return -pilotInput.Z;
                    case DirectionType.BACKWARD: return pilotInput.Z;
                    default: throw new Exception("Unsupported User Input Direction Type: " + direction);
                }
            }
        }
    }
}
