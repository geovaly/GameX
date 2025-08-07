using RequestResponseFramework.Shared;
using RequestResponseFramework.Shared.Requests;

namespace RequestResponseFramework.Client;

public class EventHandlerClientRequestExecutor : IClientRequestExecutor
{
    public event EventHandler<Event>? EventsReceived;

    public Task<IResponse> TryExecuteAsync(IRequest request)
    {
        if (request is Event e)
        {
            EventsReceived?.Invoke(this, e);
            return Task.FromResult<IResponse>(new Ok<VoidResult>(VoidResult.Instance));
        }
        else
        {
            throw new NotSupportedException("Only events are supported");
        }
    }

    public async Task<Response<TResult>> TryExecuteAsync<TResult>(Request<TResult> request)
    {
        return (await TryExecuteAsync(request as IRequest) as Response<TResult>)!;
    }
}