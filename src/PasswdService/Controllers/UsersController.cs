using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PasswdService.Models;

namespace PasswdService.Controllers
{
    /// <summary>Provides RESTful API methods for users defined in the <c>/etc/passwd</c> file.</summary>
    [ApiController]
    [Produces("application/json")]
    [Route("users")]
    public class UsersController : ControllerBase
    {
        /// <summary>The store used to get users defined in the <c>/etc/passwd</c> file.</summary>
        private readonly IUserStore store;

        /// <summary>Initializes a new instance of the <see cref="UsersController"/> class.</summary>
        /// <param name="store">The store used to get users defined in the <c>/etc/passwd</c> file.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="store"/> is <c>null</c>.</exception>
        public UsersController(IUserStore store) =>
            this.store = store ?? throw new ArgumentNullException(nameof(store));

        /// <summary>Returns an array of all users defined in the <c>/etc/passwd</c> file.</summary>
        /// <response code="200">The array of all users defined in the <c>/etc/passwd</c> file.</response>
        [HttpGet("", Name = nameof(GetAllUsers))]
        public ActionResult<IEnumerable<User>> GetAllUsers() =>
            new ActionResult<IEnumerable<User>>(this.store.GetUsers());

        /// <summary>
        ///     Returns the user defined in the <c>/etc/passwd</c> file matching the specified user identifier.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <response code="200">
        ///     The user defined in the <c>/etc/passwd</c> file matching the user identifier.
        /// </response>
        /// <response code="404">If a matching user could not be found.</response>
        [HttpGet("{userId}", Name = nameof(GetUserById))]
        [ProducesResponseType(404)]
        public ActionResult<User> GetUserById(uint userId)
        {
            var user = this.store.GetUser(userId);
            if (user == null)
            {
                return this.NotFound();
            }

            return user;
        }

        /// <summary>
        ///     Returns an array of all users defined in the <c>/etc/passwd</c> file matching the specified query string.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <response code="200">
        ///     The array of all users defined in the <c>/etc/passwd</c> file matching the query string.
        /// </response>
        /// <response code="400">If the query string is malformed.</response>
        [HttpGet("query", Name = nameof(FindUsers))]
        [ProducesResponseType(400)]
        public ActionResult<IEnumerable<User>> FindUsers([FromQuery] UserQuery queryString)
        {
            var query = this.store.GetUsers();
            if (queryString.name != null)
            {
                query = query.Where(user => queryString.name.Equals(user.Name));
            }

            if (queryString.uid.HasValue)
            {
                query = query.Where(user => queryString.uid.Value == user.UserId);
            }

            if (queryString.gid.HasValue)
            {
                query = query.Where(user => queryString.gid.Value == user.GroupId);
            }

            if (queryString.comment != null)
            {
                query = query.Where(user => queryString.comment.Equals(user.Comment));
            }

            if (queryString.home != null)
            {
                query = query.Where(user => queryString.home.Equals(user.Home));
            }

            if (queryString.shell != null)
            {
                query = query.Where(user => queryString.shell.Equals(user.Shell));
            }

            return new ActionResult<IEnumerable<User>>(query);
        }
    }
}
