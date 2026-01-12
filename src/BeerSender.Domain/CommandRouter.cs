using Marten;
using Microsoft.AspNetCore.Http;
using System.Runtime.InteropServices;

namespace BeerSender.Domain
{
    public class CommandRouter(IServiceProvider serviceProvider, IDocumentStore store, IHttpContextAccessor httpContextAccessor) 
    {
        public async Task HandleCommand( ICommand command)
        {
            var commandType = command.GetType();
            var handlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
            var handler = serviceProvider.GetService(handlerType);
            var methodInfo = handlerType.GetMethod("Handle");

            await using var session = store.IdentitySession();

            var commandId = Guid.NewGuid();

            StoreCommand(session, command, commandId);

            ConfigureSession(session, commandId);
                
            var handle = (Task)methodInfo?.Invoke(handler, [session, command])!;
            await handle;

            await session.SaveChangesAsync();
        }

        private void ConfigureSession(IDocumentSession session, Guid commandId)
        {
            session.CausationId = commandId.ToString();

            session.CorrelationId = commandId.ToString();

            session.SetHeader("TraceIdentifier", httpContextAccessor.HttpContext?.TraceIdentifier ?? string.Empty);
        }

        private void StoreCommand(IDocumentSession session, ICommand command, Guid commandId)
        {
            LoggedCommand loggedCommand = new(
                commandId,
                httpContextAccessor.HttpContext.User.Identity?.Name,
                DateTime.UtcNow,
                command);

            session.Insert(loggedCommand);
        }
    }

    public record LoggedCommand(
        Guid CommandId,
        string? UserName,
        DateTime Timestamp,
        ICommand Command);
}
