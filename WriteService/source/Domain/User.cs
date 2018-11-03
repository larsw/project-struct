namespace Domain
{
  using System;
  using Cedar.Domain;
  public class User : AggregateBase
  {
    private Action<string> _logger;
    private string _name;

    public User(Action<string> logger, string id, IEventRouter router)
          : base(id, router)
    {
      _logger = logger;
    }

    public void ChangeName(string name)
    {
      if (name == null) throw new ArgumentNullException(nameof(name));

      RaiseEvent(new UserNameChanged(name));
    }

    private void Apply(UserNameChanged @event)
    {
      _logger("Name changed");
      _name = @event.Name;
    }
  }

  public class UserNameChanged
  {
    public string Name { get; }

    public UserNameChanged(string name)
    {
      Name = name;
    }
  }
}
