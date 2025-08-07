using RequestResponseFramework.Server.Http;

namespace HttpSampleProject.Backend.ApiLayer
{
    using Microsoft.AspNetCore.Mvc;
    using RequestResponseFramework.Shared.Json;
    using RequestResponseFramework.Server;
    using System.Text.Json;

    namespace ScreeningTest.Backend.ApiLayer.Controllers
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
