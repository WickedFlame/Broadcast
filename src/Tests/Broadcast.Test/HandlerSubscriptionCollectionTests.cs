using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Broadcast.Test
{
    public class HandlerSubscriptionCollectionTests
    {
        private HandlerSubscriptionCollection _subscriptions;

        [SetUp]
        public void SetUp()
        {
            _subscriptions = [];
        }

        [Test]
        public void HandlerSubscriptionCollection_Add()
        {
            var handler = new CountEventHandler();

            _subscriptions.Add(typeof(CountEvent), handler);

            _subscriptions.Single()
                .Should().BeEquivalentTo(new
                {
                    EventType = typeof(CountEvent),
                    Handler = handler
                });
        }

        [Test]
        public void HandlerSubscriptionCollection_Add_Null()
        {
            var act = () => _subscriptions.Add(typeof(CountEvent), null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void HandlerSubscriptionCollection_Add_Subscription()
        {
            var handler = new CountEventHandler();

            var subscription = new HandlerSubscription(typeof(CountEvent), handler);

            _subscriptions.Add(subscription);

            _subscriptions.Single()
                .Should().Be(subscription);
        }

        [Test]
        public void HandlerSubscriptionCollection_Add_Subscription_Null()
        {
            var act = () => _subscriptions.Add(null);

            act.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void HandlerSubscriptionCollection_Add_MultipleSame()
        {
            var sub1 = new HandlerSubscription(typeof(CountEvent), Mock.Of<IEventHandler>());
            var sub2 = new HandlerSubscription(typeof(CountEvent), Mock.Of<IEventHandler>());

            _subscriptions.Add(sub1);
            _subscriptions.Add(sub2);

            _subscriptions.Count()
                .Should().Be(2);
        }

        [Test]
        public void HandlerSubscriptionCollection_Get_Multiple()
        {
            var sub1 = new HandlerSubscription(typeof(CountEvent), Mock.Of<IEventHandler>());
            var sub2 = new HandlerSubscription(typeof(CountEvent), Mock.Of<IEventHandler>());
            var sub3 = new HandlerSubscription(typeof(string), Mock.Of<IEventHandler>());

            _subscriptions.Add(sub1);
            _subscriptions.Add(sub2);
            _subscriptions.Add(sub3);

            _subscriptions.Get(typeof(CountEvent)).All(s => s.EventType == typeof(CountEvent))
                .Should().BeTrue();
        }

        [Test]
        public void HandlerSubscriptionCollection_Get_Multiple_Count()
        {
            var sub1 = new HandlerSubscription(typeof(CountEvent), Mock.Of<IEventHandler>());
            var sub2 = new HandlerSubscription(typeof(CountEvent), Mock.Of<IEventHandler>());
            var sub3 = new HandlerSubscription(typeof(string), Mock.Of<IEventHandler>());

            _subscriptions.Add(sub1);
            _subscriptions.Add(sub2);
            _subscriptions.Add(sub3);

            _subscriptions.Get(typeof(CountEvent)).Count()
                .Should().Be(2);
        }
    }
}
