using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PasswdService.Controllers
{
    public sealed class GroupsControllerTests : IClassFixture<PasswdServiceWebApplicationFactory>
    {
        private readonly PasswdServiceWebApplicationFactory factory;

        public GroupsControllerTests(PasswdServiceWebApplicationFactory factory) =>
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));

        [Fact]
        public async Task GetGroup_NotFound()
        {
            // Arrange
            var client = this.factory.CreateClient();

            // Act
            var response = await client.GetAsync("/groups/65535");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetGroup_Ok()
        {
            // Arrange
            var client = this.factory.CreateClient();

            // Act
            var response = await client.GetAsync("/groups/25");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var entity = await response.Content.ReadAsStringAsync();
            Assert.Equal("{\"name\":\"floppy\",\"gid\":25,\"members\":[\"kyle\"]}", entity);
        }

        [Fact]
        public async Task GetGroups_Ok()
        {
            // Arrange
            var client = this.factory.CreateClient();

            // Act
            var response = await client.GetAsync("/groups");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var entity = await response.Content.ReadAsStringAsync();
            Assert.Equal("[{\"name\":\"root\",\"gid\":0,\"members\":[]},{\"name\":\"daemon\",\"gid\":1,\"members\":[]},{\"name\":\"floppy\",\"gid\":25,\"members\":[\"kyle\"]},{\"name\":\"kyle\",\"gid\":1000,\"members\":[]}]", entity);
        }
    }
}
