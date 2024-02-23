using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.DirectoryServices.AccountManagement;

namespace ActiveDirectoryGroupsApi.Controllers
{

    public class ActiveDirectoryController : ApiController
    {
        //List all groups a user belongs to, no need to be authenticated
        [Route("api/ActiveDirectory/Users")]
        public IHttpActionResult GetGroupsFromUser(string username)
        {
            List<string> userGroups = new List<string>();

            try
            {
                using (var context = new PrincipalContext(ContextType.Domain))
                using (var user = UserPrincipal.FindByIdentity(context, username))
                {
                    if (user == null)
                    {
                        return NotFound();
                    }

                    var groups = user.GetAuthorizationGroups();
                    foreach (var group in groups)
                    {
                        userGroups.Add(group.SamAccountName);
                    }
                }

                // Return the list of groups in JSON format
                return Ok(userGroups); 
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"An error occurred: {ex.Message}"));
            }
        }

        // List of all users in a group, no need to be authenticated
        [Route("api/ActiveDirectory/Groups/{groupName}/Users")]
        public IHttpActionResult GetUsersInGroup(string groupName)
        {
            List<string> groupMembers = new List<string>();

            try
            {
                using (var context = new PrincipalContext(ContextType.Domain))
                using (var group = GroupPrincipal.FindByIdentity(context, groupName))
                {
                    if (group == null)
                    {
                        return NotFound(); // Returns a 404 Not Found if the group doesn't exist
                    }

                    foreach (var member in group.GetMembers())
                    {
                        UserPrincipal user = member as UserPrincipal;
                        if (user != null)
                        {
                            // Add the user's display name
                            groupMembers.Add(user.SamAccountName);
                        }
                    }
                }

                // Return the list of group members in JSON format
                return Ok(groupMembers);
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"An error occurred: {ex.Message}"));
            }
        }

        //Add a user to a group, must be authenticated
        [HttpPost]
        [Route("api/ActiveDirectory/Groups/{groupName}/AddUser")]
        public IHttpActionResult AddUserToGroup(string groupName, [FromBody] string username)
        {

            string domainName = "DOMAIN_NAME";
            string adUsername = "AD_USERNAME";
            string adPassword = "AD_PASSWORD";


            // Validate groupName and username to ensure they are not null or empty
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return BadRequest("Parameter 'groupName' cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest("Parameter 'username' cannot be null or empty.");
            }

            try
            {
                // Use the credentials to authenticate with the PrincipalContext
                using (var context = new PrincipalContext(ContextType.Domain, domainName, adUsername, adPassword))
                {
                    var group = GroupPrincipal.FindByIdentity(context, groupName);
                    if (group == null)
                    {
                        return NotFound(); // Group not found
                    }

                    var user = UserPrincipal.FindByIdentity(context, username);
                    if (user == null)
                    {
                        return NotFound(); // User not found
                    }

                    if (!group.Members.Contains(user))
                    {
                        group.Members.Add(user);
                        group.Save();
                        return Ok($"User {username} added to group {groupName}.");
                    }
                    else
                    {
                        return BadRequest($"User {username} is already a member of group {groupName}.");
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"An error occurred: {ex.Message}"));
            }
        }


    }
}
 