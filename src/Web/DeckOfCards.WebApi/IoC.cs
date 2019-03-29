using Lamar;
using MediatR;
using System.Reflection;

namespace DeckOfCards.WebApi
{
    public static class IoC
    {
        public static ServiceRegistry CreateLamarIocContainer()
        {
            var registry = new ServiceRegistry();

            registry.Scan(scanner =>
            {
                // old Structuremap calls fail at runtime, but they compile
                //scanner.IncludeNamespace("ApiKickstart");
                //scanner.LookForRegistries();
                //scanner.AssembliesFromApplicationBaseDirectory();
                //scanner.TheCallingAssembly();
                scanner.WithDefaultConventions();

                //scanner.Assembly(Assembly.Load(nameof(DeckOfCards.QueryHandlers)));
                //scanner.Assembly(Assembly.Load(nameof(DeckOfCards.CommandHandlers)));
                scanner.Assembly(Assembly.Load("DeckOfCards.QueryHandlers")); // todo: improve assembly targeting logic
                scanner.Assembly(Assembly.Load("DeckOfCards.CommandHandlers"));
                //scanner.AssemblyContainingType<CardTemplateQueryHandler>(); 
                //scanner.AssemblyContainingType<NewDeckOfCardsCommandHandler>(); // todo: improve assembly targeting logic

                // auto register the open generics for our handler classes - https://github.com/wooderz/MediatR/wiki
                scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<>)); // Handlers with no response
                scanner.ConnectImplementationsToTypesClosing(typeof(IRequestHandler<,>)); // Handlers with a response
                scanner.ConnectImplementationsToTypesClosing(typeof(INotificationHandler<>));
            });

            // Mediatr works best when configured directly with your IoC container - https://github.com/jbogard/MediatR/wiki
            registry.For<IMediator>().Use<Mediator>().Scoped();
            registry.For<ServiceFactory>().Use(ctx => ctx.GetInstance);
            return registry;
        }
    }
}
