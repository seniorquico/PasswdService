using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PasswdService.Models;

namespace PasswdService.Controllers
{
    /// <summary>Provides RESTful API methods for groups defined in the <c>/etc/group</c> file.</summary>
    [ApiController]
    [Produces("application/json")]
    [Route("groups")]
    public class GroupsController : ControllerBase
    {
        /// <summary>The store used to get groups defined in the <c>/etc/group</c> file.</summary>
        private readonly IGroupStore store;

        /// <summary>Initializes a new instance of the <see cref="GroupsController"/> class.</summary>
        /// <param name="store">The store used to get groups defined in the <c>/etc/group</c> file.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="store"/> is <c>null</c>.</exception>
        public GroupsController(IGroupStore store) =>
            this.store = store ?? throw new ArgumentNullException(nameof(store));

        /// <summary>Returns an array of all groups defined in the <c>/etc/group</c> file.</summary>
        /// <response code="200">The array of all groups defined in the <c>/etc/group</c> file.</response>
        [HttpGet("", Name = nameof(GetAllGroups))]
        public ActionResult<IEnumerable<Group>> GetAllGroups() =>
            new ActionResult<IEnumerable<Group>>(this.store.GetGroups());

        /// <summary>
        ///     Returns the group defined in the <c>/etc/group</c> file matching the specified group identifier.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <response code="200">
        ///     The group defined in the <c>/etc/group</c> file matching the group identifier.
        /// </response>
        /// <response code="404">If a matching group could not be found.</response>
        [HttpGet("{groupId}", Name = nameof(GetGroupById))]
        [ProducesResponseType(404)]
        public ActionResult<Group> GetGroupById(uint groupId)
        {
            var group = this.store.GetGroup(groupId);
            if (group == null)
            {
                return this.NotFound();
            }

            return group;
        }

        /// <summary>
        ///     Returns an array of all groups defined in the <c>/etc/group</c> file matching the specified query string.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <response code="200">
        ///     The array of all groups defined in the <c>/etc/group</c> file matching the query string.
        /// </response>
        /// <response code="400">If the query string is malformed.</response>
        [HttpGet("query", Name = nameof(FindGroups))]
        [ProducesResponseType(400)]
        public ActionResult<IEnumerable<Group>> FindGroups([FromQuery] GroupQuery queryString)
        {
            var query = this.store.GetGroups();
            if (queryString.name != null)
            {
                query = query.Where(group => queryString.name.Equals(group.Name));
            }

            if (queryString.gid.HasValue)
            {
                query = query.Where(group => queryString.gid.Value == group.GroupId);
            }

            // Check if the list of members are the same, ignoring duplicates and order.
            if (queryString.members != null)
            {
                query = query.Where(group => !group.Members.Except(queryString.members).Any() && !queryString.members.Except(group.Members).Any());
            }

            return new ActionResult<IEnumerable<Group>>(query);
        }

        /// <summary>
        ///     Returns an array of all groups defined in the <c>/etc/group</c> file to which the specified user is a
        ///     member.
        /// </summary>
        /// <param name="userId">The user identifier.</param>
        /// <response code="200">
        ///     The array of all groups defined in the <c>/etc/group</c> file to which the user is a member.
        /// </response>
        /// <response code="404">If a matching user could not be found.</response>
        [HttpGet("/users/{userId}/groups", Name = nameof(GetUserGroups))]
        public ActionResult<IEnumerable<Group>> GetUserGroups(uint userId)
        {
            var groups = this.store.GetGroupsContainingUser(userId);
            if (groups == null)
            {
                // The user could not be found.
                return this.NotFound();
            }

            return new ActionResult<IEnumerable<Group>>(groups);
        }
    }
}
