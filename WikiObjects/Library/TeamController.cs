using MongoDB.Driver;
using System.Collections.Generic;
using WikiObjects.Data.ModelInterface;
using WikiObjects.Library;
using WikiObjects.Library.Exceptions;

namespace WikiObjects.Controllers
{
    public class TeamController : BaseMembership
    {
        private TeamInterface teamInterface = new TeamInterface();

        public TeamController() : base(new TeamInterface())
        {
            teamInterface = membershipInterface as TeamInterface;
        }

        public Team Create(string name, User owner)
        {
            try
            {
                return teamInterface.Create(name, owner);
            }
            catch (MongoWriteException)
            {
                throw new InvalidArugment();
            }
        }

        public long Delete(string teamId, User subject)
        {
            if (!membershipInterface.IsAdmin(teamId, subject))
            {
                throw new NotAuthorized();
            }

            return teamInterface.Delete(teamId);
        }

        public Team Get(string teamId, User subject)
        {
            if (!teamInterface.IsReader(teamId, subject))
            {
                throw new NotAuthorized();
            }

            return teamInterface.GetById(teamId);
        }

        public Team Update(string teamId, Dictionary<string, string> updates, User subject)
        {
            if (!teamInterface.IsAdmin(teamId, subject))
            {
                throw new NotAuthorized();
            }
            return teamInterface.Update(teamId, updates);
        }
    }
}
