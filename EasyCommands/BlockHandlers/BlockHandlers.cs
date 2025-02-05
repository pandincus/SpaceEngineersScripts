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
        public static class BlockHandlerRegistry {
            public delegate List<Object> BlockProvider(BlockType blockType, String name);
            public static BlockProvider BLOCK_PROVIDER = GetBlocks;
            public static BlockProvider GROUP_BLOCK_PROVIDER = GetBlocksInGroup;

            static readonly Dictionary<BlockType, BlockHandler> blockHandlers = new Dictionary<BlockType, BlockHandler> {
               { BlockType.AIRVENT, new AirVentBlockHandler()},
               { BlockType.ANTENNA, new AntennaBlockHandler()},
               { BlockType.BATTERY, new BatteryBlockHandler()},
               { BlockType.BEACON, new BeaconBlockHandler()},
               { BlockType.CAMERA, new CameraBlockHandler() },
               { BlockType.COCKPIT, new ShipControllerHandler<IMyCockpit>()},
               { BlockType.CONNECTOR, new ConnectorBlockHandler() },
               { BlockType.DETECTOR, new OreDetectorHandler() },
               { BlockType.DISPLAY, new TextSurfaceHandler() },
               { BlockType.DRILL, new FunctionalBlockHandler<IMyShipDrill>() },
               { BlockType.DOOR, new DoorBlockHandler() },
               { BlockType.ENGINE, new FunctionalBlockHandler<IMyPowerProducer>() },
               { BlockType.GEAR, new LandingGearHandler() },
               { BlockType.GENERATOR, new GasGeneratorHandler()},
               { BlockType.GRINDER, new FunctionalBlockHandler<IMyShipGrinder>() },
               { BlockType.GUN, new GunBlockHandler<IMyUserControllableGun>() },
               { BlockType.LIGHT, new LightBlockHandler() },
               { BlockType.MERGE, new MergeBlockHandler() },
               { BlockType.PARACHUTE, new ParachuteBlockHandler() },
               { BlockType.PROGRAM, new ProgramBlockHandler() },
               { BlockType.PISTON, new PistonBlockHandler() },
               { BlockType.PROJECTOR, new ProjectorBlockHandler() },
               { BlockType.REACTOR, new FunctionalBlockHandler<IMyReactor>()},
               { BlockType.REMOTE, new RemoteControlBlockHandler()},
               { BlockType.ROTOR, new RotorBlockHandler() },
               { BlockType.SOUND, new SoundBlockHandler() },
               { BlockType.SENSOR, new SensorBlockHandler() },
               { BlockType.SUSPENSION, new WheelSuspensionBlockHandler() },
               { BlockType.TANK, new GasTankBlockHandler() },
               { BlockType.TIMER, new FunctionalBlockHandler<IMyTimerBlock>() },
               { BlockType.THRUSTER, new ThrusterBlockHandler()},
               { BlockType.WELDER, new FunctionalBlockHandler<IMyShipWelder>() },
            };

            public static BlockHandler GetBlockHandler(BlockType blockType) {
                if (!blockHandlers.ContainsKey(blockType)) throw new Exception("Unsupported Block Type: " + blockType);
                return blockHandlers[blockType];
            }
            public static List<Object> GetBlocks(BlockType blockType, String customName) {
                return blockHandlers[blockType].GetBlocks(customName);
            }
            public static List<Object> GetBlocksInGroup(BlockType blockType, String groupName) {
                return blockHandlers[blockType].GetBlocksInGroup(groupName);
            }
        }

        //Property Getters
        public delegate Primitive GetProperty<T>(T block);
        public delegate Primitive GetPropertyDirection<T>(T block, DirectionType direction);
        public delegate void SetProperty<T>(T block, Primitive value);
        public delegate void SetPropertyDirection<T>(T block, DirectionType direction, Primitive value);
        public delegate void IncrementProperty<T>(T block, Primitive deltaValue);
        public delegate void IncrementPropertyDirection<T>(T block, DirectionType direction, Primitive deltaValue);
        public delegate void MovePropertyValue<T>(T block, DirectionType direction);
        public delegate void ReversePropertyValue<T>(T block);

        //Getters
        public delegate bool GetBooleanProperty<T>(T block);
        public delegate string GetStringProperty<T>(T block);
        public delegate float GetNumericProperty<T>(T block);

        //Setters
        public delegate void SetBooleanProperty<T>(T block, bool value);
        public delegate void SetStringProperty<T>(T block, String value);
        public delegate void SetNumericProperty<T>(T block, float value);

        public class PropertyHandler<T> {
            public GetProperty<T> Get;
            public GetPropertyDirection<T> GetDirection;
            public SetProperty<T> Set;
            public SetPropertyDirection<T> SetDirection;
            public IncrementProperty<T> Increment;
            public IncrementPropertyDirection<T> IncrementDirection;
            public MovePropertyValue<T> Move;
            public ReversePropertyValue<T> Reverse;
        }

        public class SimplePropertyHandler<T> : PropertyHandler<T> {
            public SimplePropertyHandler(GetProperty<T> Get, SetProperty<T> Set, Primitive delta) {
                this.Get = Get;
                this.Set = Set;
                SetDirection = (b, d, v) => Set(b, v);
                Increment = (b, v) => Set(b, Get(b).Plus(v));
                IncrementDirection = (b, d, v) => Increment(b, Multiply(v, d));
                Move = (b, d) => Set(b, Get(b).Plus(Multiply(delta,d)));
                Reverse = (b) => Set(b, Get(b).Not());
            }
            private Primitive Multiply(Primitive p, DirectionType d) { return (d == DirectionType.DOWN) ? p.Not() : p; }
        }

        public class SimpleNumericPropertyHandler<T> : SimplePropertyHandler<T> {
            public SimpleNumericPropertyHandler(GetNumericProperty<T> GetValue, SetNumericProperty<T> SetValue, float delta)
                : base((b) => new NumberPrimitive(GetValue(b)), (b,v)=> {
                    if (v.GetType() != PrimitiveType.NUMERIC) throw new Exception("Cannot assign non-number to this property");
                    SetValue(b, (float)v.GetValue());
                }, new NumberPrimitive(delta)) {
            }
        }

        public class SimpleStringPropertyHandler<T> : SimplePropertyHandler<T> {
            public SimpleStringPropertyHandler(GetStringProperty<T> GetValue, SetStringProperty<T> SetValue)
                : base((b) => new StringPrimitive(GetValue(b)), (b, v) => {
                    SetValue(b,v.GetValue().ToString());
                }, new StringPrimitive("")) {
            }
        }

        public class SimpleBooleanPropertyHandler<T> : SimplePropertyHandler<T> {
            public SimpleBooleanPropertyHandler(GetBooleanProperty<T> GetValue, SetBooleanProperty<T> SetValue)
                : base((b) => new BooleanPrimitive(GetValue(b)), (b, v) => {
                    if (v.GetType() != PrimitiveType.BOOLEAN) throw new Exception("Cannot assign non-boolean to this property");
                    SetValue(b, (bool)v.GetValue());
                }, new BooleanPrimitive(true)) {
            }
        }

        public class PropertyValueNumericPropertyHandler<T> : SimpleNumericPropertyHandler<T> where T : class, IMyTerminalBlock {
            public PropertyValueNumericPropertyHandler(String propertyName, float delta) : base((b) => b.GetValueFloat(propertyName), (b, v) => b.SetValueFloat(propertyName, v), delta) {
            }
        }

        public interface BlockHandler {
            PropertyType GetDefaultProperty(PrimitiveType type);
            PropertyType GetDefaultProperty(DirectionType direction);
            DirectionType GetDefaultDirection();
            List<Object> GetBlocks(String name);
            List<Object> GetBlocksInGroup(String groupName);

            Primitive GetPropertyValue(Object block, PropertyType property);
            Primitive GetPropertyValue(Object block, PropertyType property, DirectionType direction);

            void SetPropertyValue(Object block, PropertyType property, Primitive value);
            void SetPropertyValue(Object block, PropertyType property, DirectionType direction, Primitive value);
            void IncrementPropertyValue(Object block, PropertyType property, Primitive value);
            void IncrementPropertyValue(Object block, PropertyType property, DirectionType direction, Primitive value);
            void MoveNumericPropertyValue(Object block, PropertyType property, DirectionType direction);
            void ReverseNumericPropertyValue(Object block, PropertyType property);
        }

        public class FunctionalBlockHandler<T> : TerminalBlockHandler<T> where T : class, IMyFunctionalBlock {
            public FunctionalBlockHandler() : base() {
                AddPropertyHandler(PropertyType.POWER, new SimpleBooleanPropertyHandler<T>((block) => block.Enabled, (block, enabled) => block.Enabled = enabled));
                defaultPropertiesByPrimitive[PrimitiveType.BOOLEAN] = PropertyType.POWER;
            }
        }

        public abstract class TerminalBlockHandler<T> : BlockHandler<T> where T : class, IMyTerminalBlock {
            public override List<T> GetBlocksOfType(String name) {
                List<T> blocks = new List<T>();
                PROGRAM.GridTerminalSystem.GetBlocksOfType<T>(blocks, block => block.CustomName.Equals(name));
                return blocks;
            }

            public override List<T> GetBlocksOfTypeInGroup(String groupName) {
                List<IMyBlockGroup> blockGroups = new List<IMyBlockGroup>();
                PROGRAM.GridTerminalSystem.GetBlockGroups(blockGroups);
                IMyBlockGroup group = blockGroups.Find(g => g.Name.Equals(groupName));
                if (group == null) { throw new Exception("Unable to find requested block group: " + groupName); }
                List<T> blocks = new List<T>();
                group.GetBlocksOfType<T>(blocks);
                return blocks;
            }

            protected override string Name(T block) { return block.CustomName; }

            protected String GetCustomProperty(T block, String key) { return GetCustomData(block).GetValueOrDefault(key); }
            protected void SetCustomProperty(T block, String key, String value) {
                Dictionary<String, String> d = GetCustomData(block);
                d[key] = value;SaveCustomData(block, d);
            }
            protected void SaveCustomData(T block, Dictionary<String, String> data) {
                block.CustomData = String.Join("\n",data.Keys.Select(k => k + "=" + data[k] + '\n').ToList());
            }
            protected Dictionary<String, String> GetCustomData(T block) {
                List<String> keys = block.CustomData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                return keys.ToDictionary(k => k.Split('=')[0], v => v.Split('=')[1]);
            }
        }

        public abstract class BlockHandler<T> : BlockHandler where T : class {
            protected Dictionary<PropertyType, PropertyHandler<T>> propertyHandlers = new Dictionary<PropertyType, PropertyHandler<T>>();
            protected PropertyType defaultBooleanProperty = PropertyType.POWER;
            protected PropertyType defaultStringProperty = PropertyType.NAME;
            protected Dictionary<PrimitiveType, PropertyType> defaultPropertiesByPrimitive = new Dictionary<PrimitiveType, PropertyType>();
            protected Dictionary<DirectionType, PropertyType> defaultPropertiesByDirection = new Dictionary<DirectionType, PropertyType>();
            protected DirectionType? defaultDirection = null;

            public List<Object> GetBlocks(String name) { return GetBlocksOfType(name).Select(t => t as object).ToList(); }
            public List<Object> GetBlocksInGroup(String groupName) { return GetBlocksOfTypeInGroup(groupName).Select(t => t as object).ToList(); }

            public abstract List<T> GetBlocksOfType(String name);
            public abstract List<T> GetBlocksOfTypeInGroup(String name);

            public DirectionType GetDefaultDirection() {
                if (!defaultDirection.HasValue) throw new Exception(GetType() + " Does Not Have a Default Direction");
                return defaultDirection.Value;
            }
            public PropertyType GetDefaultProperty(DirectionType direction) {
                if (!defaultPropertiesByDirection.ContainsKey(direction)) throw new Exception(GetType() + " Does Not Have A Default Property for Direction: " + direction);
                return defaultPropertiesByDirection[direction];
            }
            public PropertyType GetDefaultProperty(PrimitiveType type) {
                if (!defaultPropertiesByPrimitive.ContainsKey(type)) throw new Exception(GetType() + " Does Not Have A Default Property for Primitive: " + type);
                return defaultPropertiesByPrimitive[type];
            }
            public Primitive GetPropertyValue(object block, PropertyType property) {
                return propertyHandlers[property].Get((T)block);
            }
            public Primitive GetPropertyValue(object block, PropertyType property, DirectionType direction) {
                return propertyHandlers[property].GetDirection((T)block, direction);
            }
            public void SetPropertyValue(Object block, PropertyType property, Primitive value) {
                Debug("Setting " + Name(block) + " " + property + " to " + value.GetValue());
                propertyHandlers[property].Set((T)block, value);
            }
            public void SetPropertyValue(Object block, PropertyType property, DirectionType direction, Primitive value) {
                Debug("Setting " + Name(block) + " " + property + " to " + value.GetValue() + " in " + direction + " direction");
                propertyHandlers[property].SetDirection((T)block, direction, value);
            }
            public void IncrementPropertyValue(Object block, PropertyType property, Primitive value) {
                Debug("Incrementing " + Name(block) + " " + property + " by " + value.GetValue());
                propertyHandlers[property].Increment((T)block, value);
            }
            public void IncrementPropertyValue(Object block, PropertyType property, DirectionType direction, Primitive value) {
                Debug("Incrementing " + Name(block) + " " + property + " by " + value.GetValue() + " in " + direction + " direction");
                propertyHandlers[property].IncrementDirection((T)block, direction, value);
            }
            public void MoveNumericPropertyValue(Object block, PropertyType property, DirectionType direction) {
                Debug("Moving " + Name(block) + " " + property + " in " + direction + " direction");
                propertyHandlers[property].Move((T)block, direction);
            }
            public void ReverseNumericPropertyValue(Object block, PropertyType property) {
                Debug("Reversing " + Name(block) + " " + property);
                propertyHandlers[property].Reverse((T)block);
            }
            private string Name(object block) {
                return Name((T)block);
            }
            protected abstract string Name(T block);

            protected void AddBooleanHandler(PropertyType property, GetBooleanProperty<T> Get) {
                AddBooleanHandler(property, Get, (b, v) => { });
            }

            protected void AddBooleanHandler(PropertyType property, GetBooleanProperty<T> Get, SetBooleanProperty<T> Set) {
                propertyHandlers[property] = new SimpleBooleanPropertyHandler<T>(Get, Set);
            }

            protected void AddPropertyHandler(PropertyType property, PropertyHandler<T> handler) {
                propertyHandlers[property] = handler;
            }

            protected void AddStringHandler(PropertyType property, GetStringProperty<T> Get, SetStringProperty<T> Set) {
                propertyHandlers[property] = new SimpleStringPropertyHandler<T>(Get, Set);
            }

            protected void AddNumericHandler(PropertyType property, GetNumericProperty<T> Get) {
                propertyHandlers[property] = new SimpleNumericPropertyHandler<T>(Get, (b, v) => { }, 0);
            }

            protected void AddNumericHandler(PropertyType property, GetNumericProperty<T> Get, SetNumericProperty<T> Set, float delta) {
                propertyHandlers[property] = new SimpleNumericPropertyHandler<T>(Get, Set, delta);
            }
        }
    }
}
