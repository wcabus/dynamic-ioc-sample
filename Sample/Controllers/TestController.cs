using System.Net;
using System.Web.Http;
using Sample.Contracts;

namespace Sample.Controllers
{
    [RoutePrefix("api")]
    public class TestController : ApiController
    {
        private readonly IRepository _repository;

        public TestController(IRepository repository)
        {
            _repository = repository;
        }

        [Route]
        public IHttpActionResult Get()
        {
            return Ok(_repository.GetData());
        }

        [Route, HttpPost]
        public IHttpActionResult SwitchAssemblies()
        {
            Startup.ConfigureDependencies(ControllerContext.Configuration);
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
