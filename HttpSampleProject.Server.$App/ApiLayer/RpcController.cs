using RequestResponseFramework.Server.Http;

namespace HttpSampleProject.Server.ApiLayer
{
    using Microsoft.AspNetCore.Mvc;
    using RequestResponseFramework.Server;
    using RequestResponseFramework.Shared.Json;
    using System.Text.Json;

    namespace ScreeningTest.Server.ApiLayer.Controllers
    {

        [ApiController]
        [Route("[controller]")]
        public class RpcController(ILogger<RpcController> logger, IServerRequestExecutor serverRequestExecutor, IJsonSerializerOptionsProvider jsonSerializerOptionsProvider)
            : RequestControllerBase(logger, serverRequestExecutor, jsonSerializerOptionsProvider)
        {

            [HttpPost]
            public Task<ContentResult> ExecuteRpc([FromBody] JsonElement body) => Execute(body);
        }
    }

}
