

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;


namespace AutoMoqCore.Tests.Scenario
{
    
    public class Situation_where_the_wrong_http_context_base_is_passed_inTests
    {

        [Fact]
        public void Reproduce_the_issue()
        {

            var claim = new List<Claim>
            {
                new Claim("foo", "bar")
            };
            var auto = new AutoMoqer();
            var httpRequestBase = auto.GetMock<HttpRequest>().Object;

            auto.GetMock<HttpContext>()
                .Setup(c=>c.Request)
                .Returns(httpRequestBase);

            auto.GetMock<HttpRequest>()
                .SetupGet(x => x.HttpContext.User.Claims).Returns(claim);
              

            var svc = auto.Create<ContextService>();
            var hascookie = svc.HasCookie("foo");
            hascookie.Should().BeTrue();
        }

        public class ContextService
        {
            private readonly HttpContext _contextBase;

            public ContextService(HttpContext contextBase)
            {
                _contextBase = contextBase;
            }

            public bool HasCookie(string cookie)
            {
                var c = _contextBase.Request.HttpContext.User.Claims.First(d=>d.Type == cookie);
                if (c == null)
                    return false;
                else
                    return true;
            }
        }
    }
}