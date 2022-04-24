using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using Meep.Tech.Data.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data {
  public partial struct Stat {
    public partial class Sheet {

      /// <summary>
      /// For making new types of stat sheets.
      /// </summary>
      public class Type : Archetype<Stat.Sheet, Stat.Sheet.Type> {

        protected override Dictionary<string, object> DefaultTestParams
         => new() {
           { nameof(Sheet.BaseStats), new Dictionary<Stat.Type, float> () }
         };

        /// <summary>
        /// A general description of this type
        /// </summary>
        public string Description {
          get;
        }

        /// <summary>
        /// The default stat types set up by this sheet when making a new model.
        /// </summary>
        public IReadOnlyCollection<Stat.Type> DefaultBaseStatTypes {
          get => _defaultStats;
           
        } HashSet<Stat.Type> _defaultStats
          = new();
        /// <summary>
        /// Used to add more base stat types in the ctor.
        /// </summary>
        protected IReadOnlyCollection<Stat.Type> AdditionalBaseStatTypes {
          init => value.Select(_defaultStats.Add);
        }

        /// <summary>
        /// The default derived stats that can be depleted/have a temp value.
        /// </summary>
        public IReadOnlyCollection<Stat.Type> DepleteableStats {
          get => _depleteableStats;
        } HashSet<Stat.Type> _depleteableStats 
          = new();
        /// <summary>
        /// Used to add more depleteable stats in the ctor.
        /// </summary>
        protected IReadOnlyCollection<Stat.Type> AdditionalDepleteableStats {
          init => value.Select(_depleteableStats.Add);
        }

        /// <summary>
        /// The default derived stats set up by this sheet when making a new model.
        /// </summary>
        public IReadOnlyDictionary<Stat.Type, DerivedStat> DefaultDerivedStats {
          get => _derivedStats;
        } Dictionary<Stat.Type, DerivedStat> _derivedStats
          = new(); 

        /// <summary>
        /// Used to set more drerived stats in the ctor.
        /// </summary>
        protected IReadOnlyDictionary<Stat.Type, DerivedStat> AdditionalDerivedStats {
          init {
            value.ForEach(e => {
              _derivedStats.Add(e.Key, e.Value);
            });
          }
        }

        /// <summary>
        /// A constructor for making new types of stat sheets.
        /// </summary>
        protected Type(Identity id, string description, HashSet<Stat.Type> defaultFixedStats = null, HashSet<Stat.Type> depleteableStatTypes = null, Dictionary<Stat.Type, DerivedStat> derivedStats = null)
          : base(id ?? new Identity("None")) {
          Description = description;
          _defaultStats = (defaultFixedStats ?? new()).Concat(depleteableStatTypes ?? Enumerable.Empty<Stat.Type>()).ToHashSet();
          _depleteableStats = depleteableStatTypes ?? _depleteableStats;
          _derivedStats = derivedStats ?? new(); 
        }

        /// <summary>
        /// XBam ctor for the base type.
        /// </summary>
        Type()
          : this(new Identity("None"), "A Generic Stat Sheet that can be used for Anything.") { }

        /// <summary>
        /// Make a stat sheet.
        /// </summary>
        public Sheet Make(Dictionary<Stat.Type, float> baseValues = null, Dictionary<Stat.Type, float> tempValues = null, IEnumerable<Modifier> modifiers = null, IEnumerable<Stat.Sheet> dependencies = null)
          => Make(
            (nameof(Stat.Sheet.BaseStats), baseValues),
            ("TempStatValues", tempValues),
            (nameof(Stat.Modifiers), modifiers),
            (nameof(Stat.Sheet.Dependencies), dependencies)
          );

        /// <summary>
        /// Make a new model with the default stat types
        /// </summary>
        public override Func<IBuilder<Stat.Sheet>, Stat.Sheet> ModelConstructor {
          get {
            return builder => {
              Stat.Sheet @return;

              /// set up all the base stats if it's a special archetype
              if (builder.Archetype != Stat.Sheet.Types.Get<Stat.Sheet.Type>()) {
                // make with special values
                IEnumerable<IStat> baseStats = DefaultBaseStatTypes.Select<Stat.Type, IStat>(a => DepleteableStats.Contains(a)
                  ? a.MakeDepleteable()
                  : a.Make());

                @return = _derivedStats.Any()
                  ? new Stat.Sheet(
                    baseStats.ToHashSet(),
                    DefaultDerivedStats,
                    builder.GetParam<Dictionary<Stat.Sheet.Type, Stat.Sheet>>(nameof(Stat.Sheet.Dependencies))
                  ) : new Stat.Sheet(
                    baseStats.ToHashSet(),
                    builder.GetParam<Dictionary<Stat.Sheet.Type, Stat.Sheet>>(nameof(Stat.Sheet.Dependencies))
                  );

                /// update the base values
                foreach (var stat in builder.GetParam<Dictionary<Stat.Type, float>>(nameof(Stat.Sheet.BaseStats)) ?? Enumerable.Empty<KeyValuePair<Stat.Type, float>>()) {
                  @return.ModifyBase(stat.Key, stat.Value, out _);
                }
              } // if there is not a special archetype.
              else {
                @return = new Sheet(
                  builder.GetParam<Dictionary<Stat.Type, float>>(nameof(Stat.Sheet.BaseStats))?.Select(
                    a => DepleteableStats.Contains(a.Key) ? a.Key.MakeDepleteable() : a.Key.Make() as IStat
                  ).ToHashSet() ?? throw new ArgumentNullException(nameof(Stat.Sheet.BaseStats)),
                  builder.GetParam<Dictionary<Stat.Sheet.Type, Stat.Sheet>>(nameof(Stat.Sheet.Dependencies))
                );
              }

              /// overwrite depelteable stats with their values
              foreach (var stat in builder.GetParam<Dictionary<Stat.Type, float>>("TempStatValues") ?? Enumerable.Empty<KeyValuePair<Stat.Type, float>>()) {
                DepleteableStat compiled = stat.Key.MakeDepleteable(@return[stat.Key].BaseValue);
                compiled = compiled.WithUpdatedTempBaseValue(stat.Value);
                @return[stat.Key] = compiled;
              }


              /// add any modifiers to their stats
              foreach (var modifiers in builder.GetParam<Dictionary<Stat.Type, IEnumerable<Modifier>>>(nameof(Stat.Modifiers)) ?? Enumerable.Empty<KeyValuePair<Stat.Type, IEnumerable<Modifier>>>()) {
                modifiers.Value.ForEach(m => @return.AddModifier(modifiers.Key, m, out _));
              }

              return @return;
            };
          }
        }

        protected override JObject SerializeModelToJson(IModel model) {
          Stat.Sheet toSerialize = (Stat.Sheet)model;
          var @return = JObject.FromObject(new {
            archetype 
              = JToken.FromObject(toSerialize.Archetype, Id.Universe.ModelSerializer.JsonSerializer)
          });
          if (toSerialize.BaseStats.Any()) {
            @return.Add("baseValues", JObject.FromObject(toSerialize.BaseStats
                .ToDictionary(s => s.Id.Key, s => toSerialize.Get(s).BaseValue)));
          }
          if (toSerialize.Stats.Any(s => s is DepleteableStat)) {
            @return.Add("tempValues", JObject.FromObject(toSerialize.Stats
                .Where(s => s is DepleteableStat)
                .ToDictionary(s => s.Archetype.Id.Key, s => ((DepleteableStat)s).TemporaryBaseValue)));
          }
          if (toSerialize.Stats.Any(s => s.Modifiers.Any())) {
            @return.Add("modifiers", JObject.FromObject(
              toSerialize.Stats
                .Select(s => new KeyValuePair<Stat.Type, IEnumerable<Modifier>>(
                  s.Archetype,
                  s.Modifiers
                )).Where(s => s.Value.Any())
                .ToDictionary(s => s.Key, s => s.Value)
            ));
          }

          return @return;
        }

        protected override IModel DeserializeModelFromJson(JObject jObject, System.Type deserializeToTypeOverride = null, params (string key, object value)[] withConfigurationParameters) {
          if (jObject.TryGetValue(nameof(Archetype), out JToken archetypeKey)) {
            Stat.Sheet.Type archetype = archetypeKey.ToObject<Stat.Sheet.Type>();
            return archetype.Make(
              jObject.TryGetValue<Dictionary<Stat.Type, float>>("baseValues"),
              jObject.TryGetValue<Dictionary<Stat.Type, float>>("tempValues"),
              jObject.TryGetValue<Dictionary<Stat.Type, IEnumerable<Modifier>>>("Modifiers")?
                .Values.SelectMany(Comparitors.Identity)
            );
          } else throw new ArgumentNullException(nameof(Archetype));
        }
      }
    }
  }
}
