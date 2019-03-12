using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PasswdService.Controllers
{
    public sealed class UsersControllerTests : IClassFixture<PasswdServiceWebApplicationFactory>
    {
        private readonly PasswdServiceWebApplicationFactory factory;

        public UsersControllerTests(PasswdServiceWebApplicationFactory factory) =>
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));

        [Fact]
        public async Task GetUser_NotFound()
        {
            // Arrange
            var client = this.factory.CreateClient();

            // Act
            var response = await client.GetAsync("/users/65535");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetUser_Ok()
        {
            // Arrange
            var client = this.factory.CreateClient();

            // Act
            var response = await client.GetAsync("/users/1000");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var entity = await response.Content.ReadAsStringAsync();
            Assert.Equal("{\"name\":\"kyle\",\"uid\":1000,\"gid\":1000,\"comment\":\",,,\",\"home\":\"/home/kyle\",\"shell\":\"/bin/bash\"}", entity);
        }

        [Fact]
        public async Task GetUsers_Ok()
        {
            // Arrange
            var client = this.factory.CreateClient();

            // Act
            var response = await client.GetAsync("/users");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var entity = await response.Content.ReadAsStringAsync();
            Assert.Equal("[{\"name\":\"root\",\"uid\":0,\"gid\":0,\"comment\":\"root\",\"home\":\"/root\",\"shell\":\"/bin/bash\"},{\"name\":\"kyle\",\"uid\":1000,\"gid\":1000,\"comment\":\",,,\",\"home\":\"/home/kyle\",\"shell\":\"/bin/bash\"}]", entity);
        }
    }
}
