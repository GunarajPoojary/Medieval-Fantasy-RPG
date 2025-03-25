using System.Collections.Generic;
using UnityEngine;

namespace Realfy
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }
    public class MoveCommand : ICommand
    {
        private Transform _transform;
        private Vector3 _direction;
        private Vector3 _previousPosition;

        public MoveCommand(Transform transform, Vector3 direction)
        {
            _transform = transform;
            _direction = direction;
        }

        public void Execute()
        {
            _previousPosition = _transform.position;
            _transform.position += _direction;
        }

        public void Undo()
        {
            _transform.position = _previousPosition;
        }
    }
    public class CommandManager : MonoBehaviour
    {
        private Stack<ICommand> _commandStack = new Stack<ICommand>();
        private Stack<ICommand> _redoStack = new Stack<ICommand>();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            _commandStack.Push(command);
            _redoStack.Clear(); // Clear redo stack on new command execution
        }

        public void UndoCommand()
        {
            if (_commandStack.Count > 0)
            {
                var command = _commandStack.Pop();
                command.Undo();
                _redoStack.Push(command);
            }
        }

        public void RedoCommand()
        {
            if (_redoStack.Count > 0)
            {
                var command = _redoStack.Pop();
                command.Execute();
                _commandStack.Push(command);
            }
        }
    }
}