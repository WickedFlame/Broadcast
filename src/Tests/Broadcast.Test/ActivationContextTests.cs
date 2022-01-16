using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Broadcast.Test
{
    public class ActivationContextTests
    {
        [Test]
        public void ActivationContext_ctor()
        {
            Assert.DoesNotThrow(() => new ActivationContext());
        }

        [Test]
        public void ActivationContext_Resolve()
        {
            var ctx = new ActivationContext();
            ctx.Resolve<Resolveable>().Should().NotBeNull();
        }

        [Test]
        public void ActivationContext_Resolve_Type()
        {
            var ctx = new ActivationContext();
            ctx.Resolve(typeof(Resolveable)).Should().NotBeNull();
        }

        [Test]
        public void ActivationContext_Resolve_CheckType()
        {
            var ctx = new ActivationContext();
            ctx.Resolve<Resolveable>().Should().BeOfType<Resolveable>();
        }

        [Test]
        public void ActivationContext_Resolve_Type_CheckType()
        {
            var ctx = new ActivationContext();
            ctx.Resolve(typeof(Resolveable)).Should().BeOfType<Resolveable>();
        }

        [Test]
        public void ActivationContext_Resolve_UseSmallestCtor()
        {
            var ctx = new ActivationContext();
            ctx.Resolve<ResolveableCtor>().Should().NotBeNull();
        }

        [Test]
        public void ActivationContext_Resolve_CheckCtor()
        {
            var ctx = new ActivationContext();
            ctx.Resolve<MultipleCtor>().Value.Should().Be("1");
        }

        [Test]
        public void ActivationContext_Resolve_UnresolvableCtor()
        {
            var ctx = new ActivationContext();
            Action task = () => ctx.Resolve<UnresolvableCtor>();
            task.Should().NotThrow<Exception>();
        }

        [Test]
        public void ActivationContext_Resolve_UnresolvableCtor_Null()
        {
            var ctx = new ActivationContext();
            ctx.Resolve<UnresolvableCtor>().Value.Should().NotBeNull();
        }

        [Test]
        public void ActivationContext_Register()
        {
            var ctx = new ActivationContext();
            ctx.Register<IUnresolvableCtor, UnresolvableCtor>();

            ctx.Registrations.Should().ContainSingle(r => r.Key == typeof(IUnresolvableCtor)); 
        }

        [Test]
        public void ActivationContext_Register_Resolve()
        {
            var ctx = new ActivationContext();
            ctx.Register<IUnresolvableCtor, UnresolvableCtor>();

            ctx.Resolve<IUnresolvableCtor>().Should().BeOfType<UnresolvableCtor>();
        }

        [Test]
        public void ActivationContext_Register_Func()
        {
            var inst = new UnresolvableCtor("singleton");

            var ctx = new ActivationContext();
            ctx.Register<IUnresolvableCtor>(() => inst);

            ctx.Registrations.Should().ContainSingle(r => r.Key == typeof(IUnresolvableCtor));
        }

        [Test]
        public void ActivationContext_Register_Func_Resolve()
        {
            var inst = new UnresolvableCtor("singleton");

            var ctx = new ActivationContext();
            ctx.Register<IUnresolvableCtor>(() => inst);

            ctx.Resolve<IUnresolvableCtor>().Should().BeSameAs(inst).And.Subject.As<IUnresolvableCtor>().Value.Should().Be("singleton");
        }

        [Test]
        public void ActivationContext_RegisterSingleton()
        {
            var ctx = new ActivationContext();
            ctx.RegisterSingleton<IUnresolvableCtor>(new UnresolvableCtor("singleton"));

            ctx.Registrations.Should().ContainSingle(r => r.Key == typeof(IUnresolvableCtor));
        }

        [Test]
        public void ActivationContext_RegisterSingleton_Resolve()
        {
            var inst = new UnresolvableCtor("singleton");

            var ctx = new ActivationContext();
            ctx.RegisterSingleton<IUnresolvableCtor>(inst);

            ctx.Resolve<IUnresolvableCtor>().Should().BeSameAs(inst).And.Subject.As<IUnresolvableCtor>().Value.Should().Be("singleton");
        }

        [Test]
        public void ActivationContext_RegisterSingleton_Resolve_Multiple()
        {
            var ctx = new ActivationContext();
            ctx.RegisterSingleton<IUnresolvableCtor>(new UnresolvableCtor("singleton"));

            ctx.Resolve<IUnresolvableCtor>().Should().BeSameAs(ctx.Resolve<IUnresolvableCtor>());
        }

        [Test]
        public void ActivationContext_RegisterSingleton_Func()
        {
            var inst = new UnresolvableCtor("singleton");

            var ctx = new ActivationContext();
            ctx.RegisterSingleton<IUnresolvableCtor>(() => inst);

            ctx.Registrations.Should().ContainSingle(r => r.Key == typeof(IUnresolvableCtor));
        }

        [Test]
        public void ActivationContext_RegisterSingleton_Func_Resolve()
        {
            var inst = new UnresolvableCtor("singleton");

            var ctx = new ActivationContext();
            ctx.RegisterSingleton<IUnresolvableCtor>(() => inst);

            ctx.Resolve<IUnresolvableCtor>().Should().BeSameAs(inst).And.Subject.As<IUnresolvableCtor>().Value.Should().Be("singleton");
        }

        [Test]
        public void ActivationContext_RegisterSingleton_Func_Resolve_Multiple()
        {
            var ctx = new ActivationContext();
            ctx.RegisterSingleton<IUnresolvableCtor>(() => new UnresolvableCtor("singleton"));

            ctx.Resolve<IUnresolvableCtor>().Should().BeSameAs(ctx.Resolve<IUnresolvableCtor>());
        }

        [Test]
        public void ActivationContext_ChildContext()
        {
            var ctx = new ActivationContext();
            ctx.Register<IUnresolvableCtor, UnresolvableCtor>();

            var child = ctx.ChildContext().Should().NotBeNull();
        }

        [Test]
        public void ActivationContext_ChildContext_Registrations()
        {
            var ctx = new ActivationContext();
            ctx.Register<IUnresolvableCtor, UnresolvableCtor>();

            var child = ctx.ChildContext();
            ((ActivationContext)child).Registrations.Should().ContainSingle(r => r.Key == typeof(IUnresolvableCtor));
        }

        [Test]
        public void ActivationContext_ChildContext_Registrations_Same()
        {
            var ctx = new ActivationContext();
            ctx.Register<IUnresolvableCtor, UnresolvableCtor>();

            var child = ctx.ChildContext();
            ((ActivationContext)child).Registrations[typeof(IUnresolvableCtor)].Should().BeSameAs(ctx.Registrations[typeof(IUnresolvableCtor)]);
        }

        [Test]
        public void ActivationContext_ChildContext_Resolve_Same()
        {
            var ctx = new ActivationContext();
            ctx.RegisterSingleton<IUnresolvableCtor>(() => new UnresolvableCtor(""));

            var child = ctx.ChildContext();
            child.Resolve<IUnresolvableCtor>().Should().BeSameAs(ctx.Resolve<IUnresolvableCtor>());
        }

        [Test]
        public void ActivationContext_ChildContext_IsCopy()
        {
            var ctx = new ActivationContext();
            ctx.Register<IUnresolvableCtor, UnresolvableCtor>();

            ctx.ChildContext().Should().NotBeSameAs(ctx);
        }

        public class Resolveable { }

        public class ResolveableCtor
        {
            public ResolveableCtor() { }

            public ResolveableCtor(string value)
            {
                throw new InvalidOperationException("This should not be called");
            }
        }

        public interface IUnresolvableCtor
        {
            string Value { get; }
        }

        public class UnresolvableCtor: IUnresolvableCtor
        {
            public UnresolvableCtor(string value)
            {
                Value = value;
            }

            public string Value { get; }
        }

        public class MultipleCtor
        {
            public MultipleCtor()
            {
                Value = "1";
            }

            public MultipleCtor(string value)
            {
                Value = "2";
            }

            public string Value { get; }
        }
    }
}
