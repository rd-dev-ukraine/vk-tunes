using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AutoNotifier.Test
{
    [TestClass]
    public class DirtyCheckerTest
    {
        [TestMethod]
        public void GetChanges_Success()
        {
            // Arrange
            var checker = new DirtyChecker<Foo>();
            var foo = new Foo
            {
                Value = 10,
                Name = "Test"
            };

            var prevState = checker.GetState(foo);

            // Act
            foo.Value = 20;
            foo.Name = "Changed";

            var newState = checker.GetState(foo);

            // Assert
            var changes = checker.GetChanges(prevState, newState);

            Assert.AreEqual(2, changes.Count);
            Assert.IsTrue(changes.Contains(nameof(foo.Value)));
            Assert.IsTrue(changes.Contains(nameof(foo.Name)));
        }
    }

    public class Foo
    {
        public int Value { get; set; }

        public string Name { get; set; }

        public List<string> Address { get; set; }
    }
}