using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PasswdService.Controllers
{
    public sealed class InvalidGroupsControllerTests : IClassFixture<InvalidPasswdServiceWebApplicationFactory>
    {
        private readonly InvalidPasswdServiceWebApplicationFactory factory;

        public InvalidGroupsControllerTests(InvalidPasswdServiceWebApplicationFactory factory) =>
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));

        [Fact]
        public async Task GetGroups_InternalServerError()
        {
            // Arrange
            var client = this.factory.CreateClient();

            // Act
            var response = await client.GetAsync("/groups/25");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);

            var entity = await response.Content.ReadAsStringAsync();
            Assert.Equal("{\"title\":\"The requested information is currently unavailable.\",\"status\":500,\"instance\":\"https://www.kyledodson.com/store-error\"}", entity);
        }
    }
}
