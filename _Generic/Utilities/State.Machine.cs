using Meep.Tech.Collections.Generic;
using Meep.Tech.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpiritWorlds.Data.Utilities {

  /// <summary>
  /// A state something can be in.
  /// </summary>
  public class State : Model<State, State.Type>, IUnique {
    List<State> _nextPotentialStates = new();
    Random _seedBasedRandomizer;

    string IUnique.Id { get; set; }

    /// <summary>
    /// A machine for moving between states.
    /// </summary>
    public class Machine : Model<Machine, Machine.Type> {
      Dictionary<string, State> _statesById = new();
      List<string> _previousStates = new();
      HashSet<string> _initialStates = new();
      HashSet<string> _freelyAccessableStates = new();

      /// <summary>
      /// An archetype for making pre-built state machines.
      /// </summary>
      public class Type : Archetype<Machine, Machine.Type> {
        public static Identity Basic {
          get;
        } = new Identity("Basic");

        protected Type(Identity id)
          : base(id ?? Basic) { }

        /// <summary>
        /// Make a new state machine.
        /// </summary>
        /// <returns></returns>
        public virtual Machine Make(
          IEnumerable<State> states = null,
          Dictionary<string, IEnumerable<string>> directOutgoingStateConnections = null,
          Random seedBasedRandomizer = null,
          params (string name, object value)[] @params
        )
          => Make(@params
              .Append((nameof(States), states))
              .Append((nameof(SeedBasedRandomizer), seedBasedRandomizer))
              .Append((nameof(State.NextPotentialStates), directOutgoingStateConnections)));
      }

      /// <summary>
      /// All states this machine has access to
      /// </summary>
      public IEnumerable<State> States
        => _statesById.Values;

      /// <summary>
      /// The current state the machine is in.
      /// </summary>
      public State CurrentState {
        get;
        private set;
      }

      /// <summary>
      /// A seed based randomizer
      /// TODO: this can be set via make.
      /// </summary>
      public Random SeedBasedRandomizer {
        get;
        private set;
      }

      /// <summary>
      /// The history of states changed by this machine, highest index is the previous state.
      /// </summary>
      public IReadOnlyList<string> History
        => _previousStates;

      /// <summary>
      /// The previous state the machine was in.
      /// </summary>
      public State PreviousState
        => _statesById[History.Last()];

      protected Machine(IBuilder<Machine> builder) {
        /// get data needed to build a state machine.
        SeedBasedRandomizer = builder.GetParam<Random>(nameof(SeedBasedRandomizer));
        _statesById = builder.GetParam<IEnumerable<State>>(nameof(States))
          .ToDictionary(s => s.GetUniqueId());
        var directStateToStateConnections = builder.GetParam<Dictionary<string, IEnumerable<string>>>(nameof(States));

        foreach (State state in _statesById.Values) {
          // set up the state and place it in the right buckets
          state._seedBasedRandomizer ??= SeedBasedRandomizer;
          if (state.Archetype.IsInitial) {
            _initialStates.Add(state.GetUniqueId());
          }
          if (state.Archetype.IsAccessableToAnyState) {
            _freelyAccessableStates.Add(state.GetUniqueId());
          }

          /// collect outgoing links
          var potentialNextStateIds = directStateToStateConnections?
            .TryToGet(state.GetUniqueId());
          if (state.Archetype.IsOpenEnded && _freelyAccessableStates.Any()) {
            potentialNextStateIds 
              = _freelyAccessableStates.Concat(potentialNextStateIds ?? Enumerable.Empty<string>());
          }

          /// connect outgoing links
          AddExistingStatesToNextPotentialOptionsForState(
            state,
            withDirectAccessToExistingStates: 
              potentialNextStateIds
                ?.Distinct()
                .Select(e => _statesById[e])
                // TODO: shuffle them before ordering them?
                .OrderByDescending(e => e.Archetype.WeightAsNextChoice));
        }
      }

      /// <summary>
      /// Determine the next state based on inputs.
      /// </summary>
      public State DetermineNextState(IEnumerable<IInput> inputs) {
        // initalize the first state
        if (CurrentState is null) {
          Initialize();
        } // find the next state
        else {
          TagedCollection<Tag, IInput> inputsByTag = new();
          inputs.ForEach(i => inputsByTag.Add(i.Tags, i));

          bool stateChanged = false;
          foreach (State potentialNextState in CurrentState.NextPotentialStates) {
            if (potentialNextState.CalculateProbabilityOfBeingActivatedByInput(this, inputsByTag) > SeedBasedRandomizer.NextDouble()) {
              CurrentState.Archetype.OnExit(CurrentState, this);
              _previousStates.Add(CurrentState.GetUniqueId());
              CurrentState = potentialNextState;
              CurrentState.Archetype.OnEnter(CurrentState, this);
              if (CurrentState.GetUniqueId() != _previousStates.Last()) {
                stateChanged = true;
              }

              break;
            }
          }
          if (!stateChanged) {
            CurrentState.Archetype.OnUpdate(CurrentState, this);
          }
        }

        return CurrentState;
      }

      /// <summary>
      /// Initialize the state machine.
      /// (optional) provide a specific initial state
      /// </summary>
      protected void Initialize(State initialState = null) {
        SeedBasedRandomizer ??= new();
        if (initialState != null) {
          if (_statesById.ContainsKey(initialState.GetUniqueId())) {
            CurrentState = _statesById[initialState.GetUniqueId()];
          }
        }
        else {
          CurrentState = _statesById[_initialStates.RandomEntry(SeedBasedRandomizer)];
        }

        CurrentState.Archetype.OnEnter(CurrentState, this);
      }

      /// <summary>
      /// Add a new state to the state machine.
      /// </summary>
      protected void AddStateWithConnections(State state, IEnumerable<State> accessableDirectlyFromExistingStates = null, IEnumerable<State> withDirectAccessToExistingStates = null) {
        AddState(state);
        AddExistingStatesToNextPotentialOptionsForState(state, withDirectAccessToExistingStates);
        AddStateToNextPotentialPotionsForExistingStates(state, accessableDirectlyFromExistingStates);
      }

      /// <summary>
      /// Add just a state to the current machine
      /// </summary>
      protected void AddState(State state) {
        _statesById[state.GetUniqueId()] = state;
        state._seedBasedRandomizer ??= SeedBasedRandomizer;
        if (state.Archetype.IsInitial) {
          _initialStates.Add(state.GetUniqueId());
        }
        if (state.Archetype.IsAccessableToAnyState) {
          _freelyAccessableStates.Add(state.GetUniqueId());
        }
      }

      /// <summary>
      /// Add the given state as a next potential state to each provided already existing state in the machine.
      /// </summary>
      protected void AddStateToNextPotentialPotionsForExistingStates(State state, IEnumerable<State> accessableDirectlyFromExistingStates) {
        if (state.Archetype.IsAccessableToAnyState) {
          accessableDirectlyFromExistingStates
            .Concat(States.Where(s => s.Archetype.IsOpenEnded && !accessableDirectlyFromExistingStates.Any(e => s.GetUniqueId() == e.GetUniqueId())));
        }

        accessableDirectlyFromExistingStates?.ForEach(otherState => {
          if (_statesById.TryGetValue(otherState.GetUniqueId(), out otherState) && ! otherState.NextPotentialStates.Contains(state)) {
            otherState._nextPotentialStates.Add(state);
          }
        });
      }

      /// <summary>
      /// Add each alreacy existing given state in the machine as a next potential state to the individual provided state.
      /// </summary>
      protected void AddExistingStatesToNextPotentialOptionsForState(State state, IEnumerable<State> withDirectAccessToExistingStates) {
        if (state.Archetype.IsOpenEnded) {
          withDirectAccessToExistingStates
            .Concat(States.Where(s => s.Archetype.IsAccessableToAnyState && !withDirectAccessToExistingStates.Any(e => s.GetUniqueId() == e.GetUniqueId())));
        }

        withDirectAccessToExistingStates
          ?.ForEach(otherState => { 
            if (!state.NextPotentialStates.Contains(otherState)) {
              state._nextPotentialStates.Add(otherState);
            }
          });
      }

      /// <summary>
      /// An input for a state machine.
      /// </summary>
      public interface IInput {

        /// <summary>
        /// Tags associated with this input.
        /// Some can be provided by default.
        /// </summary>
        IEnumerable<Tag> Tags {
          get;
          internal protected set;
        }

        /// <summary>
        /// Add tags to an input
        /// </summary>
        void AddTags(IEnumerable<Tag> tags)
          => Tags = Tags.Concat(tags);

        /// <summary>
        /// Add tags to an input
        /// </summary>
        void AddTags(params Tag[] tags)
          => AddTags((IEnumerable<Tag>)tags);
      }
    }

    /// <summary>
    /// A type of State.
    /// </summary>
    public abstract class Type : Archetype<State, Type> {

      /// <summary>
      /// If this is a potential initial state the machine can start with.
      /// </summary>
      public virtual bool IsInitial {
        get;
      }

      /// <summary>
      /// If this can be accessed from any state potentially.
      /// States with this set to true get added to the NextPotentialStates collection of states in the same machine where IsOpenEnded=true.
      /// </summary>
      public virtual bool IsAccessableToAnyState {
        get;
      }

      /// <summary>
      /// If this state should have any states in the same machine with IsAccessableToAnyState=true included in it's NextPotentialStates options.
      /// </summary>
      public virtual bool IsOpenEnded {
        get;
      }

      /// <summary>
      ///  for determining order of picking the next state. higher is more important.
      /// </summary>
      public int WeightAsNextChoice {
        get;
      }

      /// <summary>
      /// The default next potential states to automatically attach whenever this State type is added to a machine.
      /// </summary>
      public virtual IEnumerable<State> DefaultNextPotentialStates {
        get => _DefaultNextPotentialStates ?? new List<State>();
      }  /** <summary> The backing field used to initialize and override the initail value of DefaultNextPotentialStates. 
          * You can this syntax to override or add to the base initial value: 
          * '=> _DefaultNextPotentialStates ??= base.DefaultNextPotentialStates.Append(...' </summary> 
          **/
      protected IEnumerable<State> _DefaultNextPotentialStates {
        get => _defaultNextPotentialStates; set => _defaultNextPotentialStates = value;
      }
      IEnumerable<State> _defaultNextPotentialStates;

      protected Type(Identity id)
        : base(id) { }

      /// <summary>
      /// Calculate the probability (between 0 and 1) of this state being activated given the input.
      /// </summary>
      protected internal abstract float CalculateProbabilityOfBeingActivatedByInput(State state, Machine machine, TagedCollection<Tag, Machine.IInput> inputs);

      /// <summary>
      /// Called when this state is entered by a state machine.
      /// </summary>
      protected internal abstract void OnEnter(State state, State.Machine machine);

      /// <summary>
      /// Called when this state is exited by a state machine.
      /// </summary>
      protected internal abstract void OnExit(State state, State.Machine machine);

      /// <summary>
      /// Called when the state machine runs on this state without changing states.
      /// </summary>
      protected internal virtual void OnUpdate(State state, State.Machine machine) { }
    }

    /// <summary>
    /// A seed based randomizer
    /// </summary>
    public Random SeedBasedRandomizer {
      get => _seedBasedRandomizer ??= Meep.Tech.Noise.RNG.Static;
      private set => _seedBasedRandomizer = value;
    }

    /// <summary>
    /// The next potential states this state could access.
    /// </summary>
    public IEnumerable<State> NextPotentialStates
      => _nextPotentialStates;

    protected State(IBuilder<State> builder) {
      SeedBasedRandomizer = builder.GetParam<Random>(nameof(SeedBasedRandomizer));
    }

    /// <summary>
    /// Calculate the probability (between 0 and 1) of this state being activated given the input.
    /// </summary>
    public float CalculateProbabilityOfBeingActivatedByInput(State.Machine machine, TagedCollection<Tag, Machine.IInput> inputs)
      => Archetype.CalculateProbabilityOfBeingActivatedByInput(this, machine, inputs);
  }
}