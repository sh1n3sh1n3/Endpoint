using Endpoint.Application.Builders;
using Endpoint.Application.Enums;
using Endpoint.Application.ValueObjects;
using System.Collections.Generic;
using Xunit;

namespace Endpoint.UnitTests
{
    public class MethodBuilderTests
    {
        [Fact]
        public async void Constructor()
        {
            Setup();

            var sut = CreateMethodBuilder();
        }

        [Fact]
        public void BasicSignature()
        {
            var expected = "        public void Get()";

            var actual = new MethodSignatureBuilder()
                .WithEndpointType(EndpointType.Get)
                .WithIndent(2)
                .Build();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BasicAsyncSignature()
        {
            var expected = "        public async Task Get()";

            var actual = new MethodSignatureBuilder()
                .WithEndpointType(EndpointType.Get)
                .WithAsync(true)
                .WithReturnType("Task")
                .WithIndent(2)
                .Build();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void BasicAsyncSignatureWithParams()
        {
            var expected = "public async Task Create([FromBody]CreateCustomer.Request request)";

            var actual = new MethodSignatureBuilder()
                .WithEndpointType(EndpointType.Create)
                .WithParameter("[FromBody]CreateCustomer.Request request")
                .WithAsync(true)
                .WithReturnType("Task")
                .WithIndent(2)
                .Build();

            Assert.Equal(expected, actual);
        }
        [Fact]
        public void Signature()
        {
            var expected = "        public async Task<ActionResult<GetCustomerById.Response>> GetById([FromRoute]GetCustomerById.Request request)";

            var actual = new MethodSignatureBuilder()
                .WithEndpointType(EndpointType.GetById)
                .WithParameter(new ParameterBuilder("GetCustomerById.Request", "request").WithFrom(From.Route).Build())
                .WithAsync(true)
                .WithReturnType(TypeBuilder.WithActionResult("GetCustomerById.Response"))
                .WithIndent(2)
                .Build();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void ApiEndpoint()
        {
            var expected = new string[]
            {
                "[Authorize]",
                "[HttpGet(\"{customerId}\", Name = \"GetCustomerByIdRoute\")]",
                "[ProducesResponseType((int)HttpStatusCode.InternalServerError)]",
                "[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]",
                "[ProducesResponseType(typeof(GetCustomerById.Response), (int)HttpStatusCode.OK)]",
                "[ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]",
                "public async Task<ActionResult<GetCustomerById.Response>> GetById([FromRoute]GetCustomerById.Request request)",
                "{",
                "    var response = await _mediator.Send(request);",
                "    ",
                "        if (response.Customer == null)",
                "        {",
                "            return new NotFoundObjectResult(request.CustomerId);",
                "        }",
                "    ",
                "    return response;",
                "}"
            };
            Setup();

            var actual = new MethodBuilder()
                .WithEndpointType(EndpointType.GetById)
                .WithResource("Customer")
                .WithAuthorize(true)
                .WithIndent(2)
                .Build();

            Assert.Equal(expected, actual);

        }

        [Fact]
        public void GetApiEndpoint()
        {
            var expected = new string[]
            {
                "[HttpGet(Name = \"GetCustomersRoute\")]",
                "[ProducesResponseType((int)HttpStatusCode.InternalServerError)]",
                "[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]",
                "[ProducesResponseType(typeof(GetCustomers.Response), (int)HttpStatusCode.OK)]",
                "public async Task<ActionResult<GetCustomers.Response>> Get()",
                "    => await _mediator.Send(new GetCustomers.Request());",
            };
            Setup();

            var actual = new MethodBuilder()
                .WithEndpointType(EndpointType.Get)
                .WithResource("Customer")
                .WithAuthorize(false)
                .Build();

            Assert.Equal(expected, actual);

        }

        [Fact]
        public void CreateApiEndpoint()
        {
            var expected = new string[]
            {
                "[HttpPost(Name = \"CreateCustomerRoute\")]",
                "[ProducesResponseType((int)HttpStatusCode.InternalServerError)]",
                "[ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]",
                "[ProducesResponseType(typeof(CreateCustomer.Response), (int)HttpStatusCode.OK)]",
                "public async Task<ActionResult<CreateCustomer.Response>> Create()",
                "    => await _mediator.Send(new CreateCustomer.Request());",
            };
            Setup();

            var actual = new MethodBuilder()
                .WithEndpointType(EndpointType.Create)
                .WithResource("Customer")
                .WithAuthorize(false)
                .Build();

            Assert.Equal(expected, actual);

        }

        [Fact]
        public void ToDtoExtensionMethod()
        {
            string[] expected = new List<string> {
                "public static CustomerDto ToDto(this Customer customer)",
                "{",
                "    return new ()",
                "    {",
                "        CustomerId = customer.CustomerId",
                "    };",
                "}"
            }.ToArray();

            var actual = new MethodBuilder()                
                .IsStatic()
                .WithName("ToDto")
                .WithReturnType("CustomerDto")
                .WithPropertyName("CustomerId")
                .WithParameter(new ParameterBuilder("Customer","customer", true).Build())
                .WithBody(new() {
                    "    return new ()",
                    "    {",
                    "        CustomerId = customer.CustomerId",
                    "    };"
                })
                .Build();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Configure()
        {
            var expected = new List<string>
            {
                "public void Configure(EntityTypeBuilder<Foo> builder)",
                "{",
                "    ",
                "}"
            }.ToArray();

            var actual = new MethodBuilder()
                    .WithName("Configure")
                    .WithReturnType("void")
                    .WithBody(new() { "" })
                    .WithParameter(new ParameterBuilder(new TypeBuilder().WithGenericType("EntityTypeBuilder", ((Token)"Foo").PascalCase)
                    .Build(), "builder").Build()).Build();

            Assert.Equal(expected, actual);
        }
        private void Setup()
        {

        }

        private MethodBuilder CreateMethodBuilder()
        {
            return new();
        }
    }
}
