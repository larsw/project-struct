using System;
using System.Linq;
using Xunit;
using Xunit.Abstractions;
using Domain;
using Cedar.Domain;

namespace Domain.Tests
{
  public class UserTests
  {
    private readonly ITestOutputHelper _output;
    public UserTests(ITestOutputHelper output)
    {
      _output = output;
    }

    [Fact]
    public void ChangeName()
    {
      var router = new ConventionEventRouter();
      var user = new User(_output.WriteLine, "user1", router);
      for (var i = 0; i < 10; i++)
      {
        user.ChangeName("lars");
      }
      var uncommittedEvents = ((IAggregate)user).TakeUncommittedEvents().ToList();

      var user2 = new User(_output.WriteLine, "user2", router);
      var rehydrator = ((IAggregate)user2).BeginRehydrate();
      foreach(var @event in uncommittedEvents)
      {
        rehydrator.ApplyEvent(@event);
      }
    }
  }
}
