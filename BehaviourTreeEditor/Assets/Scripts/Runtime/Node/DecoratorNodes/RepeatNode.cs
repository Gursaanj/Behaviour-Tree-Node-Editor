using UnityEngine;

namespace Gbt
{
    /// <summary>
    /// Repeats ChildNode's execution for given number of executions
    /// </summary>
    public class RepeatNode : DecoratorNode
    {
        [Tooltip("Number of repetitions performed. Leave as -1 for infinite cycles")]
        public int numberOfRepetitions = -1;
        
        private bool _commitInfiniteRepetitions = false;
        private int _currentCycle;
        
        public override string InspectorName { get; protected set; } = "Repeat Node";

        public override void InjectData(object data)
        {
            if (data is int givenRepetitions)
            {
                if (givenRepetitions < -1)
                {
                    Debug.LogError("Invalid amount of repetitions");
                }
                else
                {
                    numberOfRepetitions = givenRepetitions;
                }
            }
        }

        protected override void OnStart()
        {
            if (numberOfRepetitions < -1)
            {
                _hasValidConditions = false;
                Debug.LogWarning($"Field {nameof(numberOfRepetitions)} cannot be less than -1, will return failure state.");
                return;
            }

            _commitInfiniteRepetitions = numberOfRepetitions == -1;
            _currentCycle = 0;
        }

        protected override void OnStop()
        {
            
        }

        protected override State OnUpdate()
        {
            if (Child.state == State.Failure)
            {
                return State.Failure;
            }

            if (_commitInfiniteRepetitions)
            {
                Child.Update();
                return State.Running;
            }

            if (Child.state == State.Success)
            {
                _currentCycle++;
            }

            if (_currentCycle < numberOfRepetitions)
            {
                Child.Update();
                return State.Running;
            }

            return State.Success;
        }
    }
}
