using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.Requests;

namespace RequestResponseFramework.Backend
{
    public interface IRequestHandler<in TRequest, TResult> where TRequest : Request<TResult> where TResult : RequestResult
    {
        Task<Response<TResult>> HandleAsync(TRequest request);
    }

    public abstract class RequestHandler<TRequest, TResult> : IRequestHandler<TRequest, TResult> where TRequest : Request<TResult> where TResult : RequestResult
    {
        public abstract Task<Response<TResult>> HandleAsync(TRequest command);

        protected Task<Response<TResult>> CreateOkTask(TResult result)
        {
            return Task.FromResult(CreateOk(result));
        }

        protected Task<Response<TResult>> CreateNotOkTask(RequestException exception)
        {
            return Task.FromResult(CreateNotOk(exception));
        }

        protected Response<VoidResult> CreateOkVoid()
        {
            return new Ok<VoidResult>(VoidResult.Instance);
        }
        protected Response<TResult> CreateOk(TResult result)
        {
            return new Ok<TResult>(result);
        }

        protected Response<TResult> CreateNotOk(RequestException exception)
        {
            return new NotOk<TResult>(exception);
        }
    }


    public abstract class CommandHandler<TCommand, TResult> : RequestHandler<TCommand, TResult> where TCommand : Command<TResult> where TResult : RequestResult
    {

    }

    public abstract class QueryHandler<TQuery, TResult> : RequestHandler<TQuery, TResult> where TQuery : Query<TResult> where TResult : RequestResult
    {
    }

    public abstract class EventHandler<TEvent> : RequestHandler<TEvent, VoidResult> where TEvent : Event
    {
    }


}
