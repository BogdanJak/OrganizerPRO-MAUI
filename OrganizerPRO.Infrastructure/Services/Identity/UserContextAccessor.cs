namespace OrganizerPRO.Infrastructure.Services.Identity;

public class UserContextAccessor : IUserContextAccessor
{
    private sealed class Node
    {
        public UserContext? Value;
        public Node? Parent;
    }


    private readonly AsyncLocal<Node?> _current = new();
    public UserContext? Current => _current.Value?.Value;

    public IDisposable Push(UserContext context)
    {
        var node = new Node
        {
            Value = context,
            Parent = _current.Value
        };
        _current.Value = node;
        return new Pop(this, node.Parent);
    }

    private sealed class Pop : IDisposable
    {
        private readonly UserContextAccessor _owner;
        private readonly Node? _restore;

        public Pop(UserContextAccessor owner, Node? restore)
        {
            _owner = owner;
            _restore = restore;
        }

        public void Dispose()
        {
            _owner._current.Value = _restore;
        }
    }


    public void Set(UserContext context)
    {
        _current.Value = new Node
        {
            Value = context,
            Parent = null
        };
    }

    public void Clear()
    {
        _current.Value = null;
    }
}
