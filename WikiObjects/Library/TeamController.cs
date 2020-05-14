using WikiObjects.Data.ModelInterface;
using WikiObjects.Library;

namespace WikiObjects.Controllers
{
    class TeamController
    {
        public Team Create(string name, User owner)
        {
            return TeamInterface.Create(name, owner);
        }

        public bool AddAdmin(string teamId, string newAdminId, User subject)
        {
            if (!TeamInterface.IsAdmin(teamId, subject))
            {
                throw new NotAuthorized();
            }

            return TeamInterface.AddMember(teamId, UserInterface.GetById(newAdminId), MemberList.admins);
        }

        public bool AddReader(string teamId, string newAdminId, User subject)
        {
            if (!TeamInterface.IsAdmin(teamId, subject))
            {
                throw new NotAuthorized();
            }

            return TeamInterface.AddMember(teamId, UserInterface.GetById(newAdminId), MemberList.readers);
        }

        public bool RemoveMember(string teamId, string unMemberId, User subject)
        {
            if (!TeamInterface.IsAdmin(teamId, subject))
            {
                throw new NotAuthorized();
            }

            return TeamInterface.RemoveMember(teamId, unMemberId)
        }
    }
}
